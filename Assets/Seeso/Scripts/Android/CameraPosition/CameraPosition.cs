public class CameraPosition
{
    public string modelName;
    public float screenOriginX;
    public float screenOriginY;
    public bool cameraOnLongerAxis;

    public CameraPosition(string modelName, float screenOriginX, float screenOriginY, bool cameraOnLongerAxis)
    {
        this.modelName = modelName;
        this.screenOriginX = screenOriginX;
        this.screenOriginY = screenOriginY;
        this.cameraOnLongerAxis = cameraOnLongerAxis;
    }
}
