const ProgressBar = ({ currentTime, duration, onSeek }) => (
    <div className="progress-bar text-center">
        <input
            type="range"
            min="0"
            max={duration || 100}
            value={currentTime}
            step="0.01"
            onChange={(e) => onSeek(parseFloat(e.target.value))}
        />
    </div>
);

export default ProgressBar;