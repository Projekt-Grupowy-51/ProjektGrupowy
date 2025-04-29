import React from "react";
import { Link } from "react-router-dom";
import NavigateButton from "../NavigateButton";
import DeleteButton from "../DeleteButton";
import { useNavigate } from "react-router-dom";
import httpClient from "../../httpclient";
import { useNotification } from "../../context/NotificationContext";
import { useTranslation } from "react-i18next";

const ProjectDetailsTab = ({ project }) => {
  const navigate = useNavigate();
  const { addNotification } = useNotification();
  const {t} = useTranslation(['common', 'projects']);

  const handleDeleteProject = async () => {
    try {
      await httpClient.delete(`/Project/${project.id}`);
      // DeleteButton will show success notification automatically
      navigate("/projects");
    } catch (error) {
      // addNotification(
      //   error.response?.data?.message || "Failed to delete project",
      //   "error"
      // );
    }
  };

  return (
    <div className="card shadow-sm">
      <div
        className="card-header text-white"
        style={{ background: "var(--gradient-blue)" }}
      >
        <h5 className="card-title mb-0">{t('projects:project_details')}</h5>
      </div>
      <div className="card-body">
        <p className="card-text">
          <strong>Description:</strong> {project.description}
        </p>
        <div className="d-flex mt-3">
          <NavigateButton
            actionType='Edit'
            value={t('common:buttons.edit')}
            path={`/projects/edit/${project.id}`}
          />
          <NavigateButton actionType='Back' value={t('common:buttons.back')}/>
          <DeleteButton onClick={handleDeleteProject} />
        </div>
      </div>
    </div>
  );
};

export default ProjectDetailsTab;
