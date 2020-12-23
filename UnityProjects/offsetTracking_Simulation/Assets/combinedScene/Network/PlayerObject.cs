using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using Mirror;

//The goals for this player object class:
//Allow a button press to push an update to the Leap {space bar on the server PC} (eventually this task will be offloaded to the Leap, but for testing purposes this is easier);
//Display a very simple GUI/control scheme;
//Display the current live camera feed.

//Note: To run the server, [CONTENT] should be made inactive.
namespace MagicLeap
{
    public class PlayerObject : NetworkBehaviour
    {
        /// <summary>
        /// Data File variables-must be changed before each new participant.
        /// </summary>
        private string folderLoc = ".\\Output\\";
        private string subjectNumber = "s666";
        private string experimentTitle = "XRayTriangulationInitialTrials";
        private string testingBlockFileName;
        //private int variableNumber = 6;
        //Initial variables include subject number, lateral position (the position in the hall outside the room), controller origin, controller rotation, the position of the target center, and distance of estimates from the target.
        private string[] variableTypes = new string[] { "subject", "lateralPosition", "origin\t\t", "rot\t\t", "targetCenter", "distanceMissed", "VirtualCondition" };
        //private int[] lateralDistances = new int[] { 0, 1, 2, 3, 4, 5, -1, -2, -3, -4, -5 };//1 game unit = 1 meter.  Should be double-checked, but seems to hold true.
        private int[] lateralDistances = new int[] { -1, 0, 1, 2, 3, 4, 5 };//1 game unit = 1 meter.  Should be double-checked, but seems to hold true.
        int lateralDistanceLimit = 2;
        private string realCondition = "Virt";
        private CombinedHolder variableHolder;

        /// <summary>
        /// Normal variables.
        /// </summary>

        private MLInputController controller;

        private GameObject cloudHolder = null;
        private GameObject targetPos = null;
        private RaycastVisualizer rv = null;
        private PlanesExampleNetworked planesExampleNet = null;

        private itemImporter scriptName;

        private GameObject cube;

        private Shader defaultPoint = null;
        private Shader standardShader = null;
        private Material brickMaterial = null;

        private bool newDataNeeded = true;

        private Mesh newMesh = null;

        private MeshFilter a;
        private MeshRenderer b;
        private BoxCollider c;
        private SphereCollider d;

        bool calibrationPhase = false;
        bool exploratoryPhase = false;
        bool testingPhase = false;
        bool ultimatePhase = false;
        string phaseState = "";

        bool calibrated = false;
        Quaternion calibratedQuaternionOffset;

        int testCount = 0;
        string countString = "0";

        bool firstTimeOutput = true;
        string outputFileName = "";

        Vector3 pos;
        Vector3 zen;
        Quaternion orient;
        float w;

        public GUIStyle phaseStyle;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            gameObject.name = "Local";
        }

        // Start is called before the first frame update
        void Start()
        {
            //controller = MLInput.GetController(MLInput.Hand.Left);

            testingBlockFileName = folderLoc + experimentTitle + subjectNumber + ".order";

            cube = GameObject.Find("pointCloud");

            defaultPoint = Shader.Find("Default Point");
            standardShader = Shader.Find("Standard");

            brickMaterial = Resources.Load("UnlitWall", typeof(Material)) as Material;

            phaseState = "Setup Phase";

#if UNITY_STANDALONE_WIN
            //Create the order file.
            //lateralDistances = generateRandomOrder<int>();
            outputLateralDistances();
#elif UNITY_EDITOR
            //lateralDistances = generateRandomOrder();
            outputLateralDistances();
#else
            rv = GameObject.Find("Cursor (Head)").GetComponent<RaycastVisualizer>();
            planesExampleNet = GameObject.Find("PlanesExample").GetComponent<PlanesExampleNetworked>();
#endif

            variableHolder = GameObject.Find("[ALWAYSONCONTENT]").GetComponent<CombinedHolder>();
            bool temporaryVar = variableHolder.getCalibrationTargetVisibility();

            cloudHolder = GameObject.Find("followerItem");
            targetPos = GameObject.Find("calibrationTarget");
            if(targetPos != null)
            {
                targetPos.SetActive(temporaryVar);
            }           

            if (!isLocalPlayer)
            {
                //Actions to take for the non-controlling participants.
                return;
            }

            
            //variableHolder = GameObject.Find("[ALWAYSONCONTENT]").GetComponent<CombinedHolder>();
            //targetPos.SetActive(false);
            //rv = GameObject.Find("Cursor (Head)").GetComponent<RaycastVisualizer>();
            //planesExampleNet = GameObject.Find("PlanesExample").GetComponent<PlanesExampleNetworked>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer)
            {
                if (isServer)
                {
                    //Actions to take for the server
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        //If there is at least one client connected to the host/server (in addition to the host itself).
                        if (NetworkServer.connections.Count > 1)
                        {
                            //Call and activate the code to load and transmit the point cloud.
                            if (newDataNeeded)
                            {
                                //This path points to the point cloud PLY data.
                                //In the future, we will use take a new picture on each pass; currently, we are just using a single photo.
                                //string path = "D:/Nate/MLEnvironment/XRay/3D Room Models (PLY)/chairInHallExport.ply";

                                //TODO: Set newMesh to the pcd mesh in bunny.pcd.
                                //string path = "D:/Nate/MLEnvironment/XRay/offsetTracking/Assets/bunny.pcd";

                                GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                newMesh = temp.GetComponent<MeshFilter>().mesh;

                                if (newMesh != null)
                                {
                                    Debug.Log("Mesh isn't null.");
                                }

                                //This sends newMesh to all clients through a binary stream.
                                CmdDoServerSideThing();

                                newDataNeeded = false;
                            }

                            CmdSetModel();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.E))
                    {
                        //PRE-CONDITONS: Walls mapped; fiducial placed and set.
                        if (!exploratoryPhase)
                        {
                            //Reset screen; allow screen placement.  Remove commands for fiducial placing; make fiducial graphic invisible.
                            CmdSetExploratoryPhase();
                        }
                        //PRE-CONDITIONS: User must have set screen and explored environment.
                        else if (!testingPhase)
                        {
                            //Remove screen (and placing functionality); make environment objects inactive.
                            CmdSetTestingPhase();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.T))
                    {
                        if(targetPos.activeSelf)
                        {
                            controllerCalibration(true);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.N))
                    {
                        blankScreen();
                    }
                    else if (Input.GetKeyDown(KeyCode.R))
                    {
                        realCondition = "Real";
                        variableHolder.setCondition();
                        CmdSetRealCondition();
                    }
                }
                //Actions for players who aren't the server.
                else
                {

                }

                //Display camera feed.
                //TODO: It might be interesting to display a live feed of the camera on the server PC.

                return;
            }
            else
            {
                //Actions to take for everyone but the local player.
                return;
            }
        }

        void OnGUI()
        {
            if(isServer)
            {
                phaseStyle = new GUIStyle(GUI.skin.label);
                phaseStyle.fontSize = 26;
                if (isLocalPlayer)
                {
                    GUI.Label(new Rect(400, 50, 200, 200), phaseState, phaseStyle);
                    GUI.Label(new Rect(450, 100, 200, 200), realCondition, phaseStyle);
                }
                else
                {
                    if (testCount >= lateralDistances.Length)
                    {
                        GUI.Label(new Rect(400, 300, 200, 200), "Finished", phaseStyle);
                    }
                    else
                    {
                        GUI.Label(new Rect(400, 300, 200, 200), (lateralDistances[testCount]).ToString(), phaseStyle);
                    }
                }

                if (NetworkServer.connections.Count > 1)
                {
                    GUI.Label(new Rect(200, 150, 200, 200), ("Network\nConnections: " + NetworkServer.connections.Count), phaseStyle);
                }
            }
        }

        //Take the array lateralDistances and randomize it.  Do not allow adjacent entries within lateralDistanceLimit to be side-by-side.
        private int[] generateRandomOrder()
        {
            int numberTries = 20;

            List<int> randomList = new List<int>();
            List<int> generatedList = lateralDistances.OfType<int>().ToList();

            int totalItems = lateralDistances.GetLength(0);
            int temp = 0;

            if (totalItems <= 1)
            {
                return generatedList.ToArray();
            }

            int r = UnityEngine.Random.Range(0, totalItems - 1);
            //Debug.Log("Check distance:" + checkDistance(generatedList));
            while (!checkDistance(generatedList) && numberTries > 0)
            {
                while (temp < totalItems)
                {
                    r = UnityEngine.Random.Range(0, (totalItems - temp - 1));
                    randomList.Add(generatedList[r]); //add it to the new, random list
                    generatedList.RemoveAt(r); //remove to avoid duplicates
                    temp++;
                }

                generatedList = randomList;
                numberTries--;
            }

            return generatedList.ToArray();
        }

        void blankScreen()
        {
            RpcBlankScreen();
        }

        public void sendDebugInfo(string name)
        {
            CmdSendDebugInfo(name);
        }

        [Command]
        void CmdSendDebugInfo(string name)
        {
            Debug.Log(name);
        }

        [ClientRpc]
        void RpcBlankScreen()
        {
#if UNITY_STANDALONE_WIN
            Debug.Log("Windows Standalone version: RPC blank screen called.");
#elif UNITY_EDITOR
            Debug.Log("Unity Editor version: RPC blank screen called.");
#else
            rv.activateScreen(false);
            planesExampleNet.blankScreen();
#endif
        }

        bool checkDistance(List<int> generatedList)
        {
            int temp = 0;
            bool output = true;
            int totalItems = generatedList.Count;

            if (totalItems <= 1)
            {
                return true;
            }

            while (temp < totalItems)
            {
                if (temp == 0)
                {
                    if (temp + 1 < totalItems)
                    {
                        if (Math.Abs(generatedList[temp] - generatedList[(temp + 1)]) <= lateralDistanceLimit)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (temp + 1 < totalItems)
                    {
                        if (Math.Abs(generatedList[temp] - generatedList[(temp + 1)]) <= lateralDistanceLimit)
                        {
                            return false;
                        }
                        if (Math.Abs(generatedList[temp] - generatedList[(temp - 1)]) <= lateralDistanceLimit)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Math.Abs(generatedList[temp] - generatedList[(temp - 1)]) <= lateralDistanceLimit)
                        {
                            return false;
                        }
                    }
                }
                temp++;
            }

            return output;
        }

        void outputLateralDistances()
        {
            System.IO.File.WriteAllLines(testingBlockFileName,
            lateralDistances.Select(tb => (tb.ToString())));
        }

        void setModel()
        {
            //Debug.Log("Prior to cloud deletion");
            //For some reason, Unity will not tolerate destruction or transform of the pointCloud object.  Possibly because it's a prefab?  Unknown.
            //In the meanwhile, I shall use the gameObject placeholderItem instead of the pointCloud.  It is possible this problem will go away on its own
            //when we automate the AR camera.
            //SOLUTION: Turns out, pointCloud is a special reserved variable name in the Magic Leap.  We can't use it!

            if (cube != null)
            {
                GameObject.Destroy(cube);

                GameObject unused = new GameObject("deliciousData");

                if (newMesh != null)
                {
                    /*string path = "D:/Nate/MLEnvironment/XRay/3D Room Models (PLY)/chairInHallExport.ply";
                    GameObject objectFromFile = Resources.Load<GameObject>(path);
                    objectFromFile.transform.position = cube.transform.position;
                    objectFromFile.transform.SetParent(cloudHolder.transform);*/

                    Debug.Log("New mesh in use.");

                    MeshFilter meshForUn = unused.AddComponent<MeshFilter>();
                    meshForUn.sharedMesh = newMesh;
                    MeshRenderer meshRenderer = unused.AddComponent<MeshRenderer>();
                    meshRenderer.material = brickMaterial;
                    meshRenderer.material.shader = standardShader;
                    unused.transform.position = cube.transform.position;
                    unused.transform.SetParent(cloudHolder.transform);

                    newDataNeeded = true;
                }

                cube = unused;
                Debug.Log("Cube deleted.");
            }


            /*if (cloudHolder != null)
            {
                Debug.Log("cloudHolder is definitely real!");
                foreach (Transform child in cloudHolder.transform)
                {
                    Debug.Log("Removing child object.");
                    GameObject.Destroy(child.gameObject);
                    //Renderer rend = child.gameObject.GetComponent<Renderer>();
                    //rend.material.shader = standardShader;
                }
            }*/
            //Debug.Log("Post cloud deletion");
        }

        //This code will only run on the server.  The RPC must be called to execute any code on the clients.
        [Command]
        void CmdDoServerSideThing()
        {
            SendMesh("meshName", newMesh);
            //RpcDoServerSideThing();
        }

        [Command]
        void CmdSetRealCondition()
        {
            realCondition = "Real";
            RpcSetRealCondition();  
        }

        [ClientRpc]
        void RpcSetRealCondition()
        {
            realCondition = "Real";
            targetPos.SetActive(false);
            //cloudHolder.SetActive(false);
        }
        
        //This code will run on *each* client, including the host.
        [ClientRpc]
        void RpcSetTrackingPosition()
        {
#if UNITY_STANDALONE_WIN
                        Debug.Log("Registered set position command.");
#elif UNITY_EDITOR
            Debug.Log("Registered set position command.");
#else
                        planesExampleNet.setTrackingPosition();
#endif
        }

        //This code will only run on the server.  The RPC must be called to execute any code on the clients.
        [Command]
        void CmdSetModel()
        {
            RpcSetModel();
        }

        //This code will run on *each* client, including the host.
        [ClientRpc]
        void RpcSetModel()
        {
            //cube.transform.localScale = cube.transform.localScale * 10;
            setModel();
        }

        //A carrier for the data needed to reconstruct a mesh.
        [System.Serializable]
        public class MeshData
        {
            public string name;
            public int[] triangles;
            public SerializableVector3[] vertices;
            public SerializableVector2[] uv2;
            public SerializableVector3[] normals;
        }

        //Only to be called by server.
        //Formats the mesh into a binary format so that it can be sent to clients.
        public void SendMesh(string name, Mesh mesh)
        {
            var mem = new MemoryStream();
            var fmt = new BinaryFormatter();
            fmt.Serialize(mem, new MeshData
            {
                triangles = mesh.triangles,
                normals = Array.ConvertAll(mesh.normals, delegate (Vector3 a) { return (SerializableVector3)a; }),
                vertices = Array.ConvertAll(mesh.vertices, delegate (Vector3 a) { return (SerializableVector3)a; }),
                uv2 = Array.ConvertAll(mesh.uv2, delegate (Vector2 a) { return (SerializableVector2)a; }),
                name = name
            });
            RpcGetMesh(mem.GetBuffer());
        }

        //This code will run on *each* client, including the host.
        //Recieves the sent mesh via binary formatting.
        [ClientRpc]
        void RpcGetMesh(byte[] data)
        {
            var fmt = new BinaryFormatter();
            var mem = new MemoryStream(data);
            var md = fmt.Deserialize(mem) as MeshData;
            var mesh = new Mesh
            {
                vertices = Array.ConvertAll(md.vertices, delegate (SerializableVector3 a) { return (Vector3)a; }),
                normals = Array.ConvertAll(md.normals, delegate (SerializableVector3 a) { return (Vector3)a; }),
                triangles = md.triangles,
                uv2 = Array.ConvertAll(md.uv2, delegate (SerializableVector2 a) { return (Vector2)a; })
            };
            mesh.RecalculateBounds();
            newMesh = mesh;

            Debug.Log("Mesh payload delivered!");
        }

        //This code will run on *each* client, including the host.
        [ClientRpc]
        void RpcSetUltimatePhase()
        {

            GameObject zenPos2 = GameObject.Find("zenith");
            zenPos2.SetActive(false);
#if UNITY_STANDALONE_WIN
            phaseState = "Ultimate Phase";
#elif UNITY_EDITOR
            phaseState = "Ultimate Phase";
#else
            //PRE-CONDITONS: Walls mapped; fiducial placed and set.
            planesExampleNet.moveToUltimatePhase();
            phaseState = "Ultimate Phase";
#endif
        }

        bool setUltimatePhase()
        {
            RpcSetUltimatePhase();
            return true;
        }

        void controllerCalibration(bool input)
        {
            if (input == true)
            {
                targetPos.SetActive(true);
                CmdSetCalibrationPhase(true);
                Debug.Log("Blue sphere, out there!");
            }
            else
            {
                targetPos.SetActive(false);
                CmdSetCalibrationPhase(false);
            }
            //Have a user point at object and press trigger.
        }

        double findDistanceMissed()
        {
            //Zen, pos, and orient must already be defined for this function to work properly.
            
            double result = 0;

            //Quaternion conjOrient = new Quaternion(-orient.x, -orient.y, -orient.z, orient.w);

            //This needs some testing, yo!  I'm really not sure if I set this up correctly.
            //Vector3 point2Temp = orient*pos;//Plus an arbitrary distance along the line formed by the quaternion/Euler angles.
            //Vector3 point2 = Quaternion.Inverse(conjOrient) * point2Temp;

            Quaternion baseline = Quaternion.FromToRotation(Vector3.forward, zen - pos);

            if(calibrated)
            {
                orient = orient * Quaternion.Inverse(calibratedQuaternionOffset);
            }

            float angle = Math.Abs(Quaternion.Angle(orient, baseline));
            //Debug.Log("First angle calculated: " + angle);
            //We're interested in the rotation around the Z-axis, so we select the first Euler angle.
            angle = orient.eulerAngles.z;
            //Debug.Log("Second angle calculated: " + angle);

            Debug.Log("Angle: " + angle);
            //Debug.Log("QZen: " + orient);
            //Debug.Log("QId: " + baseline);
            //double pointDistance = Math.Sqrt(Math.Pow(pos.x - zen.x, 2.0) + Math.Pow(pos.y - zen.y, 2.0) + Math.Pow(pos.z - zen.z, 2.0));
            double pointDistance = Math.Sqrt(Math.Pow(pos.x - zen.x, 2.0) + Math.Pow(pos.y - zen.y, 2.0));
            Debug.Log("Straight distance to target: " + pointDistance);
            result = pointDistance * Math.Tan(Math.PI * angle/180.0);
            Debug.Log("The tan of angle * distance is: " + result);
            return result;
        }

        //This code will only run on the server.  The RPC must be called to execute any code on the clients.
        [Command]
        void CmdSetTestingPhase()
        {
            RpcSetTestingPhase();
        }

        [Command]
        void CmdRejectTestingPhase()
        {
            testingPhase = false;
            phaseState = "Exploratory Phase";
        }

        //This code will run on *each* client, including the host.
        [ClientRpc]
        void RpcSetTestingPhase()
        {
            Debug.Log("Setting testing phase.");
            //cube.transform.localScale = cube.transform.localScale * 10;
            if (setTestingPhase())
            {
                testingPhase = true;
            } 
            else
            {
                //Note: It is assumed that only one server//Unity Editor or Windows version will be running at a time.  If this is not the case, this logic may not work properly.
                CmdRejectTestingPhase();
            }
        }

        bool setTestingPhase()
        {
#if UNITY_STANDALONE_WIN
            phaseState = "Testing Phase";
            return true;
#elif UNITY_EDITOR
            phaseState = "Testing Phase";
            return true;
#else
            //PRE-CONDITONS: Walls mapped; fiducial placed and set.
            if (planesExampleNet.moveToTestingPhase())
            {
                //Remove screen (and placing functionality); make environment objects inactive.
                //rv.destroyScreen();
                //If we want to instead briefly make the screen invisible, we can use:
                rv.activateScreen(false);
                phaseState = "Testing Phase";
                return true;
            }
            else
            {
                return false;
            }
#endif
        }
        
        //This code will only run on the server.  The RPC must be called to execute any code on the clients.
        [Command]
        public void CmdSetCalibrationPhase(bool input)
        {
            RpcSetCalibrationPhase(input);
        }

        //This code will run on *each* client, including the host.
        [ClientRpc]
        void RpcSetCalibrationPhase(bool input)
        {
            calibrationPhase = input;

            //cube.transform.localScale = cube.transform.localScale * 10;
            setCalibrationPhase(input);
            if(!input && targetPos != null)
            {
                targetPos.SetActive(false);
            }
        }

        bool setCalibrationPhase(bool input)
        {
#if UNITY_STANDALONE_WIN
            if(input)
            {
                phaseState = "Calibration Phase";
                calibrationPhase = true;
            }
            else
            {
                if (ultimatePhase)
                {
                    calibrationPhase = false;
                    phaseState = "Ultimate Phase";
                }
                else if (testingPhase)
                {
                    calibrationPhase = false;
                    phaseState = "Testing Phase";
                }
                else
                {
                    calibrationPhase = false;
                    phaseState = "Exploratory Phase";
                }
            }
            return input;
#elif UNITY_EDITOR
            if (input)
            {
                phaseState = "Calibration Phase";
                calibrationPhase = true;
            }
            else
            {
                if (ultimatePhase)
                {
                    calibrationPhase = false;
                    phaseState = "Ultimate Phase";
                }
                else if (testingPhase)
                {
                    calibrationPhase = false;
                    phaseState = "Testing Phase";
                }
                else
                {
                    calibrationPhase = false;
                    phaseState = "Exploratory Phase";
                }
            }
            return input;
#else
            //PRE-CONDITONS: Walls mapped; fiducial placed and set.
            if (planesExampleNet.moveToCalibrationPhase(input))
            {
                calibrationPhase = true;
                phaseState = "Calibration Phase";
                return true;
            }
            else
            {
                if(ultimatePhase)
                {
                    calibrationPhase = false;
                    phaseState = "Ultimate Phase";
                }
                else if(testingPhase)
                {
                    calibrationPhase = false;
                    phaseState = "Testing Phase";
                }
                else
                {
                    calibrationPhase = false;
                    phaseState = "Exploratory Phase";
                }
                return false;
            }
#endif
        }

        //This code will only run on the server.  The RPC must be called to execute any code on the clients.
        [Command]
        void CmdSetExploratoryPhase()
        {
            RpcSetExploratoryPhase();
        }

        [Command]
        void CmdRejectExploratoryPhase()
        {
            exploratoryPhase = false;
            phaseState = "Setup Phase";
        }

        //This code will run on *each* client, including the host.
        [ClientRpc]
        void RpcSetExploratoryPhase()
        {
            Debug.Log("Setting exploratory phase.");
            //cube.transform.localScale = cube.transform.localScale * 10;
            if (setExploratoryPhase())
            {
                exploratoryPhase = true;
            }
            else
            {
                //Note: It is assumed that only one server//Unity Editor or Windows version will be running at a time.  If this is not the case, this logic may not work properly.
                //This does not yet work.
                CmdRejectExploratoryPhase();
            }
        }

        bool setExploratoryPhase()
        {
            //GameObject zenPos = GameObject.Find("zenith");
            //zenPos.SetActive(false);
#if UNITY_STANDALONE_WIN
            phaseState = "Exploratory Phase";
            return true;
#elif UNITY_EDITOR
            phaseState = "Exploratory Phase";
            return true;
#else
            if (planesExampleNet.moveToExploratoryPhase())
            {  
                
                //Reset screen; allow screen placement.  Remove commands for fiducial placing; make fiducial graphic invisible.
                phaseState = "Exploratory Phase";
                return true;
            }
            else
            {
                return false;
            }
            //PRE-CONDITONS: Walls mapped; fiducial placed and set.
            return true;
            
#endif

        }

        [Command]
        public void CmdSetPos(float posX, float posY, float posZ)
        {
            pos = new Vector3(posX, posY, posZ);
        }
        [Command]
        public void CmdSetW(float input)
        {
            w = input;
        }

        [Command]
        public void CmdSetZen(float x, float y, float z)
        {
            zen = new Vector3(x, y, z);
        }

        [Command]
        public void CmdOutputDataLine(float orientX, float orientY, float orientZ)
        {
            //GameObject zenPos2 = GameObject.Find("zenith");
            //zenPos2.SetActive(false);
            orient = new Quaternion(orientX, orientY, orientZ, w);

            //Debug.Log("Made it into output date line command.");
            string outputString = "";
            
            if (firstTimeOutput)
            {
                //Can add date if further differentiation is desired.
                outputFileName = folderLoc + experimentTitle + subjectNumber + ".out";
                if(!File.Exists(outputFileName))
                {
                    string temp = "";
                    using (StreamWriter sw = File.CreateText(outputFileName))
                    {
                        int i = 0;
                        while(i < variableTypes.Length)
                        {
                            temp = temp + variableTypes[i] + "\t\t";

                            if(i == 3)
                            {
                                temp += "\t";
                            }

                            i++;
                        }
                        sw.WriteLine(temp);
                    }
                }
                firstTimeOutput = false;
            }
            Debug.Log("Starting.");
            double distanceMissed = findDistanceMissed();
            outputString += subjectNumber + "\t\t" + lateralDistances[testCount] +"\t\t\t" + pos.ToString() + "\t\t" + orient.ToString() + "\t" + zen + "\t\t" + distanceMissed + "\t\t" + variableHolder.getCondition();
            testCount++;

            Debug.Log("Streaming output.");
            using (StreamWriter sw = File.AppendText(outputFileName))
            {
                sw.WriteLine(outputString);
            }
            //Debug.Log("Finished.");
            if(testCount >= lateralDistances.Length)
            {
                countString = "Finished";
                setUltimatePhase();
            }
        }

        //Not complete yet.
        [Command]
        public void CmdOutputCalibrationLine(float orientX, float orientY, float orientZ)
        {
            orient = new Quaternion(orientX, orientY, orientZ, w);

            //Zen, pos, and orient must already be defined for this to work properly.
            double result = 0;


            Quaternion baseline = Quaternion.FromToRotation(Vector3.forward, zen - pos);

            calibratedQuaternionOffset = orient * Quaternion.Inverse(baseline);

            float angle = Math.Abs(Quaternion.Angle(orient, baseline));
            //Debug.Log("First angle calculated: " + angle);
            //We're interested in the rotation around the Z-axis, so we select the first Euler angle.
            angle = orient.eulerAngles.z;
            //Debug.Log("Second angle calculated: " + angle);

            Debug.Log("Angle:" + angle);
            //Debug.Log("QZen:" + orient);
            //Debug.Log("QId:" + baseline);
            //double pointDistance = Math.Sqrt(Math.Pow(pos.x - zen.x, 2.0) + Math.Pow(pos.y - zen.y, 2.0) + Math.Pow(pos.z - zen.z, 2.0));
            double pointDistance = Math.Sqrt(Math.Pow(pos.x - zen.x, 2.0) + Math.Pow(pos.y - zen.y, 2.0));
            Debug.Log("Straight distance to target:" + pointDistance);
            result = pointDistance * Math.Tan(Math.PI * angle / 180.0);
            Debug.Log("The tan of angle * distance is:" + result);


            calibrated = true;

            RpcSetCalibrationPhase(false);
            //CmdSetCalibrationPhase(false);
        }

        //This code will run on *each* client, including the host.
        [ClientRpc]
        void RpcSetTestCount(int count)
        {
            testCount = count;
        }


        /// <summary>
        /// Since unity doesn't flag the Vector3 as serializable, we
        /// need to create our own version. This one will automatically convert
        /// between Vector3 and SerializableVector3
        /// </summary>
        [System.Serializable]
        public struct SerializableVector3
        {
            /// <summary>
            /// x component
            /// </summary>
            public float x;

            /// <summary>
            /// y component
            /// </summary>
            public float y;

            /// <summary>
            /// z component
            /// </summary>
            public float z;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="rX"></param>
            /// <param name="rY"></param>
            /// <param name="rZ"></param>
            public SerializableVector3(float rX, float rY, float rZ)
            {
                x = rX;
                y = rY;
                z = rZ;
            }

            /// <summary>
            /// Returns a string representation of the object
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return String.Format("[{0}, {1}, {2}]", x, y, z);
            }

            /// <summary>
            /// Automatic conversion from SerializableVector3 to Vector3
            /// </summary>
            /// <param name="rValue"></param>
            /// <returns></returns>
            public static implicit operator Vector3(SerializableVector3 rValue)
            {
                return new Vector3(rValue.x, rValue.y, rValue.z);
            }

            /// <summary>
            /// Automatic conversion from Vector3 to SerializableVector3
            /// </summary>
            /// <param name="rValue"></param>
            /// <returns></returns>
            public static implicit operator SerializableVector3(Vector3 rValue)
            {
                return new SerializableVector3(rValue.x, rValue.y, rValue.z);
            }
        }

        /// <summary>
        /// Since unity doesn't flag the Vector3 as serializable, we
        /// need to create our own version. This one will automatically convert
        /// between Vector3 and SerializableVector3
        /// </summary>
        [System.Serializable]
        public struct SerializableVector2
        {
            /// <summary>
            /// x component
            /// </summary>
            public float x;

            /// <summary>
            /// y component
            /// </summary>
            public float y;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="rX"></param>
            /// <param name="rY"></param>
            public SerializableVector2(float rX, float rY)
            {
                x = rX;
                y = rY;
            }

            /// <summary>
            /// Returns a string representation of the object
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return String.Format("[{0}, {1}]", x, y);
            }

            /// <summary>
            /// Automatic conversion from SerializableVector3 to Vector3
            /// </summary>
            /// <param name="rValue"></param>
            /// <returns></returns>
            public static implicit operator Vector2(SerializableVector2 rValue)
            {
                return new Vector2(rValue.x, rValue.y);
            }

            /// <summary>
            /// Automatic conversion from Vector3 to SerializableVector3
            /// </summary>
            /// <param name="rValue"></param>
            /// <returns></returns>
            public static implicit operator SerializableVector2(Vector2 rValue)
            {
                return new SerializableVector2(rValue.x, rValue.y);
            }
        }
    }
}