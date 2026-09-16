"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import {
  getFlashcards,
  createFlashcard,
  reviewFlashcard,
  type FlashcardGetDto,
} from "@/lib/api";
import { EmptyState, LoadingSpinner, PageError } from "@/components/ui";

type Mode = "browse" | "study";

function isDue(nextReview: string | null, now: Date): boolean {
  if (!nextReview) return true;
  return new Date(nextReview) <= now;
}

function sortByNextReviewAsc(a: FlashcardGetDto, b: FlashcardGetDto): number {
  if (a.nextReview === null && b.nextReview === null) return 0;
  if (a.nextReview === null) return -1;
  if (b.nextReview === null) return 1;
  return new Date(a.nextReview).getTime() - new Date(b.nextReview).getTime();
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

/* ── Add Card Form ── */
function CreateForm({
  onCreated,
  onClose,
}: {
  onCreated: (card: FlashcardGetDto) => void;
  onClose: () => void;
}) {
  const [front, setFront] = useState("");
  const [back, setBack] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = useCallback(
    async (e: React.FormEvent) => {
      e.preventDefault();
      if (!front.trim() || !back.trim()) return;
      setSubmitting(true);
      setError(null);
      try {
        const created = await createFlashcard(front.trim(), back.trim());
        onCreated(created);
        setFront("");
        setBack("");
        onClose();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to create card");
      } finally {
        setSubmitting(false);
      }
    },
    [front, back, onCreated, onClose],
  );

  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="flex items-center justify-between border-b border-slate-100 pb-3">
          <div className="flex items-center gap-2">
            <span className="text-base">✏️</span>
            <h3 className="text-sm font-bold text-[#002b49]">Create SAT Practice Flashcard</h3>
          </div>
          <button type="button" onClick={onClose} className="text-xs font-semibold text-slate-400 hover:text-slate-700">
            Cancel ✕
          </button>
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
              Front (SAT Term / Formula / Prompt)
            </label>
            <textarea
              className="w-full rounded-xl border border-slate-300 p-3 text-sm focus:border-[#0077c8] focus:outline-none focus:ring-1 focus:ring-[#0077c8] min-h-[90px] resize-none"
              placeholder="e.g. Ubiquitous (adj.) or Area of a Sector"
              value={front}
              onChange={(e) => setFront(e.target.value)}
              autoFocus
            />
          </div>
          <div>
            <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
              Back (Definition / Solution / Context)
            </label>
            <textarea
              className="w-full rounded-xl border border-slate-300 p-3 text-sm focus:border-[#0077c8] focus:outline-none focus:ring-1 focus:ring-[#0077c8] min-h-[90px] resize-none"
              placeholder="e.g. Present, appearing, or found everywhere."
              value={back}
              onChange={(e) => setBack(e.target.value)}
            />
          </div>
        </div>

        {error && <p className="text-xs font-semibold text-red-600">{error}</p>}

        <div className="flex justify-end gap-2 border-t border-slate-100 pt-3">
          <button
            type="button"
            onClick={onClose}
            className="rounded-full border border-slate-300 px-4 py-1.5 text-xs font-bold text-slate-600 hover:bg-slate-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="rounded-full bg-[#0077c8] px-5 py-1.5 text-xs font-bold text-white shadow-xs hover:bg-[#005a9c] disabled:opacity-50"
            disabled={submitting || !front.trim() || !back.trim()}
          >
            {submitting ? "Saving…" : "Save Flashcard"}
          </button>
        </div>
      </form>
    </div>
  );
}

/* ── 3D Flashcard Study Engine ── */
function StudyView({
  cards,
  onComplete,
  onExit,
}: {
  cards: FlashcardGetDto[];
  onComplete: () => void;
  onExit: () => void;
}) {
  const [index, setIndex] = useState(0);
  const [flipped, setFlipped] = useState(false);
  const [sessionCards, setSessionCards] = useState<FlashcardGetDto[]>(cards);
  const [reviewing, setReviewing] = useState(false);

  const currentCard = sessionCards[index];
  const done = index >= sessionCards.length;

  const handleGrade = useCallback(
    async (quality: number) => {
      if (!currentCard || reviewing) return;
      setReviewing(true);
      try {
        const updated = await reviewFlashcard(currentCard.flashcardId, quality);
        setSessionCards((prev) => {
          const next = [...prev];
          next[index] = updated;
          return next;
        });
        setFlipped(false);
        setIndex((i) => i + 1);
      } catch {
        setFlipped(false);
        setIndex((i) => i + 1);
      } finally {
        setReviewing(false);
      }
    },
    [currentCard, index, reviewing],
  );

  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (reviewing) return;
      if (!flipped && (e.code === "Space" || e.key === "Enter")) {
        e.preventDefault();
        setFlipped(true);
      } else if (flipped && !done) {
        if (e.key === "1") handleGrade(0);
        else if (e.key === "2") handleGrade(2);
        else if (e.key === "3") handleGrade(4);
        else if (e.key === "4") handleGrade(5);
      }
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [flipped, done, reviewing, handleGrade]);

  if (done) {
    return (
      <div className="mx-auto max-w-xl rounded-2xl border border-slate-200 bg-white p-10 text-center shadow-md">
        <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-emerald-100 text-3xl">
          🎓
        </div>
        <h2 className="mt-4 text-2xl font-bold text-[#002b49]">Session Complete!</h2>
        <p className="mt-2 text-sm text-slate-600">
          You reviewed {cards.length} card{cards.length !== 1 ? "s" : ""} in this study interval.
        </p>
        <div className="mt-6">
          <button
            onClick={onComplete}
            className="rounded-full bg-[#0077c8] px-6 py-2.5 text-sm font-bold text-white shadow-xs hover:bg-[#005a9c]"
          >
            Return to Flashcard Deck
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div className="flex items-center justify-between">
        <button
          onClick={onExit}
          className="rounded-md px-2.5 py-1 text-xs font-semibold text-slate-500 hover:bg-slate-100"
        >
          ← Exit Session
        </button>
        <span className="rounded-full bg-slate-100 px-3 py-0.5 text-xs font-bold text-slate-700">
          Card {index + 1} of {sessionCards.length}
        </span>
      </div>

      {/* 3D Flip Card Container */}
      <div
        className="relative min-h-[320px] w-full cursor-pointer select-none rounded-2xl border border-slate-200 bg-white p-8 shadow-md transition-all hover:shadow-lg flex flex-col justify-between"
        onClick={() => !flipped && setFlipped(true)}
      >
        <div className="flex items-center justify-between border-b border-slate-100 pb-2">
          <span className="text-[11px] font-bold uppercase tracking-wider text-slate-400">
            {flipped ? "Definition & Answer" : "SAT Concept / Question"}
          </span>
          <span className="text-xs text-sky-600 font-semibold">
            {flipped ? "Click to flip back" : "Click card or press Space to reveal"}
          </span>
        </div>

        <div className="my-auto py-6 text-center">
          <p
            className={`whitespace-pre-wrap ${
              flipped
                ? "text-xl font-medium text-slate-800 leading-relaxed font-serif"
                : "text-2xl font-bold text-[#002b49]"
            }`}
          >
            {flipped ? currentCard.back : currentCard.front}
          </p>
        </div>

        <div className="border-t border-slate-100 pt-3 text-center text-xs text-slate-400">
          {flipped ? "Rate your recall quality below" : "Press Space or click to flip"}
        </div>
      </div>

      {/* SM-2 Recall Feedback Buttons */}
      {flipped && (
        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="mb-3 text-center text-xs font-semibold text-slate-600">
            How accurately did you recall this concept?
          </p>
          <div className="grid grid-cols-4 gap-2">
            <button
              onClick={() => handleGrade(0)}
              disabled={reviewing}
              className="flex flex-col items-center rounded-xl border border-red-200 bg-red-50 p-2.5 text-xs font-bold text-red-700 hover:bg-red-100 transition disabled:opacity-50"
            >
              <span>Again</span>
              <span className="text-[10px] text-red-500 font-normal mt-0.5">[Key 1]</span>
            </button>
            <button
              onClick={() => handleGrade(2)}
              disabled={reviewing}
              className="flex flex-col items-center rounded-xl border border-amber-200 bg-amber-50 p-2.5 text-xs font-bold text-amber-700 hover:bg-amber-100 transition disabled:opacity-50"
            >
              <span>Hard</span>
              <span className="text-[10px] text-amber-500 font-normal mt-0.5">[Key 2]</span>
            </button>
            <button
              onClick={() => handleGrade(4)}
              disabled={reviewing}
              className="flex flex-col items-center rounded-xl border border-sky-200 bg-sky-50 p-2.5 text-xs font-bold text-[#0077c8] hover:bg-sky-100 transition disabled:opacity-50"
            >
              <span>Good</span>
              <span className="text-[10px] text-sky-500 font-normal mt-0.5">[Key 3]</span>
            </button>
            <button
              onClick={() => handleGrade(5)}
              disabled={reviewing}
              className="flex flex-col items-center rounded-xl border border-emerald-200 bg-emerald-50 p-2.5 text-xs font-bold text-emerald-700 hover:bg-emerald-100 transition disabled:opacity-50"
            >
              <span>Easy</span>
              <span className="text-[10px] text-emerald-500 font-normal mt-0.5">[Key 4]</span>
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

/* ── Main Flashcards Hub ── */
export default function FlashcardsPage() {
  const [cards, setCards] = useState<FlashcardGetDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [mode, setMode] = useState<Mode>("browse");
  const [showCreate, setShowCreate] = useState(false);

  const loadCards = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getFlashcards();
      setCards(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load flashcards");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const data = await getFlashcards();
        if (!cancelled) setCards(data);
      } catch (err) {
        if (!cancelled) setError(err instanceof Error ? err.message : "Failed to load flashcards");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  const now = useMemo(() => new Date(), []);

  const dueCards = useMemo(
    () => cards.filter((c) => isDue(c.nextReview, now)).sort(sortByNextReviewAsc),
    [cards, now],
  );

  const notDueCards = useMemo(
    () => cards.filter((c) => !isDue(c.nextReview, now)).sort(sortByNextReviewAsc),
    [cards, now],
  );

  const handleCreated = useCallback((card: FlashcardGetDto) => {
    setCards((prev) => [card, ...prev]);
  }, []);

  if (loading) {
    return (
      <div className="flex h-96 flex-col items-center justify-center">
        <LoadingSpinner />
        <p className="mt-3 text-sm font-medium text-slate-500">Loading SAT Flashcard Deck…</p>
      </div>
    );
  }

  if (error) return <PageError message={error} onRetry={loadCards} />;

  if (mode === "study") {
    return (
      <StudyView
        cards={dueCards}
        onComplete={() => {
          setMode("browse");
          loadCards();
        }}
        onExit={() => setMode("browse")}
      />
    );
  }

  return (
    <div className="space-y-8 pb-12">
      {/* Header Banner */}
      <div className="rounded-2xl border border-slate-200 bg-white p-6 sm:p-8 shadow-sm">
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <div className="flex items-center gap-2">
              <span className="rounded-md bg-[#0077c8] px-2.5 py-0.5 text-[11px] font-bold tracking-wider text-white uppercase">
                Spaced Repetition
              </span>
              <span className="text-xs text-slate-500">SM-2 Memory Algorithm</span>
            </div>
            <h1 className="mt-2 text-2xl font-extrabold text-[#002b49] sm:text-3xl">
              SAT Vocabulary & Math Formulas
            </h1>
            <p className="mt-1 text-xs sm:text-sm text-slate-600">
              Retain high-frequency vocabulary and essential geometry/algebra formulas using spaced repetition.
            </p>
          </div>

          <div className="flex items-center gap-3">
            {dueCards.length > 0 && (
              <button
                onClick={() => setMode("study")}
                className="rounded-full bg-[#0077c8] px-5 py-2.5 text-xs font-bold text-white shadow-xs hover:bg-[#005a9c] transition"
              >
                Study Now ({dueCards.length} Due) →
              </button>
            )}
            <button
              onClick={() => setShowCreate((s) => !s)}
              className="rounded-full border border-slate-300 bg-white px-4 py-2.5 text-xs font-bold text-slate-700 hover:bg-slate-50 transition"
            >
              {showCreate ? "Close" : "+ New Flashcard"}
            </button>
          </div>
        </div>
      </div>

      {showCreate && (
        <CreateForm onCreated={handleCreated} onClose={() => setShowCreate(false)} />
      )}

      {cards.length === 0 ? (
        <EmptyState
          title="Flashcard Deck is Empty"
          message="Add your first SAT vocabulary word or math theorem to start studying."
          action={
            <button
              onClick={() => setShowCreate(true)}
              className="rounded-full bg-[#0077c8] px-5 py-2 text-xs font-bold text-white hover:bg-[#005a9c]"
            >
              + Create First Card
            </button>
          }
        />
      ) : (
        <div className="space-y-8">
          {/* Due Cards */}
          {dueCards.length > 0 && (
            <section className="space-y-3">
              <div className="flex items-center justify-between">
                <h3 className="text-xs font-bold uppercase tracking-wider text-red-600">
                  Ready for Review ({dueCards.length})
                </h3>
              </div>
              <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
                {dueCards.map((card) => (
                  <div
                    key={card.flashcardId}
                    className="rounded-xl border border-red-200 bg-white p-5 shadow-xs transition hover:border-red-400"
                  >
                    <p className="font-bold text-[#002b49] text-base">{card.front}</p>
                    <p className="mt-2 text-xs text-slate-600 line-clamp-2 leading-relaxed">{card.back}</p>
                    <div className="mt-4 border-t border-slate-100 pt-2 flex items-center justify-between text-[11px] text-slate-400">
                      <span>Created {formatDate(card.createdAt)}</span>
                      <span className="font-semibold text-red-600">Due now</span>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}

          {/* Not Due Cards */}
          {notDueCards.length > 0 && (
            <section className="space-y-3">
              <h3 className="text-xs font-bold uppercase tracking-wider text-slate-400">
                Scheduled for Later ({notDueCards.length})
              </h3>
              <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
                {notDueCards.map((card) => (
                  <div
                    key={card.flashcardId}
                    className="rounded-xl border border-slate-200 bg-white p-5 shadow-xs"
                  >
                    <p className="font-bold text-slate-800 text-base">{card.front}</p>
                    <p className="mt-2 text-xs text-slate-500 line-clamp-2 leading-relaxed">{card.back}</p>
                    <div className="mt-4 border-t border-slate-100 pt-2 flex items-center justify-between text-[11px] text-slate-400">
                      <span>Next review: {card.nextReview ? formatDate(card.nextReview) : "Scheduled"}</span>
                      <span className="text-emerald-600 font-medium">Retained ✓</span>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}
        </div>
      )}
    </div>
  );
}
