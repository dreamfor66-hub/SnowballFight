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
    public class ServerLauncher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields


        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private TMP_Text feedbackText;


        #endregion

        #region Private Fields

        bool isConnecting;
        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {

        }


        private void Update()
        {

        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");
            isConnecting = false;
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
            Debug.Log("OnJoinRandomFailed() was called by PUN. 방 없어서 새로 방 만들었삼.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.

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
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }
        #endregion


        #region Public Methods

        public void Connect()
        {

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            isConnecting = true;
            feedbackText.text = "입장 성공.";
            if (PhotonNetwork.IsConnected)
            {
                LogFeedback("Joining Room...");
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinLobby();
                SceneManager.LoadScene("Lobby");
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