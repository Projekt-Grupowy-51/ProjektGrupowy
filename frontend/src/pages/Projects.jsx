import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import DeleteConfirmationModal from "../components/DeleteConfirmationModal"; // Add import
import "./css/ScientistProjects.css";

const Projects = () => {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [projectToDelete, setProjectToDelete] = useState(null);
  const navigate = useNavigate();
  const location = useLocation();

  // Modified delete state
  const [deleteModal, setDeleteModal] = useState({
    show: false,
    itemType: "project",
    itemId: null,
  });

  useEffect(() => {
    if (location.state?.success) {
      setSuccess(location.state.success);
    }
  }, [location.state]);

  const fetchProjects = async () => {
    try {
      const response = await httpClient.get("/Project");
      const sortedProjects = response.data.sort((a, b) => a.id - b.id);
      setProjects(sortedProjects);
      setError("");
    } catch (error) {
      setError(error.response?.data?.message || "Failed to load projects");
    } finally {
      setLoading(false);
    }
  };

  const handleConfirmDelete = async () => {
    try {
      await httpClient.delete(`/Project/${deleteModal.itemId}`);
      setProjects((prev) =>
        prev.filter((project) => project.id !== deleteModal.itemId)
      );
      setSuccess("Project deleted successfully");
    } catch (error) {
      setError(error.response?.data?.message || "Failed to delete project");
    } finally {
      setDeleteModal({ show: false, itemType: "project", itemId: null });
    }
  };

  const handleCancelDelete = () => {
    setDeleteModal({ show: false, itemType: "project", itemId: null });
  };

  useEffect(() => {
    if (location.state?.successMessage) {
      setSuccess(location.state.successMessage);
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  useEffect(() => {
    fetchProjects();
  }, []);

  return (
    <div className="container">
      <div className="content">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <h1 className="heading">Projects</h1>
          <button
            className="btn btn-primary"
            onClick={() => navigate("/projects/add")}
          >
            <i className="fas fa-plus-circle me-2"></i>Add New Project
          </button>
        </div>

        {success && (
          <div className="alert alert-success mb-4">
            <i className="fas fa-check-circle me-2"></i>
            {success}
          </div>
        )}
        {error && <div className="alert alert-danger mb-4">{error}</div>}

        {loading ? (
          <div className="text-center py-5">
            <div className="spinner-border text-primary" role="status">
              <span className="visually-hidden">Loading...</span>
            </div>
            <p className="mt-3">Loading projects...</p>
          </div>
        ) : projects.length > 0 ? (
          <table className="normal-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Description</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {projects.map((project) => (
                <tr key={project.id}>
                  <td>{project.id}</td>
                  <td>{project.name}</td>
                  <td>{project.description}</td>
                  <td>
                    <div className="d-flex justify-content-start">
                      <button
                        className="btn btn-info me-2"
                        onClick={() => navigate(`/projects/${project.id}`)}
                      >
                        <i className="fas fa-eye me-1"></i>Details
                      </button>
                      <button
                        className="btn btn-danger"
                        onClick={() =>
                          setDeleteModal({
                            show: true,
                            itemType: "project",
                            itemId: project.id,
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
          <div className="alert alert-info text-center">
            <i className="fas fa-info-circle me-2"></i>No projects found
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

export default Projects;
