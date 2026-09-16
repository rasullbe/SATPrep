"use client";

import { type ReactNode, useState, useEffect } from "react";

export function Card({ className = "", children }: { className?: string; children: ReactNode }) {
  return (
    <div className={`rounded-xl border border-slate-200 bg-white p-6 shadow-sm ${className}`}>
      {children}
    </div>
  );
}

export function CardHover({ className = "", children }: { className?: string; children: ReactNode }) {
  return (
    <div
      className={`rounded-xl border border-slate-200 bg-white p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:border-[#0077c8] hover:shadow-md ${className}`}
    >
      {children}
    </div>
  );
}

export function CardTitle({
  className = "",
  children,
}: {
  className?: string;
  children: ReactNode;
}) {
  return <h3 className={`text-lg font-semibold text-slate-900 ${className}`}>{children}</h3>;
}

export function StatCard({
  label,
  value,
  hint,
  icon,
}: {
  label: string;
  value: string | number;
  hint?: string;
  icon?: ReactNode;
}) {
  return (
    <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
      <div className="flex items-center justify-between">
        <p className="text-xs font-semibold uppercase tracking-wider text-slate-500">{label}</p>
        {icon && <span className="text-slate-400">{icon}</span>}
      </div>
      <p className="mt-2 text-2xl font-bold tracking-tight text-[#002b49]">{value}</p>
      {hint ? <p className="mt-1 text-xs text-slate-500">{hint}</p> : null}
    </div>
  );
}

export function LoadingSpinner({ className = "" }: { className?: string }) {
  return (
    <div className={`flex items-center justify-center ${className}`} aria-label="Loading">
      <div className="h-7 w-7 animate-spin rounded-full border-[3px] border-slate-200 border-t-[#0077c8]" />
    </div>
  );
}

export function PageError({
  message,
  onRetry,
}: {
  message: string;
  onRetry?: () => void;
}) {
  return (
    <div className="flex flex-col items-center justify-center gap-4 py-16 text-center">
      <div className="flex h-12 w-12 items-center justify-center rounded-full bg-red-50 text-red-600">
        <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
      </div>
      <p className="text-sm font-medium text-slate-700">{message}</p>
      {onRetry ? (
        <button
          className="rounded-full border border-slate-300 bg-white px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
          onClick={onRetry}
        >
          Try again
        </button>
      ) : null}
    </div>
  );
}

export function EmptyState({
  title,
  message,
  action,
}: {
  title: string;
  message: string;
  action?: ReactNode;
}) {
  return (
    <div className="flex flex-col items-center justify-center gap-2 rounded-xl border border-dashed border-slate-300 bg-white py-16 text-center">
      <p className="text-base font-semibold text-slate-800">{title}</p>
      <p className="max-w-sm text-sm text-slate-500">{message}</p>
      {action && <div className="mt-4">{action}</div>}
    </div>
  );
}

export function ProgressBar({ value, className = "" }: { value: number; className?: string }) {
  const clamped = Math.max(0, Math.min(100, value));
  return (
    <div className={`h-2 w-full overflow-hidden rounded-full bg-slate-100 ${className}`}>
      <div
        className="h-full rounded-full bg-[#0077c8] transition-all duration-300"
        style={{ width: `${clamped}%` }}
      />
    </div>
  );
}

/* Official SAT Math Reference Formula Sheet Modal (as in Bluebook) */
export function FormulaSheetModal({
  isOpen,
  onClose,
}: {
  isOpen: boolean;
  onClose: () => void;
}) {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    if (isOpen) window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4 backdrop-blur-xs">
      <div className="relative max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-2xl bg-white shadow-2xl">
        <div className="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-slate-50 px-6 py-4">
          <div className="flex items-center gap-2">
            <span className="text-xl">📐</span>
            <h3 className="text-base font-bold text-[#002b49]">SAT Reference Sheet</h3>
          </div>
          <button
            onClick={onClose}
            className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-200 hover:text-slate-700"
            aria-label="Close"
          >
            ✕
          </button>
        </div>

        <div className="space-y-6 p-6 text-sm text-slate-700">
          <p className="text-xs text-slate-500 italic">
            Reference information provided on all SAT Math modules.
          </p>

          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Circle Area</p>
              <p className="mt-1 font-mono text-[#0077c8]">A = πr²</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Circumference</p>
              <p className="mt-1 font-mono text-[#0077c8]">C = 2πr</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Rectangle Area</p>
              <p className="mt-1 font-mono text-[#0077c8]">A = lw</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Triangle Area</p>
              <p className="mt-1 font-mono text-[#0077c8]">A = ½bh</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Pythagorean</p>
              <p className="mt-1 font-mono text-[#0077c8]">c² = a² + b²</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Rectangular Box</p>
              <p className="mt-1 font-mono text-[#0077c8]">V = lwh</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Cylinder Volume</p>
              <p className="mt-1 font-mono text-[#0077c8]">V = πr²h</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Sphere Volume</p>
              <p className="mt-1 font-mono text-[#0077c8]">V = ⁴⁄₃πr³</p>
            </div>
            <div className="rounded-lg border border-slate-200 p-3 text-center">
              <p className="font-semibold text-slate-900">Cone Volume</p>
              <p className="mt-1 font-mono text-[#0077c8]">V = ⅓πr²h</p>
            </div>
          </div>

          <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
            <h4 className="font-semibold text-slate-900">Special Right Triangles</h4>
            <div className="mt-2 grid grid-cols-1 gap-3 sm:grid-cols-2 text-xs">
              <div className="rounded-md bg-white p-3 border border-slate-200">
                <p className="font-bold text-[#002b49]">30° - 60° - 90° Triangle</p>
                <p className="mt-1 text-slate-600">Sides in ratio: <span className="font-mono text-[#0077c8]">x : x√3 : 2x</span></p>
                <p className="text-slate-500 text-[11px] mt-0.5">Opposite 30° is x, 60° is x√3, 90° is 2x</p>
              </div>
              <div className="rounded-md bg-white p-3 border border-slate-200">
                <p className="font-bold text-[#002b49]">45° - 45° - 90° Triangle</p>
                <p className="mt-1 text-slate-600">Sides in ratio: <span className="font-mono text-[#0077c8]">s : s : s√2</span></p>
                <p className="text-slate-500 text-[11px] mt-0.5">Legs are equal, hypotenuse is s√2</p>
              </div>
            </div>
          </div>

          <div className="rounded-lg bg-blue-50/70 p-3 text-xs text-blue-900 border border-blue-100">
            <p className="font-semibold">Key Facts:</p>
            <ul className="mt-1 list-inside list-disc space-y-0.5">
              <li>The number of degrees of arc in a circle is 360°.</li>
              <li>The number of radians of arc in a circle is 2π.</li>
              <li>The sum of the measures in degrees of the angles of a triangle is 180°.</li>
            </ul>
          </div>
        </div>

        <div className="border-t border-slate-200 bg-slate-50 px-6 py-3 text-right">
          <button
            onClick={onClose}
            className="rounded-full bg-[#0077c8] px-5 py-1.5 text-sm font-semibold text-white transition hover:bg-[#005a9c]"
          >
            Close Sheet
          </button>
        </div>
      </div>
    </div>
  );
}

/* Built-in Scientific Calculation Tool for SAT Examination */
export function CalculatorModal({
  isOpen,
  onClose,
}: {
  isOpen: boolean;
  onClose: () => void;
}) {
  const [expr, setExpr] = useState("");
  const [result, setResult] = useState<string | null>(null);

  const handleDigit = (val: string) => {
    setExpr((prev) => prev + val);
  };

  const handleClear = () => {
    setExpr("");
    setResult(null);
  };

  const handleCalculate = () => {
    try {
      // Safe arithmetic evaluator for numbers and basic operators
      const sanitized = expr.replace(/×/g, "*").replace(/÷/g, "/");
      if (!/^[\d\.\+\-\*\/\(\)\s\^]+$/.test(sanitized)) {
        setResult("Error");
        return;
      }
      // Replace ^ with **
      const py = sanitized.replace(/\^/g, "**");
      // Evaluate basic arithmetic
      const res = Function(`"use strict"; return (${py})`)();
      setResult(String(res));
    } catch {
      setResult("Error");
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4 backdrop-blur-xs">
      <div className="relative w-full max-w-sm overflow-hidden rounded-2xl bg-white shadow-2xl border border-slate-200">
        <div className="flex items-center justify-between border-b border-slate-200 bg-slate-50 px-5 py-3">
          <div className="flex items-center gap-2">
            <span className="text-lg">🧮</span>
            <h3 className="text-sm font-bold text-[#002b49]">SAT Calculator</h3>
          </div>
          <button
            onClick={onClose}
            className="rounded-lg p-1 text-slate-400 hover:bg-slate-200 hover:text-slate-700"
          >
            ✕
          </button>
        </div>

        <div className="p-4 space-y-3">
          <div className="rounded-xl border border-slate-200 bg-slate-50 p-3 text-right">
            <input
              type="text"
              value={expr}
              onChange={(e) => setExpr(e.target.value)}
              placeholder="0"
              className="w-full bg-transparent text-right font-mono text-xl text-slate-800 focus:outline-none"
            />
            {result !== null && (
              <p className="mt-1 font-mono text-lg font-bold text-[#0077c8]">= {result}</p>
            )}
          </div>

          <div className="grid grid-cols-4 gap-2 text-sm font-semibold">
            <button onClick={handleClear} className="rounded-lg bg-red-50 py-2.5 text-red-600 hover:bg-red-100">C</button>
            <button onClick={() => handleDigit("(")} className="rounded-lg bg-slate-100 py-2.5 hover:bg-slate-200">(</button>
            <button onClick={() => handleDigit(")")} className="rounded-lg bg-slate-100 py-2.5 hover:bg-slate-200">)</button>
            <button onClick={() => handleDigit("÷")} className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100">÷</button>

            <button onClick={() => handleDigit("7")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">7</button>
            <button onClick={() => handleDigit("8")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">8</button>
            <button onClick={() => handleDigit("9")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">9</button>
            <button onClick={() => handleDigit("×")} className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100">×</button>

            <button onClick={() => handleDigit("4")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">4</button>
            <button onClick={() => handleDigit("5")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">5</button>
            <button onClick={() => handleDigit("6")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">6</button>
            <button onClick={() => handleDigit("-")} className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100">-</button>

            <button onClick={() => handleDigit("1")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">1</button>
            <button onClick={() => handleDigit("2")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">2</button>
            <button onClick={() => handleDigit("3")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">3</button>
            <button onClick={() => handleDigit("+")} className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100">+</button>

            <button onClick={() => handleDigit("0")} className="col-span-2 rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">0</button>
            <button onClick={() => handleDigit(".")} className="rounded-lg border border-slate-200 py-2.5 hover:bg-slate-50">.</button>
            <button onClick={handleCalculate} className="rounded-lg bg-[#0077c8] py-2.5 text-white hover:bg-[#005a9c]">=</button>
          </div>
        </div>
      </div>
    </div>
  );
}

/* Digital SAT Official Section Directions Modal */
export function DirectionsModal({
  isOpen,
  onClose,
  title,
  timeMinutes,
}: {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  timeMinutes?: number | null;
}) {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4 backdrop-blur-xs">
      <div className="relative w-full max-w-lg overflow-hidden rounded-2xl bg-white shadow-2xl">
        <div className="flex items-center justify-between border-b border-slate-200 bg-slate-50 px-6 py-4">
          <h3 className="text-base font-bold text-[#002b49]">Directions: {title}</h3>
          <button
            onClick={onClose}
            className="rounded-lg p-1 text-slate-400 hover:bg-slate-200 hover:text-slate-700"
          >
            ✕
          </button>
        </div>
        <div className="space-y-4 p-6 text-sm text-slate-700 leading-relaxed">
          <p>
            The questions in this section address a number of important reading and writing or math skills. Each question has a single best answer.
          </p>
          <ul className="list-disc list-inside space-y-1 text-slate-600">
            <li>Read each question and its passage/context carefully before choosing your answer.</li>
            <li>All questions are multiple choice. Select the best answer for each question.</li>
            {timeMinutes && (
              <li>
                You have <strong>{timeMinutes} minutes</strong> to complete this section. You can flag questions to review before finishing.
              </li>
            )}
            <li>You may return to any question in this section at any time before submitting.</li>
          </ul>
        </div>
        <div className="border-t border-slate-200 bg-slate-50 px-6 py-3 text-right">
          <button
            onClick={onClose}
            className="rounded-full bg-[#0077c8] px-5 py-1.5 text-sm font-semibold text-white transition hover:bg-[#005a9c]"
          >
            Got it
          </button>
        </div>
      </div>
    </div>
  );
}