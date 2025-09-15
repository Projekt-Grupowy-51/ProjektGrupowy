import React from 'react';
import PropTypes from 'prop-types';

const ActionButtons = ({
  children,
  align = 'start',
  gap = 2,
  className = '',
  ...props
}) => {
  const getAlignmentClass = () => {
    const alignments = {
      start: 'justify-content-start',
      center: 'justify-content-center',
      end: 'justify-content-end',
      between: 'justify-content-between',
      around: 'justify-content-around'
    };
    return alignments[align] || alignments.start;
  };

  const classes = [
    'd-flex',
    getAlignmentClass(),
    `gap-${gap}`,
    className
  ].filter(Boolean).join(' ');

  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

ActionButtons.propTypes = {
  children: PropTypes.node.isRequired,
  align: PropTypes.oneOf(['start', 'center', 'end', 'between', 'around']),
  gap: PropTypes.oneOf([0, 1, 2, 3, 4, 5]),
  className: PropTypes.string
};

export default ActionButtons;