using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SBF
{
    public class ItemObject : MonoBehaviourPunCallbacks
    {
        //[HideInInspector] public GameObject owner;
        //float bulletDur;
        //float bulletSpd;
        //List<Collider> HC = new List<Collider>();
        //[HideInInspector] public Vector3 shootAngle;

        public ItemData data;

        private void Awake()
        {
            var colls = GetComponentsInChildren<HitCollider>();
            
        }

        private void OnTriggerEnter(Collider other)
        {

        }
        
    }

}