import axios from "axios";

export const API_BASE_URL = "http://localhost:5000/api";

const httpClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

httpClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Handle 401 Unauthorized
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        await axios.post(
          `${API_BASE_URL}/Auth/RefreshToken`,
          {},
          {
            withCredentials: true,
            headers: {
              "Content-Type": "application/json",
              Accept: "application/json",
            },
          }
        );
        return httpClient(originalRequest);
      } catch (refreshError) {
        return Promise.reject(refreshError);
      }
    }

    // Handle 403 Forbidden
    if (error.response?.status === 403 || error.response?.status === 401) {
      console.error(
        "Access denied: You do not have permission to access this resource."
      );
      // Optionally redirect to a custom 403 page
      window.location.href = "/forbidden";
    }

    return Promise.reject(error);
  }
);

export default httpClient;
