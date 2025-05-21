import axios, {
  type AxiosInstance,
  AxiosError,
  type AxiosResponse,
  type InternalAxiosRequestConfig,
} from "axios";
import { startRequest, endRequest } from "../loading/LoadingService";

export const API_BASE_URL: string =
  import.meta.env.VITE_API_URL ??
  (import.meta.env.DEV ? "http://localhost:5000/api" : "");

interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  skipLoadingScreen?: boolean;
  _retry?: boolean;
}

class HttpClient {
  private static instance: AxiosInstance;

  private constructor() {}

  public static getInstance(): AxiosInstance {
    if (!HttpClient.instance) {
      HttpClient.instance = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true,
        timeout: 10000,
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
      });

      HttpClient.instance.interceptors.request.use(
        (config: CustomAxiosRequestConfig) => {
          if (!config.skipLoadingScreen) startRequest();
          return config;
        },
        (error: AxiosError) => {
          if (
            (error.config as CustomAxiosRequestConfig)?.skipLoadingScreen ===
            false
          ) {
            endRequest();
          }
          return Promise.reject(error);
        }
      );

      HttpClient.instance.interceptors.response.use(
        (response: AxiosResponse) => {
          const config = response.config as CustomAxiosRequestConfig;
          if (!config.skipLoadingScreen) endRequest();
          return response;
        },
        async (error: AxiosError) => {
          const config = error.config as CustomAxiosRequestConfig;
          if (!config?.skipLoadingScreen) endRequest();

          const originalRequest = config;

          if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
              await axios.post(
                `${API_BASE_URL}/Auth/RefreshToken`,
                {},
                { withCredentials: true }
              );
              return HttpClient.instance(originalRequest);
            } catch (refreshError) {
              if (
                !["/login", "/forbidden", "/error"].includes(
                  window.location.pathname
                )
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
          } else if (error.response?.status && error.response.status >= 500) {
            window.location.href = "/error";
          }

          return Promise.reject(error);
        }
      );
    }

    return HttpClient.instance;
  }
}

export default HttpClient.getInstance();
