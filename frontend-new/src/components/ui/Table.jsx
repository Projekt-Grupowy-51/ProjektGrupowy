import React from 'react';
import PropTypes from 'prop-types';

const Table = ({ 
  children, 
  striped = false, 
  bordered = false, 
  hover = false, 
  size = '', 
  responsive = false,
  className = '',
  ...props 
}) => {
  const getTableClasses = () => {
    const classes = ['table'];
    
    if (striped) classes.push('table-striped');
    if (bordered) classes.push('table-bordered');
    if (hover) classes.push('table-hover');
    if (size) classes.push(`table-${size}`);
    
    return classes.join(' ');
  };

  const table = (
    <table className={[getTableClasses(), className].filter(Boolean).join(' ')} {...props}>
      {children}
    </table>
  );

  if (responsive) {
    return (
      <div className="table-responsive">
        {table}
      </div>
    );
  }

  return table;
};

const TableHead = ({ children, variant = '', className = '', ...props }) => {
  const classes = [variant ? `table-${variant}` : '', className].filter(Boolean).join(' ');
  
  return (
    <thead className={classes} {...props}>
      {children}
    </thead>
  );
};

const TableBody = ({ children, className = '', ...props }) => {
  const classes = ['', className].filter(Boolean).join(' ');
  
  return (
    <tbody className={classes} {...props}>
      {children}
    </tbody>
  );
};

const TableRow = ({ children, variant = '', className = '', ...props }) => {
  const classes = [variant ? `table-${variant}` : '', className].filter(Boolean).join(' ');
  
  return (
    <tr className={classes} {...props}>
      {children}
    </tr>
  );
};

const TableCell = ({ children, header = false, className = '', ...props }) => {
  const Tag = header ? 'th' : 'td';
  
  return (
    <Tag className={className} {...props}>
      {children}
    </Tag>
  );
};

Table.Head = TableHead;
Table.Body = TableBody;
Table.Row = TableRow;
Table.Cell = TableCell;

Table.propTypes = {
  children: PropTypes.node.isRequired,
  striped: PropTypes.bool,
  bordered: PropTypes.bool,
  hover: PropTypes.bool,
  size: PropTypes.oneOf(['', 'sm', 'lg']),
  responsive: PropTypes.bool,
  className: PropTypes.string
};

TableHead.propTypes = {
  children: PropTypes.node.isRequired,
  variant: PropTypes.oneOf(['', 'dark', 'light']),
  className: PropTypes.string
};

TableBody.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

TableRow.propTypes = {
  children: PropTypes.node.isRequired,
  variant: PropTypes.oneOf(['', 'primary', 'secondary', 'success', 'danger', 'warning', 'info', 'light', 'dark']),
  className: PropTypes.string
};

TableCell.propTypes = {
  children: PropTypes.node,
  header: PropTypes.bool,
  className: PropTypes.string
};

export default Table;
export { TableHead, TableBody, TableRow, TableCell };
