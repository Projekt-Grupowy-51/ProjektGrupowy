import axios from "axios";
import { startRequest, endRequest } from "./services/loadingService";

export const API_BASE_URL = "http://localhost:5000/api";

const httpClient = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
    headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
    },
});

httpClient.interceptors.request.use(
    config => {
        startRequest();
        return config;
    },
    error => {
        endRequest();
        return Promise.reject(error);
    }
);

httpClient.interceptors.response.use(
    response => {
        endRequest();
        return response;
    },
    async (error) => {
        endRequest();
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                await axios.post(`${API_BASE_URL}/Auth/RefreshToken`, {}, { withCredentials: true });
                return httpClient(originalRequest);
            } catch (refreshError) {
                if (window.location.pathname !== "/login" && window.location.pathname !== "/forbidden" && window.location.pathname !== "error") {
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
        }

        else if (error.response?.status >= 500) {
            window.location.href = "/error";
        }

        return Promise.reject(error);
    }
);

export default httpClient;
