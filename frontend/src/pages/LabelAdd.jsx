import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from 'react-i18next';

const LabelAdd = () => {
  const [formData, setFormData] = useState({
    name: "",
    colorHex: "#3498db",
    type: "range",
    shortcut: "",
    subjectId: null,
  });
  const [subjectName, setSubjectName] = useState("");
  const navigate = useNavigate();
  const location = useLocation();
  const { addNotification } = useNotification();
  const { t } = useTranslation(['labels', 'common']);

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const subjectId = queryParams.get("subjectId");

    if (subjectId) {
      setFormData((prev) => ({ ...prev, subjectId: parseInt(subjectId) }));
      fetchSubjectName(parseInt(subjectId));
    }
  }, [location.search]);

  const fetchSubjectName = async (id) => {
    const response = await httpClient.get(`/subject/${id}`);
    setSubjectName(response.data.name);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const validateForm = () => {
    if (formData.shortcut.length !== 1) {
      addNotification(t('labels:notification.validation.shortcut'), "error");
      return false;
    }
    if (!/^#[0-9A-Fa-f]{6}$/.test(formData.colorHex)) {
      addNotification(t('labels:notification.validation.color'), "error");
      return false;
    }
    if (!formData.name) {
      addNotification(t('labels:notification.validation.name'), "error");
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm() || !formData.name || !formData.type || !formData.subjectId) {
      return;
    }
    
    await httpClient.post("/label", formData);
    navigate(`/subjects/${formData.subjectId}`);
  };

  if (!formData.subjectId) {
    return (
      <div className="container py-4">
        <div className="alert alert-danger">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {t('labels:notification.missing_subject')}
        </div>
        <NavigateButton path={`/subjects/${formData.subjectId}`} actionType="Back" />
      </div>
    );
  }

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-lg-8">
          <div className="card shadow-sm">
            <div className="card-header bg-primary text-white">
              <h1 className="heading mb-0">{t('labels:add_title')}</h1>
            </div>
            <div className="card-body">
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="name" className="form-label">
                    {t('labels:form.name')}
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
                  <label htmlFor="colorHex" className="form-label">
                    {t('labels:form.color')}
                  </label>
                  <div className="input-group">
                    <input
                      type="color"
                      id="colorHex"
                      name="colorHex"
                      value={formData.colorHex}
                      onChange={handleChange}
                      className="form-control form-control-color"
                      title={t('labels:form.color')}
                      style={{ maxWidth: "60px" }}
                      required
                    />
                    <input
                      type="text"
                      className="form-control form-control-color"
                      value={formData.colorHex}
                      onChange={(e) =>
                        setFormData({ ...formData, colorHex: e.target.value })
                      }
                      pattern="#[0-9A-Fa-f]{6}"
                      placeholder="#RRGGBB"
                      required
                    />
                    <span
                      className="input-group-text"
                      style={{
                        backgroundColor: formData.colorHex,
                        width: "40px",
                      }}
                    ></span>
                  </div>
                  <div className="form-text">
                    {t('labels:form.color_format')}
                  </div>
                </div>

                <div className="mb-3">
                  <label htmlFor="type" className="form-label">
                    {t('labels:form.type')}
                  </label>
                  <select
                    id="type"
                    name="type"
                    value={formData.type}
                    onChange={handleChange}
                    className="form-select"
                    required
                  >
                    <option value="range">{t('labels:form.type_options.range')}</option>
                    <option value="point">{t('labels:form.type_options.point')}</option>
                  </select>
                </div>

                <div className="mb-3">
                  <label htmlFor="shortcut" className="form-label">
                    {t('labels:form.shortcut')}
                  </label>
                  <input
                    type="text"
                    id="shortcut"
                    name="shortcut"
                    value={formData.shortcut}
                    onChange={handleChange}
                    className="form-control"
                    maxLength="1"
                    placeholder={t('labels:form.shortcut_hint')}
                    required
                  />
                  <div className="form-text">
                    {t('labels:form.shortcut_hint')}
                  </div>
                </div>

                <div className="mb-4">
                  <label htmlFor="subjectId" className="form-label">
                    {t('labels:form.subject')}
                  </label>
                  <div className="input-group">
                    <span className="input-group-text">
                      <i className="fas fa-folder"></i>
                    </span>
                    <input
                      type="text"
                      className="form-control"
                      value={subjectName || `${t('common:subject.id')}: ${formData.subjectId}`}
                      disabled
                    />
                  </div>
                </div>

                <div className="d-flex">
                  <button type="submit" className="btn btn-primary me-2">
                    <i className="fas fa-plus-circle me-2"></i>
                    {t('labels:buttons.add')}
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

export default LabelAdd;
