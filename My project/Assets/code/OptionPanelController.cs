using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsPanelController : MonoBehaviour
{
    [Header("Audio Settings (0~100%)")]
    public Slider bgmVolumeSlider;     // 배경음악 볼륨 슬라이더 (0~100)
    public Slider sfxVolumeSlider;     // 효과음(SFX) 볼륨 슬라이더 (0~100)
    public Slider masterVolumeSlider;  // 마스터(전체) 볼륨 슬라이더 (0~100)

    [Header("Audio Sources (실시간 볼륨 적용)")]
    public AudioSource bgmAudioSource; // 배경음악 재생 AudioSource
    public AudioSource sfxAudioSource; // 효과음 재생 AudioSource

    [Header("Audio Mixer")]
    public AudioMixer newAudioMixer;   // NewAudioMixer asset 할당 (Master 하위에 BGM, SFX 그룹 있음)


    [Header("Video/Graphics Settings")]
    public Dropdown resolutionDropdown; // 해상도 선택 드롭다운
    public Toggle fullscreenToggle;     // 전체 화면 여부 토글

    [Header("Controller (Key Info)")]
    public Text controllerInfoText;     // 컨트롤러 키 안내 텍스트

    [Header("Control Buttons")]
    public Button backButton;  // 옵션 패널 닫기(뒤로가기)
    public Button saveButton;  // 설정 저장 버튼
    public Button resetButton; // 설정 초기화 버튼

    // PlayerPrefs 저장 키
    private const string KEY_BGM_VOLUME = "BGM_VOLUME";
    private const string KEY_SFX_VOLUME = "SFX_VOLUME";
    private const string KEY_MASTER_VOLUME = "MASTER_VOLUME";
    private const string KEY_RESOLUTION = "RESOLUTION_INDEX";
    private const string KEY_FULLSCREEN = "FULLSCREEN";

    private Resolution[] availableResolutions;

    // 컨트롤러(키 안내) 기본 문자열
    private string controllerInfo =
        "Jump: Space\n" +
        "Move Left: Left Arrow\n" +
        "Move Right: Right Arrow";

    void Start()
    {

        if (resolutionDropdown == null)
        {
        Debug.LogError("resolutionDropdown is not assigned in the inspector!");
        // 여기서 return 하면 이후 코드는 실행되지 않음.
        return;
        }
        // 해상도 목록 가져와 드롭다운 옵션으로 추가
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            options.Add(option);

            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        // 저장된 설정 불러오기 (저장값 없으면 기본값 사용)
        LoadSettings();

        // 컨트롤러 키 안내 텍스트 설정 (수정 불가능한 정보로 표시)
        if (controllerInfoText != null)
            controllerInfoText.text = controllerInfo;

        // 버튼 이벤트 연결
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveSettings);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSettings);
    }


    // 슬라이더의 값(0~100)을 -80dB ~ 0dB로 변환하는 헬퍼 메서드
    float SliderValueToDecibels(float sliderValue)
    {
        return Mathf.Lerp(-80f, 0f, sliderValue / 100f);
    }


    #region 슬라이더 실시간 볼륨 조절
    void OnBGMVolumeChanged(float value) {
        if (newAudioMixer != null)
        {
            float dB = SliderValueToDecibels(value);
            newAudioMixer.SetFloat("BGMVolume", dB);
        }
    }

    void OnSFXVolumeChanged(float value)
    {
        if (newAudioMixer != null)
        {
            float dB = SliderValueToDecibels(value);
            newAudioMixer.SetFloat("SFXVolume", dB);
        }
    }

    void OnMasterVolumeChanged(float value)
    {
        if (newAudioMixer != null)
        {
            float dB = SliderValueToDecibels(value);
            newAudioMixer.SetFloat("MasterVolume", dB);
        }
    }

    #endregion

    #region 설정 불러오기, 저장, 초기화

    void LoadSettings()
    {
        // 오디오 설정 불러오기 (기본값 100%)
        float bgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 50f);
        float sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 50f);
        float masterVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, 50f);
        if (bgmVolumeSlider != null) bgmVolumeSlider.value = bgmVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;

        // 비디오 설정 불러오기
        int resIndex = PlayerPrefs.GetInt(KEY_RESOLUTION, resolutionDropdown.options.Count - 1);
        if (resolutionDropdown != null) resolutionDropdown.value = resIndex;
        bool isFullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 1) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;
    }

    void SaveSettings()
    {
        if (bgmVolumeSlider != null)
            PlayerPrefs.SetFloat(KEY_BGM_VOLUME, bgmVolumeSlider.value);
        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat(KEY_SFX_VOLUME, sfxVolumeSlider.value);
        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, masterVolumeSlider.value);

        if (resolutionDropdown != null)
            PlayerPrefs.SetInt(KEY_RESOLUTION, resolutionDropdown.value);
        if (fullscreenToggle != null)
            PlayerPrefs.SetInt(KEY_FULLSCREEN, fullscreenToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();

        // 비디오 설정 적용
        Resolution selectedRes = availableResolutions[resolutionDropdown.value];
        Screen.SetResolution(selectedRes.width, selectedRes.height, fullscreenToggle.isOn);

        Debug.Log("설정이 저장되었습니다.");
    }

    void ResetSettings()
    {
        // 오디오 슬라이더 기본값 100%
        if (bgmVolumeSlider != null) bgmVolumeSlider.value = 50f;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = 50f;
        if (masterVolumeSlider != null) masterVolumeSlider.value = 50f;

        // 비디오 기본값: 최댓값 해상도, 전체 화면 켜짐
        if (resolutionDropdown != null) resolutionDropdown.value = resolutionDropdown.options.Count - 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = true;

        Debug.Log("설정이 기본값으로 초기화되었습니다.");
    }

    #endregion

    #region 버튼 이벤트 처리

    void OnBackButton()
    {
        // 옵션 패널 닫기(필요에 따라 이전 메뉴로 돌아가는 로직 추가)
        gameObject.SetActive(false);
        Debug.Log("옵션 패널이 닫혔습니다.");
    }

    #endregion
}
