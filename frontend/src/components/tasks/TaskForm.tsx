import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { taskSchema, type TaskFormValues } from '../../hooks/useTaskForm';
import type { Task } from '../../types/task';
import { Input } from '../ui/Input';
import { Select } from '../ui/Select';
import { Textarea } from '../ui/Textarea';
import { Button } from '../ui/Button';

interface TaskFormProps {
  initialValues?: Task;
  onSubmit: (values: TaskFormValues) => Promise<void>;
  onCancel: () => void;
}

const statusOptions = [
  { value: 'Todo', label: 'To Do' },
  { value: 'InProgress', label: 'In Progress' },
  { value: 'Done', label: 'Done' },
];

const priorityOptions = [
  { value: 'Low', label: 'Low' },
  { value: 'Medium', label: 'Medium' },
  { value: 'High', label: 'High' },
];

function toDateInputValue(iso: string | null | undefined): string {
  if (!iso) return '';
  return iso.split('T')[0];
}

export function TaskForm({ initialValues, onSubmit, onCancel }: TaskFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<TaskFormValues>({
    resolver: zodResolver(taskSchema),
    defaultValues: {
      title: initialValues?.title ?? '',
      description: initialValues?.description ?? '',
      status: initialValues?.status ?? 'Todo',
      priority: initialValues?.priority ?? 'Medium',
      dueDate: toDateInputValue(initialValues?.dueDate),
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4" noValidate>
      <Input
        id="title"
        label="Title *"
        placeholder="What needs to be done?"
        error={errors.title?.message}
        {...register('title')}
      />

      <Textarea
        id="description"
        label="Description"
        placeholder="Optional details..."
        error={errors.description?.message}
        {...register('description')}
      />

      <div className="grid grid-cols-2 gap-3">
        <Select
          id="status"
          label="Status"
          options={statusOptions}
          error={errors.status?.message}
          {...register('status')}
        />
        <Select
          id="priority"
          label="Priority"
          options={priorityOptions}
          error={errors.priority?.message}
          {...register('priority')}
        />
      </div>

      <Input
        id="dueDate"
        type="date"
        label="Due Date"
        error={errors.dueDate?.message}
        {...register('dueDate')}
      />

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          {initialValues ? 'Save Changes' : 'Create Task'}
        </Button>
      </div>
    </form>
  );
}
