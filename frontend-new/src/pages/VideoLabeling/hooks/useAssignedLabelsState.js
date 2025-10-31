import { useState, useEffect, useCallback } from "react";
import VideoService from "../../../services/VideoService.js";
import AssignedLabelService from "../../../services/AssignedLabelService.js";
import config from "../../../config/config.js";

export const useAssignedLabelsState = (videos, assignment) => {
  const [assignedLabels, setAssignedLabels] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saveLoading, setSaveLoading] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(config.ui.defaultPageSize);
  const [totalPages, setTotalPages] = useState(0);
  const [totalItems, setTotalItems] = useState(0);

  const loadAssignedLabels = useCallback(
    async (page = currentPage, size = pageSize) => {
      if (!videos.length || !assignment?.subjectId) return;

      setLoading(true);

      try {
        // Fetch paginated labels for all videos
        const promises = videos.map((video) =>
          VideoService.getAssignedLabelsBySubjectPaginated(
            video.id,
            assignment.subjectId,
            page,
            size
          )
        );
        const responses = await Promise.all(promises);

        // Combine all assigned labels from all videos
        const allLabels = responses.flatMap(
          (response) => response.assignedLabels || []
        );

        // Use the total count from the first video (assuming all have similar counts)
        // Or sum them if labels are unique per video
        const totalCount = responses.reduce(
          (sum, response) => sum + (response.totalLabelCount || 0),
          0
        );

        setAssignedLabels(allLabels);
        setTotalItems(totalCount);
        setTotalPages(Math.ceil(totalCount / size));
        setCurrentPage(page);
      } catch (error) {
        console.error("Failed to load assigned labels:", error);
        setAssignedLabels([]);
      } finally {
        setLoading(false);
      }
    },
    [videos, assignment?.subjectId, currentPage, pageSize]
  );

  // Load labels when videos or subjectId change
  useEffect(() => {
    loadAssignedLabels();
  }, [loadAssignedLabels]);

  const handleSaveLabel = useCallback(
    async (labelData) => {
      setSaveLoading(true);
      try {
        // Save label for ALL videos, not just first one
        const savePromises = videos.map((video) => {
          const requestData = {
            ...labelData,
            videoId: video.id,
          };
          return AssignedLabelService.create(requestData);
        });

        await Promise.all(savePromises);

        // Reload current page of assigned labels after saving
        await loadAssignedLabels(currentPage, pageSize);
      } finally {
        setSaveLoading(false);
      }
    },
    [videos, assignment?.subjectId, currentPage, pageSize, loadAssignedLabels]
  );

  const handleDeleteLabel = useCallback(
    async (assignedLabelId) => {
      setDeleteLoading(true);
      try {
        await AssignedLabelService.delete(assignedLabelId);

        // Reload current page of assigned labels after deletion
        await loadAssignedLabels(currentPage, pageSize);
      } finally {
        setDeleteLoading(false);
      }
    },
    [currentPage, pageSize, loadAssignedLabels]
  );

  const handlePageChange = useCallback(
    (newPage) => {
      loadAssignedLabels(newPage, pageSize);
    },
    [pageSize, loadAssignedLabels]
  );

  const handlePageSizeChange = useCallback(
    (newSize) => {
      setPageSize(newSize);
      loadAssignedLabels(1, newSize);
    },
    [loadAssignedLabels]
  );

  return {
    assignedLabels,
    assignedLabelsLoading: loading,
    labelOperationLoading: saveLoading || deleteLoading,
    currentPage,
    pageSize,
    totalPages,
    totalItems,
    loadAssignedLabels,
    handleSaveLabel,
    handleDeleteLabel,
    handlePageChange,
    handlePageSizeChange,
  };
};
