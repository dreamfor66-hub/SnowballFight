using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SBF.Network;

namespace SBF.UI
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("플레이어 타겟에서 얼마나 다른지 오프셋")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        [Tooltip("플레이어 이름이 적힐 UI 텍스트")]
        [SerializeField]
        private TMP_Text playerNameText;


        [Tooltip("플레이어 남은 체력을 UI에 전송")]
        [SerializeField]
        private Slider playerHealthSlider;


        Transform targetTransform;
        Vector3 targetPosition;

        PlayerController target;

        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }

        void Update()
        {
            if (target == null)
            {
                Debug.Log("타겟 널임");
                //Destroy(this.gameObject);
                //return;
            }
            // Reflect the Player Health
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.curHealth;
            }
        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerController _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a><Color> PlayerMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            target = _target;
            
            PlayerController _characterController = _target.GetComponent<PlayerController>();
            // Get data from the Player that won't change during the lifetime of this Component

            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
            else
            {
                playerNameText.text = "이름없음";
            }
        }

        void FixedUpdate()
        {
            //Debug.Log(target);
            // #Critical
            // Follow the Target GameObject on screen.
            if (target != null)
            {
                //targetPosition = targetTransform.position;
                //targetPosition.y += characterControllerHeight;
                transform.position = Camera.main.WorldToScreenPoint(target.gameObject.transform.position) + screenOffset;
            }
        }
        #endregion

    }
}