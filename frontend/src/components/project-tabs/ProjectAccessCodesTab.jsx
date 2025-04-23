import React, { useState, useEffect } from "react";
import httpClient from "../../httpclient";
import { useNotification } from "../../context/NotificationContext";

const ProjectAccessCodesTab = ({
  projectId,
  accessCodes = [],
  onSuccess,
  onError,
}) => {
  const [creationError, setCreationError] = useState("");
  const [codeExpiration, setCodeExpiration] = useState(0);
  const [customCodeExpiration, setCustomCodeExpiration] = useState(0);
  const [visibleCodes, setVisibleCodes] = useState({});
  const [localAccessCodes, setLocalAccessCodes] = useState([]);
  const [loading, setLoading] = useState(true);
  const { addNotification } = useNotification();

  // Fetch access codes if they're not provided via props
  useEffect(() => {
    if (accessCodes && accessCodes.length > 0) {
      setLocalAccessCodes(accessCodes);
      setLoading(false);
    } else {
      fetchAccessCodes();
    }
  }, [projectId]);

  const fetchAccessCodes = async () => {
    try {
      setLoading(true);
      const response = await httpClient.get(`/AccessCode/project/${projectId}`);
      setLocalAccessCodes(response.data || []);
    } catch (error) {
      const errorMessage =
        error.response?.data?.message || "Failed to load access codes";
      if (onError) {
        onError(error);
      } else {
        //addNotification(errorMessage, "error");
      }
    } finally {
      setLoading(false);
    }
  };

  // const notifySuccess = (message) => {
  //   if (onSuccess) {
  //     onSuccess(message);
  //   } else {
  //     addNotification(message, "success");
  //   }
  // };

  const notifyError = (error) => {
    if (onError) {
      onError(error);
    } else {
      // addNotification(
      //   error.response?.data?.message || "An error occurred",
      //   "error"
      // );
    }
  };

  const handleCreateAccessCode = async () => {
    setCreationError("");

    try {
      const json = {
        projectId: parseInt(projectId),
        expiration: parseInt(codeExpiration),
        customExpiration: parseInt(customCodeExpiration),
      };
      await httpClient.post("/AccessCode/project", json);
      setCodeExpiration(0);
      //notifySuccess("Access code created successfully!");

      // Refresh the codes list after creating a new one
      fetchAccessCodes();
    } catch (error) {
      setCreationError(
        error.response?.data?.message || "Failed to create code"
      );
      notifyError(error);
    }
  };

  const handleCustomCodeExpirationChange = (e) => {
    if (e.target.value < 0) {
      setCustomCodeExpiration(0);
    } else if (e.target.value > 3600) {
      setCustomCodeExpiration(3600);
    } else {
      setCustomCodeExpiration(e.target.value);
    }
  };

  const handleCopyCode = async (code) => {
    try {
      await navigator.clipboard.writeText(code);
      addNotification("Code copied to clipboard!", "success");
    } catch (err) {
      console.error("Failed to copy code:", err);
      addNotification("Failed to copy code. Please try again.", "error");
    }
  };

  const handleRetireCode = async (code) => {
    try {
      await httpClient.put(`/AccessCode/${code}/retire`);
      fetchAccessCodes();
    } catch (error) {
      console.error("Failed to retire code:", error);
      notifyError(error);
    }
  };

  const toggleCodeVisibility = (code) => {
    setVisibleCodes((prev) => ({
      ...prev,
      [code]: !prev[code],
    }));
  };

  return (
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
                  codeExpiration === 0 ? "btn-primary" : "btn-outline-primary"
                }`}
                onClick={() => setCodeExpiration(0)}
              >
                14 Days
              </button>
              <button
                className={`btn ${
                  codeExpiration === 1 ? "btn-primary" : "btn-outline-primary"
                }`}
                onClick={() => setCodeExpiration(1)}
              >
                30 Days
              </button>
              <button
                className={`btn ${
                  codeExpiration === 3 ? "btn-primary" : "btn-outline-primary"
                }`}
                onClick={() => setCodeExpiration(3)}
              >
                Custom
              </button>
              <button
                className={`btn ${
                  codeExpiration === 2 ? "btn-primary" : "btn-outline-primary"
                }`}
                onClick={() => setCodeExpiration(2)}
              >
                Unlimited
              </button>
            </div>
            {codeExpiration === 3 && (
              <div className="input-group">
                <span
                  className="input-group-text"
                  style={{ height: "calc(1.5em + 0.75rem + 2px)" }}
                >
                  Days
                </span>
                <input
                  type="number"
                  min="0"
                  onInput={handleCustomCodeExpirationChange}
                  max="3600"
                  step="1"
                  className="form-control"
                  style={{ height: "calc(1.5em + 0.75rem + 2px)" }}
                />
              </div>
            )}
            <button
              className="btn btn-success"
              onClick={handleCreateAccessCode}
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

      {loading ? (
        <div className="d-flex justify-content-center my-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Loading access codes...</span>
          </div>
        </div>
      ) : localAccessCodes.length > 0 ? (
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
            {localAccessCodes
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
                      <code
                        style={{
                          filter: visibleCodes[code.code]
                            ? "blur(0px)"
                            : "blur(6px)",
                          transition: "filter 0.3s ease",
                          userSelect: visibleCodes[code.code] ? "auto" : "none",
                          pointerEvents: visibleCodes[code.code]
                            ? "auto"
                            : "none",
                        }}
                      >
                        {code.code}
                      </code>
                      <button
                        className="btn btn-link p-0"
                        onClick={() => toggleCodeVisibility(code.code)}
                        title={
                          visibleCodes[code.code] ? "Hide code" : "Show code"
                        }
                      >
                        <i
                          className={`fas fa-eye${
                            visibleCodes[code.code] ? "-slash" : ""
                          }`}
                        />
                      </button>
                    </div>
                  </td>
                  <td>{new Date(code.createdAtUtc).toLocaleString()}</td>
                  <td>
                    {code.isValid
                      ? code.expiresAtUtc
                        ? new Date(code.expiresAtUtc).toLocaleString()
                        : "Never"
                      : "Expired"}
                  </td>
                  <td>
                    {code.isValid ? (
                      <span className="badge bg-success">✓ Valid</span>
                    ) : (
                      <span className="badge bg-danger">✗ Invalid</span>
                    )}
                  </td>
                  <td>
                    <button
                      className="btn btn-outline-primary"
                      onClick={() => handleCopyCode(code.code)}
                    >
                      <i className="fas fa-copy me-1"></i>Copy
                    </button>
                    {code.isValid && (
                      <button
                        className="btn btn-outline-danger"
                        onClick={() => handleRetireCode(code.code)}
                      >
                        <i className="fa-solid fa-trash-can me-1"></i>
                        Retire
                      </button>
                    )}
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
  );
};

export default ProjectAccessCodesTab;
