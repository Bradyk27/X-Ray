using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using UnityEngine;
using UnityEditor;


//For this to work properly, the program must be started with no model childed to trackedItem,
//There must be a regular function that sets tryImporting to true,
//and there must be data being streamed in.

//TODO: Testing, fallbacks for if no network stream is running, tests to make sure items are being deleted from Leap.
    public class itemImporter : MonoBehaviour
    {
        bool tryImporting;
        //string importName;

        string oldPath;

        GameObject trackedItem;

        GameObject cameraData;

        Vector3 pointCloudScale;
        Quaternion pointCloudRotation;
        Vector3 pointCloudPositionOffset;

        // Start is called before the first frame update
        void Start()
        {
            tryImporting = false;
            //importName = "name";
            oldPath = "";
            cameraData = null;

            trackedItem = GameObject.Find("trackedItem");

            pointCloudScale = new Vector3(-20, 20, 20); 
            pointCloudRotation = Quaternion.Euler(0, 180, 0);
            pointCloudPositionOffset = new Vector3(0, (float)-.52, 0);
        }

        // Update is called once per frame
        void Update()
        {
            //if (tryImporting)
            //{
                //Check for network input from DATASTREAMING.

                //If network input is true:
                //createMesh();
            //}
        }

        public string importPLY()
        {
            tryImporting = false;

            string fileLocation = "";

            //Send handshake to dataStreaming.
            //fileLocation = BaseApp::getWritablePath() + importName + ".ply";

            //// Encode texture into PNG
            //byte[] bytes = tex.EncodeToPNG();
            // For testing purposes, also write to a file in the project folder
            // File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);

            /*using (var fileStream = File.Create("C:\\Path\\To\\File"))
            {
                myOtherObject.InputStream.Seek(0, SeekOrigin.Begin);
                myOtherObject.InputStream.CopyTo(fileStream);
            }*/

            return fileLocation;
        }

        //public bool createMesh()
        //{
        //    //Maybe set up try//catch here?
        //    if(oldPath != "")
        //    {
        //        File.Delete(oldPath);
        //        Destroy(cameraData);
        //    }

        //    //string fileName = importPLY();

        //    string path = "tmp/" + "path.file";

        //    //Stream myOtherObject = null;//This stream should be set to the input data from the network connection.

        //    //retrieve the writeable path and the prepare the data.txt file to be read
        //    //ifstream myfilein(BaseApp::getWritablePath() + "data.txt");
        //    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write);

        //    //fs.Seek(0, SeekOrigin.Begin);
        //    //fs.CopyTo(myOtherObject);

        //    fs.Close();
        //    //myOtherObject.Close();

        //    GameObject ab = Resources.Load<GameObject>(path); // This is how you would load a blender file at fileLocation

        //    Mesh body = ab.GetComponent<MeshFilter>().sharedMesh;



        //    GameObject pointCloudData = new GameObject("pointCloudData");
        //    pointCloudData.AddComponent<MeshFilter>();
        //    pointCloudData.GetComponent<MeshFilter>().mesh = body;
        //    pointCloudData.AddComponent<MeshRenderer>();
        //    pointCloudData.GetComponent<MeshRenderer>().material = Resources.Load("Default Point") as Material;//I *believe* that this includes the shader assigned to Default Point.
        //    pointCloudData.transform.SetParent(trackedItem.transform);

        //    pointCloudData.transform.position += pointCloudPositionOffset;
        //    pointCloudData.transform.localRotation = pointCloudRotation;
        //    pointCloudData.transform.localScale = pointCloudScale;

        //    cameraData = pointCloudData;

        //    oldPath = path;

        //    return true;
        //}

        public bool createMeshFromNetwork(GameObject cloudData)
        {
            foreach (Transform child in trackedItem.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            GameObject pointCloudData = new GameObject("pointCloudData");
            pointCloudData.AddComponent<MeshFilter>();
            pointCloudData.GetComponent<MeshFilter>().mesh = cloudData.GetComponent<MeshFilter>().sharedMesh; ;
            pointCloudData.AddComponent<MeshRenderer>();
            pointCloudData.GetComponent<MeshRenderer>().material = Resources.Load("Default Point") as Material;//I *believe* that this includes the shader assigned to Default Point.
            pointCloudData.transform.SetParent(trackedItem.transform);

            pointCloudData.transform.position += pointCloudPositionOffset;
            pointCloudData.transform.localRotation = pointCloudRotation;
            pointCloudData.transform.localScale = pointCloudScale;

            cameraData = pointCloudData;

            return true;
        }
    }