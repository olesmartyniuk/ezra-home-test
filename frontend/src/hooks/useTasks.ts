import { useEffect } from 'react';
import { useTaskContext } from '../context/TaskContext';

/**
 * Convenience hook that auto-fetches tasks whenever filters change.
 * Components can use this hook instead of calling fetchTasks manually.
 */
export function useTasks() {
  const ctx = useTaskContext();
  const { fetchTasks, state } = ctx;

  useEffect(() => {
    fetchTasks();
  }, [state.filters, fetchTasks]);

  return ctx;
}
