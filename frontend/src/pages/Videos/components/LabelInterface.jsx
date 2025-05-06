import { getTextColor } from "../utils";
import DataTable from "../../../components/DataTable.jsx";
import DeleteButton from "../../../components/DeleteButton.jsx";
import { useTranslation } from "react-i18next";

const LabelInterface = ({ labels, assignedLabels, labelActions }) => {
    const { t } = useTranslation(['videos', 'common']);

    const labelColumns = [
        { 
            field: "labelName", 
            header: t('table.label'),
            render: (assignedLabel) => {
                const matchingLabel = labels.find((label) => label.id === assignedLabel.labelId);
                const colorHex = matchingLabel?.colorHex || "#ccc";
                return (
                    <div style={{ display: "flex", alignItems: "center", gap: "10px" }}>
                        <div
                            style={{
                                backgroundColor: colorHex,
                                width: "20px",
                                height: "20px",
                                borderRadius: "50%",
                                border: "1px solid #ccc",
                            }}
                        />
                        <span>{assignedLabel.labelName}</span>
                    </div>
                );
            }, 
        },
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
                <div className="assigned-labels-table-container">
                    <DataTable
                        columns={labelColumns}
                        data={assignedLabels}
                        navigateButton={(label) => (
                            <DeleteButton
                                onClick={() => labelActions.handleDelete(label.id)}
                                itemType={`label ${label.labelName}`}
                            />
                        )}
                        tableClassName="normal-table labels-table"
                        defaultSort={{ key: "start", direction: "asc" }} 
                    />
                </div>
            </div>
        </>
    );
};

export default LabelInterface;