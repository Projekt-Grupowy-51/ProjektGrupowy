import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpClient";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useNotification } from "../context/NotificationContext";

const Projects = () => {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();
  const { addNotification } = useNotification();

  // Remove deleteModal state since it's now handled by the global context

  useEffect(() => {
    if (location.state?.success) {
      addNotification(location.state.success, "success");
    }
  }, [location.state, addNotification]);

  const fetchProjects = async () => {
    try {
      const response = await httpClient.get("/Project");
      const sortedProjects = response.data.sort((a, b) => a.id - b.id);
      setProjects(sortedProjects);
    } catch (error) {
      addNotification(error.response?.data?.message || "Failed to load projects", "error");
    } finally {
      setLoading(false);
    }
  };

  // Simplified delete handler - will be passed to DeleteButton
  const handleDeleteProject = async (projectId) => {
    try {
      await httpClient.delete(`/Project/${projectId}`);
      setProjects((prev) =>
        prev.filter((project) => project.id !== projectId)
      );
    } catch (error) {
      addNotification(error.response?.data?.message || "Failed to delete project", "error");
    }
  };

  useEffect(() => {
    if (location.state?.successMessage) {
      addNotification(location.state.successMessage, "success");
      window.history.replaceState({}, document.title);
    }
  }, [location.state, addNotification]);

  useEffect(() => {
    fetchProjects();
  }, []);

  // Define columns configuration
  const columns = [
    { field: "id", header: "ID" },
    { field: "name", header: "Name" },
    { field: "description", header: "Description" }
  ];

  return (
    <div className="container">
      <div className="content">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <h1 className="heading">Projects</h1>
          <NavigateButton path={`/projects/add`} actionType="Add" />
        </div>

        {loading ? (
          <div className="text-center py-5">
            <div className="spinner-border text-primary" role="status">
              <span className="visually-hidden">Loading...</span>
            </div>
            <p className="mt-3">Loading projects...</p>
          </div>
        ) : projects.length > 0 ? (
          <DataTable
            columns={columns}
            data={projects}
            navigateButton={(project) => (
              <NavigateButton path={`/projects/${project.id}`} actionType="Details"  />
            )}
            deleteButton={(project) => (
              <DeleteButton
                onClick={() => handleDeleteProject(project.id)}
                itemType={`project "${project.name}"`}
              />
            )}
          />
        ) : (
          <div className="alert alert-info text-center">
            <i className="fas fa-info-circle me-2"></i>No projects found
          </div>
        )}
      </div>
    </div>
  );
};

export default Projects;
