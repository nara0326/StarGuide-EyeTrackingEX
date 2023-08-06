public class CalibrationDelegate
{
    public delegate void onCalibrationNextPoint(float x, float y);
    public delegate void onCalibrationProgress(float progress);
    public delegate void onCalibrationFinished(double[] calibrationData);
}