import React from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";

const QualitySelector = ({
  availableQualities,
  originalQuality,
  selectedQualityIndex,
  onQualityChange,
}) => {
  const { t } = useTranslation(["videos"]);

  // Get quality options based on available qualities
  const getQualityOptions = () => {
    if (
      !availableQualities ||
      availableQualities.length === 0 ||
      !originalQuality
    ) {
      return [];
    }

    const qualities = availableQualities;
    const original = originalQuality;

    const options = [
      {
        value: "original",
        label: t("videos:quality.original"),
        description: original,
      },
    ];

    // Find 2x lower quality
    const [origHeight] = original.split("x").map(Number);
    const twoXLower = qualities.find((q) => {
      const [height] = q.split("x").map(Number);
      return height === Math.floor(origHeight / 2);
    });

    if (twoXLower) {
      options.push({
        value: "2x",
        label: t("videos:quality.2x_lower"),
        description: twoXLower,
      });
    }

    // Find 4x lower quality
    const fourXLower = qualities.find((q) => {
      const [height] = q.split("x").map(Number);
      return height === Math.floor(origHeight / 4);
    });

    if (fourXLower) {
      options.push({
        value: "4x",
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
    // Pass the quality index directly ("original", "2x", or "4x")
    onQualityChange(value);
  };

  return (
    <div className="quality-selector mb-3">
      <label className="form-label mb-2">
        <i className="fas fa-film me-2"></i>
        {t("videos:quality.video_quality")}
      </label>
      <select
        className="form-select form-select-sm"
        value={selectedQualityIndex || "original"}
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
  availableQualities: PropTypes.arrayOf(PropTypes.string),
  originalQuality: PropTypes.string,
  selectedQualityIndex: PropTypes.string,
  onQualityChange: PropTypes.func.isRequired,
};

export default QualitySelector;
