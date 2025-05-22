import SignalRService from "./SignalRService";

let signalRServiceInstance = null;

export const getSignalRService = (addNotification) => {
  if (!signalRServiceInstance) {
    signalRServiceInstance = new SignalRService(addNotification);
  }
  return signalRServiceInstance;
};
