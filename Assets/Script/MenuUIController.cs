using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using SBF.Network ;
using UnityEngine.SceneManagement;

namespace SBF.UI
{
    public class MenuUIController : MonoBehaviour
    {
        public GameObject menuPanel;
        public GameObject endPanel;
        public PlayerController characon;
        public bool endTrigger;

        private bool isMenu = false;

        // Start is called before the first frame update
        void Awake()
        {
            //Cursor.lockState = CursorLockMode.Confined;
            endTrigger = true;
            menuPanel.SetActive(false);
            endPanel.SetActive(false);
            
        }

        private void Update()
        {

            //if (characon.die)
            //{
            //    if (endTrigger)
            //    {
            //        endTrigger = false;
            //        endPanel.SetActive(true);
            //    }
            //}
        }

        // Update is called once per frame
        public void MenuOpen()
        {
            isMenu = true;
            menuPanel.SetActive(true);
        }

        public void GameEnd()
        {
            GameManager.Instance.LeaveRoom();
        }
        public void LeaveRoom()
        {
            GameManager.Instance.LeaveRoom();
        }

        public void MenuOff()
        {
            isMenu = false;
            menuPanel.SetActive(false);

        }

    }
}