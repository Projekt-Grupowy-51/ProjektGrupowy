import React, { useState, useMemo, useEffect } from 'react';
import './css/DataTable.css';
import { useTranslation} from "react-i18next";

const DataTable = ({
                     columns,
                     data,
                     navigateButton,
                     deleteButton,
                     tableClassName = "normal-table",
                     showRowNumbers = false,
                     defaultSort = null 
                   }) => {
  const showActions = navigateButton || deleteButton;
  const {t} = useTranslation(['common']);

  const [sortConfig, setSortConfig] = useState({
    key: defaultSort?.key || null,
    direction: defaultSort?.direction || 'desc'
  });

  useEffect(() => {
    if (columns.length > 0 && sortConfig.key === null) {
      setSortConfig({
        key: defaultSort?.key || columns[0].field,
        direction: defaultSort?.direction || 'desc'
      });
    }
  }, [columns, sortConfig.key, defaultSort]);

  const handleSort = (key) => {
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
          {showRowNumbers && <th>#</th>}
          {columns.map((column) => (
              <th
                  key={column.field}
                  onClick={() => handleSort(column.field)}
                  className="sortable-header"
                  title="Click to sort"
              >
                <div className="header-content">
                  <span>{column.header}</span>
                  <span className="sort-indicator">{getSortIndicator(column.field)}</span>
                </div>
              </th>
          ))}
          {showActions && <th>{t('common:actions')}</th>}
        </tr>
        </thead>
        <tbody>
        {sortedData.length > 0 ? (
            sortedData.map((item, rowIndex) => (
                <tr key={rowIndex}>
                  {showRowNumbers && <td>{rowIndex + 1}</td>}
                  {columns.map((column) => (
                      <td key={`${rowIndex}-${column.field}`}>
                        {column.render ? column.render(item, item, rowIndex) : item[column.field]}
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
              <td colSpan={columns.length + (showRowNumbers ? 1 : 0) + (showActions ? 1 : 0)} className="text-center">
                No data available
              </td>
            </tr>
        )}
        </tbody>
      </table>
  );
};

export default DataTable;
