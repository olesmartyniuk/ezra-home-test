import type { TaskStatus, TaskPriority } from '../../types/task';

const statusConfig: Record<TaskStatus, { label: string; className: string }> = {
  Todo: { label: 'To Do', className: 'bg-gray-100 text-gray-600' },
  InProgress: { label: 'In Progress', className: 'bg-blue-100 text-blue-700' },
  Done: { label: 'Done', className: 'bg-green-100 text-green-700' },
};

const priorityConfig: Record<TaskPriority, { label: string; className: string }> = {
  Low: { label: 'Low', className: 'bg-slate-100 text-slate-600' },
  Medium: { label: 'Medium', className: 'bg-yellow-100 text-yellow-700' },
  High: { label: 'High', className: 'bg-red-100 text-red-700' },
};

export function TaskStatusBadge({ status }: { status: TaskStatus }) {
  const { label, className } = statusConfig[status];
  return (
    <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${className}`}>
      {label}
    </span>
  );
}

export function TaskPriorityBadge({ priority }: { priority: TaskPriority }) {
  const { label, className } = priorityConfig[priority];
  return (
    <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${className}`}>
      {label}
    </span>
  );
}
