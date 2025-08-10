import React, { useState } from 'react';
import PropTypes from 'prop-types';

const Tabs = ({ children, tabs, activeTab, onTabChange, defaultTab, className = '' }) => {
  // Support both children pattern and tabs array pattern
  const isTabsArray = tabs && Array.isArray(tabs);
  const [internalActiveTab, setInternalActiveTab] = useState(
    activeTab || defaultTab || (isTabsArray ? tabs[0]?.id : children?.[0]?.props.eventKey)
  );

  const currentActiveTab = activeTab || internalActiveTab;

  const handleTabChange = (eventKey) => {
    setInternalActiveTab(eventKey);
    if (onTabChange) {
      onTabChange(eventKey);
    }
  };

  const tabNavItems = isTabsArray 
    ? tabs.map((tab) => (
        <button
          key={tab.id}
          className={`nav-link ${currentActiveTab === tab.id ? 'active' : ''}`}
          onClick={() => handleTabChange(tab.id)}
          type="button"
        >
          {tab.icon && <i className={`${tab.icon} me-2`}></i>}
          {tab.label}
          {tab.badge !== undefined && tab.badge !== null && (
            <span className="badge bg-primary rounded-pill ms-2">
              {tab.badge}
            </span>
          )}
        </button>
      ))
    : React.Children.map(children, (child) => {
        if (React.isValidElement(child)) {
          const { eventKey, title, icon, badge } = child.props;
          return (
            <button
              key={eventKey}
              className={`nav-link ${currentActiveTab === eventKey ? 'active' : ''}`}
              onClick={() => handleTabChange(eventKey)}
              type="button"
            >
              {icon && <i className={`${icon} me-2`}></i>}
              {title}
              {badge && (
                <span className="badge bg-primary rounded-pill ms-2">
                  {badge}
                </span>
              )}
            </button>
          );
        }
        return null;
      });

  const activeTabContent = isTabsArray
    ? tabs.find(tab => tab.id === currentActiveTab)?.content
    : React.Children.toArray(children).find(
        (child) => React.isValidElement(child) && child.props.eventKey === currentActiveTab
      );

  return (
    <div className={className}>
      <nav>
        <div className="nav nav-tabs" role="tablist">
          {tabNavItems}
        </div>
      </nav>
      <div className="tab-content mt-3">
        <div className="tab-pane fade show active">
          {activeTabContent}
        </div>
      </div>
    </div>
  );
};

const TabPane = ({ children, eventKey, title, icon, badge }) => {
  // This component is used as a child of Tabs but doesn't render directly
  return <div>{children}</div>;
};

Tabs.Pane = TabPane;

Tabs.propTypes = {
  children: PropTypes.node,
  tabs: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.string.isRequired,
      label: PropTypes.string.isRequired,
      icon: PropTypes.string,
      badge: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
      content: PropTypes.node.isRequired
    })
  ),
  activeTab: PropTypes.string,
  onTabChange: PropTypes.func,
  defaultTab: PropTypes.string,
  className: PropTypes.string
};

TabPane.propTypes = {
  children: PropTypes.node.isRequired,
  eventKey: PropTypes.string.isRequired,
  title: PropTypes.string.isRequired,
  icon: PropTypes.string,
  badge: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
};

export default Tabs;
