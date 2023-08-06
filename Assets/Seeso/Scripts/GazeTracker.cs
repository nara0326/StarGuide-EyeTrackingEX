using UnityEngine;
public class GazeTracker
{

    //제공된 라이선스와 초기화 결과에 대한 콜백을 사용하여 응시 추적기를 초기화합니다. 콜백함수 
    public static void initGazeTracker(string license, InitializationDelegate.onInitialized callback)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.InitGazeTracker(license, callback);
#elif UNITY_IOS
        IOSBridgeManager.InitGazeTracker(license, callback);
#endif
    }

    //제공된 라이선스, 초기화 결과에 대한 콜백 및 사용자 상태 옵션을 사용하여 응시 추적기를 초기화합니다.
    public static void initGazeTracker(string license, InitializationDelegate.onInitialized callback, UserStatusOption option)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.InitGazeTracker(license, callback, option);
#elif UNITY_IOS
        IOSBridgeManager.InitGazeTracker(license, callback, option);
#endif
    }

    //응시 추적기를 초기화 해제하고 할당된 리소스를 해제합니다.
    public static void deinitGazeTracker()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.DeinitGazeTracker();
#elif UNITY_IOS
        IOSBridgeManager.DeinitGazeTracker();
#endif
    }

    public static bool isFoundDevice()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().IsDeviceFound();
#endif
        return false;
    }

    public static void addCameraPosition(CameraPosition cameraPosition)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().AddCameraPosition(cameraPosition);
#endif
    }

    public static CameraPosition getCameraPosition()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().GetCameraPosition();
#endif
        return null;
    }

    public static CameraPosition[] getCameraPositionList()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().GetCameraPositionList();
#endif
        return null;
    }

    public static void selectCameraPositions(int idx)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SelectCameraPosition(idx);
#endif
    }

    // 시선 추적 프로세스를 시작합니다.
    public static void startTracking()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().StartTracking();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().StartTracking();
#endif
    }

    //응시 추적 프로세스를 중지합니다.
    public static void stopTracking()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().StopTracking();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().StopTracking();
#endif 
    }

    public static void setAttentionInterval(int interval)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SetAttentionInterval(interval);
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().IsTracking();
#endif
    }

    public static float getAttentionScore()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().GetAttentionScore();
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().GetAttentionScore();
#endif
    }

    public static void setAttentionRegion(float left, float top, float right, float bottom)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SetAttentionRegion(left, top, right, bottom);
#elif UNITY_IOS
         IOSBridgeManager.SharedInstance().SetAttentionRegion(left, top, right, bottom);
#endif
    }

    public static float[] getAttentionRegion()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().GetAttentionRegion();
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().GetAttentionRegion();
#endif
        return null;
    }

    public static void removeAttentionRegion()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().RemoveAttentionRegion();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().RemoveAttentionRegion();
#endif
    }

    public static bool isTracking()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().IsTracking();
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().IsTracking();
#endif
    }
    // 시선 추적을 위해 원하는 프레임 속도(FPS)를 설정합니다.
    public static bool setTrackingFPS(int fps)
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().SetTrackingFPS(fps);
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().SetTrackingFPS(fps);
#endif 
    }

    public static bool startCalibration(CalibrationModeType mode, AccuracyCriteria criteria, float left, float top, float right, float bottom)
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().StartCalibration(mode, criteria, left, top, right, bottom);
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().StartCalibration(mode, criteria, left, top, right, bottom);
#endif 
    }

    public static bool startCalibration(CalibrationModeType mode, AccuracyCriteria criteria)
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().StartCalibration(mode, criteria);
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().StartCalibration(mode, criteria);
#endif 
    }
    //지정된 보정 영역에서 보정 프로세스를 시작합니다.
    public static bool startCalibration(float left, float top, float right, float bottom)
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().StartCalibration(left, top, right, bottom);
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().StartCalibration(left, top, right, bottom);
#endif 
    }

    //보정 영역을 지정하지 않고 보정 프로세스를 시작합니다
    public static bool startCalibration()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().StartCalibration();
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().StartCalibration();
#endif 
    }

    //보정 프로세스를 중지합니다.
    public static void stopCalibration()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().StopCalibration();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().StopCalibration();
#endif 
    }

    //보정을 위한 응시 데이터 수집을 시작합니다.
    public static bool startCollectSamples()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().StartCollectSamples();
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().StartCollectSamples();
#endif 
    }

    //보정 과정에서 얻은 보정 데이터를 설정합니다.
    public static bool setCalibrationData(double[] calibrationData)
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.SharedInstance().SetCalibrationData(calibrationData);
#elif UNITY_IOS
        return IOSBridgeManager.SharedInstance().SetCalibrationData(calibrationData);
#endif 
    }

    public static void setCameraPreview(float left, float top, float right, float bottom)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SetCameraPreview(left, top, right, bottom);
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().SetCameraPreview(left,top,right,bottom);
#endif
    }

    public static void removeCameraPreview()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().RemoveCameraPreview();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().RemoveCameraPreview();
#endif
    }

    //시작 및 중지 이벤트를 추적하기 위한 콜백을 설정합니다
    public static void setStatusCallback(StatusDelegate.onStarted onStarted, StatusDelegate.onStopped onStopped)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SetStatusCallback(onStarted, onStopped);
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().SetStatusCallback(onStarted, onStopped);
#endif
    }

    public static void removeStatusCallback()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().RemoveStatusCallback();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().RemoveStatusCallback();
#endif
    }

    //응시 데이터 업데이트에 대한 콜백을 설정합니다
    public static void setGazeCallback(GazeDelegate.onGaze onGaze)
    { 
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SetGazeCallback(onGaze);
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().SetGazeCallback(onGaze);
#endif
    }

    public static void removeGazeCallback()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().RemoveGazeCallback();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().RemoveGazeCallback();
#endif
    }

    public static void setCalibrationCallback(CalibrationDelegate.onCalibrationNextPoint onCalibrationNext, CalibrationDelegate.onCalibrationProgress onCalibrationProgress, CalibrationDelegate.onCalibrationFinished onCalibrationFinished)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SetCalibrationCallback(onCalibrationNext, onCalibrationProgress, onCalibrationFinished);
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().SetCalibrationCallback(onCalibrationNext, onCalibrationProgress, onCalibrationFinished);
#endif

    }

    public static void removeCalibrationCallback()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().RemoveCalibrationCallback();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().RemoveCalibrationCallback();
#endif
    }

    //보정 진행 상황 및 결과에 대한 콜백을 설정합니다.
    public static void setUserStatusCallback(UserStatusDelegate.onAttention onAttention, UserStatusDelegate.onBlink onBlink, UserStatusDelegate.onDrowsiness onDrowsiness)
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().SetUserStatusCallback(onAttention, onBlink, onDrowsiness);
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().SetUserStatusCallback(onAttention, onBlink, onDrowsiness);
#endif
    }

    public static void removeUserStatusCallback()
    {
#if UNITY_ANDROID
        AndroidBridgeManager.SharedInstance().RemoveUserStatusCallback();
#elif UNITY_IOS
        IOSBridgeManager.SharedInstance().RemoveUserStatusCallback();
#endif
    }

    public static string getVersionName()
    {
#if UNITY_ANDROID
        return AndroidBridgeManager.GetVersionName();
#elif UNITY_IOS
        return IOSBridgeManager.GetVersionName();
#endif
    }
}
