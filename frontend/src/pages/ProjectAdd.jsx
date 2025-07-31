import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from 'react-i18next';
import keycloak from "../keycloak.js";

function ProjectAdd() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    finished: false,
  });
  const { t } = useTranslation(['projects', 'common']);

  const handleSubmit = async (e) => {
    e.preventDefault();

    await httpClient.post("/Project", formData);
    navigate("/projects");
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  return (
      <div className="container py-4">
        <div className="row justify-content-center">
          <div className="col-lg-8">
            <div className="card shadow-sm">
              <div className="card-header bg-primary text-white">
                <h1 className="heading mb-0">{t('projects:add_title')}</h1>
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

                  <div className="d-flex">
                    <button
                        type="submit"
                        className="btn btn-primary me-2"
                    >
                      <i className="fas fa-plus-circle me-2"></i>
                      {t('projects:buttons.create')}
                    </button>
                    <NavigateButton path="/projects" actionType="Back" value={t('common:buttons.cancel')} />
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
  );
}

export default ProjectAdd;
