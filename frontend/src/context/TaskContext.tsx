import {
  createContext,
  useContext,
  useReducer,
  useCallback,
  type ReactNode,
} from 'react';
import { tasksApi } from '../api/tasksApi';
import type { Task, CreateTaskPayload, UpdateTaskPayload, TaskFilters, TaskStatus } from '../types/task';

// --- State ---

interface TaskState {
  tasks: Task[];
  loading: boolean;
  error: string | null;
  filters: TaskFilters;
  isModalOpen: boolean;
  editingTask: Task | null;
}

const initialState: TaskState = {
  tasks: [],
  loading: false,
  error: null,
  filters: { sortBy: 'createdAt', sortOrder: 'desc' },
  isModalOpen: false,
  editingTask: null,
};

// --- Actions ---

type Action =
  | { type: 'SET_LOADING'; payload: boolean }
  | { type: 'SET_ERROR'; payload: string | null }
  | { type: 'SET_TASKS'; payload: Task[] }
  | { type: 'ADD_TASK'; payload: Task }
  | { type: 'UPDATE_TASK'; payload: Task }
  | { type: 'DELETE_TASK'; payload: number }
  | { type: 'SET_FILTERS'; payload: Partial<TaskFilters> }
  | { type: 'OPEN_MODAL'; payload: Task | null }
  | { type: 'CLOSE_MODAL' };

function reducer(state: TaskState, action: Action): TaskState {
  switch (action.type) {
    case 'SET_LOADING':
      return { ...state, loading: action.payload };
    case 'SET_ERROR':
      return { ...state, error: action.payload, loading: false };
    case 'SET_TASKS':
      return { ...state, tasks: action.payload, loading: false, error: null };
    case 'ADD_TASK':
      return { ...state, tasks: [action.payload, ...state.tasks] };
    case 'UPDATE_TASK':
      return {
        ...state,
        tasks: state.tasks.map((t) => (t.id === action.payload.id ? action.payload : t)),
      };
    case 'DELETE_TASK':
      return { ...state, tasks: state.tasks.filter((t) => t.id !== action.payload) };
    case 'SET_FILTERS':
      return { ...state, filters: { ...state.filters, ...action.payload } };
    case 'OPEN_MODAL':
      return { ...state, isModalOpen: true, editingTask: action.payload };
    case 'CLOSE_MODAL':
      return { ...state, isModalOpen: false, editingTask: null };
    default:
      return state;
  }
}

// --- Context ---

interface TaskContextValue {
  state: TaskState;
  fetchTasks: () => Promise<void>;
  createTask: (data: CreateTaskPayload) => Promise<void>;
  updateTask: (id: number, data: UpdateTaskPayload) => Promise<void>;
  updateTaskStatus: (id: number, status: TaskStatus) => Promise<void>;
  deleteTask: (id: number) => Promise<void>;
  setFilters: (filters: Partial<TaskFilters>) => void;
  openModal: (task?: Task) => void;
  closeModal: () => void;
  clearError: () => void;
}

const TaskContext = createContext<TaskContextValue | null>(null);

export function TaskProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(reducer, initialState);

  const fetchTasks = useCallback(async () => {
    dispatch({ type: 'SET_LOADING', payload: true });
    try {
      const tasks = await tasksApi.getAll(state.filters);
      dispatch({ type: 'SET_TASKS', payload: tasks });
    } catch (err) {
      dispatch({ type: 'SET_ERROR', payload: (err as Error).message });      
    }
  }, [state.filters]);

  const createTask = useCallback(async (data: CreateTaskPayload) => {
    try {
      const task = await tasksApi.create(data);
      dispatch({ type: 'ADD_TASK', payload: task });
    } catch (err) {
      dispatch({ type: 'SET_ERROR', payload: (err as Error).message });
      throw err;
    }
  }, []);

  const updateTask = useCallback(async (id: number, data: UpdateTaskPayload) => {
    try {
      const task = await tasksApi.update(id, data);
      dispatch({ type: 'UPDATE_TASK', payload: task });
    } catch (err) {
      dispatch({ type: 'SET_ERROR', payload: (err as Error).message });
      throw err;
    }
  }, []);

  const updateTaskStatus = useCallback(async (id: number, status: TaskStatus) => {
    try {
      const task = await tasksApi.updateStatus(id, status);
      dispatch({ type: 'UPDATE_TASK', payload: task });
    } catch (err) {
      dispatch({ type: 'SET_ERROR', payload: (err as Error).message });
      throw err;
    }
  }, []);

  const deleteTask = useCallback(async (id: number) => {
    try {
      await tasksApi.delete(id);
      dispatch({ type: 'DELETE_TASK', payload: id });
    } catch (err) {
      dispatch({ type: 'SET_ERROR', payload: (err as Error).message });
      throw err;
    }
  }, []);

  const setFilters = useCallback((filters: Partial<TaskFilters>) => {
    dispatch({ type: 'SET_FILTERS', payload: filters });
  }, []);

  const openModal = useCallback((task?: Task) => {
    dispatch({ type: 'OPEN_MODAL', payload: task ?? null });
  }, []);

  const closeModal = useCallback(() => {
    dispatch({ type: 'CLOSE_MODAL' });
  }, []);

  const clearError = useCallback(() => {
    dispatch({ type: 'SET_ERROR', payload: null });
  }, []);

  return (
    <TaskContext.Provider
      value={{
        state,
        fetchTasks,
        createTask,
        updateTask,
        updateTaskStatus,
        deleteTask,
        setFilters,
        openModal,
        closeModal,
        clearError,
      }}
    >
      {children}
    </TaskContext.Provider>
  );
}

export function useTaskContext(): TaskContextValue {
  const ctx = useContext(TaskContext);
  if (!ctx) throw new Error('useTaskContext must be used within TaskProvider');
  return ctx;
}
