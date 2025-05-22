import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from 'react-i18next';
import { getProjectSubjects, getProjectVideoGroups } from "../services/api/projectService";
import { createAssignment } from "../services/api/assignmentService";

const SubjectVideoGroupAssignmentAdd = () => {
  const { t } = useTranslation(['assignments', 'common']);
  const [formData, setFormData] = useState({
    subjectId: "",
    videoGroupId: "",
  });
  const [subjects, setSubjects] = useState([]);
  const [videoGroups, setVideoGroups] = useState([]);
  const [projectId, setProjectId] = useState(null);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const projectIdParam = queryParams.get("projectId");
    if (projectIdParam) {
      setProjectId(parseInt(projectIdParam));
      fetchSubjectsAndVideoGroups(parseInt(projectIdParam));
    }
  }, [location.search]);

  const fetchSubjectsAndVideoGroups = async (projectId: number) => {
    const [subjectsData, videoGroupsData] = await Promise.all([
      getProjectSubjects(projectId),
      getProjectVideoGroups(projectId),
    ]);

    setSubjects(subjectsData);
    setVideoGroups(videoGroupsData);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.subjectId || !formData.videoGroupId) {
      return;
    }

    await createAssignment({
      subjectId: parseInt(formData.subjectId),
      videoGroupId: parseInt(formData.videoGroupId),
    });
    navigate(`/projects/${projectId}`);
  };

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-lg-8">
          <div className="card shadow-sm">
            <div className="card-header bg-primary text-white">
              <h1 className="heading mb-0">{t('assignments:add_title')}</h1>
            </div>
            <div className="card-body">
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="subjectId" className="form-label">
                    {t('assignments:form.subject')}
                  </label>
                  <select
                    id="subjectId"
                    name="subjectId"
                    value={formData.subjectId}
                    onChange={handleChange}
                    className="form-select"
                    required
                  >
                    <option value="">{t('assignments:form.select_subject')}</option>
                    {subjects.map((subject) => (
                      <option key={subject.id} value={subject.id}>
                        {subject.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="mb-4">
                  <label htmlFor="videoGroupId" className="form-label">
                    {t('assignments:form.video_group')}
                  </label>
                  <select
                    id="videoGroupId"
                    name="videoGroupId"
                    value={formData.videoGroupId}
                    onChange={handleChange}
                    className="form-select"
                    required
                  >
                    <option value="">{t('assignments:form.select_video_group')}</option>
                    {videoGroups.map((videoGroup) => (
                      <option key={videoGroup.id} value={videoGroup.id}>
                        {videoGroup.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="d-flex">
                  <button
                    type="submit"
                    className="btn btn-primary me-2"
                  >
                    <i className="fas fa-plus-circle me-2"></i>
                    {t('assignments:buttons.create')}
                  </button>
                  <NavigateButton path={`/projects/${projectId}`} actionType="Back" value={t('common:buttons.cancel')} />
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SubjectVideoGroupAssignmentAdd;
