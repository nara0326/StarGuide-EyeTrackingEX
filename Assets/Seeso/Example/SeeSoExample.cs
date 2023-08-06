using System.Collections;
using System;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using System.Runtime.InteropServices;
#endif


using UnityEngine.UI;
public class SeeSoExample : MonoBehaviour
{
    // GazePointer을 보이기 위한 벡터 캔버스 
    public GameObject OverlayCanvas;

    // Status
    bool isInitialized;
    bool isTracking;
    bool isCalibrating;
    bool isShowPreview;
    UserStatusOption Option;

    // Buttons
    public GameObject Initialize;
    public GameObject Deinitialize;
    public GameObject StartTracking;
    public GameObject StopTracking;
    public GameObject StartCalibration;
    public GameObject StopCalibration;
    public GameObject SetCameraPreview;
    public GameObject RemoveCameraPreview;


    bool useFilteredGaze = true;
    GazeFilter gazeFilter = new GazeFilter();

    public void setUseFilteredGaze(bool b)
    {
        useFilteredGaze = b;
    }


    // Informations
    public GameObject GazePoint;

    public GameObject TrackingState;
    TrackingState trackingState;

    public GameObject EyeMovementState;
    EyeMovementState eyeMovementState;

    // User Status
    public GameObject AttentionScore;
    public GameObject BlinkState;
    public GameObject DrowsinessState;
    public GameObject ToggleAttention;
    public GameObject ToggleBlink;
    public GameObject ToggleDrowsiness;
    string userStatusAttention;
    string userStatusBlink;
    string userStatusDrowsiness;

    const string LisenseKey = "prod_p2w9yepluo1hojftnsh85wvkpczxm6gq6snsjhdc";

    // FPS
    public GameObject TrackingFPS;
    public GameObject GameFPS;


#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern bool hasIOSCameraPermission();
    [DllImport("__Internal")]
    private static extern void requestIOSCameraPermission();
#endif

    // Tracking FPS
    int targetFPS;

    public void setCameraFpsRateFromSlider(float fps)
    {
        targetFPS = (int) fps;
    }

    public void lazySetCameraFpsRateFromSlider()
    {
        GazeTracker.setTrackingFPS(targetFPS);
    }

    bool isNewGaze;
    float gazeX;
    float gazeY;

    float systemWidth;
    float systemHeight;

    // For Orientation
    ScreenOrientation orientation;

    // For Calibration
    public GameObject CalibrationPoint;
    bool isNextStepReady;
    bool isCalibrationFinished;
    float calibrationProgress;
    float calibrationX;
    float calibrationY;

    // For Camera Preview
    public GameObject PreviewHolder;
    float previewLeft, previewTop, previewRight, previewBottom; // native coordinates
    
    // Game FPS
    int trackingFPS;
    int trackingFPSCount;

    float timer, refresh, avgFramerate;

    void Start()
    {
        Debug.Log("SeeSo Core Version : " + GazeTracker.getVersionName());
        orientation = Screen.orientation;

        // portrait based
        systemWidth = Mathf.Min(Display.main.systemWidth, Display.main.systemHeight);
        systemHeight = Mathf.Max(Display.main.systemWidth, Display.main.systemHeight);

        // Request Camera Permission

        if (!HasCameraPermission())
        {
            RequestCameraPermission();
        }
        StartCoroutine(_checkTrackingFps());
    }

    bool HasCameraPermission()
    {
#if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
        return hasIOSCameraPermission();
#endif
    }

    void RequestCameraPermission()
    {
#if UNITY_ANDROID
        Permission.RequestUserPermission(Permission.Camera);
#elif UNITY_IOS
        requestIOSCameraPermission();
#endif
    }

    void Update()
    {
        // Orientation Check

        ScreenOrientation curOrientation = Screen.orientation;
        if (curOrientation != orientation)
        {
            if (isShowPreview)
            {
                removeCameraPreview();
            }
        }
        orientation = curOrientation;

        // Game FPS Check
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        GameFPS.GetComponent<Text>().text = "Game FPS : " + avgFramerate.ToString();


        // Tracking FPS Check
        TrackingFPS.GetComponent<Text>().text = "Tracking FPS : " + trackingFPS;


        if (isTracking)
        {
            if (isNewGaze)
            {
                Vector2 overlayCanvasSizeDelta = OverlayCanvas.GetComponent<RectTransform>().sizeDelta;
                GazePoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(gazeX * overlayCanvasSizeDelta.x, gazeY * overlayCanvasSizeDelta.y);
                isNewGaze = false;
                TrackingState.GetComponent<Text>().text = "Gaze : " + trackingState.ToString();
                EyeMovementState.GetComponent<Text>().text = "Eye Movement : " + eyeMovementState.ToString();
                AttentionScore.GetComponent<Text>().text = "AttentionScore : " + userStatusAttention;
                BlinkState.GetComponent<Text>().text = "BlinkState : " + userStatusBlink;
                DrowsinessState.GetComponent<Text>().text = "DrowsinessState : " + userStatusDrowsiness;
            }

            if (isCalibrating)
            {
                GazePoint.SetActive(false);
            }
            else
            {
                GazePoint.SetActive(true);
            }
        }
        else
        {
            TrackingState.GetComponent<Text>().text = "Gaze : NONE";
            EyeMovementState.GetComponent<Text>().text = "Eye Movement : NONE";
            AttentionScore.GetComponent<Text>().text = "AttentionScore : NONE";
            BlinkState.GetComponent<Text>().text = "BlinkState : NONE";
            DrowsinessState.GetComponent<Text>().text = "DrowsinessState : NONE";
            GazePoint.SetActive(false);
        }


        // Calibrate Progress
        if (isCalibrating)
        {
            CalibrationPoint.SetActive(true);

            if (isNextStepReady)
            {
                Vector2 overlayCanvasSizeDelta = OverlayCanvas.GetComponent<RectTransform>().sizeDelta;
                CalibrationPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(calibrationX * overlayCanvasSizeDelta.x, calibrationY * overlayCanvasSizeDelta.y);
                isNextStepReady = false;
                GazeTracker.startCollectSamples();
            }

            if (isCalibrationFinished)
            {
                isCalibrationFinished = false;
                isNextStepReady = false;
                isCalibrating = false;
            }

            if (CalibrationPoint.activeSelf)
            {
                CalibrationPoint.GetComponent<RawImage>().color = new Color(calibrationProgress, 0f, 0f);
            }

        }
        else
        {
            CalibrationPoint.SetActive(false);
        }

        // Button Visibility
        if (isInitialized)
        {
            Initialize.SetActive(false);
            Deinitialize.SetActive(true);
        }
        else
        {
            Initialize.SetActive(true);
            Deinitialize.SetActive(false);
        }

        if (isTracking)
        {
            StartTracking.SetActive(false);
            StopTracking.SetActive(true);
        }
        else
        {
            StartTracking.SetActive(true);
            StopTracking.SetActive(false);
        }

        if (isCalibrating)
        {
            StartCalibration.SetActive(false);
            StopCalibration.SetActive(true);
        }
        else
        {
            StartCalibration.SetActive(true);
            StopCalibration.SetActive(false);
        }

        if (isShowPreview)
        {
            SetCameraPreview.SetActive(false);
            RemoveCameraPreview.SetActive(true);
        }
        else
        {
            SetCameraPreview.SetActive(true);
            RemoveCameraPreview.SetActive(false);
        }
    }

    public void initialize()
    {
        if (isInitialized) return;

        ToggleAttention.GetComponent<Toggle>().enabled = false;
        ToggleBlink.GetComponent<Toggle>().enabled = false;
        ToggleDrowsiness.GetComponent<Toggle>().enabled = false;
        ToggleAttention.GetComponentInChildren<Text>().color = Color.gray;
        ToggleBlink.GetComponentInChildren<Text>().color = Color.gray;
        ToggleDrowsiness.GetComponentInChildren<Text>().color = Color.gray;

        Option = new UserStatusOption();

        if (ToggleAttention.GetComponent<Toggle>().isOn)
        {
            Option.useAttention();
        }

        if (ToggleBlink.GetComponent<Toggle>().isOn)
        {
            Option.useBlink();
        }

        if (ToggleDrowsiness.GetComponent<Toggle>().isOn)
        {
            Option.useDrowsiness();
        }

        userStatusAttention = "NONE";
        userStatusBlink = "NONE";
        userStatusDrowsiness = "NONE";
        if (UsingUserStatus()){
            GazeTracker.initGazeTracker(LisenseKey, onInitialized, Option);
        }
        else
        {
            GazeTracker.initGazeTracker(LisenseKey, onInitialized);
        }
    }

    public void deinitialize()
    {
        if (isShowPreview)
        {
            removeCameraPreview();
        }

        GazeTracker.deinitGazeTracker();
        isInitialized = false;
        isTracking = false;
        isCalibrating = false;

        ToggleAttention.GetComponent<Toggle>().enabled = true;
        ToggleBlink.GetComponent<Toggle>().enabled = true;
        ToggleDrowsiness.GetComponent<Toggle>().enabled = true;
        ToggleAttention.GetComponentInChildren<Text>().color = Color.black;
        ToggleBlink.GetComponentInChildren<Text>().color = Color.black;
        ToggleDrowsiness.GetComponentInChildren<Text>().color = Color.black;
    }

    public void startTracking()
    { 
        if (isTracking || !isInitialized) return;
        GazeTracker.setStatusCallback(onStarted, onStopped);
        GazeTracker.setGazeCallback(onGaze);

        if (UsingUserStatus())
        {
            GazeTracker.setUserStatusCallback(onAttention, onBlink, onDrowsiness);
        }
        GazeTracker.startTracking();
    }

    public void stopTracking()
    {
        if (!isInitialized || !isTracking) return;
        GazeTracker.stopTracking();
        isTracking = false;
    }

    public void startCalibration()
    {
        if (!isTracking) return;
        if (isCalibrating) return;

        GazeTracker.setCalibrationCallback(onCalibrationNextPoint, onCalibrationProgress, onCalibrationFinished);
        bool success = GazeTracker.startCalibration();

        if (!success)
        {
            Debug.Log("startCalibration() fail, please check camera permission OR call startTracking() First");
        }
        else
        {
            isCalibrating = true;
        }
    }

    public void stopCalibration()
    {
        if (!isInitialized || !isCalibrating) return;

        GazeTracker.stopCalibration();

        isCalibrating = false;
        isNextStepReady = false;
        isCalibrationFinished = false;
    }

    public void setCameraPreview()
    {
        if (!isTracking) return;
        if (isShowPreview) return;

        setPreviewSize();
        GazeTracker.setCameraPreview(previewLeft, previewTop, previewRight, previewBottom);
        isShowPreview = true;
    }

    public void removeCameraPreview()
    {
        if (!isInitialized) return;

        GazeTracker.removeCameraPreview();
        isShowPreview = false;
    }

    public void setTrackingFps(int fps)
    {
        GazeTracker.setTrackingFPS(fps);
    }

    public void setCalibrationData(double[] calibrationData)
    {
        bool result = GazeTracker.setCalibrationData(calibrationData);
        Debug.Log("setCalibrationData : " + result);
    }

    void onStarted()
    {
        isTracking = true;
    }

    void onStopped(StatusErrorType error)
    {
        isTracking = false;
    }

    bool UsingUserStatus()
    {
        return Option.isUseAttention() || Option.isUseBlink() || Option.isUseDrowsiness();
    }

    void onGaze(GazeInfo gazeInfo)
    {
        Debug.Log("onGaze " + gazeInfo.timestamp + "," + gazeInfo.x + "," + gazeInfo.y + "," + gazeInfo.trackingState + "," + gazeInfo.eyeMovementState + "," + gazeInfo.screenState);

        isNewGaze = true;

        trackingFPSCount++;
        trackingState = gazeInfo.trackingState;


        if (!useFilteredGaze)
        {
            gazeX = _convertCoordinateX(gazeInfo.x);
            gazeY = _convertCoordinateY(gazeInfo.y);

        }
        else
        {
            gazeFilter.filterValues(gazeInfo.timestamp, _convertCoordinateX(gazeInfo.x), _convertCoordinateY(gazeInfo.y));
            gazeX = gazeFilter.getFilteredX();
            gazeY = gazeFilter.getFilteredY();
        }
    }
    

    void onCalibrationNextPoint(float x, float y)
    {
        Debug.Log("onCalibrationNextPoint" + x + "," + y);

        calibrationX = _convertCoordinateX(x);
        calibrationY = _convertCoordinateY(y);
        isNextStepReady = true;
    }

    void onCalibrationProgress(float progress)
    {
        Debug.Log("onCalibrationProgress" + progress);
        calibrationProgress = progress;
    }
        
    void onCalibrationFinished(double[] calibrationData)
    {
        Debug.Log("OnCalibrationFinished" + calibrationData.Length);
        isCalibrationFinished = true;
    }

    void onAttention(long timestampBegin, long timestamEnd, float score)
    {
        userStatusAttention = "" + score;
        Debug.Log("onAttention " + score);
    }

    void onBlink(long timestamp, bool isBlinkLeft, bool isBlinkRight, bool isBlink, float eyeOpenness)
    {
        userStatusBlink = "blink : " + isBlink;
        Debug.Log("onBlink " + isBlinkLeft + ", " + isBlinkRight + ", " + isBlink);
    }

    void onDrowsiness(long timestamp, bool isDrowsiness)
    {
        userStatusDrowsiness = "" + isDrowsiness;
        Debug.Log("onDrowsiness " + isDrowsiness);
    }

    public void onInitialized(InitializationErrorType error)
    {
        Debug.Log("onInitialized result : " + error);
        if(error == InitializationErrorType.ERROR_NONE)
        {
            isInitialized = true;
        }
        else
        {
            isInitialized = false;
        }
    }

    void setPreviewSize()
    {
        Vector2 overlayCanvasSizeDelta = OverlayCanvas.GetComponent<RectTransform>().sizeDelta;
        Vector3 position = PreviewHolder.GetComponent<RectTransform>().position;
        Rect rect = PreviewHolder.GetComponent<RectTransform>().rect;

        float width = systemWidth;
        float height = systemHeight;
        float deltaX = overlayCanvasSizeDelta.x;
        float deltaY = overlayCanvasSizeDelta.y;
        if (ScreenOrientation.Portrait != Screen.orientation && ScreenOrientation.PortraitUpsideDown != Screen.orientation)
        {
            width = systemHeight;
            height = systemWidth;
        }
        float scaleX = width / deltaX;
        float scaleY = height / deltaY;

        previewLeft = position.x;
        previewTop = height - position.y;
        previewRight = previewLeft + (rect.width * scaleX);
        previewBottom = previewTop + (rect.height * scaleY);
    }

    float _convertCoordinateX(float x)
    {
        float screenWidth = systemWidth;
        if (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
        {
            screenWidth = systemHeight;
        }

        return gazeX = x / screenWidth - 0.5f;
    }

    float _convertCoordinateY(float y)
    {
        float screenHeight = systemHeight;
        if (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
        {
            screenHeight = systemWidth;
        }
        return gazeY = 0.5f - y / screenHeight;
    }

    IEnumerator _checkTrackingFps()
    {
        while (true)
        {
            trackingFPS = trackingFPSCount;
            trackingFPSCount = 0;
            yield return new WaitForSeconds(1f);
        }
    }    
}
    
