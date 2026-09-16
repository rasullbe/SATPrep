"use client";

import Link from "next/link";
import { getQuizzes, type QuizGetDto } from "@/lib/api";
import { EmptyState, LoadingSpinner, PageError } from "@/components/ui";
import { useAsyncData } from "@/hooks/useAsyncData";

export default function QuizzesPage() {
  const { data: quizzes, loading, error, reload } = useAsyncData<QuizGetDto[]>(getQuizzes);

  if (loading) {
    return (
      <div className="flex h-96 flex-col items-center justify-center">
        <LoadingSpinner />
        <p className="mt-3 text-sm font-medium text-slate-500">Loading official SAT practice tests…</p>
      </div>
    );
  }

  if (error) return <PageError message={error} onRetry={reload} />;

  return (
    <div className="space-y-8 pb-12">
      {/* Header Banner */}
      <div className="rounded-2xl border border-slate-200 bg-white p-6 sm:p-8 shadow-sm">
        <div className="flex items-center gap-2">
          <span className="rounded-md bg-[#0077c8] px-2.5 py-0.5 text-[11px] font-bold tracking-wider text-white uppercase">
            Official Simulation
          </span>
          <span className="text-xs text-slate-500">College Board Bluebook Standards</span>
        </div>
        <h1 className="mt-3 text-2xl font-extrabold text-[#002b49] sm:text-3xl">
          Digital SAT Practice Tests
        </h1>
        <p className="mt-2 max-w-2xl text-sm text-slate-600 leading-relaxed">
          Practice under true test-day conditions. Every exam replicates the timing, split-screen passage layout, on-screen Math formula reference sheet, and option elimination tools used in the official College Board Bluebook app.
        </p>

        {/* Test Day Readiness Highlights */}
        <div className="mt-6 grid grid-cols-2 gap-3 sm:grid-cols-4 border-t border-slate-100 pt-6 text-xs text-slate-600">
          <div className="flex items-center gap-2">
            <span className="text-base text-[#0077c8]">⏱️</span>
            <span>Accurate Section Timers</span>
          </div>
          <div className="flex items-center gap-2">
            <span className="text-base text-[#0077c8]">📐</span>
            <span>Math Formula Sheets</span>
          </div>
          <div className="flex items-center gap-2">
            <span className="text-base text-[#0077c8]">✂️</span>
            <span>Option Strikethrough</span>
          </div>
          <div className="flex items-center gap-2">
            <span className="text-base text-[#0077c8]">📊</span>
            <span>Instant 1600 Scoring</span>
          </div>
        </div>
      </div>

      {/* Quizzes List */}
      {!quizzes || quizzes.length === 0 ? (
        <EmptyState
          title="No Practice Tests Published"
          message="Tests are currently being synced with the SAT question bank. Check back shortly."
        />
      ) : (
        <div className="grid gap-6 md:grid-cols-2">
          {quizzes.map((quiz, idx) => (
            <div
              key={quiz.quizId}
              className="flex flex-col justify-between rounded-2xl border border-slate-200 bg-white p-6 shadow-xs transition-all hover:border-[#0077c8] hover:shadow-md"
            >
              <div>
                <div className="flex items-start justify-between">
                  <span className="flex h-9 w-9 items-center justify-center rounded-xl bg-[#002b49] font-mono text-sm font-bold text-white shadow-xs">
                    {String(idx + 1).padStart(2, "0")}
                  </span>
                  <span className="rounded-full bg-sky-50 px-2.5 py-0.5 text-xs font-semibold text-[#0077c8] border border-sky-100">
                    {quiz.isPublished ? "Official Release" : "Draft"}
                  </span>
                </div>

                <h3 className="mt-4 text-lg font-bold text-slate-900">
                  {quiz.title}
                </h3>
                {quiz.description ? (
                  <p className="mt-2 text-xs text-slate-500 leading-relaxed line-clamp-3">
                    {quiz.description}
                  </p>
                ) : (
                  <p className="mt-2 text-xs text-slate-400 italic">
                    Comprehensive Digital SAT module containing Reading, Writing, or Math practice items.
                  </p>
                )}

                <div className="mt-4 flex flex-wrap items-center gap-2 text-xs">
                  <span className="rounded-md bg-slate-100 px-2.5 py-1 font-semibold text-slate-700">
                    {quiz.questionCount} Questions
                  </span>
                  <span className="rounded-md bg-slate-100 px-2.5 py-1 font-semibold text-slate-700">
                    {quiz.timeLimitMinutes != null ? `${quiz.timeLimitMinutes} Minutes` : "Untimed"}
                  </span>
                  <span className="rounded-md bg-emerald-50 px-2.5 py-1 font-semibold text-emerald-700 border border-emerald-100">
                    Bluebook Mode
                  </span>
                </div>
              </div>

              <div className="mt-6 border-t border-slate-100 pt-4">
                <Link
                  href={`/quizzes/${quiz.quizId}`}
                  className="flex w-full items-center justify-center gap-2 rounded-full bg-[#0077c8] py-2.5 text-sm font-bold text-white shadow-xs transition hover:bg-[#005a9c]"
                >
                  <span>Launch Practice Test</span>
                  <span>→</span>
                </Link>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}