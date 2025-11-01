import * as signalR from "@microsoft/signalr";

class SignalRService {
  connection = null;
  hubUrl =
    import.meta.env.VITE_SIGNALR_HUB_URL ?? "http://localhost:5000/signalr";
  startPromise = null; // Track ongoing start attempts
  stopPromise = null; // Track ongoing stop attempts

  constructor(addNotification, getToken, isAuthenticated) {
    this.addNotification = addNotification;
    this.getToken = getToken;
    this.isAuthenticated = isAuthenticated;

    console.log("SignalRService constructor called");
    console.log("SignalR hub URL:", this.hubUrl);

    if (!this.hubUrl) {
      throw new Error(
        "VITE_SIGNALR_HUB_URL environment variable is not defined. Please check your .env file."
      );
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: async () => {
          try {
            const token = this.getToken();
            console.log(
              "SignalR using token:",
              token ? "Token present" : "No token"
            );
            return token;
          } catch (error) {
            console.error("Failed to get token for SignalR:", error);
            return null;
          }
        },
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          // Exponential backoff: 0, 2, 10, 30 seconds, then stop
          if (retryContext.elapsedMilliseconds < 60000) {
            return Math.min(
              1000 * Math.pow(2, retryContext.previousRetryCount),
              30000
            );
          } else {
            // Stop reconnecting after 1 minute
            return null;
          }
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connection.onclose(async (error) => {
      if (error) {
        console.error("Connection closed with error:", error);
      } else {
        console.log("Connection closed gracefully.");
      }
    });

    this.connection.onreconnecting((error) => {
      console.warn("Connection lost. Reconnecting...", error);
    });

    this.connection.onreconnected((connectionId) => {
      console.log(
        "Connection reestablished. Connected with connectionId:",
        connectionId
      );
    });
  }

  async start() {
    // If a start is already in progress, wait for it
    if (this.startPromise) {
      console.log(
        "SignalR start already in progress. Waiting for it to complete..."
      );
      try {
        await this.startPromise;
        return;
      } catch (error) {
        // If the previous start failed, we'll try again
        console.log("Previous start attempt failed. Retrying...");
      }
    }

    const state = this.connection.state;

    if (state === signalR.HubConnectionState.Connected) {
      console.log("SignalR already connected.");
      return;
    }

    if (
      state === signalR.HubConnectionState.Connecting ||
      state === signalR.HubConnectionState.Reconnecting
    ) {
      console.log(
        `SignalR is already ${state}. Waiting for connection to establish...`
      );
      // Wait for the connection to establish
      try {
        await this.waitForConnection(5000);
        console.log("SignalR connection established after waiting.");
        return;
      } catch (error) {
        console.error("Failed to establish connection:", error);
        throw error;
      }
    }

    // If connection is in the process of disconnecting, wait for it to fully disconnect
    if (state === signalR.HubConnectionState.Disconnecting) {
      console.log(
        "Connection is disconnecting. Waiting for full disconnect..."
      );
      try {
        await this.waitForDisconnected(5000);
        console.log("Connection fully disconnected. Proceeding to start...");
      } catch (error) {
        console.error("Failed to wait for disconnection:", error);
        throw error;
      }
    }

    // Create a promise for this start attempt
    this.startPromise = (async () => {
      try {
        if (!this.isAuthenticated()) {
          throw new Error(
            "User is not authenticated. Cannot connect to SignalR hub."
          );
        }

        await this.connection.start();
        console.log("SignalR connection started successfully.");
      } catch (error) {
        console.error("Error starting SignalR connection:", error);
        if (
          error.message.includes("Unauthorized") ||
          error.message.includes("401")
        ) {
          console.error(
            "SignalR connection failed due to authorization. Check if token is valid."
          );
        }
        throw error;
      } finally {
        // Clear the promise after completion (success or failure)
        this.startPromise = null;
      }
    })();

    return this.startPromise;
  }

  async waitForConnection(timeoutMs = 5000) {
    const start = Date.now();
    while (
      this.connection.state !== signalR.HubConnectionState.Connected &&
      this.connection.state !== signalR.HubConnectionState.Disconnected
    ) {
      if (Date.now() - start > timeoutMs) {
        throw new Error("Timed out waiting for SignalR to connect.");
      }
      await new Promise((res) => setTimeout(res, 100));
    }

    if (this.connection.state === signalR.HubConnectionState.Disconnected) {
      throw new Error("Connection was disconnected while waiting.");
    }
  }

  async waitForDisconnected(timeoutMs = 5000) {
    const start = Date.now();
    while (this.connection.state !== signalR.HubConnectionState.Disconnected) {
      if (Date.now() - start > timeoutMs) {
        throw new Error("Timed out waiting for SignalR to disconnect.");
      }
      await new Promise((res) => setTimeout(res, 100));
    }
  }

  async stop() {
    // If a stop is already in progress, wait for it
    if (this.stopPromise) {
      console.log(
        "SignalR stop already in progress. Waiting for it to complete..."
      );
      return this.stopPromise;
    }

    // If already disconnected, nothing to do
    if (this.connection.state === signalR.HubConnectionState.Disconnected) {
      console.log("SignalR already disconnected.");
      return;
    }

    // Wait for any ongoing start to complete first
    if (this.startPromise) {
      console.log("Waiting for start operation to complete before stopping...");
      try {
        await this.startPromise;
      } catch (error) {
        // Start failed, so we can proceed with stop
        console.log("Start operation failed, proceeding with stop.");
      }
    }

    this.stopPromise = (async () => {
      try {
        await this.connection.stop();
        console.log("SignalR connection stopped.");
      } catch (error) {
        console.error("Error stopping SignalR connection:", error);
      } finally {
        this.stopPromise = null;
      }
    })();

    return this.stopPromise;
  }

  onMessage(methodName, callback) {
    try {
      this.connection.off(methodName);
      this.connection.on(methodName, callback);
    } catch (error) {
      console.error(`Error setting up SignalR message handler: ${error}`);
    }
  }
}

export default SignalRService;
