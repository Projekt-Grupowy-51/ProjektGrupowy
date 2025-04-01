import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import httpClient from "../httpClient";
import "./css/ScientistProjects.css";

const LabelerVideoGroups = () => {
    const { labelerId } = useParams();
    const [assignments, setAssignments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [accessCode, setAccessCode] = useState("");
    const [joinError, setJoinError] = useState("");
    const [successMessage, setSuccessMessage] = useState("");
    const navigate = useNavigate();

    const handleJoinProject = async () => {
        setJoinError("");
        setSuccessMessage("");
        if (!accessCode.trim()) {
            setJoinError("Please enter an access code");
            return;
        }

        try {
            await httpClient.post("/project/join", {
                AccessCode: accessCode.trim(),
            });
            setSuccessMessage("Successfully joined the project!");
            setAccessCode("");
            fetchAssignments();
        } catch (error) {
            setJoinError(
                error.response?.data?.message || "Invalid or expired access code"
            );
        }
    };

  const fetchAssignments = async () => {
    try {
      const response = await httpClient.get(`/SubjectVideoGroupAssignment`);
      setAssignments(response.data);
      setError("");
    } catch (error) {
      console.log(error);
      setError(error.response?.data?.message || "Failed to load assignments");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAssignments();
  }, [labelerId]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p className="error">{error}</p>;

    return (
        <div className="container">
            <div className="content">
                <div className="d-flex justify-content-between align-items-center mb-4">
                    <h1 className="heading">My Assignments</h1>
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
                {joinError && (
                    <div className="alert alert-danger mt-2 mb-0">
                        <i className="fas fa-exclamation-triangle me-2"></i>
                        {joinError}
                    </div>
                )}
                {successMessage && (
                    <div className="alert alert-success mt-2 mb-0">
                        <i className="fas fa-check-circle me-2"></i>
                        {successMessage}
                    </div>
                )}
                {error && (
                    <div className="alert alert-danger mb-4">
                        <i className="fas fa-exclamation-circle me-2"></i>
                        {error}
                    </div>
                )}

                {loading ? (
                    <div className="text-center py-5">
                        <div className="spinner-border text-primary" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                        <p className="mt-3">Loading assignments...</p>
                    </div>
                ) : assignments.length > 0 ? (
                    <table className="normal-table">
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>Subject ID</th>
                            <th>Video Group ID</th>
                            <th>Actions</th>
                        </tr>
                        </thead>
                        <tbody>
                        {assignments.map((assignment) => (
                            <tr key={assignment.id}>
                                <td>{assignment.id}</td>
                                <td>{assignment.subjectId}</td>
                                <td>{assignment.videoGroupId}</td>
                                <td>
                                    <div className="btn-group">
                                        <button
                                            className="btn btn-info btn-sm me-2"
                                            onClick={() => navigate(`/video-group/${assignment.videoGroupId}`)}
                                        >
                                            <i className="fas fa-eye me-1"></i>Details
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                ) : (
                    <div className="alert alert-info text-center">
                        <i className="fas fa-info-circle me-2"></i>
                        No assignments found. Join a project using an access code.
                    </div>
                )}
            </div>
        </div>
    );
};

export default LabelerVideoGroups;
