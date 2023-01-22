using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Photon.Pun;
using SBF.Network;
using SBF.UI;

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
        [Tooltip("���� �÷��̾� �ν��Ͻ� ������ �����÷��̾ ���� �˱� ���� �֤���")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("�÷��̾��� UI ���ӿ�����Ʈ ������")]
        public GameObject playerUiPrefab;

        Vector3 rotOffset_Normal = new Vector3 (90,0,0);
        Vector3 rotOffset_Reverse = new Vector3 (-90, 0, 180);

        Transform bulletPos;
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
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);

            die = false;
            attackTrigger = true;
            dieTrigger = true;
            isDash = false;
            dashTrigger = true;
            curHealth = maxHealth;
            bulletPos = GetComponentInChildren<BulletPos>().gameObject.transform;
            //cam = Camera.main;
            animator = GetComponentInChildren<Animator>();
            if (!animator)
            {
                Debug.LogError("�ִϸ����� ��������", this);
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
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            cam = Camera.main;
            //ī�޶� ������ �Ǵ� �� ���� �ܾ��
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
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
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

            if (die)
            {
                if (dieTrigger)
                {
                    Die();
                    dieTrigger = false;
                }
            }
            else
            {
                if (PV.IsMine)
                {
                    ArrowRotation();
                    StatusCheck();
                    if (!isDash)
                    {
                        LookRotCheck();
                        MoveInputCheck();
                        AttackInputCheck();
                        DashInputCheck();
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
                    if (!isDash)
                    {
                        Move();
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
                    PV.RPC("Attack", RpcTarget.All);
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
        void Attack()
        {
            if (cam == null)
            {
                Debug.Log("�� �������� �õ��� �ߴµ�, cam�� null�̶� �߻����� ������");
            }
            if (cam != null)
            {
                Debug.Log("���������� ��� ����");
                Vector3 point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                Vector3 rotDir = new Vector3((point - transform.position).x, pointArrow.position.y, (point - transform.position).z);

                //GameObject addObject = (GameObject)Instantiate(moveData.VfxPreFabs, playerBody.transform.position // �� ������ ������
                GameObject bullet = (GameObject)Instantiate(charData.AttackPrefs, bulletPos.transform.position, Quaternion.Euler(rotDir));
                //bullet.transform.position = bulletPos.transform.position;
                //bullet.transform.position += rotDir * 10f * Time.deltaTime; // �ѹ��� �̵���Ű�� �ڵ忴��;

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

        void StatusCheck()
        {
            if (curHealth > 0)
            {
                return;
            }
            else if (curHealth <= 0)
            {
                die = true;
            }

        }

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

    }
}
