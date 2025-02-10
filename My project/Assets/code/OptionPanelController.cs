using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsPanelController : MonoBehaviour
{
    [Header("OptionPanel")]
    public GameObject OptionPanel;
    // OptionPanel 내에서 각 세트별 하위 패널들
    [Header("Option Sub Panels")]
    public GameObject AudioSet;       // 오디오 설정 패널
    public GameObject VideoSet;       // 그래픽(비디오) 설정 패널
    public GameObject ControllerSet;  // 컨트롤러(키 안내) 설정 패널
    public GameObject ETCSet;         // 기타 정보(예: 개발자소개, 펀딩 등) 패널

    // OptionPanel 내부 버튼들
    public Button AudioButton;
    public Button VideoButton;
    public Button ControllerButton;
    public Button ETCButton;
    public Button OptionBackButton;   // OptionPanel에서 뒤로가기(닫기) 버튼

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
    public TMP_Dropdown resolutionDropdown; // 해상도 선택 드롭다운
    public Toggle fullscreenToggle;     // 전체 화면 여부 토글

    [Header("Controller (Key Info)")]
    public Text controllerInfoText;     // 컨트롤러 키 안내 텍스트

    [Header("Control Buttons")]
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
            return;
        }

        // 1. 수동으로 4개의 해상도 옵션을 설정
        List<string> options = new List<string>()
        {
            "1920 x 1080",
            "1680 x 1050",
            "1440 x 900",
            "1280 x 800"
        };
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        // 2. availableResolutions 배열을 4개의 해상도로 직접 생성
        availableResolutions = new Resolution[4];
        availableResolutions[0] = new Resolution() { width = 1920, height = 1080 };
        availableResolutions[1] = new Resolution() { width = 1680, height = 1050 };
        availableResolutions[2] = new Resolution() { width = 1440, height = 900 };
        availableResolutions[3] = new Resolution() { width = 1280, height = 800 };

        // 기본 해상도를 1920x1080 (인덱스 0)으로 설정
        resolutionDropdown.value = 0;

        // 컨트롤러 키 안내 텍스트 설정 (수정 불가능한 정보로 표시)
        if (controllerInfoText != null)
            controllerInfoText.text = controllerInfo;

        // 저장된 설정 불러오기 (저장값 없으면 기본값 사용)
        LoadSettings();

        // 버튼 이벤트 연결
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveSettings);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSettings);

        if (bgmVolumeSlider != null) {
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
        if (sfxVolumeSlider != null) {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        if (masterVolumeSlider != null) {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if(AudioButton != null)
            AudioButton.onClick.AddListener(ShowAudioSet);
        if(VideoButton != null)
            VideoButton.onClick.AddListener(ShowVideoSet);
        if(ControllerButton != null)
            ControllerButton.onClick.AddListener(ShowControllerSet);
        if(ETCButton != null)
            ETCButton.onClick.AddListener(ShowETCSet);

        if(OptionBackButton != null)
            OptionBackButton.onClick.AddListener(() => { 
                OptionPanel.SetActive(false);
            });
        
    }


    float ConvertSliderValueToDecibels(float sliderValue)
    {
        float volume = sliderValue / 100f;
        Debug.Log($"ConvertSliderValueToDecibels - Slider Value: {sliderValue} -> Volume (0-1): {volume}");
    
        // 0에 가까운 값은 로그 계산 시 무한대에 가까워지므로 별도로 처리
        if (volume <= 0.0001f) {
            Debug.Log("Volume is near zero; returning -80 dB");
            return -80f;
        }
        // 20 * log10(volume)은 volume이 1일 때 0 dB, 0.5일 때 약 -6 dB를 반환
        float dB = 20f * Mathf.Log10(volume);
        Debug.Log($"Calculated dB: {dB}");

        return dB;
    }


    #region 슬라이더 실시간 볼륨 조절
    public void OnBGMVolumeChanged(float sliderValue)
    {
        float dB = ConvertSliderValueToDecibels(sliderValue);
        newAudioMixer.SetFloat("BGMVolume", dB);
        Debug.Log($"OnBGMVolumeChanged - Slider Value: {sliderValue}, dB: {dB}");

    }

    public void OnSFXVolumeChanged(float sliderValue)
    {
        float dB = ConvertSliderValueToDecibels(sliderValue);
        newAudioMixer.SetFloat("SFXVolume", dB);
        Debug.Log($"OnSFXVolumeChanged - Slider Value: {sliderValue}, dB: {dB}");

    }

    public void OnMasterVolumeChanged(float sliderValue)
    {
        float dB = ConvertSliderValueToDecibels(sliderValue);
        newAudioMixer.SetFloat("MasterVolume", dB);
        Debug.Log($"OnMasterVolumeChanged - Slider Value: {sliderValue}, dB: {dB}");

    }

    #endregion

    #region 설정 불러오기, 저장, 초기화

    public void LoadSettings()
    {
        // 오디오 설정 불러오기 (기본값 100%)
        float bgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 50f);
        float sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 50f);
        float masterVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, 50f);
        if (bgmVolumeSlider != null) bgmVolumeSlider.value = bgmVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;

        // 비디오 설정 불러오기
        int resIndex = PlayerPrefs.GetInt(KEY_RESOLUTION, 0);
        if (resolutionDropdown != null) resolutionDropdown.value = resIndex;
        bool isFullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 0) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;

    }

    public void SaveSettings()
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

    public void ResetSettings()
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

    #region OPTION_PANEL LOGIC
    // 각 하위 옵션 세트를 표시하는 메서드들.
    private void ShowAudioSet() {
        if (AudioSet != null) AudioSet.SetActive(true);
        if (VideoSet != null) VideoSet.SetActive(false);
        if (ControllerSet != null) ControllerSet.SetActive(false);
        if (ETCSet != null) ETCSet.SetActive(false);
    }

    private void ShowVideoSet() {
        if (AudioSet != null) AudioSet.SetActive(false);
        if (VideoSet != null) VideoSet.SetActive(true);
        if (ControllerSet != null) ControllerSet.SetActive(false);
        if (ETCSet != null) ETCSet.SetActive(false);
    }

    private void ShowControllerSet() {
        if (AudioSet != null) AudioSet.SetActive(false);
        if (VideoSet != null) VideoSet.SetActive(false);
        if (ControllerSet != null) ControllerSet.SetActive(true);
        if (ETCSet != null) ETCSet.SetActive(false);
    }

    private void ShowETCSet() {
        if (AudioSet != null) AudioSet.SetActive(false);
        if (VideoSet != null) VideoSet.SetActive(false);
        if (ControllerSet != null) ControllerSet.SetActive(false);
        if (ETCSet != null) ETCSet.SetActive(true);
    }
    #endregion
}

