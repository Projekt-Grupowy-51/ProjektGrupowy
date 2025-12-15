import { useState, useMemo } from "react";
import { useNotification } from "../../contexts/NotificationContext.jsx";
import { isMobileDevice } from "../../utils/deviceDetection.js";
import "./NotificationToast.css";

const NotificationToast = () => {
  const { notifications, removeNotification, removeGroup } = useNotification();
  const [expandedGroupId, setExpandedGroupId] = useState(null);
  const isMobile = isMobileDevice();

  // Group notifications by groupId on mobile
  const displayNotifications = useMemo(() => {
    if (!isMobile) {
      return notifications.map((n) => ({ ...n, isGroup: false, count: 1 }));
    }

    const groupMap = new Map();
    notifications.forEach((notification) => {
      const { groupId } = notification;
      if (!groupMap.has(groupId)) {
        groupMap.set(groupId, []);
      }
      groupMap.get(groupId).push(notification);
    });

    const result = [];
    groupMap.forEach((group, groupId) => {
      if (group.length > 1) {
        // Multiple notifications in this group - show as grouped
        result.push({
          ...group[0], // Use first notification's properties
          isGroup: true,
          count: group.length,
          groupId,
          notifications: group,
        });
      } else {
        // Single notification - show normally
        result.push({
          ...group[0],
          isGroup: false,
          count: 1,
        });
      }
    });

    return result;
  }, [notifications, isMobile]);

  if (notifications.length === 0) {
    return null;
  }

  const handleGroupClick = (groupId, e) => {
    e.stopPropagation();
    setExpandedGroupId(groupId);
  };

  const handleCloseModal = () => {
    setExpandedGroupId(null);
  };

  const handleRemoveGroup = (groupId, e) => {
    e.stopPropagation();
    removeGroup(groupId);
    setExpandedGroupId(null);
  };

  const expandedGroup = expandedGroupId
    ? displayNotifications.find((n) => n.groupId === expandedGroupId)
    : null;

  return (
    <>
      <div className="notification-container">
        {displayNotifications.map((notification) => (
          <div
            key={notification.groupId || notification.id}
            className={`notification notification-${notification.type} ${
              notification.isGroup ? "notification-grouped" : ""
            }`}
            onClick={() =>
              notification.isGroup
                ? null
                : removeNotification(notification.id)
            }
          >
            <div className="notification-content">
              <div className="notification-icon">
                {notification.type === "success" && "✓"}
                {notification.type === "error" && "✗"}
                {notification.type === "info" && "ℹ"}
                {notification.type === "warning" && "⚠"}
              </div>
              <div className="notification-message">{notification.message}</div>
              {notification.isGroup ? (
                <div
                  className="notification-count-badge"
                  onClick={(e) => handleGroupClick(notification.groupId, e)}
                >
                  <span className="notification-count-number">
                    {notification.count}
                  </span>
                </div>
              ) : (
                <button
                  className="notification-close"
                  onClick={(e) => {
                    e.stopPropagation();
                    removeNotification(notification.id);
                  }}
                >
                  ×
                </button>
              )}
            </div>
          </div>
        ))}
      </div>

      {expandedGroupId && expandedGroup && (
        <div className="notification-modal-overlay" onClick={handleCloseModal}>
          <div
            className="notification-modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="notification-modal-header">
              <h3>Notifications ({expandedGroup.count})</h3>
              <button
                className="notification-modal-close"
                onClick={handleCloseModal}
              >
                ×
              </button>
            </div>
            <div className="notification-modal-content">
              {expandedGroup.notifications.map((notif) => (
                <div
                  key={notif.id}
                  className={`notification-modal-item notification-${notif.type}`}
                >
                  <div className="notification-icon">
                    {notif.type === "success" && "✓"}
                    {notif.type === "error" && "✗"}
                    {notif.type === "info" && "ℹ"}
                    {notif.type === "warning" && "⚠"}
                  </div>
                  <div className="notification-message">{notif.message}</div>
                  <button
                    className="notification-close"
                    onClick={(e) => {
                      e.stopPropagation();
                      removeNotification(notif.id);
                    }}
                  >
                    ×
                  </button>
                </div>
              ))}
            </div>
            <div className="notification-modal-footer">
              <button
                className="notification-modal-dismiss-all"
                onClick={(e) => handleRemoveGroup(expandedGroup.groupId, e)}
              >
                Dismiss All
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default NotificationToast;
