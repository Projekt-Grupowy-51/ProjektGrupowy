// useSignalR.js
import { useEffect, useRef } from "react";
import { useKeycloak } from "./useKeycloak.js";
import { useNotification } from "../contexts/NotificationContext.jsx";
import signalRClient from "../services/signalR/SignalRClientSingleton.js";
import { MessageTypes } from "../services/signalR/MessageTypes.js";

export const useSignalR = () => {
  const { isAuthenticated, getToken } = useKeycloak();
  const { addNotification } = useNotification();

  const addNotificationRef = useRef(addNotification);
  const getTokenRef = useRef(getToken);
  const isAuthRef = useRef(isAuthenticated);
  const initializedRef = useRef(false);

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

  // one-time init & handlers
  useEffect(() => {
    if (!initializedRef.current) {
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

      initializedRef.current = true;
    }

    // stop ONLY on unmount
    return () => {
      if (signalRClient.isServiceInitialized()) {
        signalRClient.stop().catch(console.error);
      }
    };
  }, []); // <- important: empty deps

  // start/stop when auth flips
  useEffect(() => {
    if (!initializedRef.current) return;
    if (isAuthenticated) {
      signalRClient
        .start()
        .catch((err) => console.error("SignalR start failed:", err));
    } else if (signalRClient.isServiceInitialized()) {
      signalRClient.stop().catch(console.error);
    }
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
  };
};
