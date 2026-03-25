import { Modal } from '../ui/Modal';
import { TaskForm } from './TaskForm';
import { useTaskContext } from '../../context/TaskContext';
import type { TaskFormValues } from '../../hooks/useTaskForm';

export function TaskFormModal() {
  const { state, closeModal, createTask, updateTask } = useTaskContext();
  const { isModalOpen, editingTask } = state;

  async function handleSubmit(values: TaskFormValues) {
    const payload = {
      ...values,
      description: values.description || null,
      dueDate: values.dueDate ? `${values.dueDate}T00:00:00Z` : null,
    };

    if (editingTask) {
      await updateTask(editingTask.id, payload);
    } else {
      await createTask(payload);
    }
    closeModal();
  }

  return (
    <Modal
      isOpen={isModalOpen}
      onClose={closeModal}
      title={editingTask ? 'Edit Task' : 'New Task'}
    >
      <TaskForm
        initialValues={editingTask ?? undefined}
        onSubmit={handleSubmit}
        onCancel={closeModal}
      />
    </Modal>
  );
}
