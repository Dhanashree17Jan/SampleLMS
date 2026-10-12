import fs from 'node:fs';
import assert from 'node:assert/strict';

// RFC 4180-style quoted CSV parser; no application, network or database access.
function parseCsv(text) {
  const rows = []; let row = [], value = '', quoted = false;
  for (let i = 0; i < text.length; i++) {
    const c = text[i];
    if (c === '"') {
      if (quoted && text[i + 1] === '"') { value += '"'; i++; }
      else quoted = !quoted;
    } else if (c === ',' && !quoted) { row.push(value); value = ''; }
    else if (c === '\n' && !quoted) {
      row.push(value.replace(/\r$/, ''));
      if (row.length > 1 || row[0] !== '') rows.push(row);
      row = []; value = '';
    }
    else value += c;
  }
  assert(!quoted, 'Unterminated CSV quote');
  if (value || row.length) { row.push(value); rows.push(row); }
  return rows;
}
const root = new URL('./', import.meta.url);
const [columns, ...rows] = parseCsv(fs.readFileSync(new URL('test-cases.csv', root), 'utf8'));
const expected = ['ID','Module','Requirements','Title','Role','Priority','Prerequisites','TestData','Steps','ExpectedResult','AutomationReference','Status','ActualResult','Evidence','DefectID'];
assert.deepEqual(columns, expected);
assert.equal(rows.length, 145);
const ids = new Set(); const mapped = new Set(); const modules = new Map();
for (const row of rows) {
  assert.equal(row.length, columns.length, 'CSV column count');
  const c = Object.fromEntries(columns.map((name, i) => [name, row[i]]));
  assert.match(c.ID, /^TC-[A-Z]+-\d{3}$/);
  assert(!ids.has(c.ID), `Duplicate ${c.ID}`); ids.add(c.ID);
  for (const field of columns.slice(0, 12)) assert(c[field].trim(), `${c.ID}: missing ${field}`);
  assert(['P0','P1','P2'].includes(c.Priority));
  assert.equal(c.Status, 'Not Run', `${c.ID}: initial status`);
  assert(!c.ActualResult && !c.Evidence && !c.DefectID, `${c.ID}: unexecuted evidence must be blank`);
  for (const requirement of c.Requirements.match(/\b(?:FR|AC)\d{2}\b/g) || []) mapped.add(requirement);
  const module = c.ID.split('-')[1].toLowerCase();
  modules.set(module, (modules.get(module) || 0) + 1);
  const doc = fs.readFileSync(new URL(`${module}.md`, root), 'utf8');
  assert(doc.includes(`## ${c.ID} — ${c.Title}`), `${c.ID}: missing Markdown definition`);
  assert(doc.includes(c.Steps) && doc.includes(c.ExpectedResult), `${c.ID}: CSV/Markdown mismatch`);
}
for (const [prefix, count] of [['FR',21],['AC',23]]) {
  for (let i = 1; i <= count; i++) assert(mapped.has(prefix + String(i).padStart(2, '0')), `Missing ${prefix}${i}`);
}
assert.equal(modules.size, 14);
console.log(`Document validation passed: ${rows.length} unique cases, ${modules.size} modules, all FR01–FR21/AC01–AC23 mapped; every case Not Run.`);
