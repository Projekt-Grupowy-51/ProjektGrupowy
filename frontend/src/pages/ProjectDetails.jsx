import React, { useState, useEffect } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useLocation } from "react-router-dom";
import httpClient from "../httpClient";
import DeleteConfirmationModal from "../components/DeleteConfirmationModal"; // Import the external modal
import "./css/ScientistProjects.css";

const ProjectDetails = () => {
  const { id } = useParams();
  const [project, setProject] = useState(null);
  const [subjects, setSubjects] = useState([]);
  const [videoGroups, setVideoGroups] = useState([]);
  const [assignments, setAssignments] = useState([]);
  const [activeTab, setActiveTab] = useState("details");
  const [error, setError] = useState("");
  const [labelers, setLabelers] = useState([]);
  const [accessCodes, setAccessCodes] = useState([]);
  const [creationError, setCreationError] = useState("");
  //const [selectedDuration, setSelectedDuration] = useState('');
  const [codeExpiration, setCodeExpiration] = useState(0);
  const [selectedLabeler, setSelectedLabeler] = useState("");
  const [selectedAssignment, setSelectedAssignment] = useState("");
  const [assignmentError, setAssignmentError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [visibleCodes, setVisibleCodes] = useState({});
  const navigate = useNavigate();
  const location = useLocation();
  const [deleteModal, setDeleteModal] = useState({
    show: false,
    itemType: "",
    endpoint: "",
    itemId: null,
    assignmentId: null,
    labelerId: null,
  });

  const fetchData = async () => {
    try {
      const [
        projectRes,
        subjectsRes,
        videoGroupsRes,
        assignmentsRes,
        labelersRes,
        accessCodesRes,
      ] = await Promise.all([
        httpClient.get(`/project/${id}`),
        httpClient.get(`/project/${id}/subjects`),
        httpClient.get(`/project/${id}/videogroups`),
        httpClient.get(`/project/${id}/SubjectVideoGroupAssignments`),
        httpClient.get(`/project/${id}/Labelers`),
        httpClient.get(`/AccessCode/project/${id}`),
      ]);
      setProject(projectRes.data);
      setSubjects(subjectsRes.data);
      setVideoGroups(videoGroupsRes.data);
      setAssignments(assignmentsRes.data);
      setLabelers(labelersRes.data);
      setAccessCodes(accessCodesRes.data);
    } catch (error) {
      setError(error.response?.data?.message || "Failed to load project data");
    }
  };

  const handleUnassignAllLabelers = async () => {
    try {
      await httpClient.post(`/project/${id}/unassign-all`);
      await fetchData();
      setSuccessMessage("All labelers unassigned successfully!");
    } catch (error) {
      setError(error.response?.data?.message || "Failed to unassign labelers");
    }
  };

  const handleConfirmDelete = async () => {
    try {
      if (deleteModal.endpoint === "unassign-labeler") {
        await httpClient.delete(
          `/SubjectVideoGroupAssignment/${deleteModal.assignmentId}/unassign-labeler/${deleteModal.labelerId}`
        );
        await fetchData();
        setSuccessMessage("Labeler assignment removed successfully!");
      } else {
        await httpClient.delete(
          `/${deleteModal.endpoint}/${deleteModal.itemId}`
        );
        await fetchData();
        setSuccessMessage(
          `${
            deleteModal.itemType.charAt(0).toUpperCase() +
            deleteModal.itemType.slice(1)
          } deleted successfully!`
        );
      }
    } catch (error) {
      setError(error.response?.data?.message || "Operation failed");
    } finally {
      setDeleteModal({
        show: false,
        itemType: "",
        endpoint: "",
        itemId: null,
        assignmentId: null,
        labelerId: null,
      });
    }
  };

  const handleCancelDelete = () => {
    setDeleteModal({ show: false, itemType: "", endpoint: "", itemId: null });
  };

  const handleCreateAccessCode = async () => {
    setCreationError("");

    // let expiresAt = null;
    // if (selectedDuration === "14days" || selectedDuration === "30days") {
    //   const days = selectedDuration === "14days" ? 14 : 30;
    //   const now = new Date();
    //   expiresAt =
    //     new Date(
    //       Date.UTC(
    //         now.getUTCFullYear(),
    //         now.getUTCMonth(),
    //         now.getUTCDate() + days,
    //         now.getUTCHours(),
    //         now.getUTCMinutes(),
    //         now.getUTCSeconds()
    //       )
    //     )
    //       .toISOString()
    //       .split(".")[0] + "Z";
    // }

    // console.log("Code expiration:", {
    //   projectId: parseInt(id),
    //   accessCodeExpiration: codeExpiration,
    // });

    // return;

    try {
      await httpClient.post("/AccessCode/project", {
        projectId: parseInt(id),
        expiration: codeExpiration,
      });
      //setSelectedDuration('');
      setCodeExpiration(0);
      await fetchData();
      setSuccessMessage("Access code created successfully!");
    } catch (error) {
      setCreationError(
        error.response?.data?.message || "Failed to create code"
      );
    }
  };

  const handleCopyCode = (code) => {
    navigator.clipboard
      .writeText(code)
      .then(() => {
        setSuccessMessage("Code copied to clipboard!");
      })
      .catch(() => {
        setError("Failed to copy code. Please try again.");
      });
  };

  const handleDistributeLabelers = async () => {
    try {
      await httpClient.post(`/project/${id}/distribute`);
      await fetchData();
      setSuccessMessage("Labelers distributed successfully!");
    } catch (error) {
      setError(error.response?.data?.message || "Distribution failed");
    }
  };

  const handleAssignLabeler = async () => {
    if (!selectedLabeler || !selectedAssignment) {
      setAssignmentError("Please select both a labeler and an assignment");
      return;
    }

    try {
      await httpClient.post(
        `/SubjectVideoGroupAssignment/${selectedAssignment}/assign-labeler/${selectedLabeler}`
      );
      setSelectedLabeler("");
      setSelectedAssignment("");
      setAssignmentError("");
      await fetchData();
      setSuccessMessage("Labeler assigned successfully!");
    } catch (error) {
      setAssignmentError(
        error.response?.data?.message || "Failed to assign labeler"
      );
    }
  };

  const handleUnassignLabeler = (assignmentId, labelerId) => {
    setDeleteModal({
      show: true,
      itemType: "labeler assignment",
      endpoint: "unassign-labeler",
      assignmentId: assignmentId,
      labelerId: labelerId,
    });
  };

  const toggleCodeVisibility = (code) => {
    setVisibleCodes((prev) => ({
      ...prev,
      [code]: !prev[code],
    }));
  };

  useEffect(() => {
    if (location.state?.successMessage) {
      setSuccessMessage(location.state.successMessage);
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  useEffect(() => {
    fetchData();
  }, [id]);

  if (!project)
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
        <h1 className="heading mb-4">{project.name}</h1>
        {successMessage && (
          <div className="alert alert-success mb-3">
            <i className="fas fa-check-circle me-2"></i>
            {successMessage}
          </div>
        )}
        {error && <div className="alert alert-danger mb-3">{error}</div>}

        <div className="tab-navigation">
          <button
            className={`tab-button ${activeTab === "details" ? "active" : ""}`}
            onClick={() => setActiveTab("details")}
          >
            <i className="fas fa-info-circle me-2"></i>Details
          </button>
          <button
            className={`tab-button ${activeTab === "subjects" ? "active" : ""}`}
            onClick={() => setActiveTab("subjects")}
          >
            <i className="fas fa-folder me-2"></i>Subjects
          </button>
          <button
            className={`tab-button ${activeTab === "videos" ? "active" : ""}`}
            onClick={() => setActiveTab("videos")}
          >
            <i className="fas fa-film me-2"></i>Video Groups
          </button>
          <button
            className={`tab-button ${
              activeTab === "assignments" ? "active" : ""
            }`}
            onClick={() => setActiveTab("assignments")}
          >
            <i className="fas fa-tasks me-2"></i>Assignments
          </button>
          <button
            className={`tab-button ${activeTab === "labelers" ? "active" : ""}`}
            onClick={() => setActiveTab("labelers")}
          >
            <i className="fas fa-users me-2"></i>Labelers
          </button>
          <button
            className={`tab-button ${
              activeTab === "accessCodes" ? "active" : ""
            }`}
            onClick={() => setActiveTab("accessCodes")}
          >
            <i className="fas fa-key me-2"></i>Access Codes
          </button>
        </div>

        <div className="tab-content mt-4">
          {activeTab === "details" && (
            <div className="card shadow-sm">
              <div
                className="card-header text-white"
                style={{ background: "var(--gradient-blue)" }}
              >
                <h5 className="card-title mb-0">Project Details</h5>
              </div>
              <div className="card-body">
                <p className="card-text">
                  <strong>Description:</strong> {project.description}
                </p>
                <div className="d-flex mt-3">
                  <button
                    className="btn btn-primary me-2"
                    onClick={() => navigate(`/projects/edit/${project.id}`)}
                  >
                    <i className="fas fa-edit me-2"></i>Edit Project
                  </button>
                  <button
                    className="btn btn-secondary"
                    onClick={() => navigate("/projects")}
                    style={{ height: "fit-content", margin: "1%" }}
                  >
                    <i className="fas fa-arrow-left me-2"></i>Back to Projects
                  </button>
                </div>
              </div>
            </div>
          )}

          {activeTab === "subjects" && (
            <div className="subjects">
              <div className="d-flex justify-content-end mb-3">
                <Link to={`/subjects/add?projectId=${id}`}>
                  <button
                    className="btn btn-primary"
                    style={{ minWidth: "200px" }}
                  >
                    <i className="fas fa-plus-circle me-2"></i>Add Subject
                  </button>
                </Link>
              </div>
              {subjects.length > 0 ? (
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
                    {subjects.map((subject) => (
                      <tr key={subject.id}>
                        <td>{subject.id}</td>
                        <td>{subject.name}</td>
                        <td>{subject.description}</td>
                        <td>
                          <div className="btn-group">
                            <button
                              className="btn btn-info btn-sm me-2"
                              onClick={() =>
                                navigate(`/subjects/${subject.id}`)
                              }
                            >
                              <i className="fas fa-eye me-1"></i>Details
                            </button>
                            <button
                              className="btn btn-danger btn-sm"
                              onClick={() =>
                                setDeleteModal({
                                  show: true,
                                  itemType: "subject",
                                  endpoint: "subject",
                                  itemId: subject.id,
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
                  <i className="fas fa-info-circle me-2"></i>No subjects found
                </div>
              )}
            </div>
          )}

          {activeTab === "videos" && (
            <div className="videos">
              <div className="d-flex justify-content-end mb-3">
                <Link to={`/video-groups/add?projectId=${id}`}>
                  <button
                    className="btn btn-primary"
                    style={{ minWidth: "200px" }}
                  >
                    <i className="fas fa-plus-circle me-2"></i>Add Video Group
                  </button>
                </Link>
              </div>
              {videoGroups.length > 0 ? (
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
                    {videoGroups.map((video) => (
                      <tr key={video.id}>
                        <td>{video.id}</td>
                        <td>{video.name}</td>
                        <td>{video.description}</td>
                        <td>
                          <div className="btn-group">
                            <button
                              className="btn btn-info btn-sm me-2"
                              onClick={() =>
                                navigate(`/video-groups/${video.id}`)
                              }
                            >
                              <i className="fas fa-eye me-1"></i>Details
                            </button>
                            <button
                              className="btn btn-danger btn-sm"
                              onClick={() =>
                                setDeleteModal({
                                  show: true,
                                  itemType: "video group",
                                  endpoint: "videogroup",
                                  itemId: video.id,
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
                  <i className="fas fa-info-circle me-2"></i>No video groups
                  found
                </div>
              )}
            </div>
          )}

          {activeTab === "assignments" && (
            <div className="assignments">
              <div className="d-flex justify-content-end mb-3">
                <Link to={`/assignments/add?projectId=${id}`}>
                  <button
                    className="btn btn-primary"
                    style={{ minWidth: "200px" }}
                  >
                    <i className="fas fa-plus-circle me-2"></i>Add Assignment
                  </button>
                </Link>
              </div>
              {assignments.length > 0 ? (
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
                              onClick={() =>
                                navigate(`/assignments/${assignment.id}`)
                              }
                            >
                              <i className="fas fa-eye me-1"></i>Details
                            </button>
                            <button
                              className="btn btn-danger btn-sm"
                              onClick={() =>
                                setDeleteModal({
                                  show: true,
                                  itemType: "assignment",
                                  endpoint: "SubjectVideoGroupAssignment",
                                  itemId: assignment.id,
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
                  <i className="fas fa-info-circle me-2"></i>No assignments
                  found
                </div>
              )}
            </div>
          )}

          {activeTab === "labelers" && (
            <div className="labelers">
              <div className="d-flex justify-content-end mb-3">
                <button
                  className="btn btn-primary"
                  onClick={handleDistributeLabelers}
                  style={{ minWidth: "200px" }}
                >
                  <i className="fas fa-random me-2"></i>Distribute Labelers
                </button>
              </div>

              <div
                className="card shadow-sm mb-4"
                style={{ marginTop: "25px" }}
              >
                <div
                  className="card-header bg-info text-white"
                  style={{ background: "var(--gradient-blue)" }}
                >
                  <h5 className="card-title mb-0">
                    Assign Labeler to Assignment
                  </h5>
                </div>
                <div className="card-body">
                  {assignmentError && (
                    <div className="alert alert-danger mb-3">
                      <i className="fas fa-exclamation-triangle me-2"></i>
                      {assignmentError}
                    </div>
                  )}
                  <div className="assignment-form">
                    <div className="row mb-3">
                      <div className="col-md-6">
                        <label htmlFor="labelerSelect" className="form-label">
                          Select Labeler:
                        </label>
                        <select
                          id="labelerSelect"
                          className="form-select"
                          value={selectedLabeler}
                          onChange={(e) => setSelectedLabeler(e.target.value)}
                        >
                          <option value="">-- Select Labeler --</option>
                          {labelers.map((labeler) => (
                            <option key={labeler.id} value={labeler.id}>
                              {labeler.name} (ID: {labeler.id})
                            </option>
                          ))}
                        </select>
                      </div>
                      <div className="col-md-6">
                        <label
                          htmlFor="assignmentSelect"
                          className="form-label"
                        >
                          Select Assignment:
                        </label>
                        <select
                          id="assignmentSelect"
                          className="form-select"
                          value={selectedAssignment}
                          onChange={(e) =>
                            setSelectedAssignment(e.target.value)
                          }
                        >
                          <option value="">-- Select Assignment --</option>
                          {assignments.map((assignment) => {
                            const subject = subjects.find(
                              (s) => s.id === assignment.subjectId
                            );
                            const videoGroup = videoGroups.find(
                              (vg) => vg.id === assignment.videoGroupId
                            );
                            return (
                              <option key={assignment.id} value={assignment.id}>
                                Assignment #{assignment.id} - Subject:{" "}
                                {subject?.name || "Unknown"} (ID:{" "}
                                {assignment.subjectId}), Video Group:{" "}
                                {videoGroup?.name || "Unknown"} (ID:{" "}
                                {assignment.videoGroupId})
                              </option>
                            );
                          })}
                        </select>
                      </div>
                    </div>
                    <button
                      className="btn btn-success"
                      disabled={!selectedLabeler || !selectedAssignment}
                      onClick={handleAssignLabeler}
                      style={{ minWidth: "200px" }}
                    >
                      <i className="fas fa-user-plus me-2"></i>Assign Labeler
                    </button>
                  </div>
                </div>
              </div>

              <h3 className="mb-4">Project Labelers</h3>
              {labelers.length > 0 ? (
                <table className="normal-table">
                  <thead>
                    <tr>
                      <th>Labeler ID</th>
                      <th>Username</th>
                    </tr>
                  </thead>
                  <tbody>
                    {labelers.map((labeler) => (
                      <tr key={labeler.id}>
                        <td>{labeler.id}</td>
                        <td>{labeler.name}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              ) : (
                <div className="alert alert-info">
                  <i className="fas fa-info-circle me-2"></i>No labelers found
                  in this project
                </div>
              )}

              <div className="row align-items-center mb-3 mt-4">
                <div className="col">
                  <h3 className="mb-0">Assignment Labelers</h3>
                </div>
                <div className="col-auto">
                  <button
                    className="btn btn-danger text-nowrap"
                    onClick={handleUnassignAllLabelers}
                  >
                    <i className="fa-solid fa-user-xmark me-1"></i>
                    Unassign All Labelers
                  </button>
                </div>
              </div>

              {assignments.some(
                (assignment) =>
                  assignment.labelers && assignment.labelers.length > 0
              ) ? (
                <table className="normal-table">
                  <thead>
                    <tr>
                      <th>Labeler ID</th>
                      <th>Username</th>
                      <th>Video Group ID</th>
                      <th>Subject ID</th>
                      <th>Assignment ID</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {assignments
                      .filter(
                        (assignment) =>
                          assignment.labelers && assignment.labelers.length > 0
                      )
                      .flatMap((assignment) =>
                        assignment.labelers.map((labeler) => ({
                          labeler,
                          videoGroupId: assignment.videoGroupId,
                          subjectId: assignment.subjectId,
                          assignmentId: assignment.id,
                        }))
                      )
                      .map((item, index) => (
                        <tr
                          key={`${item.labeler.id}-${item.videoGroupId}-${index}`}
                        >
                          <td>{item.labeler.id}</td>
                          <td>{item.labeler.name}</td>
                          <td>{item.videoGroupId}</td>
                          <td>{item.subjectId}</td>
                          <td>{item.assignmentId}</td>
                          <td>
                            <button
                              className="btn btn-danger btn-sm"
                              onClick={() =>
                                handleUnassignLabeler(
                                  item.assignmentId,
                                  item.labeler.id
                                )
                              }
                            >
                              <i className="fas fa-user-minus me-1"></i>Unassign
                            </button>
                          </td>
                        </tr>
                      ))}
                  </tbody>
                </table>
              ) : (
                <div className="alert alert-info">
                  <i className="fas fa-info-circle me-2"></i>No labelers found
                  in assignments
                </div>
              )}
            </div>
          )}

          {activeTab === "accessCodes" && (
            <div className="access-codes">
              <div className="card shadow-sm mb-4">
                <div
                  className="card-header bg-primary text-white"
                  style={{ background: "var(--gradient-blue)" }}
                >
                  <h5 className="card-title mb-0">Generate Access Codes</h5>
                </div>
                <div className="card-body">
                  <div className="duration-options d-inline-grid align-items-center gap-3 mb-3">
                    <div className="btn-group">
                      <button
                        className={`btn ${
                          codeExpiration === 0
                            ? "btn-primary"
                            : "btn-outline-primary"
                        }`}
                        onClick={() => setCodeExpiration(0)}
                      >
                        14 Days
                      </button>
                      <button
                        className={`btn ${
                          codeExpiration === 1
                            ? "btn-primary"
                            : "btn-outline-primary"
                        }`}
                        onClick={() => setCodeExpiration(1)}
                      >
                        30 Days
                      </button>
                      <button
                        className={`btn ${
                          codeExpiration === 2
                            ? "btn-primary"
                            : "btn-outline-primary"
                        }`}
                        onClick={() => setCodeExpiration(2)}
                      >
                        Unlimited
                      </button>
                    </div>
                    <button
                      className="btn btn-success"
                      onClick={handleCreateAccessCode}
                      //   disabled={!codeExpiration}
                    >
                      <i className="fas fa-key me-2"></i>Generate Access Code
                    </button>
                  </div>
                  {creationError && (
                    <div className="alert alert-danger">
                      <i className="fas fa-exclamation-triangle me-2"></i>
                      {creationError}
                    </div>
                  )}
                </div>
              </div>

                            {accessCodes.length > 0 ? (
                                <table className="normal-table" id="access-codes-table">
                                    <thead>
                                    <tr>
                                        <th>Code</th>
                                        <th>Created At</th>
                                        <th>Expires At</th>
                                        <th>Valid</th>
                                        <th>Actions</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    {accessCodes
                                        .slice()
                                        .sort((a, b) => {
                                            if (!a.expiresAtUtc && !b.expiresAtUtc) return 0;
                                            if (!a.expiresAtUtc) return -1;
                                            if (!b.expiresAtUtc) return 1;
                                            return new Date(b.expiresAtUtc) - new Date(a.expiresAtUtc);
                                        })
                                        .map((code) => (
                                            <tr key={code.code}>
                                                <td>
                                                    <div className="d-flex align-items-center gap-2">
                                                        <span>
                                                            {visibleCodes[code.code] ? code.code : '*******'}
                                                        </span>
                                                        <button
                                                            className="btn btn-link p-0"
                                                            onClick={() => toggleCodeVisibility(code.code)}
                                                            title={visibleCodes[code.code] ? "Hide code" : "Show code"}
                                                        >
                                                            <i className={`fas fa-eye${visibleCodes[code.code] ? '-slash' : ''}`} />
                                                        </button>
                                                    </div>
                                                </td>
                                                <td>{new Date(code.createdAtUtc).toLocaleString()}</td>
                                                <td>
                                                    {code.expiresAtUtc
                                                        ? new Date(code.expiresAtUtc).toLocaleString()
                                                        : 'Never'}
                                                </td>
                                                <td>
                                                    {code.isValid ?
                                                        <span className="badge bg-success">✓ Valid</span> :
                                                        <span className="badge bg-danger">✗ Invalid</span>}
                                                </td>
                                                <td>
                                                    <button
                                                        className="btn btn-outline-primary btn-sm"
                                                        onClick={() => handleCopyCode(code.code)}
                                                    >
                                                        <i className="fas fa-copy me-1"></i>Copy
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <div className="alert alert-info text-center">
                                    <i className="fas fa-info-circle me-2"></i>No access codes found
                                </div>
                            )}
                        </div>
                    )}
                </div>
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

export default ProjectDetails;
