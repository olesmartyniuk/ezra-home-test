import { useEffect } from 'react';
import { useTaskContext } from '../../context/TaskContext';

export function Toast() {
  const { state, clearError } = useTaskContext();
  const { error } = state;

  useEffect(() => {
    if (!error) return;
    const timer = setTimeout(clearError, 5000);
    return () => clearTimeout(timer);
  }, [error, clearError]);

  if (!error) return null;

  return (
    <div
      role="alert"
      className="fixed top-4 right-4 z-50 flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 shadow-lg max-w-sm"
    >
      <svg className="mt-0.5 h-4 w-4 shrink-0" fill="currentColor" viewBox="0 0 20 20">
        <path
          fillRule="evenodd"
          d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
          clipRule="evenodd"
        />
      </svg>
      <span className="flex-1">{error}</span>
      <button
        onClick={clearError}
        className="text-red-400 hover:text-red-600 transition-colors"
        aria-label="Dismiss error"
      >
        <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
  );
}
