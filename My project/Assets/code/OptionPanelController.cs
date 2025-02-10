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

        // 컨트롤러 키 안내 텍스트 설정 (수정 불가능한 정보로 표시)
        if (controllerInfoText != null)
            controllerInfoText.text = controllerInfo;

        // 저장된 설정 불러오기 (저장값 없으면 기본값 사용)
        LoadSettings();

        // 버튼 이벤트 연결
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
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
        int resIndex = PlayerPrefs.GetInt(KEY_RESOLUTION, resolutionDropdown.options.Count - 1);
        if (resolutionDropdown != null) resolutionDropdown.value = resIndex;
        bool isFullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 1) == 1;
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

    #region 버튼 이벤트 처리

    public void OnBackButton()
    {
        // 옵션 패널 닫기(필요에 따라 이전 메뉴로 돌아가는 로직 추가)
        gameObject.SetActive(false);
        Debug.Log("옵션 패널이 닫혔습니다.");
    }

    #endregion
}
