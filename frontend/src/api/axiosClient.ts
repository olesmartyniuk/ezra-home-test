import axios from 'axios';

export const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000/api',
  headers: { 'Content-Type': 'application/json' },
  timeout: 10_000,
});

// Normalize API errors into plain Error objects with a useful message
axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const data = error.response?.data;
    const message =
      data?.title ??
      data?.detail ??
      error.message ??
      'An unexpected error occurred.';
    return Promise.reject(new Error(message));
  }
);
