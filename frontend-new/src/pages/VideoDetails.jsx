import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Table, Alert } from '../components/ui';
import { LoadingSpinner, ErrorAlert, PageHeader, DetailSection } from '../components/common';
import { useVideoDetails } from '../hooks/useVideoDetails.js';

const VideoDetails = () => {
    const { t } = useTranslation(['videos', 'common']);
    const {
        video,
        videoLoading,
        videoError,
        assignedLabels,
        labelsLoading,
        labelsError,
        videoStreamUrl,
        videoStreamLoading,
        handleBackToVideoGroup,
        formatDuration,
    } = useVideoDetails();

    const [duration, setDuration] = useState(null);

    // Extract duration from video metadata
    useEffect(() => {
        if (!videoStreamUrl) return;

        const videoEl = document.createElement('video');
        videoEl.src = videoStreamUrl;
        videoEl.preload = 'metadata';

        videoEl.onloadedmetadata = () => {
            setDuration(parseFloat(videoEl.duration.toFixed(2)));
            URL.revokeObjectURL(videoEl.src);
        };
    }, [videoStreamUrl]);

    if (videoLoading) {
        return (
            <Container>
                <LoadingSpinner message={t('common:states.loading')} />
            </Container>
        );
    }

    if (videoError) {
        return (
            <Container>
                <ErrorAlert error={videoError} />
            </Container>
        );
    }

    if (!video) {
        return (
            <Container>
                <ErrorAlert error={t('videos:messages.not_found')} />
            </Container>
        );
    }

    return (
        <Container className="py-4">
            <PageHeader
                title={video.title}
                subtitle={
                    duration
                        ? `${t('videos:details.duration')}: ${formatDuration(duration)}`
                        : ''
                }
                icon="fas fa-video"
                actions={
                    <Button
                        variant="outline-secondary"
                        icon="fas fa-arrow-left"
                        onClick={handleBackToVideoGroup}
                    >
                        {t('common:buttons.back')}
                    </Button>
                }
            />

            <Card className="mb-4">
                <Card.Header>
                    <Card.Title level={5}>
                        <i className="fas fa-play-circle me-2"></i>
                        {t('videos:details.video_player')}
                    </Card.Title>
                </Card.Header>
                <Card.Body>
                    <div
                        className="video-container"
                        style={{
                            position: 'relative',
                            paddingTop: '56.25%',
                            backgroundColor: '#000',
                            borderRadius: '8px',
                            overflow: 'hidden',
                        }}
                    >
                        {videoStreamLoading ? (
                            <div className="position-absolute top-50 start-50 translate-middle text-white">
                                <LoadingSpinner message={t('videos:details.loading_video')} />
                            </div>
                        ) : videoStreamUrl ? (
                            <video
                                style={{
                                    position: 'absolute',
                                    top: 0,
                                    left: 0,
                                    width: '100%',
                                    height: '100%',
                                    objectFit: 'contain',
                                }}
                                controls
                                src={videoStreamUrl}
                                type="video/mp4"
                            >
                                {t('videos:details.video_not_supported')}
                            </video>
                        ) : (
                            <div className="position-absolute top-50 start-50 translate-middle text-white">
                                <i className="fas fa-exclamation-triangle me-2"></i>
                                {t('videos:details.failed_to_load')}
                            </div>
                        )}
                    </div>
                </Card.Body>
            </Card>

            <DetailSection
                title={t('videos:details.assigned_labels')}
                icon="fas fa-tags"
                showHeader={false}
            >
                <Card>
                    <Card.Body>
                        {labelsLoading ? (
                            <LoadingSpinner message={t('videos:details.loading')} size="small" />
                        ) : labelsError ? (
                            <ErrorAlert error={labelsError} />
                        ) : assignedLabels.length > 0 ? (
                            <Table striped hover responsive>
                                <Table.Head>
                                    <Table.Row>
                                        <Table.Cell header>#</Table.Cell>
                                        <Table.Cell header>{t('videos:table.label')}</Table.Cell>
                                        <Table.Cell header>{t('videos:table.labeler')}</Table.Cell>
                                        <Table.Cell header>{t('videos:table.subject')}</Table.Cell>
                                        <Table.Cell header>{t('videos:table.start')}</Table.Cell>
                                        <Table.Cell header>{t('videos:table.end')}</Table.Cell>
                                        <Table.Cell header>{t('videos:table.ins_date')}</Table.Cell>
                                    </Table.Row>
                                </Table.Head>
                                <Table.Body>
                                    {assignedLabels.map((label, index) => (
                                        <Table.Row key={label.id}>
                                            <Table.Cell>{index + 1}</Table.Cell>
                                            <Table.Cell>
                        <span className="badge bg-primary">
                          {label.labelName || t('common:states.unknown')}
                        </span>
                                            </Table.Cell>
                                            <Table.Cell>{label.labelerName || t('common:states.unknown')}</Table.Cell>
                                            <Table.Cell>{label.subjectName || t('common:states.unknown')}</Table.Cell>
                                            <Table.Cell>
                                                {label.start ? `${label.start} s` : t('common:states.not_available')}
                                            </Table.Cell>
                                            <Table.Cell>
                                                {label.end ? `${label.end} s` : t('common:states.not_available')}
                                            </Table.Cell>
                                            <Table.Cell>
                                                {label.insDate
                                                    ? new Date(label.insDate).toLocaleString()
                                                    : t('common:states.not_available')}
                                            </Table.Cell>
                                        </Table.Row>
                                    ))}
                                </Table.Body>
                            </Table>
                        ) : (
                            <Alert variant="info">
                                <i className="fas fa-info-circle me-2"></i>
                                {t('videos:details.no_labels')}
                            </Alert>
                        )}
                    </Card.Body>
                </Card>
            </DetailSection>
        </Container>
    );
};

export default VideoDetails;
