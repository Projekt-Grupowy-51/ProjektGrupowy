import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";

const VideoGroupAdd = () => {
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    projectId: null,
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    // Extract projectId from query params if available
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
    setLoading(true);
    setError("");

    // Validate form
    if (!formData.name || !formData.description || !formData.projectId) {
      setError("Please fill in all required fields.");
      setLoading(false);
      return;
    }

    try {
      await httpClient.post("/videogroup", formData);
      navigate(`/projects/${formData.projectId}`, {
        state: { successMessage: "Video group added successfully!" },
      });
    } catch (err) {
      setError(
        err.response?.data?.message || "An error occurred. Please try again."
      );
      setLoading(false);
    }
  };

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-lg-8">
          <div className="card shadow-sm">
            <div className="card-header bg-primary text-white">
              <h1 className="heading mb-0">Add New Video Group</h1>
            </div>
            <div className="card-body">
              {error && (
                <div className="alert alert-danger mb-4">
                  <i className="fas fa-exclamation-triangle me-2"></i>
                  {error}
                </div>
              )}

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="name" className="form-label">
                    Group Name
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
                    Description
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
                    disabled={loading}
                  >
                    <i className="fas fa-plus-circle me-2"></i>
                    {loading ? "Adding..." : "Add Video Group"}
                  </button>
                  <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={() => navigate(`/projects/${formData.projectId}`)}
                    disabled={loading}
                  >
                    <i className="fas fa-times me-2"></i>Cancel
                  </button>
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
