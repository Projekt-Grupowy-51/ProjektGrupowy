import axios from "axios";
import { startRequest, endRequest } from "./services/loadingService";
import keycloak from "./keycloak";

export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ||
  (import.meta.env.DEV ? "http://localhost:5000/api/general-backend" : "");

const httpClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

httpClient.interceptors.request.use(
  (config) => {
    if (!config.skipLoadingScreen) startRequest();

    // Dodaj token Keycloak do nagłówków
    if (keycloak.token) {
      config.headers.Authorization = `Bearer ${keycloak.token}`;
    }

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
        // Spróbuj odnowić token w Keycloak
        const refreshed = await keycloak.updateToken(70);

        if (refreshed) {
          // Token został odnowiony, zaktualizuj nagłówek i ponów żądanie
          originalRequest.headers.Authorization = `Bearer ${keycloak.token}`;
          return httpClient(originalRequest);
        }
      } catch (refreshError) {
        console.error("Token refresh failed:", refreshError);

        // Jeśli odświeżenie nie powiodło się, przekieruj do logowania
        if (keycloak.authenticated) {
          keycloak.logout();
        }

        return Promise.reject(refreshError);
      }
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
