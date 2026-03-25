import { TaskCard } from './TaskCard';
import { Spinner } from '../ui/Spinner';
import { EmptyState } from '../ui/EmptyState';
import { ErrorBanner } from '../ui/ErrorBanner';
import { useTasks } from '../../hooks/useTasks';

export function TaskList() {
  const { state, fetchTasks } = useTasks();
  const { tasks, loading, error, filters } = state;

  if (loading && tasks.length === 0) {
    return (
      <div className="flex justify-center py-16">
        <Spinner />
      </div>
    );
  }

  if (error) {
    return (
      <ErrorBanner
        message={error}
        onDismiss={() => fetchTasks()}
      />
    );
  }

  if (tasks.length === 0) {
    const hasFilters = filters.status || filters.priority || filters.search;
    return (
      <EmptyState
        title={hasFilters ? 'No matching tasks' : 'No tasks yet'}
        description={
          hasFilters
            ? 'Try adjusting or clearing your filters.'
            : 'Click "Add Task" to create your first task.'
        }
      />
    );
  }

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {tasks.map((task) => (
        <TaskCard key={task.id} task={task} />
      ))}
    </div>
  );
}
