using UnityEngine;
using Photon.Pun;
using SBF.Network;

namespace SBF
{
    //���� �ʿ� ���� �����µ� ī�޶󶧹��� ���� ���ܼ� ���� �ܾ����..
    public class CameraWork : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        [Tooltip("The distance in the local x-z plane to the target")]
        [SerializeField]
        private float distance = 7.0f;

        [Tooltip("The height we want the camera to be above the target")]
        [SerializeField]
        private float height = 3.0f;

        [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool followOnStart = false;

        [Tooltip("The Smoothing for the camera to follow the target")]
        [SerializeField]
        private float smoothSpeed = 0.125f;

        // cached transform of the target
        Transform cameraTransform;

        // maintain a flag internally to reconnect if target is lost or camera is switched
        bool isFollowing;

        // Cache for camera offset
        Vector3 cameraOffset = Vector3.zero;

        #endregion

        #region MonoBehaviour Callbacks

        /// &lt;summary&gt;
        /// MonoBehaviour method called on GameObject by Unity during initialization phase
        /// &lt;/summary&gt;
        void Start()
        {
            if (photonView.IsMine)
            {
                // Start following the target if wanted.
                if (followOnStart)
                {
                    OnStartFollowing();
                }
            }
        }


        void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (photonView.IsMine)
            {
                if (cameraTransform == null)
                {
                    OnStartFollowing();
                }
                // The transform target may not destroy on level load,
                // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
                if (cameraTransform == null && isFollowing)
                {
                    OnStartFollowing();
                }

                // only follow is explicitly declared
                if (isFollowing)
                {
                    Follow();
                }
            }
        }

        #endregion

        #region Public Methods

        /// &lt;summary&gt;
        /// Raises the start following event.
        /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
        /// &lt;/summary&gt;
        public void OnStartFollowing()
        {
            cameraTransform = GetComponent<PlayerController>().cam.transform;
            isFollowing = true;
            // we don't smooth anything, we go straight to the right camera shot
            Cut();
        }

        #endregion

        #region Private Methods

        /// &lt;summary&gt;
        /// Follow the target smoothly
        /// &lt;/summary&gt;
        void Follow()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);

            //cameraTransform.LookAt(this.transform.position + centerOffset);
        }


        void Cut()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

            //cameraTransform.LookAt(this.transform.position + centerOffset);
        }
        #endregion
    }


}