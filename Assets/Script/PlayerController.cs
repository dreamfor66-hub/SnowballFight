using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Photon.Pun;
using SBF.Network;
using SBF.UI;
using TMPro;

namespace SBF
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        
        public PhotonView PV;
        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(attackHold);
                stream.SendNext(curHealth);
            }
            else
            {
                // Network player, receive data
                this.attackHold = (bool)stream.ReceiveNext();
                this.curHealth = (float)stream.ReceiveNext();
            }
        }

        #endregion
        [Tooltip("로컬 플레이어 인스턴스 씬에서 로컬플레이어가 뭔지 알기 위해 넣ㅇ므")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("플레이어의 UI 게임오브젝트 프리팹")]
        public GameObject playerUiPrefab;
        public GameObject localUI;

        Vector3 rotOffset_Normal = new Vector3 (90,0,0);
        Vector3 rotOffset_Reverse = new Vector3 (-90, 0, 180);

        [SerializeField]Transform bulletPos;
        Animator animator;
        [SerializeField] public CharacterData charData;
        float maxHealth => charData.MaxHp;
        [HideInInspector] public float curHealth;
        public bool die = false;
        bool dieTrigger = true;
        bool attackTrigger = true;
        bool attackHold;
        bool dashTrigger = true;
        bool isDash = false;

        public bool isChat = false;
        

        [Tooltip("플레이어가 채팅한 걸 전달해줌")]
        public GameObject chatbox;
        public TMP_Text playerChatName;
        public TMP_Text playerChatText;

        public int saved_skinkey;
        public int saved_hairkey;
        public int saved_facehairkey;
        public int saved_clothkey;
        public int saved_pantskey;
        public int saved_backkey;
        public int saved_haircolorkey;
        public int saved_eyescolorkey;

        Vector3 moveDir;
        [SerializeField]  Transform pointArrow;
        [SerializeField] Transform charBody;

        [HideInInspector] public Camera cam;

        // Start is called before the first frame update
        void Awake()
        {
            PV = GetComponent<PhotonView>();
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (PV.IsMine)
            {
                PlayerController.LocalPlayerInstance = this.gameObject;
            }
            else
            {
                pointArrow.gameObject.SetActive(false);
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);

            

            die = false;
            attackTrigger = true;
            dieTrigger = true;
            isDash = false;
            dashTrigger = true;
            curHealth = maxHealth;
            //bulletPos = GetComponentInChildren<BulletPos>().gameObject.transform;
            //cam = Camera.main;
            animator = GetComponentInChildren<Animator>();
            if (!animator)
            {
                Debug.LogError("애니메이터 빠져있음", this);
            }
        }

        void Start()
        {
#if UNITY54ORNEWER
// Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif
            if (playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                //if (PV.IsMine)
                //{
                    localUI = _uiGo;
                    chatbox = localUI.GetComponent<PlayerUI>().chatbox;
                    //playerChatName = localUI.GetComponent<PlayerUI>().playerChatName;
                    //playerChatText = localUI.GetComponent<PlayerUI>().playerChatText;
                //}
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            cam = Camera.main;
            //카메라가 문제가 되는 것 같아 긁어옴
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
            if (_cameraWork != null)
            {
                if (PV.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }
            
        }

#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
            GameObject _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            if (PV.IsMine)
            {
                localUI = _uiGo;
                chatbox = localUI.GetComponent<PlayerUI>().chatbox;
                playerChatName = localUI.GetComponent<PlayerUI>().playerChatName;
                playerChatText = localUI.GetComponent<PlayerUI>().playerChatText;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (PV.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            
            if (cam == null)
            {
                cam = Camera.main;
            }
            else if (cam != null)
            {
                    
            }

            
            if (!die)
            {
                if (PV.IsMine)
                {
                    ArrowRotation();
                    PV.RPC("StatusCheck",RpcTarget.AllBufferedViaServer);
                    if (!isDash && !isChat)
                    {
                        MoveInputCheck();
                        LookRotCheck();
                        DashInputCheck();
                        if (SceneManager.GetActiveScene().name != ("Room"))
                        {
                            AttackInputCheck();
                        }
                    }
                }
            }
            
        }
        void FixedUpdate()
        {
            if (PV.IsMine)
            {
                if (SceneManager.GetActiveScene().name != "Room for 1")
                {
                    if (die)
                    {
                        if (dieTrigger)
                        {
                            PV.RPC("Die", RpcTarget.AllBufferedViaServer);
                            dieTrigger = false;
                        }
                    }
                    if (!isDash && !isChat)
                    {
                        Move();
                    }
                    else if (!isDash && isChat)
                    {
                        animator.ResetTrigger("Move");
                        animator.SetTrigger("Idle");
                    }
                }
            }
        }

        void MoveInputCheck()
        {
            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        void DashInputCheck()
        {
            if (!dashTrigger)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DashCooldown(charData.DashCooldown));
                PV.RPC("Dash", RpcTarget.All);
            }
           
        }

        [PunRPC]
        void Dash()
        {
            animator.SetTrigger("Dash");
            StartCoroutine(Dash(charData.DashDur));
        }

        void AttackInputCheck()
        {
            //if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    Attack();
            //}
            if (attackHold)
            {
                if(attackTrigger)
                {
                    StartCoroutine(AttackCooldown(charData.AttackCooldown));

                    Vector3 point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                    Vector3 rotDir = new Vector3((point - transform.position).x, pointArrow.position.y, (point - transform.position).z);

                    PV.RPC("Attack", RpcTarget.All, rotDir);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                attackHold = true;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                attackHold = false;
            }
        }

        IEnumerator AttackCooldown(float value)
        {
            var time = value;

            while (time > 0)
            {
                attackTrigger = false;
                yield return new WaitForFixedUpdate();

                time -= Time.deltaTime;
            }
            attackTrigger = true;
        }

        IEnumerator DashCooldown(float value)
        {
            var time = value;

            while (time > 0)
            {
                dashTrigger = false;
                yield return new WaitForFixedUpdate();

                time -= Time.deltaTime;
            }
            dashTrigger = true;
        }

        IEnumerator Dash(float value)
        {
            var time = 0f;
            isDash = true;
            while (time < value)
            {
                float normalizedTime = (time / value);
                yield return new WaitForFixedUpdate();

                transform.position += moveDir.normalized * charData.DashSpd * charData.DashMoveCurve.Evaluate(normalizedTime) * Time.deltaTime;

                time += Time.deltaTime;
            }
            isDash = false;
        }

        [PunRPC]
        void Attack(Vector3 rotDir)
        {
            if (cam == null)
            {
                Debug.Log("눈 던지려는 시도는 했는데, cam이 null이라 발사하지 못했음");
            }
            if (cam != null)
            {
                Debug.Log("정상적으로 쏘고 있음");
               

                //GameObject addObject = (GameObject)Instantiate(moveData.VfxPreFabs, playerBody.transform.position // 이 포맷을 따른다
                //GameObject bullet = (GameObject)Instantiate(charData.AttackPrefs, bulletPos.transform.position, Quaternion.Euler(rotDir));
                GameObject bullet = (GameObject)Instantiate(charData.AttackPrefs, transform.position + (rotDir.normalized * 0.1f), Quaternion.Euler(rotDir));
                //bullet.transform.position = bulletPos.transform.position;
                //bullet.transform.position += rotDir * 10f * Time.deltaTime; // 한번만 이동시키는 코드였네;

                bullet.GetComponent<BulletObject>().owner = gameObject;
                bullet.GetComponent<BulletObject>().shootAngle = new Vector3(rotDir.x, 0, rotDir.z).normalized;
            }
        }

        void LookRotCheck()
        {
            //Debug.Log(pointArrow.rotation.eulerAngles.y);
            //if (pointArrow.rotation.eulerAngles.y < 180)
            //{
            //    charBody.rotation = Quaternion.Euler(rotOffset_Reverse);
            //}
            //else if (pointArrow.rotation.eulerAngles.y > 180)
            //{
            //    charBody.rotation = Quaternion.Euler(rotOffset_Normal);
            //}
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                return;
            }
            if (Input.GetAxisRaw("Horizontal") == 1)
            {
                charBody.rotation = Quaternion.Euler(rotOffset_Reverse);
            }
            else
            {
                charBody.rotation = Quaternion.Euler(rotOffset_Normal);
            }
        }

        void Move()
        {

            //rb.velocity = moveDir.normalized * charData.MoveSpd * Time.deltaTime;
            //rb.AddForce(moveDir.normalized * charData.MoveSpd * Time.deltaTime);

            transform.position += moveDir.normalized * charData.MoveSpd * Time.deltaTime;

            if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                animator.ResetTrigger("Move");
                animator.SetTrigger("Idle");
            }
            else
            {
                animator.ResetTrigger("Idle");
                animator.SetTrigger("Move");
            }

        }

        [PunRPC]
        void StatusCheck()
        {
            
            //if (Input.GetKeyDown(KeyCode.Return))
            //{
            //if (isChat)
            //{
            //    isChat = false;
            //}
            //else if (!isChat)
            //{
            //    GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            //    GM.SendMessage("Send");
            //    isChat = true;
            //}

            //}

            if (curHealth > 0)
            {
                return;
            }
            else if (curHealth <= 0)
            {
                die = true;
            }

        }


        [PunRPC]
        void Die()
        {
            animator.SetTrigger("Die");
        }

        void ArrowRotation()
        {
            if (cam != null)
            {
                Vector3 point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                Vector3 rotDir = new Vector3((point - transform.position).x, pointArrow.position.y, (point - transform.position).z);
                pointArrow.forward = rotDir;
                pointArrow.rotation = Quaternion.Euler(0, pointArrow.eulerAngles.y, pointArrow.eulerAngles.z);
            }

            
            //pointArrow.forward = new Vector3(0, rotDir.y, rotDir.z);


        }



        private void OnTriggerEnter(Collider other)
        {
            
            //if (!photonView.IsMine)
            //{
            //    return;
            //}

            if (!other.GetComponentInParent<BulletObject>())
            {
                return;
            }
            var bulletobj = other.GetComponentInParent<BulletObject>();

            if (bulletobj.owner == null)
            {
                return;
            }

            else if (bulletobj.owner == this.transform.root.gameObject)
            {
                return;
            }

            if (bulletobj != null)
            {
                curHealth -= 10;
                Debug.Log(curHealth);
                Destroy(bulletobj.transform.root.gameObject);
            }


            else
            {

            }
            

            //var collTag = other.gameObject.GetComponent<ZST_SmartTile>().tileTag;

            //Debug.Log("Hi " + collTag);
        }

        void ChatRPCReceiver(string msg)
        {
            PV.RPC("ChatboxRPC", RpcTarget.All, msg, PhotonNetwork.NickName);
            
        }

        [PunRPC]
        void ChatboxRPC(string msg, string name)
        {
            StopCoroutine("ChatBoxDuration"); //왜 이름으로 안하고 함수형태로 하면 완전한 재시작이 안되고 중간부터 다시 실행시키는거지?
            localUI.GetComponent<PlayerUI>().namebox.SetActive(true);
            localUI.GetComponent<PlayerUI>().chatbox.SetActive(false);

            StartCoroutine("ChatBoxDuration");
            localUI.GetComponent<PlayerUI>().namebox.SetActive(false);
            localUI.GetComponent<PlayerUI>().chatbox.SetActive(true);
            localUI.GetComponent<PlayerUI>().playerChatText.text = msg;
            localUI.GetComponent<PlayerUI>().playerChatName.text = name;
        }

        IEnumerator ChatBoxDuration()
        {
            var time = 3f;

            while (time > 0)
            {
                yield return new WaitForFixedUpdate();

                time -= Time.deltaTime;
            }
            localUI.GetComponent<PlayerUI>().chatbox.SetActive(false);
            localUI.GetComponent<PlayerUI>().namebox.SetActive(true);
        }
    }
}
