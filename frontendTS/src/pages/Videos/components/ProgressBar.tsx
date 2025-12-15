const ProgressBar = ({ currentTime, duration, onSeek }) => {
    const handleSeekChange = (e) => {
        const newTime = parseFloat(e.target.value);
        if (newTime !== currentTime) {
            onSeek(newTime); 
        }
    };

    return (
        <div className="progress-bar text-center">
            <input
                type="range"
                min="0"
                max={duration || 100} 
                value={currentTime}
                step="0.01"
                onChange={handleSeekChange}
            />
        </div>
    );
};

export default ProgressBar;
