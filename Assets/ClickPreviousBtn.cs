using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickPreviousBtn : MonoBehaviour
{
    // public ToggleGroup toggleGroupV;
    // public ToggleGroup toggleGroupN;

    // public Slider sliderB;
    // public Slider sliderE;

    public void onClickPreviousBtn()
    {
    //     // 토글 그룹 값 저장
    //     SaveToggleGroupValue(toggleGroupV);
    //     SaveToggleGroupValue(toggleGroupN);
        
    //     // 슬라이더 값 저장
    //     SaveSliderValue(sliderB);
    //     SaveSliderValue(sliderE);

    //     // 씬 전환
        SceneManager.LoadScene("TwelveStars");
    }

    // private void SaveToggleGroupValue(ToggleGroup toggleGroup)
    // {
    //     // 토글 그룹의 현재 선택된 토글 값을 저장
    //     Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
    //     for (int i = 0; i < toggles.Length; i++)
    //     {
    //         PlayerPrefs.SetInt($"Toggle{i}", toggles[i].isOn ? 1 : 0);
    //     }

    //     PlayerPrefs.Save();
    // }

    // private void SaveSliderValue()
    // {
    //     // 슬라이더의 현재 값 저장
    //     PlayerPrefs.SetFloat("SliderValue", slider.value);
    //     PlayerPrefs.Save();
    // }
}
