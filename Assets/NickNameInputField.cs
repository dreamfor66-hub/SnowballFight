using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


namespace SBF.Network
{
    [RequireComponent(typeof(TMP_InputField))]
    public class NickNameInputField : MonoBehaviour
    {
        #region Private Constants


        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";


        #endregion


        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Awake()
        {

            /// &lt;summary&gt;
            /// MonoBehaviour method called on GameObject by Unity during initialization phase.
            /// &lt;/summary&gt;
            string defaultName = "asdfgh";
            TMP_InputField _inputField = this.GetComponent<TMP_InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                    //_inputField.text = defaultName;
                }
            }


            PhotonNetwork.NickName = defaultName;

        }


        #endregion


        #region Public Methods


        /// &lt;summary&gt;
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// &lt;/summary&gt;
        /// &lt;param name="value"&gt;The name of the Player&lt;/param&gt;
        public void SetPlayerName()
        {
            TMP_InputField inputField = GetComponent<TMP_InputField>();
            // #Important
            string value = inputField.text;
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
            Debug.Log(PhotonNetwork.NickName);
        }


        #endregion
    }
}
