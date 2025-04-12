import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import httpClient from "../httpClient";
import "./css/ScientistProjects.css";
import DeleteButton from "../components/DeleteButton";

const SubjectVideoGroupAssignmentDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [assignmentDetails, setAssignmentDetails] = useState(null);
  const [subject, setSubject] = useState(null);
  const [videoGroup, setVideoGroup] = useState(null);
  const [labelers, setLabelers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState({
    main: "",
    labelers: "",
  });

  const fetchData = async () => {
    try {
      setLoading(true);
      setError({ main: "", labelers: "" });

      const assignmentResponse = await httpClient.get(
        `/SubjectVideoGroupAssignment/${id}`
      );
      setAssignmentDetails(assignmentResponse.data);

      const [subjectResponse, videoGroupResponse] = await Promise.all([
        httpClient.get(`/subject/${assignmentResponse.data.subjectId}`),
        httpClient.get(`/videogroup/${assignmentResponse.data.videoGroupId}`),
      ]);

      setSubject(subjectResponse.data);
      setVideoGroup(videoGroupResponse.data);

      try {
        const labelersResponse = await httpClient.get(
          `/SubjectVideoGroupAssignment/${id}/labelers`
        );
        setLabelers(labelersResponse.data);
      } catch (err) {
        setError((prev) => ({
          ...prev,
          labelers: "Error loading labelers",
        }));
      }
    } catch (error) {
      setError((prev) => ({
        ...prev,
        main: "Failed to load assignment details",
      }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id) fetchData();
  }, [id]);

  const handleDelete = async () => {
    if (!window.confirm("Are you sure you want to delete this assignment?"))
      return;

    try {
      await httpClient.delete(`/SubjectVideoGroupAssignment/${id}`);
      navigate("/assignments");
    } catch (error) {
      setError((prev) => ({ ...prev, main: "Deletion failed" }));
    }
  };

  if (loading)
    return (
      <div className="container d-flex justify-content-center align-items-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  if (error.main)
    return (
      <div className="container mt-4">
        <div className="alert alert-danger">{error.main}</div>
      </div>
    );

  return (
    <div className="container py-4">
      <div className="row mb-4">
        <div className="col">
          <div className="d-flex justify-content-between align-items-center">
            <h1 className="heading mb-0">Assignment #{assignmentDetails.id} Details</h1>
            <div className="d-flex justify-content-end">
              <DeleteButton onClick={handleDelete} />
              <button
                className="btn btn-secondary me-2 text-nowrap"
                onClick={fetchData}
              >
                <i className="fas fa-sync-alt me-1"></i> Refresh
              </button>
            </div>
          </div>
        </div>
      </div>

      {error.labelers && (
        <div className="row mb-3">
          <div className="col">
            {error.labelers && <div className="error">{error.labelers}</div>}
          </div>
        </div>
      )}

      <div className="row g-4 mb-4">
        {/* Subject Details */}
        <div className="col-md-6">
          <div className="card shadow-sm h-100">
            <div className="card-header bg-gradient-blue text-white">
              <h5 className="card-title mb-0">Subject Details</h5>
            </div>
            <div className="card-body">
              {subject ? (
                <table className="w-100">
                  <tbody>
                    <tr>
                      <th scope="row">Name</th>
                      <td>{subject.name}</td>
                    </tr>
                    <tr>
                      <th scope="row">Description</th>
                      <td>{subject.description}</td>
                    </tr>
                  </tbody>
                </table>
              ) : (
                <div className="error">Error loading subject</div>
              )}
            </div>
          </div>
        </div>

        {/* Video Group Details */}
        <div className="col-md-6">
          <div className="card shadow-sm h-100">
            <div className="card-header bg-gradient-blue text-white">
              <h5 className="card-title mb-0">Video Group Details</h5>
            </div>
            <div className="card-body">
              {videoGroup ? (
                <table className="w-100">
                  <tbody>
                    <tr>
                      <th scope="row">Name</th>
                      <td>{videoGroup.name}</td>
                    </tr>
                    <tr>
                      <th scope="row">Description</th>
                      <td>{videoGroup.description}</td>
                    </tr>
                  </tbody>
                </table>
              ) : (
                <div className="error">Error loading video group</div>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Labelers Table */}
      <div className="row mb-4">
        <div className="col-12">
          <div className="assigned-labels">
            <h3 className="p-2">Assigned Labelers</h3>
            <div className="assigned-labels-table">
              {labelers.length > 0 ? (
                <table className="normal-table">
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Email</th>
                    </tr>
                  </thead>
                  <tbody>
                    {labelers.map((labeler) => {
                      console.log(labeler); // Możesz logować tutaj
                      return (
                        <tr key={labeler.id}>
                          <td>
                            {labeler.name} {labeler.surname}
                          </td>
                          <td>{labeler.email}</td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              ) : (
                <div className="text-center py-4">
                  <i className="fas fa-user-slash fs-1 text-muted"></i>
                  <p className="text-muted mt-2">
                    {error.labelers
                      ? "Error loading labelers"
                      : "No labelers assigned"}
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      <div className="d-flex justify-content-center mt-4">
        <Link
          to={videoGroup ? `/projects/${videoGroup.projectId}` : "/projects"}
          className="back-btn"
          style={{ height: "fit-content", margin: "1%" }}
        >
          <i className="fas fa-arrow-left me-2"></i> Back to Project
        </Link>
      </div>
    </div>
  );
};

export default SubjectVideoGroupAssignmentDetails;
