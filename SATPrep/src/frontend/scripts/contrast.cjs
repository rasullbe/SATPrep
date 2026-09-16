function lin(c) { c /= 255; return c <= 0.04045 ? c / 12.92 : Math.pow((c + 0.055) / 1.055, 2.4); }
function lum([r, g, b]) { return 0.2126 * lin(r) + 0.7152 * lin(g) + 0.0722 * lin(b); }
function ratio(a, b) { const l1 = Math.max(lum(a), lum(b)); const l2 = Math.min(lum(a), lum(b)); return (l1 + 0.05) / (l2 + 0.05); }
function parse(s) { const m = /#(\w\w)(\w\w)(\w\w)/.exec(s); return [parseInt(m[1], 16), parseInt(m[2], 16), parseInt(m[3], 16)]; }
const ivory = parse("#f7f3e9"), card = parse("#fffdf6"), gold100 = parse("#faf0d7");
const checks = [
  ["surface-400 #7a715f on ivory", "#7a715f", ivory],
  ["surface-400 #7a715f on card", "#7a715f", card],
  ["surface-500 #6c6351 on ivory", "#6c6351", ivory],
  ["surface-500 #6c6351 on card", "#6c6351", card],
  ["success #267e58 on card", "#267e58", card],
  ["danger #a63c36 on card", "#a63c36", card],
  ["warning #986709 on card", "#986709", card],
  ["gold-700 #7c5918 on ivory", "#7c5918", ivory],
  ["gold-700 #7c5918 on gold-100", "#7c5918", gold100],
  ["brand-700 #4338ca on card", "#4338ca", card],
  ["accent-600 #0d9488 on card", "#0d9488", card],
];
for (const [label, fg, bg] of checks) console.log(`${label.padEnd(32)} -> ${ratio(parse(fg), bg).toFixed(2)}`);