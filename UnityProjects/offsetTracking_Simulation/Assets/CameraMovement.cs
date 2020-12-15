using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {   
    }

    public float speed = 5.0f;
    // Update is called once per frame
    void Update()
 {
     if(Input.GetKey(KeyCode.RightArrow))
     {
         transform.Rotate(Vector3.up, speed * Time.deltaTime * 10);
     }
     if(Input.GetKey(KeyCode.LeftArrow))
     {
         transform.Rotate(Vector3.down, speed * Time.deltaTime * 10);
     }
     if(Input.GetKey(KeyCode.DownArrow))
     {
         transform.Translate(new Vector3(0,0, -speed * Time.deltaTime));
     }
     if(Input.GetKey(KeyCode.UpArrow))
     {
         transform.Translate(new Vector3(0,0, speed * Time.deltaTime));
     }
 }
}
