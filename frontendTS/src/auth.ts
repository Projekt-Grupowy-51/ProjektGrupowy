import httpClient from "./httpclient";

class AuthService {
    async login(username, password) {
        try {
            const response = await httpClient.post("/Auth/Login", {
                userName: username,
                password,
            });
            return response.data;
        } catch (error) {
            throw new Error("Login failed");
        }
    }

    async register(userData) {
        try {
            const response = await httpClient.post("/Auth/Register", userData);
            return response.data;
        } catch (error) {
            console.log(error);
            throw new Error(error.response.data?.message.toString() ?? error.message);
        }
    }

    async logout() {
        try {
            await httpClient.post("/Auth/Logout");
        } catch (error) {
            throw new Error("Logout failed");
        }
    }

    async checkAuth() {
        try {
            const response = await httpClient.get("/Auth/CheckAuth");
            return response.data;
        } catch (error) {
            throw new Error("Check auth failed");
        }
    }
}

export default new AuthService();
