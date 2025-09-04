import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { Card, Button, Table } from '../../../ui';
import { EmptyState, LoadingSpinner } from '../../../common';
import { useProjectAccessCodes } from '../../../../hooks/useProjectAccessCodes';

const ProjectAccessCodesTab = ({ projectId }) => {
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [expirationDays, setExpirationDays] = useState(14);

  const {
    accessCodes,
    visibleCodes,
    loading,
    createAccessCode,
    copyCode,
    retireCode,
    toggleVisibility
  } = useProjectAccessCodes(projectId);

  const handleCreate = async () => {
    await createAccessCode(expirationDays);
    setShowCreateForm(false);
  };

  if (loading) return <LoadingSpinner message="Loading access codes..." />;

  return (
    <div>
      <Card>
        <Card.Header>
          <div className="d-flex justify-content-between align-items-center">
            <Card.Title level={5}>
              Access Codes ({accessCodes?.length || 0})
            </Card.Title>
            <Button
              variant="primary"
              size="sm"
              onClick={() => setShowCreateForm(!showCreateForm)}
            >
              {showCreateForm ? 'Cancel' : 'Generate Code'}
            </Button>
          </div>
        </Card.Header>
        <Card.Body>
          {showCreateForm && (
            <div className="mb-4 p-3 bg-light rounded">
              <div className="row g-3 align-items-end">
                <div className="col-md-6">
                  <label className="form-label">Expiration (days)</label>
                  <select 
                    className="form-select"
                    value={expirationDays}
                    onChange={(e) => setExpirationDays(parseInt(e.target.value))}
                  >
                    <option value={14}>14 days</option>
                    <option value={30}>30 days</option>
                    <option value={-1}>Never expires</option>
                  </select>
                </div>
                <div className="col-md-6">
                  <Button
                    variant="success"
                    onClick={handleCreate}
                    className="w-100"
                  >
                    Create Access Code
                  </Button>
                </div>
              </div>
            </div>
          )}

          {!accessCodes || accessCodes.length === 0 ? (
            <EmptyState
              icon="fas fa-key"
              message="No access codes generated yet"
            />
          ) : (
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>Code</Table.Cell>
                  <Table.Cell header>Created</Table.Cell>
                  <Table.Cell header>Expires</Table.Cell>
                  <Table.Cell header>Status</Table.Cell>
                  <Table.Cell header>Actions</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {accessCodes?.map((code) => (
                  <Table.Row key={code.code}>
                    <Table.Cell>
                      <div className="d-flex align-items-center gap-2">
                        <code
                          style={{
                            filter: visibleCodes[code.code] ? "none" : "blur(4px)",
                            cursor: "pointer"
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
                          {visibleCodes[code.code] ? '👁️' : '🔒'}
                        </Button>
                      </div>
                    </Table.Cell>
                    <Table.Cell>
                      {new Date(code.createdAtUtc).toLocaleDateString()}
                    </Table.Cell>
                    <Table.Cell>
                      {code.expiresAtUtc 
                        ? new Date(code.expiresAtUtc).toLocaleDateString()
                        : 'Never'
                      }
                    </Table.Cell>
                    <Table.Cell>
                      <span className={`badge ${code.isValid ? 'bg-success' : 'bg-danger'}`}>
                        {code.isValid ? 'Valid' : 'Expired'}
                      </span>
                    </Table.Cell>
                    <Table.Cell>
                      <div className="d-flex gap-2">
                        <Button
                          size="sm"
                          variant="primary"
                          onClick={() => copyCode(code.code)}
                        >
                          Copy
                        </Button>
                        {code.isValid && (
                          <Button
                            size="sm"
                            variant="outline-danger"
                            onClick={() => {
                              if (confirm('Retire this access code?')) {
                                retireCode(code.code);
                              }
                            }}
                          >
                            Retire
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
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired
};

export default ProjectAccessCodesTab;
