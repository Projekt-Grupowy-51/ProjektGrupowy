import React from "react";
import { useParams, useNavigate } from "react-router-dom";
import "./css/ScientistProjects.css";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from 'react-i18next';
import useAssignmentDetails from "../hooks/useAssignmentDetails";

const SubjectVideoGroupAssignmentDetails = () => {
  const { t } = useTranslation(['assignments', 'common']);
  const { id } = useParams();
  const navigate = useNavigate();
  const { assignment, subject, videoGroup, labelers, fetchData, handleDelete } = useAssignmentDetails(id ? parseInt(id) : undefined);

  const labelerColumns = [
    { field: "name", header: t('assignments:columns.name') },
  ];

  const onDelete = async () => {
    if (!window.confirm(t('assignments:confirm.delete'))) return;
    await handleDelete();
    navigate(-1);
  };

  if (!assignment) {
    return null;
  }

  return (
    <div className="container py-4">
      <div className="row mb-4">
        <div className="col">
          <div className="d-flex justify-content-between align-items-center">
            <h1 className="heading mb-0">
              {t('assignments:details.title', { id: assignment.id })}
            </h1>
            <div className="d-flex justify-content-end">
              <DeleteButton onConfirm={onDelete} />
              <button className="btn btn-secondary me-2 text-nowrap" onClick={fetchData}>
                <i className="fas fa-sync-alt me-1"></i> {t('common:buttons.refresh')}
              </button>
              <NavigateButton path={`/projects/${assignment.projectId}`} actionType="Back" />
            </div>
          </div>
        </div>
      </div>

      <div className="row g-4 mb-4">
        <div className="col-md-6">
          <div className="card shadow-sm h-100">
            <div className="card-header bg-gradient-blue text-white">
              <h5 className="card-title mb-0">{t('assignments:details.subject')}</h5>
            </div>
            <div className="card-body">
              {subject ? (
                  <table className="w-100">
                    <tbody>
                    <tr>
                      <th scope="row">{t('assignments:details.name')}</th>
                      <td>{subject.name}</td>
                    </tr>
                    <tr>
                      <th scope="row">{t('assignments:details.description')}</th>
                      <td>{subject.description}</td>
                    </tr>
                    </tbody>
                  </table>
              ) : (
                  <div className="error">{t('assignments:errors.load_subject')}</div>
              )}
            </div>
          </div>
        </div>

        <div className="col-md-6">
          <div className="card shadow-sm h-100">
            <div className="card-header bg-gradient-blue text-white">
              <h5 className="card-title mb-0">{t('assignments:details.video_group')}</h5>
            </div>
            <div className="card-body">
              {videoGroup ? (
                  <table className="w-100">
                    <tbody>
                    <tr>
                      <th scope="row">{t('assignments:details.name')}</th>
                      <td>{videoGroup.name}</td>
                    </tr>
                    <tr>
                      <th scope="row">{t('assignments:details.description')}</th>
                      <td>{videoGroup.description}</td>
                    </tr>
                    </tbody>
                  </table>
              ) : (
                  <div className="error">{t('assignments:errors.load_video_group')}</div>
              )}
            </div>
          </div>
        </div>
      </div>

      <div className="row mb-4">
        <div className="col-12">
          <div className="assigned-labels">
            <h3 className="p-2">{t('assignments:details.assigned_labelers')}</h3>
            <div className="assigned-labels-table">
              {labelers.length > 0 ? (
                  <DataTable
                      showRowNumbers={true}
                      columns={labelerColumns}
                      data={labelers}
                  />
              ) : (
                  <div className="text-center py-4">
                    <i className="fas fa-user-slash fs-1 text-muted"></i>
                    <p className="text-muted mt-2">{t('assignments:details.no_labelers')}</p>
                  </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SubjectVideoGroupAssignmentDetails;
