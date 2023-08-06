using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
using static InitializationDelegate;
using static StatusDelegate;
using static UserStatusDelegate;
#elif UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class EyeTracking : MonoBehaviour
{
    // GazePointer을 보이기 위한 벡터 캔버스 
    public GameObject OverlayCanvas;

    // Status
    bool isInitialized;
    bool isTracking;
    UserStatusOption Option;

    // 실행 버튼 
    public GameObject StartTracking;

    // 시선좌표 필터링 관련 
    bool useFilteredGaze = true;
    GazeFilter gazeFilter = new GazeFilter();

    // 응시 지점
    public GameObject GazePoint;

    // 트래킹 상태 SUCCESS / FAILED
    public GameObject TrackingState;
    TrackingState trackingState;

    // User Status
    public GameObject BlinkState;
    string userStatusBlink;
    string userStatusAttention;
    string userStatusDrowsiness;

    //라이센스 키 
    const string LisenseKey = "prod_p2w9yepluo1hojftnsh85wvkpczxm6gq6snsjhdc";


#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern bool hasIOSCameraPermission();
    [DllImport("__Internal")]
    private static extern void requestIOSCameraPermission();
#endif

    // 응시 좌표 
    bool isNewGaze;
    float gazeX;
    float gazeY;

    //화면 비율 
    float systemWidth;
    float systemHeight;

    // 스크린 오리지널 방향 
    ScreenOrientation orientation;

    void Start()
    {
        Debug.Log("SeeSo Core Version : " + GazeTracker.getVersionName());

        // 화면 길이 기준 
        systemWidth = Mathf.Min(Display.main.systemWidth, Display.main.systemHeight);
        systemHeight = Mathf.Max(Display.main.systemWidth, Display.main.systemHeight);

        // 카메라 권한 요청 

        if (!HasCameraPermission())
        {
            RequestCameraPermission();
        }
        //init
        Initialized();
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
        orientation = curOrientation;

       if (isTracking)
        {
            if (isNewGaze)
            {
                Vector2 overlayCanvasSizeDelta = OverlayCanvas.GetComponent<RectTransform>().sizeDelta;
                GazePoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(gazeX * overlayCanvasSizeDelta.x, gazeY * overlayCanvasSizeDelta.y);
                isNewGaze = false;
            }
        }
       else
        {
            GazePoint.SetActive(true);
        }

        // Button Visibility
        if (isTracking)
        {
            StartTracking.SetActive(false);
        }
        else
        {
            StartTracking.SetActive(true);
        }


    }

    //init 함수 
    public void Initialized()
    {
        if (isInitialized) return;

        Option = new UserStatusOption();
        //깜빡임 확인 항상 활성화 
        Option.useBlink();

        // user 상태 확인 후 init
        if (UsingUserStatus())
        {
            GazeTracker.initGazeTracker(LisenseKey, onInitialized, Option);
        }
        else
        {
            GazeTracker.initGazeTracker(LisenseKey, onInitialized);
        }

    }
    // StartTacking 
    public void startTracking()
    {
        if (isTracking || !isInitialized) return;
        // 시선 추적이 시작되고 중지될때 각각 호출되는 콜백함수 설정 
        GazeTracker.setStatusCallback(onStarted, onStopped);
        //ongaze 콜백 
        GazeTracker.setGazeCallback(onGaze);

        //
        if (UsingUserStatus())
        {
            GazeTracker.setUserStatusCallback(onAttention, onBlink, onDrowsiness);
        }
        //트래킹 시작
        GazeTracker.startTracking();
    }



    // init의 결과 확인 콜백함수 
    public void onInitialized(InitializationErrorType error)
    {
        Debug.Log("onInitialized result : " + error);
        if (error == InitializationErrorType.ERROR_NONE)
        {
            isInitialized = true;
        }
        else
        {
            isInitialized = false;
        }
    }

    // 깜빡임 상태 반환 
    bool UsingUserStatus()
    {
        return  Option.isUseBlink();
    }

    //시선 좌표 저장 
    void onGaze(GazeInfo gazeInfo)
    {
        Debug.Log("onGaze " + gazeInfo.timestamp + "," + gazeInfo.x + "," + gazeInfo.y + "," + gazeInfo.trackingState + "," + gazeInfo.screenState);

        isNewGaze = true;

        // 조건에 따라 응시 좌표를 필터링 하는 작업 false경우 실행. 우리 어플에선 여기만 쓸 듯 
        if (!useFilteredGaze)
        {   
            //좌표계로 변환 
            gazeX = _convertCoordinateX(gazeInfo.x);
            gazeY = _convertCoordinateY(gazeInfo.y);

        }
        else
        {
            // gazeFilter를 사용하여 응시좌표 필터링 
            // 타임스탬프로 변환된 값들을 필터링
            gazeFilter.filterValues(gazeInfo.timestamp, _convertCoordinateX(gazeInfo.x), _convertCoordinateY(gazeInfo.y));
            gazeX = gazeFilter.getFilteredX();
            gazeY = gazeFilter.getFilteredY();
        }
    }

    // 화면 너비와 방향에 따라 좌표값을 -0.5~0.5 사이의 정규화된 좌표로 변환 
    float _convertCoordinateX(float x)
    {
        float screenWidth = systemWidth;

        // orientation 변수를 사용해서 현재 화면 방향을 결정하고 그에 따라 screenWidth 조절 
        // 화면이 가로방향인지 세로 방향인지 확인 후 true면 가로방향으로 인해 화면 너비를 높이로 바꾼다. 
        if (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
        {
            screenWidth = systemHeight;
        }

        return gazeX = x / screenWidth - 0.5f;
    }

    float _convertCoordinateY(float y)
    {
        float screenHeight = systemHeight;

        // orientation 변수를 사용해서 현재 화면 방향을 결정하고 그에 따라 screenWidth 조절 
        // true이면 높이를 너비로 바꾼다. 
        if (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
        {
            screenHeight = systemWidth;
        }
        return gazeY = 0.5f - y / screenHeight;
    }

    // Tracking 상태 반환 
    void onStarted()
    {
        isTracking = true;
    }
    void onStopped(StatusErrorType error)
    {
        isTracking = false;
    }
    //깜빡임 정보 
    void onBlink(long timestamp, bool isBlinkLeft, bool isBlinkRight, bool isBlink, float eyeOpenness)
    {
        userStatusBlink = "blink : " + isBlink;
        Debug.Log("onBlink " + isBlinkLeft + ", " + isBlinkRight + ", " + isBlink);
    }

    //이건 필요 없을 듯 
    void onAttention(long timestampBegin, long timestamEnd, float score)
    {
        userStatusAttention = "" + score;
        Debug.Log("onAttention " + score);
    }
    void onDrowsiness(long timestamp, bool isDrowsiness)
    {
        userStatusDrowsiness = "" + isDrowsiness;
        Debug.Log("onDrowsiness " + isDrowsiness);
    }

}



