import React from 'react';
import PropTypes from 'prop-types';

const TabListGroup = ({ items, renderItem, maxHeight = '300px' }) => {
  return (
    <div 
      className="list-group" 
      style={{ maxHeight, overflowY: 'auto' }}
    >
      {items.map((item) => (
        <div 
          key={item.id} 
          className="list-group-item d-flex justify-content-between align-items-center"
        >
          {renderItem(item)}
        </div>
      ))}
    </div>
  );
};

TabListGroup.propTypes = {
  items: PropTypes.array.isRequired,
  renderItem: PropTypes.func.isRequired,
  maxHeight: PropTypes.string
};

export default TabListGroup;