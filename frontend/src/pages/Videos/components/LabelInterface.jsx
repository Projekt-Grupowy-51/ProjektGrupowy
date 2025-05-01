import { getTextColor } from "../utils";
import DataTable from "../../../components/DataTable.jsx";
import DeleteButton from "../../../components/DeleteButton.jsx";
import { useTranslation } from "react-i18next";

const LabelInterface = ({ labels, assignedLabels, labelActions }) => {
    const { t } = useTranslation(['videos', 'common']);

    const labelColumns = [
        { field: "labelName", header: t('table.label') },
        { field: "labelerName", header: t('table.labeler') },
        { field: "subjectName", header: t('table.subject') },
        { field: "start", header: t('table.start') },
        { field: "end", header: t('table.end') },
        {
            field: "insDate",
            header: t('table.ins_date'),
            render: (label) => new Date(label.insDate).toLocaleString(),
        },
        { field: "videoId", header: t('table.videoId') }
    ];

    return (
        <>
            <div className="labels-container">
                {labels.map((label) => (
                    <button
                        key={label.id}
                        className="btn label-btn"
                        style={{
                            backgroundColor: label.colorHex,
                            color: getTextColor(label.colorHex),
                        }}
                        onClick={() => labelActions.handleLabelClick(label.id)}
                    >
                        {label.name} [{label.shortcut}]
                    </button>
                ))}
            </div>

            <div className="assigned-labels">
                <h3>Assigned Labels:</h3>
                <DataTable
                    columns={labelColumns}
                    data={assignedLabels}
                    navigateButton={(label) => (
                        <DeleteButton
                            onClick={() => labelActions.handleDelete(label.id)}
                            itemType={`label ${label.labelName}`}
                        />
                    )}
                />
            </div>
        </>
    );
};

export default LabelInterface;