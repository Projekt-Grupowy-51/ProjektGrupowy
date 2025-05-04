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
    return Promise.reject(error);
    if (error.response?.status === 401) {
      window.location.href = "/login";
      return Promise.reject(error);
    }

    if (error.response?.status === 403) {
      window.location.href = "/forbidden";
      return Promise.reject(error);
    }

    window.location.href = "/error";
    return Promise.reject(error);
  }
);

export default httpClient;
