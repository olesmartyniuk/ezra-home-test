import { useEffect, useRef } from 'react';
import { useTaskContext } from '../context/TaskContext';

/**
 * Convenience hook that auto-fetches tasks whenever filters change.
 * Components can use this hook instead of calling fetchTasks manually.
 */
export function useTasks() {
  const ctx = useTaskContext();
  const { fetchTasks, state } = ctx;
  const filtersRef = useRef(state.filters);

  useEffect(() => {
    filtersRef.current = state.filters;
    fetchTasks();
  }, [state.filters]);

  return ctx;
}
