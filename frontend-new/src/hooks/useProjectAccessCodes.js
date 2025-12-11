import { useState, useEffect, useMemo } from "react";
import AccessCodeService from "../services/AccessCodeService.js";

export const useProjectAccessCodes = (projectIdRaw) => {
  const projectId = useMemo(() => parseInt(projectIdRaw, 10), [projectIdRaw]);
  const [visibleCodes, setVisibleCodes] = useState({});
  const [accessCodes, setAccessCodes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchAccessCodes = () => {
    if (!projectId) return Promise.resolve();

    setLoading(true);
    setError(null);
    return AccessCodeService.getByProjectId(projectId)
      .then((codes) => {
        const sortedCodes = codes.sort(
          (a, b) =>
            new Date(b.createdAtUtc || 0) - new Date(a.createdAtUtc || 0)
        );
        setAccessCodes(sortedCodes);
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchAccessCodes();
  }, [projectId]);

  const createAccessCode = async ({ expiration, customExpiration }) => {
    try {
      await AccessCodeService.createForProject({
        projectId,
        expiration,
        customExpiration,
      });
      await fetchAccessCodes();
    } catch (err) {
      setError(err.message);
      throw err;
    }
  };

  const retireCode = async (code) => {
    try {
      await AccessCodeService.retireCode(code);
      await fetchAccessCodes();
    } catch (err) {
      setError(err.message);
      throw err;
    }
  };

  const copyCode = async (code) => {
    try {
      await navigator.clipboard.writeText(code);
    } catch {
      const textArea = document.createElement("textarea");
      textArea.value = code;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand("copy");
      document.body.removeChild(textArea);
    }
  };

  const toggleVisibility = (code) => {
    setVisibleCodes((prev) => ({ ...prev, [code]: !prev[code] }));
  };

  return {
    accessCodes,
    visibleCodes,
    loading,
    error,
    createAccessCode,
    retireCode,
    copyCode,
    toggleVisibility,
    refetch: fetchAccessCodes,
  };
};
