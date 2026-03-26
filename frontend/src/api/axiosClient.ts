import axios from 'axios';

let authToken: string | null = null;
let onUnauthorized: (() => void) | null = null;

export function setAuthToken(token: string | null) {
  authToken = token;
}

export function setUnauthorizedHandler(handler: () => void) {
  onUnauthorized = handler;
}

export const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10_000,
});

axiosClient.interceptors.request.use((config) => {
  if (authToken) config.headers.Authorization = `Bearer ${authToken}`;
  return config;
});

axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      onUnauthorized?.();
      return Promise.reject(new Error('Session expired. Please sign in again.'));
    }
    const data = error.response?.data;
    const message =
      data?.title ??
      data?.detail ??
      error.message ??
      'An unexpected error occurred.';
    return Promise.reject(new Error(message));
  }
);
