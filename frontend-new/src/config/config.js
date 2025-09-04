// Configuration for the application
const config = {
  // Set to true to use mock services, false to use real API
  useMockServices: false,
  
  // API configuration
  api: {
    baseUrl: import.meta.env.VITE_API_BASE_URL || 
             (import.meta.env.DEV ? "http://localhost:5000/api" : ""),
    timeout: 10000
  },
  
  // UI configuration
  ui: {
    defaultPageSize: 10,
    debounceTime: 300,
    toastDuration: 5000
  },
  
  // Feature flags
  features: {
    enableNotifications: true,
    enableDarkMode: false,
    enableAdvancedSearch: false
  }
};

export { config };
export default config;
