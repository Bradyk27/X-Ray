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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;
using Mirror;

namespace MagicLeap
{
    /// <summary>
    /// This class handles the functionality of updating the bounding box
    /// for the planes query params through input. This class also updates
    /// the UI text containing the latest useful info on the planes queries.
    /// </summary>
    [RequireComponent(typeof(Planes))]
    public class PlanesExampleNetworked : MonoBehaviour
    {
        protected bool _triggerPressed = false;
        #region Private Variables

        [SerializeField, Tooltip("Whether the screen reappears after each trial.")]
        bool displayScreenAfterEachTest = false;

        [Space, SerializeField, Tooltip("Flag specifying if plane extents are bounded.")]
        private bool _bounded = false;

        [SerializeField, Space, Tooltip("Wireframe cube to represent bounds.")]
        private GameObject _boundsWireframeCube = null;

        [SerializeField, Space, Tooltip("Text to display number of planes.")]
        private Text _numberOfPlanesText = null;

        [SerializeField, Tooltip("Text to display number of boundaries.")]
        private Text _numberOfBoundariesText = null;

        [SerializeField, Tooltip("Text to display if planes extents are bounded or boundless.")]
        private Text _boundedExtentsText = null;

        [Space, SerializeField, Tooltip("ControllerConnectionHandler reference.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        [Space, SerializeField, Tooltip("ControllerConnectionHandler reference.")]
        private NetworkManager _networkMan = null;

        [Space, SerializeField, Tooltip("RaycastVisualizer reference.")]
        RaycastVisualizer selected = null;

        [Space, SerializeField, Tooltip("CombinedImageTrackingExample reference.")]
        CombinedImageTrackingExample comb = null;

        [Space, SerializeField, Tooltip("Planes Script.")]
        Planes planesScript = null;

        MeshRenderer planeVisual = null;

        private MLInputController controller;

        private Planes _planesComponent;

        private bool testingPhase = false;
        private bool exploratoryPhase = false;
        private bool calibrationPhase = false;
        private bool ultimatePhase = false;

        private bool manualControl = true;

        private GameObject screen;
        private PlayerObject pl;
        private uint standardMaximumPlaneCount;

        private static readonly Vector3 _boundedExtentsSize = new Vector3(5.0f, 5.0f, 5.0f);
        // Distance close to sensor's maximum recognition distance.
        private static readonly Vector3 _boundlessExtentsSize = new Vector3(10.0f, 10.0f, 10.0f);

        private Camera _camera;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Check editor set variables for null references.
        /// </summary>
        void Awake()
        {
            if (_numberOfPlanesText == null)
            {
                Debug.LogError("Error: PlanesExample._numberOfPlanesText is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_numberOfBoundariesText == null)
            {
                Debug.LogError("Error: PlanesExample._numberOfBoundariesText is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_boundedExtentsText == null)
            {
                Debug.LogError("Error: PlanesExample._boundedExtentsText is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_boundsWireframeCube == null)
            {
                Debug.LogError("Error: PlanesExample._boundsWireframeCube is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_controllerConnectionHandler == null)
            {
                Debug.LogError("Error: PlanesExample._controllerConnectionHandler is not set, disabling script.");
                enabled = false;
                return;
            }

            MLInput.OnControllerButtonDown += OnButtonDown;
            MLInput.OnTriggerDown += OnTriggerDown;
            MagicLeapDevice.RegisterOnHeadTrackingMapEvent(OnHeadTrackingMapEvent);

            _planesComponent = GetComponent<Planes>();
            _camera = Camera.main;
        }

        /// <summary>
        /// Start bounds based on _bounded state.
        /// </summary>
        void Start()
        {
            UpdateBounds();

            screen = GameObject.Find("screen");
            pl = (PlayerObject)GameObject.Find("Local").GetComponent<PlayerObject>();
            standardMaximumPlaneCount = planesScript.MaxPlaneCount;
        }

        /// <summary>
        /// Update position of the planes component to camera position.
        /// Planes query center is based on this position.
        /// </summary>
        void Update()
        {
            _planesComponent.gameObject.transform.position = _camera.transform.position;
        }

        /// <summary>
        /// Cleans up the component.
        /// </summary>
        void OnDestroy()
        {
            MagicLeapDevice.UnregisterOnHeadTrackingMapEvent(OnHeadTrackingMapEvent);
            MLInput.OnControllerButtonDown -= OnButtonDown;
            MLInput.OnTriggerDown -= OnTriggerDown;
        }
        #endregion

        #region Public Methods
        public bool moveToTestingPhase()
        {
            try
            {
                //Make original screen invisible.
                //GameObject screen = GameObject.Find("screen");
                screen.SetActive(false);
            }
            catch{ }

            Object[] pArray = FindObjectsOfType(typeof(PlayerObject));
            if (pArray.Length > 1)
            {
                if(comb.moveToTestingPhase())
                {
                    testingPhase = true;
                    manualControl = false;
                    makeDetectedPlanesInvisible();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }    
        }

        void sendDebugInfo(string name)
        {
            pl.sendDebugInfo(name);
        }

        public bool moveToExploratoryPhase()
        {
            if (comb.moveToExploratoryPhase())
            {
                Object[] pArray = FindObjectsOfType(typeof(PlayerObject));
                if (pArray.Length > 1)
                {
                    exploratoryPhase = true;
                    manualControl = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool moveToCalibrationPhase(bool input)
        {
            calibrationPhase = input;

            return input;
        }

        public bool moveToUltimatePhase()
        {
            ultimatePhase = true;
            return true;
        }

        public bool blankScreen()
        {
            screen.SetActive(false);
            manualControl = false;
            comb.toggleCursor();
            makeDetectedPlanesInvisible();
            return true;
        }

        public void callButtonDown(byte controllerId, MLInputControllerButton button)
        {
            OnButtonDown(controllerId, button);
        }

        public void setTrackingPosition()
        {
            controller = _controllerConnectionHandler.ConnectedController;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Update plane query bounds extents based on if the current _bounded status is true(bounded)
        /// or false(boundless).
        /// </summary>
        private void UpdateBounds()
        {
            _planesComponent.transform.localScale = _bounded ? _boundedExtentsSize : _boundlessExtentsSize;
            _boundsWireframeCube.SetActive(_bounded);

            _boundedExtentsText.text = string.Format("Bounded Extents: ({0},{1},{2})",
                _planesComponent.transform.localScale.x,
                _planesComponent.transform.localScale.y,
                _planesComponent.transform.localScale.z);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Callback handler, changes text when new planes are received.
        /// </summary>
        /// <param name="planes"> Array of new planes. </param>
        /// <param name="planes"> Array of new boundaries. </param>
        public void OnPlanesUpdate(MLWorldPlane[] planes, MLWorldPlaneBoundaries[] boundaries)
        {
            _numberOfPlanesText.text = string.Format("Number of Planes: {0}/{1}", planes.Length, _planesComponent.MaxPlaneCount);
            _numberOfBoundariesText.text = string.Format("Number of Boundaries: {0}/{1}", boundaries.Length, _planesComponent.MaxPlaneCount);
        }

        /// <summary>
        /// Handles the event for button down. Changes from bounded to boundless and viceversa
        /// when pressing home button
        /// </summary>
        /// <param name="controllerId">The id of the controller.</param>
        /// <param name="button">The button that is being released.</param>
        private void OnButtonDown(byte controllerId, MLInputControllerButton button)
        {
            if(!testingPhase && !exploratoryPhase)
            {
                if (_controllerConnectionHandler.IsControllerValid(controllerId) && button == MLInputControllerButton.HomeTap)
                {
                    //_bounded = !_bounded;
                    //UpdateBounds();

                    //NetworkManager eo = GameObject.Find("NetworkManager").GetComponent("NetworkManager") as NetworkManager;
                    //eo.StartClient();
                    _networkMan.StartClient();
                    //_networkMan.StartHost();

                    //Change to other scene.
                    //SceneManager.LoadScene("combinedSceneWithSnapshotMirrorNetworking");
                }
            }
        }

        /// <summary>
        /// Handles the event for trigger down.
        /// </summary>
        /// <param name="controllerId">The id of the controller.</param>
        /// <param name="triggerValue">The value of the trigger.</param>
        private void OnTriggerDown(byte controllerId, float triggerValue)
        {
            if(calibrationPhase)
            {
                controller = _controllerConnectionHandler.ConnectedController;
                Vector3 calPos = GameObject.Find("calibrationTarget").transform.position;//For a standard Unity cube, this is the center.

                float controllerW = controller.Orientation.w;
                float controllerOX = controller.Orientation.x;
                float controllerOY = controller.Orientation.y;
                float controllerOZ = controller.Orientation.z;
                float controllerPX = controller.Position.x;
                float controllerPY = controller.Position.y;
                float controllerPZ = controller.Position.z;

                float calX = calPos.x;
                float calY = calPos.y;
                float calZ = calPos.z;


                pl = (PlayerObject)GameObject.Find("Local").GetComponent<PlayerObject>();
                //Generate and output a line of data.
                //pl.CmdOutputDataLine(_controller.Position, _controller.Orientation);
                pl.CmdSetPos(controllerPX, controllerPY, controllerPZ);
                pl.CmdSetW(controllerW);
                pl.CmdSetZen(calX, calY, calZ);

                pl.CmdOutputCalibrationLine(controllerOX, controllerOY, controllerOZ);
                //pl.CmdSetCalibrationPhase(false);
            }
            else
            {
                if (ultimatePhase)
                {
                    //Thanks for playing!
                }
                else if (!manualControl)
                {
                    pl = (PlayerObject)GameObject.Find("Local").GetComponent<PlayerObject>();
                    
                    controller = _controllerConnectionHandler.ConnectedController;
                    Vector3 zenPos = GameObject.Find("zenith").transform.position;//For a standard Unity cube, this is the center.

                    float controllerW = controller.Orientation.w;
                    float controllerOX = controller.Orientation.x;
                    float controllerOY = controller.Orientation.y;
                    float controllerOZ = controller.Orientation.z;
                    float controllerPX = controller.Position.x;
                    float controllerPY = controller.Position.y;
                    float controllerPZ = controller.Position.z;

                    float zenX = zenPos.x;
                    float zenY = zenPos.y;
                    float zenZ = zenPos.z;


                    pl = (PlayerObject)GameObject.Find("Local").GetComponent<PlayerObject>();
                    //Generate and output a line of data.
                    //pl.CmdOutputDataLine(_controller.Position, _controller.Orientation);
                    pl.CmdSetPos(controllerPX, controllerPY, controllerPZ);
                    pl.CmdSetW(controllerW);
                    pl.CmdSetZen(zenX, zenY, zenZ);

                    pl.CmdOutputDataLine(controllerOX, controllerOY, controllerOZ);
                    //GameObject zenPos = GameObject.Find("zenith");
                    //zenPos.SetActive(false);
                    if (displayScreenAfterEachTest)
                    {
                        sendDebugInfo("Is this working?");
                        manualControl = true;
                        screen.SetActive(true);
                        comb.toggleCursor();
                        makeDetectedPlanesVisible();
                        sendDebugInfo("Is this working?");
                    }
                }
                else
                {
                    if (_controllerConnectionHandler.IsControllerValid(controllerId))
                    {
                        pl = (PlayerObject)GameObject.Find("Local").GetComponent<PlayerObject>();

                        //sendDebugInfo("Is this working?");
                        selected.RaycastTriggerPress();
                    }
                }
            }
        }

        private void makeDetectedPlanesInvisible()
        {
            planesScript.MaxPlaneCount = 0;
        }

        private void makeDetectedPlanesVisible()
        {
            planesScript.MaxPlaneCount = 512;
        }

        /// <summary>
        /// Handle in charge of clearing all planes if map gets lost.
        /// </summary>
        /// <param name="mapEvents"> Map Events that happened. </param>
        private void OnHeadTrackingMapEvent(MLHeadTrackingMapEvent mapEvents)
        {
            if (mapEvents.IsLost())
            {
                _numberOfPlanesText.text = string.Format("Number of Planes: 0/{0}", _planesComponent.MaxPlaneCount);
            }
        }
        #endregion
    }
}
