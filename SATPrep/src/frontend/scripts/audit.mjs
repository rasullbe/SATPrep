import { chromium } from "playwright-core";
import { mkdirSync, writeFileSync } from "node:fs";
import { join } from "node:path";

const OUT = process.env.SHOTS_DIR || join(process.env.LOCALAPPDATA || ".", "Temp", "opencode");
mkdirSync(OUT, { recursive: true });
const BASE = process.env.BASE_URL || "http://localhost:3000";
const EMAIL = process.env.SHOT_EMAIL || "ali1@gmail.com";
const PASSWORD = process.env.SHOT_PASSWORD || "Test123!";

function auditFunction() {
  const parseColor = (str) => {
    const m = /rgba?\(([\d.]+),\s*([\d.]+),\s*([\d.]+)(?:,\s*([\d.]+))?\)/.exec(str);
    if (!m) return null;
    return { r: +m[1], g: +m[2], b: +m[3], a: m[4] === undefined ? 1 : +m[4] };
  };
  const comp = (f, b) => ({ r: f.r * f.a + b.r * (1 - f.a), g: f.g * f.a + b.g * (1 - f.a), b: f.b * f.a + b.b * (1 - f.a) });
  const lin = (c) => (c <= 0.04045 ? c / 12.92 : Math.pow((c + 0.055) / 1.055, 2.4));
  const lum = (c) => 0.2126 * lin(c.r / 255) + 0.7152 * lin(c.g / 255) + 0.0722 * lin(c.b / 255);
  const ratio = (a, b) => {
    const l1 = Math.max(lum(a), lum(b));
    const l2 = Math.min(lum(a), lum(b));
    return (l1 + 0.05) / (l2 + 0.05);
  };
  const effectiveBg = (el) => {
    const chain = [];
    let node = el;
    while (node && node.nodeType === 1) {
      const c = parseColor(getComputedStyle(node).backgroundColor);
      if (c && c.a > 0) chain.push(c);
      node = node.parentElement;
    }
    let bg = { r: 255, g: 255, b: 255, a: 1 };
    for (let i = chain.length - 1; i >= 0; i--) bg = comp(chain[i], bg);
    return bg;
  };
  const visible = (el) => {
    const r = el.getBoundingClientRect();
    const s = getComputedStyle(el);
    return r.width > 0 && r.height > 0 && s.visibility !== "hidden" && +s.opacity > 0.01;
  };
  const ownText = (el) =>
    Array.from(el.childNodes)
      .filter((n) => n.nodeType === 3)
      .map((n) => n.textContent)
      .join("")
      .trim();

  const failures = [];
  const all = document.querySelectorAll("body *");
  for (const el of all) {
    if (!visible(el)) continue;
    const text = ownText(el);
    if (text.length < 2) continue;
    if (/^(?:[^\w...\p{L}])+$/u.test(text)) continue;
    const s = getComputedStyle(el);
    if (!s.color || s.color === "currentcolor") continue;
    const fg = parseColor(s.color);
    if (!fg) continue;
    const bg = effectiveBg(el);
    const r = ratio(fg, bg);
    const size = parseFloat(s.fontSize);
    const bold = parseInt(s.fontWeight) >= 700;
    const large = size >= 18.66 && (size >= 24 || bold);
    const need = large ? 3.0 : 4.5;
    if (r < need) {
      failures.push({
        tag: el.tagName.toLowerCase(),
        text: text.slice(0, 46),
        size: Math.round(size),
        bold,
        fg: s.color,
        bg: `rgb(${Math.round(bg.r)},${Math.round(bg.g)},${Math.round(bg.b)})`,
        ratio: +r.toFixed(2),
      });
    }
  }

  const stats = [];
  for (const grid of document.querySelectorAll("main .grid")) {
    for (const child of grid.children) {
      const r = child.getBoundingClientRect();
      const inner = child.firstElementChild;
      const pad = inner ? getComputedStyle(inner).padding : "";
      stats.push({
        cls: (child.className || "").toString(),
        top: Math.round(r.top),
        height: Math.round(r.height),
        width: Math.round(r.width),
        pad,
      });
    }
  }

  const emptySections = [];
  const off = (el) => el.getBoundingClientRect();
  for (const el of document.querySelectorAll("main section, main .grid > div")) {
    if (!visible(el)) continue;
    const r = off(el);
    if (r.height < 220) continue;
    const kids = Array.from(el.children).filter(visible);
    if (!kids.length) continue;
    const last = kids.reduce((a, b) => (off(b).bottom > off(a).bottom ? b : a));
    const slack = Math.round(r.bottom - off(last).bottom);
    if (slack > 80) {
      emptySections.push({
        cls: (el.className || "").toString(),
        h: Math.round(r.height),
        slackBelowLastChild: slack,
      });
    }
  }

  return { failures, stats, emptySections };
}

const report = [];
const describe = async (name, url, pg = page) => {
  await pg.goto(url, { waitUntil: "networkidle" });
  await pg.waitForTimeout(1200);
  const data = await pg.evaluate(auditFunction);
  report.push(`\n===== ${name} (${url}) =====`);
  report.push(`\n--- CONTRAST FAILURES (${data.failures.length}) ---`);
  for (const f of data.failures)
    report.push(`${f.ratio}  ${f.fg} on ${f.bg}  ${f.tag} ${f.size}px${f.bold ? " bold" : ""}  "${f.text}"`);
  if (!data.failures.length) report.push("(none)");
  report.push(`\n--- GRID-ITEM MEASUREMENTS ---`);
  for (const s of data.stats) report.push(`top=${s.top} h=${s.height} w=${s.width} pad="${s.pad}" cls="${s.cls}"`);
  report.push(`\n--- POSSIBLE EMPTY SECTIONS (slack > 80px) ---`);
  for (const e of data.emptySections) report.push(`h=${e.h} slack=${e.slackBelowLastChild} cls="${e.cls}"`);
  if (!data.emptySections.length) report.push("(none)");
};

const browser = await chromium.launch({ channel: "msedge", headless: true });
const page = await browser.newPage({ viewport: { width: 1440, height: 900 } });
page.setDefaultTimeout(15000);

await page.goto(`${BASE}/login`, { waitUntil: "networkidle" });
await page.fill("#email", EMAIL);
await page.fill("#password", PASSWORD);
await Promise.all([page.waitForURL("**/dashboard"), page.click("button[type=submit]")]);
await page.waitForTimeout(1500);

await describe("dashboard", `${BASE}/dashboard`);
await describe("quizzes", `${BASE}/quizzes`);
await describe("quiz-detail", `${BASE}/quizzes/1`);
await describe("history", `${BASE}/history`);
await describe("flashcards", `${BASE}/flashcards`);

try {
  const attempts = await page.evaluate(async () => {
    const res = await fetch("/api/quiz-attempts", { credentials: "include" });
    const data = await res.json();
    return Array.isArray(data) ? data.filter((a) => a.completedAt).map((a) => a.quizAttemptId) : [];
  });
  if (attempts.length > 0) await describe("results", `${BASE}/results/${attempts[0]}`);
} catch {}

try {
  const anon = await browser.newPage({ viewport: { width: 1440, height: 900 } });
  await describe("login", `${BASE}/login`, anon);
  await describe("register", `${BASE}/register`, anon);
  await anon.close();
} catch {}

await browser.close();
writeFileSync(join(OUT, "audit-report.txt"), report.join("\n"), "utf8");
console.log("audit written -> " + join(OUT, "audit-report.txt"));
console.log(report.join("\n").slice(0, 6000));