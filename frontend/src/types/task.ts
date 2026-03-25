export type TaskStatus = 'Todo' | 'InProgress' | 'Done';
export type TaskPriority = 'Low' | 'Medium' | 'High';

export interface Task {
  id: number;
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskPayload {
  title: string;
  description?: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate?: string | null;
}

export type UpdateTaskPayload = CreateTaskPayload;

export interface TaskFilters {
  status?: TaskStatus | '';
  priority?: TaskPriority | '';
  sortBy?: 'createdAt' | 'dueDate' | 'priority' | 'status';
  sortOrder?: 'asc' | 'desc';
  search?: string;
}
