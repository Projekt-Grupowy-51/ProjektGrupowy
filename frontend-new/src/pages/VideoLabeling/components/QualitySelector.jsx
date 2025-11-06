import React from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";

const QualitySelector = ({ videos, selectedQuality, onQualityChange }) => {
  const { t } = useTranslation(["videos"]);

  // Get quality options based on available qualities from videos
  const getQualityOptions = () => {
    if (!videos || videos.length === 0) return [];

    // Get the first video's qualities as reference
    const firstVideo = videos[0];
    if (!firstVideo?.availableQualities || !firstVideo?.originalQuality) {
      return [];
    }

    const qualities = firstVideo.availableQualities;
    const original = firstVideo.originalQuality;

    const options = [
      {
        value: "original", // Use "original" as marker instead of actual quality
        label: t("videos:quality.original"),
        description: original,
        isOriginal: true,
      },
    ];

    // Find 2x lower quality
    const twoXLower = qualities.find((q) => {
      const [height] = q.split("x").map(Number);
      const [origHeight] = original.split("x").map(Number);
      return height === Math.floor(origHeight / 2);
    });

    if (twoXLower) {
      options.push({
        value: twoXLower,
        label: t("videos:quality.2x_lower"),
        description: twoXLower,
      });
    }

    // Find 4x lower quality
    const fourXLower = qualities.find((q) => {
      const [height] = q.split("x").map(Number);
      const [origHeight] = original.split("x").map(Number);
      return height === Math.floor(origHeight / 4);
    });

    if (fourXLower) {
      options.push({
        value: fourXLower,
        label: t("videos:quality.4x_lower"),
        description: fourXLower,
      });
    }

    return options;
  };

  const qualityOptions = getQualityOptions();

  if (qualityOptions.length === 0) {
    return null;
  }

  const handleQualityChange = (value) => {
    // Pass null if "original" is selected, otherwise pass the actual quality value
    onQualityChange(value === "original" ? null : value);
  };

  return (
    <div className="quality-selector mb-3">
      <label className="form-label mb-2">
        <i className="fas fa-film me-2"></i>
        {t("videos:quality.video_quality")}
      </label>
      <select
        className="form-select form-select-sm"
        value={selectedQuality || "original"}
        onChange={(e) => handleQualityChange(e.target.value)}
      >
        {qualityOptions.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label} ({option.description})
          </option>
        ))}
      </select>
    </div>
  );
};

QualitySelector.propTypes = {
  videos: PropTypes.arrayOf(
    PropTypes.shape({
      availableQualities: PropTypes.arrayOf(PropTypes.string),
      originalQuality: PropTypes.string,
    })
  ).isRequired,
  selectedQuality: PropTypes.string,
  onQualityChange: PropTypes.func.isRequired,
};

export default QualitySelector;
