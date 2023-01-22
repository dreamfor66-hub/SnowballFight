using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

using Photon.Pun;
using SBF.Network;

namespace SBF
{
    public class PlayerSkinChanger : MonoBehaviourPunCallbacks
    {
        //������ ����ϴ� ����Ʈ ������. �Ʒ��� "������ ����ϴ� ��ü���� ��ġ"�� �̰ɷ� ��ġ�ؿ´�.
        SPUM_SpriteList spumList;

        //��� ��������Ʈ���� ����Ʈ. �̰� Resourse ���Ϸ� ��ü�Ϸ���?
        public List<Sprite> usableSkinHeadList = new List<Sprite>();
        public List<Sprite> usableSkinBodyList = new List<Sprite>();
        public List<Sprite> usableSkinArmLeftList = new List<Sprite>();
        public List<Sprite> usableSkinArmRightList = new List<Sprite>();
        public List<Sprite> usableSkinFootLeftList = new List<Sprite>();
        public List<Sprite> usableSkinFootRightList = new List<Sprite>();

        public List<Sprite> usableHairList = new List<Sprite>();
        public List<Sprite> usableFaceHairList = new List<Sprite>();
        public List<Sprite> usableClothBodyList = new List<Sprite>();
        public List<Sprite> usableArmLeftList = new List<Sprite>();
        public List<Sprite> usableArmRightList = new List<Sprite>();
        public List<Sprite> usablePantsLeftList = new List<Sprite>();
        public List<Sprite> usablePantsRightList = new List<Sprite>();
        public List<Sprite> usableBackList = new List<Sprite>();

        public List<Color> usableColorList = new List<Color>();

        public List<string> hairTextList = new List<string>();
        public List<string> faceHairTextList = new List<string>();
        public List<string> skinTextList = new List<string>();
        public List<string> clothTextList = new List<string>();
        public List<string> pantsTextList = new List<string>();
        public List<string> backTextList = new List<string>();


        //������ ���� �ִ� ��ü���� ��ġ

        //public int cur_hairkey;
        //public int cur_facehairkey;
        //public int cur_clothkey;
        //public int cur_pantskey;
        //public int cur_backkey;

        SpriteRenderer cur_skinHead;
        SpriteRenderer cur_skinBody;
        SpriteRenderer cur_skinArmLeft;
        SpriteRenderer cur_skinArmRight;
        SpriteRenderer cur_skinFootLeft;
        SpriteRenderer cur_skinFootRight;

        SpriteRenderer cur_hair;
        SpriteRenderer cur_faceHair;
        SpriteRenderer cur_clothBody;
        SpriteRenderer cur_armLeft;
        SpriteRenderer cur_armRight;
        SpriteRenderer cur_pantsLeft;
        SpriteRenderer cur_pantsRight;
        SpriteRenderer cur_back;
        
        SpriteRenderer cur_eyesLeft;
        SpriteRenderer cur_eyesRight;

        bool skinTrigger = true;
        NetworkManager NM;
        public PlayerController PC;
        public GameManager GM;

        //����� ��������Ʈ���� ���� List �ȿ� ������ ���缭 ����ִ´�.
        //enum�� ���� 0~10������ ������ �����Ѵ�.
        //enum ����Ʈ�� �����ϸ� = ���� �������� �ٲ��,
        // Update���� ���� �������� ���� ��������Ʈ�� ��ü�ǵ��� �Ѵ�.

        //������ ������ save�ϸ� photon���� ���� ������ ������.

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                NM = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
                ReferCheck();
                SkinCheck();
            }
            if (SceneManager.GetActiveScene().name != "Lobby")
            {
                ReferCheck();
                PC = transform.root.GetComponent<PlayerController>();
                GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //cur_hair = usableHairList.FindIndex(0,1);
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                SkinCheck();
            }

            
            
        }

        private void FixedUpdate()
        {
            if (SceneManager.GetActiveScene().name != "Lobby" && PhotonNetwork.LocalPlayer.CustomProperties["skinkey"] != null && skinTrigger)
            {
                SkinChange();
            }
        }

        void ReferCheck()
        {
            spumList = GetComponentInChildren<SPUM_SpriteList>();
            cur_skinHead = GetComponentInChildren<CustomizePartSkinHead>().gameObject.GetComponent<SpriteRenderer>();
            cur_skinBody = GetComponentInChildren<CustomizePartSkinBody>().gameObject.GetComponent<SpriteRenderer>();
            cur_skinArmLeft = GetComponentInChildren<CustomizePartSkinArmLeft>().gameObject.GetComponent<SpriteRenderer>();
            cur_skinArmRight = GetComponentInChildren<CustomizePartSkinArmRight>().gameObject.GetComponent<SpriteRenderer>();
            cur_skinFootLeft = GetComponentInChildren<CustomizePartSkinFootLeft>().gameObject.GetComponent<SpriteRenderer>();
            cur_skinFootRight = GetComponentInChildren<CustomizePartSkinFootRight>().gameObject.GetComponent<SpriteRenderer>();

            cur_hair = GetComponentInChildren<CustomizePartHair>().gameObject.GetComponent<SpriteRenderer>();
            cur_faceHair = GetComponentInChildren<CustomizePartFaceHair>().gameObject.GetComponent<SpriteRenderer>();
            cur_clothBody = GetComponentInChildren<CustomizePartClothBody>().gameObject.GetComponent<SpriteRenderer>();
            cur_armLeft = GetComponentInChildren<CustomizePartArmLeft>().gameObject.GetComponent<SpriteRenderer>();
            cur_armRight = GetComponentInChildren<CustomizePartArmRight>().gameObject.GetComponent<SpriteRenderer>();
            cur_pantsLeft = GetComponentInChildren<CustomizePartPantsLeft>().gameObject.GetComponent<SpriteRenderer>();
            cur_pantsRight = GetComponentInChildren<CustomizePartPantsRight>().gameObject.GetComponent<SpriteRenderer>();
            cur_back = GetComponentInChildren<CustomizePartBack>().gameObject.GetComponent<SpriteRenderer>();

            cur_eyesLeft = GetComponentInChildren<CustomizePartEyesLeft>().gameObject.GetComponent<SpriteRenderer>();
            cur_eyesRight = GetComponentInChildren<CustomizePartEyesRight>().gameObject.GetComponent<SpriteRenderer>();

        }


        public void SkinCheck()
        {
            if (NM != null)
            {
                cur_skinHead.sprite = usableSkinHeadList[NM.cur_skinkey];
                cur_skinBody.sprite = usableSkinBodyList[NM.cur_skinkey];
                cur_skinArmLeft.sprite = usableSkinArmLeftList[NM.cur_skinkey];
                cur_skinArmRight.sprite = usableSkinArmRightList[NM.cur_skinkey];
                cur_skinFootLeft.sprite = usableSkinFootLeftList[NM.cur_skinkey];
                cur_skinFootRight.sprite = usableSkinFootRightList[NM.cur_skinkey];

                cur_hair.sprite = usableHairList[NM.cur_hairkey];
                cur_faceHair.sprite = usableFaceHairList[NM.cur_facehairkey];
                cur_clothBody.sprite = usableClothBodyList[NM.cur_clothkey];
                cur_armLeft.sprite = usableArmLeftList[NM.cur_clothkey];
                cur_armRight.sprite = usableArmRightList[NM.cur_clothkey];
                cur_pantsLeft.sprite = usablePantsLeftList[NM.cur_pantskey];
                cur_pantsRight.sprite = usablePantsRightList[NM.cur_pantskey];
                cur_back.sprite = usableBackList[NM.cur_backkey];

                cur_hair.color = usableColorList[NM.cur_haircolorkey];
                cur_faceHair.color = usableColorList[NM.cur_haircolorkey];
                cur_eyesLeft.color = usableColorList[NM.cur_eyescolorkey];
                cur_eyesRight.color = usableColorList[NM.cur_eyescolorkey];
            }

            PlayerPrefs.SetInt("saved_skinkey", NM.cur_skinkey);
            PlayerPrefs.SetInt("saved_hairkey", NM.cur_hairkey);
            PlayerPrefs.SetInt("saved_facehairkey", NM.cur_facehairkey);
            PlayerPrefs.SetInt("saved_clothkey", NM.cur_clothkey);
            PlayerPrefs.SetInt("saved_pantskey", NM.cur_pantskey);
            PlayerPrefs.SetInt("saved_backkey", NM.cur_backkey);
            PlayerPrefs.SetInt("saved_haircolorkey", NM.cur_haircolorkey);
            PlayerPrefs.SetInt("saved_eyescolorkey", NM.cur_eyescolorkey);

            //Debug.Log(PlayerPrefs.GetInt("saved_hairkey"));


            //cur_hair = usableHairList2.;
        }
        public void SkinChange()
        {
            var CP = PhotonNetwork.LocalPlayer.CustomProperties;
            //if (PC.saved_skinkey != null)
            //{

            //Debug.Log((int)GM.playerCP["skinkey"]);

            cur_skinHead.sprite = usableSkinHeadList[(int)CP["skinkey"]];
            cur_skinBody.sprite = usableSkinBodyList[(int)CP["skinkey"]];
            cur_skinArmLeft.sprite = usableSkinArmLeftList[(int)CP["skinkey"]];
            cur_skinArmRight.sprite = usableSkinArmRightList[(int)CP["skinkey"]];
            cur_skinFootLeft.sprite = usableSkinFootLeftList[(int)CP["skinkey"]];
            cur_skinFootRight.sprite = usableSkinFootRightList[(int)CP["skinkey"]];

            cur_hair.sprite = usableHairList[(int)CP["hairkey"]];
            cur_faceHair.sprite = usableFaceHairList[(int)CP["facehairkey"]];
            cur_clothBody.sprite = usableClothBodyList[(int)CP["clothkey"]];
            cur_armLeft.sprite = usableArmLeftList[(int)CP["clothkey"]];
            cur_armRight.sprite = usableArmRightList[(int)CP["clothkey"]];
            cur_pantsLeft.sprite = usablePantsLeftList[(int)CP["pantskey"]];
            cur_pantsRight.sprite = usablePantsRightList[(int)CP["pantskey"]];
            cur_back.sprite = usableBackList[(int)CP["backkey"]];

            cur_hair.color = usableColorList[(int)CP["haircolorkey"]];
            cur_faceHair.color = usableColorList[(int)CP["haircolorkey"]];
            cur_eyesLeft.color = usableColorList[(int)CP["eyescolorkey"]];
            cur_eyesRight.color = usableColorList[(int)CP["eyescolorkey"]];

            //}
            skinTrigger = false;
        }
    }


    
}