using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;


namespace SBF.Network
{
#pragma warning disable 649
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        public TMP_InputField roomNameInput;
        public Button[] rooms;
        public Button prevButton;
        public Button nextButton;

        public GameObject noRoomNoticePanel;

        List<RoomInfo> localList = new List<RoomInfo>();
        int curPage = 1, maxPage, multiple;

        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private TMP_Text feedbackText;
        [SerializeField]
        private TMP_Text playerNameText;

        [SerializeField]
        private TMP_Text playerCountText;

        [Tooltip("이름을 적고, 게임 시작 버튼을 누를 수 있게 해주는 것")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("유저가 진행도를 알 수 있게 해주는 레이블")]

        #endregion

        #region Private Fields

        bool isConnecting;
        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            isConnecting = true;
            OnConnectedToMaster();
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            controlPanel.SetActive(true);
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();

        }


        private void Update()
        {

            PlayerCountCheck();

            

        }

        public override void OnConnectedToMaster()
        {

            //LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
            //// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            ////PhotonNetwork.JoinRandomRoom();
            //Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            //feedbackText.text = (PhotonNetwork.NickName+"님 안녕하세용");
            PhotonNetwork.JoinLobby();
            

        }

        public override void OnJoinedLobby()
        {
            playerNameText.text = (PhotonNetwork.NickName);
            localList.Clear();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");
            controlPanel.SetActive(true);
            isConnecting = false;
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
            Debug.Log("OnJoinRandomFailed() was called by PUN. 방 없어서 새로 방 만들었삼.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom("Room" + Random.Range(0, 100), new RoomOptions { MaxPlayers = maxPlayersPerRoom });


        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            //base.OnCreateRoomFailed(returnCode, message);
            PhotonNetwork.CreateRoom("Room" + Random.Range(0, 100), new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public void PlayerCountCheck()
        {
            int curPlayerCount = PhotonNetwork.CountOfPlayers;
            playerCountText.text = string.Format("로비 " + (curPlayerCount - PhotonNetwork.CountOfPlayersInRooms) +"명 / 전체 "+ curPlayerCount + "명 접속 중");
        }

        public override void OnJoinedRoom()
        {
            LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
            Debug.Log("OnJoinedRoom() called by PUN. 클라 방에 들어감");
            //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            //{
            //    Debug.Log("매칭 씬에 들어감");
                // #Critical
                // Load the Room Level.
                //SceneManager.LoadScene(1);
            PhotonNetwork.LoadLevel("Room");
           
            //}
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            int roomCount = roomList.Count;
            if (PhotonNetwork.CountOfRooms == 0)
            {
                foreach (Button btns in rooms)
                {
                    btns.gameObject.SetActive(false);
                }
                noRoomNoticePanel.SetActive(true);
            }
            else
            {
                foreach (Button btns in rooms)
                {
                    btns.gameObject.SetActive(true);
                }
                noRoomNoticePanel.SetActive(false);
            }
            for (int i = 0; i < roomCount; i++)
            {
                if (!roomList[i].RemovedFromList)
                {
                    if (!localList.Contains(roomList[i]))
                    {
                        localList.Add(roomList[i]);
                    }
                    else
                    {
                        localList[localList.IndexOf(roomList[i])] = roomList[i];
                    }
                }
                else if (localList.IndexOf(roomList[i]) != -1)
                {
                    localList.RemoveAt(localList.IndexOf(roomList[i]));
                }
                LocalListRenewal();
            }
        }

        #endregion


        #region Public Methods

        public void CreateRoom()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            isConnecting = true;
            feedbackText.text = "";
            if (PhotonNetwork.IsConnected)
            {
                controlPanel.SetActive(true);
                LogFeedback("Joining Room...");
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.CreateRoom(roomNameInput.text == "" ? "Room" + Random.Range(0,100) : roomNameInput.text, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            }
            else
            {
                LogFeedback("Connecting...");
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public void Connect()
        {

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            isConnecting = true;
            feedbackText.text = "";
            if (PhotonNetwork.IsConnected)
            {
                controlPanel.SetActive(true);
                LogFeedback("Joining Room...");
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                LogFeedback("Connecting...");
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server. 서버에 연결된 상태인지 아닌지 체크. 이런 글자들 다 그대로 옮겨놓는게 낫겠지?

        }

        public void LocalListClick(int num)
        {
            if (num == -2)
            {
                --curPage;
            }
            else if (num == -1)
            {
                ++curPage;
            }
            else PhotonNetwork.JoinRoom(localList[multiple + num].Name);
            LocalListRenewal();
        }

        public void LocalListRenewal()
        {
            // 최대 페이지 정의. 룸 개수를 배열의 최대 개수로 나눈 값을 들고온다.
            maxPage = (localList.Count % rooms.Length == 0) ? localList.Count / rooms.Length : localList.Count / rooms.Length + 1;

            //이전 다음버튼이 꺼지도록 처리한다.
            prevButton.interactable = (curPage <= 1) ? false : true;
            nextButton.interactable = (curPage >= maxPage) ? false : true;

            //페이지에 맞게 리스트에 대입한다.
            multiple = (curPage - 1) * rooms.Length;
            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i].interactable = (multiple + i < localList.Count) ? true : false;
                rooms[i].transform.GetChild(0).GetComponent<TMP_Text>().text = (multiple + i < localList.Count) ? localList[multiple + i].Name : "";
                rooms[i].transform.GetChild(1).GetComponent<TMP_Text>().text = (multiple + i < localList.Count) ? localList[multiple + i].PlayerCount + "/" + localList[multiple + i] : "";
            }
        }

        void LogFeedback(string message)
        {
            // we do not assume there is a feedbackText defined.
            if (feedbackText == null)
            {
                return;
            }

            // add new messages as a new line and at the bottom of the log.
            feedbackText.text = System.Environment.NewLine + message;
        }
        #endregion

    }


}