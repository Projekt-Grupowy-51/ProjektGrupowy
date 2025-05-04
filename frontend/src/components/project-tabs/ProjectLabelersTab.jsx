import React, { useState, useEffect } from "react";
import NavigateButton from "../../components/NavigateButton";
import DeleteButton from "../../components/DeleteButton";
import DataTable from "../../components/DataTable";
import httpClient from "../../httpclient";
import { useNotification } from "../../context/NotificationContext";
import { useTranslation } from "react-i18next";

const ProjectLabelersTab = ({
  projectId,
  onLabelersUpdate,
}) => {
  const { addNotification } = useNotification();
  const [labelers, setLabelers] = useState([]);
  const [unassignedLabelers, setUnassignedLabelers] = useState([]);
  const [assignments, setAssignments] = useState([]);
  const [selectedLabeler, setSelectedLabeler] = useState("");
  const [selectedAssignment, setSelectedAssignment] = useState("");
  const [selectedCustomAssignments, setSelectedCustomAssignment] = useState({});
  const { t } = useTranslation(['common', 'project']);

  const formatAssignmentOption = (assignment) =>
    `Assignment #${assignment.id} - Subject: ${
      assignment.subjectName || "Unknown"
    } (ID: ${assignment.subjectId}), ` +
    `Video Group: ${assignment.videoGroupName || "Unknown"} (ID: ${
      assignment.videoGroupId
    })`;

  const assignLabelerToAssignment = async (labelerId, assignmentId) => {
    await httpClient.post(
      `/SubjectVideoGroupAssignment/${assignmentId}/assign-labeler/${labelerId}`
    );
    return true;
  };

  const LabelerColumns = [
    { field: "name", header: "Username" },
    {
      field: "assignmentOptions",
      header: "Assign labeler",
      render: (labeler) => (
        <div className="d-flex p-2 justify-content-between align-items-center">
          <select
            id="assignmentSelect"
            className="form-select w-auto m-0"
            value={selectedCustomAssignments[labeler.id] || ""}
            onChange={(e) =>
              handleCustomLabelerAssignmentChange(labeler.id, e.target.value)
            }
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
            onClick={() =>
              handleAssignLabeler(
                labeler.id,
                selectedCustomAssignments[labeler.id]
              )
            }
            disabled={!selectedCustomAssignments[labeler.id]}
          >
            <i className="fas fa-user-plus me-2"></i> Assign labeler
          </button>
        </div>
      ),
    },
  ];

  const assignedLabelerColumns = [
    { field: "labelerName", header: "Username" },
    { field: "videoGroupName", header: "Video Group" },
    { field: "subjectName", header: "Subject" },
    {
      field: "details",
      header: "Assignment Details",
      render: (item) => (
        <NavigateButton
          path={`/assignments/${item.assignmentId}`}
          actionType="Details"
        />
      ),
    },
    {
      field: "actions",
      header: "Actions",
      render: (item) => (
        <DeleteButton
          onClick={() =>
            handleUnassignLabeler(item.assignmentId, item.labelerId)
          }
          itemType={`labeler "${item.labelerName}" from assignment`}
        />
      ),
    },
  ];

  const fetchData = async () => {
    const [labelerRes, unassignedLabelersRes, assignmentsRes] =
      await Promise.all([
        httpClient.get(`/project/${projectId}/labelers`),
        httpClient.get(`/project/${projectId}/unassigned-labelers`),
        httpClient.get(`/project/${projectId}/SubjectVideoGroupAssignments`),
      ]);

    setLabelers(labelerRes.data);
    setUnassignedLabelers(unassignedLabelersRes.data);
    setAssignments(assignmentsRes.data);

    if (onLabelersUpdate) {
      onLabelersUpdate(labelerRes.data.length);
    }
  };

  const handleCustomLabelerAssignmentChange = (labelerId, assignmentId) => {
    setSelectedCustomAssignment((prev) => {
      const updated = { ...prev };

      if (!assignmentId || !Number.isInteger(Number(assignmentId))) {
        delete updated[labelerId];
      } else {
        updated[labelerId] = assignmentId;
      }

      return updated;
    });
  };

  const handleAssignLabeler = async (labelerId, assignmentId) => {
    if (!labelerId && !assignmentId) {
      if (!selectedLabeler || !selectedAssignment) {
        addNotification(
          "Please select both a labeler and an assignment",
          "error"
        );
        return;
      }
      labelerId = selectedLabeler;
      assignmentId = selectedAssignment;
    }

    if (!labelerId || !assignmentId) {
      addNotification(
        "Please select both a labeler and an assignment",
        "error"
      );
      return;
    }

    const success = await assignLabelerToAssignment(labelerId, assignmentId);

    if (success) {
      if (labelerId === selectedLabeler) {
        setSelectedLabeler("");
        setSelectedAssignment("");
      }

      setSelectedCustomAssignment((prev) => {
        const updated = { ...prev };
        delete updated[labelerId];
        return updated;
      });

      fetchData();
    }
  };

  const handleAllSelectedAssignments = async () => {
    const entries = Object.entries(selectedCustomAssignments);

    if (entries.length === 0) {
      addNotification(
        "No labelers have been assigned to any assignment.",
        "error"
      );
      return;
    }

    await Promise.all(
      entries.map(([labelerId, assignmentId]) =>
        assignLabelerToAssignment(labelerId, assignmentId)
      )
    );

    setSelectedCustomAssignment({});
    fetchData();
  };

  const handleDistributeLabelers = async () => {
    await httpClient.post(`/project/${projectId}/distribute`);
    fetchData();
    setSelectedCustomAssignment({});
  };

  const handleUnassignLabeler = async (assignmentId, labelerId) => {
    await httpClient.delete(
      `/SubjectVideoGroupAssignment/${assignmentId}/unassign-labeler/${labelerId}`
    );
    fetchData();
  };

  const handleUnassignAllLabelers = async () => {
    await httpClient.post(`/project/${projectId}/unassign-all`);
    fetchData();
  };

  useEffect(() => {
    fetchData();
  }, [projectId]);

  const assignedLabelerRows = assignments
    .filter(
      (assignment) => assignment.labelers && assignment.labelers.length > 0
    )
    .flatMap((assignment) =>
      assignment.labelers.map((labeler) => ({
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
      <div className="card shadow-sm mb-4" style={{ marginTop: "25px" }}>
        <div
          className="card-header text-white"
          style={{ background: "var(--gradient-blue)" }}
        >
          <h5 className="card-title mb-0">Assign Labeler to Assignment</h5>
        </div>
        <div className="card-body">
          <div className="assignment-form">
            <div className="row mb-3">
              <div className="col-md-6">
                <label htmlFor="labelerSelect" className="form-label">
                  {t('projects:labeler_tab.select_labeler')}:
                </label>
                <select
                  id="labelerSelect"
                  className="form-select"
                  value={selectedLabeler}
                  onChange={(e) => setSelectedLabeler(e.target.value)}
                >
                  <option value="">-- {t('projects:labeler_tab.select_labeler')} --</option>
                  {labelers.map((labeler) => (
                    <option key={labeler.id} value={labeler.id}>
                      {labeler.name} (ID: {labeler.id})
                    </option>
                  ))}
                </select>
              </div>
              <div className="col-md-6">
                <label htmlFor="assignmentSelect" className="form-label">
                  {t('projects:labeler_tab.select_assignment')}:
                </label>
                <select
                  id="assignmentSelect"
                  className="form-select"
                  value={selectedAssignment}
                  onChange={(e) => setSelectedAssignment(e.target.value)}
                >
                  <option value="">-- {t('projects:labeler_tab.select_assignment')} --</option>
                  {assignments.map((assignment) => (
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

      <div
        className="d-flex justify-content-between align-items-center m-3"
        style={{ minHeight: "56px" }}
      >
        <h3 className="mb-0">{t('projects:labeler_tab.unassigned_labelers')}</h3>
        <div className="d-flex align-items-center gap-2">
          {unassignedLabelers.length > 0 && (
            <button
              className="btn btn-primary px-3 text-nowrap"
              onClick={handleDistributeLabelers}
            >
              <i className="fa-solid fa-wand-magic-sparkles me-2"></i>
              {t('projects:labeler_tab.distribute_labelers')}
            </button>
          )}

          {Object.keys(selectedCustomAssignments).length > 0 && (
            <button
              className="btn btn-success px-3 text-nowrap"
              onClick={handleAllSelectedAssignments}
            >
              <i className="fas fa-user-plus me-2"></i>
              {t('projects:labeler_tab.assign_all_selected')}
            </button>
          )}
        </div>
      </div>

      {unassignedLabelers.length > 0 ? (
        <DataTable
          showRowNumbers={true}
          columns={LabelerColumns}
          data={unassignedLabelers}
        />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          {t('projects:not_found.unassigned_labelers')}
        </div>
      )}

      <div
        className="d-flex justify-content-between align-items-center m-3 mt-5"
        style={{ minHeight: "56px" }}
      >
        <h3 className="mb-0">{t('projects:labeler_tab.all_labelers')}</h3>
        <div className="d-flex align-items-center gap-2">
          {labelers.length > 0 &&
            Object.keys(selectedCustomAssignments).length > 0 && (
              <button
                className="btn btn-success px-3 text-nowrap"
                onClick={handleAllSelectedAssignments}
              >
                <i className="fas fa-user-plus me-2"></i>
                {t('projects:labeler_tab.assign_all_selected')}
              </button>
            )}
        </div>
      </div>

      {labelers.length > 0 ? (
        <DataTable
          showRowNumbers={true}
          columns={LabelerColumns}
          data={labelers}
        />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          {t('projects:not_found.all_labelers')}
        </div>
      )}

      <div className="row align-items-center mb-3 mt-4">
        <div className="col">
          <h3 className="mb-0">{t('projects:labeler_tab.assigned_labelers')}</h3>
        </div>
        <div className="col-auto">
          {hasAssignedLabelers && (
            <button
              className="btn btn-danger text-nowrap"
              onClick={handleUnassignAllLabelers}
            >
              <i className="fa-solid fa-user-xmark me-1"></i>
              {t('projects:labeler_tab.unassign_all')}
            </button>
          )}
        </div>
      </div>

      {hasAssignedLabelers ? (
        <DataTable
          showRowNumbers={true}
          columns={assignedLabelerColumns}
          data={assignedLabelerRows}
        />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          {t('projects:not_found.assigned_labelers')}
        </div>
      )}
    </div>
  );
};

export default ProjectLabelersTab;
