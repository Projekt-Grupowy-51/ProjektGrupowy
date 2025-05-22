import React from "react";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import DataTable from "../components/DataTable";
import { useTranslation } from 'react-i18next';
import useLabelerVideoGroups from "../hooks/useLabelerVideoGroups";

const LabelerVideoGroups = () => {
  const { t } = useTranslation(['labeler', 'common']);
  const {
    projects,
    loading,
    accessCode,
    setAccessCode,
    expandedProjects,
    toggleProjectExpand,
    getProjectAssignments,
    handleJoinProject,
  } = useLabelerVideoGroups();

  const assignmentsColumns = [
    { field: "subjectName", header: t('labeler:projects.columns.subject') },
    { field: "videoGroupName", header: t('labeler:projects.columns.video_group') }
  ];

  if (loading) {
    return (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">{t('common:loading')}</span>
          </div>
          <p className="mt-3">{t('labeler:loading')}</p>
        </div>
    );
  }

  return (
      <div className="container">
        <div className="content">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h1 className="heading">{t('labeler:title')}</h1>
            <div className="join-project-section">
              <div className="input-group" style={{ alignItems: "center" }}>
                <input
                    type="text"
                    placeholder={t('labeler:join_project.placeholder')}
                    value={accessCode}
                    onChange={(e) => setAccessCode(e.target.value)}
                    className="form-control"
                    style={{ height: "2.5rem" }}
                />
                <button
                    onClick={handleJoinProject}
                    className="btn btn-primary"
                    disabled={loading}
                    style={{ height: "2.5rem" }}
                >
                  <i className="fas fa-plus-circle me-2"></i>
                  {t('labeler:join_project.button')}
                </button>
              </div>
            </div>
          </div>

          {projects.length > 0 ? (
              <div className="projects-container">
                {projects.map((project) => (
                    <div key={project.id} className="card mb-4">
                      <div
                          className="card-header d-flex justify-content-between align-items-center"
                          style={{ cursor: "pointer" }}
                          onClick={() => toggleProjectExpand(project.id)}
                      >
                        <h5 className="mb-0">{project.name}</h5>
                        <button className="btn btn-sm btn-light">
                          <i
                              className={`fas fa-chevron-${
                                  expandedProjects[project.id] ? "up" : "down"
                              }`}
                          ></i>
                        </button>
                      </div>
                      {expandedProjects[project.id] && (
                          <div className="card-body">
                            <h2 className="mt-3 mb-2">
                              {t('labeler:projects.assignments')}:
                            </h2>
                            {getProjectAssignments(project.id).length > 0 ? (
                                <DataTable
                                    showRowNumbers={true}
                                    columns={assignmentsColumns}
                                    data={getProjectAssignments(project.id)}
                                    navigateButton={(assignment) => (
                                        <NavigateButton
                                            path={`/video-group/${assignment.id}`}
                                            actionType="Details"
                                        />
                                    )}
                                />
                            ) : (
                                <div className="alert alert-info">
                                  <i className="fas fa-info-circle me-2"></i>
                                  {t('labeler:projects.no_assignments')}
                                </div>
                            )}
                          </div>
                      )}
                    </div>
                ))}
              </div>
          ) : (
              <div className="alert alert-info text-center">
                <i className="fas fa-info-circle me-2"></i>
                {t('labeler:projects.no_projects')}
              </div>
          )}
        </div>
      </div>
  );
};

export default LabelerVideoGroups;
