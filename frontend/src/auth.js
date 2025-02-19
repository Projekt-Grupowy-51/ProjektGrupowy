import httpClient from './httpClient';

class AuthService {
    constructor() {
        this.tokenRefreshTimeout = null;
        this.initializeTokenRefresh();
    }

    // Inicjalizacja odœwie¿ania tokena po prze³adowaniu strony
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
            throw new Error('Login failed');
        }
    }

    // Rejestracja
    async register(userData) {
        try {
            const response = await httpClient.post('/Auth/Register', userData);
            return response.data;
        } catch (error) {
            throw new Error('Registration failed');
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

    // Odœwie¿anie tokena
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

    // Zaplanuj odœwie¿enie tokena 1 minut przed wygaœniêciem
    scheduleTokenRefresh(expiresAt) {
        const now = new Date();
        const expiresIn = expiresAt.getTime() - now.getTime();
        const refreshTime = expiresIn - 1 * 60 * 1000; // 1 minut przed wygaœniêciem

        if (refreshTime <= 0) {
            // Jeœli zosta³o mniej ni¿ 1 minut, odœwie¿ natychmiast
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

    // Wyczyœæ odœwie¿enie tokena
    clearTokenRefresh() {
        if (this.tokenRefreshTimeout) {
            clearTimeout(this.tokenRefreshTimeout);
            this.tokenRefreshTimeout = null;
        }
    }
}

export default new AuthService();