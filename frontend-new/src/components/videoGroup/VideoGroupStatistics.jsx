import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Card } from '../ui';
import { LoadingSpinner, ErrorAlert } from '../common';
import { Pie, Bar } from 'react-chartjs-2';
import VideoGroupService from '../../services/VideoGroupService.js';

const VideoGroupStatistics = ({ videoGroupId }) => {
  const { t } = useTranslation(['common', 'videoGroups']);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchStatistics();
  }, [videoGroupId]);

  const fetchStatistics = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await VideoGroupService.getStatistics(videoGroupId);
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
    return <ErrorAlert error={t('videoGroups:statistics.no_data')} />;
  }

  // Prepare chart data for labels by type
  const labelsByTypeData = {
    labels: Object.keys(statistics.labelsByType || {}),
    datasets: [{
      data: Object.values(statistics.labelsByType || {}),
      backgroundColor: generateColors(Object.keys(statistics.labelsByType || {}).length),
      borderWidth: 1,
      borderColor: '#fff'
    }]
  };

  // Prepare chart data for labels by subject
  const labelsBySubjectData = {
    labels: Object.keys(statistics.labelsBySubject || {}),
    datasets: [{
      data: Object.values(statistics.labelsBySubject || {}),
      backgroundColor: generateColors(Object.keys(statistics.labelsBySubject || {}).length),
      borderWidth: 1,
      borderColor: '#fff'
    }]
  };

  // Prepare chart data for video progress
  const videoProgressData = {
    labels: Object.keys(statistics.videoProgress || {}).map(id => `Video #${id}`),
    datasets: [{
      label: t('videoGroups:statistics.labels_count'),
      data: Object.values(statistics.videoProgress || {}),
      backgroundColor: 'rgba(54, 162, 235, 0.8)',
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
        {t('videoGroups:statistics.title')}
      </h5>

      {/* KPI Cards */}
      <div className="row mb-4">
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-video fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.totalVideos}</h4>
              <p className="text-muted mb-0 small">{t('videoGroups:statistics.total_videos')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-tags fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.totalLabels}</h4>
              <p className="text-muted mb-0 small">{t('videoGroups:statistics.total_labels')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-check-circle fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.completedVideos}</h4>
              <p className="text-muted mb-0 small">{t('videoGroups:statistics.completed_videos')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-percentage fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.completionPercentage.toFixed(1)}%</h4>
              <p className="text-muted mb-0 small">{t('videoGroups:statistics.completion_rate')}</p>
            </Card.Body>
          </Card>
        </div>
      </div>

      {/* Charts Row */}
      <div className="row mb-4">
        {/* Labels by Type Chart */}
        {Object.keys(statistics.labelsByType || {}).length > 0 && (
          <div className="col-lg-4 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-pie me-2"></i>
                  {t('videoGroups:statistics.labels_by_type')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '250px' }}>
                  <Pie data={labelsByTypeData} options={chartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}

        {/* Labels by Subject Chart */}
        {Object.keys(statistics.labelsBySubject || {}).length > 0 && (
          <div className="col-lg-4 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-pie me-2"></i>
                  {t('videoGroups:statistics.labels_by_subject')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '250px' }}>
                  <Pie data={labelsBySubjectData} options={chartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}

        {/* Video Progress Chart */}
        {Object.keys(statistics.videoProgress || {}).length > 0 && (
          <div className="col-lg-4 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-bar me-2"></i>
                  {t('videoGroups:statistics.video_progress')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '250px' }}>
                  <Bar data={videoProgressData} options={barChartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}
      </div>

      {/* Empty State */}
      {statistics.totalLabels === 0 && (
        <Card>
          <Card.Body className="text-center py-4">
            <i className="fas fa-chart-bar fa-3x text-muted mb-3"></i>
            <p className="text-muted">{t('videoGroups:statistics.no_labels')}</p>
          </Card.Body>
        </Card>
      )}
    </div>
  );
};

export default VideoGroupStatistics;
