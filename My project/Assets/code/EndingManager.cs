using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    // 기존 맵 UI (예: HUD, 미니맵 등)
    public GameObject mapUI;
    // 엔딩 패널 (엔딩 화면 UI)
    public GameObject endingPanel;
    // 엔딩 상태를 나타내는 전역 변수 (다른 스크립트에서 접근 가능)
    public static bool isEnding = false;

    // 각 플레이어의 정보를 표시할 텍스트 배열 (Inspector에서 할당)
    public TMP_Text[] nameTexts;      // 플레이어 이름 표시용
    public TMP_Text[] stunTexts;      // stun 횟수 표시용
    public TMP_Text[] jumpTexts;      // jump 횟수 표시용
    public TMP_Text[] distanceTexts;  // 기준점과의 y축 거리 표시용

    public Button mainMenuButton;
    public Button exitButton;

    // 기준점 Y (예: 0 또는 원하는 값) → Inspector에서 설정하거나 기본값 사용
    public static float referenceY = 0f;

    void Start()
    {
        // 엔딩 패널은 기본적으로 비활성화
        if (endingPanel != null)
        {
            endingPanel.SetActive(false);
        }

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // 엔딩이 시작되면 기존 UI를 끄고 엔딩 패널을 활성화한 후, 플레이어 스탯을 업데이트합니다.
        if (isEnding)
        {
            TriggerEnding();
            UpdateStatsDisplay();
            isEnding = false; // 엔딩 이벤트는 한 번만 실행하도록 함
        }
    }

    // 엔딩 이벤트 처리: 기존 UI 끄고 엔딩 패널 활성화
    public void TriggerEnding()
    {
        if (mapUI != null)
        {
            mapUI.SetActive(false);
        }
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
        }
    }

    public void ReturnToMainMenu()
    {
        if (PhotonNetwork.IsConnected) {
            Debug.Log("ReturnToMainMenu 호출됨. PhotonNetwork.LeaveRoom() 실행");
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        } else {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // 각 플레이어의 스탯을 각기 다른 텍스트 배열에 업데이트하는 함수
    void UpdateStatsDisplay()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            // 플레이어 이름 업데이트
            if (nameTexts != null && i < nameTexts.Length)
            {
                nameTexts[i].text = players[i].NickName;
            }

            // 각 플레이어의 커스텀 프로퍼티에서 스탯 읽기
            int stun = 0, jump = 0;
            float yDist = 0f;
            if (players[i].CustomProperties.ContainsKey("stunCount"))
                stun = (int)players[i].CustomProperties["stunCount"];
            if (players[i].CustomProperties.ContainsKey("jumpCount"))
                jump = (int)players[i].CustomProperties["jumpCount"];
            if (players[i].CustomProperties.ContainsKey("yDistance"))
                yDist = System.Convert.ToSingle(players[i].CustomProperties["yDistance"]);

            // stun 횟수 업데이트
            if (stunTexts != null && i < stunTexts.Length)
            {
                stunTexts[i].text = stun.ToString();
            }
            // jump 횟수 업데이트
            if (jumpTexts != null && i < jumpTexts.Length)
            {
                jumpTexts[i].text = jump.ToString();
            }
            // y축 거리 업데이트 (소수점 두 자리)
            if (distanceTexts != null && i < distanceTexts.Length)
            {
                distanceTexts[i].text = yDist.ToString("F2");
            }
        }

        int playerCount = players.Length;
        // 만약 플레이어 수가 텍스트 배열의 길이보다 적다면, 나머지 슬롯을 빈 문자열("")로 설정
        for (int i = playerCount; i < nameTexts.Length; i++)
        {
        nameTexts[i].text = "";
        }
        for (int i = playerCount; i < stunTexts.Length; i++)
        {
        stunTexts[i].text = "";
        }
        for (int i = playerCount; i < jumpTexts.Length; i++)
        {
        jumpTexts[i].text = "";
        }
        for (int i = playerCount; i < distanceTexts.Length; i++)
        {
        distanceTexts[i].text = "";
        }
    }
}
