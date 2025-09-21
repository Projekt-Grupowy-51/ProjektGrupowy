// Example usage of SignalRClientSingleton

// Import the singleton
import signalRClient from "./services/signalR/SignalRClientSingleton.js";

// Example: Initialize in your main app component or authentication context
export const initializeSignalR = (
  addNotification,
  getToken,
  isAuthenticated
) => {
  try {
    // Initialize the singleton with required dependencies
    signalRClient.initialize(addNotification, getToken, isAuthenticated);

    // Start the connection
    signalRClient.start().catch((error) => {
      console.error("Failed to start SignalR connection:", error);
    });
  } catch (error) {
    console.error("Failed to initialize SignalR:", error);
  }
};
