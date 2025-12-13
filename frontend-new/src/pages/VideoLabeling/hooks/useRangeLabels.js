import { useState, useEffect, useCallback } from 'react';
import { LABEL_TYPES } from '../utils/labelUtils.js';

export const useRangeLabels = (labels, assignedLabelsState, currentTime, assignment) => {
  // State for range label creation (start time tracking per label)
  const [pendingRangeLabels, setPendingRangeLabels] = useState({});

  // Clear pending range labels when assignment changes
  useEffect(() => {
    setPendingRangeLabels({});
  }, [assignment?.id]);

  const handleLabelAction = useCallback(async (labelId) => {
    const label = labels?.find(l => l.id === labelId);
    if (!label) return;
    
    if (label.type === LABEL_TYPES.POINT) {
      await assignedLabelsState.handleSaveLabel({
        LabelId: labelId,
        Start: currentTime.toString(),
        End: currentTime.toString()
      });
    } else if (label.type === LABEL_TYPES.RANGE) {
      // Check if we already have a pending range label for this labelId
      if (pendingRangeLabels[labelId]) {
        // Second click - save the complete range label
        await assignedLabelsState.handleSaveLabel({
          LabelId: labelId,
          Start: pendingRangeLabels[labelId].startTime.toString(),
          End: currentTime.toString()
        });
        // Clear pending state for this label
        setPendingRangeLabels(prev => {
          const newState = { ...prev };
          delete newState[labelId];
          return newState;
        });
      } else {
        // First click - store start time for this label
        setPendingRangeLabels(prev => ({
          ...prev,
          [labelId]: {
            startTime: currentTime
          }
        }));
      }
    }
  }, [labels, assignedLabelsState, currentTime, pendingRangeLabels]);

  const getLabelState = useCallback((labelId) => {
    const label = labels?.find(l => l.id === labelId);
    if (!label) return { isPending: false, isRange: false };

    const isPending = !!pendingRangeLabels[labelId];
    const isRange = label.type === LABEL_TYPES.RANGE;

    return { isPending, isRange };
  }, [labels, pendingRangeLabels]);

  return {
    handleLabelAction,
    getLabelState,
    pendingRangeLabels
  };
};
