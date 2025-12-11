import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { LABEL_TYPES } from '../utils/labelUtils.js';

const LabelButtons = ({ labels, onLabelAction, getLabelState }) => {
  const { t } = useTranslation('videos');
  return (
    <div className="d-flex flex-wrap gap-1">
      {labels?.map((label) => {
        const { isPending, isRange } = getLabelState(label.id);
        
        const buttonText = `${label.name} [${label.shortcut}]`;
        
        let tooltipText = `${t('videos:labeling.shortcut')}: ${label.shortcut} | ${label.description}`;
        if (isPending) {
          tooltipText = `${t('videos:labeling.click_to_set_end_time')} | ${t('videos:labeling.shortcut')}: ${label.shortcut}`;
        } else if (isRange) {
          tooltipText = `${t('videos:labeling.click_to_set_start_time')} | ${t('videos:labeling.shortcut')}: ${label.shortcut}`;
        }
        
        return (
          <button
            key={label.id}
            className={`btn btn-sm ${isPending ? 'btn-outline-danger' : 'btn-outline-primary'}`}
            onClick={() => onLabelAction(label.id)}
            title={tooltipText}
            style={{
              borderColor: isPending ? '#ffc107' : label.colorHex,
              color: isPending ? '#000' : label.colorHex,
              backgroundColor: isPending ? '#ffc107' : 'transparent',
              fontSize: '0.75rem'
            }}
          >
            {buttonText}
          </button>
        );
      })}
    </div>
  );
};

LabelButtons.propTypes = {
  labels: PropTypes.arrayOf(PropTypes.shape({
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
    name: PropTypes.string.isRequired,
    shortcut: PropTypes.string,
    colorHex: PropTypes.string.isRequired,
    type: PropTypes.string.isRequired,
    description: PropTypes.string
  })),
  onLabelAction: PropTypes.func.isRequired,
  getLabelState: PropTypes.func.isRequired
};

export default LabelButtons;
