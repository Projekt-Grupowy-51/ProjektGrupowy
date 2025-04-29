import { getTextColor } from "../utils";
import DataTable from "../../../components/DataTable.jsx";
import DeleteButton from "../../../components/DeleteButton.jsx";

const LabelInterface = ({ labels, assignedLabels, labelActions }) => {
    const labelColumns = [
        { field: "labelName", header: "Label Name" },
        { field: "labelerName", header: "Labeler" },
        { field: "start", header: "Start" },
        { field: "end", header: "End" },
        { field: "insDate", header: "Date", render: (l) => new Date(l.insDate).toLocaleString() },
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