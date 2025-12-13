import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Chart as ChartJS, ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement, Title } from 'chart.js';
import { Pie, Bar } from 'react-chartjs-2';
import { Card, Alert } from '../../../ui';
import { LoadingSpinner, ErrorAlert } from '../../../common';
import ProjectService from '../../../../services/ProjectService';

// Register ChartJS components
ChartJS.register(
  ArcElement,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  BarElement,
  Title
);

const ProjectStatisticsTab = ({ projectId }) => {
  const { t } = useTranslation(['projects', 'common']);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchStatistics = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await ProjectService.getDetailedStats(projectId);
        setStatistics(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    if (projectId) {
      fetchStatistics();
    }
  }, [projectId]);

  // Generate colors for charts
  const generateColors = (count) => {
    const baseColors = [
      'rgba(54, 162, 235, 0.8)',
      'rgba(255, 99, 132, 0.8)',
      'rgba(255, 206, 86, 0.8)',
      'rgba(75, 192, 192, 0.8)',
      'rgba(153, 102, 255, 0.8)',
      'rgba(255, 159, 64, 0.8)',
      'rgba(199, 199, 199, 0.8)',
      'rgba(83, 102, 255, 0.8)',
      'rgba(255, 99, 255, 0.8)',
      'rgba(99, 255, 132, 0.8)',
    ];

    const colors = [];
    for (let i = 0; i < count; i++) {
      colors.push(baseColors[i % baseColors.length]);
    }
    return colors;
  };

  const createChartData = (dataDict) => {
    const labels = Object.keys(dataDict);
    const values = Object.values(dataDict);
    const colors = generateColors(labels.length);

    return {
      labels,
      datasets: [
        {
          label: t('projects:statistics.count'),
          data: values,
          backgroundColor: colors,
          borderColor: colors.map((color) => color.replace('0.8', '1')),
          borderWidth: 1,
        },
      ],
    };
  };

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        position: 'bottom',
        labels: {
          padding: 15,
          font: {
            size: 12,
          },
        },
      },
      tooltip: {
        callbacks: {
          label: function (context) {
            const label = context.label || '';
            const value = context.parsed || context.parsed.y || 0;
            const total = context.dataset.data.reduce((a, b) => a + b, 0);
            const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
            return `${label}: ${value} (${percentage}%)`;
          },
        },
      },
    },
  };

  const barChartOptions = {
    ...chartOptions,
    indexAxis: 'y',
    plugins: {
      ...chartOptions.plugins,
      legend: {
        display: false,
      },
    },
    scales: {
      x: {
        beginAtZero: true,
        ticks: {
          stepSize: 1,
        },
      },
    },
  };

  if (loading) {
    return (
      <div className="p-4">
        <LoadingSpinner message={t('common:states.loading')} size="small" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4">
        <ErrorAlert error={error} />
      </div>
    );
  }

  if (!statistics || statistics.total_labels === 0) {
    return (
      <div className="p-4">
        <Alert variant="info">
          <i className="fas fa-info-circle me-2"></i>
          {t('projects:statistics.no_data')}
        </Alert>
      </div>
    );
  }

  const hasLabelsBySubject = Object.keys(statistics.labels_by_subject || {}).length > 0;
  const hasLabelsByType = Object.keys(statistics.labels_by_type || {}).length > 0;
  const hasLabelsByLabeler = Object.keys(statistics.labels_by_labeler || {}).length > 0;

  return (
    <div className="p-4">
      {/* KPI Cards */}
      <div className="row g-3 mb-4">
        <div className="col-md-3 col-sm-6">
          <Card className="text-center">
            <Card.Body>
              <h2 className="mb-0 text-primary">{statistics.total_labels || 0}</h2>
              <p className="mb-0 text-muted small">{t('projects:statistics.total_labels')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3 col-sm-6">
          <Card className="text-center">
            <Card.Body>
              <h2 className="mb-0 text-success">{statistics.total_videos || 0}</h2>
              <p className="mb-0 text-muted small">{t('projects:statistics.total_videos')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3 col-sm-6">
          <Card className="text-center">
            <Card.Body>
              <h2 className="mb-0 text-info">{statistics.total_labelers || 0}</h2>
              <p className="mb-0 text-muted small">{t('projects:statistics.total_labelers')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3 col-sm-6">
          <Card className="text-center">
            <Card.Body>
              <h2 className="mb-0 text-warning">{statistics.progress_percentage?.toFixed(1) || 0}%</h2>
              <p className="mb-0 text-muted small">{t('projects:statistics.progress')}</p>
            </Card.Body>
          </Card>
        </div>
      </div>

      {/* Progress Bar */}
      {statistics.total_assignments > 0 && (
        <Card className="mb-4">
          <Card.Body>
            <div className="d-flex justify-content-between align-items-center mb-2">
              <h6 className="mb-0">{t('projects:statistics.assignment_progress')}</h6>
              <span className="badge bg-primary">
                {statistics.completed_assignments} / {statistics.total_assignments}
              </span>
            </div>
            <div className="progress" style={{ height: '25px' }}>
              <div
                className="progress-bar progress-bar-striped progress-bar-animated"
                role="progressbar"
                style={{ width: `${statistics.progress_percentage}%` }}
                aria-valuenow={statistics.progress_percentage}
                aria-valuemin="0"
                aria-valuemax="100"
              >
                {statistics.progress_percentage?.toFixed(1)}%
              </div>
            </div>
          </Card.Body>
        </Card>
      )}

      {/* Charts */}
      <div className="row g-4">
        {hasLabelsBySubject && (
          <div className="col-md-6">
            <Card>
              <Card.Header>
                <Card.Title level={6}>
                  <i className="fas fa-book me-2"></i>
                  {t('projects:statistics.labels_by_subject')}
                </Card.Title>
              </Card.Header>
              <Card.Body>
                <div style={{ position: 'relative', height: '300px' }}>
                  <Pie
                    data={createChartData(statistics.labels_by_subject)}
                    options={chartOptions}
                  />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}

        {hasLabelsByType && (
          <div className="col-md-6">
            <Card>
              <Card.Header>
                <Card.Title level={6}>
                  <i className="fas fa-tags me-2"></i>
                  {t('projects:statistics.labels_by_type')}
                </Card.Title>
              </Card.Header>
              <Card.Body>
                <div style={{ position: 'relative', height: '300px' }}>
                  <Pie
                    data={createChartData(statistics.labels_by_type)}
                    options={chartOptions}
                  />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}

        {hasLabelsByLabeler && (
          <div className="col-12">
            <Card>
              <Card.Header>
                <Card.Title level={6}>
                  <i className="fas fa-users me-2"></i>
                  {t('projects:statistics.top_labelers')}
                </Card.Title>
              </Card.Header>
              <Card.Body>
                <div style={{ position: 'relative', height: '300px' }}>
                  <Bar
                    data={createChartData(
                      Object.fromEntries(
                        Object.entries(statistics.labels_by_labeler || {})
                          .sort(([,a], [,b]) => b - a)
                          .slice(0, 10)
                      )
                    )}
                    options={barChartOptions}
                  />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}
      </div>

      {/* Additional Stats */}
      <div className="row g-3 mt-3">
        <div className="col-md-4">
          <Card>
            <Card.Body className="text-center">
              <i className="fas fa-folder-open fa-2x text-primary mb-2"></i>
              <h4 className="mb-0">{statistics.total_subjects || 0}</h4>
              <p className="mb-0 text-muted small">{t('projects:statistics.total_subjects')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-4">
          <Card>
            <Card.Body className="text-center">
              <i className="fas fa-layer-group fa-2x text-success mb-2"></i>
              <h4 className="mb-0">{statistics.total_video_groups || 0}</h4>
              <p className="mb-0 text-muted small">{t('projects:statistics.total_video_groups')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-4">
          <Card>
            <Card.Body className="text-center">
              <i className="fas fa-tasks fa-2x text-warning mb-2"></i>
              <h4 className="mb-0">{statistics.total_assignments || 0}</h4>
              <p className="mb-0 text-muted small">{t('projects:statistics.total_assignments')}</p>
            </Card.Body>
          </Card>
        </div>
      </div>
    </div>
  );
};

ProjectStatisticsTab.propTypes = {
  projectId: PropTypes.number.isRequired,
};

export default ProjectStatisticsTab;
