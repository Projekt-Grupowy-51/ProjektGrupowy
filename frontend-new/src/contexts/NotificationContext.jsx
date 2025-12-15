import React, { createContext, useContext, useState, useRef } from "react";

const NotificationContext = createContext();

export const useNotification = () => useContext(NotificationContext);

const DEBOUNCE_WINDOW = 3000; // milliseconds

export const NotificationProvider = ({ children }) => {
  const [notifications, setNotifications] = useState([]);
  const lastGroupIdRef = useRef(null);
  const lastNotificationTimeRef = useRef(null);

  const addNotification = (message, type = "success") => {
    const id = crypto.randomUUID();
    const timestamp = Date.now();

    // Determine if this notification should be grouped
    let groupId = null;
    if (
      lastNotificationTimeRef.current &&
      timestamp - lastNotificationTimeRef.current <= DEBOUNCE_WINDOW
    ) {
      // Within debounce window, use the same groupId
      groupId = lastGroupIdRef.current;
    } else {
      // Start a new group
      groupId = crypto.randomUUID();
      lastGroupIdRef.current = groupId;
    }

    lastNotificationTimeRef.current = timestamp;

    setNotifications((prev) => [
      ...prev,
      { id, message, type, timestamp, groupId },
    ]);

    // Preserve original 5-second timeout for each notification
    setTimeout(() => {
      removeNotification(id);
    }, 5000);

    return id;
  };

  const removeNotification = (id) => {
    setNotifications((prev) =>
      prev.filter((notification) => notification.id !== id)
    );
  };

  const removeGroup = (groupId) => {
    setNotifications((prev) =>
      prev.filter((notification) => notification.groupId !== groupId)
    );
  };

  return (
    <NotificationContext.Provider
      value={{
        notifications,
        addNotification,
        removeNotification,
        removeGroup,
      }}
    >
      {children}
    </NotificationContext.Provider>
  );
};
