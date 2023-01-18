using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using System.Collections;

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
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            controlPanel.SetActive(true);
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            OnConnectedToMaster();
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
            playerNameText.text = (PhotonNetwork.NickName);

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
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

        }

        public void PlayerCountCheck()
        {
            int curPlayerCount = PhotonNetwork.CountOfPlayers;
            playerCountText.text = string.Format(curPlayerCount + "명 접속 중");
        }

        public override void OnJoinedRoom()
        {
            LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
            Debug.Log("OnJoinedRoom() called by PUN. 클라 방에 들어감");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("매칭 씬에 들어감");

                // #Critical
                // Load the Room Level.
                //SceneManager.LoadScene(1);
                PhotonNetwork.LoadLevel("Room");
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
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
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