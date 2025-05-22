import { useEffect, useState, useCallback } from 'react';
import { getAssignment, getAssignmentLabelers, deleteAssignment } from '../services/api/assignmentService';
import { getSubject } from '../services/api/subjectService';
import { getVideoGroup } from '../services/api/videoGroupService';

export default function useAssignmentDetails(id?: number) {
    const [assignment, setAssignment] = useState<any>(null);
    const [subject, setSubject] = useState<any>(null);
    const [videoGroup, setVideoGroup] = useState<any>(null);
    const [labelers, setLabelers] = useState<any[]>([]);

    const fetchData = useCallback(async () => {
        if (!id) return;
        const assignmentData = await getAssignment(id);
        setAssignment(assignmentData);
        const [subjectData, videoGroupData, labelerData] = await Promise.all([
            getSubject(assignmentData.subjectId),
            getVideoGroup(assignmentData.videoGroupId),
            getAssignmentLabelers(id),
        ]);
        setSubject(subjectData);
        setVideoGroup(videoGroupData);
        setLabelers(labelerData);
    }, [id]);

    const handleDelete = async () => {
        if (!id) return;
        await deleteAssignment(id);
    };

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    return { assignment, subject, videoGroup, labelers, fetchData, handleDelete };
}
