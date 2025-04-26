import React, { useState, useEffect } from "react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import NavigateButton from "../components/NavigateButton";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import "./css/ScientistProjects.css";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from 'react-i18next';

const SubjectDetails = () => {
  const { t } = useTranslation(['subjects', 'common']);
  const { id } = useParams();
  const [subjectDetails, setSubjectDetails] = useState(null);
  const [labels, setLabels] = useState([]);
  const navigate = useNavigate();
  const location = useLocation();
  const { addNotification } = useNotification();

  const labelColumns = [
    { field: "name", header: t('subjects:columns.name') },
    { field: "shortcut", header: t('subjects:columns.shortcut') },
    {
      field: "colorHex",
      header: t('subjects:columns.color'),
      render: (label) => (
          <div
              style={{
                backgroundColor: label.colorHex,
                width: "20px",
                height: "20px",
                display: "inline-block",
                marginRight: "5px",
                borderRadius: "3px",
              }}
          ></div>
      ),
    },
  ];

  const fetchSubjectDetails = async () => {
    try {
      const response = await httpClient.get(`/subject/${id}`);
      setSubjectDetails(response.data);
      await fetchLabels();
    } catch (error) {
      addNotification(t('subjects:notifications.load_error'), "error");
    }
  };

  const fetchLabels = async () => {
    try {
      const response = await httpClient.get(`/subject/${id}/label`);
      setLabels(response.data.filter(l => l.subjectId === parseInt(id)).sort((a, b) => a.id - b.id));
    } catch (error) {
      addNotification(t('subjects:notifications.labels_error'), "error");
    }
  };

  useEffect(() => {
    if (location.state?.successMessage) {
      addNotification(location.state.successMessage, "success");
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  useEffect(() => {
    if (id) fetchSubjectDetails();
  }, [id]);

  const handleDeleteLabel = async (labelId) => {
    try {
      await httpClient.delete(`/label/${labelId}`);
      await fetchSubjectDetails();
      addNotification(t('subjects:notifications.delete_success'), "success");
    } catch (error) {
      addNotification(t('subjects:notifications.delete_error'), "error");
    }
  };

  if (!subjectDetails) return (
      <div className="container d-flex justify-content-center align-items-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">{t('common:loading')}</span>
        </div>
      </div>
  );

  return (
      <div className="container">
        <div className="content">
          <h1 className="heading mb-4">{subjectDetails.name}</h1>

          <div className="card shadow-sm mb-4">
            <div className="card-header bg-info text-white" style={{ background: "var(--gradient-blue)" }}>
              <h5 className="card-title mb-0">{t('subjects:details.title')}</h5>
            </div>
            <div className="card-body">
              <p className="card-text">
                <strong>{t('subjects:details.description')}:</strong> {subjectDetails.description}
              </p>
            </div>
          </div>

          <div className="d-flex justify-content-between mb-2">
            <NavigateButton path={`/labels/add?subjectId=${id}`} actionType="Add" />
            <NavigateButton actionType="Back" />
          </div>

          <h2 className="section-title">{t('subjects:labels.title')}</h2>

          {labels.length > 0 ? (
              <DataTable
                  showRowNumbers={true}
                  columns={labelColumns}
                  data={labels}
                  navigateButton={(label) => (
                      <NavigateButton
                          path={`/labels/edit/${label.id}`}
                          actionType="Edit"
                      />
                  )}
                  deleteButton={(label) => (
                      <DeleteButton
                          onConfirm={() => handleDeleteLabel(label.id)}
                          itemName={label.name}
                          itemType={t('subjects:labels.label')}
                      />
                  )}
              />
          ) : (
              <div className="alert alert-info">
                <i className="fas fa-info-circle me-2"></i>
                {t('subjects:labels.empty')}
              </div>
          )}
        </div>
      </div>
  );
};

export default SubjectDetails;