import { useNotification } from "../context/NotificationContext";
import { getSignalRService } from "./SignalRServiceInstance";
import { useEffect, useRef } from "react";
import { MessageTypes } from "../config/messageTypes";
import { useAuth } from "../context/AuthContext";

const SignalRListener = () => {
  const { addNotification } = useNotification();
  const { isAuthenticated } = useAuth();
  const hasStartedRef = useRef(false);
  const signalRServiceRef = useRef(null);

  useEffect(() => {
    if (!isAuthenticated && hasStartedRef.current) {
      signalRServiceRef.current?.stop();
      hasStartedRef.current = false;
    }
  }, [isAuthenticated]);

  useEffect(() => {
    let cancelled = false;

    const startConnection = async () => {
      if (!isAuthenticated || hasStartedRef.current) return;

      const signalRService = getSignalRService(addNotification);
      signalRServiceRef.current = signalRService;

      signalRService.onMessage(MessageTypes.Success, function (msg) {
        addNotification(msg, "success");
      });
      signalRService.onMessage(MessageTypes.Error, function (msg) {
        addNotification(msg, "error");
      });
      signalRService.onMessage(MessageTypes.Warning, function (msg) {
        addNotification(msg, "warning");
      });
      signalRService.onMessage(MessageTypes.Info, function (msg) {
        addNotification(msg, "info");
      });

      await signalRService.start();

      if (!cancelled) hasStartedRef.current = true;
    };

    startConnection();

    return () => {
      cancelled = true;
    };
  }, [isAuthenticated]);

  return null;
};

export default SignalRListener;
