import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import DataTable from "../components/DataTable";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from "react-i18next";

const VideoDetails = () => {
    const { id: videoId } = useParams();
    const [videoData, setVideoData] = useState(null);
    const [videoStream, setVideoStream] = useState(null);
    const [labels, setLabels] = useState([]);
    const videoRef = useRef(null);
    const { t } = useTranslation(['videos', 'common']);

    useEffect(() => {
        fetchVideoDetails(videoId);
        fetchVideoStream(videoId);
        fetchAssignedLabels(videoId);
    }, [videoId]);

    const fetchVideoDetails = async (videoId) => {
        const response = await httpClient.get(`/Video/${videoId}`);
        setVideoData(response.data);
    };

    async function fetchVideoStream(videoId) {
        const response = await httpClient.get(`/Video/${videoId}/stream`, {
            responseType: "blob",
        });
        const streamUrl = URL.createObjectURL(response.data);
        setVideoStream(streamUrl);
    }

    const fetchAssignedLabels = async (videoId) => {
        const response = await httpClient.get(`/Video/${videoId}/assignedlabels`);
        setLabels(response.data);
    };

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
        { field: "videoId", header: t('table.videoId')}
    ];

    if (!videoData) {
        return <div className="container text-center">{t('details.loading')}</div>;
    }

    return (
        <div className="container">
            <div className="d-flex justify-content-end mb-3">
                <NavigateButton actionType="Back" />
            </div>

            <h1 className="text-center mb-4">{videoData.title}</h1>

            <div
                className="video-container mb-4"
                style={{ position: "relative", paddingTop: "56.25%" }}
            >
                <video
                    ref={videoRef}
                    style={{
                        position: "absolute",
                        top: 0,
                        left: 0,
                        width: "100%",
                        height: "100%",
                        objectFit: "contain",
                    }}
                    controls
                    src={videoStream}
                    type="video/mp4"
                />
            </div>
            <div className="assigned-labels">
                <h3 className="">{t('details.assigned_labels')}</h3>
                <div className="assigned-labels-table">
                    <DataTable
                        showRowNumbers={true}
                        columns={labelColumns}
                        data={labels}
                    />
                </div>
            </div>
        </div>
    );
};

export default VideoDetails;
