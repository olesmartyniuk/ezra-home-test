import { Button } from '../ui/Button';
import { useTaskContext } from '../../context/TaskContext';
import { useAuth } from '../../context/AuthContext';

export function Header() {
  const { openModal } = useTaskContext();
  const { user, signOut } = useAuth();

  return (
    <header className="bg-white border-b sticky top-0 z-40">
      <div className="max-w-6xl mx-auto px-4 sm:px-6 py-4 flex items-center justify-between">
        <div className="flex items-center gap-2">
          <svg className="h-7 w-7 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" />
          </svg>
          <h1 className="text-xl font-bold text-gray-900">ToDo List</h1>
        </div>

        <div className="flex items-center gap-3">
          {user && (
            <div className="flex items-center gap-2">
              {user.picture && (
                <img src={user.picture} alt={user.name} className="h-8 w-8 rounded-full" referrerPolicy="no-referrer" />
              )}
              <span className="text-sm text-gray-700 hidden sm:block">{user.name}</span>
            </div>
          )}
          <Button onClick={() => openModal()}>
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Add Task
          </Button>
          <Button variant="secondary" onClick={signOut}>Sign out</Button>
        </div>
      </div>
    </header>
  );
}
