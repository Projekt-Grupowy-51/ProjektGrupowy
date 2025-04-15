import React, { useState, useMemo, useEffect } from 'react';
import './css/DataTable.css';

const DataTable = ({
                     columns,
                     data,
                     navigateButton,
                     deleteButton,
                     tableClassName = "normal-table",
                     showRowNumbers = false
                   }) => {
  const showActions = navigateButton || deleteButton;

  // Add row number column if needed
  const finalColumns = useMemo(() => {
    const cols = [...columns];
    if (showRowNumbers) {
      cols.unshift({
        field: '__rowNumber__',
        header: '#',
        render: (_, __, index) => index + 1,
        nonSortable: true
      });
    }
    return cols;
  }, [columns, showRowNumbers]);

  const [sortConfig, setSortConfig] = useState({
    key: null,
    direction: 'desc'
  });

  // Initialize sort with first sortable column
  useEffect(() => {
    if (finalColumns.length > 0 && sortConfig.key === null) {
      const firstSortable = finalColumns.find(c => !c.nonSortable);
      if (firstSortable) {
        setSortConfig({
          key: firstSortable.field,
          direction: 'desc'
        });
      }
    }
  }, [finalColumns, sortConfig.key]);

  const handleSort = (key, isSortable) => {
    if (!isSortable) return;

    let direction = 'asc';
    if (sortConfig.key === key && sortConfig.direction === 'asc') {
      direction = 'desc';
    }
    setSortConfig({ key, direction });
  };

  const sortedData = useMemo(() => {
    if (!sortConfig.key) return data;

    return [...data].sort((a, b) => {
      const aValue = a[sortConfig.key];
      const bValue = b[sortConfig.key];

      if (aValue === null || aValue === undefined) return 1;
      if (bValue === null || bValue === undefined) return -1;

      if (typeof aValue === 'string' && typeof bValue === 'string') {
        return sortConfig.direction === 'asc'
            ? aValue.localeCompare(bValue)
            : bValue.localeCompare(aValue);
      }

      if (aValue instanceof Date && bValue instanceof Date) {
        return sortConfig.direction === 'asc'
            ? aValue - bValue
            : bValue - aValue;
      }

      return sortConfig.direction === 'asc'
          ? aValue > bValue ? 1 : -1
          : aValue < bValue ? 1 : -1;
    });
  }, [data, sortConfig]);

  const getSortIndicator = (field) => {
    if (sortConfig.key !== field) return null;
    return sortConfig.direction === 'asc' ? ' ▲' : ' ▼';
  };

  return (
      <table className={tableClassName}>
        <thead>
        <tr>
          {finalColumns.map((column) => (
              <th
                  key={column.field}
                  onClick={() => handleSort(column.field, !column.nonSortable)}
                  className={column.nonSortable ? '' : 'sortable-header'}
                  title={column.nonSortable ? '' : 'Click to sort'}
              >
                <div className="header-content">
                  <span>{column.header}</span>
                  {!column.nonSortable && (
                      <span className="sort-indicator">
                    {getSortIndicator(column.field)}
                  </span>
                  )}
                </div>
              </th>
          ))}
          {showActions && <th>Actions</th>}
        </tr>
        </thead>
        <tbody>
        {sortedData.length > 0 ? (
            sortedData.map((item, rowIndex) => (
                <tr key={rowIndex}>
                  {finalColumns.map((column) => (
                      <td key={`${rowIndex}-${column.field}`}>
                        {column.render
                            ? column.render(item[column.field], item, rowIndex)
                            : item[column.field]}
                      </td>
                  ))}
                  {showActions && (
                      <td>
                        <div className="d-flex justify-content-start">
                          {navigateButton && navigateButton(item)}
                          {deleteButton && deleteButton(item)}
                        </div>
                      </td>
                  )}
                </tr>
            ))
        ) : (
            <tr>
              <td colSpan={finalColumns.length + (showActions ? 1 : 0)} className="text-center">
                No data available
              </td>
            </tr>
        )}
        </tbody>
      </table>
  );
};

export default DataTable;