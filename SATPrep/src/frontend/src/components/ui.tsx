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
  const [activeTab, setActiveTab] = useState<"all" | "2d" | "triangles" | "3d">("all");

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
      <div className="relative max-h-[92vh] w-full max-w-3xl overflow-hidden rounded-2xl bg-white shadow-2xl border border-slate-200 flex flex-col">
        {/* Modal Header */}
        <div className="flex items-center justify-between border-b border-slate-200 bg-slate-50 px-6 py-3.5">
          <div className="flex items-center gap-2.5">
            <span className="flex h-8 w-8 items-center justify-center rounded-lg bg-[#002b49] text-white text-base font-bold shadow-xs">
              📐
            </span>
            <div>
              <h3 className="text-base font-bold text-[#002b49] leading-none">SAT Math Reference Sheet</h3>
              <p className="text-[11px] text-slate-500 mt-0.5">Official College Board formula sheet provided during test</p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-200 hover:text-slate-700 transition"
            aria-label="Close"
          >
            ✕
          </button>
        </div>

        {/* Filter Navigation Tabs */}
        <div className="flex items-center gap-2 border-b border-slate-200 bg-white px-6 py-2 text-xs">
          <button
            onClick={() => setActiveTab("all")}
            className={`rounded-full px-3 py-1 font-semibold transition ${
              activeTab === "all" ? "bg-[#002b49] text-white" : "text-slate-600 hover:bg-slate-100"
            }`}
          >
            All Formulas
          </button>
          <button
            onClick={() => setActiveTab("2d")}
            className={`rounded-full px-3 py-1 font-semibold transition ${
              activeTab === "2d" ? "bg-[#0077c8] text-white" : "text-slate-600 hover:bg-slate-100"
            }`}
          >
            Area & Circumference
          </button>
          <button
            onClick={() => setActiveTab("triangles")}
            className={`rounded-full px-3 py-1 font-semibold transition ${
              activeTab === "triangles" ? "bg-[#0077c8] text-white" : "text-slate-600 hover:bg-slate-100"
            }`}
          >
            Special Triangles
          </button>
          <button
            onClick={() => setActiveTab("3d")}
            className={`rounded-full px-3 py-1 font-semibold transition ${
              activeTab === "3d" ? "bg-[#0077c8] text-white" : "text-slate-600 hover:bg-slate-100"
            }`}
          >
            3D Volume
          </button>
        </div>

        {/* Formulas Body */}
        <div className="flex-1 overflow-y-auto p-6 space-y-6 text-slate-800">
          {/* 1. 2D Area & Perimeter Grid */}
          {(activeTab === "all" || activeTab === "2d") && (
            <div>
              <h4 className="text-xs font-bold uppercase tracking-wider text-slate-400 mb-3">
                2D Geometry: Area & Circumference
              </h4>
              <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
                {/* Circle */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3.5 text-center">
                  <svg width="70" height="70" viewBox="0 0 100 100" className="text-[#0077c8]">
                    <circle cx="50" cy="50" r="38" fill="#eff8ff" stroke="currentColor" strokeWidth="2.5" />
                    <line x1="50" y1="50" x2="88" y2="50" stroke="#002b49" strokeWidth="2" strokeDasharray="3 2" />
                    <circle cx="50" cy="50" r="3" fill="#002b49" />
                    <text x="68" y="44" fontSize="14" fill="#002b49" fontWeight="bold" fontFamily="monospace">r</text>
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Circle</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">A = πr²</p>
                  <p className="font-mono text-[11px] text-slate-500">C = 2πr</p>
                </div>

                {/* Rectangle */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3.5 text-center">
                  <svg width="70" height="70" viewBox="0 0 100 100" className="text-[#0077c8]">
                    <rect x="15" y="25" width="70" height="50" fill="#eff8ff" stroke="currentColor" strokeWidth="2.5" rx="3" />
                    <text x="47" y="18" fontSize="13" fill="#002b49" fontWeight="bold" fontFamily="monospace">ℓ</text>
                    <text x="90" y="55" fontSize="13" fill="#002b49" fontWeight="bold" fontFamily="monospace">w</text>
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Rectangle</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">A = ℓw</p>
                  <p className="font-mono text-[11px] text-slate-500">P = 2ℓ + 2w</p>
                </div>

                {/* Triangle */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3.5 text-center">
                  <svg width="70" height="70" viewBox="0 0 100 100" className="text-[#0077c8]">
                    <polygon points="50,15 15,80 85,80" fill="#eff8ff" stroke="currentColor" strokeWidth="2.5" />
                    <line x1="50" y1="15" x2="50" y2="80" stroke="#002b49" strokeWidth="1.5" strokeDasharray="3 2" />
                    <rect x="50" y="72" width="8" height="8" fill="none" stroke="#002b49" strokeWidth="1" />
                    <text x="54" y="52" fontSize="12" fill="#002b49" fontWeight="bold" fontFamily="monospace">h</text>
                    <text x="47" y="95" fontSize="13" fill="#002b49" fontWeight="bold" fontFamily="monospace">b</text>
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Triangle</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">A = ½bh</p>
                </div>

                {/* Pythagorean Theorem */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3.5 text-center">
                  <svg width="70" height="70" viewBox="0 0 100 100" className="text-[#0077c8]">
                    <polygon points="20,20 20,80 80,80" fill="#eff8ff" stroke="currentColor" strokeWidth="2.5" />
                    <rect x="20" y="70" width="10" height="10" fill="none" stroke="#002b49" strokeWidth="1" />
                    <text x="8" y="55" fontSize="13" fill="#002b49" fontWeight="bold" fontFamily="monospace">a</text>
                    <text x="48" y="94" fontSize="13" fill="#002b49" fontWeight="bold" fontFamily="monospace">b</text>
                    <text x="56" y="46" fontSize="13" fill="#002b49" fontWeight="bold" fontFamily="monospace">c</text>
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Pythagorean</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">c² = a² + b²</p>
                </div>
              </div>
            </div>
          )}

          {/* 2. Special Right Triangles */}
          {(activeTab === "all" || activeTab === "triangles") && (
            <div>
              <h4 className="text-xs font-bold uppercase tracking-wider text-slate-400 mb-3">
                Special Right Triangles (Crucial for SAT Math)
              </h4>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                {/* 30-60-90 Triangle */}
                <div className="flex items-center gap-4 rounded-xl border border-slate-200 bg-white p-4 shadow-xs">
                  <svg width="110" height="110" viewBox="0 0 120 120" className="shrink-0 text-[#0077c8]">
                    <polygon points="25,20 25,100 95,100" fill="#eff8ff" stroke="currentColor" strokeWidth="2.5" />
                    <rect x="25" y="90" width="10" height="10" fill="none" stroke="#002b49" strokeWidth="1" />
                    <text x="32" y="38" fontSize="10" fill="#002b49" fontWeight="bold">30°</text>
                    <text x="70" y="94" fontSize="10" fill="#002b49" fontWeight="bold">60°</text>
                    <text x="50" y="114" fontSize="12" fill="#002b49" fontWeight="bold" fontFamily="monospace">x√3</text>
                    <text x="8" y="65" fontSize="12" fill="#002b49" fontWeight="bold" fontFamily="monospace">x</text>
                    <text x="68" y="55" fontSize="12" fill="#0077c8" fontWeight="bold" fontFamily="monospace">2x</text>
                  </svg>
                  <div>
                    <h5 className="font-bold text-[#002b49] text-sm">30° - 60° - 90° Triangle</h5>
                    <p className="mt-1 text-xs text-slate-600 leading-relaxed">
                      Side ratio: <span className="font-mono font-bold text-[#0077c8]">x : x√3 : 2x</span>
                    </p>
                    <ul className="mt-1.5 space-y-0.5 text-[11px] text-slate-500">
                      <li>• Opposite 30° = <strong className="text-slate-700">x</strong></li>
                      <li>• Opposite 60° = <strong className="text-slate-700">x√3</strong></li>
                      <li>• Hypotenuse (opposite 90°) = <strong className="text-slate-700">2x</strong></li>
                    </ul>
                  </div>
                </div>

                {/* 45-45-90 Triangle */}
                <div className="flex items-center gap-4 rounded-xl border border-slate-200 bg-white p-4 shadow-xs">
                  <svg width="110" height="110" viewBox="0 0 120 120" className="shrink-0 text-[#0077c8]">
                    <polygon points="25,25 25,100 100,100" fill="#eff8ff" stroke="currentColor" strokeWidth="2.5" />
                    <rect x="25" y="90" width="10" height="10" fill="none" stroke="#002b49" strokeWidth="1" />
                    <text x="32" y="45" fontSize="10" fill="#002b49" fontWeight="bold">45°</text>
                    <text x="75" y="94" fontSize="10" fill="#002b49" fontWeight="bold">45°</text>
                    <text x="56" y="114" fontSize="12" fill="#002b49" fontWeight="bold" fontFamily="monospace">s</text>
                    <text x="12" y="68" fontSize="12" fill="#002b49" fontWeight="bold" fontFamily="monospace">s</text>
                    <text x="70" y="56" fontSize="12" fill="#0077c8" fontWeight="bold" fontFamily="monospace">s√2</text>
                  </svg>
                  <div>
                    <h5 className="font-bold text-[#002b49] text-sm">45° - 45° - 90° Triangle</h5>
                    <p className="mt-1 text-xs text-slate-600 leading-relaxed">
                      Side ratio: <span className="font-mono font-bold text-[#0077c8]">s : s : s√2</span>
                    </p>
                    <ul className="mt-1.5 space-y-0.5 text-[11px] text-slate-500">
                      <li>• Both legs are equal = <strong className="text-slate-700">s</strong></li>
                      <li>• Hypotenuse = <strong className="text-slate-700">s√2</strong></li>
                      <li>• Isosceles right triangle</li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* 3. 3D Volume Formulas */}
          {(activeTab === "all" || activeTab === "3d") && (
            <div>
              <h4 className="text-xs font-bold uppercase tracking-wider text-slate-400 mb-3">
                3D Spatial Geometry: Volumes
              </h4>
              <div className="grid grid-cols-2 gap-4 sm:grid-cols-5">
                {/* Rectangular Solid */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3 text-center">
                  <svg width="60" height="55" viewBox="0 0 100 80" className="text-[#0077c8]">
                    <rect x="15" y="30" width="50" height="40" fill="#eff8ff" stroke="currentColor" strokeWidth="2" />
                    <polygon points="15,30 35,15 85,15 65,30" fill="#dbeafe" stroke="currentColor" strokeWidth="2" />
                    <polygon points="65,30 85,15 85,55 65,70" fill="#bfdbfe" stroke="currentColor" strokeWidth="2" />
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Box</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">V = ℓwh</p>
                </div>

                {/* Cylinder */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3 text-center">
                  <svg width="60" height="55" viewBox="0 0 100 80" className="text-[#0077c8]">
                    <ellipse cx="50" cy="20" rx="30" ry="10" fill="#dbeafe" stroke="currentColor" strokeWidth="2" />
                    <rect x="20" y="20" width="60" height="40" fill="#eff8ff" />
                    <line x1="20" y1="20" x2="20" y2="60" stroke="currentColor" strokeWidth="2" />
                    <line x1="80" y1="20" x2="80" y2="60" stroke="currentColor" strokeWidth="2" />
                    <ellipse cx="50" cy="60" rx="30" ry="10" fill="#eff8ff" stroke="currentColor" strokeWidth="2" />
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Cylinder</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">V = πr²h</p>
                </div>

                {/* Sphere */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3 text-center">
                  <svg width="60" height="55" viewBox="0 0 100 80" className="text-[#0077c8]">
                    <circle cx="50" cy="40" r="30" fill="#eff8ff" stroke="currentColor" strokeWidth="2" />
                    <ellipse cx="50" cy="40" rx="30" ry="8" fill="none" stroke="#002b49" strokeWidth="1.5" strokeDasharray="3 2" />
                    <line x1="50" y1="40" x2="80" y2="40" stroke="#002b49" strokeWidth="1.5" />
                    <text x="62" y="36" fontSize="11" fill="#002b49" fontWeight="bold">r</text>
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Sphere</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">V = ⁴⁄₃πr³</p>
                </div>

                {/* Cone */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3 text-center">
                  <svg width="60" height="55" viewBox="0 0 100 80" className="text-[#0077c8]">
                    <polygon points="50,15 20,60 80,60" fill="#eff8ff" />
                    <line x1="50" y1="15" x2="20" y2="60" stroke="currentColor" strokeWidth="2" />
                    <line x1="50" y1="15" x2="80" y2="60" stroke="currentColor" strokeWidth="2" />
                    <ellipse cx="50" cy="60" rx="30" ry="9" fill="#eff8ff" stroke="currentColor" strokeWidth="2" />
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Cone</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">V = ⅓πr²h</p>
                </div>

                {/* Pyramid */}
                <div className="flex flex-col items-center justify-between rounded-xl border border-slate-200 bg-slate-50/70 p-3 text-center">
                  <svg width="60" height="55" viewBox="0 0 100 80" className="text-[#0077c8]">
                    <polygon points="50,15 15,65 55,75" fill="#eff8ff" stroke="currentColor" strokeWidth="2" />
                    <polygon points="50,15 55,75 85,60" fill="#dbeafe" stroke="currentColor" strokeWidth="2" />
                  </svg>
                  <p className="mt-2 text-xs font-bold text-slate-800">Pyramid</p>
                  <p className="font-mono text-xs font-bold text-[#0077c8]">V = ⅓ℓwh</p>
                </div>
              </div>
            </div>
          )}

          {/* 4. Official Angle & Degree Facts */}
          <div className="rounded-xl border border-blue-200 bg-gradient-to-r from-blue-50/70 to-indigo-50/40 p-4 text-xs text-[#002b49]">
            <p className="font-bold flex items-center gap-1.5 text-sm mb-1.5">
              <span>💡</span> Official College Board Exam Reference Rules:
            </p>
            <ul className="grid grid-cols-1 gap-2 sm:grid-cols-3 mt-2 text-slate-700">
              <li className="flex items-start gap-1.5 bg-white p-2.5 rounded-lg border border-blue-100 shadow-2xs">
                <span className="font-bold text-[#0077c8]">1.</span>
                <span>The number of degrees of arc in a circle is <strong>360°</strong>.</span>
              </li>
              <li className="flex items-start gap-1.5 bg-white p-2.5 rounded-lg border border-blue-100 shadow-2xs">
                <span className="font-bold text-[#0077c8]">2.</span>
                <span>The number of radians of arc in a circle is <strong>2π</strong>.</span>
              </li>
              <li className="flex items-start gap-1.5 bg-white p-2.5 rounded-lg border border-blue-100 shadow-2xs">
                <span className="font-bold text-[#0077c8]">3.</span>
                <span>The sum of the measures in degrees of the angles of a triangle is <strong>180°</strong>.</span>
              </li>
            </ul>
          </div>
        </div>

        {/* Modal Footer */}
        <div className="border-t border-slate-200 bg-slate-50 px-6 py-3 text-right">
          <button
            onClick={onClose}
            className="rounded-full bg-[#0077c8] px-6 py-2 text-sm font-bold text-white shadow-xs transition hover:bg-[#005a9c] active:scale-98"
          >
            Close Sheet [Esc]
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
  const [history, setHistory] = useState<{ expr: string; result: string }[]>([]);

  const handleDigit = (val: string) => {
    setExpr((prev) => prev + val);
  };

  const handleClear = () => {
    setExpr("");
    setResult(null);
  };

  const handleBackspace = () => {
    setExpr((prev) => (prev.length > 0 ? prev.slice(0, -1) : ""));
  };

  const handleSqrt = () => {
    setExpr((prev) => {
      if (!prev) return "sqrt(";
      return `sqrt(${prev})`;
    });
  };

  const handleCalculate = () => {
    if (!expr.trim()) return;
    try {
      let sanitized = expr
        .replace(/×/g, "*")
        .replace(/÷/g, "/")
        .replace(/π/g, `(${Math.PI})`)
        .replace(/sqrt\(/g, "Math.sqrt(")
        .replace(/\^/g, "**");

      // Validate allowed characters
      if (!/^[\d\.\+\-\*\/\(\)\s\^Math\.sqrtPI]+$/.test(sanitized)) {
        setResult("Error");
        return;
      }

      // Safe arithmetic evaluator
      const res = Function(`"use strict"; return (${sanitized})`)();
      if (typeof res === "number" && !isNaN(res)) {
        const formatted = Number.isInteger(res) ? String(res) : parseFloat(res.toFixed(6)).toString();
        setResult(formatted);
        setHistory((prev) => [{ expr, result: formatted }, ...prev.slice(0, 3)]);
      } else {
        setResult("Error");
      }
    } catch {
      setResult("Error");
    }
  };

  // Global physical keyboard support when modal is open
  useEffect(() => {
    if (!isOpen) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        onClose();
      } else if (e.key === "Enter" || e.key === "=") {
        e.preventDefault();
        handleCalculate();
      } else if (e.key === "Backspace") {
        handleBackspace();
      } else if (/^[0-9\+\-\*\/\(\)\.\^]$/.test(e.key)) {
        let mapped = e.key;
        if (mapped === "*") mapped = "×";
        if (mapped === "/") mapped = "÷";
        setExpr((prev) => prev + mapped);
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, expr, onClose]);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4 backdrop-blur-xs">
      <div className="relative w-full max-w-sm overflow-hidden rounded-2xl bg-white shadow-2xl border border-slate-200">
        {/* Header */}
        <div className="flex items-center justify-between border-b border-slate-200 bg-slate-50 px-5 py-3.5">
          <div className="flex items-center gap-2">
            <span className="flex h-7 w-7 items-center justify-center rounded-lg bg-[#002b49] text-white text-sm font-bold shadow-xs">
              🧮
            </span>
            <div>
              <h3 className="text-sm font-bold text-[#002b49] leading-none">SAT Scientific Calculator</h3>
              <p className="text-[10px] text-slate-400 mt-0.5">Keyboard enabled (0-9, +, -, *, /, Enter)</p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="rounded-lg p-1 text-slate-400 hover:bg-slate-200 hover:text-slate-700 transition"
          >
            ✕
          </button>
        </div>

        <div className="p-4 space-y-3">
          {/* Display & Calculation Tape */}
          <div className="rounded-xl border border-slate-200 bg-slate-50 p-3 text-right">
            {history.length > 0 && (
              <div className="mb-1 text-[11px] font-mono text-slate-400 truncate">
                {history[0].expr} = {history[0].result}
              </div>
            )}
            <input
              type="text"
              value={expr}
              onChange={(e) => setExpr(e.target.value)}
              placeholder="0"
              className="w-full bg-transparent text-right font-mono text-2xl font-semibold text-slate-800 focus:outline-none"
            />
            {result !== null && (
              <p className="mt-1 font-mono text-xl font-bold text-[#0077c8]">= {result}</p>
            )}
          </div>

          {/* Keypad Grid */}
          <div className="grid grid-cols-4 gap-2 text-sm font-semibold">
            <button
              onClick={handleClear}
              className="rounded-lg bg-red-50 py-2.5 text-xs font-bold text-red-600 hover:bg-red-100 active:scale-98 transition"
            >
              C
            </button>
            <button
              onClick={handleBackspace}
              className="rounded-lg bg-slate-100 py-2.5 text-slate-700 hover:bg-slate-200 active:scale-98 transition"
            >
              ⌫
            </button>
            <button
              onClick={handleSqrt}
              className="rounded-lg bg-sky-50 py-2.5 font-mono text-[#0077c8] hover:bg-sky-100 active:scale-98 transition"
            >
              √x
            </button>
            <button
              onClick={() => handleDigit("÷")}
              className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100 active:scale-98 transition"
            >
              ÷
            </button>

            <button
              onClick={() => handleDigit("7")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              7
            </button>
            <button
              onClick={() => handleDigit("8")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              8
            </button>
            <button
              onClick={() => handleDigit("9")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              9
            </button>
            <button
              onClick={() => handleDigit("×")}
              className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100 active:scale-98 transition"
            >
              ×
            </button>

            <button
              onClick={() => handleDigit("4")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              4
            </button>
            <button
              onClick={() => handleDigit("5")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              5
            </button>
            <button
              onClick={() => handleDigit("6")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              6
            </button>
            <button
              onClick={() => handleDigit("-")}
              className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100 active:scale-98 transition"
            >
              -
            </button>

            <button
              onClick={() => handleDigit("1")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              1
            </button>
            <button
              onClick={() => handleDigit("2")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              2
            </button>
            <button
              onClick={() => handleDigit("3")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              3
            </button>
            <button
              onClick={() => handleDigit("+")}
              className="rounded-lg bg-sky-50 py-2.5 text-[#0077c8] hover:bg-sky-100 active:scale-98 transition"
            >
              +
            </button>

            <button
              onClick={() => handleDigit("(")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-700 hover:bg-slate-50 active:scale-98 transition"
            >
              (
            </button>
            <button
              onClick={() => handleDigit("0")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-800 hover:bg-slate-50 active:scale-98 transition"
            >
              0
            </button>
            <button
              onClick={() => handleDigit(")")}
              className="rounded-lg border border-slate-200 py-2.5 text-slate-700 hover:bg-slate-50 active:scale-98 transition"
            >
              )
            </button>
            <button
              onClick={() => handleDigit("^")}
              className="rounded-lg bg-slate-100 py-2.5 text-slate-700 hover:bg-slate-200 active:scale-98 transition font-mono"
            >
              xʸ
            </button>

            <button
              onClick={() => handleDigit(".")}
              className="rounded-lg border border-slate-200 py-2.5 font-bold hover:bg-slate-50 active:scale-98 transition"
            >
              .
            </button>
            <button
              onClick={() => handleDigit("π")}
              className="rounded-lg border border-slate-200 py-2.5 font-bold text-[#002b49] hover:bg-slate-50 active:scale-98 transition"
            >
              π
            </button>
            <button
              onClick={handleCalculate}
              className="col-span-2 rounded-lg bg-[#0077c8] py-2.5 text-white font-bold shadow-xs hover:bg-[#005a9c] active:scale-98 transition"
            >
              = Calculate
            </button>
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