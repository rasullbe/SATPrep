export class ApiError extends Error {
  status: number;
  fieldErrors: { field: string; message: string }[];

  constructor(status: number, message: string, fieldErrors: { field: string; message: string }[] = []) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.fieldErrors = fieldErrors;
  }
}

export class NetworkError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "NetworkError";
  }
}

export function isUnauthorizedError(err: unknown): boolean {
  return err instanceof ApiError && err.status === 401;
}

let csrfPromise: Promise<string> | null = null;

async function getCsrfToken(): Promise<string> {
  if (csrfPromise) return csrfPromise;

  csrfPromise = fetch("/api/auth/csrf", { credentials: "include" })
    .then((res) => {
      if (!res.ok) throw new Error("Failed to fetch CSRF token");
      const cookie = document.cookie
        .split("; ")
        .find((c) => c.startsWith("satprep_csrf="));
      return cookie ? decodeURIComponent(cookie.substring("satprep_csrf=".length)) : "";
    })
    .finally(() => {
      setTimeout(() => {
        csrfPromise = null;
      }, 5000);
    });

  return csrfPromise;
}

export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const { method = "GET", body, ...rest } = options;
  const isMutating = ["POST", "PUT", "PATCH", "DELETE"].includes(method);

  const headers: Record<string, string> = {
    ...((options.headers as Record<string, string>) || {}),
  };

  if (isMutating && body && typeof body === "string") {
    headers["Content-Type"] = "application/json";
  }

  if (isMutating) {
    try {
      const csrfToken = await getCsrfToken();
      if (csrfToken) headers["X-CSRF-Token"] = csrfToken;
    } catch {
      // CSRF fetch failed — proceed without token
    }
  }

  try {
    const response = await fetch(path, {
      method,
      body,
      headers,
      credentials: "include",
      ...rest,
    });

    if (response.status === 401 && !path.includes("/auth/")) {
      // Try token refresh
      try {
        const refreshRes = await fetch("/api/auth/refresh", {
          method: "POST",
          credentials: "include",
          headers: { "X-CSRF-Token": headers["X-CSRF-Token"] || "" },
        });

        if (refreshRes.ok) {
          return apiFetch<T>(path, options);
        }
      } catch {
        // Refresh failed — redirect to login
      }

      if (typeof window !== "undefined" && !window.location.pathname.includes("/login")) {
        window.location.href = `/login?from=${encodeURIComponent(window.location.pathname)}`;
      }
    }

    if (response.status === 403 && isMutating) {
      // CSRF token may have expired — re-fetch and retry once
      csrfPromise = null;
      const newToken = await getCsrfToken();
      if (newToken) {
        return apiFetch<T>(path, options);
      }
    }

    if (!response.ok) {
      let message = `Request failed with status ${response.status}`;
      let fieldErrors: { field: string; message: string }[] = [];

      try {
        const errorData = await response.json();
        message = errorData.message || message;
        if (errorData.errors) fieldErrors = errorData.errors;
      } catch {
        // Could not parse error body
      }

      throw new ApiError(response.status, message, fieldErrors);
    }

    if (response.status === 204) return undefined as T;

    return (await response.json()) as T;
  } catch (err) {
    if (err instanceof ApiError) throw err;
    throw new NetworkError(
      err instanceof Error ? err.message : "Network request failed. Please check your connection."
    );
  }
}

// === Types ===

export interface AuthSession {
  userId: number;
  name: string;
  email: string;
  role: string;
  accessTokenExpiresAt: string;
}

export interface UserGetDto {
  userId: number;
  name: string;
  email: string;
  role: string;
  createdAt: string;
  studyStreak: number;
  totalStudyMinutes: number;
  lastStudiedAt: string | null;
}

export interface QuizGetDto {
  quizId: number;
  title: string;
  description: string;
  createdById: number;
  createdAt: string;
  timeLimitMinutes: number | null;
  shuffleQuestions: boolean;
  isPublished: boolean;
  questionCount: number;
}

export interface QuizDetailGetDto {
  quizId: number;
  title: string;
  description: string;
  createdById: number;
  createdAt: string;
  timeLimitMinutes: number | null;
  shuffleQuestions: boolean;
  isPublished: boolean;
  questions: QuestionSummaryDto[];
}

export interface QuestionSummaryDto {
  questionId: number;
  text: string;
  difficulty: string;
}

export interface FlashcardCreateDto {
  front: string;
  back: string;
}

export interface FlashcardUpdateDto {
  front: string;
  back: string;
}

export interface QuizTakeQuestion {
  questionId: number;
  text: string;
  difficulty: string;
  choices: ChoiceGetDto[];
}

export interface QuizTakeDto {
  quizId: number;
  title: string;
  description: string;
  timeLimitMinutes: number | null;
  questionCount: number;
  questions: QuizTakeQuestion[];
}

export interface QuizAttemptGetDto {
  quizAttemptId: number;
  quizId: number;
  userId: number;
  startedAt: string;
  completedAt: string | null;
  score: number;
  pointsCorrect: number;
  totalQuestions: number;
  timeTakenSeconds: number | null;
  answers: AnswerResultGetDto[];
}

export interface AnswerResultGetDto {
  questionId: number;
  selectedChoiceId: number | null;
  isCorrect: boolean;
  answeredAt: string;
}

export interface QuizAttemptResultDto {
  quizAttemptId: number;
  quizId: number;
  quizTitle: string;
  score: number;
  pointsCorrect: number;
  totalQuestions: number;
  timeTakenSeconds: number | null;
  startedAt: string;
  completedAt: string;
  questions: QuestionResultDto[];
  sectionBreakdown: SectionScoreDto[];
}

export interface QuestionResultDto {
  questionId: number;
  text: string;
  difficulty: string;
  topicId: number;
  topicName: string;
  subjectName: string;
  choices: ChoiceGetDto[];
  selectedChoiceId: number | null;
  correctChoiceId: number | null;
  isCorrect: boolean;
}

export interface ChoiceGetDto {
  choiceId: number;
  text: string;
}

export interface SectionScoreDto {
  subjectName: string;
  sections: ScoreDetailDto[];
}

export interface ScoreDetailDto {
  name: string;
  correct: number;
  total: number;
  percentage: number;
}

export interface DashboardDto {
  user: UserStatsDto;
  availableQuizzes: QuizGetDto[];
  recentAttempts: QuizAttemptGetDto[];
  weakAreas: TopicAccuracyDto[];
}

export interface UserStatsDto {
  userId: number;
  name: string;
  studyStreak: number;
  totalStudyMinutes: number;
  totalSessions: number;
  totalQuestionsAnswered: number;
  lastStudiedAt: string | null;
  averageScore: number;
}

export interface TopicAccuracyDto {
  topicId: number;
  topicName: string;
  subjectId: number;
  subjectName: string;
  questionsAttempted: number;
  correctAnswers: number;
  accuracyPercentage: number;
}

export interface FlashcardGetDto {
  flashcardId: number;
  userId: number;
  front: string;
  back: string;
  createdAt: string;
  nextReview: string | null;
}

export interface SubjectGetDto {
  subjectId: number;
  name: string;
  topics: TopicGetDto[];
}

export interface TopicGetDto {
  topicId: number;
  subjectId: number;
  name: string;
}

// === API Functions ===

export async function getMe(): Promise<AuthSession> {
  return apiFetch<AuthSession>("/api/auth/me");
}

export async function login(email: string, password: string): Promise<AuthSession> {
  return apiFetch<AuthSession>("/api/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export async function register(name: string, email: string, password: string): Promise<AuthSession> {
  return apiFetch<AuthSession>("/api/auth/register", {
    method: "POST",
    body: JSON.stringify({ name, email, password }),
  });
}

export async function logout(): Promise<void> {
  return apiFetch<void>("/api/auth/logout", { method: "POST" });
}

export async function getDashboard(): Promise<DashboardDto> {
  return apiFetch<DashboardDto>("/api/stats/dashboard");
}

export async function getQuizzes(): Promise<QuizGetDto[]> {
  return apiFetch<QuizGetDto[]>("/api/quizzes/published");
}

export async function getQuizTakeData(quizId: number): Promise<QuizTakeDto> {
  return apiFetch<QuizTakeDto>(`/api/quizzes/take/${quizId}`);
}

export async function getQuizDetails(quizId: number): Promise<QuizDetailGetDto> {
  return apiFetch<QuizDetailGetDto>(`/api/quizzes/details/${quizId}`);
}

export async function startQuizAttempt(quizId: number): Promise<QuizAttemptGetDto> {
  return apiFetch<QuizAttemptGetDto>("/api/quiz-attempts", {
    method: "POST",
    body: JSON.stringify({ quizId }),
  });
}

export async function completeQuizAttempt(
  attemptId: number,
  answers: { questionId: number; selectedChoiceId: number | null }[],
  timeTakenSeconds: number
): Promise<QuizAttemptResultDto> {
  return apiFetch<QuizAttemptResultDto>(`/api/quiz-attempts/${attemptId}/complete`, {
    method: "POST",
    body: JSON.stringify({ timeTakenSeconds, answers }),
  });
}

export async function getAttempt(attemptId: number): Promise<QuizAttemptGetDto> {
  return apiFetch<QuizAttemptGetDto>(`/api/quiz-attempts/${attemptId}`);
}

export async function getAttempts(): Promise<QuizAttemptGetDto[]> {
  return apiFetch<QuizAttemptGetDto[]>("/api/quiz-attempts");
}

export async function getFlashcards(): Promise<FlashcardGetDto[]> {
  return apiFetch<FlashcardGetDto[]>("/api/flashcards");
}

export async function createFlashcard(front: string, back: string): Promise<FlashcardGetDto> {
  return apiFetch<FlashcardGetDto>("/api/flashcards", {
    method: "POST",
    body: JSON.stringify({ front, back }),
  });
}

export async function reviewFlashcard(flashcardId: number, quality: number): Promise<FlashcardGetDto> {
  return apiFetch<FlashcardGetDto>(`/api/flashcards/${flashcardId}/review`, {
    method: "POST",
    body: JSON.stringify({ quality }),
  });
}

export async function getSubjects(): Promise<SubjectGetDto[]> {
  return apiFetch<SubjectGetDto[]>("/api/subjects");
}