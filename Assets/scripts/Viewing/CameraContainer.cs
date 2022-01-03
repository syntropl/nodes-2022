using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;

public class CameraContainer : MonoBehaviour
{
    public Camera camera;
    public emptyMono gazePointer;
    public Plane hudPlane;

    public bool lookAtGazePointer;

    public Viewpoint testViewpoint1;
    public Viewpoint testViewpoint2;
    public Transform objectToFollow;


    public void KeepLookingAtObject(Transform obj)
    {
        gazePointer.transform.SetParent(obj);
        gazePointer.MoveTowards(obj.position);
    }

    [ExposeMethodInEditor]
    public void StopFollowingObject()
    {
        gazePointer.transform.SetParent(null);
    }


    public void ApplyViewpoint(Viewpoint viewpoint)
    {
        lookAtGazePointer = true;
        //KeepLookingAtObject(viewpoint.focalPoint);

        this.MoveTowards(viewpoint.transform.position);//, 0.3f, 0.15f);
        
        gazePointer.MoveTowards(viewpoint.focalPoint.transform.position);
    }

    [ExposeMethodInEditor]
    private void LookAtFocalPoint()
    {
        Vector3 direction = gazePointer.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    [ExposeMethodInEditor]
    public void TestFollowObject()
    {
        KeepLookingAtObject(objectToFollow);
    }

    [ExposeMethodInEditor]
    public void TestView1()
    {
        Debug.Log("ApplyViewpoint(testViewpoint1);");
        ApplyViewpoint(testViewpoint1);
    }

    [ExposeMethodInEditor]
    public void TestView2()
    {
        ApplyViewpoint(testViewpoint2);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            lookAtGazePointer = !lookAtGazePointer;
        }


        if (lookAtGazePointer)
        {
            LookAtFocalPoint();
        }

    }

    private void Start()
    {
        if (camera == null)
        {
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            if (camera == null)
            {
                Debug.LogError("no GameObject named \"Main Camera\" was found");
            }
        }




        TestView1();

    }



    

}
