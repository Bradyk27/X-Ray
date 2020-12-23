/*
.______    __  ___ .______       __    __       _______. _______
|   _  \  |  |/  / |   _  \     |  |  |  |     /       ||   ____|
|  |_)  | |  '  /  |  |_)  |    |  |  |  |    |   (----`|  |__
|   _  <  |    <   |      /     |  |  |  |     \   \    |   __|
|  |_)  | |  .  \  |  |\  \----.|  `--'  | .----)   |   |  |____
|______/  |__|\__\ | _| `._____| \______/  |_______/    |_______|
                                                                */

//Adapted from https://answers.ros.org/question/339483/ros-sharp-unity3d-import-pointcloud2/ & https://github.com/siemens/ros-sharp/blob/master/Libraries/RosBridgeClient/PointCloud.cs
// To Dos:
//Investigate faster publishing methods (depth + image) or  different pointcloud topic
//No idea why but cloudmap is like displaced more than voxelcloud? lol\
//Clean up variable declarations

using UnityEngine;
using System;
using UnityEditor;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class PointcloudSubscriber : Subscriber<Messages.Sensor.PointCloud2>
    {
        //Mesh variables
        public RgbPoint3[] Points;
        private Mesh myMesh;
        // public MeshRenderer meshRenderer;
        private Vector3[] vertices;
        //private int[] triangles;
        private Color[] colors;
        //Subscription variable
        private bool isMessageReceived = false;
        //Game Object
        private GameObject pcObject;

        protected override void Start()
        {
            Debug.Log("Start\n");
            Debug.Log("Start2\n");
            //Start Unity Subscriber
            base.Start();
            //Null reference exception: Object reference not set to an instance of an object (In ML Debug console)
            Debug.Log("Got to 1\n");
            //Create empty new mesh for points
            pcObject = GameObject.Find("PointObject");
            Debug.Log("Got to 2\n");
            myMesh = new Mesh();
            Debug.Log("Got to 3!\n");
            pcObject.GetComponent<MeshFilter>().mesh = myMesh;
            //meshRenderer.material = new Material(Shader.Find("Custom/VertexColor"));
            Debug.Log("Got to 4!\n");
        }

        private void Update() //Function to update when new messages are received
        {
            if(isMessageReceived){
                ProcessMessage();
            }
        }

        protected override void ReceiveMessage(Messages.Sensor.PointCloud2 message) //Automatically called when new mesage received
        {
            Debug.Log("ReceiveMessage\n");
            //Processes message, transforms into form Unity can understand
            //NOTE: Pointcloud.cs methods could be implemented here (depth & image)
            long I = message.data.Length / message.point_step;
            RgbPoint3[] Points = new RgbPoint3[I];
            byte[] byteSlice = new byte[message.point_step];
            for (long i = 0; i < I; i++)
            {
                Array.Copy(message.data, i * message.point_step, byteSlice, 0, message.point_step);
                Points[i] = new RgbPoint3(byteSlice, message.fields);
            }
            vertices = new Vector3[I];
            colors = new Color[I];
            for (var i = 0; i < I; i++)
            {
                vertices[i].x = Points[i].x;
                vertices[i].y = Points[i].z;
                vertices[i].z = Points[i].y;
                colors[i].r = (float)((float)Points[i].rgb[0] / 255.0);
                colors[i].g = (float)((float)Points[i].rgb[1] / 255.0);
                colors[i].b = (float)((float)Points[i].rgb[2] / 255.0);
                colors[i].a = 1.0F;
                //Debug.Log("Colors: " + colors[i].ToString());
                //Debug.Log("Vertex Colors: " + Points[i].rgb[0] + " " + Points[i].rgb[1] + " " + Points[i].rgb[2] + "\n");
            }
            isMessageReceived = true;
        }

        private void ProcessMessage() //Clears mesh and loads new vertices
        {
            Debug.Log("ProcessMessage\n");

            myMesh.Clear(); //Removed try / catch loop
            myMesh.vertices = vertices;
            myMesh.colors = colors;

            //Graphs mesh as points. Works with /rtabmap/cloud_map & /voxel_cloud
            int[] indices = new int[vertices.Length];
            for(int i = 0; i < myMesh.vertices.Length; i++){
                indices[i] = i;
            }
            myMesh.SetIndices(indices, MeshTopology.Points, 0);
            myMesh.RecalculateBounds();
            //AssetDatabase.CreateAsset(mesh, "Assets/testMeshColorRTABMap4.asset");
            //AssetDatabase.SaveAssets();
            isMessageReceived = false; //Resets and waits for new message
        }
    }
}
