import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import enCommon from './languages/en/common.json';
import plCommon from './languages/pl/common.json';
import enAuth from './languages/en/auth.json';
import plAuth from './languages/pl/auth.json';
import enLabels from './languages/en/labels.json';
import plLabels from './languages/pl/labels.json';
import enLabeler from './languages/en/labeler.json';
import plLabeler from './languages/pl/labeler.json';
import enProjects from './languages/en/projects.json';
import plProjects from './languages/pl/projects.json';
import enSubjects from './languages/en/subjects.json';
import plSubjects from './languages/pl/subjects.json';
import enAssignments from './languages/en/assignments.json';
import plAssignments from './languages/pl/assignments.json';
import enVideos from './languages/en/videos.json';
import plVideos from './languages/pl/videos.json';
import enVideoGroups from './languages/en/videoGroups.json';
import plVideoGroups from './languages/pl/videoGroups.json';

const resources = {
    en: {
        common: enCommon,
        auth: enAuth,
        labels: enLabels,
        labeler: enLabeler,
        projects: enProjects,
        subjects: enSubjects,
        assignments: enAssignments,
        videos: enVideos,
        videoGroups: enVideoGroups,
    },
    pl: {
        common: plCommon,
        auth: plAuth,
        labels: plLabels,
        labeler: plLabeler,
        projects: plProjects,
        subjects: plSubjects,
        assignments: plAssignments,
        videos: plVideos,
        videoGroups: plVideoGroups,
    }
};


i18n
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
        resources,
        fallbackLng: 'en',
        ns: ['common', 'auth', 'labels', 'labeler', 'projects', 'subjects', 'assignments', 'videoGroups', 'videos'],
        defaultNS: 'common',
        detection: {
            order: ['querystring', 'localStorage', 'navigator', 'htmlTag'],
            caches: ['localStorage'],
            lookupLocalStorage: 'i18nextLng',
        },
        interpolation: {
            escapeValue: false
        }
    });

export default i18n;