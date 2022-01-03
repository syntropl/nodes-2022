using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;

public class Viewpoint : MonoBehaviour
{
    public Transform focalPoint;

    [ExposeMethodInEditor]
    public void JumpCameraHere()
    {
        Graph graph = this.GetGraph();
        graph.camera.transform.position = this.transform.position;
        graph.camera.gazePointer.transform.position = focalPoint.transform.position;
        graph.camera.transform.rotation = Quaternion.LookRotation(focalPoint.position, Vector3.up);
    }

    [ExposeMethodInEditor]
    public void MoveCameraHere()
    {
        Graph graph = this.GetGraph();
        graph.camera.ApplyViewpoint(this);
    }


    private void Start()
    {
        if (focalPoint == null)
        {
            Debug.LogError($"{gameObject.name}'s Viewpoint component requires a focalPoint Transform to work");
        }

        if (focalPoint.GetComponent<MeshRenderer>() == null) { Debug.LogError("focal point mesh renderer not found"); }

        MeshRenderer focalMesh = focalPoint.GetComponent<MeshRenderer>();

        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = false;
        }
        //focalMesh.enabled = false;

        if (this.GetComponent<MeshRenderer>() == null) { Debug.LogError("viewpoint mesh renderer not found"); }
        MeshRenderer viewPosition = GetComponent<MeshRenderer>();
        viewPosition.enabled = false;




    }
}
