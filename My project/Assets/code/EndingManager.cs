using System.Collections;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    // 기존 맵 UI (예: 게임 중 표시되는 HUD나 미니맵 등)
    public GameObject mapUI;

    // 엔딩 패널 (엔딩 화면 UI)
    public GameObject endingPanel;

    // 엔딩 상태를 나타내는 변수 (다른 스크립트에서 변경할 수 있습니다.)
    public static bool isEnding = false;
    private bool changePanel = false;
    private int num = 0;
    public GameObject stunPanel;
    public GameObject winnerPanel;

    void Start()
    {
        // 게임 시작 시엔 엔딩 패널은 비활성화 해둡니다.
        if (endingPanel != null)
        {
            endingPanel.SetActive(false);
        }
    }

    void Update()
    {
        // isEnding이 true로 설정되면, 엔딩 이벤트를 실행합니다.
        if (isEnding)
        {
            TriggerEnding();
            // 한 번 실행된 후 더 이상 반복 실행되지 않도록 isEnding을 false로 전환하거나
            // 별도의 플래그로 실행 여부를 관리합니다.
            isEnding = false;
            changePanel = true;
        }
    }

    // 엔딩 이벤트를 처리하는 함수
    public void TriggerEnding()
    {
        // 기존 맵 UI를 비활성화
        if (mapUI != null)
        {
            mapUI.SetActive(false);
        }
        // 엔딩 패널을 활성화
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
        }
        // 필요한 추가 엔딩 연출(예: 사운드, 애니메이션 등)을 여기서 실행할 수 있습니다.
    }
}
