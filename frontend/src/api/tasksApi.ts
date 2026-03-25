import { axiosClient } from './axiosClient';
import type { Task, CreateTaskPayload, UpdateTaskPayload, TaskFilters, TaskStatus } from '../types/task';

export const tasksApi = {
  getAll: (filters?: TaskFilters) => {
    // Strip empty string values so they don't appear as query params
    const params: Record<string, string> = {};
    if (filters) {
      Object.entries(filters).forEach(([key, val]) => {
        if (val !== '' && val !== undefined && val !== null) {
          params[key] = String(val);
        }
      });
    }
    return axiosClient.get<Task[]>('/tasks', { params }).then((r) => r.data);
  },

  getById: (id: number) =>
    axiosClient.get<Task>(`/tasks/${id}`).then((r) => r.data),

  create: (payload: CreateTaskPayload) =>
    axiosClient.post<Task>('/tasks', payload).then((r) => r.data),

  update: (id: number, payload: UpdateTaskPayload) =>
    axiosClient.put<Task>(`/tasks/${id}`, payload).then((r) => r.data),

  updateStatus: (id: number, status: TaskStatus) =>
    axiosClient.patch<Task>(`/tasks/${id}/status`, { status }).then((r) => r.data),

  delete: (id: number) => axiosClient.delete(`/tasks/${id}`),
};
