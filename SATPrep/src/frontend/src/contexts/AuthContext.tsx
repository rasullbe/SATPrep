"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";
import { getMe, login as apiLogin, logout as apiLogout, register as apiRegister } from "@/lib/api";
import {
  deleteCookie,
  SESSION_COOKIE,
} from "@/lib/auth";
import { useRouter } from "next/navigation";

export interface SessionUser {
  userId: number;
  name: string;
  email: string;
  role: string;
  accessTokenExpiresAt: string;
}

interface AuthContextValue {
  user: SessionUser | null;
  loading: boolean;
  refreshSession: () => Promise<SessionUser | null>;
  login: (email: string, password: string) => Promise<SessionUser>;
  register: (name: string, email: string, password: string) => Promise<SessionUser>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<SessionUser | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  const refreshSession = useCallback(async () => {
    try {
      const session = await getMe();
      setUser(session);
      return session;
    } catch {
      setUser(null);
      return null;
    }
  }, []);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const session = await getMe();
        if (!cancelled) setUser(session);
      } catch {
        if (!cancelled) setUser(null);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  const persist = useCallback((session: SessionUser) => {
    setUser(session);
    try {
      document.cookie = `${SESSION_COOKIE}=${encodeURIComponent(
        JSON.stringify(session)
      )}; path=/; max-age=172800; samesite=lax`;
    } catch {
      // Cookie persistence is best-effort
    }
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const session = await apiLogin(email, password);
      persist(session);
      return session;
    },
    [persist]
  );

  const register = useCallback(
    async (name: string, email: string, password: string) => {
      const session = await apiRegister(name, email, password);
      persist(session);
      return session;
    },
    [persist]
  );

  const logout = useCallback(async () => {
    try {
      await apiLogout();
    } catch {
      // Best-effort server logout
    }
    deleteCookie(SESSION_COOKIE);
    setUser(null);
    router.push("/login");
  }, [router]);

  return (
    <AuthContext.Provider value={{ user, loading, refreshSession, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within an AuthProvider");
  return ctx;
}