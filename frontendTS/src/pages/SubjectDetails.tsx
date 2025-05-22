import React from "react";
import { useParams } from "react-router-dom";
import NavigateButton from "../components/NavigateButton";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import "./css/ScientistProjects.css";
import { useTranslation } from 'react-i18next';
import useSubject from "../hooks/useSubject";
import { deleteLabel } from "../services/api/labelService";
import useSubjectLabels from "../hooks/useSubjectLabels";

const SubjectDetails = () => {
  const { t } = useTranslation(['subjects', 'common']);
  const { id } = useParams();
  const { subject } = useSubject(id ? parseInt(id) : undefined);
  const { labels, fetchLabels } = useSubjectLabels(id ? parseInt(id) : undefined);

  const labelColumns = [
    { field: "name", header: t('subjects:columns.name') },
    { field: "shortcut", header: t('subjects:columns.shortcut') },
    {
      field: "colorHex",
      header: t('subjects:columns.color'),
      render: (label) => (
          <div
              style={{
                backgroundColor: label.colorHex,
                width: "20px",
                height: "20px",
                display: "inline-block",
                marginRight: "5px",
                borderRadius: "3px",
              }}
          ></div>
      ),
    },
  ];



  const handleDeleteLabel = async (labelId: number) => {
    await deleteLabel(labelId);
    fetchLabels();
  };

  return (
      <div className="container">
        <div className="content">
            <h1 className="heading mb-4">{subject?.name}</h1>

          <div className="card shadow-sm mb-4">
            <div className="card-header bg-info text-white" style={{ background: "var(--gradient-blue)" }}>
              <h5 className="card-title mb-0">{t('subjects:details.title')}</h5>
            </div>
            <div className="card-body">
              <p className="card-text">
                <strong>{t('subjects:details.description')}:</strong> {subject?.description}
              </p>
            </div>
          </div>

          <div className="d-flex justify-content-between mb-2">
            <NavigateButton path={`/labels/add?subjectId=${id}`} actionType="Add" />
            <NavigateButton path={`/projects/${subject?.projectId}`} actionType="Back" />
          </div>

          <h2 className="section-title">{t('subjects:labels.title')}</h2>

          {labels.length > 0 ? (
              <DataTable
                  showRowNumbers={true}
                  columns={labelColumns}
                  data={labels}
                  navigateButton={(label) => (
                      <NavigateButton
                          path={`/labels/edit/${label.id}`}
                          actionType="Edit"
                      />
                  )}
                  deleteButton={(label) => (
                      <DeleteButton
                          onClick={() => handleDeleteLabel(label.id)}
                          itemType={t('subjects:labels.label')}
                      />
                  )}
              />
          ) : (
              <div className="alert alert-info">
                <i className="fas fa-info-circle me-2"></i>
                {t('subjects:labels.empty')}
              </div>
          )}
        </div>
      </div>
  );
};

export default SubjectDetails;
