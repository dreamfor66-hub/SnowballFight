using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

using Photon.Pun;
using Photon.Realtime;


namespace SBF.Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        bool isConnecting;
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;


        [Tooltip("�̸��� ����, ���� ���� ��ư�� ���� �� �ְ� ���ִ� ��")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("������ ���൵�� �� �� �ְ� ���ִ� ���̺�")]
        [SerializeField]
        private GameObject progressLabel;

        #endregion

        #region Private Fields

        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            //Connect();
        }

       

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
            }
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause); //�Ӷ�°��� �̰�
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. �� ��� ���� �� �������.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Ŭ�� �濡 ��");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'MainGameScene' ");

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
            //PhotonNetwork.ConnectUsingSettings();
            isConnecting = true;
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server. ������ ����� �������� �ƴ��� üũ. �̷� ���ڵ� �� �״�� �Űܳ��°� ������?
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        #endregion

    }


}