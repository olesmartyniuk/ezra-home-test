import { GoogleOAuthProvider } from '@react-oauth/google';
import { AuthProvider, useAuth } from './context/AuthContext';
import { TaskProvider } from './context/TaskContext';
import { PageLayout } from './components/layout/PageLayout';
import { TaskFilters } from './components/tasks/TaskFilters';
import { TaskList } from './components/tasks/TaskList';
import { TaskFormModal } from './components/tasks/TaskFormModal';
import { Toast } from './components/ui/Toast';
import { LoginPage } from './components/auth/LoginPage';

const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID;

function AuthenticatedApp() {
  const { user } = useAuth();

  if (!user) return <LoginPage />;

  return (
    <TaskProvider>
      <PageLayout>
        <div className="flex flex-col gap-6">
          <TaskFilters />
          <TaskList />
        </div>
        <TaskFormModal />
      </PageLayout>
      <Toast />
    </TaskProvider>
  );
}

export default function App() {
  return (
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <AuthProvider>
        <AuthenticatedApp />
      </AuthProvider>
    </GoogleOAuthProvider>
  );
}
