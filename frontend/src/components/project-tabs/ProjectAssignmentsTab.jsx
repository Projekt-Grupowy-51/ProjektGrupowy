import React, { useState, useEffect } from "react";
import httpClient from "../../httpclient";
import NavigateButton from "../NavigateButton";
import DeleteButton from "../DeleteButton";
import DataTable from "../DataTable";
import { useTranslation } from "react-i18next";

const ProjectAssignmentsTab = ({ projectId }) => {
  const [assignments, setAssignments] = useState([]);
  const { t } = useTranslation(['common', 'projects']);

  useEffect(() => {
    fetchAssignments();
  }, [projectId]);

  const fetchAssignments = async () => {
    const response = await httpClient.get(
      `/project/${projectId}/SubjectVideoGroupAssignments`
    );
    setAssignments(response.data);
  };

  const handleDeleteAssignment = async (assignmentId) => {
    await httpClient.delete(`/SubjectVideoGroupAssignment/${assignmentId}`);
    setAssignments(
      assignments.filter((assignment) => assignment.id !== assignmentId)
    );
  };

  const assignmentColumns = [
    { field: "subjectName", header: t('projects:assignment.subject') },
    { field: "videoGroupName", header: t('projects:assignment.video_group') },
  ];

  return (
    <div className="assignments">
      <div className="d-flex justify-content-end mb-3">
        <NavigateButton 
          actionType="Add"
          path={`/assignments/add?projectId=${projectId}`}
          value={t('projects:add.assignment')} 
        />
      </div>
      
      {assignments.length > 0 ? (
        <DataTable
          showRowNumbers={true}
          columns={assignmentColumns}
          data={assignments}
          navigateButton={(assignment) => (
            <NavigateButton
              path={`/assignments/${assignment.id}`}
              actionType="Add"
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
          <i className="fas fa-info-circle me-2"></i>
          {t('projects:not_found.assignment')}
        </div>
      )}
    </div>
  );
};

export default ProjectAssignmentsTab;
