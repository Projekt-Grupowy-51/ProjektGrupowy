import SignalRService from "./SignalRClient.js";

class SignalRClientSingleton {
  static instance = null;

  constructor() {
    // Prevent direct instantiation
    if (SignalRClientSingleton.instance) {
      return SignalRClientSingleton.instance;
    }

    this.signalRService = null;
    this.isInitialized = false;

    SignalRClientSingleton.instance = this;
  }

  /**
   * Get the singleton instance
   * @returns {SignalRClientSingleton}
   */
  static getInstance() {
    if (!SignalRClientSingleton.instance) {
      SignalRClientSingleton.instance = new SignalRClientSingleton();
    }
    return SignalRClientSingleton.instance;
  }

  /**
   * Initialize the SignalR service with required dependencies
   * @param {Function} addNotification - Function to add notifications
   * @param {Function} getToken - Function to get authentication token
   * @param {Function} isAuthenticated - Function to check authentication status
   */
  initialize(addNotification, getToken, isAuthenticated) {
    if (this.isInitialized) {
      console.warn("SignalR client is already initialized");
      return;
    }

    try {
      this.signalRService = new SignalRService(
        addNotification,
        getToken,
        isAuthenticated
      );
      this.isInitialized = true;
      console.log("SignalR client singleton initialized successfully");
    } catch (error) {
      console.error("Failed to initialize SignalR client singleton:", error);
      throw error;
    }
  }

  /**
   * Get the SignalR service instance
   * @returns {SignalRService}
   */
  getService() {
    if (!this.isInitialized || !this.signalRService) {
      throw new Error(
        "SignalR client singleton not initialized. Call initialize() first."
      );
    }
    return this.signalRService;
  }

  /**
   * Start the SignalR connection
   */
  async start() {
    const service = this.getService();
    return await service.start();
  }

  /**
   * Stop the SignalR connection
   */
  async stop() {
    const service = this.getService();
    return await service.stop();
  }

  /**
   * Register a message handler
   * @param {string} methodName - The hub method name to listen for
   * @param {Function} callback - The callback function to execute
   */
  onMessage(methodName, callback) {
    const service = this.getService();
    return service.onMessage(methodName, callback);
  }

  /**
   * Wait for the connection to be disconnected
   * @param {number} timeoutMs - Timeout in milliseconds
   */
  async waitForDisconnected(timeoutMs = 5000) {
    const service = this.getService();
    return await service.waitForDisconnected(timeoutMs);
  }

  /**
   * Get the current connection state
   */
  getConnectionState() {
    if (!this.isInitialized || !this.signalRService) {
      return null;
    }
    return this.signalRService.connection?.state;
  }

  /**
   * Check if the service is initialized
   * @returns {boolean}
   */
  isServiceInitialized() {
    return this.isInitialized;
  }

  /**
   * Reset the singleton instance (useful for testing)
   */
  static reset() {
    if (SignalRClientSingleton.instance?.signalRService) {
      SignalRClientSingleton.instance.signalRService
        .stop()
        .catch(console.error);
    }
    SignalRClientSingleton.instance = null;
  }
}

// Export the singleton instance
export default SignalRClientSingleton.getInstance();
