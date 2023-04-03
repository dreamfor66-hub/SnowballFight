using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SBF
{
    public class BulletObject : MonoBehaviourPunCallbacks
    {
        [HideInInspector] public GameObject owner;
        float bulletDur;
        public float bulletDmg;
        public ItemType bulletType;
        float bulletSpd;
        List<Collider> HC = new List<Collider>();
        [HideInInspector] public Vector3 shootAngle;

        private void Awake()
        {
            var colls = GetComponentsInChildren<HitCollider>();
            
            foreach (HitCollider hcs in colls)
            {
                HC.Add(hcs.gameObject.GetComponent<Collider>());
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            bulletDur = owner.GetComponent<PlayerController>().charData.AttackDur;
            bulletSpd = owner.GetComponent<PlayerController>().charData.AttackSpd;
            var abc = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            StartCoroutine(BulletMove(bulletDur,bulletSpd, shootAngle));
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ground")
            {
                return;
            }
            Debug.Log("³ª ¿©ƒ…¾î");
            //var characon = other.GetComponent<PlayerController>();
            //if (other.gameObject == owner)
            //{
            //    return;
            //}

            //if (characon != null)
            //{
            //    characon.curHealth -= 10;
            //    Debug.Log(characon.curHealth);
            //    Destroy(this.gameObject);
            //}

            if (other.tag == "Wall")
            {
                Destroy(this.gameObject);
            }
        }

        IEnumerator BulletMove(float value, float spd, Vector3 destination)
        {
            var time = value;

            while (time > 0)
            {
                yield return new WaitForFixedUpdate();

                transform.position += destination * spd * Time.deltaTime;

                time -= Time.deltaTime;
            }
            Destroy(this.gameObject);
        }
        
    }

}