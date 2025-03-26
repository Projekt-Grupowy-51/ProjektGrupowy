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
  const navigate = useNavigate();

  const handleJoinProject = async () => {
    setJoinError("");
    if (!accessCode.trim()) {
      setJoinError("Please enter an access code");
      return;
    }

    try {
      await httpClient.post("/project/join", {
        AccessCode: accessCode.trim(),
      });
      alert("Successfully joined the project!");
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
        <div>
          <div className="row align-items-center mb-3">
            <div className="col-lg-8">
              <h1 className="heading">SubjectVideoGroupAssignments</h1>
            </div>
            <div className="col-lg-4">
              <div className="d-flex justify-content-start align-items-center">
                <input
                  type="text"
                  placeholder="Access code"
                  value={accessCode}
                  onChange={(e) => setAccessCode(e.target.value)}
                  className={`form-control me-2 ${
                    joinError ? "is-invalid" : ""
                  }`}
                />
                <button
                  onClick={handleJoinProject}
                  className="btn btn-success"
                  style={{ whiteSpace: "nowrap" }} // Prevents text wrapping
                >
                  Join Project
                </button>
              </div>
              {joinError && <div className="text-danger mt-2">{joinError}</div>}
            </div>
          </div>
        </div>

        {error && <div className="error">{error}</div>}

        {loading ? (
          <div style={{ padding: "20px", textAlign: "center" }}>
            Loading SubjectVideoGroupAssignments...
          </div>
        ) : assignments.length > 0 ? (
          <table className="normal-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Subject</th>
                <th>Video Group</th>
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
                    <button
                      className="btn btn-info"
                      onClick={() => navigate(`/video/${assignment.id}`)}
                    >
                      Details
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p className="error">No assignments found</p>
        )}
      </div>
    </div>
  );
};

export default LabelerVideoGroups;
