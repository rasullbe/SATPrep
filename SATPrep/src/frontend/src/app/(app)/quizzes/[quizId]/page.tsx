"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import {
  getQuizTakeData,
  startQuizAttempt,
  completeQuizAttempt,
  type QuizTakeDto,
  type QuizAttemptGetDto,
  type ChoiceGetDto,
} from "@/lib/api";
import {
  LoadingSpinner,
  PageError,
  FormulaSheetModal,
  CalculatorModal,
  DirectionsModal,
} from "@/components/ui";

type Phase = "intro" | "active" | "review" | "submitting";

function formatTime(seconds: number): string {
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, "0")}`;
}

interface ParsedQuestion {
  passage: string | null;
  prompt: string;
  choices: { choiceId: number; text: string; letter: string }[];
  isStudentProduced: boolean;
}

function parseQuestion(rawText: string, rawChoices: ChoiceGetDto[]): ParsedQuestion {
  let cleanedText = (rawText || "").trim();

  // 1. Check if choices are embedded in text: e.g. "(A) 5 (B) 9.67 (C) 15 (D) 29"
  const choiceMatch = cleanedText.match(
    /\(A\)\s*([\s\S]*?)\s*\(B\)\s*([\s\S]*?)\s*\(C\)\s*([\s\S]*?)\s*\(D\)\s*([\s\S]*)$/i
  );

  let extractedChoices: { text: string; letter: string }[] = [];
  if (choiceMatch) {
    cleanedText = cleanedText.replace(choiceMatch[0], "").trim();
    extractedChoices = [
      { letter: "A", text: choiceMatch[1].trim() },
      { letter: "B", text: choiceMatch[2].trim() },
      { letter: "C", text: choiceMatch[3].trim() },
      { letter: "D", text: choiceMatch[4].trim() },
    ];
  }

  // 2. Resolve choices
  let finalChoices: { choiceId: number; text: string; letter: string }[] = [];

  if (rawChoices && rawChoices.length > 0) {
    finalChoices = rawChoices.map((c, idx) => {
      let t = c.text.trim();
      t = t.replace(/^\([A-D]\)\s*/i, "").replace(/^[A-D][\)\.]\s*/i, "");
      return {
        choiceId: c.choiceId,
        text: t,
        letter: ["A", "B", "C", "D", "E"][idx] || `${idx + 1}`,
      };
    });
  } else if (extractedChoices.length > 0) {
    finalChoices = extractedChoices.map((ec, idx) => ({
      choiceId: -(idx + 1), // Virtual ID if synthesized
      text: ec.text,
      letter: ec.letter,
    }));
  }

  // 3. Separate passage vs prompt
  let passage: string | null = null;
  let prompt: string = cleanedText;

  if (cleanedText.includes("\n\n")) {
    const parts = cleanedText.split("\n\n");
    passage = parts.slice(0, parts.length - 1).join("\n\n").trim();
    prompt = parts[parts.length - 1].trim();
  } else if (cleanedText.includes("\n")) {
    const lines = cleanedText.split("\n").filter((l) => l.trim().length > 0);
    if (lines.length > 1) {
      passage = lines.slice(0, lines.length - 1).join("\n").trim();
      prompt = lines[lines.length - 1].trim();
    }
  }

  return {
    passage,
    prompt: prompt || cleanedText,
    choices: finalChoices,
    isStudentProduced: finalChoices.length === 0,
  };
}

export default function BluebookExamPage() {
  const { quizId } = useParams<{ quizId: string }>();
  const router = useRouter();

  const [quiz, setQuiz] = useState<QuizTakeDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [phase, setPhase] = useState<Phase>("intro");
  const [attempt, setAttempt] = useState<QuizAttemptGetDto | null>(null);

  // Answers map: questionId -> selectedChoiceId (or numeric choice ID)
  const [answers, setAnswers] = useState<Map<number, number>>(new Map());
  // Free text answers for Student-Produced Response (Grid-in) questions
  const [textAnswers, setTextAnswers] = useState<Map<number, string>>(new Map());

  const [flagged, setFlagged] = useState<Set<number>>(new Set());
  const [eliminatedChoices, setEliminatedChoices] = useState<Map<number, Set<number>>>(new Map());
  const [currentIndex, setCurrentIndex] = useState(0);

  // Bluebook Exam Tools State
  const [timeRemaining, setTimeRemaining] = useState<number | null>(null);
  const [isTimerHidden, setIsTimerHidden] = useState(false);
  const [isNavMenuOpen, setIsNavMenuOpen] = useState(false);
  const [isFormulaSheetOpen, setIsFormulaSheetOpen] = useState(false);
  const [isCalculatorOpen, setIsCalculatorOpen] = useState(false);
  const [isDirectionsOpen, setIsDirectionsOpen] = useState(false);
  const [isEliminatorMode, setIsEliminatorMode] = useState(false);

  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const startTimeRef = useRef<number>(0);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const loadQuiz = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getQuizTakeData(Number(quizId));
      setQuiz(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load test.");
    } finally {
      setLoading(false);
    }
  }, [quizId]);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const data = await getQuizTakeData(Number(quizId));
        if (!cancelled) setQuiz(data);
      } catch (err) {
        if (!cancelled) setError(err instanceof Error ? err.message : "Failed to load test.");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [quizId]);

  const submitTest = useCallback(async () => {
    if (!attempt || submitting) return;
    setSubmitting(true);
    setSubmitError(null);

    const elapsedSeconds = Math.floor((Date.now() - startTimeRef.current) / 1000);
    const answersArray = Array.from(answers.entries()).map(([questionId, selectedChoiceId]) => ({
      questionId,
      selectedChoiceId: selectedChoiceId > 0 ? selectedChoiceId : null,
    }));

    try {
      const result = await completeQuizAttempt(attempt.quizAttemptId, answersArray, elapsedSeconds);
      if (timerRef.current) clearInterval(timerRef.current);
      router.push(`/results/${result.quizAttemptId}`);
    } catch (err) {
      setSubmitError(err instanceof Error ? err.message : "Failed to submit test. Try again.");
      setSubmitting(false);
    }
  }, [attempt, answers, submitting, router]);

  useEffect(() => {
    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, []);

  const handleStart = useCallback(async () => {
    if (!quiz) return;
    try {
      const a = await startQuizAttempt(quiz.quizId);
      setAttempt(a);
      startTimeRef.current = Date.now();
      setPhase("active");
      setCurrentIndex(0);

      if (quiz.timeLimitMinutes != null) {
        const totalSeconds = quiz.timeLimitMinutes * 60;
        setTimeRemaining(totalSeconds);

        timerRef.current = setInterval(() => {
          setTimeRemaining((prev) => {
            if (prev == null || prev <= 1) {
              if (timerRef.current) clearInterval(timerRef.current);
              return 0;
            }
            return prev - 1;
          });
        }, 1000);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not start exam attempt.");
    }
  }, [quiz]);

  // Auto-submit when time expires
  useEffect(() => {
    if (timeRemaining === 0 && (phase === "active" || phase === "review") && !submitting) {
      const timer = setTimeout(() => {
        void submitTest();
      }, 0);
      return () => clearTimeout(timer);
    }
  }, [timeRemaining, phase, submitting, submitTest]);

  const selectChoice = (questionId: number, choiceId: number) => {
    setAnswers((prev) => {
      const next = new Map(prev);
      next.set(questionId, choiceId);
      return next;
    });
  };

  const setFreeResponseAnswer = (questionId: number, val: string) => {
    setTextAnswers((prev) => {
      const next = new Map(prev);
      next.set(questionId, val);
      return next;
    });
    if (val.trim()) {
      setAnswers((prev) => {
        const next = new Map(prev);
        next.set(questionId, 1); // Mark as answered
        return next;
      });
    } else {
      setAnswers((prev) => {
        const next = new Map(prev);
        next.delete(questionId);
        return next;
      });
    }
  };

  const toggleFlag = (questionId: number) => {
    setFlagged((prev) => {
      const next = new Set(prev);
      if (next.has(questionId)) {
        next.delete(questionId);
      } else {
        next.add(questionId);
      }
      return next;
    });
  };

  const toggleEliminate = (questionId: number, choiceId: number) => {
    setEliminatedChoices((prev) => {
      const next = new Map(prev);
      const currentSet = new Set(next.get(questionId) || []);
      if (currentSet.has(choiceId)) {
        currentSet.delete(choiceId);
      } else {
        currentSet.add(choiceId);
        if (answers.get(questionId) === choiceId) {
          const updatedAnswers = new Map(answers);
          updatedAnswers.delete(questionId);
          setAnswers(updatedAnswers);
        }
      }
      next.set(questionId, currentSet);
      return next;
    });
  };

  if (loading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center bg-[#001726] text-white">
        <LoadingSpinner />
        <p className="mt-4 text-sm font-medium tracking-wide text-slate-300">Initializing Digital SAT Environment…</p>
      </div>
    );
  }

  if (error || !quiz) {
    return (
      <div className="flex h-screen items-center justify-center p-6">
        <PageError message={error || "Unable to load test"} onRetry={loadQuiz} />
      </div>
    );
  }

  const questions = quiz?.questions || [];
  const answeredCount = answers.size;
  const currentQuestion = questions[currentIndex];

  const parsed = useMemo(() => {
    return parseQuestion(currentQuestion?.text || "", currentQuestion?.choices || []);
  }, [currentQuestion]);

  const isCurrentAnswered = answers.has(currentQuestion?.questionId);
  const isQuestionFlagged = flagged.has(currentQuestion?.questionId);
  const currentEliminated = eliminatedChoices.get(currentQuestion?.questionId) || new Set<number>();

  // Digital SAT Pro Keyboard Shortcuts Engine
  useEffect(() => {
    if (phase !== "active" || !currentQuestion) return;

    const handleExamKeyDown = (e: KeyboardEvent) => {
      // Don't intercept if student is typing in an input, textarea, or if modals are open
      const target = e.target as HTMLElement;
      if (
        target?.tagName === "INPUT" ||
        target?.tagName === "TEXTAREA" ||
        isCalculatorOpen ||
        isFormulaSheetOpen ||
        isDirectionsOpen
      ) {
        return;
      }

      const key = e.key.toLowerCase();

      // 1. Multiple Choice letter shortcuts: 'a', 'b', 'c', 'd' or '1', '2', '3', '4'
      if (!parsed.isStudentProduced && parsed.choices.length > 0) {
        let choiceIndex = -1;
        if (key === "a" || key === "1") choiceIndex = 0;
        else if (key === "b" || key === "2") choiceIndex = 1;
        else if (key === "c" || key === "3") choiceIndex = 2;
        else if (key === "d" || key === "4") choiceIndex = 3;

        if (choiceIndex >= 0 && choiceIndex < parsed.choices.length) {
          e.preventDefault();
          const targetChoice = parsed.choices[choiceIndex];
          if (!currentEliminated.has(targetChoice.choiceId)) {
            selectChoice(currentQuestion.questionId, targetChoice.choiceId);
          }
          return;
        }
      }

      // 2. Navigation: ArrowLeft / 'j' for Previous, ArrowRight / 'k' for Next
      if (e.key === "ArrowLeft" || key === "j") {
        e.preventDefault();
        setCurrentIndex((i) => Math.max(0, i - 1));
      } else if (e.key === "ArrowRight" || key === "k") {
        e.preventDefault();
        setCurrentIndex((i) => Math.min(questions.length - 1, i + 1));
      }

      // 3. Mark for Review: 'm'
      else if (key === "m") {
        e.preventDefault();
        toggleFlag(currentQuestion.questionId);
      }

      // 4. Cross-out toggle: 'x'
      else if (key === "x" && !parsed.isStudentProduced) {
        e.preventDefault();
        setIsEliminatorMode((prev) => !prev);
      }
    };

    window.addEventListener("keydown", handleExamKeyDown);
    return () => window.removeEventListener("keydown", handleExamKeyDown);
  }, [
    phase,
    parsed,
    currentQuestion,
    currentEliminated,
    questions.length,
    isCalculatorOpen,
    isFormulaSheetOpen,
    isDirectionsOpen,
  ]);

  /* ------------------- INTRO PHASE ------------------- */
  if (phase === "intro") {
    return (
      <div className="min-h-screen bg-[#f8fafc] py-12 px-4 sm:px-6">
        <div className="mx-auto max-w-2xl">
          <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-lg">
            <div className="bg-[#002b49] px-8 py-6 text-white">
              <div className="flex items-center justify-between">
                <span className="rounded-md bg-[#0077c8] px-2.5 py-1 text-xs font-bold tracking-wider uppercase">
                  College Board
                </span>
                <span className="text-xs font-medium text-sky-200">Official Bluebook Format</span>
              </div>
              <h1 className="mt-3 text-2xl font-bold tracking-tight text-white sm:text-3xl">
                {quiz.title}
              </h1>
              {quiz.description && (
                <p className="mt-2 text-sm text-slate-300 leading-relaxed">{quiz.description}</p>
              )}
            </div>

            <div className="space-y-6 p-8">
              <div className="grid grid-cols-2 gap-4 border-b border-slate-100 pb-6 text-sm sm:grid-cols-3">
                <div className="rounded-xl bg-slate-50 p-4 border border-slate-200/80">
                  <p className="text-xs font-semibold uppercase text-slate-500">Total Questions</p>
                  <p className="mt-1 text-2xl font-bold text-[#002b49]">{quiz.questionCount}</p>
                </div>
                <div className="rounded-xl bg-slate-50 p-4 border border-slate-200/80">
                  <p className="text-xs font-semibold uppercase text-slate-500">Time Limit</p>
                  <p className="mt-1 text-2xl font-bold text-[#002b49]">
                    {quiz.timeLimitMinutes != null ? `${quiz.timeLimitMinutes} min` : "Untimed"}
                  </p>
                </div>
                <div className="col-span-2 sm:col-span-1 rounded-xl bg-slate-50 p-4 border border-slate-200/80">
                  <p className="text-xs font-semibold uppercase text-slate-500">Calculator & Tools</p>
                  <p className="mt-1 text-sm font-semibold text-[#0077c8]">Active on-screen</p>
                </div>
              </div>

              <div>
                <h2 className="text-base font-bold text-slate-900">Module Instructions</h2>
                <ul className="mt-3 space-y-2.5 text-sm text-slate-600">
                  <li className="flex items-start gap-2.5">
                    <span className="flex h-5 w-5 shrink-0 items-center justify-center rounded-full bg-blue-100 text-xs font-bold text-[#0077c8]">
                      ✓
                    </span>
                    <span>Timing is automatic. A countdown clock will be displayed at the top of your screen.</span>
                  </li>
                  <li className="flex items-start gap-2.5">
                    <span className="flex h-5 w-5 shrink-0 items-center justify-center rounded-full bg-blue-100 text-xs font-bold text-[#0077c8]">
                      ✓
                    </span>
                    <span>
                      Flag questions using the <strong>Mark for Review</strong> button to return to them before submitting.
                    </span>
                  </li>
                  <li className="flex items-start gap-2.5">
                    <span className="flex h-5 w-5 shrink-0 items-center justify-center rounded-full bg-blue-100 text-xs font-bold text-[#0077c8]">
                      ✓
                    </span>
                    <span>
                      Access the on-screen <strong>Reference Sheet</strong> for Math formulas and the built-in <strong>Calculator</strong>.
                    </span>
                  </li>
                  <li className="flex items-start gap-2.5">
                    <span className="flex h-5 w-5 shrink-0 items-center justify-center rounded-full bg-blue-100 text-xs font-bold text-[#0077c8]">
                      ✓
                    </span>
                    <span>Use the <strong>Cross-out</strong> tool to strike through eliminated options.</span>
                  </li>
                </ul>
              </div>

              <div className="pt-4">
                <button
                  onClick={handleStart}
                  className="w-full rounded-full bg-[#0077c8] py-3.5 text-base font-bold text-white shadow-md transition-all hover:bg-[#005a9c] hover:shadow-lg active:scale-[0.99]"
                >
                  Start Practice Module
                </button>
                <div className="mt-3 text-center">
                  <button
                    onClick={() => router.push("/quizzes")}
                    className="text-xs font-medium text-slate-500 hover:text-slate-800"
                  >
                    ← Return to Practice Tests catalog
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  /* ------------------- REVIEW PHASE (PRE-SUBMISSION) ------------------- */
  if (phase === "review") {
    const unansweredCount = questions.length - answeredCount;
    return (
      <div className="flex min-h-screen flex-col bg-[#f8fafc]">
        <header className="bluebook-header sticky top-0 z-20 flex h-14 items-center justify-between px-6">
          <div className="flex items-center gap-3">
            <span className="font-bold text-[#002b49]">{quiz.title}</span>
            <span className="rounded-md bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-600">
              Review Screen
            </span>
          </div>
          {timeRemaining != null && (
            <div className="font-mono text-sm font-bold text-slate-800">
              {isTimerHidden ? "--:--" : formatTime(timeRemaining)}
            </div>
          )}
        </header>

        <main className="flex-1 mx-auto w-full max-w-4xl p-6 sm:p-10">
          <div className="rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
            <h1 className="text-2xl font-bold text-[#002b49]">Check Your Work</h1>
            <p className="mt-2 text-sm text-slate-600">
              On test day, you won’t be able to return to this module once you submit. Select any question number below to review or change your answer.
            </p>

            {unansweredCount > 0 ? (
              <div className="mt-4 flex items-center gap-3 rounded-lg border border-amber-300 bg-amber-50 p-4 text-sm text-amber-900">
                <span className="text-xl">⚠️</span>
                <div>
                  <p className="font-bold">You have {unansweredCount} unanswered question{unansweredCount > 1 ? "s" : ""}.</p>
                  <p className="text-xs text-amber-800">There is no penalty for guessing on the SAT. We strongly recommend answering all questions.</p>
                </div>
              </div>
            ) : (
              <div className="mt-4 flex items-center gap-2 rounded-lg border border-emerald-200 bg-emerald-50 p-3 text-xs font-medium text-emerald-800">
                <span>✓ All {questions.length} questions have been answered.</span>
              </div>
            )}

            <div className="mt-8">
              <div className="flex items-center justify-between pb-3 border-b border-slate-200">
                <h3 className="text-sm font-bold text-slate-800">Questions in this Module</h3>
                <div className="flex items-center gap-4 text-xs text-slate-500">
                  <span className="flex items-center gap-1.5">
                    <span className="h-3 w-3 rounded-xs bg-[#002b49]" /> Answered
                  </span>
                  <span className="flex items-center gap-1.5">
                    <span className="h-3 w-3 rounded-xs border border-dashed border-slate-400 bg-white" /> Unanswered
                  </span>
                  <span className="flex items-center gap-1.5">
                    <span className="text-amber-500">🚩</span> For Review
                  </span>
                </div>
              </div>

              <div className="mt-4 grid grid-cols-4 gap-3 sm:grid-cols-6 md:grid-cols-8">
                {questions.map((q, idx) => {
                  const answered = answers.has(q.questionId);
                  const isFlag = flagged.has(q.questionId);
                  return (
                    <button
                      key={q.questionId}
                      onClick={() => {
                        setCurrentIndex(idx);
                        setPhase("active");
                      }}
                      className={`relative flex h-14 flex-col items-center justify-center rounded-lg border transition ${
                        answered
                          ? "border-[#002b49] bg-[#002b49] text-white hover:bg-[#0a3d66]"
                          : "border-dashed border-slate-400 bg-white text-slate-800 hover:border-[#0077c8] hover:bg-sky-50"
                      }`}
                    >
                      <span className="text-base font-bold">{idx + 1}</span>
                      {isFlag && (
                        <span className="absolute -top-1.5 -right-1.5 flex h-4 w-4 items-center justify-center rounded-full bg-amber-400 text-[10px] text-white shadow-xs">
                          🚩
                        </span>
                      )}
                    </button>
                  );
                })}
              </div>
            </div>

            {submitError && (
              <p className="mt-6 text-center text-sm font-semibold text-red-600">{submitError}</p>
            )}

            <div className="mt-10 flex items-center justify-between border-t border-slate-200 pt-6">
              <button
                onClick={() => setPhase("active")}
                className="btn-bluebook-secondary"
              >
                ← Return to Questions
              </button>

              <button
                onClick={submitTest}
                disabled={submitting}
                className="rounded-full bg-[#15803d] px-8 py-2.5 text-sm font-bold text-white shadow-md transition hover:bg-[#166534] disabled:opacity-50"
              >
                {submitting ? "Submitting Answers…" : "Submit Final Answers"}
              </button>
            </div>
          </div>
        </main>
      </div>
    );
  }

  /* ------------------- ACTIVE EXAM PHASE ------------------- */
  return (
    <div className="flex h-screen flex-col overflow-hidden bg-[#f8fafc] text-slate-900 select-none">
      {/* 1. BLUEBOOK TOP NAVIGATION BAR */}
      <header className="bluebook-header flex h-14 shrink-0 items-center justify-between px-4 sm:px-6">
        <div className="flex items-center gap-3">
          <span className="font-bold text-[#002b49] text-sm sm:text-base tracking-tight truncate max-w-[200px] sm:max-w-md">
            {quiz.title}
          </span>
          <button
            onClick={() => setIsDirectionsOpen(true)}
            className="hidden sm:inline-flex items-center gap-1 rounded-md border border-slate-300 bg-white px-2.5 py-1 text-xs font-semibold text-slate-700 hover:bg-slate-50 transition"
          >
            <span>Directions</span>
            <span className="text-[10px] text-slate-400">▾</span>
          </button>
        </div>

        {/* Center: Hideable Countdown Timer */}
        {timeRemaining != null && (
          <div className="flex items-center gap-2">
            <div className="flex items-center gap-2 rounded-full border border-slate-200 bg-white px-3.5 py-1 shadow-xs">
              <span
                className={`font-mono text-sm font-bold ${
                  timeRemaining <= 180 ? "text-red-600 animate-pulse" : "text-slate-800"
                }`}
              >
                {isTimerHidden ? "--:--" : formatTime(timeRemaining)}
              </span>
              <button
                onClick={() => setIsTimerHidden((prev) => !prev)}
                className="text-[11px] font-semibold text-[#0077c8] hover:underline"
              >
                {isTimerHidden ? "Show" : "Hide"}
              </button>
            </div>
          </div>
        )}

        {/* Right: Bluebook Utilities */}
        <div className="flex items-center gap-1 sm:gap-2">
          {!parsed.isStudentProduced && (
            <button
              onClick={() => setIsEliminatorMode((prev) => !prev)}
              title="Cross out answer choices"
              className={`flex items-center gap-1 rounded-md px-2.5 py-1 text-xs font-semibold transition ${
                isEliminatorMode
                  ? "bg-amber-100 text-amber-900 border border-amber-300"
                  : "text-slate-600 hover:bg-slate-100 border border-transparent"
              }`}
            >
              <span className="text-sm">✂️</span>
              <span className="hidden md:inline">Cross-out</span>
            </button>
          )}

          <button
            onClick={() => setIsCalculatorOpen(true)}
            title="Calculator"
            className="flex items-center gap-1 rounded-md px-2.5 py-1 text-xs font-semibold text-slate-600 hover:bg-slate-100 transition"
          >
            <span className="text-sm">🧮</span>
            <span className="hidden md:inline">Calculator</span>
          </button>

          <button
            onClick={() => setIsFormulaSheetOpen(true)}
            title="Math Reference Sheet"
            className="flex items-center gap-1 rounded-md px-2.5 py-1 text-xs font-semibold text-slate-600 hover:bg-slate-100 transition"
          >
            <span className="text-sm">📐</span>
            <span className="hidden md:inline">Reference</span>
          </button>
        </div>
      </header>

      {/* 2. MAIN SPLIT-PANE EXAMINATION CANVAS */}
      <main className="flex-1 overflow-y-auto p-4 sm:p-6">
        <div className="mx-auto h-full max-w-6xl">
          <div className="grid h-full grid-cols-1 gap-6 lg:grid-cols-2">
            {/* LEFT COLUMN: Stimulus / Passage / Problem Context */}
            <div className="flex flex-col rounded-xl border border-slate-200 bg-white p-6 shadow-sm overflow-y-auto">
              <div className="mb-3 flex items-center justify-between border-b border-slate-100 pb-2">
                <span className="text-xs font-bold uppercase tracking-wider text-slate-400">
                  {parsed.passage ? "Passage / Stimulus" : "Problem Statement"}
                </span>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-[11px] font-medium text-slate-600">
                  {currentQuestion.difficulty || "Medium"} Difficulty
                </span>
              </div>

              <div className="exam-passage flex-1">
                {parsed.passage ? (
                  <p className="whitespace-pre-wrap">{parsed.passage}</p>
                ) : (
                  <div className="space-y-4">
                    <p className="whitespace-pre-wrap font-sans text-base font-semibold text-slate-900 leading-relaxed">
                      {parsed.prompt}
                    </p>
                    <div className="rounded-lg bg-sky-50/60 border border-sky-100 p-3 text-xs text-sky-900">
                      💡 <strong>SAT Pacing Tip:</strong> Check your calculations carefully. You may use the Reference Sheet (📐) and Calculator (🧮) at any time.
                    </div>
                  </div>
                )}
              </div>
            </div>

            {/* RIGHT COLUMN: Question Prompt & Choices / Numeric Input */}
            <div className="flex flex-col justify-between rounded-xl border border-slate-200 bg-white p-6 shadow-sm overflow-y-auto">
              <div>
                {/* Header: Question Number & Mark for Review */}
                <div className="mb-4 flex items-center justify-between border-b border-slate-100 pb-3">
                  <div className="flex items-center gap-2">
                    <span className="flex h-7 w-7 items-center justify-center rounded-md bg-[#002b49] text-sm font-bold text-white">
                      {currentIndex + 1}
                    </span>
                    <span className="text-xs font-semibold text-slate-500">
                      Question {currentIndex + 1} of {questions.length}
                    </span>
                  </div>

                  <button
                    onClick={() => toggleFlag(currentQuestion.questionId)}
                    className={`mark-for-review-btn ${isQuestionFlagged ? "active" : ""}`}
                  >
                    <svg
                      className="h-3.5 w-3.5"
                      fill={isQuestionFlagged ? "currentColor" : "none"}
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                      strokeWidth="2"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        d="M3 21v-4m0 0V5a2 2 0 012-2h6.5l1 1H21l-3 6 3 6h-8.5l-1-1H5a2 2 0 00-2 2zm9-13.5V9"
                      />
                    </svg>
                    <span>{isQuestionFlagged ? "Marked for Review" : "Mark for Review"}</span>
                  </button>
                </div>

                {/* Prompt Question */}
                <div className="mb-6 font-semibold text-slate-900 text-[15px] leading-relaxed">
                  {parsed.prompt}
                </div>

                {/* A. MULTIPLE CHOICE RENDERER */}
                {!parsed.isStudentProduced && parsed.choices.length > 0 && (
                  <div className="space-y-3">
                    {parsed.choices.map((choice) => {
                      const isSelected = answers.get(currentQuestion.questionId) === choice.choiceId;
                      const isEliminated = currentEliminated.has(choice.choiceId);

                      return (
                        <div key={choice.choiceId} className="flex items-center gap-2">
                          <button
                            onClick={() => {
                              if (!isEliminated) {
                                selectChoice(currentQuestion.questionId, choice.choiceId);
                              }
                            }}
                            disabled={isEliminated}
                            className={`bluebook-choice ${isSelected ? "selected" : ""} ${
                              isEliminated ? "eliminated" : ""
                            }`}
                          >
                            <span className="bluebook-letter-badge">{choice.letter}</span>
                            <span className="flex-1 font-medium">{choice.text}</span>
                          </button>

                          {isEliminatorMode && (
                            <button
                              onClick={() => toggleEliminate(currentQuestion.questionId, choice.choiceId)}
                              title={isEliminated ? "Restore choice" : "Cross out choice"}
                              className={`flex h-9 w-9 shrink-0 items-center justify-center rounded-md border text-xs font-bold transition ${
                                isEliminated
                                  ? "border-amber-400 bg-amber-50 text-amber-800"
                                  : "border-slate-300 bg-white text-slate-400 hover:border-slate-500 hover:text-slate-800"
                              }`}
                            >
                              {isEliminated ? "↩" : "✕"}
                            </button>
                          )}
                        </div>
                      );
                    })}
                  </div>
                )}

                {/* B. STUDENT-PRODUCED RESPONSE (GRID-IN / NUMERIC INPUT) */}
                {parsed.isStudentProduced && (
                  <div className="space-y-4 rounded-xl border border-slate-200 bg-slate-50/60 p-5">
                    <div className="flex items-center justify-between">
                      <span className="text-xs font-bold uppercase tracking-wider text-[#0077c8]">
                        Student-Produced Response (Grid-In)
                      </span>
                      <span className="text-[11px] text-slate-500">Type number or fraction</span>
                    </div>

                    <div className="flex items-center gap-3">
                      <input
                        type="text"
                        value={textAnswers.get(currentQuestion.questionId) || ""}
                        onChange={(e) => setFreeResponseAnswer(currentQuestion.questionId, e.target.value)}
                        placeholder="Answer (e.g. 5 or 3/4)"
                        className="w-full max-w-xs rounded-xl border-2 border-slate-300 bg-white p-3 font-mono text-lg font-bold text-[#002b49] focus:border-[#0077c8] focus:outline-none"
                      />
                      {textAnswers.get(currentQuestion.questionId) && (
                        <span className="text-xs font-bold text-emerald-600">✓ Recorded</span>
                      )}
                    </div>

                    <p className="text-xs text-slate-500">
                      Enter up to 5 characters, including fraction bar (/) or decimal point (.).
                    </p>
                  </div>
                )}
              </div>

              {/* Status Hint */}
              <div className="mt-6 flex items-center justify-between border-t border-slate-100 pt-3 text-xs text-slate-400">
                <span className={isCurrentAnswered ? "text-emerald-700 font-semibold" : ""}>
                  {isCurrentAnswered ? "✓ Answer saved" : "Unanswered"}
                </span>
                <span>Press Next when ready</span>
              </div>
            </div>
          </div>
        </div>
      </main>

      {/* 3. BLUEBOOK BOTTOM NAVIGATION BAR */}
      <footer className="bluebook-footer relative flex h-16 shrink-0 items-center justify-between px-4 sm:px-8">
        <div className="flex items-center gap-4">
          <div className="text-xs font-semibold text-slate-600 sm:text-sm">
            Question <span className="font-bold text-[#002b49]">{currentIndex + 1}</span> of{" "}
            <span className="font-bold text-[#002b49]">{questions.length}</span>
          </div>

          {/* Desktop Shortcut Hints */}
          <div className="hidden xl:flex items-center gap-2 text-[11px] text-slate-400">
            <span className="text-slate-300">|</span>
            <span className="rounded bg-slate-100 border border-slate-200 px-1.5 py-0.5 font-mono text-[10px] font-bold text-slate-600">A-D</span>
            <span>Select</span>
            <span className="rounded bg-slate-100 border border-slate-200 px-1.5 py-0.5 font-mono text-[10px] font-bold text-slate-600">← / →</span>
            <span>Nav</span>
            <span className="rounded bg-slate-100 border border-slate-200 px-1.5 py-0.5 font-mono text-[10px] font-bold text-slate-600">M</span>
            <span>Flag</span>
            <span className="rounded bg-slate-100 border border-slate-200 px-1.5 py-0.5 font-mono text-[10px] font-bold text-slate-600">X</span>
            <span>Cross-out</span>
          </div>
        </div>

        {/* Center: Question Grid Navigator Popover */}
        <div className="relative">
          <button
            onClick={() => setIsNavMenuOpen((prev) => !prev)}
            className="flex items-center gap-2 rounded-full border border-slate-300 bg-white px-4 py-1.5 text-xs font-bold text-slate-800 shadow-xs hover:bg-slate-50 transition active:scale-98"
          >
            <span>Question {currentIndex + 1} of {questions.length}</span>
            <span className="text-[11px] text-slate-400">{isNavMenuOpen ? "▼" : "▲"}</span>
          </button>

          {isNavMenuOpen && (
            <div className="absolute bottom-16 left-1/2 -translate-x-1/2 z-30 w-80 sm:w-96 rounded-2xl border border-slate-200 bg-white p-4 shadow-2xl">
              <div className="flex items-center justify-between pb-2 border-b border-slate-100 text-xs">
                <span className="font-bold text-[#002b49]">Question Navigator</span>
                <button
                  onClick={() => setIsNavMenuOpen(false)}
                  className="text-slate-400 hover:text-slate-700"
                >
                  ✕
                </button>
              </div>

              {/* Status breakdown pills */}
              <div className="my-2 flex items-center justify-between text-[11px] bg-slate-50 p-2 rounded-lg border border-slate-100">
                <span className="flex items-center gap-1 font-semibold text-slate-700">
                  <span className="h-2.5 w-2.5 rounded-full bg-[#002b49]" /> {answeredCount} Answered
                </span>
                <span className="flex items-center gap-1 font-semibold text-slate-500">
                  <span className="h-2.5 w-2.5 rounded-full border border-dashed border-slate-400 bg-white" /> {questions.length - answeredCount} Unanswered
                </span>
                <span className="flex items-center gap-1 font-semibold text-amber-700">
                  <span>🚩</span> {flagged.size} Flagged
                </span>
              </div>

              <div className="my-3 grid grid-cols-5 gap-2 max-h-56 overflow-y-auto p-1">
                {questions.map((q, idx) => {
                  const answered = answers.has(q.questionId);
                  const isCurrent = idx === currentIndex;
                  const hasFlag = flagged.has(q.questionId);

                  return (
                    <button
                      key={q.questionId}
                      onClick={() => {
                        setCurrentIndex(idx);
                        setIsNavMenuOpen(false);
                      }}
                      className={`relative flex h-10 w-10 items-center justify-center rounded-lg text-xs font-bold transition active:scale-95 ${
                        isCurrent
                          ? "ring-2 ring-[#0077c8] ring-offset-1 bg-[#002b49] text-white"
                          : answered
                            ? "bg-[#002b49] text-white hover:bg-[#0a3d66]"
                            : "border border-dashed border-slate-400 bg-white text-slate-700 hover:border-[#0077c8] hover:bg-sky-50"
                      }`}
                    >
                      {idx + 1}
                      {hasFlag && (
                        <span className="absolute -top-1 -right-1 h-3.5 w-3.5 rounded-full bg-amber-400 ring-1 ring-white flex items-center justify-center text-[8px] text-white font-bold">
                          🚩
                        </span>
                      )}
                    </button>
                  );
                })}
              </div>

              {/* Jump to Next Unanswered */}
              {questions.length - answeredCount > 0 && (
                <div className="border-t border-slate-100 pt-2 text-center">
                  <button
                    onClick={() => {
                      const nextUnansweredIdx = questions.findIndex((q) => !answers.has(q.questionId));
                      if (nextUnansweredIdx !== -1) {
                        setCurrentIndex(nextUnansweredIdx);
                        setIsNavMenuOpen(false);
                      }
                    }}
                    className="text-xs font-bold text-[#0077c8] hover:underline"
                  >
                    Jump to next unanswered question →
                  </button>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Right: Back & Next / Review Buttons */}
        <div className="flex items-center gap-3">
          <button
            onClick={() => setCurrentIndex((i) => Math.max(0, i - 1))}
            disabled={currentIndex === 0}
            className="btn-bluebook-secondary"
          >
            ← Back
          </button>

          {currentIndex === questions.length - 1 ? (
            <button
              onClick={() => setPhase("review")}
              className="btn-bluebook bg-[#15803d] hover:bg-[#166534]"
            >
              Review Section →
            </button>
          ) : (
            <button
              onClick={() => setCurrentIndex((i) => Math.min(questions.length - 1, i + 1))}
              className="btn-bluebook"
            >
              Next →
            </button>
          )}
        </div>
      </footer>

      {/* Bluebook Modals */}
      <FormulaSheetModal
        isOpen={isFormulaSheetOpen}
        onClose={() => setIsFormulaSheetOpen(false)}
      />
      <CalculatorModal
        isOpen={isCalculatorOpen}
        onClose={() => setIsCalculatorOpen(false)}
      />
      <DirectionsModal
        isOpen={isDirectionsOpen}
        onClose={() => setIsDirectionsOpen(false)}
        title={quiz.title}
        timeMinutes={quiz.timeLimitMinutes}
      />
    </div>
  );
}
