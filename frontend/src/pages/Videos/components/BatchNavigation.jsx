import { formatTime } from "../utils";

const BatchNavigation = ({
    currentBatch,
    totalBatches,
    playerState,
    controls,
    batchState,
}) => (
    <div className="row">
        <div className="col-12">
            <div className="pagination d-flex align-items-center justify-content-between">
                <button
                    className="btn btn-primary pagination-button"
                    onClick={() => {
                        batchState.handleBatchChange(currentBatch - 1);
                        controls.resetPlaybackSpeed(); 
                        controls.handleSeek(0); 
                        if (playerState.isPlaying) controls.handlePlayStop(); 
                        setTimeout(() => {
                            controls.setPlayerState((prev) => {
                                const newState = {
                                    ...prev,
                                    currentTime: 0, 
                                    duration: 0, 
                                    playbackSpeed: 1, 
                                };
                                return newState;
                            });
                        }, 0);
                    }}
                    disabled={currentBatch <= 1}
                >
                    Previous
                </button>

                <div className="time-display text-center">
                    {formatTime(playerState.currentTime)} /{" "}
                    {formatTime(playerState.duration || 0)}
                </div>

                <div className="controls d-inline">
                    <button
                        className="btn btn-primary seek-btn mx-1"
                        onClick={() => controls.handleSeek(playerState.currentTime - 5)}
                    >
                        -5s
                    </button>
                    <button
                        className="btn btn-primary seek-btn mx-1"
                        onClick={() => controls.handleSeek(playerState.currentTime - 1)}
                    >
                        -1s
                    </button>
                    <button
                        className="btn btn-primary play-stop-btn mx-1"
                        onClick={controls.handlePlayStop}
                    >
                        {playerState.isPlaying ? "⏹" : "▶"} 
                    </button>
                    <button
                        className="btn btn-primary seek-btn mx-1"
                        onClick={() => controls.handleSeek(playerState.currentTime + 1)}
                    >
                        +1s
                    </button>
                    <button
                        className="btn btn-primary seek-btn mx-1"
                        onClick={() => controls.handleSeek(playerState.currentTime + 5)}
                    >
                        +5s
                    </button>
                </div>

                <select
                    className="form-select w-auto mx-3"
                    value={playerState.playbackSpeed}
                    onChange={(e) => controls.handleSpeedChange(parseFloat(e.target.value))}
                >
                    {[0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 1.75, 2.0].map((speed) => (
                        <option key={speed} value={speed}>
                            {speed}x
                        </option>
                    ))}
                </select>

                <button
                    className="btn btn-primary pagination-button"
                    onClick={() => {
                        batchState.handleBatchChange(currentBatch + 1);
                        controls.resetPlaybackSpeed(); 
                        controls.handleSeek(0); 
                        if (playerState.isPlaying) controls.handlePlayStop(); 
                        setTimeout(() => {
                            controls.setPlayerState((prev) => {
                                const newState = {
                                    ...prev,
                                    currentTime: 0, 
                                    duration: 0, 
                                    playbackSpeed: 1, 
                                };
                                return newState;
                            });
                        }, 0);
                    }}
                    disabled={currentBatch >= totalBatches}
                >
                    Next
                </button>
            </div>
        </div>
    </div>
);

export default BatchNavigation;