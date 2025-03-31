import httpClient from './httpClient';

class AuthService {
    constructor() {
        this.tokenRefreshTimeout = null;
        this.initializeTokenRefresh();
    }

    // Inicjalizacja od�wie�ania tokena po prze�adowaniu strony
    initializeTokenRefresh() {
        const storedExpiresAt = localStorage.getItem('tokenExpiresAt');
        if (storedExpiresAt) {
            const expiresAt = new Date(storedExpiresAt);
            if (expiresAt > new Date()) {
                this.scheduleTokenRefresh(expiresAt);
            } else {
                localStorage.removeItem('tokenExpiresAt');
            }
        }
    }

    // Logowanie
    async login(username, password) {
        try {
            const response = await httpClient.post('/Auth/Login', { userName: username, password });
            const expiresAt = new Date(response.data.expiresAt);
            localStorage.setItem('tokenExpiresAt', expiresAt.toISOString());
            this.scheduleTokenRefresh(expiresAt);
            return response.data;
        } catch (error) {
            console.log(error.message);
            throw new Error('Login failed');
        }
    }

    // Rejestracja
    async register(userData) {
        try {
            console.log(userData);
            const response = await httpClient.post('/Auth/Register', userData);
            return response.data;
        } catch (error) {
            throw new Error(error.response.data?.message.toString() ?? error.message);
        }
    }

    async verifyToken() {
        try {
            const response = await httpClient.get('/Auth/VerifyToken');
            return response.data;
        } catch (error) {
            return { IsAuthenticated: false };
        }
    }

    // Wylogowanie
    async logout() {
        try {
            await httpClient.post('/Auth/Logout');
            this.clearTokenRefresh();
            localStorage.removeItem('tokenExpiresAt');
        } catch (error) {
            throw new Error('Logout failed');
        }
    }

    // Od�wie�anie tokena
    async refreshToken() {
        try {
            const response = await httpClient.post('/Auth/RefreshToken');
            const expiresAt = new Date(response.data.expiresAt);
            localStorage.setItem('tokenExpiresAt', expiresAt.toISOString());
            this.scheduleTokenRefresh(expiresAt);
            return response.data;
        } catch (error) {
            throw new Error('Token refresh failed');
        }
    }

    // Zaplanuj od�wie�enie tokena 1 minut przed wyga�ni�ciem
    scheduleTokenRefresh(expiresAt) {
        const now = new Date();
        const expiresIn = expiresAt.getTime() - now.getTime();
        const refreshTime = expiresIn - 1 * 60 * 1000; // 1 minut przed wyga�ni�ciem

        if (refreshTime <= 0) {
            // Je�li zosta�o mniej ni� 1 minut, od�wie� natychmiast
            this.refreshToken();
            return;
        }

        if (this.tokenRefreshTimeout) {
            clearTimeout(this.tokenRefreshTimeout);
        }

        this.tokenRefreshTimeout = setTimeout(() => {
            this.refreshToken();
        }, refreshTime);
    }

    // Wyczy�� od�wie�enie tokena
    clearTokenRefresh() {
        if (this.tokenRefreshTimeout) {
            clearTimeout(this.tokenRefreshTimeout);
            this.tokenRefreshTimeout = null;
        }
    }
}

export default new AuthService();