using UnityEngine;
class GazeCallback_Proxy : AndroidJavaProxy
{

    public GazeCallback_Proxy() : base("camp.visual.gazetracker.callback.GazeCallback")
    {

    }

    void onGaze(AndroidJavaObject _gazeInfo)
    {
        long timestamp = _gazeInfo.Get<long>("timestamp");
        float x = _gazeInfo.Get<float>("x");
        float y = _gazeInfo.Get<float>("y");
        TrackingState trackingState = (TrackingState)_gazeInfo.Get<AndroidJavaObject>("trackingState").Call<int>("ordinal");
        EyeMovementState eyeMovementState = (EyeMovementState)_gazeInfo.Get<AndroidJavaObject>("eyeMovementState").Call<int>("ordinal");
        ScreenState screenState = (ScreenState)_gazeInfo.Get<AndroidJavaObject>("screenState").Call<int>("ordinal");

        GazeInfo gazeInfo = new GazeInfo(timestamp,
                                         x,
                                         y,
                                         trackingState,
                                         eyeMovementState,
                                         screenState);

        AndroidBridgeManager.SharedInstance().sendGazeInfo(gazeInfo);
    }

}