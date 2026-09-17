"use client";

import { useState } from "react";
import Link from "next/link";
import { getMistakes, type UserMistakeDto } from "@/lib/api";
import { LoadingSpinner, PageError, EmptyState } from "@/components/ui";
import { useAsyncData } from "@/hooks/useAsyncData";

const LETTERS = ["A", "B", "C", "D", "E"];

export default function MistakesPage() {
  const { data: mistakes, loading, error, reload } = useAsyncData<UserMistakeDto[]>(getMistakes);
  const [filter, setFilter] = useState<string>("all");
  const [searchQuery, setSearchQuery] = useState("");
  const [mode, setMode] = useState<"practice" | "review">("practice");

  // Re-attempt interactive state for Practice Mode
  const [practiceAnswers, setPracticeAnswers] = useState<Map<number, number>>(new Map());
  const [revealedQuestions, setRevealedQuestions] = useState<Set<number>>(new Set());

  if (loading) {
    return (
      <div className="flex h-96 flex-col items-center justify-center">
        <LoadingSpinner />
        <p className="mt-3 text-sm font-medium text-slate-500">Loading your SAT Mistake Bank…</p>
      </div>
    );
  }

  if (error) return <PageError message={error} onRetry={reload} />;

  const list = mistakes || [];
  const subjects = Array.from(new Set(list.map((m) => m.subjectName).filter(Boolean)));

  const filteredList = list.filter((m) => {
    if (filter !== "all" && m.subjectName !== filter) return false;
    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase();
      const matchText = m.text?.toLowerCase().includes(q);
      const matchTopic = m.topicName?.toLowerCase().includes(q);
      const matchSubject = m.subjectName?.toLowerCase().includes(q);
      if (!matchText && !matchTopic && !matchSubject) return false;
    }
    return true;
  });

  const handleSelectPracticeChoice = (questionId: number, choiceId: number) => {
    setPracticeAnswers((prev) => {
      const next = new Map(prev);
      next.set(questionId, choiceId);
      return next;
    });
  };

  const handleCheckAnswer = (questionId: number) => {
    setRevealedQuestions((prev) => {
      const next = new Set(prev);
      next.add(questionId);
      return next;
    });
  };

  const handleResetQuestion = (questionId: number) => {
    setRevealedQuestions((prev) => {
      const next = new Set(prev);
      next.delete(questionId);
      return next;
    });
    setPracticeAnswers((prev) => {
      const next = new Map(prev);
      next.delete(questionId);
      return next;
    });
  };

  // Practice score metrics
  let practiceSolvedCorrect = 0;
  let practiceAttempted = 0;
  revealedQuestions.forEach((qId) => {
    const item = list.find((m) => m.questionId === qId);
    if (item) {
      practiceAttempted++;
      if (practiceAnswers.get(qId) === item.correctChoiceId) {
        practiceSolvedCorrect++;
      }
    }
  });

  return (
    <div className="space-y-8 pb-12">
      {/* Header Banner */}
      <div className="rounded-2xl border border-slate-200 bg-white p-6 sm:p-8 shadow-sm">
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <div className="flex items-center gap-2">
              <span className="rounded-md bg-red-100 px-2.5 py-0.5 text-[11px] font-bold tracking-wider text-red-800 uppercase">
                Mistake Bank
              </span>
              <span className="text-xs text-slate-500">Targeted Error Elimination</span>
            </div>
            <h1 className="mt-2 text-2xl font-extrabold text-[#002b49] sm:text-3xl">
              SAT Error Log & Re-Attempt Arena
            </h1>
            <p className="mt-1 text-sm text-slate-600 max-w-2xl leading-relaxed">
              Every missed question is a direct opportunity to raise your score. Re-solve questions without looking at the answer or analyze your past mistake.
            </p>
          </div>

          {/* Mode Switcher */}
          <div className="flex shrink-0 items-center rounded-xl bg-slate-100 p-1 border border-slate-200">
            <button
              onClick={() => setMode("practice")}
              className={`flex items-center gap-1.5 rounded-lg px-3.5 py-2 text-xs font-bold transition ${
                mode === "practice"
                  ? "bg-[#002b49] text-white shadow-xs"
                  : "text-slate-600 hover:text-slate-900"
              }`}
            >
              <span>⚡ Practice Re-test</span>
            </button>
            <button
              onClick={() => setMode("review")}
              className={`flex items-center gap-1.5 rounded-lg px-3.5 py-2 text-xs font-bold transition ${
                mode === "review"
                  ? "bg-[#002b49] text-white shadow-xs"
                  : "text-slate-600 hover:text-slate-900"
              }`}
            >
              <span>📖 Detailed Analysis</span>
            </button>
          </div>
        </div>

        {/* Practice Stats if attempted */}
        {mode === "practice" && practiceAttempted > 0 && (
          <div className="mt-4 flex items-center gap-3 rounded-xl bg-emerald-50 border border-emerald-200 p-3 text-xs text-emerald-900">
            <span className="text-lg">🎯</span>
            <div>
              <span className="font-bold">Practice Re-Test Score: </span>
              <span>{practiceSolvedCorrect} of {practiceAttempted} solved correctly on re-attempt!</span>
            </div>
          </div>
        )}

        {/* Search & Domain Filter Bar */}
        {list.length > 0 && (
          <div className="mt-6 flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-4 border-t border-slate-100 pt-6">
            <div className="flex flex-wrap items-center gap-2">
              <span className="text-xs font-bold text-slate-600 mr-1">Domain:</span>
              <button
                onClick={() => setFilter("all")}
                className={`rounded-full px-3.5 py-1 text-xs font-bold transition ${
                  filter === "all"
                    ? "bg-[#002b49] text-white"
                    : "border border-slate-200 bg-white text-slate-700 hover:bg-slate-50"
                }`}
              >
                All ({list.length})
              </button>
              {subjects.map((s) => (
                <button
                  key={s}
                  onClick={() => setFilter(s)}
                  className={`rounded-full px-3.5 py-1 text-xs font-bold transition ${
                    filter === s
                      ? "bg-[#0077c8] text-white"
                      : "border border-slate-200 bg-white text-slate-700 hover:bg-slate-50"
                  }`}
                >
                  {s} ({list.filter((m) => m.subjectName === s).length})
                </button>
              ))}
            </div>

            <div className="relative w-full sm:w-64">
              <input
                type="text"
                placeholder="Search questions or skills…"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full rounded-xl border border-slate-300 bg-white py-1.5 pl-8 pr-3 text-xs text-slate-900 focus:border-[#0077c8] focus:outline-none"
              />
              <span className="absolute left-2.5 top-1/2 -translate-y-1/2 text-xs text-slate-400">
                🔍
              </span>
            </div>
          </div>
        )}
      </div>

      {/* Mistakes List */}
      {list.length === 0 ? (
        <EmptyState
          title="No Mistakes Recorded"
          message="Awesome work! You currently have zero missed questions logged. Keep taking practice tests to challenge yourself."
          action={
            <Link
              href="/quizzes"
              className="rounded-full bg-[#0077c8] px-5 py-2 text-xs font-bold text-white hover:bg-[#005a9c]"
            >
              Take a Practice Test →
            </Link>
          }
        />
      ) : filteredList.length === 0 ? (
        <EmptyState
          title="No Questions Match Your Criteria"
          message="No recorded mistakes matched your filter or search query."
        />
      ) : (
        <div className="space-y-6">
          {filteredList.map((m, idx) => {
            const isRevealed = revealedQuestions.has(m.questionId);
            const userPracticeChoice = practiceAnswers.get(m.questionId);
            const isAnsweredInPractice = userPracticeChoice !== undefined;
            const isCorrectInPractice = isRevealed && userPracticeChoice === m.correctChoiceId;

            return (
              <div
                key={`${m.questionId}-${m.answeredAt}`}
                className={`rounded-2xl border bg-white p-6 shadow-xs transition-all ${
                  mode === "practice" && isRevealed
                    ? isCorrectInPractice
                      ? "border-emerald-300 ring-2 ring-emerald-100"
                      : "border-red-300 ring-2 ring-red-100"
                    : "border-slate-200"
                }`}
              >
                {/* Header info */}
                <div className="flex flex-wrap items-center justify-between gap-2 border-b border-slate-100 pb-3">
                  <div className="flex items-center gap-2">
                    <span className="flex h-6 w-6 items-center justify-center rounded-md bg-slate-100 text-xs font-bold text-slate-700">
                      #{idx + 1}
                    </span>
                    <span className="rounded-md bg-slate-100 px-2 py-0.5 text-xs font-bold text-slate-700">
                      {m.subjectName || "SAT Prep"}
                    </span>
                    {m.topicName && (
                      <span className="rounded-md bg-sky-50 px-2 py-0.5 text-xs font-semibold text-[#0077c8]">
                        {m.topicName}
                      </span>
                    )}
                    <span className="rounded-full bg-slate-100 px-2 py-0.5 text-[10px] text-slate-500">
                      {m.difficulty || "Medium"} Difficulty
                    </span>
                  </div>

                  <Link
                    href={`/quizzes/${m.quizId}`}
                    className="text-xs font-bold text-[#0077c8] hover:underline"
                  >
                    From: {m.quizTitle} ↺
                  </Link>
                </div>

                {/* Question Prompt */}
                <div className="mt-4 font-medium text-slate-900 leading-relaxed text-sm sm:text-base">
                  <p className="whitespace-pre-wrap">{m.text}</p>
                </div>

                {/* ─── MODE A: INTERACTIVE RE-ATTEMPT PRACTICE ─── */}
                {mode === "practice" && (
                  <div className="mt-5 space-y-3">
                    {m.choices && m.choices.length > 0 && (
                      <div className="space-y-2">
                        {m.choices.map((c, cIdx) => {
                          const letter = LETTERS[cIdx] || `${cIdx + 1}`;
                          const isSelected = userPracticeChoice === c.choiceId;
                          const isCorrect = m.correctChoiceId === c.choiceId;

                          let style =
                            "border-slate-200 bg-white text-slate-700 hover:border-[#0077c8] hover:bg-sky-50/50 cursor-pointer";

                          if (isRevealed) {
                            if (isCorrect) {
                              style = "border-emerald-500 bg-emerald-50/80 text-emerald-900 font-semibold";
                            } else if (isSelected) {
                              style = "border-red-400 bg-red-50 text-red-900 font-semibold";
                            } else {
                              style = "border-slate-200 opacity-60 text-slate-500";
                            }
                          } else if (isSelected) {
                            style = "border-[#0077c8] bg-sky-50 text-[#002b49] font-semibold ring-1 ring-[#0077c8]";
                          }

                          return (
                            <button
                              key={c.choiceId}
                              onClick={() => {
                                if (!isRevealed) {
                                  handleSelectPracticeChoice(m.questionId, c.choiceId);
                                }
                              }}
                              disabled={isRevealed}
                              className={`flex w-full items-center gap-3 rounded-xl border px-4 py-3 text-left text-xs sm:text-sm transition-all active:scale-[0.99] ${style}`}
                            >
                              <span
                                className={`flex h-7 w-7 shrink-0 items-center justify-center rounded-full border text-xs font-bold transition ${
                                  isRevealed && isCorrect
                                    ? "border-emerald-600 bg-emerald-600 text-white"
                                    : isRevealed && isSelected
                                      ? "border-red-600 bg-red-600 text-white"
                                      : isSelected
                                        ? "border-[#0077c8] bg-[#0077c8] text-white"
                                        : "border-slate-300 text-slate-500"
                                }`}
                              >
                                {letter}
                              </span>
                              <span className="flex-1">{c.text}</span>
                              {isRevealed && isCorrect && (
                                <span className="text-xs font-bold text-emerald-700">Correct Answer ✓</span>
                              )}
                              {isRevealed && isSelected && !isCorrect && (
                                <span className="text-xs font-bold text-red-600">Your choice ✕</span>
                              )}
                            </button>
                          );
                        })}
                      </div>
                    )}

                    {/* Action Bar for Practice Mode */}
                    <div className="mt-4 flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 pt-4">
                      <div className="flex items-center gap-2">
                        {!isRevealed ? (
                          <button
                            onClick={() => handleCheckAnswer(m.questionId)}
                            disabled={!isAnsweredInPractice}
                            className="rounded-full bg-[#0077c8] px-5 py-2 text-xs font-bold text-white shadow-xs transition hover:bg-[#005a9c] disabled:opacity-40 active:scale-98"
                          >
                            Check My Answer
                          </button>
                        ) : (
                          <button
                            onClick={() => handleResetQuestion(m.questionId)}
                            className="rounded-full border border-slate-300 bg-white px-4 py-1.5 text-xs font-bold text-slate-700 hover:bg-slate-50 transition"
                          >
                            ↺ Re-try Again
                          </button>
                        )}

                        {!isRevealed && (
                          <button
                            onClick={() => handleCheckAnswer(m.questionId)}
                            className="text-xs font-semibold text-slate-500 hover:text-slate-800 underline"
                          >
                            Reveal Answer
                          </button>
                        )}
                      </div>

                      {isRevealed && (
                        <div className="flex items-center gap-2 text-xs font-bold">
                          {isCorrectInPractice ? (
                            <span className="text-emerald-700 bg-emerald-50 px-3 py-1 rounded-full border border-emerald-200">
                              🎯 Great job! You solved it correctly on this try.
                            </span>
                          ) : (
                            <span className="text-red-700 bg-red-50 px-3 py-1 rounded-full border border-red-200">
                              Need review. See the correct solution marked in green.
                            </span>
                          )}
                        </div>
                      )}
                    </div>
                  </div>
                )}

                {/* ─── MODE B: DETAILED ANALYSIS MODE ─── */}
                {mode === "review" && (
                  <div className="mt-5 space-y-2">
                    <div className="text-xs font-semibold text-slate-500 mb-2">
                      Comparison: Your Previous Test Submission vs Correct Key
                    </div>
                    {m.choices && m.choices.length > 0 && (
                      <div className="space-y-2">
                        {m.choices.map((c, cIdx) => {
                          const letter = LETTERS[cIdx] || `${cIdx + 1}`;
                          const isYourChoice = m.selectedChoiceId === c.choiceId;
                          const isCorrect = m.correctChoiceId === c.choiceId;

                          let rowStyle = "border-slate-200 bg-white text-slate-700";
                          if (isCorrect) {
                            rowStyle = "border-emerald-500 bg-emerald-50/70 text-emerald-900 font-semibold";
                          } else if (isYourChoice) {
                            rowStyle = "border-red-400 bg-red-50 text-red-900 font-semibold";
                          }

                          return (
                            <div
                              key={c.choiceId}
                              className={`flex items-center gap-3 rounded-xl border px-4 py-2.5 text-xs sm:text-sm ${rowStyle}`}
                            >
                              <span
                                className={`flex h-6 w-6 shrink-0 items-center justify-center rounded-full border text-xs font-bold ${
                                  isCorrect
                                    ? "border-emerald-600 bg-emerald-600 text-white"
                                    : isYourChoice
                                      ? "border-red-600 bg-red-600 text-white"
                                      : "border-slate-300 text-slate-500"
                                }`}
                              >
                                {letter}
                              </span>
                              <span className="flex-1">{c.text}</span>
                              {isYourChoice && (
                                <span className="text-xs font-bold text-red-600">Your past choice (Incorrect)</span>
                              )}
                              {isCorrect && (
                                <span className="text-xs font-bold text-emerald-700">Correct Answer ✓</span>
                              )}
                            </div>
                          );
                        })}
                      </div>
                    )}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
