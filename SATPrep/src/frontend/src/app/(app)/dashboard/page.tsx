"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { getDashboard, type DashboardDto } from "@/lib/api";
import { formatRelativeTime } from "@/lib/auth";
import { useAuth } from "@/contexts/AuthContext";
import { useAsyncData } from "@/hooks/useAsyncData";
import { PageError, LoadingSpinner } from "@/components/ui";

function calculateSatScore(percentage: number): number {
  if (percentage <= 0) return 400;
  // Scaled from 400 to 1600, rounded to nearest 10 (official SAT scoring convention)
  const raw = 400 + (percentage / 100) * 1200;
  return Math.min(1600, Math.max(400, Math.round(raw / 10) * 10));
}

export default function DashboardPage() {
  const { user } = useAuth();
  const { data, loading, error, reload } = useAsyncData<DashboardDto>(getDashboard);
  const [today, setToday] = useState("");

  useEffect(() => {
    setToday(
      new Date().toLocaleDateString("en-US", {
        weekday: "long",
        month: "long",
        day: "numeric",
      })
    );
  }, []);

  if (loading) {
    return (
      <div className="flex h-96 flex-col items-center justify-center">
        <LoadingSpinner />
        <p className="mt-3 text-sm text-slate-500 font-medium">Loading your SAT score portal…</p>
      </div>
    );
  }

  if (error || !data) {
    return <PageError message={error ?? "Could not load dashboard data"} onRetry={reload} />;
  }

  const firstName = (user?.name ?? data.user.name).split(" ")[0];
  const compositeSat = calculateSatScore(data.user.averageScore);
  const rwScore = Math.round(compositeSat / 2 / 10) * 10;
  const mathScore = compositeSat - rwScore;

  return (
    <div className="space-y-8">
      {/* 1. Header Banner - College Board My SAT Portal */}
      <div className="flex flex-col gap-4 rounded-2xl bg-[#002b49] p-6 text-white shadow-md sm:flex-row sm:items-center sm:justify-between sm:p-8">
        <div>
          <div className="flex items-center gap-2">
            <span className="rounded-md bg-[#0077c8] px-2.5 py-0.5 text-[11px] font-bold tracking-wider uppercase">
              Student Dashboard
            </span>
            <span className="text-xs text-sky-200">{today}</span>
          </div>
          <h1 className="mt-2 text-2xl font-bold tracking-tight text-white sm:text-3xl">
            Welcome back, {firstName}
          </h1>
          <p className="mt-1 text-sm text-slate-300">
            {data.user.studyStreak > 0
              ? `🔥 ${data.user.studyStreak}-day study streak! Keep building test-day confidence.`
              : "Every practice module brings you closer to your target score."}
          </p>
        </div>

        <Link
          href="/quizzes"
          className="inline-flex items-center justify-center gap-2 rounded-full bg-[#0077c8] px-6 py-3 text-sm font-bold text-white shadow-md transition hover:bg-[#005a9c] active:scale-98 shrink-0"
        >
          <span>Take a Practice Test</span>
          <span>→</span>
        </Link>
      </div>

      {/* 2. Key Metrics Grid: SAT Scaled Score, Streak, Hours, Total Questions */}
      <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {/* SAT Composite Score Card */}
        <div className="relative overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Estimated SAT Score
            </span>
            <span className="rounded-full bg-amber-50 px-2 py-0.5 text-[11px] font-bold text-amber-700 border border-amber-200">
              400–1600 Scale
            </span>
          </div>

          <div className="mt-3 flex items-baseline gap-2">
            <span className="text-4xl font-extrabold text-[#002b49] tracking-tight">
              {data.user.totalQuestionsAnswered > 0 ? compositeSat : "—"}
            </span>
            {data.user.totalQuestionsAnswered > 0 && (
              <span className="text-xs font-semibold text-slate-400">
                ({Math.round(data.user.averageScore)}% accuracy)
              </span>
            )}
          </div>

          {data.user.totalQuestionsAnswered > 0 ? (
            <div className="mt-3 flex items-center gap-3 border-t border-slate-100 pt-3 text-xs text-slate-600">
              <span>RW: <strong className="text-slate-900">{rwScore}</strong></span>
              <span className="text-slate-300">|</span>
              <span>Math: <strong className="text-slate-900">{mathScore}</strong></span>
            </div>
          ) : (
            <p className="mt-3 text-xs text-slate-400 border-t border-slate-100 pt-3">
              Take your first test to unlock estimated score
            </p>
          )}
        </div>

        {/* Study Streak */}
        <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Study Streak
            </span>
            <span className="text-lg">🔥</span>
          </div>
          <div className="mt-3 flex items-baseline gap-1">
            <span className="text-4xl font-extrabold text-[#002b49] tracking-tight">
              {data.user.studyStreak}
            </span>
            <span className="text-sm font-semibold text-slate-500">days</span>
          </div>
          <p className="mt-3 border-t border-slate-100 pt-3 text-xs text-slate-500">
            {data.user.studyStreak > 0 ? "Consistent practice pays off on test day" : "Start your streak today"}
          </p>
        </div>

        {/* Study Time */}
        <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Total Study Time
            </span>
            <span className="text-lg">⏱️</span>
          </div>
          <div className="mt-3 flex items-baseline gap-1">
            <span className="text-4xl font-extrabold text-[#002b49] tracking-tight">
              {Math.floor(data.user.totalStudyMinutes / 60)}h {data.user.totalStudyMinutes % 60}m
            </span>
          </div>
          <p className="mt-3 border-t border-slate-100 pt-3 text-xs text-slate-500">
            Across {data.user.totalSessions} practice sessions
          </p>
        </div>

        {/* Questions Answered */}
        <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Questions Solved
            </span>
            <span className="text-lg">✏️</span>
          </div>
          <div className="mt-3 flex items-baseline gap-1">
            <span className="text-4xl font-extrabold text-[#002b49] tracking-tight">
              {data.user.totalQuestionsAnswered}
            </span>
            <span className="text-sm font-semibold text-slate-500">questions</span>
          </div>
          <p className="mt-3 border-t border-slate-100 pt-3 text-xs text-slate-500">
            Covering Math and Reading & Writing
          </p>
        </div>
      </div>

      {/* 3. Main Content: Practice Tests & Recent Attempts */}
      <div className="grid gap-8 lg:grid-cols-[7fr_5fr]">
        {/* Practice Tests Catalog Section */}
        <section className="space-y-4">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-lg font-bold text-[#002b49]">Digital SAT Practice Tests</h2>
              <p className="text-xs text-slate-500">Full-length modules timed with Bluebook exam tools</p>
            </div>
            <Link href="/quizzes" className="text-xs font-semibold text-[#0077c8] hover:underline">
              View All ({data.availableQuizzes.length})
            </Link>
          </div>

          <div className="space-y-3">
            {data.availableQuizzes.length === 0 ? (
              <div className="rounded-xl border border-dashed border-slate-300 bg-white p-8 text-center">
                <p className="text-sm font-semibold text-slate-700">No practice tests currently active</p>
                <p className="text-xs text-slate-500 mt-1">Check back shortly for new official-format modules.</p>
              </div>
            ) : (
              data.availableQuizzes.slice(0, 4).map((quiz, idx) => (
                <Link
                  key={quiz.quizId}
                  href={`/quizzes/${quiz.quizId}`}
                  className="group flex flex-col justify-between rounded-xl border border-slate-200 bg-white p-5 shadow-xs transition-all hover:border-[#0077c8] hover:shadow-md sm:flex-row sm:items-center"
                >
                  <div className="flex items-start gap-3">
                    <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-sky-50 text-xs font-bold text-[#0077c8]">
                      #{idx + 1}
                    </span>
                    <div>
                      <h3 className="text-sm font-bold text-slate-900 group-hover:text-[#0077c8] transition">
                        {quiz.title}
                      </h3>
                      {quiz.description && (
                        <p className="mt-0.5 text-xs text-slate-500 line-clamp-1">{quiz.description}</p>
                      )}
                      <div className="mt-2 flex items-center gap-3 text-xs text-slate-500">
                        <span className="font-semibold text-slate-700">{quiz.questionCount} Questions</span>
                        <span>•</span>
                        <span>{quiz.timeLimitMinutes != null ? `${quiz.timeLimitMinutes} mins` : "Untimed"}</span>
                        <span>•</span>
                        <span className="text-emerald-700 font-medium">Digital Bluebook format</span>
                      </div>
                    </div>
                  </div>

                  <div className="mt-3 sm:mt-0 flex items-center gap-2">
                    <span className="rounded-full bg-[#0077c8] px-4 py-1.5 text-xs font-bold text-white shadow-xs group-hover:bg-[#005a9c] transition">
                      Start Test →
                    </span>
                  </div>
                </Link>
              ))
            )}
          </div>
        </section>

        {/* Recent Attempts & Score Reports */}
        <section className="space-y-4">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-lg font-bold text-[#002b49]">Recent Score Reports</h2>
              <p className="text-xs text-slate-500">Review completed tests and detailed feedback</p>
            </div>
            <Link href="/history" className="text-xs font-semibold text-[#0077c8] hover:underline">
              Full History
            </Link>
          </div>

          <div className="space-y-3">
            {data.recentAttempts.length === 0 ? (
              <div className="rounded-xl border border-dashed border-slate-300 bg-white p-8 text-center">
                <span className="text-2xl">🎯</span>
                <p className="mt-2 text-sm font-bold text-slate-800">No completed exams yet</p>
                <p className="mt-1 text-xs text-slate-500">
                  Complete your first practice module to generate an official score report.
                </p>
                <Link
                  href="/quizzes"
                  className="mt-4 inline-block rounded-full bg-[#0077c8] px-4 py-1.5 text-xs font-bold text-white shadow-xs hover:bg-[#005a9c]"
                >
                  Start Now
                </Link>
              </div>
            ) : (
              data.recentAttempts.slice(0, 5).map((attempt) => {
                const attemptSat = calculateSatScore(attempt.score);
                return (
                  <Link
                    key={attempt.quizAttemptId}
                    href={`/results/${attempt.quizAttemptId}`}
                    className="group flex items-center justify-between rounded-xl border border-slate-200 bg-white p-4 shadow-xs transition hover:border-[#0077c8] hover:shadow-sm"
                  >
                    <div className="flex items-center gap-3">
                      <div className="flex h-11 w-11 shrink-0 flex-col items-center justify-center rounded-lg bg-[#002b49] text-white">
                        <span className="text-xs font-bold">{attemptSat}</span>
                        <span className="text-[9px] text-sky-200">SAT</span>
                      </div>
                      <div>
                        <p className="text-xs font-bold text-slate-800 group-hover:text-[#0077c8] transition">
                          {attempt.pointsCorrect} / {attempt.totalQuestions} Questions Correct
                        </p>
                        <p className="text-[11px] text-slate-400">
                          {formatRelativeTime(attempt.startedAt)} · Score: {attempt.score}%
                        </p>
                      </div>
                    </div>
                    <span className="text-xs font-bold text-[#0077c8] group-hover:translate-x-0.5 transition">
                      View Report →
                    </span>
                  </Link>
                );
              })
            )}
          </div>
        </section>
      </div>

      {/* 4. Domain Mastery / Weak Areas Breakdown */}
      {data.weakAreas.length > 0 && (
        <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-center justify-between pb-4 border-b border-slate-100">
            <div>
              <h2 className="text-lg font-bold text-[#002b49]">SAT Knowledge & Skills Mastery</h2>
              <p className="text-xs text-slate-500">Domain breakdown to target priority study areas</p>
            </div>
            <span className="rounded-full bg-sky-50 px-3 py-1 text-xs font-semibold text-[#0077c8]">
              {data.weakAreas.length} Topics Evaluated
            </span>
          </div>

          <div className="mt-4 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {data.weakAreas.map((area) => {
              const accuracy = Math.round(area.accuracyPercentage);
              const isStrong = accuracy >= 70;
              const isModerate = accuracy >= 50 && accuracy < 70;

              return (
                <div key={area.topicId} className="rounded-xl border border-slate-200 bg-slate-50/70 p-4">
                  <div className="flex items-start justify-between">
                    <div>
                      <span className="text-[10px] font-bold uppercase tracking-wider text-slate-400">
                        {area.subjectName}
                      </span>
                      <h4 className="font-bold text-slate-800 text-sm">{area.topicName}</h4>
                    </div>
                    <span
                      className={`text-xs font-bold px-2 py-0.5 rounded-md ${
                        isStrong
                          ? "bg-emerald-100 text-emerald-800"
                          : isModerate
                            ? "bg-amber-100 text-amber-800"
                            : "bg-red-100 text-red-800"
                      }`}
                    >
                      {accuracy}%
                    </span>
                  </div>

                  {/* Progress track */}
                  <div className="mt-3 h-2 w-full overflow-hidden rounded-full bg-slate-200">
                    <div
                      className={`h-full rounded-full ${
                        isStrong ? "bg-emerald-600" : isModerate ? "bg-amber-500" : "bg-red-500"
                      }`}
                      style={{ width: `${accuracy}%` }}
                    />
                  </div>

                  <p className="mt-2 text-[11px] text-slate-500">
                    {area.correctAnswers} of {area.questionsAttempted} correct
                  </p>
                </div>
              );
            })}
          </div>
        </section>
      )}
    </div>
  );
}