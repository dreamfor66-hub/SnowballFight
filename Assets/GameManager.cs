using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace SBF.Network
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public bool instantiateTrigger = true;
        public bool loadAranaTrigger = true;
        public bool roomCloseTrigger = true;

        private void Awake()
        {
            //instantiateTrigger = true;
            //loadAranaTrigger = true;
        }

        void Start()
        {
            Instance = this;
            //instantiateTrigger = true;
            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Lobby");

                return;
            }
            if (playerPrefab == null)
            { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

                Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            
            else
            {
                if (PlayerController.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
                    Debug.LogError("인스턴스 트리거 밝혔거든");
                    //instantiateTrigger = false;

                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                    //instantiateTrigger = false;
                }
            }

        }

        #region Photon Callbacks

        public void Update()
        {
            //if (SceneManager.GetActiveScene().name != "Room for 2")
            //{
            //    return;
            //}
            //if (instantiateTrigger) /*&& SceneManager.GetActiveScene().name == "Room for 2"*/
            //{
            //    //여기서 부터는 매칭 이후에 작동해야 하는 상황임
            //    if (playerPrefab == null)
            //    { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

            //        Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            //    }
            //    else
            //    {
            //        if (PlayerController.LocalPlayerInstance == null)
            //        {
            //            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            //            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            //            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
            //            Debug.LogError("인스턴스 트리거 밝혔거든");
            //            instantiateTrigger = false;

            //        }
            //        else
            //        {
            //            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            //            instantiateTrigger = false;
            //        }
            //    }
            //}
            //}

            //이건 매칭이 끝나고 방에 들어와서 모든 플레이어가 자리에 준비중일때 시작할 수 있음
            //if (SceneManager.GetActiveScene().name == "Room for 2" && /*roomCloseTrigger &&*/ (PhotonNetwork.CurrentRoom.IsOpen || PhotonNetwork.CurrentRoom.IsVisible))
            //{
            //    Debug.Log("방 닫았음;");
            //    PhotonNetwork.CurrentRoom.IsOpen = false;
            //    PhotonNetwork.CurrentRoom.IsVisible = false;
            //    //roomCloseTrigger = false;
            //}

            //if (PhotonNetwork.CurrentRoom.IsOpen == false && PhotonNetwork.CurrentRoom.IsVisible == false)
            //{
            //    Debug.Log("나 제대로 닫았다니께");
            //}
        }

        /// <summary>
        /// Called when a Photon Player got connected. We need to then load a bigger scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

            //if (loadAranaTrigger)
            //{
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
                //        loadAranaTrigger = false;
            }
            //}
        }

        /// <summary>
		/// Called when a Photon Player got disconnected. We need to load a smaller scene.
		/// </summary>
		/// <param name="other">Other.</param>
		public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Lobby");
        }


        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }

            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            PhotonNetwork.LoadLevel("Room");
        }


        #endregion


    }
}