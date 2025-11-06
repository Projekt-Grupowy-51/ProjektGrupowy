// useSignalR.js
// This hook should ONLY be called once at the app level (App.jsx)
// For child components that need SignalR access, use useSignalRConnection instead
import { useEffect, useRef } from "react";
import { useKeycloak } from "./useKeycloak.js";
import { useNotification } from "../contexts/NotificationContext.jsx";
import signalRClient from "../services/signalR/SignalRClientSingleton.js";
import { MessageTypes } from "../services/signalR/MessageTypes.js";

// Global flag to ensure initialization happens only once across all instances
let globalInitialized = false;

export const useSignalR = () => {
  const { isAuthenticated, getToken } = useKeycloak();
  const { addNotification } = useNotification();

  const addNotificationRef = useRef(addNotification);
  const getTokenRef = useRef(getToken);
  const isAuthRef = useRef(isAuthenticated);
  const previousAuthRef = useRef(isAuthenticated);

  // keep refs fresh without retriggering init
  useEffect(() => {
    addNotificationRef.current = addNotification;
  }, [addNotification]);
  useEffect(() => {
    getTokenRef.current = getToken;
  }, [getToken]);
  useEffect(() => {
    isAuthRef.current = isAuthenticated;
  }, [isAuthenticated]);

  // one-time global initialization - happens only once per app lifecycle
  useEffect(() => {
    if (!globalInitialized && !signalRClient.isServiceInitialized()) {
      console.log("Initializing SignalR client (global)");

      signalRClient.initialize(
        (msg, level) => addNotificationRef.current(msg, level),
        () => getTokenRef.current(),
        () => isAuthRef.current
      );

      signalRClient.onMessage(MessageTypes.Success, function (msg) {
        addNotificationRef.current(msg, "success");
      });
      signalRClient.onMessage(MessageTypes.Error, function (msg) {
        addNotificationRef.current(msg, "error");
      });
      signalRClient.onMessage(MessageTypes.Warning, function (msg) {
        addNotificationRef.current(msg, "warning");
      });
      signalRClient.onMessage(MessageTypes.Info, function (msg) {
        addNotificationRef.current(msg, "info");
      });

      globalInitialized = true;
    }

    // NO cleanup here - the singleton persists across component mounts/unmounts
  }, []); // <- important: empty deps

  // start/stop when auth changes (but only if it actually changed)
  useEffect(() => {
    if (!globalInitialized) return;

    // Only act if auth state actually changed
    if (previousAuthRef.current === isAuthenticated) {
      return;
    }

    console.log(
      `Auth state changed: ${previousAuthRef.current} -> ${isAuthenticated}`
    );
    previousAuthRef.current = isAuthenticated;

    let isMounted = true;

    if (isAuthenticated) {
      const startConnection = async () => {
        try {
          if (isMounted) {
            await signalRClient.start();
          }
        } catch (err) {
          if (isMounted) {
            console.error("SignalR start failed:", err);
          }
        }
      };
      startConnection();
    } else if (signalRClient.isServiceInitialized()) {
      signalRClient.stop().catch(console.error);
    }

    return () => {
      isMounted = false;
    };
  }, [isAuthenticated]);

  return {
    isConnected: signalRClient.getConnectionState() === "Connected",
    connectionState: signalRClient.getConnectionState(),
    sendMessage: (methodName, ...args) => {
      if (signalRClient.isServiceInitialized()) {
        return signalRClient
          .getService()
          .connection.invoke(methodName, ...args);
      }
      throw new Error("SignalR not initialized");
    },
    on: (methodName, callback) => {
      if (signalRClient.isServiceInitialized()) {
        return signalRClient.getService().connection.on(methodName, callback);
      }
    },
    off: (methodName, callback) => {
      if (signalRClient.isServiceInitialized()) {
        return signalRClient.getService().connection.off(methodName, callback);
      }
    },
  };
};
