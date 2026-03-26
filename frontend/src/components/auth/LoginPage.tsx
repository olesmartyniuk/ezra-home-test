import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from '../../context/AuthContext';

export function LoginPage() {
  const { signIn, isLoading } = useAuth();

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="bg-white rounded-xl border shadow-sm p-8 flex flex-col items-center gap-6 w-full max-w-sm">
        <div className="flex items-center gap-2">
          <svg
            className="h-8 w-8 text-blue-600"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"
            />
          </svg>
          <h1 className="text-2xl font-bold text-gray-900">TaskFlow</h1>
        </div>

        <p className="text-gray-500 text-sm text-center">Sign in to manage your tasks</p>

        {isLoading ? (
          <p className="text-sm text-gray-500">Signing in…</p>
        ) : (
          <GoogleLogin
            onSuccess={(response) => {
              if (response.credential) signIn(response.credential);
            }}
            onError={() => {}}
            useOneTap
          />
        )}
      </div>
    </div>
  );
}
