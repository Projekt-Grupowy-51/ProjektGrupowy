import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link, useLocation } from "react-router-dom";
import httpClient from "../httpClient";
import NavigateButton from "../components/NavigateButton";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import "./css/ScientistProjects.css";
import { useNotification } from "../context/NotificationContext";

const SubjectDetails = () => {
  const { id } = useParams();
  const [subjectDetails, setSubjectDetails] = useState(null);
  const [labels, setLabels] = useState([]);
  const navigate = useNavigate();
  const location = useLocation();
  const { addNotification } = useNotification();

  const fetchSubjectDetails = async () => {
    try {
      const response = await httpClient.get(`/subject/${id}`);
      setSubjectDetails(response.data);
      await fetchLabels();
    } catch (error) {
      addNotification("Failed to load subject details", "error");
    }
  };

  const fetchLabels = async () => {
    try {
      const response = await httpClient.get(`/subject/${id}/label`);
      const filteredLabels = response.data
        .filter((label) => label.subjectId === parseInt(id))
        .sort((a, b) => a.id - b.id);
      setLabels(filteredLabels);
    } catch (error) {
      addNotification("Failed to load labels", "error");
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
    } catch (error) {
      addNotification("Failed to delete label. Please try again.", "error");
    }
  };
  
  // Define columns for labels table
  const labelColumns = [
    { field: "name", header: "Name" },
    { field: "shortcut", header: "Shortcut" },
    { field: "colorHex", header: "Color", render: (label) => (
      <div style={{ 
        backgroundColor: label.colorHex, 
        width: '20px', 
        height: '20px', 
        display: 'inline-block',
        marginRight: '5px',
        borderRadius: '3px'
      }}></div>
    )},
  ];

  const assignmentsColumns = [
    { field: "id", header: "ID" },
    { field: "videoGroupId", header: "Video Group ID" },
    { field: "videoGroupName", header: "Video Group", render: (item) => item.videoGroupName || "Unknown" },
    // Add other relevant fields
  ];

  if (!subjectDetails)
    return (
      <div className="container d-flex justify-content-center align-items-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );

  return (
    <div className="container">
      <div className="content">
        <h1 className="heading mb-4">{subjectDetails.name}</h1>

        <div className="card shadow-sm mb-4">
          <div
            className="card-header bg-info text-white"
            style={{ background: "var(--gradient-blue)" }}
          >
            <h5 className="card-title mb-0">Subject Details</h5>
          </div>
          <div className="card-body">
            <p className="card-text">
              <strong>Description:</strong> {subjectDetails.description}
            </p>
          </div>
        </div>

        <div className="d-flex justify-content-between mb-2">
          <NavigateButton path={`/labels/add?subjectId=${id}`} actionType="Add" />
          <NavigateButton actionType="Back" />
        </div>

        <h2 className="section-title">Labels</h2>

        {labels.length > 0 ? (
          <DataTable
            columns={labelColumns}
            data={labels}
            navigateButton={(label) => (
              <NavigateButton path={`/labels/edit/${label.id}`} actionType="Edit" />
            )}
            deleteButton={(label) => (
              <DeleteButton 
                onClick={() => handleDeleteLabel(label.id)}
                itemType={`label "${label.name}"`}
              />
            )}
          />
        ) : (
          <div className="alert alert-info">
            <i className="fas fa-info-circle me-2"></i>No labels found for this
            subject
          </div>
        )}
      </div>
    </div>
  );
};

export default SubjectDetails;
