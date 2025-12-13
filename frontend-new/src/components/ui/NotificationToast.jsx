import React from "react";
import { useNotification } from "../../contexts/NotificationContext.jsx";
import "./NotificationToast.css";

const NotificationToast = () => {
  const { notifications, removeNotification } = useNotification();

  if (notifications.length === 0) {
    return null;
  }

  return (
    <div className="notification-container">
      {notifications.map((notification) => (
        <div
          key={notification.id}
          className={`notification notification-${notification.type}`}
          onClick={() => removeNotification(notification.id)}
        >
          <div className="notification-content">
            <div className="notification-icon">
              {notification.type === "success" && "✓"}
              {notification.type === "error" && "✗"}
              {notification.type === "info" && "ℹ"}
              {notification.type === "warning" && "⚠"}
            </div>
            <div className="notification-message">{notification.message}</div>
            <button
              className="notification-close"
              onClick={(e) => {
                e.stopPropagation();
                removeNotification(notification.id);
              }}
            >
              ×
            </button>
          </div>
        </div>
      ))}
    </div>
  );
};

export default NotificationToast;
