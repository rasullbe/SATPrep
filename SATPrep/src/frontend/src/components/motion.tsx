"use client";

import { Children, type CSSProperties, type ReactNode } from "react";
import { useCountUp } from "@/hooks/useCountUp";

export function AnimatedNumber({
  value,
  delay = 0,
  duration = 950,
  format,
  className = "",
}: {
  value: number;
  delay?: number;
  duration?: number;
  format?: (value: number) => string;
  className?: string;
}) {
  const animated = useCountUp(value, duration, delay);
  const text = format ? format(animated) : Math.round(animated).toLocaleString();
  return <span className={`tabular ${className}`}>{text}</span>;
}

export function RadialGauge({
  value,
  size = 150,
  stroke = 11,
  color = "#d4a02b",
  track = "rgba(239,233,218,0.14)",
  delay = 250,
  children,
  className = "",
}: {
  value: number;
  size?: number;
  stroke?: number;
  color?: string;
  track?: string;
  delay?: number;
  children?: ReactNode;
  className?: string;
}) {
  const animated = useCountUp(value / 100, 1300, delay);
  const radius = (size - stroke) / 2;
  const circumference = 2 * Math.PI * radius;
  const dashOffset = circumference * (1 - animated);

  return (
    <div className={`relative ${className}`} style={{ width: size, height: size }}>
      <svg width={size} height={size} role="img" aria-hidden="true">
        <circle
          cx={size / 2}
          cy={size / 2}
          r={radius}
          fill="none"
          stroke={track}
          strokeWidth={stroke}
        />
        <circle
          cx={size / 2}
          cy={size / 2}
          r={radius}
          fill="none"
          stroke={color}
          strokeWidth={stroke}
          strokeLinecap="round"
          strokeDasharray={circumference}
          strokeDashoffset={dashOffset}
          transform={`rotate(-90 ${size / 2} ${size / 2})`}
          style={{ filter: `drop-shadow(0 2px 6px ${color}55)` }}
        />
      </svg>
      <div className="absolute inset-0 flex flex-col items-center justify-center">{children}</div>
    </div>
  );
}

export function AnimatedProgressBar({
  value,
  delay = 0,
  duration = 1000,
  className = "",
  barClassName = "",
}: {
  value: number;
  delay?: number;
  duration?: number;
  className?: string;
  barClassName?: string;
}) {
  const animated = useCountUp(value, duration, delay);
  const clamped = Math.max(0, Math.min(100, animated));
  return (
    <div className={`h-2 w-full overflow-hidden rounded-full bg-surface-200 ${className}`}>
      <div
        className={`h-full rounded-full bg-gradient-to-r from-brand-500 to-gold-400 ${barClassName}`}
        style={{ width: `${clamped}%` }}
      />
    </div>
  );
}

export function Stagger({
  children,
  step = 90,
  className = "",
}: {
  children: ReactNode;
  step?: number;
  className?: string;
}) {
  const items = Children.toArray(children);
  return (
    <div className={className}>
      {items.map((child, index) => (
        <div
          key={index}
          className="reveal"
          style={{ animationDelay: `${index * step}ms` } as CSSProperties}
        >
          {child}
        </div>
      ))}
    </div>
  );
}