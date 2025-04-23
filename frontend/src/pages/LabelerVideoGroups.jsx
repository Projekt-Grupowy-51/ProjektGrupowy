import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import DataTable from "../components/DataTable";
import { useNotification } from "../context/NotificationContext";

const LabelerVideoGroups = () => {
  const { labelerId } = useParams();
  const [projects, setProjects] = useState([]);
  const [assignments, setAssignments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [accessCode, setAccessCode] = useState("");
  const [expandedProjects, setExpandedProjects] = useState({});
  const navigate = useNavigate();
  const { addNotification } = useNotification();

  const assignmentsColumns = [
    { field: "subjectName", header: "Subject" },
    { field: "videoGroupName", header: "Video Group" },
  ];

  const handleJoinProject = async () => {
    if (!accessCode.trim()) {
      addNotification("Please enter an access code", "error");
      return;
      }
      console.log(accessCode);
    try {
      await httpClient.post("/project/join", {
          AccessCode: accessCode.trim(),
      });
      addNotification("Successfully joined the project!", "success");
      setAccessCode("");
      fetchProjects();
    } catch (error) {
        console.log(error);
      addNotification(
        error.response?.data?.message || "Invalid or expired access code",
        "error"
      );
    }
  };

  const fetchProjects = async () => {
    setLoading(true);
    try {
      const response = await httpClient.get("/project");
      setProjects(response.data);
      const expanded = {};
      response.data.forEach((project) => {
        expanded[project.id] = false;
      });
      setExpandedProjects(expanded);
      await fetchAssignments();
    } catch (error) {
      addNotification(
        error.response?.data?.message || "Failed to load projects",
        "error"
      );
      setLoading(false);
    }
  };

  const fetchAssignments = async () => {
    try {
      const response = await httpClient.get(`/SubjectVideoGroupAssignment`);
      setAssignments(response.data);
    } catch (error) {
      addNotification(
        error.response?.data?.message || "Failed to load assignments",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  const toggleProjectExpand = (projectId) => {
    setExpandedProjects((prev) => ({
      ...prev,
      [projectId]: !prev[projectId],
    }));
  };

  // Group assignments by project
  const getProjectAssignments = (projectId) => {
    return assignments.filter(
      (assignment) => assignment.projectId === projectId
    );
  };

  useEffect(() => {
    fetchProjects();
  }, [labelerId]);

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
        <p className="mt-3">Loading data...</p>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="content">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <h1 className="heading">My Projects & Assignments</h1>
          <div className="join-project-section">
            <div className="input-group" style={{ alignItems: "center" }}>
              <input
                type="text"
                placeholder="Paste access code"
                value={accessCode}
                onChange={(e) => setAccessCode(e.target.value)}
                className="form-control"
                style={{ height: "2.5rem" }}
              />
              <button
                onClick={handleJoinProject}
                className="btn btn-primary"
                disabled={loading}
                style={{ height: "2.5rem" }}
              >
                <i className="fas fa-plus-circle me-2"></i>Join Project
              </button>
            </div>
          </div>
        </div>

        {projects.length > 0 ? (
          <div className="projects-container">
            {projects.map((project) => (
              <div key={project.id} className="card mb-4">
                <div
                  className="card-header d-flex justify-content-between align-items-center"
                  style={{ cursor: "pointer" }}
                  onClick={() => toggleProjectExpand(project.id)}
                >
                  <h5 className="mb-0">{project.name}</h5>
                  <button className="btn btn-sm btn-light">
                    <i
                      className={`fas fa-chevron-${
                        expandedProjects[project.id] ? "up" : "down"
                      }`}
                    ></i>
                  </button>
                </div>
                {expandedProjects[project.id] && (
                  <div className="card-body">
                    <h2 className="mt-3 mb-2">Assignments:</h2>
                    {getProjectAssignments(project.id).length > 0 ? (
                      <DataTable
                        showRowNumbers={true}
                        columns={assignmentsColumns}
                        data={getProjectAssignments(project.id)}
                        navigateButton={(assignment) => (
                          <NavigateButton
                            path={`/video-group/${assignment.id}`}
                            actionType="Details"
                          />
                        )}
                      />
                    ) : (
                      <div className="alert alert-info">
                        No assignments for this project yet.
                      </div>
                    )}
                  </div>
                )}
              </div>
            ))}
          </div>
        ) : (
          <div className="alert alert-info text-center">
            <i className="fas fa-info-circle me-2"></i>
            You haven't joined any projects yet. Join a project using an access
            code.
          </div>
        )}
      </div>
    </div>
  );
};

export default LabelerVideoGroups;
