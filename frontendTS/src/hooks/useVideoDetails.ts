import { useEffect, useState } from 'react';
import { Video } from '../models/Video';
import { AssignedLabel } from '../models/AssignedLabel';
import { getVideo, getVideoStream, getAssignedLabels } from '../services/api/videoService';

interface VideoDetailsState {
    video?: Video;
    stream?: string;
    labels: AssignedLabel[];
    loading: boolean;
}

export default function useVideoDetails(id?: number) {
    const [state, setState] = useState<VideoDetailsState>({ labels: [], loading: true });

    useEffect(() => {
        if (!id) return;
        const fetch = async () => {
            try {
                const [video, streamBlob, labels] = await Promise.all([
                    getVideo(id),
                    getVideoStream(id),
                    getAssignedLabels(id),
                ]);
                setState({
                    video,
                    stream: URL.createObjectURL(streamBlob),
                    labels,
                    loading: false,
                });
            } catch {
                setState(prev => ({ ...prev, loading: false }));
            }
        };
        fetch();
    }, [id]);

    return state;
}
