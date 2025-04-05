import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link, useLocation } from "react-router-dom";
import httpClient from "../httpClient";
import DeleteConfirmationModal from "../components/DeleteConfirmationModal";
import "./css/ScientistProjects.css";

const SubjectDetails = () => {
  const { id } = useParams();
  const [subjectDetails, setSubjectDetails] = useState(null);
  const [labels, setLabels] = useState([]);
  const navigate = useNavigate();
  const [successMessage, setSuccessMessage] = useState("");
  const [error, setError] = useState("");
  const location = useLocation();
  const [deleteModal, setDeleteModal] = useState({
    show: false,
    itemType: "label",
    itemId: null,
  });

  const fetchSubjectDetails = async () => {
    try {
      const response = await httpClient.get(`/subject/${id}`);
      setSubjectDetails(response.data);
      await fetchLabels();
    } catch (error) {
      console.error("Error fetching subject details:", error);
      setError("Failed to load subject details");
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
      console.error("Error fetching labels:", error);
      setError("Failed to load labels");
    }
  };

  useEffect(() => {
    if (location.state?.successMessage) {
      setSuccessMessage(location.state.successMessage);
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  useEffect(() => {
    if (id) fetchSubjectDetails();
  }, [id]);

  const handleConfirmDelete = async () => {
    try {
      await httpClient.delete(`/label/${deleteModal.itemId}`);
      setLabels(labels.filter((label) => label.id !== deleteModal.itemId));
      setSuccessMessage("Label deleted successfully!");
      setError("");
    } catch (error) {
      console.error("Error deleting label:", error);
      setError("Failed to delete label. Please try again.");
      setSuccessMessage("");
    } finally {
      setDeleteModal({ show: false, itemType: "label", itemId: null });
    }
  };

  const handleCancelDelete = () => {
    setDeleteModal({ show: false, itemType: "label", itemId: null });
  };

  const addLabel = () => {
    navigate(`/labels/add?subjectId=${id}`);
  };

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

        {error && (
          <div className="alert alert-danger mb-4">
            <i className="fas fa-exclamation-triangle me-2"></i>
            {error}
          </div>
        )}

        {successMessage && (
          <div className="alert alert-success mb-4">
            <i className="fas fa-check-circle me-2"></i>
            {successMessage}
          </div>
        )}

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
          <button className="btn btn-primary" onClick={addLabel}>
            <i className="fas fa-plus-circle me-2"></i>Add New Label
          </button>
          <Link
            className="btn btn-secondary"
            to={`/projects/${subjectDetails.projectId}`}
            style={{ height: "fit-content", margin: "1%" }}
          >
            <i className="fas fa-arrow-left me-2"></i>Back to Project
          </Link>
        </div>

        <h2 className="section-title">Labels</h2>

        {labels.length > 0 ? (
          <table className="normal-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Type</th>
                <th>Color</th>
                <th>Shortcut</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {labels.map((label) => (
                <tr key={label.id}>
                  <td>{label.id}</td>
                  <td>{label.name}</td>
                  <td>{label.type}</td>
                  <td>
                    <div className="d-flex align-items-center">
                      <div
                        className="color-preview me-2"
                        style={{ backgroundColor: label.colorHex }}
                      />
                      <span>{label.colorHex}</span>
                    </div>
                  </td>
                  <td>{label.shortcut}</td>
                  <td>
                    <div className="d-flex justify-content-start">
                      <button
                        className="btn btn-info me-2"
                        onClick={() => navigate(`/labels/edit/${label.id}`)}
                      >
                        <i className="fas fa-eye me-1"></i>Details
                      </button>
                      <button
                        className="btn btn-danger"
                        onClick={() =>
                          setDeleteModal({
                            show: true,
                            itemType: "label",
                            itemId: label.id,
                          })
                        }
                      >
                        <i className="fas fa-trash me-1"></i>Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <div className="alert alert-info">
            <i className="fas fa-info-circle me-2"></i>No labels found for this
            subject
          </div>
        )}
      </div>

      <DeleteConfirmationModal
        show={deleteModal.show}
        itemType={deleteModal.itemType}
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
      />
    </div>
  );
};

export default SubjectDetails;
