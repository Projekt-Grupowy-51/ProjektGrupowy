import React from "react";
import { useParams } from "react-router-dom";
import DeleteButton from "../components/DeleteButton";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import DataTable from "../components/DataTable";
import { useTranslation } from "react-i18next";
import { deleteVideo } from "../services/api/videoService";
import useVideoGroupDetails from "../hooks/useVideoGroupDetails";

const VideoGroupDetails = () => {
  const { id } = useParams();
  const { t } = useTranslation(['videos', 'common']);
  const { videoGroup, videos, setVideos } = useVideoGroupDetails(id ? parseInt(id) : undefined);

  const handleDeleteVideo = async (videoId: number) => {
    await deleteVideo(videoId);
    setVideos(videos.filter((video) => video.id !== videoId));
  };

  const videoColumns = [
    { field: "title", header: t('videos:table.title') },
    { field: "positionInQueue", header: t('videos:table.position') },
  ];

  if (!videoGroup) return null;

  return (
    <div className="container">
      <div className="content">
        <h1 className="heading mb-4">{videoGroup.name}</h1>

        <div className="d-flex justify-content-between mb-4">
          <NavigateButton
            path={`/videos/add?videogroupId=${id}`}
            actionType="Add"
            value={t('common:buttons.add')}
          />
          <NavigateButton path={`/projects/${videoGroup.projectId}`} actionType="Back" value={t('common:buttons.back')} />
        </div>

        {videos.length > 0 ? (
          <DataTable
            showRowNumbers={true}
            columns={videoColumns}
            data={videos}
            tableClassName="normal-table table-hover"
            navigateButton={(video) => (
              <NavigateButton
                path={`/videos/${video.id}`}
                actionType="Details"
                value={t('common:buttons.details')}
              />
            )}
            deleteButton={(video) => (
              <DeleteButton
                onClick={() => handleDeleteVideo(video.id)}
                itemType={video.title}
              />
            )}
          />
        ) : (
          <div className="card-body text-center py-5">
            <i className="fas fa-film fs-1 text-muted opacity-50"></i>
            <p className="text-muted mt-3 mb-0">
              {t('videos:video_group_details.no_videos')}
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default VideoGroupDetails;
