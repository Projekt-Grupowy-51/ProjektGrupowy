import React, { useState, useEffect } from "react";
import NavigateButton from "../../components/NavigateButton";
import DeleteButton from "../../components/DeleteButton";
import DataTable from "../../components/DataTable";
import httpClient from "../../httpclient";
import { useNotification } from "../../context/NotificationContext";
import { useTranslation} from "react-i18next";

const ProjectVideosTab = ({ projectId }) => {
  const [videoGroups, setVideoGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const { addNotification } = useNotification();
  const { t } = useTranslation(['common', 'projects']);

  // Define columns for video groups table
  const videoGroupColumns = [
    { field: "name", header: t('common:name') },
    { field: "description", header: t('common:description') },
  ];

  const fetchVideoGroups = async () => {
    try {
      setLoading(true);
      const response = await httpClient.get(
        `/project/${projectId}/videogroups`
      );
      setVideoGroups(response.data);
    } catch (error) {
      //addNotification("Failed to load video groups", "error");
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteVideoGroup = async (videoGroupId) => {
    try {
      await httpClient.delete(`/videogroup/${videoGroupId}`);
      setVideoGroups(videoGroups.filter((group) => group.id !== videoGroupId));
    } catch (error) {
      // addNotification(
      //   error.response?.data?.message || "Failed to delete video group",
      //   "error"
      // );
    }
  };

  useEffect(() => {
    fetchVideoGroups();
  }, [projectId]);

  if (loading) {
    return (
      <div className="d-flex justify-content-center my-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">{t('projects:video_groups.loading')}</span>
        </div>
      </div>
    );
  }

  return (
    <div className="videos">
      <div className="d-flex justify-content-end mb-3">
        <NavigateButton
          actionType='Add'
          path={`/video-groups/add?projectId=${projectId}`}
          value={t('projects:add.video_group')}
        />
      </div>
      {videoGroups.length > 0 ? (
        <DataTable
          showRowNumbers={true}
          columns={videoGroupColumns}
          data={videoGroups}
          navigateButton={(video) => (
            <NavigateButton
              path={`/video-groups/${video.id}`}
              actionType='Details'
              value={t('common:buttons.details')}
            />
          )}
          deleteButton={(video) => (
            <DeleteButton
              onClick={() => handleDeleteVideoGroup(video.id)}
              itemType={t('projects:add.video_group')}
            />
          )}
        />
      ) : (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>{t('projects:not_found.video_group')}
        </div>
      )}
    </div>
  );
};

export default ProjectVideosTab;
