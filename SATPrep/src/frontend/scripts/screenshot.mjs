import { chromium } from "playwright-core";
import { mkdirSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const OUT = process.env.SHOTS_DIR || join(process.env.LOCALAPPDATA || ".", "Temp", "opencode", "shots");
mkdirSync(OUT, { recursive: true });

const BASE = process.env.BASE_URL || "http://localhost:3000";
const EMAIL = process.env.SHOT_EMAIL || "ali1@gmail.com";
const PASSWORD = process.env.SHOT_PASSWORD || "Test123!";

const browser = await chromium.launch({
  channel: "msedge",
  headless: true,
  args: ["--force-color-profile=srgb", "--hide-scrollbars"],
});
const page = await browser.newPage({ viewport: { width: 1440, height: 900 } });
page.setDefaultTimeout(15000);

const shot = async (name) => {
  await page.evaluate(() => document.fonts.ready);
  await page.screenshot({ path: join(OUT, `${name}.png`), fullPage: true });
  console.log(`saved ${name}.png`);
};

await page.goto(`${BASE}/login`, { waitUntil: "networkidle" });
await page.fill("#email", EMAIL);
await page.fill("#password", PASSWORD);
await Promise.all([page.waitForURL("**/dashboard"), page.click("button[type=submit]")]);
await page.waitForSelector("main");
await page.waitForTimeout(1600);
await shot("dashboard");

await page.goto(`${BASE}/quizzes`, { waitUntil: "networkidle" });
await page.waitForTimeout(900);
await shot("quizzes");

await page.goto(`${BASE}/quizzes/1`, { waitUntil: "networkidle" });
await page.waitForTimeout(700);
await shot("quiz-detail");

await page.goto(`${BASE}/history`, { waitUntil: "networkidle" });
await page.waitForTimeout(700);
await shot("history");

await page.goto(`${BASE}/flashcards`, { waitUntil: "networkidle" });
await page.waitForTimeout(500);
await shot("flashcards");

try {
  const attempts = await page.evaluate(async () => {
    const res = await fetch("/api/quiz-attempts", { credentials: "include" });
    const data = await res.json();
    return Array.isArray(data)
      ? data.filter((a) => a.completedAt).map((a) => a.quizAttemptId)
      : [];
  });
  if (attempts.length > 0) {
    await page.goto(`${BASE}/results/${attempts[0]}`, { waitUntil: "networkidle" });
    await page.waitForTimeout(700);
    await shot("results");
  }
} catch {
  console.log("no completed attempt, skipping results");
}

await browser.close();
console.log(`done -> ${OUT}`);