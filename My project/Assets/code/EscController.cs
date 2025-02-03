using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EscController : MonoBehaviourPunCallbacks
{
    [Header("ESCPanel")]
    public GameObject escPanel;
    public Button continueButton;
    public Button optionsButton;
    public Button mainMenuButton;
    public Button exitButton;

    [Header("OptionsPanel")]
    public GameObject optionsPanel;

    private bool isPaused = false;

    void Start()
    {
        // 시작 시 패널들을 모두 숨김
        if (escPanel != null)
            escPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // 각 버튼의 클릭 이벤트 등록
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptions);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // ESC키를 누르면 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 우선 optionsPanel이 켜져 있다면 optionsPanel만 꺼짐
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                optionsPanel.SetActive(false);
                return;
            }

            // optionsPanel이 꺼져있는 경우 ESC패널을 토글
            if (isPaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (escPanel != null)
            escPanel.SetActive(true);
        // 필요에 따라 Time.timeScale = 0f; 로 게임 일시정지도 할 수 있음.
    }

    public void ContinueGame()
    {
        isPaused = false;
        if (escPanel != null)
            escPanel.SetActive(false);
        // 필요에 따라 Time.timeScale = 1f; 로 게임 재개 처리.
    }

    public void OpenOptions()
    {
        // 옵션 패널 열려있으면 옵션 패널만 닫고 끝내기
        if (optionsPanel != null) {
            optionsPanel.SetActive(true);
            return;
        }
    }

    public void ReturnToMainMenu()
    {
        isPaused = false;
        // 옵션 패널이 켜져 있다면 먼저 꺼줍니다.
        if (optionsPanel != null) {
            optionsPanel.SetActive(false);
        }
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // MainMenu 씬으로 전환
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}