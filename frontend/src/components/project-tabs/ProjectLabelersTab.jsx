import React, { useState, useEffect } from "react";
import NavigateButton from "../../components/NavigateButton";
import DeleteButton from "../../components/DeleteButton";
import DataTable from "../../components/DataTable";
import httpClient from "../../httpClient";
import { useNotification } from "../../context/NotificationContext";

const ProjectLabelersTab = ({ 
  projectId, 
  onSuccess, 
  onError 
}) => {
  const { addNotification } = useNotification();
  const [labelers, setLabelers] = useState([]);
  const [unassignedLabelers, setUnassignedLabelers] = useState([]);
  const [assignments, setAssignments] = useState([]);
  const [selectedLabeler, setSelectedLabeler] = useState("");
  const [selectedAssignment, setSelectedAssignment] = useState("");
  const [selectedCustomAssignments, setSelectedCustomAssignment] = useState({});
  const [loading, setLoading] = useState(true);

  // Helper function to format assignment option label
  const formatAssignmentOption = (assignment) => (
    `Assignment #${assignment.id} - Subject: ${assignment.subjectName || "Unknown"} (ID: ${assignment.subjectId}), ` +
    `Video Group: ${assignment.videoGroupName || "Unknown"} (ID: ${assignment.videoGroupId})`
  );

  // Helper function to assign a labeler to an assignment
  const assignLabelerToAssignment = async (labelerId, assignmentId) => {
    try {
      await httpClient.post(
        `/SubjectVideoGroupAssignment/${assignmentId}/assign-labeler/${labelerId}`
      );
      return true;
    } catch (error) {
      addNotification(error.response?.data?.message || "Failed to assign labeler", "error");
      return false;
    }
  };

  const LabelerColumns = [
    { field: "id", header: "Labeler ID" },
    { field: "name", header: "Username" },
    { field: "assignmentOptions", header: "Assign labeler", render: (labeler) => (
      <div className="d-flex p-2 justify-content-between align-items-center">
        <select
          id="assignmentSelect"
          className="form-select w-auto m-0"
          value={selectedCustomAssignments[labeler.id] || ""}
          onChange={(e) => handleCustomLabelerAssignmentChange(labeler.id, e.target.value)}
        >
          <option value="">-- Select Assignment --</option>
          {assignments.map((assignment) => (
            <option key={assignment.id} value={assignment.id}>
              {formatAssignmentOption(assignment)}
            </option>
          ))}
        </select>

        <button
          className="btn btn-success ms-2"
          onClick={() => handleAssignLabeler(labeler.id, selectedCustomAssignments[labeler.id])}
          disabled={!selectedCustomAssignments[labeler.id]}
        >
          <i className="fas fa-user-plus me-2"></i> Assign labeler
        </button>
      </div>
    )}
  ];

  // Define columns for assigned labelers table
  const assignedLabelerColumns = [
    { field: "labelerName", header: "Username" },
    { field: "videoGroupName", header: "Video Group" },
    { field: "subjectName", header: "Subject" },
    { field: "details", header: "Assignment Details", render: (item) => (
      <NavigateButton path={`/assignments/${item.assignmentId}`} actionType="Details" />
    )},
    { field: "actions", header: "Actions", render: (item) => (
      <DeleteButton 
        onClick={() => handleUnassignLabeler(item.assignmentId, item.labelerId)}
        itemType={`labeler "${item.labelerName}" from assignment`}
      />
    )}
  ];

  const fetchData = async () => {
    try {
      setLoading(true);
      const [labelerRes, unassignedLabelersRes, assignmentsRes] = await Promise.all([
        httpClient.get(`/project/${projectId}/labelers`),
        httpClient.get(`/project/${projectId}/unassigned-labelers`),
        httpClient.get(`/project/${projectId}/SubjectVideoGroupAssignments`)
      ]);

      setLabelers(labelerRes.data);
      setUnassignedLabelers(unassignedLabelersRes.data);
      setAssignments(assignmentsRes.data);
    } catch (err) {
      addNotification("Failed to load labeler data", "error");
    } finally {
      setLoading(false);
    }
  };

  const handleCustomLabelerAssignmentChange = (labelerId, assignmentId) => {
    if (!Number.isInteger(Number(labelerId))) return;

    setSelectedCustomAssignment(prev => {
      const updated = { ...prev };
      
      if (!assignmentId || !Number.isInteger(Number(assignmentId))) {
        delete updated[labelerId];
      } else {
        updated[labelerId] = assignmentId;
      }
      
      return updated;
    });
  };

  // Unified function to handle all types of labeler assignments
  const handleAssignLabeler = async (labelerId, assignmentId) => {
    // For the form in the card
    if (!labelerId && !assignmentId) {
      if (!selectedLabeler || !selectedAssignment) {
        addNotification("Please select both a labeler and an assignment", "error");
        return;
      }
      labelerId = selectedLabeler;
      assignmentId = selectedAssignment;
    }

    // Validation
    if (!labelerId || !assignmentId) {
      addNotification("Please select both a labeler and an assignment", "error");
      return;
    }

    const success = await assignLabelerToAssignment(labelerId, assignmentId);
    
    if (success) {
      // Clear form selections if this was from the form
      if (labelerId === selectedLabeler) {
        setSelectedLabeler("");
        setSelectedAssignment("");
      }
      
      // Clear from custom assignments if applicable
      setSelectedCustomAssignment(prev => {
        const updated = { ...prev };
        delete updated[labelerId];
        return updated;
      });
      
      fetchData();
      addNotification("Labeler assigned successfully!", "success");
    }
  };

  const handleAllSelectedAssignments = async () => {
    const entries = Object.entries(selectedCustomAssignments);

    if (entries.length === 0) {
      addNotification("No labelers have been assigned to any assignment.", "error");
      return;
    }

    try {
      const results = await Promise.all(
        entries.map(([labelerId, assignmentId]) => 
          assignLabelerToAssignment(labelerId, assignmentId)
        )
      );
      
      const allSucceeded = results.every(result => result === true);
      
      if (allSucceeded) {
        setSelectedCustomAssignment({});
        fetchData();
        addNotification("All labelers assigned successfully!", "success");
      } else {
        addNotification("One or more assignments failed.", "error");
      }
    } catch (error) {
      addNotification("Failed to process assignments", "error");
    }
  };

  const handleDistributeLabelers = async () => {
    try {
      await httpClient.post(`/project/${projectId}/distribute`);
      fetchData();
      setSelectedCustomAssignment({});
      addNotification("Labelers distributed successfully!", "success");
    } catch (error) {
      addNotification("Failed to distribute labelers", "error");
    }
  };

  const handleUnassignLabeler = async (assignmentId, labelerId) => {
    try {
      await httpClient.delete(`/SubjectVideoGroupAssignment/${assignmentId}/unassign-labeler/${labelerId}`);
      fetchData();
    } catch (error) {
      addNotification("Failed to unassign labeler", "error");
    }
  };

  const handleUnassignAllLabelers = async () => {
    try {
      await httpClient.post(`/project/${projectId}/unassign-all`);
      fetchData();
      addNotification("All labelers unassigned successfully!", "success");
    } catch (error) {
      addNotification("Failed to unassign all labelers", "error");
    }
  };

  useEffect(() => {
    fetchData();
  }, [projectId]);

  if (loading) {
    return (
      <div className="d-flex justify-content-center my-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading labeler data...</span>
        </div>
      </div>
    );
  }

  // Prepare assigned labelers data for rendering
  const assignedLabelerRows = assignments
    .filter(assignment => assignment.labelers && assignment.labelers.length > 0)
    .flatMap(assignment => 
      assignment.labelers.map(labeler => ({
        labelerId: labeler.id,
        labelerName: labeler.name,
        videoGroupName: assignment.videoGroupName || "Unknown",
        subjectName: assignment.subjectName || "Unknown",
        assignmentId: assignment.id,
      }))
    );

  const hasAssignedLabelers = assignedLabelerRows.length > 0;

  return (
    <div className="labelers">
      {/* Remove the error alert since we're using notifications now */}
      
      {/* Assignment form */}
      <div className="card shadow-sm mb-4" style={{ marginTop: "25px" }}>
        <div className="card-header text-white" style={{ background: "var(--gradient-blue)" }}>
          <h5 className="card-title mb-0">Assign Labeler to Assignment</h5>
        </div>
        <div className="card-body">
          {/* Remove the assignmentError alert since we're using notifications now */}
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
                  onChange={e => setSelectedLabeler(e.target.value)}
                >
                  <option value="">-- Select Labeler --</option>
                  {labelers.map(labeler => (
                    <option key={labeler.id} value={labeler.id}>
                      {labeler.name} (ID: {labeler.id})
                    </option>
                  ))}
                </select>
              </div>
              <div className="col-md-6">
                <label htmlFor="assignmentSelect" className="form-label">
                  Select Assignment:
                </label>
                <select
                  id="assignmentSelect"
                  className="form-select"
                  value={selectedAssignment}
                  onChange={e => setSelectedAssignment(e.target.value)}
                >
                  <option value="">-- Select Assignment --</option>
                  {assignments.map(assignment => (
                    <option key={assignment.id} value={assignment.id}>
                      {formatAssignmentOption(assignment)}
                    </option>
                  ))}
                </select>
              </div>
            </div>
            <button
              className="btn btn-success"
              disabled={!selectedLabeler || !selectedAssignment}
              onClick={() => handleAssignLabeler()}
              style={{ minWidth: "200px" }}
            >
              <i className="fas fa-user-plus me-2"></i>Assign Labeler
            </button>
          </div>
        </div>
      </div>

      {/* Unassigned Labelers section */}
      <div className="d-flex justify-content-between align-items-center m-3" style={{ minHeight: "56px" }}>
        <h3 className="mb-0">Unassigned Labelers</h3>
        <div className="d-flex align-items-center gap-2">
          {unassignedLabelers.length > 0 && (
            <button className="btn btn-primary px-3 text-nowrap" onClick={handleDistributeLabelers}>
              <i className="fa-solid fa-wand-magic-sparkles me-2"></i>
              Distribute Labelers
            </button>
          )}

          {Object.keys(selectedCustomAssignments).length > 0 && (
            <button className="btn btn-success px-3 text-nowrap" onClick={handleAllSelectedAssignments}>
              <i className="fas fa-user-plus me-2"></i>
              Assign all selected
            </button>
          )}
        </div>
      </div>

      {unassignedLabelers.length > 0 ? (
        <DataTable columns={LabelerColumns} data={unassignedLabelers} />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          There are no labelers awaiting assignment
        </div>
      )}

      {/* All Labelers section */}
      <div className="d-flex justify-content-between align-items-center m-3 mt-5" style={{ minHeight: "56px" }}>
        <h3 className="mb-0">All Labelers</h3>
        <div className="d-flex align-items-center gap-2">
          {labelers.length > 0 && Object.keys(selectedCustomAssignments).length > 0 && (
            <button className="btn btn-success px-3 text-nowrap" onClick={handleAllSelectedAssignments}>
              <i className="fas fa-user-plus me-2"></i>
              Assign all selected
            </button>
          )}
        </div>
      </div>

      {labelers.length > 0 ? (
        <DataTable columns={LabelerColumns} data={labelers} />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          No labelers available in this project
        </div>
      )}

      {/* Assigned Labelers section */}
      <div className="row align-items-center mb-3 mt-4">
        <div className="col">
          <h3 className="mb-0">Assigned Labelers</h3>
        </div>
        <div className="col-auto">
          {hasAssignedLabelers && (
            <button className="btn btn-danger text-nowrap" onClick={handleUnassignAllLabelers}>
              <i className="fa-solid fa-user-xmark me-1"></i>
              Unassign All Labelers
            </button>
          )}
        </div>
      </div>

      {hasAssignedLabelers ? (
        <DataTable 
          columns={assignedLabelerColumns} 
          data={assignedLabelerRows} 
        />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          No labelers found in assignments
        </div>
      )}
    </div>
  );
};

export default ProjectLabelersTab;