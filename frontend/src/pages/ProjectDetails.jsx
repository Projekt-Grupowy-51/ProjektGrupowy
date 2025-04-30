import React, { useState, useEffect } from "react";
import { useParams, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import { getSignalRService } from "../services/SignalRServiceInstance";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from 'react-i18next';
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
  const [labelersCount, setLabelersCount] = useState(0);
  const { addNotification } = useNotification();
  const { t } = useTranslation(['projects', 'common']);

  useEffect(() => {
    const startSignalR = async () => {
      const signalRService = getSignalRService(addNotification);
      signalRService.onMessage(MessageTypes.LabelersCountChanged, (msg) => {
        setLabelersCount(msg);
      });
      signalRService.onMessage(MessageTypes.ReportGenerated, () => {
        fetchReports();
      });
    };
    startSignalR();
  }, [id]);

  const fetchReports = async () => {
    const response = await httpClient.get(`/project/${id}/reports`);
    setReports(response.data);
  };

  const fetchBasicProjectData = async () => {
    const projectRes = await httpClient.get(`/project/${id}`);
    setProject(projectRes.data);
    await fetchReports();
  };

  useEffect(() => {
    fetchBasicProjectData();
  }, [id]);

  useEffect(() => {
    const cachedTab = localStorage.getItem(`project_${id}_activeTab`);
    if (cachedTab) {
      setActiveTab(cachedTab);
    }
  }, [id]);
  
  const handleTabChange = (tabName) => {
    setActiveTab(tabName);
    localStorage.setItem(`project_${id}_activeTab`, tabName);
  };

  if (!project) {
    return (
      <div className="container">
        <div className="alert alert-danger">
          <i className="fas fa-exclamation-circle me-2"></i>
          {t('projects:notifications.details_error')}
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="content">
        <h1 className="heading mb-4">{project?.name}</h1>

        <div className="tab-navigation">
          <button
            className={`tab-button ${activeTab === "details" ? "active" : ""}`}
            onClick={() => handleTabChange("details")}
          >
            <i className="fas fa-info-circle me-2"></i>Details
          </button>
          <button
            className={`tab-button ${activeTab === "subjects" ? "active" : ""}`}
            onClick={() => handleTabChange("subjects")}
          >
            <i className="fas fa-folder me-2"></i>Subjects
          </button>
          <button
            className={`tab-button ${activeTab === "videos" ? "active" : ""}`}
            onClick={() => handleTabChange("videos")}
          >
            <i className="fas fa-film me-2"></i>Video Groups
          </button>
          <button
            className={`tab-button ${activeTab === "assignments" ? "active" : ""}`}
            onClick={() => handleTabChange("assignments")}
          >
            <i className="fas fa-tasks me-2"></i>Assignments
          </button>
          <button
            className={`tab-button ${activeTab === "labelers" ? "active" : ""}`}
            onClick={() => handleTabChange("labelers")}
          >
            <i className="fas fa-users me-2"></i>Labelers
            <span className="badge rounded-pill text-bg-primary ms-2">
              {labelersCount}
            </span>
          </button>
          <button
            className={`tab-button ${activeTab === "accessCodes" ? "active" : ""}`}
            onClick={() => handleTabChange("accessCodes")}
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
            <ProjectSubjectsTab projectId={id} />
          )}
          {activeTab === "videos" && (
            <ProjectVideosTab projectId={id} />
          )}
          {activeTab === "assignments" && (
            <ProjectAssignmentsTab projectId={id} />
          )}
          {activeTab === "labelers" && (
            <ProjectLabelersTab
              projectId={id}
              onLabelersUpdate={(count) => setLabelersCount(count)}
            />
          )}
          {activeTab === "accessCodes" && (
            <ProjectAccessCodesTab projectId={id} />
          )}
        </div>
      </div>
    </div>
  );
};

export default ProjectDetails;
