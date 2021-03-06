// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;

namespace MagicLeap
{
    /// <summary>
    /// This provides an example of interacting with the image tracker visualizers using the controller
    /// </summary>
    [RequireComponent(typeof(PrivilegeRequester))]
    public class ImageTrackingExample : MonoBehaviour
    {
        #region Public Enum
        public enum ViewMode : int
        {
            All = 0,
            AxisOnly,
            TrackingCubeOnly,
            DemoOnly,
        }

        public GameObject[] TrackerBehaviours;
        #endregion

        #region Private Variables
        private ViewMode _viewMode = ViewMode.All;

        private Camera mainCamera;
        private Transform robotTransform;

        private MLHandKeyPose[] gestures; // Holds the different hand poses we will look for

        private bool _changeHeight = false;

        private float _moveSpeed = .02f;

        [SerializeField, Tooltip("Image Tracking Visualizers to control")]
        private ImageTrackingVisualizer [] _visualizers = null;

        [SerializeField, Tooltip("The View Mode text.")]
        private Text _viewModeLabel = null;

        [SerializeField, Tooltip("The Tracker Status text.")]
        private Text _trackerStatusLabel = null;

        [SerializeField, Tooltip("Relative x position of the main camera as compared to the fiducial.")]
        private Text _positionXText = null;

        [SerializeField, Tooltip("Relative y position of the main camera as compared to the fiducial.")]
        private Text _positionYText = null;

        [SerializeField, Tooltip("Relative z position of the main camera as compared to the fiducial.")]
        private Text _positionZText = null;

        //Rotation about the x-axis
        [SerializeField, Tooltip("Rotation about the x-axis of the fiducial.")]
        private Text _rotationXText = null;


        [SerializeField, Tooltip("Rotation about the y-axis of the fiducial.")]
        private Text _rotationYText = null;

        [SerializeField, Tooltip("Rotation about the z-axis of the fiducial.")]
        private Text _rotationZText = null;

        [Space, SerializeField, Tooltip("ControllerConnectionHandler reference.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        private PrivilegeRequester _privilegeRequester = null;

        private GameObject trackingCube;
        private GameObject trackedItem;

        private float trackedItemXOffset = (float) .001 * 20;//x position
        private float trackedItemYOffset = 0;//y position
        private float trackedItemZOffset = (float) -.005 * 20;//z position

        private float trackedItemScale = .05f;//scale
        private float smallModelTrackedItemScale = .05f;//The scale for a miniature car model
        private float fullsizeTrackedItemScale = 1.0f;//The scale for a full-size car model

        private bool _hasStarted = false;
        #endregion

        #region Unity Methods

        // Using Awake so that Privileges is set before PrivilegeRequester Start
        void Awake()
        {
            if (_controllerConnectionHandler == null)
            {
                Debug.LogError("Error: ImageTrackingExample._controllerConnectionHandler is not set, disabling script.");
                enabled = false;
                return;
            }

            robotTransform = new GameObject().transform;

            // If not listed here, the PrivilegeRequester assumes the request for
            // the privileges needed, CameraCapture in this case, are in the editor.
            _privilegeRequester = GetComponent<PrivilegeRequester>();

            // Before enabling the MLImageTrackerBehavior GameObjects, the scene must wait until the privilege has been granted.
            _privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;

            mainCamera = Camera.main;

            robotTransform.position = Vector3.zero;
            robotTransform.localEulerAngles = Vector3.zero;

            //find gameObject tracking cube.
            trackingCube = GameObject.Find("Tracking_Cube");

            trackedItem = GameObject.Find("trackedItem");
            //trackedItem.transform.localScale = trackedItem.transform.localScale * trackedItemScale;
            trackedItem.SetActive(false);

            //Find and make inactive deep space exploration.
            GameObject space = GameObject.Find("DeepSpaceExploration");
            space.SetActive(false);

            MLHands.Start(); // Start the hand tracking.

            gestures = new MLHandKeyPose[4]; //Assign the gestures we will look for.
            gestures[0] = MLHandKeyPose.Ok;
            gestures[1] = MLHandKeyPose.Fist;
            gestures[2] = MLHandKeyPose.OpenHand;
            gestures[3] = MLHandKeyPose.Finger;
            // Enable the hand poses.
            MLHands.KeyPoseManager.EnableKeyPoses(gestures, true, false);
        }


        private void Update()
        {
            if(trackingCube.activeSelf)
            {
                Vector3 posVector = mainCamera.transform.position - trackingCube.transform.position + robotTransform.position;
                float xRot = mainCamera.transform.rotation.eulerAngles.x - trackingCube.transform.rotation.eulerAngles.x - robotTransform.rotation.eulerAngles.x;
                float yRot = mainCamera.transform.rotation.eulerAngles.y - trackingCube.transform.rotation.eulerAngles.y - robotTransform.rotation.eulerAngles.y;
                float zRot = mainCamera.transform.rotation.eulerAngles.z - trackingCube.transform.rotation.eulerAngles.z - robotTransform.rotation.eulerAngles.z;

                _positionXText.text = "X: " + posVector.x.ToString();
                _positionYText.text = "Y: " + posVector.y.ToString();
                _positionZText.text = "Z: " + posVector.z.ToString();

                _rotationXText.text = "Rotation(X): " + xRot.ToString();
                _rotationYText.text = "Rotation(Y): " + yRot.ToString();
                _rotationZText.text = "Rotation(Z): " + zRot.ToString();
            }
            else
            {
                _positionXText.text = null;
                _positionYText.text = null;
                _positionZText.text = null;

                _rotationXText.text = null;
                _rotationYText.text = null;
                _rotationZText.text = null;

            }

            if (_changeHeight)
            {
                if (_controllerConnectionHandler.ConnectedController.Touch1PosAndForce.z > 0.0f)
                {
                    float Y = _controllerConnectionHandler.ConnectedController.Touch1PosAndForce.y;

                    Vector3 upward = Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, Vector3.forward));

                    Vector3 force = new Vector3(0, 1, 0);

                    if(Y == 0)
                    {
                        force = force * 0;
                    }
                    if (Y < 0)
                    {
                        force = force * -1;
                    }

                    trackedItem.transform.position += force * Time.deltaTime * _moveSpeed;

                    //space.transform.localScale = 10 * space.transform.localScale;
                    //_trackerStatusLabel.text = "Height change is on!";
                    UpdateVisualizers();
                }
            }

        }
        /// <summary>
        /// Unregister callbacks and stop input API.
        /// </summary>
        void OnDestroy()
        {
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            if (_privilegeRequester != null)
            {
                _privilegeRequester.OnPrivilegesDone -= HandlePrivilegesDone;
            }
        }

        bool GetGesture(MLHand hand, MLHandKeyPose type)
        {
            if (hand != null)
            {
                if (hand.KeyPose == type)
                {
                    if (hand.KeyPoseConfidence > 0.9f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Cannot make the assumption that a privilege is still granted after
        /// returning from pause. Return the application to the state where it
        /// requests privileges needed and clear out the list of already granted
        /// privileges. Also, unregister callbacks.
        /// </summary>
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                MLInput.OnControllerButtonDown -= HandleOnButtonDown;
                MLInput.OnTriggerDown -= HandleOnTriggerDown;

                UpdateImageTrackerBehaviours(false);

                _hasStarted = false;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Enable/Disable the correct objects depending on view options
        /// </summary>
        void UpdateVisualizers()
        {
            foreach (ImageTrackingVisualizer visualizer in _visualizers)
            {
                visualizer.UpdateViewMode(_viewMode);
            }
        }

        /// <summary>
        /// Control when to enable to image trackers based on
        /// if the correct privileges are given.
        /// </summary>
        void UpdateImageTrackerBehaviours(bool enabled)
        {
            foreach (GameObject obj in TrackerBehaviours)
            {
                obj.SetActive(enabled);
            }
        }

        /// <summary>
        /// Once privileges have been granted, enable the camera and callbacks.
        /// </summary>
        void StartCapture()
        {
            if (!_hasStarted)
            {
                UpdateImageTrackerBehaviours(true);

                if (_visualizers.Length < 1)
                {
                    Debug.LogError("Error: ImageTrackingExample._visualizers is not set, disabling script.");
                    enabled = false;
                    return;
                }
                if (_viewModeLabel == null)
                {
                    Debug.LogError("Error: ImageTrackingExample._viewModeLabel is not set, disabling script.");
                    enabled = false;
                    return;
                }
                if (_trackerStatusLabel == null)
                {
                    Debug.LogError("Error: ImageTrackingExample._trackerStatusLabel is not set, disabling script.");
                    enabled = false;
                    return;
                }

                MLInput.OnControllerButtonDown += HandleOnButtonDown;
                MLInput.OnTriggerDown += HandleOnTriggerDown;

                _hasStarted = true;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Responds to privilege requester result.
        /// </summary>
        /// <param name="result"/>
        void HandlePrivilegesDone(MLResult result)
        {
            if (!result.IsOk)
            {
                if (result.Code == MLResultCode.PrivilegeDenied)
                {
                    Instantiate(Resources.Load("PrivilegeDeniedError"));
                }

                Debug.LogErrorFormat("Error: ImageTrackingExample failed to get requested privileges, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            Debug.Log("Succeeded in requesting all privileges");
            StartCapture();
        }

        /// <summary>
        /// Handles the event for trigger down.
        /// </summary>
        /// <param name="controllerId">The id of the controller.</param>
        /// <param name="triggerValue">The value of the trigger.</param>
        private void HandleOnTriggerDown(byte controllerId, float triggerValue)
        {
            if (_hasStarted && MLImageTracker.IsStarted && _controllerConnectionHandler.IsControllerValid(controllerId))
            {
                if (MLImageTracker.GetTrackerStatus())
                {
                    MLImageTracker.Disable();
                    trackedItem.SetActive(true);
                    trackedItem.transform.position = trackingCube.transform.position - new Vector3(trackedItemXOffset*trackedItemScale, trackedItemYOffset * trackedItemScale, trackedItemZOffset * trackedItemScale);
                    _trackerStatusLabel.text = "Tracker Status: Disabled";
                    //_trackerStatusLabel.text = "" + _changeHeight;
                }
                else
                {
                    MLImageTracker.Enable();
                    _trackerStatusLabel.text = "Tracker Status: Enabled";
                    //_trackerStatusLabel.text = "" + _changeHeight;
                }
            }
        }

        /// <summary>
        /// Handles the event for button down.
        /// </summary>
        /// <param name="controllerId">The id of the controller.</param>
        /// <param name="button">The button that is being released.</param>
        private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
        {
            /*if (_controllerConnectionHandler.IsControllerValid(controllerId) && button == MLInputControllerButton.Bumper)
            {
                _viewMode = (ViewMode)((int)(_viewMode + 1) % Enum.GetNames(typeof(ViewMode)).Length);
                _viewModeLabel.text = string.Format("View Mode: {0}", _viewMode.ToString());
            }
            UpdateVisualizers();*/
            if (_controllerConnectionHandler.IsControllerValid(controllerId) && button == MLInputControllerButton.Bumper)
            {
                _changeHeight = !_changeHeight;
            }

            else if(_controllerConnectionHandler.IsControllerValid(controllerId) && button == MLInputControllerButton.HomeTap)
            {
                if(trackedItemScale < .5)
                {
                    trackedItem.transform.localScale = trackedItem.transform.localScale * (1/trackedItemScale);
                    trackedItemScale = fullsizeTrackedItemScale;
                }

                else if (trackedItemScale > .5)
                {
                    trackedItem.transform.localScale = trackedItem.transform.localScale * (smallModelTrackedItemScale);
                    trackedItemScale = smallModelTrackedItemScale;
                }
            }
            UpdateVisualizers();
        }
        #endregion
    }
}
