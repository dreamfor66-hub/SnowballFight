using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBF
{
    public class PlayerSkinReceiver : MonoBehaviour
    {
        public int cur_hairkey;
        public int cur_facehairkey;
        public int cur_clothkey;
        public int cur_pantskey;
        public int cur_backkey;

        SpriteRenderer cur_hair;
        SpriteRenderer cur_faceHair;
        SpriteRenderer cur_clothBody;
        SpriteRenderer cur_armLeft;
        SpriteRenderer cur_armRight;
        SpriteRenderer cur_pantsLeft;
        SpriteRenderer cur_pantsRight;
        SpriteRenderer cur_back;
        // Start is called before the first frame update
        void Start()
        {
            //PlayerSkinChanger.SkinCheck();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
