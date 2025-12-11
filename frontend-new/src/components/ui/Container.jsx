import React from 'react';
import PropTypes from 'prop-types';

const Container = ({ 
  children, 
  fluid = false, 
  size = '', 
  className = '',
  ...props 
}) => {
  const getContainerClass = () => {
    if (fluid) return 'container-fluid';
    if (size) return `container-${size}`;
    return 'container';
  };

  const classes = [getContainerClass(), className].filter(Boolean).join(' ');

  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

const Row = ({ children, className = '', ...props }) => {
  const classes = ['row', className].filter(Boolean).join(' ');
  
  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

const Col = ({ 
  children, 
  xs, sm, md, lg, xl, xxl,
  className = '',
  ...props 
}) => {
  const getColClasses = () => {
    const classes = ['col'];
    
    if (xs) classes.push(`col-${xs}`);
    if (sm) classes.push(`col-sm-${sm}`);
    if (md) classes.push(`col-md-${md}`);
    if (lg) classes.push(`col-lg-${lg}`);
    if (xl) classes.push(`col-xl-${xl}`);
    if (xxl) classes.push(`col-xxl-${xxl}`);
    
    return classes.join(' ');
  };

  const classes = [getColClasses(), className].filter(Boolean).join(' ');
  
  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

Container.Row = Row;
Container.Col = Col;

Container.propTypes = {
  children: PropTypes.node.isRequired,
  fluid: PropTypes.bool,
  size: PropTypes.oneOf(['', 'sm', 'md', 'lg', 'xl', 'xxl']),
  className: PropTypes.string
};

Row.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

Col.propTypes = {
  children: PropTypes.node.isRequired,
  xs: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  sm: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  md: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  lg: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  xl: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  xxl: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  className: PropTypes.string
};

export default Container;
