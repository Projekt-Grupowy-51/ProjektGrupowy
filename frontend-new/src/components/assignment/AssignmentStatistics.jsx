import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Card, Table, TableHead, TableBody, TableRow, TableCell } from '../ui';
import { LoadingSpinner, ErrorAlert } from '../common';
import { Pie, Bar } from 'react-chartjs-2';
import SubjectVideoGroupAssignmentService from '../../services/SubjectVideoGroupAssignmentService.js';

const AssignmentStatistics = ({ assignmentId }) => {
  const { t } = useTranslation(['common', 'assignments']);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [sortConfig, setSortConfig] = useState({ key: 'labelCount', direction: 'desc' });

  useEffect(() => {
    fetchStatistics();
  }, [assignmentId]);

  const fetchStatistics = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await SubjectVideoGroupAssignmentService.getStatistics(assignmentId);
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
      'rgba(162, 99, 255, 0.8)',
      'rgba(86, 255, 206, 0.8)',
    ];
    const result = [];
    for (let i = 0; i < count; i++) {
      result.push(colors[i % colors.length]);
    }
    return result;
  };

  const handleSort = (key) => {
    let direction = 'asc';
    if (sortConfig.key === key && sortConfig.direction === 'asc') {
      direction = 'desc';
    }
    setSortConfig({ key, direction });
  };

  const getSortedLabelers = () => {
    if (!statistics?.allLabelers) return [];

    const sorted = [...statistics.allLabelers].sort((a, b) => {
      if (sortConfig.direction === 'asc') {
        return a[sortConfig.key] > b[sortConfig.key] ? 1 : -1;
      } else {
        return a[sortConfig.key] < b[sortConfig.key] ? 1 : -1;
      }
    });
    return sorted;
  };

  if (loading) {
    return <LoadingSpinner message={t('common:states.loading')} />;
  }

  if (error) {
    return <ErrorAlert error={error} />;
  }

  if (!statistics) {
    return <ErrorAlert error={t('assignments:statistics.no_data')} />;
  }

  // Video status pie chart data
  const videoStatusData = {
    labels: [
      t('assignments:statistics.completed'),
      t('assignments:statistics.in_progress'),
      t('assignments:statistics.not_started')
    ],
    datasets: [{
      data: [
        statistics.videoStatus.completed,
        statistics.videoStatus.inProgress,
        statistics.videoStatus.notStarted
      ],
      backgroundColor: [
        'rgba(75, 192, 192, 0.8)',
        'rgba(255, 206, 86, 0.8)',
        'rgba(255, 99, 132, 0.8)'
      ],
      borderWidth: 1,
      borderColor: '#fff'
    }]
  };

  // Top 10 labelers bar chart
  const topLabelersData = {
    labels: Object.keys(statistics.topLabelers),
    datasets: [{
      label: t('assignments:statistics.labels_count'),
      data: Object.values(statistics.topLabelers),
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

  const getProgressBarColor = (percentage) => {
    if (percentage === 100) return 'bg-success';
    if (percentage >= 50) return 'bg-info';
    if (percentage > 0) return 'bg-warning';
    return 'bg-secondary';
  };

  return (
    <div>
      <h5 className="mb-3">
        <i className="fas fa-chart-bar me-2"></i>
        {t('assignments:statistics.title')}
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
              <p className="text-muted mb-0 small">{t('assignments:statistics.total_videos')}</p>
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
              <p className="text-muted mb-0 small">{t('assignments:statistics.total_labels')}</p>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="text-center h-100">
            <Card.Body>
              <div className="text-muted mb-2">
                <i className="fas fa-users fa-2x"></i>
              </div>
              <h4 className="mb-1">{statistics.assignedLabelersCount}</h4>
              <p className="text-muted mb-0 small">{t('assignments:statistics.assigned_labelers')}</p>
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
              <p className="text-muted mb-0 small">{t('assignments:statistics.completion_rate')}</p>
            </Card.Body>
          </Card>
        </div>
      </div>

      {/* Charts Row */}
      <div className="row mb-4">
        {/* Video Status Chart */}
        {statistics.totalVideos > 0 && (
          <div className="col-lg-6 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-pie me-2"></i>
                  {t('assignments:statistics.video_status')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '300px' }}>
                  <Pie data={videoStatusData} options={chartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}

        {/* Top 10 Labelers Chart */}
        {Object.keys(statistics.topLabelers).length > 0 && (
          <div className="col-lg-6 mb-4">
            <Card>
              <Card.Header>
                <h6 className="mb-0">
                  <i className="fas fa-chart-bar me-2"></i>
                  {t('assignments:statistics.top_labelers')}
                </h6>
              </Card.Header>
              <Card.Body>
                <div style={{ height: '300px' }}>
                  <Bar data={topLabelersData} options={barChartOptions} />
                </div>
              </Card.Body>
            </Card>
          </div>
        )}
      </div>

      {/* Video Progress List */}
      {statistics.videos && statistics.videos.length > 0 && (
        <Card className="mb-4">
          <Card.Header>
            <h6 className="mb-0">
              <i className="fas fa-tasks me-2"></i>
              {t('assignments:statistics.video_progress')}
            </h6>
          </Card.Header>
          <Card.Body>
            {statistics.videos.map((video) => (
              <div key={video.id} className="mb-3">
                <div className="d-flex justify-content-between mb-1">
                  <span className="fw-medium">{video.title}</span>
                  <span className="text-muted small">
                    {video.labelsReceived}/{video.expectedLabels} {t('assignments:statistics.labelers')}
                  </span>
                </div>
                <div className="progress">
                  <div
                    className={`progress-bar ${getProgressBarColor(video.completionPercentage)}`}
                    role="progressbar"
                    style={{ width: `${video.completionPercentage}%` }}
                    aria-valuenow={video.completionPercentage}
                    aria-valuemin="0"
                    aria-valuemax="100"
                  >
                    {video.completionPercentage.toFixed(1)}%
                  </div>
                </div>
              </div>
            ))}
          </Card.Body>
        </Card>
      )}

      {/* All Labelers Table */}
      {statistics.allLabelers && statistics.allLabelers.length > 0 && (
        <Card>
          <Card.Header>
            <h6 className="mb-0">
              <i className="fas fa-users me-2"></i>
              {t('assignments:statistics.all_labelers')}
            </h6>
          </Card.Header>
          <Card.Body>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell
                    style={{ cursor: 'pointer' }}
                    onClick={() => handleSort('name')}
                  >
                    {t('assignments:statistics.name')}
                    {sortConfig.key === 'name' && (
                      <i className={`fas fa-sort-${sortConfig.direction === 'asc' ? 'up' : 'down'} ms-1`}></i>
                    )}
                  </TableCell>
                  <TableCell>{t('assignments:statistics.email')}</TableCell>
                  <TableCell
                    style={{ cursor: 'pointer' }}
                    onClick={() => handleSort('labelCount')}
                  >
                    {t('assignments:statistics.labels_count')}
                    {sortConfig.key === 'labelCount' && (
                      <i className={`fas fa-sort-${sortConfig.direction === 'asc' ? 'up' : 'down'} ms-1`}></i>
                    )}
                  </TableCell>
                  <TableCell
                    style={{ cursor: 'pointer' }}
                    onClick={() => handleSort('completionPercentage')}
                  >
                    {t('assignments:statistics.completion')}
                    {sortConfig.key === 'completionPercentage' && (
                      <i className={`fas fa-sort-${sortConfig.direction === 'asc' ? 'up' : 'down'} ms-1`}></i>
                    )}
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {getSortedLabelers().map((labeler) => (
                  <TableRow key={labeler.id}>
                    <TableCell>{labeler.name}</TableCell>
                    <TableCell>{labeler.email}</TableCell>
                    <TableCell>{labeler.labelCount}</TableCell>
                    <TableCell>
                      <div className="d-flex align-items-center">
                        <div className="progress flex-grow-1 me-2" style={{ height: '20px' }}>
                          <div
                            className={`progress-bar ${getProgressBarColor(labeler.completionPercentage)}`}
                            role="progressbar"
                            style={{ width: `${labeler.completionPercentage}%` }}
                            aria-valuenow={labeler.completionPercentage}
                            aria-valuemin="0"
                            aria-valuemax="100"
                          >
                            {labeler.completionPercentage.toFixed(1)}%
                          </div>
                        </div>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </Card.Body>
        </Card>
      )}

      {/* Empty State */}
      {statistics.totalLabels === 0 && (
        <Card>
          <Card.Body className="text-center py-4">
            <i className="fas fa-chart-bar fa-3x text-muted mb-3"></i>
            <p className="text-muted">{t('assignments:statistics.no_labels')}</p>
          </Card.Body>
        </Card>
      )}
    </div>
  );
};

export default AssignmentStatistics;
