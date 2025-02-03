using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("MenuPanel")]
    public GameObject MenuPanel;
    public Button SingleplayButton;
    public Button MultiplayButton;
    public Button OptionButton;
    public Button ExitButton;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public Button[] CellBtn;
    public Button PreviousCellBtn;
    public Button NextCellBtn;
    public Button BackMainButton;
    public Button JoinButton;
    public Button CreateButton;
    public Button ReloadButton;

    [Header("CreatePanel")]
    public GameObject CreatePanel;
    public TMP_InputField CreateRoomNameInput;
    public TMP_Dropdown MaxPlayers;
    public TMP_Dropdown MapSelect;
    public TMP_InputField CreatePasswordInput; // 이부분은 optional 부분이므로 빈칸일 시 공개 방으로 설정
    public Button CreateRoomButton;
    public Button BackLobbyButton;

    [Header("JoinMenu")]
    public GameObject JoinMenu;
    public GameObject JoinRoomNotSelected;
    public TMP_Text JoinRoomNameText;
    public TMP_InputField JoinPlayerNameInput;
    public TMP_InputField JoinPasswordInput; // 공개 방일 시 입력 칸 비활성화
    public Button JoinRoomButton;
    public Button BackLobbyButton2;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public TMP_Text[] Players;
    public TMP_Text RoomNameText;
    public TMP_Text RoomPlayerLimitText;
    public TMP_Text RoomMapText;
    public TMP_Text[] ChatText;
    public TMP_InputField ChatInput;
    public Button ChatSendButton;
    public Button BackLobbyButton3;
    public Button GameProceedButton;

    [Header("ETC")]
    public PhotonView PV;

    private List<RoomInfo> roomList = new List<RoomInfo>();
    private int currentPage = 1;
    private int maxPage = 1;
    private int multiple = 0;
    private RoomInfo selectedRoom = null;

    public static bool isReturningToMainMenu = false;


    #region UNITY_CALLBACKS
    private void Awake() {
        Screen.SetResolution(1920, 1080, false);

        MenuPanel.SetActive(true);
        CreatePanel.SetActive(false);
        JoinMenu.SetActive(false);
        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(false);

        //SingleplayButton.onClick.AddListener(onClickSingleplay);
        MultiplayButton.onClick.AddListener(onClickMultiplay);
        //OptionButton.onClick.AddListener(onClickOption);
        ExitButton.onClick.AddListener(() => Application.Quit());

        BackMainButton.onClick.AddListener(BackToMainFromLobby);
        CreateButton.onClick.AddListener(OpenCreatePanel);
        JoinButton.onClick.AddListener(OpenJoinMenu); 
        ReloadButton.onClick.AddListener(ReloadLobby);

        CreateRoomButton.onClick.AddListener(CreateRoomConfirm);
        BackLobbyButton.onClick.AddListener(CloseCreatePanel);

        JoinRoomButton.onClick.AddListener(JoinRoomConfirm);
        BackLobbyButton2.onClick.AddListener(CloseJoinMenu);

        ChatSendButton.onClick.AddListener(SendChat);
        BackLobbyButton3.onClick.AddListener(LeaveRoom);
        GameProceedButton.onClick.AddListener(GameProceed);

        for (int i = 0; i < CellBtn.Length; i++) {
            int index = i;
            if (CellBtn[i] != null)
                CellBtn[i].onClick.AddListener(() => OnRoomCellClick(index));
        }

        PreviousCellBtn.onClick.AddListener(OnClickPreviousPage);
        NextCellBtn.onClick.AddListener(OnClickNextPage);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }
    #endregion

    private void Start()
    {
        // Start에서 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // 초기 UI 상태 설정 등 기존 Start() 로직
    }

    private void OnDestroy()
    {
        // OnDestroy에서 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (scene.name == "MainMenu")
    {
        Debug.Log("OnSceneLoaded: MainMenu 씬이 로드되었습니다. 패널 상태 초기화 진행");

        if (MenuPanel != null) {
            MenuPanel.SetActive(true);
            Debug.Log("MenuPanel 활성화");
        } else {
            Debug.LogWarning("MenuPanel 참조가 없습니다.");
        }

        if (LobbyPanel != null) {
            LobbyPanel.SetActive(false);
            Debug.Log("LobbyPanel 비활성화");
        } else {
            Debug.LogWarning("LobbyPanel 참조가 없습니다.");
        }

        if (CreatePanel != null) {
            CreatePanel.SetActive(false);
            Debug.Log("CreatePanel 비활성화");
        }

        if (JoinMenu != null) {
            JoinMenu.SetActive(false);
            Debug.Log("JoinMenu 비활성화");
        }

        if (RoomPanel != null) {
            RoomPanel.SetActive(false);
            Debug.Log("RoomPanel 비활성화");
        }
    }
}


    #region MENU_PANEL
    private void OnClickSinglePlay() {
        // SinglePlay
    }

    private void onClickMultiplay() {
        MenuPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnClickOption() {
        // OptionPanel.SetActive(true);
    }
    #endregion



    #region PHOTON_CALLBACKS (OnConnected, Lobby, RoomList)
    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        LobbyPanel.SetActive(true);
        roomList.Clear();
        currentPage = 1;
        maxPage = 1;
        MyListRenewal();
    }

    public override void OnDisconnected(DisconnectCause cause) {
    // 만약 NetworkManager가 파괴되어 UI 참조가 null이라면 바로 리턴
    if (MenuPanel == null) return;

    MenuPanel.SetActive(true);
    LobbyPanel.SetActive(false);
    CreatePanel.SetActive(false);
    JoinMenu.SetActive(false);
    RoomPanel.SetActive(false);

    roomList.Clear();
    selectedRoom = null;
}

    public override void OnRoomListUpdate(List<RoomInfo> updatedList) {
        foreach (RoomInfo info in updatedList) {
            if (!info.RemovedFromList) {
                if (!roomList.Contains(info))
                    roomList.Add(info);
                else {
                    roomList[roomList.IndexOf(info)] = info;
                }
            } else {
                int idx = roomList.IndexOf(info);
                if (idx != -1)
                    roomList.RemoveAt(idx);
            }
        }
        MyListRenewal();
    }
    #endregion


    #region LOBBY_PANEL LOGIC
    private void BackToMainFromLobby() {
        PhotonNetwork.Disconnect();
        LobbyPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }

    private void OpenCreatePanel() {
        LobbyPanel.SetActive(false);
        CreatePanel.SetActive(true);
    }
    private void CloseCreatePanel() {
        CreatePanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }
    private void OpenJoinMenu() {
        if (selectedRoom == null) {
            // JoinRoomNotSelected.SetActive(true);
            return;
        }
        JoinRoomNameText.text = selectedRoom.Name;
        string pwd = "";
        if (selectedRoom.CustomProperties.ContainsKey("pwd")) {
            pwd = selectedRoom.CustomProperties["pwd"].ToString();
        }
        bool hasPwd = !string.IsNullOrEmpty(pwd);
        JoinPasswordInput.gameObject.SetActive(hasPwd);

        JoinMenu.SetActive(true);
    }

    private void CloseJoinMenu() {
        JoinMenu.SetActive(false);
        JoinPasswordInput.text = "";
    }

    private void ReloadLobby() {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    private void OnClickPreviousPage() {
        currentPage--;
        MyListRenewal();
    }

    private void OnClickNextPage() {
        currentPage++;
        MyListRenewal();
    }

    private void OnRoomCellClick(int index) {
        int roomIndex = multiple + index;
        if (roomIndex < roomList.Count) {
            selectedRoom = roomList[roomIndex];
        }
    }

    private void MyListRenewal() {
        int cellCount = CellBtn.Length;

        // 최대 페이지
        maxPage = (roomList.Count % cellCount == 0)
            ? roomList.Count / cellCount
            : roomList.Count / cellCount + 1;
        if (maxPage == 0) maxPage = 1;

        // 페이지 범위 보정
        if (currentPage < 1) currentPage = 1;
        if (currentPage > maxPage) currentPage = maxPage;

        // 이전, 다음 버튼
        PreviousCellBtn.interactable = (currentPage > 1);
        NextCellBtn.interactable = (currentPage < maxPage);

        multiple = (currentPage - 1) * cellCount;

        // UI 표시
        for (int i=0; i<cellCount; i++) {
            Button btn = CellBtn[i];

            TMP_Text gameNameText    = btn.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text mapText         = btn.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text playerText      = btn.transform.GetChild(2).GetComponent<TMP_Text>();
            TMP_Text passwordText    = btn.transform.GetChild(3).GetComponent<TMP_Text>();
            TMP_Text statusText      = btn.transform.GetChild(4).GetComponent<TMP_Text>();

            int roomIndex = multiple + i;
            if (roomIndex < roomList.Count) {
                RoomInfo r = roomList[roomIndex];
                btn.interactable = true;

                // GameName
                string gameName = r.Name;
                gameNameText.text = gameName;

                // Map
                string mapName = "Unknown";
                if (r.CustomProperties.ContainsKey("map")) {
                    object mapObj = r.CustomProperties["map"];
                    if (mapObj != null) {
                        mapName = mapObj.ToString();
                    }
                }
                mapText.text = mapName;

                // Player
                playerText.text = $"{r.PlayerCount}/{r.MaxPlayers}";

                // Password
                bool locked = false;
                if (r.CustomProperties.ContainsKey("pwd")) {
                    string pwd = r.CustomProperties["pwd"].ToString();
                    locked = !string.IsNullOrEmpty(pwd);
                }
                passwordText.text = locked ? "O" : "X";

                // Status
                statusText.text = r.IsOpen ? "Not Yet" : "Playing";
            } else {
                btn.interactable = false;

                gameNameText.text = "";
                mapText.text = "";
                playerText.text = "";
                passwordText.text = "";
                statusText.text = "";
            }
        }
    }
    #endregion

    #region CREATE_PANEL
    private void CreateRoomConfirm() {
        PhotonNetwork.LocalPlayer.NickName = "Host";

        string roomName = CreateRoomNameInput.text;
        if (string.IsNullOrEmpty(roomName)) {
            roomName = "Room" + Random.Range(0, 9999);
        }
        byte maxPlayersVal = byte.Parse(MaxPlayers.options[MaxPlayers.value].text);
        string mapName = MapSelect.options[MapSelect.value].text;
        string pwd = CreatePasswordInput.text.Trim();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersVal;

        // 커스텀 프로퍼티 설정
        ExitGames.Client.Photon.Hashtable cp = new ExitGames.Client.Photon.Hashtable();
        cp.Add("map", mapName);
        cp.Add("pwd", pwd);
        roomOptions.CustomRoomProperties = cp;

        roomOptions.CustomRoomPropertiesForLobby = new string[] {"map", "pwd"};
        Debug.Log($"[CreateRoomConfirm] Creating Room: {roomName}");
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogWarning($"CreateRoom Failed. Code={returnCode}, Msg={message}");
        CreateRoomNameInput.text = "";
        CreateRoomConfirm();
    }
    #endregion

    #region JOIN_MENU
    private void JoinRoomConfirm() {
        if (selectedRoom == null) return;

        string playerName = JoinPlayerNameInput.text;
        if (string.IsNullOrEmpty(playerName)) {
            playerName = "Player" + Random.Range(0, 9999);
        }
        PhotonNetwork.LocalPlayer.NickName = playerName;

        string storedPwd = "";
        if (selectedRoom.CustomProperties.ContainsKey("pwd")) {
            storedPwd = selectedRoom.CustomProperties["pwd"].ToString();
        }

        string inputPwd = JoinPasswordInput.text;
        if (!string.IsNullOrEmpty(storedPwd)) {
            if (!storedPwd.Equals(inputPwd)) {
                Debug.LogWarning("Password do not match");
                return;
            }
        }
        PhotonNetwork.JoinRoom(selectedRoom.Name);
    }
    #endregion


    #region ROOM_PANEL
    public override void OnJoinedRoom() {
        Debug.Log("[OnJoinedRoom] 현재 유저가 방에 들어갔습니다. (방장 포함)");

        LobbyPanel.SetActive(false);
        CreatePanel.SetActive(false);
        JoinMenu.SetActive(false);
        RoomPanel.SetActive(true);

        selectedRoom = null;

        UpdateRoomPanelInfo();
        foreach (var t in ChatText) {
            t.text = "";
        }
        ChatInput.text = "";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomPanelInfo();
        PV.RPC("ChatRPC", RpcTarget.All,$"<color=yellow>{newPlayer.NickName} joined the game.</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomPanelInfo();
        PV.RPC("ChatRPC", RpcTarget.All, 
               $"<color=yellow>{otherPlayer.NickName} left the game.</color>");
    }

    private void UpdateRoomPanelInfo() {
        if (!PhotonNetwork.InRoom) return;

        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        RoomPlayerLimitText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        string map = "";
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("map")) {
            map = PhotonNetwork.CurrentRoom.CustomProperties["map"].ToString();
        }
        RoomMapText.text = string.IsNullOrEmpty(map) ? "Unknown" : map;

        Player[] playerList = PhotonNetwork.PlayerList;
        for (int i=0; i<Players.Length; i++) {
            if (i < playerList.Length) {
                Players[i].text = playerList[i].NickName;
            } else {
                Players[i].text = "";
            }
        }

        GameProceedButton.interactable = PhotonNetwork.IsMasterClient;

    }

    private void LeaveRoom() {
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
    // 메인 메뉴로 돌아가는 상황이라면 MainMenu UI만 보이도록 설정합니다.
    if (isReturningToMainMenu)
    {
        if (MenuPanel != null) MenuPanel.SetActive(true);
        if (LobbyPanel != null) LobbyPanel.SetActive(false);
        if (CreatePanel != null) CreatePanel.SetActive(false);
        if (JoinMenu != null) JoinMenu.SetActive(false);
        if (RoomPanel != null) RoomPanel.SetActive(false);
        // 플래그는 사용 후 리셋합니다.
        isReturningToMainMenu = false;
    }
    else
    {
        // 그 외의 상황(예, 일반적으로 로비로 돌아갈 때)
        if (LobbyPanel != null) LobbyPanel.SetActive(true);
        if (RoomPanel != null) RoomPanel.SetActive(false);
    }
    PhotonNetwork.LocalPlayer.NickName = "";
}

    private void GameProceed() {
        if (!PhotonNetwork.IsMasterClient) {
            Debug.LogWarning("게임 시작은 방장만 할 수 있습니다.");
            return;
        }

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("map")) {
            Debug.LogWarning("맵 정보가 없습니다.");
            CancelRoom();
            return;
        }

        string map = PhotonNetwork.CurrentRoom.CustomProperties["map"].ToString();
        if (string.IsNullOrEmpty(map)) {
            Debug.LogWarning("맵 정보가 없습니다.");
            CancelRoom();
            return;
        }

        string sceneToLoad = "";
        switch (map) {
            case "Original":
                sceneToLoad = "SampleScene";
                break;
            default:
                CancelRoom();
                return;
        }

        Destroy(gameObject);

        PhotonNetwork.LoadLevel(sceneToLoad);
        
    }

    private void CancelRoom() {
        PhotonNetwork.CurrentRoom.IsOpen = false;

        PV.RPC("ChatRPC", RpcTarget.All, "<color=red>맵 정보가 유효하지 않아 게임을 시작할 수 없습니다. 메인 화면으로 이동합니다.</color>");
        
        PhotonNetwork.LeaveRoom();
    }
    #endregion




    #region CHAT_LOGIC
    private void SendChat() {
        if (string.IsNullOrEmpty(ChatInput.text)) return;
        string msg = PhotonNetwork.NickName + " : " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }

    [PunRPC]
    private void ChatRPC(string msg) {
        bool isFull = true;
        for (int i=0; i<ChatText.Length; i++) {
            if (string.IsNullOrEmpty(ChatText[i].text)) {
                ChatText[i].text = msg;
                isFull = false;
                break;
            }
        }
        if (isFull) {
            for (int i=0; i<ChatText.Length-1; i++) {
                ChatText[i].text = ChatText[i+1].text;
            }
            ChatText[ChatText.Length-1].text = msg;
        }
    }
    #endregion

        
}