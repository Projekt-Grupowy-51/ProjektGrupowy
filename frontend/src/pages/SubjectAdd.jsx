import React, { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from 'react-i18next';

const AddSubject = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [subjectData, setSubjectData] = useState({
    name: "",
    description: "",
    projectId: new URLSearchParams(location.search).get("projectId"),
  });
  const { t } = useTranslation(['subjects', 'common']);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setSubjectData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await httpClient.post("/Subject", subjectData);
    navigate(`/projects/${subjectData.projectId}`);
  };

  return (
      <div className="container py-4">
        <div className="row justify-content-center">
          <div className="col-lg-8">
            <div className="card shadow-sm">
              <div className="card-header bg-primary text-white">
                <h1 className="heading mb-0">{t('subjects:add_title')}</h1>
              </div>
              <div className="card-body">
                <form onSubmit={handleSubmit}>
                  <div className="mb-3">
                    <label htmlFor="name" className="form-label">
                      {t('subjects:form.name')}
                    </label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        className="form-control"
                        value={subjectData.name}
                        onChange={handleChange}
                        required
                    />
                  </div>

                  <div className="mb-4">
                    <label htmlFor="description" className="form-label">
                      {t('subjects:form.description')}
                    </label>
                    <textarea
                        id="description"
                        name="description"
                        className="form-control"
                        value={subjectData.description}
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
                      {t('subjects:buttons.add')}
                    </button>
                    <NavigateButton actionType="Back" value={t('common:buttons.cancel')} />
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
  );
};

export default AddSubject;
