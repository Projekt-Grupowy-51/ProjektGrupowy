import React, { useState, useEffect } from "react";
import httpClient from "../../httpclient";
import { useNotification } from "../../context/NotificationContext";
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation(['common', 'projects']);

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
        error.response?.data?.message || t('projects:access_codes.errors.load_failed');
      if (onError) {
        onError(error);
      } else {
        addNotification(errorMessage, "error");
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
      addNotification(
        error.response?.data?.message || t('common:errors.general'),
        "error"
      );
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
      notifySuccess(t('projects:access_codes.success.created'));

      // Refresh the codes list after creating a new one
      fetchAccessCodes();
    } catch (error) {
      setCreationError(
        error.response?.data?.message || t('projects:access_codes.errors.create_failed')
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
      addNotification(t('projects:access_codes.success.copied'), "success");
    } catch (err) {
      console.error("Failed to copy code:", err);
      addNotification(t('projects:access_codes.errors.copy_failed'), "error");
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
          <h5 className="card-title mb-0">{t('projects:access_codes.generate_title')}</h5>
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
                {t('projects:access_codes.durations.14_days')}
              </button>
              <button
                className={`btn ${
                  codeExpiration === 1 ? "btn-primary" : "btn-outline-primary"
                }`}
                onClick={() => setCodeExpiration(1)}
              >
                {t('projects:access_codes.durations.30_days')}
              </button>
              <button
                className={`btn ${
                  codeExpiration === 3 ? "btn-primary" : "btn-outline-primary"
                }`}
                onClick={() => setCodeExpiration(3)}
              >
                {t('projects:access_codes.durations.custom')}
              </button>
              <button
                className={`btn ${
                  codeExpiration === 2 ? "btn-primary" : "btn-outline-primary"
                }`}
                onClick={() => setCodeExpiration(2)}
              >
                {t('projects:access_codes.durations.unlimited')}
              </button>
            </div>
            {codeExpiration === 3 && (
              <div className="input-group">
                <span
                  className="input-group-text"
                  style={{ height: "calc(1.5em + 0.75rem + 2px)" }}
                >
                  {t('projects:access_codes.durations.days')}
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
              <i className="fas fa-key me-2"></i>{t('projects:access_codes.buttons.generate')}
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
            <span className="visually-hidden">{t('projects:access_codes.loading')}</span>
          </div>
        </div>
      ) : localAccessCodes.length > 0 ? (
        <table className="normal-table" id="access-codes-table">
          <thead>
            <tr>
              <th>{t('projects:access_codes.columns.code')}</th>
              <th>{t('projects:access_codes.columns.created_at')}</th>
              <th>{t('projects:access_codes.columns.expires_at')}</th>
              <th>{t('projects:access_codes.columns.status')}</th>
              <th>{t('common:actions')}</th>
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
                          visibleCodes[code.code] 
                            ? t('projects:access_codes.visibility.hide') 
                            : t('projects:access_codes.visibility.show')
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
                        : t('projects:access_codes.expiration.never')
                      : t('projects:access_codes.expiration.expired')}
                  </td>
                  <td>
                    {code.isValid ? (
                      <span className="badge bg-success">{t('projects:access_codes.status.valid')}</span>
                    ) : (
                      <span className="badge bg-danger">{t('projects:access_codes.status.invalid')}</span>
                    )}
                  </td>
                  <td>
                    <button
                      className="btn btn-outline-primary"
                      onClick={() => handleCopyCode(code.code)}
                    >
                      <i className="fas fa-copy me-1"></i>{t('common:buttons.copy')}
                    </button>
                    {code.isValid && (
                      <button
                        className="btn btn-outline-danger"
                        onClick={() => handleRetireCode(code.code)}
                      >
                        <i className="fa-solid fa-trash-can me-1"></i>
                        {t('common:buttons.retire')}
                      </button>
                    )}
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      ) : (
        <div className="alert alert-info text-center">
          <i className="fas fa-info-circle me-2"></i>{t('projects:access_codes.no_codes')}
        </div>
      )}
    </div>
  );
};

export default ProjectAccessCodesTab;
