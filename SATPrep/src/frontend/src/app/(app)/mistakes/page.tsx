"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { getMistakes, type UserMistakeDto } from "@/lib/api";
import { LoadingSpinner, PageError, EmptyState } from "@/components/ui";
import { useAsyncData } from "@/hooks/useAsyncData";

const LETTERS = ["A", "B", "C", "D", "E"];

export default function MistakesPage() {
  const { data: mistakes, loading, error, reload } = useAsyncData<UserMistakeDto[]>(getMistakes);
  const [filter, setFilter] = useState<string>("all");

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
    if (filter === "all") return true;
    return m.subjectName === filter;
  });

  return (
    <div className="space-y-8 pb-12">
      {/* Header Banner */}
      <div className="rounded-2xl border border-slate-200 bg-white p-6 sm:p-8 shadow-sm">
        <div className="flex items-center gap-2">
          <span className="rounded-md bg-red-100 px-2.5 py-0.5 text-[11px] font-bold tracking-wider text-red-800 uppercase">
            Mistake Bank
          </span>
          <span className="text-xs text-slate-500">Targeted Score Improvement</span>
        </div>
        <h1 className="mt-3 text-2xl font-extrabold text-[#002b49] sm:text-3xl">
          Review & Master Your Incorrect Answers
        </h1>
        <p className="mt-1 text-sm text-slate-600 max-w-2xl leading-relaxed">
          Top scorers analyze every single error. Here are all the questions you missed across previous practice tests, organized by subject and skill domain.
        </p>

        {list.length > 0 && (
          <div className="mt-6 flex flex-wrap items-center gap-2 border-t border-slate-100 pt-6">
            <span className="text-xs font-bold text-slate-600 mr-2">Filter by Domain:</span>
            <button
              onClick={() => setFilter("all")}
              className={`rounded-full px-3.5 py-1 text-xs font-bold transition ${
                filter === "all"
                  ? "bg-[#002b49] text-white"
                  : "border border-slate-200 bg-white text-slate-700 hover:bg-slate-50"
              }`}
            >
              All Mistakes ({list.length})
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
        )}
      </div>

      {/* Mistakes List */}
      {list.length === 0 ? (
        <EmptyState
          title="No Mistakes Recorded"
          message="Awesome job! You currently have zero missed questions. Keep taking practice tests to test your knowledge."
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
          title="No Mistakes in this Subject"
          message="You have no recorded mistakes for this subject filter."
        />
      ) : (
        <div className="space-y-6">
          {filteredList.map((m, idx) => (
            <div
              key={`${m.questionId}-${m.answeredAt}`}
              className="rounded-2xl border border-red-200 bg-white p-6 shadow-xs"
            >
              <div className="flex flex-wrap items-center justify-between gap-2 border-b border-slate-100 pb-3">
                <div className="flex items-center gap-2">
                  <span className="flex h-6 w-6 items-center justify-center rounded-md bg-red-100 text-xs font-bold text-red-800">
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
                    {m.difficulty}
                  </span>
                </div>

                <Link
                  href={`/quizzes/${m.quizId}`}
                  className="text-xs font-bold text-[#0077c8] hover:underline"
                >
                  From: {m.quizTitle} ↺
                </Link>
              </div>

              <div className="mt-4 font-medium text-slate-900 leading-relaxed text-sm sm:text-base">
                <p className="whitespace-pre-wrap">{m.text}</p>
              </div>

              {m.choices && m.choices.length > 0 && (
                <div className="mt-5 space-y-2">
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
                        className={`flex items-center gap-3 rounded-lg border px-4 py-2.5 text-xs sm:text-sm ${rowStyle}`}
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
                          <span className="text-xs font-bold text-red-600">Your selection (Incorrect)</span>
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
          ))}
        </div>
      )}
    </div>
  );
}
