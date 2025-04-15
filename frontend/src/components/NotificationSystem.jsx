import React from 'react';
import { useNotification } from '../context/NotificationContext';
import './css/Notification.css';

const NotificationSystem = () => {
  const { notifications, removeNotification } = useNotification();

  if (notifications.length === 0) return null;

  return (
    <div className="notification-container">
      {notifications.map(notification => (
        <div 
          key={notification.id} 
          className={`notification notification-${notification.type}`}
        >
          <div className="notification-message">{notification.message}</div>
          <button 
            className="notification-close" 
            onClick={() => removeNotification(notification.id)}
          >
            ×
          </button>
        </div>
      ))}
    </div>
  );
};

export default NotificationSystem;
