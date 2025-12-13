import React from 'react';
import PropTypes from 'prop-types';

const Card = ({ children, className = '', ...props }) => {
  const classes = ['card', className].filter(Boolean).join(' ');
  
  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

const CardHeader = ({ children, className = '', variant = '', ...props }) => {
  const variantClass = variant ? `bg-${variant} text-white` : '';
  const classes = ['card-header', variantClass, className].filter(Boolean).join(' ');
  
  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

const CardBody = ({ children, className = '', ...props }) => {
  const classes = ['card-body', className].filter(Boolean).join(' ');
  
  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

const CardFooter = ({ children, className = '', ...props }) => {
  const classes = ['card-footer', className].filter(Boolean).join(' ');
  
  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};

const CardTitle = ({ children, className = '', level = 5, ...props }) => {
  const classes = ['card-title', className].filter(Boolean).join(' ');
  const Tag = `h${level}`;
  
  return (
    <Tag className={classes} {...props}>
      {children}
    </Tag>
  );
};

const CardText = ({ children, className = '', ...props }) => {
  const classes = ['card-text', className].filter(Boolean).join(' ');
  
  return (
    <p className={classes} {...props}>
      {children}
    </p>
  );
};

Card.Header = CardHeader;
Card.Body = CardBody;
Card.Footer = CardFooter;
Card.Title = CardTitle;
Card.Text = CardText;

Card.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

CardHeader.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  variant: PropTypes.oneOf(['', 'primary', 'secondary', 'success', 'danger', 'warning', 'info', 'light', 'dark'])
};

CardBody.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

CardFooter.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

CardTitle.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  level: PropTypes.oneOf([1, 2, 3, 4, 5, 6])
};

CardText.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

export default Card;
