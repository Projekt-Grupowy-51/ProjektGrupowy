import React from "react";
import NavigateButton from "../NavigateButton";
import DeleteButton from "../DeleteButton";
import { useNavigate } from "react-router-dom";
import httpClient from "../../httpclient";
import { useTranslation } from "react-i18next";

const ProjectDetailsTab = ({ project, reports, onReportDeleted }) => {
    const navigate = useNavigate();
    const { t } = useTranslation(['common', 'projects']);

    const handleDeleteProject = async () => {
        await httpClient.delete(`/Project/${project.id}`);
        navigate("/projects");
    };

    const handleGenerateReport = async () => {
        await httpClient.post(`/projectreport/${project.id}/generate-report`);
    };

    const handleDeleteReport = async (reportId) => {
        await httpClient.delete(`/projectreport/${reportId}`);
        if (onReportDeleted) {
            onReportDeleted();
        }
    };

    const downloadReport = async (reportId) => {
        const response = await httpClient.get(
            `/projectreport/download/${reportId}`,
            {
                responseType: "blob",
            }
        );
        const blob = new Blob([response.data], { type: "application/json" });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `report_${reportId}.json`;
        a.click();
        window.URL.revokeObjectURL(url);
    };

    return (
        <div>
            <div className="card shadow-sm mb-3">
                <div
                    className="card-header text-white"
                    style={{ background: "var(--gradient-blue)" }}
                >
                    <h5 className="card-title mb-0">{t('projects:project_details')}</h5>
                </div>
                <div className="card-body">
                    <p className="card-text">
                        <strong>Description:</strong> {project.description}
                    </p>
                    <div className="d-flex mt-3">
                        <NavigateButton
                            actionType='Edit'
                            value={t('common:buttons.edit')}
                            path={`/projects/edit/${project.id}`}
                        />
                        <NavigateButton actionType='Back' value={t('common:buttons.back')} />
                        <DeleteButton onClick={handleDeleteProject} />
                    </div>
                </div>
            </div>
            <div className="card shadow-sm">
                <div
                    className="card-header text-white"
                    style={{ background: "var(--gradient-blue)" }}
                >
                    <h5 className="card-title mb-0">Reports</h5>
                </div>
                <div className="card-body">
                    <div>
                        <button
                            className="btn btn-primary mb-3"
                            onClick={handleGenerateReport}
                        >
                            <i className="fa-solid fa-file-export me-2"></i>
                            Generate Report
                        </button>
                    </div>
                    <div>
                        <table className="table table-striped">
                            <thead>
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">Name</th>
                                    <th scope="col">Created At</th>
                                    <th scope="col">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {reports
                                    .slice()
                                    .sort((a, b) => new Date(b.createdAtUtc) - new Date(a.createdAtUtc))
                                    .map((report, index) => (
                                        <tr key={report.id}>
                                            <td>{index + 1}</td>
                                            <td>{report.name}</td>
                                            <td>{new Date(report.createdAtUtc).toLocaleString()}</td>
                                            <td>
                                                <button
                                                    className="btn btn-secondary me-2"
                                                    onClick={() => downloadReport(report.id)}
                                                >
                                                    <i className="fa-solid fa-download me-2"></i>
                                                    Download
                                                </button>
                                                <button
                                                    className="btn btn-danger"
                                                    onClick={() => handleDeleteReport(report.id)}
                                                >
                                                    <i className="fa-solid fa-trash-can me-2"></i> Delete
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProjectDetailsTab;
