import { TaskProvider } from './context/TaskContext';
import { PageLayout } from './components/layout/PageLayout';
import { TaskFilters } from './components/tasks/TaskFilters';
import { TaskList } from './components/tasks/TaskList';
import { TaskFormModal } from './components/tasks/TaskFormModal';

export default function App() {
  return (
    <TaskProvider>
      <PageLayout>
        <div className="flex flex-col gap-6">
          <TaskFilters />
          <TaskList />
        </div>
        <TaskFormModal />
      </PageLayout>
    </TaskProvider>
  );
}
