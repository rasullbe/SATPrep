"use client";

import Link from "next/link";
import { getAttempts, type QuizAttemptGetDto } from "@/lib/api";
import { LoadingSpinner, PageError, EmptyState } from "@/components/ui";
import { useAsyncData } from "@/hooks/useAsyncData";

function formatDuration(seconds: number | null): string {
  if (seconds == null) return "—";
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return m > 0 ? `${m}m ${s}s` : `${s}s`;
}

function calculateSatComposite(scorePct: number): number {
  if (scorePct <= 0) return 400;
  const raw = 400 + (scorePct / 100) * 1200;
  return Math.min(1600, Math.max(400, Math.round(raw / 10) * 10));
}

export default function HistoryPage() {
  const { data: attempts, loading, error, reload } = useAsyncData<QuizAttemptGetDto[]>(getAttempts);

  if (loading) {
    return (
      <div className="flex h-96 flex-col items-center justify-center">
        <LoadingSpinner />
        <p className="mt-3 text-sm font-medium text-slate-500">Retrieving SAT examination history…</p>
      </div>
    );
  }

  if (error) return <PageError message={error} onRetry={reload} />;

  const attemptsList = attempts || [];
  const completedAttempts = attemptsList.filter((a) => a.completedAt !== null);

  const highestScore = completedAttempts.length > 0
    ? Math.max(...completedAttempts.map((a) => calculateSatComposite(a.score)))
    : 0;

  return (
    <div className="space-y-8 pb-12">
      {/* Header Banner */}
      <div className="rounded-2xl border border-slate-200 bg-white p-6 sm:p-8 shadow-sm">
        <div className="flex items-center gap-2">
          <span className="rounded-md bg-[#0077c8] px-2.5 py-0.5 text-[11px] font-bold tracking-wider text-white uppercase">
            Score Archive
          </span>
          <span className="text-xs text-slate-500">College Board Official Records</span>
        </div>
        <h1 className="mt-3 text-2xl font-extrabold text-[#002b49] sm:text-3xl">
          SAT Examination History
        </h1>
        <p className="mt-1 text-sm text-slate-600">
          A complete timeline of all your Digital SAT practice modules and diagnostic tests.
        </p>

        {completedAttempts.length > 0 && (
          <div className="mt-6 grid grid-cols-2 gap-4 border-t border-slate-100 pt-6 sm:grid-cols-3">
            <div>
              <p className="text-xs font-semibold uppercase text-slate-400">Total Exams Completed</p>
              <p className="mt-1 text-2xl font-bold text-[#002b49]">{completedAttempts.length}</p>
            </div>
            <div>
              <p className="text-xs font-semibold uppercase text-slate-400">Personal Best (Superscore)</p>
              <p className="mt-1 text-2xl font-bold text-emerald-700">{highestScore} <span className="text-xs font-normal text-slate-400">/ 1600</span></p>
            </div>
            <div>
              <p className="text-xs font-semibold uppercase text-slate-400">Status</p>
              <p className="mt-1 text-sm font-bold text-sky-700">Official Profile Active</p>
            </div>
          </div>
        )}
      </div>

      {/* History Table */}
      {completedAttempts.length === 0 ? (
        <EmptyState
          title="No Examination Records Found"
          message="Complete your first Digital SAT practice module to record official scores and review performance."
          action={
            <Link
              href="/quizzes"
              className="rounded-full bg-[#0077c8] px-5 py-2 text-xs font-bold text-white hover:bg-[#005a9c]"
            >
              Take Practice Test →
            </Link>
          }
        />
      ) : (
        <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xs">
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead>
                <tr className="border-b border-slate-200 bg-slate-50 text-[11px] font-bold uppercase tracking-wider text-slate-500">
                  <th className="px-6 py-3.5">Test Module</th>
                  <th className="px-6 py-3.5">Test Date</th>
                  <th className="px-6 py-3.5">Estimated SAT Score</th>
                  <th className="px-6 py-3.5">Raw Score</th>
                  <th className="px-6 py-3.5">Duration</th>
                  <th className="px-6 py-3.5 text-right">Report</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {completedAttempts.map((a) => {
                  const satScore = calculateSatComposite(a.score);
                  return (
                    <tr key={a.quizAttemptId} className="hover:bg-slate-50/60 transition">
                      <td className="px-6 py-4 font-bold text-slate-900">
                        Practice Module #{a.quizId}
                      </td>
                      <td className="px-6 py-4 text-slate-600">
                        {new Date(a.startedAt).toLocaleDateString("en-US", {
                          month: "short",
                          day: "numeric",
                          year: "numeric",
                        })}
                      </td>
                      <td className="px-6 py-4">
                        <span className="inline-flex items-center gap-1.5 rounded-full bg-[#002b49] px-3 py-1 font-mono text-xs font-bold text-sky-200">
                          {satScore} SAT
                        </span>
                      </td>
                      <td className="px-6 py-4 text-slate-700 font-medium">
                        {a.pointsCorrect} / {a.totalQuestions} ({a.score}%)
                      </td>
                      <td className="px-6 py-4 text-slate-500">
                        {formatDuration(a.timeTakenSeconds)}
                      </td>
                      <td className="px-6 py-4 text-right">
                        <Link
                          href={`/results/${a.quizAttemptId}`}
                          className="rounded-full bg-sky-50 px-3.5 py-1.5 text-xs font-bold text-[#0077c8] hover:bg-[#0077c8] hover:text-white transition"
                        >
                          View Score Report →
                        </Link>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}