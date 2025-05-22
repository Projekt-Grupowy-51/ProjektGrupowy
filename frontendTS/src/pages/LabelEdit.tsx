import React from "react";
import { useParams } from "react-router-dom";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from 'react-i18next';
import useLabelEdit from "../hooks/useLabelEdit";

const LabelEdit = () => {
  const { id } = useParams();
  const { t } = useTranslation(['labels', 'common']);
  const { formData: labelData, subjectName, handleChange, handleSubmit } = useLabelEdit(id ? parseInt(id) : undefined);

  return (
      <div className="container py-4">
        <div className="row justify-content-center">
          <div className="col-lg-8">
            <div className="card shadow-sm">
              <div className="card-header bg-primary text-white">
                <h1 className="heading mb-0">{t('labels:edit_title')}</h1>
              </div>
              <div className="card-body">
                <form onSubmit={handleSubmit}>
                  <div className="mb-3">
                    <label htmlFor="name" className="form-label">
                      {t('labels:form.name')}
                    </label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        value={labelData.name}
                        onChange={handleChange}
                        className="form-control"
                        required
                    />
                  </div>

                  <div className="mb-3">
                    <label htmlFor="colorHex" className="form-label">
                      {t('labels:form.color')}
                    </label>
                    <div className="input-group">
                      <input
                          type="color"
                          id="colorHex"
                          name="colorHex"
                          value={labelData.colorHex}
                          onChange={handleChange}
                          className="form-control form-control-color"
                          title={t('labels:form.color')}
                          style={{ maxWidth: "60px" }}
                          required
                      />
                      <input
                          type="text"
                          className="form-control form-control-color"
                          value={labelData.colorHex}
                          onChange={(e) =>
                              setLabelData({ ...labelData, colorHex: e.target.value })
                          }
                          pattern="#[0-9A-Fa-f]{6}"
                          placeholder="#RRGGBB"
                          required
                      />
                      <span
                          className="input-group-text"
                          style={{ backgroundColor: labelData.colorHex, width: "40px" }}
                      ></span>
                    </div>
                    <div className="form-text">
                      {t('labels:form.color_format')}
                    </div>
                  </div>

                  <div className="mb-3">
                    <label htmlFor="type" className="form-label">
                      {t('labels:form.type')}
                    </label>
                    <select
                        id="type"
                        name="type"
                        value={labelData.type}
                        onChange={handleChange}
                        className="form-select"
                        required
                    >
                      <option value="range">{t('labels:form.type_options.range')}</option>
                      <option value="point">{t('labels:form.type_options.point')}</option>
                    </select>
                  </div>

                  <div className="mb-3">
                    <label htmlFor="shortcut" className="form-label">
                      {t('labels:form.shortcut')}
                    </label>
                    <input
                        type="text"
                        id="shortcut"
                        name="shortcut"
                        value={labelData.shortcut}
                        onChange={handleChange}
                        className="form-control"
                        maxLength="1"
                        placeholder={t('labels:form.shortcut_hint')}
                        required
                    />
                    <div className="form-text">
                      {t('labels:form.shortcut_hint')}
                    </div>
                  </div>

                  <div className="mb-4">
                    <label htmlFor="subjectId" className="form-label">
                      {t('labels:form.subject')}
                    </label>
                    <div className="input-group">
                      <span className="input-group-text">
                        <i className="fas fa-folder"></i>
                      </span>
                      <input
                          type="text"
                          className="form-control"
                          value={subjectName || `${t('common:subject.id')}: ${labelData.subjectId}`}
                          disabled
                          required
                      />
                    </div>
                  </div>

                  <div className="d-flex">
                    <button
                        type="submit"
                        className="btn btn-primary me-2"
                    >
                      <i className="fas fa-save me-2"></i>
                      {t('labels:buttons.save')}
                    </button>
                    <NavigateButton path={`/videos/${videoId}`} actionType="Back" value={t('common:buttons.cancel')} />
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
  );
};

export default LabelEdit;
