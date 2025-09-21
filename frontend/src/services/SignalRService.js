// import * as signalR from "@microsoft/signalr";

// class SignalRService {
//   connection = null;
//   hubUrl =
//     import.meta.env.VITE_SIGNALR_HUB_URL ??
//     "http://localhost:5000/notifications/hub/app";

//   constructor(addNotification, getToken, isAuthenticated) {
//     this.addNotification = addNotification;
//     this.getToken = getToken;
//     this.isAuthenticated = isAuthenticated;

//     console.log("SignalRService constructor called");
//     console.log("SignalR hub URL:", this.hubUrl);

//     if (!this.hubUrl) {
//       throw new Error(
//         "VITE_SIGNALR_HUB_URL environment variable is not defined. Please check your .env file."
//       );
//     }

//     this.connection = new signalR.HubConnectionBuilder()
//       .withUrl(this.hubUrl, {
//         accessTokenFactory: async () => {
//           try {
//             // Pobierz token bez automatycznego odświeżania
//             const token = this.getToken();
//             console.log(
//               "SignalR using token:",
//               token ? "Token present" : "No token"
//             );
//             return token;
//           } catch (error) {
//             console.error("Failed to get token for SignalR:", error);
//             return null;
//           }
//         },
//       })
//       .withAutomaticReconnect()
//       .configureLogging(signalR.LogLevel.Information)
//       .build();

//     this.connection.onclose(async () => {
//       console.log("Connection closed.");
//     });
//   }

//   async start() {
//     const state = this.connection.state;

//     if (state === signalR.HubConnectionState.Connected) {
//       console.log("SignalR already connected.");
//       return;
//     }

//     if (state === signalR.HubConnectionState.Connecting) {
//       console.log("SignalR is already connecting. Skipping wait.");
//       return;
//     }

//     try {
//       if (!this.isAuthenticated()) {
//         throw new Error(
//           "User is not authenticated. Cannot connect to SignalR hub."
//         );
//       }

//       await this.connection.start();
//       console.log("SignalR connection started successfully.");

//       this.pingInterval = setInterval(() => {
//         if (this.connection.state === signalR.HubConnectionState.Connected) {
//           this.connection
//             .invoke("Ping")
//             .catch((err) => console.warn("Ping failed:", err));
//         }
//       }, 30000); // every 30 seconds
//     } catch (error) {
//       console.error("Error starting SignalR connection:", error);
//       if (
//         error.message.includes("Unauthorized") ||
//         error.message.includes("401")
//       ) {
//         console.error(
//           "SignalR connection failed due to authorization. Check if token is valid."
//         );
//       }

//       throw error;
//     }
//   }

//   async waitForDisconnected(timeoutMs = 5000) {
//     const start = Date.now();
//     while (this.connection.state !== signalR.HubConnectionState.Disconnected) {
//       if (Date.now() - start > timeoutMs) {
//         throw new Error("Timed out waiting for SignalR to disconnect.");
//       }
//       await new Promise((res) => setTimeout(res, 100));
//     }
//   }

//   async stop() {
//     try {
//       await this.connection.stop();
//       console.log("SignalR connection stopped.");
//     } catch (error) {
//       console.error("Error stopping SignalR connection:", error);
//     }
//   }

//   onMessage(methodName, callback) {
//     try {
//       // Remove any existing handlers for this method name
//       this.connection.off(methodName);

//       // Add the new callback
//       this.connection.on(methodName, callback);
//     } catch (error) {
//       console.error(`Error setting up SignalR message handler: ${error}`);
//     }
//   }
// }

// export default SignalRService;
