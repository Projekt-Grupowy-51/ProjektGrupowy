import * as signalR from "@microsoft/signalr";

class SignalRService {
  connection = null;
  hubUrl = "http://localhost:5000/hub/app";

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connection.onclose(() => {
      console.log("Connection closed. Attempting to reconnect...");
      this.connection();
    });
  }

  async start() {
    try {
      await this.connection.start();
      console.log("SignalR connection started.");
    } catch (error) {
      console.error("Error starting SignalR connection:", error);
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

  async sendMessage(methodName, ...args) {
    try {
      await this.connection.invoke(methodName, ...args);
    } catch (error) {
      console.error(`Error sending message to SignalR hub: ${error}`);
    }
  }

  async onMessage(methodName, callback) {
    try {
      this.connection.on(methodName, callback);
    } catch (error) {
      console.error(`Error setting up SignalR message handler: ${error}`);
    }
  }
}

const signalRService = new SignalRService();
export default signalRService;
