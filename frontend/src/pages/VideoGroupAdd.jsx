import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from "react-i18next";

const VideoGroupAdd = () => {
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    projectId: null,
  });
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useTranslation(['videos', 'common']);

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const projectId = queryParams.get("projectId");
    if (projectId) {
      setFormData((prev) => ({ ...prev, projectId: parseInt(projectId) }));
    }
  }, [location.search]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await httpClient.post("/videogroup", formData);
    navigate(`/projects/${formData.projectId}`);
  };

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-lg-8">
          <div className="card shadow-sm">
            <div className="card-header bg-primary text-white">
              <h1 className="heading mb-0">{t('video_group.add_title')}</h1>
            </div>
            <div className="card-body">
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="name" className="form-label">
                    {t('video_group.name')}
                  </label>
                  <input
                    type="text"
                    id="name"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                    className="form-control"
                    required
                  />
                </div>

                <div className="mb-3">
                  <label htmlFor="description" className="form-label">
                    {t('video_group.description')}
                  </label>
                  <textarea
                    id="description"
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                    className="form-control"
                    rows="5"
                    required
                  ></textarea>
                </div>

                <div className="d-flex">
                  <button
                    type="submit"
                    className="btn btn-primary me-2"
                  >
                    <i className="fas fa-plus-circle me-2"></i>
                    {t('buttons.add')}
                  </button>
                  <NavigateButton path={`/projects/${formData.projectId}`} actionType="Back" value={t('buttons.cancel')} />
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VideoGroupAdd;
