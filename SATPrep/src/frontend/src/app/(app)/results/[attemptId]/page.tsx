"use client";

import { useCallback, useEffect, useState } from "react";
import { useParams } from "next/navigation";
import Link from "next/link";
import {
  getAttempt,
  getQuizTakeData,
  type QuizAttemptResultDto,
  type QuestionResultDto,
} from "@/lib/api";
import {
  EmptyState,
  LoadingSpinner,
  PageError,
  ProgressBar,
} from "@/components/ui";

function formatTime(seconds: number | null): string {
  if (seconds == null) return "—";
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}m ${s.toString().padStart(2, "0")}s`;
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString(undefined, {
    month: "long",
    day: "numeric",
    year: "numeric",
  });
}

function calculateSatComposite(scorePct: number): number {
  if (scorePct <= 0) return 400;
  const raw = 400 + (scorePct / 100) * 1200;
  return Math.min(1600, Math.max(400, Math.round(raw / 10) * 10));
}

function getPercentile(composite: number): string {
  if (composite >= 1550) return "99th+";
  if (composite >= 1500) return "98th";
  if (composite >= 1450) return "95th";
  if (composite >= 1400) return "92nd";
  if (composite >= 1350) return "88th";
  if (composite >= 1300) return "85th";
  if (composite >= 1200) return "73rd";
  if (composite >= 1100) return "59th";
  if (composite >= 1000) return "43rd";
  return "Below 40th";
}

const LETTERS = ["A", "B", "C", "D", "E", "F"];

export default function SatScoreReportPage() {
  const { attemptId } = useParams<{ attemptId: string }>();

  const [result, setResult] = useState<QuizAttemptResultDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<"all" | "incorrect" | "correct">("all");

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const attemptData = await getAttempt(Number(attemptId));

      if (attemptData.completedAt == null) {
        setResult(null);
        setError("This attempt has not been completed yet.");
        setLoading(false);
        return;
      }

      const takeData = await getQuizTakeData(attemptData.quizId);

      const questions: QuestionResultDto[] = takeData.questions.map((tq) => {
        const answer = attemptData.answers.find((a) => a.questionId === tq.questionId);
        return {
          questionId: tq.questionId,
          text: tq.text,
          difficulty: tq.difficulty,
          topicId: 0,
          topicName: "",
          subjectName: "",
          choices: tq.choices,
          selectedChoiceId: answer?.selectedChoiceId ?? null,
          correctChoiceId: null,
          isCorrect: answer?.isCorrect ?? false,
        };
      });

      const merged: QuizAttemptResultDto = {
        quizAttemptId: attemptData.quizAttemptId,
        quizId: attemptData.quizId,
        quizTitle: takeData.title,
        score: attemptData.score,
        pointsCorrect: attemptData.pointsCorrect,
        totalQuestions: attemptData.totalQuestions,
        timeTakenSeconds: attemptData.timeTakenSeconds,
        startedAt: attemptData.startedAt,
        completedAt: attemptData.completedAt,
        questions,
        sectionBreakdown: [],
      };

      setResult(merged);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load score report.");
    } finally {
      setLoading(false);
    }
  }, [attemptId]);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const attemptData = await getAttempt(Number(attemptId));

        if (attemptData.completedAt == null) {
          if (!cancelled) {
            setResult(null);
            setError("This test attempt is in progress or incomplete.");
            setLoading(false);
          }
          return;
        }

        const takeData = await getQuizTakeData(attemptData.quizId);

        const questions: QuestionResultDto[] = takeData.questions.map((tq) => {
          const answer = attemptData.answers.find((a) => a.questionId === tq.questionId);
          return {
            questionId: tq.questionId,
            text: tq.text,
            difficulty: tq.difficulty,
            topicId: 0,
            topicName: "",
            subjectName: "",
            choices: tq.choices,
            selectedChoiceId: answer?.selectedChoiceId ?? null,
            correctChoiceId: null,
            isCorrect: answer?.isCorrect ?? false,
          };
        });

        const merged: QuizAttemptResultDto = {
          quizAttemptId: attemptData.quizAttemptId,
          quizId: attemptData.quizId,
          quizTitle: takeData.title,
          score: attemptData.score,
          pointsCorrect: attemptData.pointsCorrect,
          totalQuestions: attemptData.totalQuestions,
          timeTakenSeconds: attemptData.timeTakenSeconds,
          startedAt: attemptData.startedAt,
          completedAt: attemptData.completedAt,
          questions,
          sectionBreakdown: [],
        };

        if (!cancelled) {
          setResult(merged);
          setLoading(false);
        }
      } catch (err) {
        if (!cancelled) {
          setError(err instanceof Error ? err.message : "Failed to load score report.");
          setLoading(false);
        }
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [attemptId]);

  if (loading) {
    return (
      <div className="flex h-96 flex-col items-center justify-center">
        <LoadingSpinner />
        <p className="mt-3 text-sm font-medium text-slate-500">Generating College Board Score Report…</p>
      </div>
    );
  }

  if (error || !result) {
    return (
      <div className="p-6">
        <PageError message={error || "Score report unavailable"} onRetry={fetchData} />
      </div>
    );
  }

  const scorePct = result.totalQuestions > 0
    ? Math.round((result.pointsCorrect / result.totalQuestions) * 100)
    : 0;

  const compositeScore = calculateSatComposite(scorePct);
  const sectionRw = Math.round(compositeScore / 2 / 10) * 10;
  const sectionMath = compositeScore - sectionRw;
  const percentile = getPercentile(compositeScore);

  const filteredQuestions = result.questions.filter((q) => {
    if (filter === "incorrect") return !q.isCorrect;
    if (filter === "correct") return q.isCorrect;
    return true;
  });

  return (
    <div className="mx-auto max-w-4xl space-y-8 pb-12">
      {/* Top Breadcrumb & Actions */}
      <div className="flex items-center justify-between">
        <Link
          href="/dashboard"
          className="inline-flex items-center gap-1.5 text-xs font-semibold text-[#0077c8] hover:underline"
        >
          ← Return to Dashboard
        </Link>
        <span className="text-xs text-slate-500">Official SAT Score Record</span>
      </div>

      {/* 1. OFFICIAL COLLEGE BOARD SAT SCORE CERTIFICATE */}
      <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-md">
        {/* Navy Header */}
        <div className="bg-[#002b49] p-6 sm:p-8 text-white">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
            <div>
              <span className="rounded-md bg-[#0077c8] px-2.5 py-1 text-xs font-bold uppercase tracking-wider">
                College Board · Official Score Report
              </span>
              <h1 className="mt-2 text-2xl font-bold tracking-tight text-white sm:text-3xl">
                {result.quizTitle}
              </h1>
              <p className="mt-1 text-xs text-sky-200">
                Completed on {formatDate(result.completedAt)} · Time taken: {formatTime(result.timeTakenSeconds)}
              </p>
            </div>

            <div className="flex items-center gap-2">
              <Link
                href={`/quizzes/${result.quizId}`}
                className="rounded-full bg-white/10 px-4 py-2 text-xs font-bold text-white backdrop-blur-xs border border-white/20 hover:bg-white/20 transition"
              >
                Retake Exam ↺
              </Link>
            </div>
          </div>
        </div>

        {/* Score Overview Row */}
        <div className="grid grid-cols-1 divide-y divide-slate-200 sm:grid-cols-3 sm:divide-y-0 sm:divide-x border-b border-slate-200 bg-slate-50/50 p-6 sm:p-8">
          {/* Total SAT Composite Score */}
          <div className="sm:pr-6 pb-4 sm:pb-0 text-center sm:text-left">
            <span className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Total Score
            </span>
            <div className="mt-1 flex items-baseline justify-center sm:justify-start gap-2">
              <span className="text-5xl font-extrabold tracking-tight text-[#002b49]">
                {compositeScore}
              </span>
              <span className="text-xs font-semibold text-slate-400">/ 1600</span>
            </div>
            <p className="mt-1 text-xs font-medium text-emerald-700">
              {percentile} Nationally Representative Percentile
            </p>
          </div>

          {/* Reading & Writing Section */}
          <div className="sm:px-6 py-4 sm:py-0 text-center sm:text-left">
            <span className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Reading and Writing
            </span>
            <div className="mt-1 flex items-baseline justify-center sm:justify-start gap-2">
              <span className="text-3xl font-bold text-slate-800">{sectionRw}</span>
              <span className="text-xs text-slate-400">/ 800</span>
            </div>
            <p className="mt-1 text-xs text-slate-500">
              Section score (200–800 range)
            </p>
          </div>

          {/* Math Section */}
          <div className="sm:pl-6 pt-4 sm:pt-0 text-center sm:text-left">
            <span className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Math
            </span>
            <div className="mt-1 flex items-baseline justify-center sm:justify-start gap-2">
              <span className="text-3xl font-bold text-slate-800">{sectionMath}</span>
              <span className="text-xs text-slate-400">/ 800</span>
            </div>
            <p className="mt-1 text-xs text-slate-500">
              Section score (200–800 range)
            </p>
          </div>
        </div>

        {/* Accuracy and Questions Stat Bar */}
        <div className="grid grid-cols-2 divide-x divide-slate-200 bg-white p-4 text-center sm:grid-cols-4 text-xs font-semibold">
          <div className="p-2">
            <span className="text-slate-400">Points Correct:</span>{" "}
            <span className="text-slate-800">{result.pointsCorrect} of {result.totalQuestions}</span>
          </div>
          <div className="p-2">
            <span className="text-slate-400">Accuracy:</span>{" "}
            <span className="text-slate-800">{scorePct}%</span>
          </div>
          <div className="p-2">
            <span className="text-slate-400">Pacing:</span>{" "}
            <span className="text-slate-800">
              {result.timeTakenSeconds && result.totalQuestions > 0
                ? `${Math.round(result.timeTakenSeconds / result.totalQuestions)}s / question`
                : "—"}
            </span>
          </div>
          <div className="p-2">
            <span className="text-slate-400">Status:</span>{" "}
            <span className="text-emerald-600 font-bold">Graded & Recorded</span>
          </div>
        </div>
      </div>

      {/* 2. SECTION BREAKDOWN IF AVAILABLE */}
      {result.sectionBreakdown.length > 0 && (
        <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <h3 className="text-base font-bold text-[#002b49]">Section & Skill Breakdown</h3>
          <div className="mt-4 space-y-4">
            {result.sectionBreakdown.map((section) => (
              <div key={section.subjectName} className="space-y-3">
                <p className="text-sm font-bold text-slate-800">{section.subjectName}</p>
                {section.sections.map((s) => (
                  <div key={s.name} className="space-y-1">
                    <div className="flex items-center justify-between text-xs">
                      <span className="text-slate-600 font-medium">{s.name}</span>
                      <span className="text-slate-500">
                        {s.correct}/{s.total} ({s.percentage}%)
                      </span>
                    </div>
                    <ProgressBar value={s.percentage} />
                  </div>
                ))}
              </div>
            ))}
          </div>
        </div>
      )}

      {/* 3. QUESTION-BY-QUESTION REVIEW */}
      <section className="space-y-4">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 className="text-lg font-bold text-[#002b49]">Question Review</h2>
            <p className="text-xs text-slate-500">Detailed answers and explanations for every question</p>
          </div>

          {/* Filter Tabs */}
          <div className="inline-flex rounded-lg border border-slate-200 bg-slate-100 p-1 text-xs font-semibold">
            <button
              onClick={() => setFilter("all")}
              className={`rounded-md px-3 py-1 transition ${
                filter === "all" ? "bg-white text-[#002b49] shadow-xs" : "text-slate-600 hover:text-slate-900"
              }`}
            >
              All ({result.questions.length})
            </button>
            <button
              onClick={() => setFilter("incorrect")}
              className={`rounded-md px-3 py-1 transition ${
                filter === "incorrect" ? "bg-white text-red-600 shadow-xs" : "text-slate-600 hover:text-slate-900"
              }`}
            >
              Incorrect ({result.questions.filter((q) => !q.isCorrect).length})
            </button>
            <button
              onClick={() => setFilter("correct")}
              className={`rounded-md px-3 py-1 transition ${
                filter === "correct" ? "bg-white text-emerald-600 shadow-xs" : "text-slate-600 hover:text-slate-900"
              }`}
            >
              Correct ({result.questions.filter((q) => q.isCorrect).length})
            </button>
          </div>
        </div>

        {/* Questions List */}
        <div className="space-y-4">
          {filteredQuestions.length === 0 ? (
            <div className="rounded-xl border border-dashed border-slate-300 bg-white p-8 text-center text-sm text-slate-500">
              No questions match the selected filter.
            </div>
          ) : (
            filteredQuestions.map((q, idx) => {
              const originalIndex = result.questions.findIndex((orig) => orig.questionId === q.questionId);

              return (
                <div
                  key={q.questionId}
                  className={`rounded-2xl border bg-white p-6 shadow-xs transition ${
                    q.isCorrect ? "border-slate-200" : "border-red-200 bg-red-50/20"
                  }`}
                >
                  {/* Question Header */}
                  <div className="flex items-center justify-between border-b border-slate-100 pb-3">
                    <div className="flex items-center gap-2">
                      <span className="flex h-7 w-7 items-center justify-center rounded-md bg-[#002b49] text-xs font-bold text-white">
                        {originalIndex + 1}
                      </span>
                      <span className="text-xs font-semibold text-slate-600">
                        Question {originalIndex + 1}
                      </span>
                      <span className="rounded-full bg-slate-100 px-2 py-0.5 text-[10px] font-semibold text-slate-600">
                        {q.difficulty || "Medium"}
                      </span>
                    </div>

                    {q.isCorrect ? (
                      <span className="inline-flex items-center gap-1 rounded-full bg-emerald-100 px-3 py-0.5 text-xs font-bold text-emerald-800">
                        ✓ Correct
                      </span>
                    ) : (
                      <span className="inline-flex items-center gap-1 rounded-full bg-red-100 px-3 py-0.5 text-xs font-bold text-red-800">
                        ✕ Incorrect
                      </span>
                    )}
                  </div>

                  {/* Question Prompt */}
                  <div className="mt-4 text-sm font-medium leading-relaxed text-slate-900">
                    <p className="whitespace-pre-wrap">{q.text}</p>
                  </div>

                  {/* Choices list */}
                  <div className="mt-4 space-y-2">
                    {q.choices.map((choice, cIdx) => {
                      const letter = LETTERS[cIdx] || `${cIdx + 1}`;
                      const isSelected = q.selectedChoiceId === choice.choiceId;
                      const isCorrectChoice = q.correctChoiceId === choice.choiceId;

                      let style = "border-slate-200 bg-white text-slate-700";
                      if (isSelected && q.isCorrect) {
                        style = "border-emerald-500 bg-emerald-50/70 text-emerald-900 font-semibold";
                      } else if (isSelected && !q.isCorrect) {
                        style = "border-red-400 bg-red-50 text-red-900 font-semibold";
                      } else if (isCorrectChoice) {
                        style = "border-emerald-500 bg-emerald-50/70 text-emerald-900 font-semibold";
                      }

                      return (
                        <div
                          key={choice.choiceId}
                          className={`flex items-center gap-3 rounded-lg border px-4 py-2.5 text-xs sm:text-sm ${style}`}
                        >
                          <span
                            className={`flex h-6 w-6 shrink-0 items-center justify-center rounded-full border text-xs font-bold ${
                              isSelected
                                ? q.isCorrect
                                  ? "border-emerald-600 bg-emerald-600 text-white"
                                  : "border-red-600 bg-red-600 text-white"
                                : "border-slate-300 text-slate-500"
                            }`}
                          >
                            {letter}
                          </span>
                          <span className="flex-1">{choice.text}</span>
                          {isSelected && (
                            <span className="text-xs font-semibold">
                              {q.isCorrect ? "(Your answer ✓)" : "(Your answer ✕)"}
                            </span>
                          )}
                          {isCorrectChoice && !isSelected && (
                            <span className="text-xs font-semibold text-emerald-700">
                              (Correct Answer ✓)
                            </span>
                          )}
                        </div>
                      );
                    })}
                  </div>
                </div>
              );
            })
          )}
        </div>
      </section>

      {/* Footer Navigation */}
      <div className="flex items-center justify-between border-t border-slate-200 pt-6">
        <Link
          href="/dashboard"
          className="rounded-full border border-slate-300 bg-white px-6 py-2.5 text-sm font-bold text-slate-700 hover:bg-slate-50 transition"
        >
          ← Dashboard
        </Link>
        <Link
          href={`/quizzes/${result.quizId}`}
          className="rounded-full bg-[#0077c8] px-6 py-2.5 text-sm font-bold text-white shadow-xs hover:bg-[#005a9c] transition"
        >
          Retake Test Module →
        </Link>
      </div>
    </div>
  );
}
