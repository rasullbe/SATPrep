"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect, type ReactNode } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { LoadingSpinner } from "@/components/ui";

const NAV_ITEMS = [
  {
    href: "/dashboard",
    label: "My SAT Dashboard",
    icon: (
      <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.8" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
      </svg>
    ),
  },
  {
    href: "/quizzes",
    label: "Practice Tests",
    icon: (
      <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.8" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
      </svg>
    ),
  },
  {
    href: "/flashcards",
    label: "SAT Flashcards",
    icon: (
      <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.8" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
      </svg>
    ),
  },
  {
    href: "/history",
    label: "Score Reports",
    icon: (
      <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.8" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
      </svg>
    ),
  },
];

export default function AppShell({ children }: { children: ReactNode }) {
  const { user, loading, logout } = useAuth();
  const pathname = usePathname();
  const router = useRouter();

  // If taking a quiz, hide sidebar completely for authentic full-screen Bluebook exam feel!
  const isTakingQuiz = pathname.includes("/quizzes/") && pathname !== "/quizzes";

  useEffect(() => {
    if (!loading && !user) {
      router.push(`/login?from=${encodeURIComponent(pathname)}`);
    }
  }, [loading, user, router, pathname]);

  if (loading || !user) {
    return (
      <div className="flex h-screen items-center justify-center bg-[#001726]">
        <div className="flex flex-col items-center gap-3">
          <LoadingSpinner className="text-white" />
          <p className="text-sm font-medium tracking-wide text-slate-300">Loading your SAT Workspace…</p>
        </div>
      </div>
    );
  }

  // Full-screen mode for the exam interface
  if (isTakingQuiz) {
    return <div className="min-h-screen bg-[#f8fafc] text-slate-900">{children}</div>;
  }

  return (
    <div className="flex min-h-screen bg-[#f8fafc]">
      {/* College Board Style Navigation Sidebar */}
      <aside className="flex w-64 shrink-0 flex-col border-r border-slate-200 bg-[#002b49] text-white">
        {/* Brand Header */}
        <div className="flex h-16 items-center gap-3 border-b border-[#0a3d66] px-5">
          <div className="flex h-8 w-8 items-center justify-center rounded-md bg-[#0077c8] font-bold text-white shadow-xs">
            SAT
          </div>
          <div>
            <span className="text-xs font-semibold tracking-wider text-sky-200 uppercase">College Board</span>
            <h1 className="text-base font-bold tracking-tight text-white">Digital SATPrep</h1>
          </div>
        </div>

        {/* Navigation links */}
        <nav className="flex-1 space-y-1 p-3">
          {NAV_ITEMS.map((item) => {
            const active = pathname === item.href || (item.href !== "/dashboard" && pathname.startsWith(item.href));
            return (
              <Link
                key={item.href}
                href={item.href}
                className={`flex items-center gap-3 rounded-lg px-3.5 py-2.5 text-sm font-semibold transition-all ${
                  active
                    ? "bg-[#0077c8] text-white shadow-xs"
                    : "text-slate-300 hover:bg-[#0a3d66] hover:text-white"
                }`}
              >
                <span className={active ? "text-white" : "text-slate-400"}>{item.icon}</span>
                {item.label}
              </Link>
            );
          })}
        </nav>

        {/* User Card & Sign Out */}
        <div className="border-t border-[#0a3d66] p-4">
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-[#0077c8] text-xs font-bold text-white ring-2 ring-sky-300/40">
              {user.name
                .split(" ")
                .map((p) => p[0])
                .slice(0, 2)
                .join("")
                .toUpperCase()}
            </div>
            <div className="min-w-0 flex-1">
              <p className="truncate text-sm font-semibold text-white">{user.name}</p>
              <p className="truncate text-xs text-sky-200">{user.email}</p>
            </div>
          </div>

          <button
            onClick={logout}
            className="mt-3 flex w-full items-center justify-center gap-2 rounded-md border border-[#0a3d66] bg-[#002238] px-3 py-1.5 text-xs font-semibold text-slate-300 transition hover:bg-red-950/40 hover:text-red-300 hover:border-red-900/50"
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            Sign out
          </button>
        </div>
      </aside>

      {/* Main Content Area */}
      <main className="flex-1 overflow-y-auto">
        <div className="mx-auto max-w-6xl p-6 lg:p-10">{children}</div>
      </main>
    </div>
  );
}