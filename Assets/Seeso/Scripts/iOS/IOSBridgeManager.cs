using AOT;
using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class IOSBridgeManager : SeeSoBridgeManager
{
    private static IOSBridgeManager manager;

    //callback
    private InitializationDelegate.onInitialized onInitialized;

    
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void iOSInitGazeTracker(string licence, IOSDelegate.initialized_gaze_tracker callback, bool isUseAttention, bool isUseBlink, bool isUseDrowsiness);

    [DllImport("__Internal")]
    private static extern void iOSDeinitGazeTracker();

    [DllImport("__Internal")]
    private static extern void iOSStartTracking();

    [DllImport("__Internal")]
    private static extern void iOSStopTracking();

    [DllImport("__Internal")]
    private static extern bool iOSIsTracking();

    [DllImport("__Internal")]
    private static extern bool iOSSetTrackingFPS(int fps);

    [DllImport("__Internal")]
    private static extern void iOSSetCameraPreview(float left, float top, float right, float bottom);

    [DllImport("__Internal")]
    private static extern void iOSRemoveCameraPreview();

    [DllImport("__Internal")]
    private static extern void iOSSetAttentionInterval(int interval);

    [DllImport("__Internal")]
    private static extern float iOSGetAttentionScore();

    [DllImport("__Internal")]
    private static extern void iOSSetAttentionRegion(float left, float top, float right, float bottom);

    [DllImport("__Internal")]
    private static extern IntPtr iOSGetAttentionRegion();

    [DllImport("__Internal")]
    private static extern void iOSRemoveAttentionRegion();

    [DllImport("__Internal")]
    private static extern bool iOSStartCalibration(int mode, int criteria, float left, float top, float right, float bottom);

    [DllImport("__Internal")]
    private static extern void iOSStopCalibration();

    [DllImport("__Internal")]
    private static extern bool iOSStartCollectSamples();

    [DllImport("__Internal")]
    private static extern bool iOSSetCalibrationData(double[] calibrationData, int length);

    [DllImport("__Internal")]
    private static extern void iOSSetDelegates(IOSDelegate.status_on_start startCallback, IOSDelegate.status_on_stop stopCallback, IOSDelegate.gaze_info gazeCallbck, IOSDelegate.calibration_on_next nextCallback, IOSDelegate.calibration_on_progress progressCallbck, IOSDelegate.calibration_on_finished finishedCallback, IOSDelegate.user_status_on_attention attentionCallback, IOSDelegate.user_status_on_blink blinkCallback, IOSDelegate.user_status_on_drowsiness drowsinessCallback);

    [DllImport("__Internal")]
    private static extern string iOSGetVersionName();

    private IOSBridgeManager(){}

    private static float FLOAT_NULL = float.NaN;

    public static new IOSBridgeManager SharedInstance()
    {
        if(manager == null)
        {
            manager = new IOSBridgeManager();
        }
        return manager;
    }

    public static new void InitGazeTracker(string license, InitializationDelegate.onInitialized onInitialized)
    {
        InitGazeTracker(license, onInitialized, new UserStatusOption());
    }

    public static new void InitGazeTracker(string license, InitializationDelegate.onInitialized onInitialized, UserStatusOption option)
    {
        SharedInstance().onInitialized = onInitialized;
        iOSInitGazeTracker(license, OnGazeTrackerInitailzed, option.isUseAttention(), option.isUseBlink(), option.isUseDrowsiness());
    }

    public static new void DeinitGazeTracker()
    {
        iOSDeinitGazeTracker();
    }


    public override void StartTracking()
    {
        iOSStartTracking();
    }

    public override void StopTracking()
    {
        iOSStopTracking();
    }

    public override bool IsTracking()
    {
        
        return iOSIsTracking();
    }

    public override bool SetTrackingFPS(int fps)
    {
        return iOSSetTrackingFPS(fps);
    }

    public override void SetCameraPreview(float left, float top, float right, float bottom)
    {
        iOSSetCameraPreview(left, top, right, bottom);
    }

    public override void RemoveCameraPreview()
    {
        iOSRemoveCameraPreview();
    }

    public override void SetAttentionInterval(int interval)
    {
        iOSSetAttentionInterval(interval);
    }

    public override float GetAttentionScore()
    {
        return iOSGetAttentionScore();
    }

    public override void SetAttentionRegion(float left, float top, float right, float bottom)
    {
        iOSSetAttentionRegion(left, top, right, bottom);
    }

    public override float[] GetAttentionRegion()
    { 
        IntPtr intptr = iOSGetAttentionRegion();
        if(intptr != null)
        {
            int length = 4;
            byte[] managedData = new byte[length * 8];

            Marshal.Copy(intptr, managedData, 0, length * 8);
            float[] array = new float[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToSingle(BitConverter.ToDouble(managedData, i * 8));
                
                if (float.IsInfinity(array[i]))
                {
                    return null;
                }
            }

            return array;
        }

        return null;
    }

    public override void RemoveAttentionRegion()
    {
        iOSRemoveAttentionRegion();
    }

    public override bool StartCalibration(CalibrationModeType mode, AccuracyCriteria criteria, float left, float top, float right, float bottom)
    {
        return iOSStartCalibration((int)mode, (int)criteria, left, top, right, bottom);
    }

    public override bool StartCalibration(CalibrationModeType mode, AccuracyCriteria criteria)
    {
        return iOSStartCalibration((int)mode, (int)criteria, FLOAT_NULL, FLOAT_NULL, FLOAT_NULL, FLOAT_NULL);
    }

    public override bool StartCalibration(float left, float top, float right, float bottom)
    {
        return iOSStartCalibration(0, 0, left, top, right, bottom);
    }

    public override bool StartCalibration()
    {
        return iOSStartCalibration(0, 0, FLOAT_NULL, FLOAT_NULL, FLOAT_NULL, FLOAT_NULL);
    }

    public override void StopCalibration()
    {
        iOSStopCalibration();
    }

    public override bool StartCollectSamples()
    {
        return iOSStartCollectSamples();
    }

    public override bool SetCalibrationData(double[] calibrationData)
    {
        return iOSSetCalibrationData(calibrationData, calibrationData.Length);
    }

    public new static string GetVersionName()
    {
        return iOSGetVersionName();
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.initialized_gaze_tracker))]
    private static void OnGazeTrackerInitailzed(int error)
    {
        if (error == 0)
        {
            iOSSetDelegates(
                OnStarted,
                OnStopped,
                OnGazeInfo,
                OnCalibrationNextPoint,
                OnCalibartionProgress,
                OnCalibrationFinished,
                OnAttention,
                OnBlink,
                OnDrowsiness);
        }
        SharedInstance().onInitialized((InitializationErrorType)error);     
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.status_on_start))]
    private static void OnStarted()
    {
        SharedInstance().onStarted();
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.status_on_stop))]
    private static void OnStopped(int error)
    {
        SharedInstance().onStopped((StatusErrorType)error);
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.gaze_info))]
    private static void OnGazeInfo(long timeStamp, float x, float y, int trackingState, int eyeMovementState, int screenState)
    {
        GazeInfo info = new GazeInfo(timeStamp, x, y, (TrackingState)trackingState, (EyeMovementState)eyeMovementState, (ScreenState)screenState);

        SharedInstance().onGaze(info);
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.calibration_on_next))]
    private static void OnCalibrationNextPoint(float x, float y)
    {
        SharedInstance().onNextPoint(x,y);
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.calibration_on_progress))]
    private static void OnCalibartionProgress(float progress)
    {
        SharedInstance().onProgress(progress);
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.calibration_on_finished))]
    private static void OnCalibrationFinished(IntPtr calibrationData,int length)
    {

        byte[] managedData = new byte[length * 8];

        Marshal.Copy(calibrationData, managedData, 0, length * 8);
        double[] array = new double[length];
        for (int i=0; i<array.Length; i++)
        {
            array[i] = BitConverter.ToDouble(managedData, i * 8);
        }
        Marshal.FreeHGlobal(calibrationData);
        SharedInstance().onFinished(array);
       
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.user_status_on_attention))]
    private static void OnAttention(long timestampBegin, long timestampEnd, float score)
    {
        if (SharedInstance().onAttention != null)
        {
            SharedInstance().onAttention(timestampBegin, timestampEnd, score);
        }
    }
    [MonoPInvokeCallback(typeof(IOSDelegate.user_status_on_blink))]
    private static void OnBlink(long timestamp, bool isBlinkLeft, bool isBlinkRight, bool isBlink, float eyeOpenness)
    {
        if (SharedInstance().onBlink != null)
        {
            SharedInstance().onBlink(timestamp, isBlinkLeft, isBlinkRight, isBlink, eyeOpenness);
        }
    }

    [MonoPInvokeCallback(typeof(IOSDelegate.user_status_on_drowsiness))]
    private static void OnDrowsiness(long timestamp, bool isDrowsiness)
    {
        if (SharedInstance().onDrowsiness != null) {
            SharedInstance().onDrowsiness(timestamp, isDrowsiness);
        }
    }
#endif
}
