import axios from "axios";
import { startRequest, endRequest } from "./services/loadingService";

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const httpClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

httpClient.interceptors.request.use(
  (config) => {
    if (!config.skipLoadingScreen) startRequest();
    return config;
  },
  (error) => {
    if (!error.config?.skipLoadingScreen) endRequest();
    return Promise.reject(error);
  }
);

httpClient.interceptors.response.use(
  (response) => {
    if (!response.config.skipLoadingScreen) endRequest();
    return response;
  },
  async (error) => {
    if (!error.config?.skipLoadingScreen) endRequest();
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        await axios.post(
          `${API_BASE_URL}/Auth/RefreshToken`,
          {},
          { withCredentials: true }
        );
        return httpClient(originalRequest);
      } catch (refreshError) {
        if (
          window.location.pathname !== "/login" &&
          window.location.pathname !== "/forbidden" &&
          window.location.pathname !== "error"
        ) {
          window.location.href = "/login";
        }

        return Promise.reject(refreshError);
      }
    }

    if (window.location.pathname === "/login") {
      return Promise.reject(error);
    }

    if (error.response?.status === 403) {
      window.location.href = "/forbidden";
    } else if (error.response?.status >= 500) {
      window.location.href = "/error";
    }

    return Promise.reject(error);
  }
);

export default httpClient;
