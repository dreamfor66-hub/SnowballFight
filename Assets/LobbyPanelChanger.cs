using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBF.UI
{
    public class LobbyPanelChanger : MonoBehaviour
    {
        public GameObject homePanel;
        public GameObject customizePanel;

        public ToggleGroup panelGroup;
        public Toggle homeToggle;
        public Toggle customizeToggle;

        //Animator anim;
        // Start is called before the first frame update
        void Start()
        {
            homePanel.SetActive(true);
            customizePanel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (homePanel.activeSelf == true)
            {
                homeToggle.isOn = true;
            }
            else if (customizePanel.activeSelf == true)
            {
                customizeToggle.isOn = true;
            }
        }

        public void PanelChange(int cur_Toggle)
        {
            if (cur_Toggle == 0)
            {
                homePanel.SetActive(true);
                customizePanel.SetActive(false);
            }
            else if (cur_Toggle == 1)
            {
                homePanel.SetActive(false);
                customizePanel.SetActive(true);
            }
        }
    }
}