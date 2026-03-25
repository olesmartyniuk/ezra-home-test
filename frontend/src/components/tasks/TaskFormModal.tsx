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
      dueDate: values.dueDate || null,
    };

    try {
      if (editingTask) {
        await updateTask(editingTask.id, payload);
      } else {
        await createTask(payload);
      }
      closeModal();
    } catch {
      // Error already dispatched to context (toast will show).
      // Swallow here to keep the modal open so the user doesn't lose their data.
    }
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
