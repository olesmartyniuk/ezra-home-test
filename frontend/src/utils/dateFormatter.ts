/**
 * Formats an ISO date string to a human-readable date (e.g. "Mar 25, 2026").
 * Returns null if the value is null/undefined.
 */
export function formatDate(iso: string | null | undefined): string | null {
  if (!iso) return null;
  return new Date(iso).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  });
}

/**
 * Returns true if the given ISO date string represents a past date (overdue).
 */
export function isOverdue(iso: string | null | undefined): boolean {
  if (!iso) return false;
  return new Date(iso) < new Date(new Date().toDateString());
}

/**
 * Returns true if the given ISO date string is today.
 */
export function isDueToday(iso: string | null | undefined): boolean {
  if (!iso) return false;
  const due = new Date(iso).toDateString();
  const today = new Date().toDateString();
  return due === today;
}
