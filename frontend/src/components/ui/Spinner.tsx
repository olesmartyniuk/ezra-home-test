export function Spinner({ className = '' }: { className?: string }) {
  return (
    <div
      role="status"
      aria-label="Loading"
      className={`h-8 w-8 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin ${className}`}
    />
  );
}
