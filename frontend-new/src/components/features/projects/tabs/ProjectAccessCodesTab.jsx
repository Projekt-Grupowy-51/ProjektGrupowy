import React, { useState } from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";
import { Card, Button, Table } from "../../../ui";
import { EmptyState, LoadingSpinner } from "../../../common";
import { useProjectAccessCodes } from "../../../../hooks/useProjectAccessCodes";

const ProjectAccessCodesTab = ({ projectId }) => {
  const { t } = useTranslation(["projects", "common"]);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [expirationType, setExpirationType] = useState(0); // 0 = In14Days, 1 = In30Days, 2 = Never
  const [customHours, setCustomHours] = useState(24);

  const {
    accessCodes,
    visibleCodes,
    loading,
    createAccessCode,
    copyCode,
    retireCode,
    toggleVisibility,
  } = useProjectAccessCodes(projectId);

  const handleCreate = async () => {
    await createAccessCode({
      expiration: expirationType,
      customExpiration: expirationType === 3 ? customHours : 0,
    });
    setShowCreateForm(false);
  };

  if (loading)
    return <LoadingSpinner message={t("projects:access_codes.loading")} />;

  return (
    <div>
      <Card>
        <Card.Header>
            <div className="d-flex justify-content-between align-items-center">
            <Card.Title level={5}>
              {t("projects:tabs.access_codes")} ({accessCodes?.length || 0})
            </Card.Title>
            <Button
              variant="primary"
              size="sm"
              onClick={() => setShowCreateForm(!showCreateForm)}
            >
              {showCreateForm
                ? t("common:buttons.cancel")
                : t("projects:access_codes.buttons.generate")}
            </Button>
          </div>
        </Card.Header>
        <Card.Body>
          {showCreateForm && (
            <div className="mb-4 p-3 bg-light rounded">
              <div className="row g-3 align-items-end">
                <div className="col-md-6">
                  <label className="form-label">
                    {t("projects:access_codes.durations.days")}
                  </label>
                  <select
                    className="form-select"
                    value={expirationType}
                    onChange={(e) =>
                      setExpirationType(parseInt(e.target.value))
                    }
                  >
                    <option value={0}>
                      {t("projects:access_codes.durations.14_days")}
                    </option>
                    <option value={1}>
                      {t("projects:access_codes.durations.30_days")}
                    </option>
                    <option value={2}>
                      {t("projects:access_codes.durations.unlimited")}
                    </option>
                    <option value={3}>
                      {t("projects:access_codes.durations.custom")}
                    </option>
                  </select>
                </div>
                {expirationType === 3 && (
                  <div className="col-md-6">
                    <label className="form-label">
                      {t("projects:access_codes.durations.custom_hours")}
                    </label>
                    <input
                      type="number"
                      className="form-control"
                      value={customHours}
                      onChange={(e) =>
                        setCustomHours(parseInt(e.target.value) || 0)
                      }
                      min="1"
                      placeholder={t(
                        "projects:access_codes.durations.hours_placeholder"
                      )}
                    />
                  </div>
                )}
                <div
                  className={expirationType === 3 ? "col-md-12" : "col-md-6"}
                >
                  <Button
                    size="sm"
                    variant="success"
                    onClick={handleCreate}
                    className="w-100"
                  >
                    {t("projects:access_codes.buttons.generate")}
                  </Button>
                </div>
              </div>
            </div>
          )}

          {!accessCodes || accessCodes.length === 0 ? (
            <EmptyState
              icon="fas fa-key"
              message={t("projects:access_codes.no_codes")}
            />
          ) : (
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>
                    {t("projects:access_codes.columns.code")}
                  </Table.Cell>
                  <Table.Cell header>
                    {t("projects:access_codes.columns.created_at")}
                  </Table.Cell>
                  <Table.Cell header>
                    {t("projects:access_codes.columns.expires_at")}
                  </Table.Cell>
                  <Table.Cell header>
                    {t("projects:access_codes.columns.status")}
                  </Table.Cell>
                  <Table.Cell header>{t("common:actions")}</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {accessCodes?.map((code) => (
                  <Table.Row key={code.code}>
                    <Table.Cell>
                      <div className="d-flex align-items-center gap-2">
                        <code
                          style={{
                            filter: visibleCodes[code.code]
                              ? "none"
                              : "blur(4px)",
                            cursor: "pointer",
                          }}
                          onClick={() => toggleVisibility(code.code)}
                        >
                          {code.code}
                        </code>
                        <Button
                          size="sm"
                          variant="link"
                          onClick={() => toggleVisibility(code.code)}
                        >
                          {visibleCodes[code.code] ? "👁️" : "🔒"}
                        </Button>
                      </div>
                    </Table.Cell>
                    <Table.Cell>
                      {new Date(code.createdAtUtc).toLocaleDateString()}
                    </Table.Cell>
                    <Table.Cell>
                      {code.expiresAtUtc
                        ? new Date(code.expiresAtUtc).toLocaleDateString()
                        : t("projects:access_codes.expiration.never")}
                    </Table.Cell>
                    <Table.Cell>
                      <span
                        className={`badge ${
                          code.isValid ? "bg-success" : "bg-danger"
                        }`}
                      >
                        {code.isValid
                          ? t("projects:access_codes.status.valid")
                          : t("projects:access_codes.expiration.expired")}
                      </span>
                    </Table.Cell>
                    <Table.Cell>
                      <div className="d-flex gap-2">
                        <Button
                          size="sm"
                          variant="primary"
                          onClick={() => copyCode(code.code)}
                        >
                          {t("common:buttons.copy")}
                        </Button>
                        {code.isValid && (
                          <Button
                            size="sm"
                            variant="outline-danger"
                            confirmAction={true}
                            confirmTitle={t("common:deleteConfirmation.title")}
                            confirmMessage={t(
                              "projects:access_codes.confirm_retire",
                              { code: code.code }
                            )}
                            confirmText={t("common:buttons.retire")}
                            cancelText={t("common:deleteConfirmation.cancel")}
                            onConfirm={() => retireCode(code.code)}
                          >
                            {t("common:buttons.retire")}
                          </Button>
                        )}
                      </div>
                    </Table.Cell>
                  </Table.Row>
                ))}
              </Table.Body>
            </Table>
          )}
        </Card.Body>
      </Card>
    </div>
  );
};

ProjectAccessCodesTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
    .isRequired,
};

export default ProjectAccessCodesTab;
