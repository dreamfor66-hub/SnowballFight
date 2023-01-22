using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;


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

        public int local_skinkey;
        public int local_hairkey;
        public int local_facehairkey;
        public int local_clothkey;
        public int local_pantskey;
        public int local_backkey;
        public int local_haircolorkey;
        public int local_eyescolorkey;

        public TMP_Text listText;
        public TMP_Text roomNameText;
        public TMP_Text roomPlayerCountText;
        public TMP_Text[] chatText;
        public TMP_InputField chatInput;

        [SerializeField] PhotonView PV;
        public Hashtable playerCP;

        public NetworkManager NM;

        private void Awake()
        {
            //KeySetUp();
            //instantiateTrigger = true;
            //loadAranaTrigger = true;
            //NM = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            //KeySetUp();

        }

        void Start()
        {
            Instance = this;
            

            
            //NetworkManager.RoomRenewal();
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
                    Debug.Log("인스턴스 트리거 밝혔거든");
                    //instantiateTrigger = false;

                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                    //instantiateTrigger = false;
                }
            }

            RoomRenewal();

            

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

            RoomRenewal();
            ChatRPC(other.NickName + "님이 참가하셨습니다.");
            Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting
            //if (loadAranaTrigger)
            //{
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                //LoadArena();
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
            RoomRenewal();
            
            ChatRPC(other.NickName + "님이 퇴장했습니다");

            Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                //LoadArena();
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

        public void RoomRenewal()
        {
            //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            //{
            //    listText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
            //}
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            roomPlayerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        }

        public void Send()
        {
            PV.RPC("ChatRPC", RpcTarget.All, "<" + PhotonNetwork.NickName + ">  " + chatInput.text);
            chatInput.text = "";
        }

        [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
        void ChatRPC(string msg)
        {
            bool isInput = false;
            for (int i = 0; i < chatText.Length; i++)
                if (chatText[i].text == "")
                {
                    isInput = true;
                    chatText[i].text = msg;
                    break;
                }
            if (!isInput) // 꽉차면 한칸씩 위로 올림
            {
                for (int i = 1; i < chatText.Length; i++) chatText[i - 1].text = chatText[i].text;
                chatText[chatText.Length - 1].text = msg;
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void LoadArena()
        {
            //앞으로 LoadArena는 "게임시작"과 같은 기능을 하도록 한다.
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }

            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            PhotonNetwork.LoadLevel("Room");
        }

        //void KeySetUp()
        //{
        //    local_skinkey = NM.cur_skinkey;
        //    local_hairkey = NM.cur_hairkey;
        //    local_facehairkey = NM.cur_facehairkey;
        //    local_clothkey = NM.cur_clothkey;
        //    local_pantskey = NM.cur_pantskey;
        //    local_backkey = NM.cur_backkey;
        //    local_haircolorkey = NM.cur_haircolorkey;
        //    local_eyescolorkey = NM.cur_eyescolorkey;
        //}

        #endregion


    }
}