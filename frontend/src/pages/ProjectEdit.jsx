import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import { useNotification } from "../context/NotificationContext";
import NavigateButton from "../components/NavigateButton";

function ProjectEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    finished: false,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { addNotification } = useNotification();

  useEffect(() => {
    const fetchProject = async () => {
      try {
        const response = await httpClient.get(`/Project/${id}`);
        const data = response.data;
        setFormData({
          name: data.name,
          description: data.description,
          finished: data.finished,
        });
      } catch (error) {
        setError(error.response?.data?.message || "Failed to load project data");
        addNotification(
          error.response?.data?.message || "Failed to load project data",
          "error"
        );
      } finally {
        setLoading(false);
      }
    };
    fetchProject();
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      await httpClient.put(`/Project/${id}`, formData);
      addNotification("Project updated successfully!", "success");
      navigate(`/projects/${id}`);
    } catch (error) {
      setError(error.response?.data?.message || "Failed to update project");
      addNotification(
        error.response?.data?.message || "Failed to update project",
        "error"
      );
    }
  };

  const handleChange = (e) => {
    const value =
      e.target.type === "checkbox" ? e.target.checked : e.target.value;
    setFormData({
      ...formData,
      [e.target.name]: value,
    });
  };

  if (loading) {
    return (
      <div className="container d-flex justify-content-center align-items-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-lg-8">
          <div className="card shadow-sm">
            <div className="card-header bg-primary text-white">
              <h1 className="heading mb-0">Edit Project</h1>
            </div>
            <div className="card-body">
              {error && <div className="alert alert-danger mb-4">{error}</div>}

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="name" className="form-label">
                    Project Name
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
                    Description
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
                    Status
                  </label>
                  <select
                    id="finished"
                    name="finished"
                    className="form-select"
                    value={formData.finished}
                    onChange={handleChange}
                  >
                    <option value={false}>Active</option>
                    <option value={true}>Completed</option>
                  </select>
                </div>

                <div className="d-flex">
                  <button type="submit" className="btn btn-primary me-2">
                    <i className="fas fa-save me-2"></i>Save Changes
                  </button>
                  <NavigateButton actionType="Back" value="Cancel" />
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
