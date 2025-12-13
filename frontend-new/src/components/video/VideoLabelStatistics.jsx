import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Chart as ChartJS, ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement, Title } from "chart.js";
import { Pie, Bar } from "react-chartjs-2";
import { Card, Alert } from "../ui";
import { LoadingSpinner, ErrorAlert } from "../common";
import VideoService from "../../services/VideoService";

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

const VideoLabelStatistics = ({ videoId }) => {
  const { t } = useTranslation(["videos", "common"]);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchStatistics = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await VideoService.getLabelStatistics(videoId);
        setStatistics(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    if (videoId) {
      fetchStatistics();
    }
  }, [videoId]);

  // Generate colors for charts
  const generateColors = (count) => {
    const baseColors = [
      "rgba(54, 162, 235, 0.8)",
      "rgba(255, 99, 132, 0.8)",
      "rgba(255, 206, 86, 0.8)",
      "rgba(75, 192, 192, 0.8)",
      "rgba(153, 102, 255, 0.8)",
      "rgba(255, 159, 64, 0.8)",
      "rgba(199, 199, 199, 0.8)",
      "rgba(83, 102, 255, 0.8)",
      "rgba(255, 99, 255, 0.8)",
      "rgba(99, 255, 132, 0.8)",
    ];

    const colors = [];
    for (let i = 0; i < count; i++) {
      colors.push(baseColors[i % baseColors.length]);
    }
    return colors;
  };

  const createChartData = (dataDict, chartType = "pie") => {
    const labels = Object.keys(dataDict);
    const values = Object.values(dataDict);
    const colors = generateColors(labels.length);

    return {
      labels,
      datasets: [
        {
          label: t("videos:details.total_labels"),
          data: values,
          backgroundColor: colors,
          borderColor: colors.map((color) => color.replace("0.8", "1")),
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
        position: "bottom",
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
            const label = context.label || "";
            const value = context.parsed || context.parsed.y || 0;
            const total = context.dataset.data.reduce((a, b) => a + b, 0);
            const percentage = ((value / total) * 100).toFixed(1);
            return `${label}: ${value} (${percentage}%)`;
          },
        },
      },
    },
  };

  const barChartOptions = {
    ...chartOptions,
    indexAxis: "y",
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
      <Card>
        <Card.Body>
          <LoadingSpinner message={t("common:states.loading")} size="small" />
        </Card.Body>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <Card.Body>
          <ErrorAlert error={error} />
        </Card.Body>
      </Card>
    );
  }

  if (!statistics || statistics.totalLabels === 0) {
    return (
      <Card>
        <Card.Header>
          <Card.Title level={5}>
            <i className="fas fa-chart-pie me-2"></i>
            {t("videos:details.labels_summary")}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          <Alert variant="info">
            <i className="fas fa-info-circle me-2"></i>
            {t("videos:details.no_statistics")}
          </Alert>
        </Card.Body>
      </Card>
    );
  }

  const hasLabelsByType = Object.keys(statistics.labelsByType || {}).length > 0;
  const hasLabelsBySubject = Object.keys(statistics.labelsBySubject || {}).length > 0;
  const hasLabelsByLabeler = Object.keys(statistics.labelsByLabeler || {}).length > 0;

  // Prepare top 10 labelers data
  const topLabelers = Object.entries(statistics.labelsByLabeler || {})
    .sort(([, a], [, b]) => b - a)
    .slice(0, 10)
    .reduce((obj, [key, value]) => {
      obj[key] = value;
      return obj;
    }, {});

  return (
    <Card className="mb-4">
      <Card.Header>
        <Card.Title level={5}>
          <i className="fas fa-chart-pie me-2"></i>
          {t("videos:details.labels_summary")}
        </Card.Title>
      </Card.Header>
      <Card.Body>
        <div className="mb-4 p-3 bg-light rounded text-center">
          <h2 className="mb-0 text-primary">{statistics.totalLabels}</h2>
          <p className="mb-0 text-muted">{t("videos:details.total_labels")}</p>
        </div>

        <div className="row g-4">
          {hasLabelsByType && (
            <div className="col-md-6">
              <h6 className="mb-3">
                <i className="fas fa-tags me-2"></i>
                {t("videos:details.labels_by_type")}
              </h6>
              <div style={{ position: "relative", height: "300px" }}>
                <Pie
                  data={createChartData(statistics.labelsByType)}
                  options={chartOptions}
                />
              </div>
            </div>
          )}

          {hasLabelsBySubject && (
            <div className="col-md-6">
              <h6 className="mb-3">
                <i className="fas fa-book me-2"></i>
                {t("videos:details.labels_by_subject")}
              </h6>
              <div style={{ position: "relative", height: "300px" }}>
                <Pie
                  data={createChartData(statistics.labelsBySubject)}
                  options={chartOptions}
                />
              </div>
            </div>
          )}

          {hasLabelsByLabeler && (
            <div className="col-12">
              <h6 className="mb-3">
                <i className="fas fa-users me-2"></i>
                {t("videos:details.top_labelers")}
              </h6>
              <div style={{ position: "relative", height: "300px" }}>
                <Bar
                  data={createChartData(topLabelers, "bar")}
                  options={barChartOptions}
                />
              </div>
            </div>
          )}
        </div>
      </Card.Body>
    </Card>
  );
};

export default VideoLabelStatistics;
