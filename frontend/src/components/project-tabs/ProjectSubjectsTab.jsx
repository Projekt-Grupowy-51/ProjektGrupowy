import React, { useState, useEffect } from "react";
import httpClient from "../../httpclient";
import NavigateButton from "../NavigateButton";
import DeleteButton from "../DeleteButton";
import DataTable from "../DataTable";
import { useNotification } from "../../context/NotificationContext";
import {t} from "i18next";
import {useTranslation} from "react-i18next";

const ProjectSubjectsTab = ({ projectId }) => {
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const { addNotification } = useNotification();
  const {t} = useTranslation(['common', 'projects']);

  const fetchSubjects = async () => {
    try {
      setLoading(true);
      const response = await httpClient.get(`/project/${projectId}/subjects`);
      setSubjects(response.data);
    } catch (error) {
      //addNotification("Failed to load subjects", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSubjects();
  }, [projectId]);

  const handleDeleteSubject = async (subjectId) => {
    try {
      await httpClient.delete(`/subject/${subjectId}`);
      setSubjects(subjects.filter((subject) => subject.id !== subjectId));
      // DeleteButton will show success notification automatically
    } catch (error) {
      // addNotification(
      //   error.response?.data?.message || "Failed to delete subject",
      //   "error"
      // );
    }
  };
  
  // Define columns for subjects table
  const subjectColumns = [
    { field: "name", header: t('common:name') },
    { field: "description", header: t('common:description') },
  ];

  if (loading) {
    return (
      <div className="d-flex justify-content-center my-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading subjects...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="subjects">
      <div className="d-flex justify-content-end mb-3">
        <NavigateButton
          actionType='Add'
          path={`/subjects/add?projectId=${projectId}`}
          value={t('projects:add.subject')}
        />
      </div>
      {subjects.length > 0 ? (
        <DataTable
          showRowNumbers={true}
          columns={subjectColumns}
          data={subjects}
          navigateButton={(subject) => (
            <NavigateButton
              path={`/subjects/${subject.id}`}
              actionType='Details'
              value={t('common:buttons.details')}
            />
          )}
          deleteButton={(subject) => (
            <DeleteButton
              onClick={() => handleDeleteSubject(subject.id)}
              itemType="subject"
            />
          )}
        />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>{t('projects:not_found.subject')}
        </div>
      )}
    </div>
  );
};

export default ProjectSubjectsTab;
