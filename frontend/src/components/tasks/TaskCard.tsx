import { useState } from 'react';
import type { Task, TaskStatus } from '../../types/task';
import { TaskStatusBadge, TaskPriorityBadge } from './TaskStatusBadge';
import { Button } from '../ui/Button';
import { formatDate, isOverdue, isDueToday } from '../../utils/dateFormatter';
import { useTaskContext } from '../../context/TaskContext';

interface TaskCardProps {
  task: Task;
}

const statusTransitions: Record<TaskStatus, TaskStatus> = {
  Todo: 'InProgress',
  InProgress: 'Done',
  Done: 'Todo',
};

const statusTransitionLabels: Record<TaskStatus, string> = {
  Todo: 'Start',
  InProgress: 'Complete',
  Done: 'Reopen',
};

export function TaskCard({ task }: TaskCardProps) {
  const { updateTaskStatus, deleteTask, openModal } = useTaskContext();
  const [isDeleting, setIsDeleting] = useState(false);
  const [isUpdatingStatus, setIsUpdatingStatus] = useState(false);

  const isDone = task.status === 'Done';
  const dueDateFormatted = formatDate(task.dueDate);
  const overdue = !isDone && isOverdue(task.dueDate);
  const dueToday = !isDone && isDueToday(task.dueDate);

  async function handleStatusChange() {
    setIsUpdatingStatus(true);
    try {
      await updateTaskStatus(task.id, statusTransitions[task.status]);
    } finally {
      setIsUpdatingStatus(false);
    }
  }

  async function handleDelete() {
    if (!window.confirm(`Delete "${task.title}"?`)) return;
    setIsDeleting(true);
    try {
      await deleteTask(task.id);
    } finally {
      setIsDeleting(false);
    }
  }

  return (
    <div className={`bg-white rounded-md border p-4 flex flex-col gap-3 ${isDone ? 'opacity-70' : ''}`}>
      <div className="flex items-start justify-between gap-2">
        <h3 className={`font-semibold text-gray-900 ${isDone ? 'line-through text-gray-500' : ''}`}>
          {task.title}
        </h3>
        <div className="flex items-center gap-1">
          <Button variant="ghost" size="sm" onClick={() => openModal(task)} aria-label="Edit task" className="p-1.5">
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
          </Button>
          <Button variant="ghost" size="sm" onClick={handleDelete} isLoading={isDeleting} aria-label="Delete task" className="p-1.5 text-red-500 hover:bg-red-50">
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </Button>
        </div>
      </div>

      {task.description && (
        <p className="text-sm text-gray-500">{task.description}</p>
      )}

      <div className="flex flex-wrap items-center gap-2">
        <TaskStatusBadge status={task.status} />
        <TaskPriorityBadge priority={task.priority} />
        {dueDateFormatted && (
          <span className={`text-xs font-medium ${overdue ? 'text-red-600' : dueToday ? 'text-amber-600' : 'text-gray-500'}`}>
            {overdue ? 'Overdue: ' : dueToday ? 'Due today: ' : 'Due: '}
            {dueDateFormatted}
          </span>
        )}
      </div>

      <div className="border-t pt-2">
        <Button variant="secondary" size="sm" onClick={handleStatusChange} isLoading={isUpdatingStatus} className="w-full">
          {statusTransitionLabels[task.status]}
        </Button>
      </div>
    </div>
  );
}
