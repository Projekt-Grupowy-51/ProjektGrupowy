import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from 'react-i18next';
import useProject from "../hooks/useProject";
import { updateProject } from "../services/api/projectService";

function ProjectEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { project } = useProject(id ? parseInt(id) : undefined);
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    finished: false,
  });
  const { t } = useTranslation(['projects', 'common']);

  React.useEffect(() => {
    if (project) {
      setFormData({
        name: project.name,
        description: project.description,
        finished: project.finished,
      });
    }
  }, [project]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id) return;
    await updateProject(parseInt(id), formData);
    navigate(`/projects/${id}`);
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    const finalValue = type === "checkbox" ? checked : name === "finished" ? value === "true" : value;
    setFormData({ ...formData, [name]: finalValue });
  };

  return (
      <div className="container py-4">
        <div className="row justify-content-center">
          <div className="col-lg-8">
            <div className="card shadow-sm">
              <div className="card-header bg-primary text-white">
                <h1 className="heading mb-0">{t('projects:edit_title')}</h1>
              </div>
              <div className="card-body">
                <form onSubmit={handleSubmit}>
                  <div className="mb-3">
                    <label htmlFor="name" className="form-label">
                      {t('projects:form.name')}
                    </label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        className="form-control"
                        value={formData.name}
                        onChange={handleChange}
                        required
                    />
                  </div>

                  <div className="mb-3">
                    <label htmlFor="description" className="form-label">
                      {t('projects:form.description')}
                    </label>
                    <textarea
                        id="description"
                        name="description"
                        className="form-control"
                        value={formData.description}
                        onChange={handleChange}
                        required
                        rows="4"
                    />
                  </div>

                  <div className="mb-4">
                    <label htmlFor="finished" className="form-label">
                      {t('projects:form.status')}
                    </label>
                    <select
                        id="finished"
                        name="finished"
                        className="form-select"
                        value={(formData.finished !== undefined ? formData.finished : false).toString()}
                        onChange={handleChange}
                    >
                      <option value="false">{t('projects:status.active')}</option>
                      <option value="true">{t('projects:status.completed')}</option>
                    </select>
                  </div>

                  <div className="d-flex">
                    <button type="submit" className="btn btn-primary me-2">
                      <i className="fas fa-save me-2"></i>
                      {t('projects:buttons.save')}
                    </button>
                    <NavigateButton path={`/projects/${id}`} actionType="Back" value={t('common:buttons.cancel')} />
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
  );
}

export default ProjectEdit;
