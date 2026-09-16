"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useAuth } from "@/contexts/AuthContext";
import { ApiError } from "@/lib/api";
import { LoadingSpinner } from "@/components/ui";

export default function RegisterPage() {
  const { user, loading, register } = useAuth();
  const router = useRouter();

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (!loading && user) {
      router.replace("/dashboard");
    }
  }, [user, loading, router]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setFieldErrors({});

    if (password !== confirmPassword) {
      setFieldErrors({ confirmPassword: "Passwords do not match." });
      return;
    }

    setSubmitting(true);

    try {
      await register(name, email, password);
      router.push("/dashboard");
    } catch (err) {
      if (err instanceof ApiError) {
        setFormError(err.message || "Registration failed. Please check your details.");
        const errors: Record<string, string> = {};
        for (const fe of err.fieldErrors) {
          errors[fe.field] = fe.message;
        }
        setFieldErrors(errors);
      } else {
        setFormError(err instanceof Error ? err.message : "Registration failed. Please try again.");
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="flex min-h-screen bg-slate-50">
      {/* Left Branding Panel: Official College Board Navy */}
      <div className="hidden w-1/2 flex-col justify-between bg-[#002b49] p-12 text-white lg:flex">
        <div>
          <div className="flex items-center gap-2">
            <span className="flex h-8 w-8 items-center justify-center rounded-md bg-[#0077c8] font-bold text-white text-xs">
              SAT
            </span>
            <span className="text-xs font-bold uppercase tracking-widest text-sky-200">
              College Board Simulation
            </span>
          </div>
          <h2 className="mt-6 text-3xl font-extrabold tracking-tight text-white">
            Register Your SAT Candidate Profile
          </h2>
          <p className="mt-3 text-sm text-slate-300 max-w-md leading-relaxed">
            Create an account to begin diagnostic exams, track estimated 1600-scale scores, and practice with authentic College Board Bluebook testing tools.
          </p>
        </div>

        <div className="space-y-4 rounded-2xl bg-white/5 p-6 border border-white/10 backdrop-blur-xs text-xs text-slate-200">
          <div className="flex items-center gap-3">
            <span className="flex h-6 w-6 items-center justify-center rounded-full bg-sky-500/20 text-[#0077c8] font-bold">
              ✓
            </span>
            <span>Complete diagnostic testing with authentic pacing</span>
          </div>
          <div className="flex items-center gap-3">
            <span className="flex h-6 w-6 items-center justify-center rounded-full bg-sky-500/20 text-[#0077c8] font-bold">
              ✓
            </span>
            <span>Personalized domain mastery and skill gap analysis</span>
          </div>
          <div className="flex items-center gap-3">
            <span className="flex h-6 w-6 items-center justify-center rounded-full bg-sky-500/20 text-[#0077c8] font-bold">
              ✓
            </span>
            <span>Vocabulary roots & Math formula spaced repetition</span>
          </div>
        </div>

        <div className="text-xs text-slate-400">
          Join thousands of students preparing for test day success.
        </div>
      </div>

      {/* Right Form Panel: Clean Student Registration */}
      <div className="flex w-full items-center justify-center px-6 py-12 lg:w-1/2">
        <div className="w-full max-w-sm rounded-2xl border border-slate-200 bg-white p-8 shadow-sm sm:p-10">
          <div className="mb-6">
            <span className="rounded-md bg-sky-50 px-2.5 py-1 text-[11px] font-bold uppercase tracking-wider text-[#0077c8]">
              New Student Registration
            </span>
            <h1 className="mt-3 text-2xl font-extrabold text-[#002b49]">Create Account</h1>
            <p className="mt-1 text-xs text-slate-500">
              Set up your profile to start official SAT practice modules.
            </p>
          </div>

          {formError && (
            <div className="mb-4 rounded-xl border border-red-200 bg-red-50 p-3 text-xs font-semibold text-red-700">
              {formError}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-3.5" noValidate>
            <div>
              <label htmlFor="name" className="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">
                Full Name
              </label>
              <input
                id="name"
                type="text"
                autoComplete="name"
                required
                className="w-full rounded-xl border border-slate-300 p-2.5 text-sm text-slate-900 focus:border-[#0077c8] focus:outline-none focus:ring-1 focus:ring-[#0077c8]"
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Alex Morgan"
              />
              {fieldErrors.name && (
                <p className="mt-1 text-xs text-red-600 font-medium">{fieldErrors.name}</p>
              )}
            </div>

            <div>
              <label htmlFor="email" className="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">
                Email Address
              </label>
              <input
                id="email"
                type="email"
                autoComplete="email"
                required
                className="w-full rounded-xl border border-slate-300 p-2.5 text-sm text-slate-900 focus:border-[#0077c8] focus:outline-none focus:ring-1 focus:ring-[#0077c8]"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="student@example.com"
              />
              {fieldErrors.email && (
                <p className="mt-1 text-xs text-red-600 font-medium">{fieldErrors.email}</p>
              )}
            </div>

            <div>
              <label htmlFor="password" className="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">
                Password
              </label>
              <input
                id="password"
                type="password"
                autoComplete="new-password"
                required
                className="w-full rounded-xl border border-slate-300 p-2.5 text-sm text-slate-900 focus:border-[#0077c8] focus:outline-none focus:ring-1 focus:ring-[#0077c8]"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="At least 6 characters"
              />
              {fieldErrors.password && (
                <p className="mt-1 text-xs text-red-600 font-medium">{fieldErrors.password}</p>
              )}
            </div>

            <div>
              <label htmlFor="confirmPassword" className="block text-xs font-bold uppercase tracking-wider text-slate-600 mb-1">
                Confirm Password
              </label>
              <input
                id="confirmPassword"
                type="password"
                autoComplete="new-password"
                required
                className="w-full rounded-xl border border-slate-300 p-2.5 text-sm text-slate-900 focus:border-[#0077c8] focus:outline-none focus:ring-1 focus:ring-[#0077c8]"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                placeholder="Repeat password"
              />
              {fieldErrors.confirmPassword && (
                <p className="mt-1 text-xs text-red-600 font-medium">{fieldErrors.confirmPassword}</p>
              )}
            </div>

            <button
              type="submit"
              className="mt-2 flex w-full items-center justify-center gap-2 rounded-full bg-[#0077c8] py-3 text-sm font-bold text-white shadow-xs transition hover:bg-[#005a9c] disabled:opacity-50"
              disabled={submitting}
            >
              {submitting ? <LoadingSpinner className="h-4 w-4 text-white" /> : null}
              <span>{submitting ? "Creating account…" : "Register Candidate Profile"}</span>
            </button>
          </form>

          <div className="mt-6 border-t border-slate-100 pt-4 text-center text-xs text-slate-600">
            Already registered?{" "}
            <Link href="/login" className="font-bold text-[#0077c8] hover:underline">
              Sign in
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}