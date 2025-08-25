import { getSignalRService } from "./SignalRServiceInstance";
import { useEffect, useRef } from "react";
import { MessageTypes } from "../config/messageTypes";
import { useAuth } from "../KeycloakProvider";

const SignalRListener = () => {
  const { isAuthenticated, getToken } = useAuth();
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

      const signalRService = getSignalRService(
        addNotification,
        getToken,
        () => isAuthenticated
      );
      signalRServiceRef.current = signalRService;

      signalRService.onMessage(MessageTypes.Success, function (msg) {});
      signalRService.onMessage(MessageTypes.Error, function (msg) {});
      signalRService.onMessage(MessageTypes.Warning, function (msg) {});
      signalRService.onMessage(MessageTypes.Info, function (msg) {});

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
