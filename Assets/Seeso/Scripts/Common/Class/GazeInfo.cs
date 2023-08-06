public class GazeInfo
{
    public long timestamp;
    public float x;
    public float y;
    public TrackingState trackingState;
    public EyeMovementState eyeMovementState;
    public ScreenState screenState;


    public GazeInfo(long _timestamp, float _x, float _y, TrackingState _trackingState, EyeMovementState _eyeMovementState, ScreenState _screenState)
    {
        timestamp = _timestamp;
        x = _x;
        y = _y;
        trackingState = _trackingState;
        eyeMovementState = _eyeMovementState;
        screenState = _screenState;
    }
}