"use client";

import { useEffect, useState } from "react";

export function useCountUp(target: number, duration = 900, delay = 0): number {
  const [value, setValue] = useState(0);

  useEffect(() => {
    let cancelled = false;
    let raf = 0;
    let start: number | null = null;
    const clamped = Math.max(0, target);

    const timeoutId = window.setTimeout(() => {
      const tick = (now: number) => {
        if (cancelled) return;
        if (start === null) start = now;
        const progress = Math.min(1, (now - start) / duration);
        const eased = 1 - Math.pow(1 - progress, 3);
        setValue(clamped * eased);
        if (progress < 1) {
          raf = requestAnimationFrame(tick);
        } else {
          setValue(clamped);
        }
      };
      raf = requestAnimationFrame(tick);
    }, delay);

    return () => {
      cancelled = true;
      window.clearTimeout(timeoutId);
      cancelAnimationFrame(raf);
    };
  }, [target, duration, delay]);

  return value;
}