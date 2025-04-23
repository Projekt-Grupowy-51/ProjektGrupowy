import * as signalR from "@microsoft/signalr";
import settings from "../config/settings.json";

class SignalRService {
  connection = null;
  hubUrl = settings.signalR.url;

  constructor(addNotification) {
    this.addNotification = addNotification;
    console.log("SignalRService constructor called");
    console.log("SignalR hub URL:", this.hubUrl);
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect() // 👈 this!
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connection.onclose(async () => {
      console.log("Connection closed.");
    });
  }

  async start() {
    const state = this.connection.state;

    if (state !== signalR.HubConnectionState.Disconnected) {
      console.log(`SignalR is in state: ${state}. Waiting for disconnect...`);
      await this.waitForDisconnected();
    }

    try {
      await this.connection.start();
      console.log("SignalR connection started.");
    } catch (error) {
      console.error("Error starting SignalR connection:", error);
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
    try {
      await this.connection.stop();
      console.log("SignalR connection stopped.");
    } catch (error) {
      console.error("Error stopping SignalR connection:", error);
    }
  }

  onMessage(methodName, callback) {
    try {
      // Remove any existing handlers for this method name
      this.connection.off(methodName);

      // Add the new callback
      this.connection.on(methodName, callback);
    } catch (error) {
      console.error(`Error setting up SignalR message handler: ${error}`);
    }
  }
}

export default SignalRService;
