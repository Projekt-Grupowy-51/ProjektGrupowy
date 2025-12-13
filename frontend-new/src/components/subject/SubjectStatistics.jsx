import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Card } from '../ui';
import { LoadingSpinner, ErrorAlert } from '../common';
import { Pie, Bar } from 'react-chartjs-2';
import SubjectService from '../../services/SubjectService.js';

const SubjectStatistics = ({ subjectId }) => {
  const { t } = useTranslation(['common', 'subjects']);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchStatistics();
  }, [subjectId]);

  const fetchStatistics = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await SubjectService.getStatistics(subjectId);
      setStatistics(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const generateColors = (count) => {
    const colors = [
      'rgba(255, 99, 132, 0.8)',
      'rgba(54, 162, 235, 0.8)',
      'rgba(255, 206, 86, 0.8)',
      'rgba(75, 192, 192, 0.8)',
      'rgba(153, 102, 255, 0.8)',
      'rgba(255, 159, 64, 0.8)',
      'rgba(99, 255, 132, 0.8)',
      'rgba(235, 54, 162, 0.8)',
    ];
    const result = [];
    for (let i = 0; i < count; i++) {
      result.push(colors[i % colors.length]);
    }
    return result;
  };

  if (loading) {
    return <LoadingSpinner message={t('common:states.loading')} />;
  }

  if (error) {
    return <ErrorAlert error={error} />;
  }

  if (!statistics) {
    return <ErrorAlert error={t('subjects:statistics.no_data')} />;
  }

  // Prepare chart data for label usage by type
  const labelUsageData = {
    labels: Object.keys(statistics.labelUsageByType || {}),
    datasets: [{
      data: Object.values(statistics.labelUsageByType || {}),
      backgroundColor: generateColors(Object.keys(statistics.labelUsageByType || {}).length),
      borderWidth: 1,
      borderColor: '#fff'
    }]
  };

  // Prepare chart data for top 10 labelers
  const topLabelers = Object.entries(statistics.labelsByLabeler || {})
    .sort(([,a], [,b]) => b - a)
    .slice(0, 10);

  const labelsByLabelerData = {
    labels: topLabelers.map(([name]) => name),
    datasets: [{
      label: t('subjects:statistics.labels_count'),
      data: topLabelers.map(([, count]) => count),
      backgroundColor: 'rgba(54, 162, 235, 0.8)',
      borderWidth: 1,
      borderColor: '#fff'
    }]
  };

  // Prepare chart data for labels by project
  const labelsByProjectData = {
    labels: Object.keys(statistics.labelsByProject || {}),
    datasets: [{
      data: Object.values(statistics.labelsByProject || {}),
      backgroundColor: generateColors(Object.keys(statistics.labelsByProject || {}).length),
      borderWidth: 1,
      borderColor: '#fff'
    }]
  };

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom',
        labels: {
          padding: 10,
          font: {
            size: 11
          }
        }
      }
    }
  };

  const barChartOptions = {
    ...chartOptions,
    plugins: {
      ...chartOptions.plugins,
      legend: {
        display: false
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          stepSize: 1
        }
      }
    }
  };

  return (
    <div>
      <h5 className="mb-3">
        <i className="fas fa-chart-bar me-2"></i>
        {t('subjects:statistics.title')}
      </h5>

      {/* KPI Cards */}
      <div className="row mb-4">
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-tags fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.totalLabels}</h4>
              <p className="text-muted mb-0 small">{t('subjects:statistics.total_label_types')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-bookmark fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.totalAssignedLabels}</h4>
              <p className="text-muted mb-0 small">{t('subjects:statistics.total_assigned')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-video fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.uniqueVideosLabeled}</h4>
              <p className="text-muted mb-0 small">{t('subjects:statistics.unique_videos')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-tasks fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.totalAssignments}</h4>
              <p className="text-muted mb-0 small">{t('subjects:statistics.total_assignments')}</p>
            </Card.Body>
          </Card>
        </div>
      </div>

      {/* Charts Row */}
      <div className="row mb-4">
        {/* Label Usage Chart */}
        {Object.keys(statistics.labelUsageByType || {}).length > 0 && (
          <div className="col-lg-4 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-pie me-2"></i>
                  {t('subjects:statistics.label_usage')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '250px' }}>
                  <Pie data={labelUsageData} options={chartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}

        {/* Top 10 Labelers Chart */}
        {Object.keys(statistics.labelsByLabeler || {}).length > 0 && (
          <div className="col-lg-4 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-bar me-2"></i>
                  {t('subjects:statistics.top_labelers')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '250px' }}>
                  <Bar data={labelsByLabelerData} options={barChartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}

        {/* Labels by Project Chart */}
        {Object.keys(statistics.labelsByProject || {}).length > 0 && (
          <div className="col-lg-4 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-pie me-2"></i>
                  {t('subjects:statistics.labels_by_project')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '250px' }}>
                  <Pie data={labelsByProjectData} options={chartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}
      </div>

      {/* Empty State */}
      {statistics.totalAssignedLabels === 0 && (
        <Card>
          <Card.Body className="text-center py-4">
            <i className="fas fa-chart-bar fa-3x text-muted mb-3"></i>
            <p className="text-muted">{t('subjects:statistics.no_labels')}</p>
          </Card.Body>
        </Card>
      )}
    </div>
  );
};

export default SubjectStatistics;
