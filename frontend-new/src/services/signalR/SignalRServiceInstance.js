import SignalRService from "./SignalRService";

let signalRServiceInstance = null;

export const getSignalRService = (
  addNotification,
  getToken,
  isAuthenticated
) => {
  if (!signalRServiceInstance) {
    signalRServiceInstance = new SignalRService(
      addNotification,
      getToken,
      isAuthenticated
    );
  }
  return signalRServiceInstance;
};
