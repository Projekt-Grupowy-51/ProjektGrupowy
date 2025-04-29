import React, { useState, useEffect } from "react";
import { useParams, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import { getSignalRService } from "../services/SignalRServiceInstance";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from 'react-i18next';

// Import tab components
import ProjectDetailsTab from "../components/project-tabs/ProjectDetailsTab";
import ProjectSubjectsTab from "../components/project-tabs/ProjectSubjectsTab";
import ProjectVideosTab from "../components/project-tabs/ProjectVideosTab";
import ProjectAssignmentsTab from "../components/project-tabs/ProjectAssignmentsTab";
import ProjectLabelersTab from "../components/project-tabs/ProjectLabelersTab";
import ProjectAccessCodesTab from "../components/project-tabs/ProjectAccessCodesTab";
import { MessageTypes } from "../config/messageTypes";

const ProjectDetails = () => {
  const { id } = useParams();
  const [project, setProject] = useState(null);
  const [reports, setReports] = useState([]);
  const [activeTab, setActiveTab] = useState("details");
  const [loading, setLoading] = useState(true);
  const [labelersCount, setLabelersCount] = useState(0);
  const location = useLocation();
  const { addNotification } = useNotification();
  const { t } = useTranslation(['projects', 'common']);


    useEffect(() => {
        const startSignalR = async () => {
            const signalRService = getSignalRService(addNotification);

      signalRService.onMessage(
        MessageTypes.LabelersCountChanged,
        function (msg) {
          setLabelersCount(msg);
        }
      );

      signalRService.onMessage(MessageTypes.ReportGenerated, function () {
        fetchReports();
      });
    };

    startSignalR();
  }, [id]);

  const fetchReports = async () => {
    try {
      const response = await httpClient.get(`/project/${id}/reports`);
      setReports(response.data);
    } catch (error) {
      //
    }
  };

  // Fetch basic project info for the header
  const fetchBasicProjectData = async () => {
    try {
      setLoading(true);
      const projectRes = await httpClient.get(`/project/${id}`);
      setProject(projectRes.data);

      await fetchReports();
    } catch (error) {
      addNotification(
          error.response?.data?.message || t('projects:notifications.details_error'),
          "error"
      );
    } finally {
      setLoading(false);
    }
  };

  // Common handler for all successful actions
  const handleSuccess = (message) => {
    addNotification(message, "success");
  };

  // Common handler for all errors
  const handleError = (error) => {
    addNotification(
      error.response?.data?.message || "Operation failed",
      "error"
    );
  };

  useEffect(() => {
    if (location.state?.successMessage) {
      addNotification(location.state.successMessage, "success");
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  useEffect(() => {
    fetchBasicProjectData();
  }, [id]);

  if (loading)
    return (
      <div className="container d-flex justify-content-center align-items-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">{t('common:loading')}</span>
        </div>
      </div>
    );

  if (!project && !loading)
    return (
      <div className="container">
        <div className="alert alert-danger">
          <i className="fas fa-exclamation-circle me-2"></i>
          {t('projects:notifications.details_error')}
        </div>
      </div>
  );

  return (
      <div className="container">
        <div className="content">
          <h1 className="heading mb-4">{project?.name}</h1>

        <div className="tab-navigation">
          <button
            className={`tab-button ${activeTab === "details" ? "active" : ""}`}
            onClick={() => setActiveTab("details")}
          >
            <i className="fas fa-info-circle me-2"></i>Details
          </button>
          <button
            className={`tab-button ${activeTab === "subjects" ? "active" : ""}`}
            onClick={() => setActiveTab("subjects")}
          >
            <i className="fas fa-folder me-2"></i>Subjects
          </button>
          <button
            className={`tab-button ${activeTab === "videos" ? "active" : ""}`}
            onClick={() => setActiveTab("videos")}
          >
            <i className="fas fa-film me-2"></i>Video Groups
          </button>
          <button
            className={`tab-button ${
              activeTab === "assignments" ? "active" : ""
            }`}
            onClick={() => setActiveTab("assignments")}
          >
            <i className="fas fa-tasks me-2"></i>Assignments
          </button>
          <button
            className={`tab-button ${activeTab === "labelers" ? "active" : ""}`}
            onClick={() => setActiveTab("labelers")}
          >
            <i className="fas fa-users me-2"></i>Labelers
            <span
              className="badge rounded-pill text-bg-primary ms-2"
              style={{ verticalAlign: "middle" }}
            >
              {labelersCount}
            </span>
          </button>
          <button
            className={`tab-button ${
              activeTab === "accessCodes" ? "active" : ""
            }`}
            onClick={() => setActiveTab("accessCodes")}
          >
            <i className="fas fa-key me-2"></i>Access Codes
          </button>
        </div>

        <div className="tab-content mt-4">
          {activeTab === "details" && (
            <ProjectDetailsTab
              project={project}
              reports={reports}
              onReportDeleted={fetchReports}
            />
          )}

          {activeTab === "subjects" && (
            <ProjectSubjectsTab
              projectId={id}
              onSuccess={handleSuccess}
              onError={handleError}
            />
          )}

          {activeTab === "videos" && (
            <ProjectVideosTab
              projectId={id}
              onSuccess={handleSuccess}
              onError={handleError}
            />
          )}

          {activeTab === "assignments" && (
            <ProjectAssignmentsTab
              projectId={id}
              onSuccess={handleSuccess}
              onError={handleError}
            />
          )}

          {activeTab === "labelers" && (
            <ProjectLabelersTab
              projectId={id}
              onSuccess={handleSuccess}
              onError={handleError}
              onLabelersUpdate={(count) => setLabelersCount(count)}
            />
          )}

          {activeTab === "accessCodes" && (
            <ProjectAccessCodesTab
              projectId={id}
              onSuccess={handleSuccess}
              onError={handleError}
            />
          )}
        </div>
      </div>
    </div>
  );
};

export default ProjectDetails;
