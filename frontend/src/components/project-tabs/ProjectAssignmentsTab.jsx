import React, { useState, useEffect } from "react";
import httpClient from "../../httpclient";
import NavigateButton from "../NavigateButton";
import DeleteButton from "../DeleteButton";
import DataTable from "../DataTable";
import { useNotification } from "../../context/NotificationContext";
import { useTranslation } from "react-i18next";

const ProjectAssignmentsTab = ({ projectId }) => {
  const [assignments, setAssignments] = useState([]);
  const [loading, setLoading] = useState(true);
  const { addNotification } = useNotification();
  const { t } = useTranslation(['common', 'projects']);

  const fetchAssignments = async () => {
    try {
      setLoading(true);
      const response = await httpClient.get(
        `/project/${projectId}/SubjectVideoGroupAssignments`
      );
      setAssignments(response.data);
    } catch (error) {
      //addNotification("Failed to load assignments", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAssignments();
  }, [projectId]);

  const handleDeleteAssignment = async (assignmentId) => {
    try {
      await httpClient.delete(`/SubjectVideoGroupAssignment/${assignmentId}`);
      setAssignments(
        assignments.filter((assignment) => assignment.id !== assignmentId)
      );
      // DeleteButton will show success notification automatically
    } catch (error) {
      // addNotification(
      //   error.response?.data?.message || "Failed to delete assignment",
      //   "error"
      // );
    }
  };

  // Define columns for assignments table
  const assignmentColumns = [
    { field: "subjectName", header: t('projects:assignment.subject') },
    { field: "videoGroupName", header: t('projects:assignment.video_group') },
  ];

  if (loading) {
    return (
      <div className="d-flex justify-content-center my-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading assignments...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="assignments">
      <div className="d-flex justify-content-end mb-3">
        <NavigateButton 
          actionType='Add'
          path={`/assignments/add?projectId=${projectId}`}
          value={t('projects:add.assignment')} />
      </div>
      {assignments.length > 0 ? (
        <DataTable
          showRowNumbers={true}
          columns={assignmentColumns}
          data={assignments}
          navigateButton={(assignment) => (
            <NavigateButton
              path={`/assignments/${assignment.id}`}
              actionType='Add'
              value={t('common:buttons.details')}
            />
          )}
          deleteButton={(assignment) => (
            <DeleteButton
              onClick={() => handleDeleteAssignment(assignment.id)}
              itemType="assignment"
            />
          )}
        />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>{t('projects:not_found.assignment')}
        </div>
      )}
    </div>
  );
};

export default ProjectAssignmentsTab;
