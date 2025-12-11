// useSignalRConnection.js
/**
 * Lightweight hook that provides access to the SignalR client without triggering connection management.
 *
 * Use this hook in child components that need to:
 * - Listen to SignalR events (on/off)
 * - Send messages to the server (sendMessage)
 * - Check connection state
 *
 * DO NOT use this to manage the connection lifecycle (start/stop).
 * Connection management is handled by useSignalR() which should only be called in App.jsx.
 *
 * @example
 * const signalR = useSignalRConnection();
 *
 * useEffect(() => {
 *   if (!signalR) return;
 *
 *   const handleEvent = (data) => {
 *     console.log('Received:', data);
 *   };
 *
 *   signalR.on('EventName', handleEvent);
 *   return () => signalR.off('EventName', handleEvent);
 * }, [signalR]);
 */
import signalRClient from "../services/signalR/SignalRClientSingleton.js";

export const useSignalRConnection = () => {
  if (!signalRClient.isServiceInitialized()) {
    console.warn(
      "SignalR client not initialized. Make sure useSignalR is called in App.jsx"
    );
    return null;
  }

  const service = signalRClient.getService();

  return {
    isConnected: signalRClient.getConnectionState() === "Connected",
    connectionState: signalRClient.getConnectionState(),
    sendMessage: (methodName, ...args) => {
      return service.connection.invoke(methodName, ...args);
    },
    on: (methodName, callback) => {
      return service.connection.on(methodName, callback);
    },
    off: (methodName, callback) => {
      return service.connection.off(methodName, callback);
    },
  };
};
