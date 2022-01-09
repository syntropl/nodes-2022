using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSmooth : MonoBehaviour
{

    public Vector3 rememberedDestination;
    public float smoothTime = 0.2f;
    public float errorMargin = 0.1f;

  

    public static Vector3 vel;// = Vector3.zero;

    [ExposeMethodInEditor]
    public void StopMovingNow()
    {
        SetDestination(transform.position, 0);
       
    }


    bool useLerp = true;

    [ExposeMethodInEditor]
    public void NewRandomMove()
    {
       
        SetDestination(zExtensionsRandom.RandomizeXY(Vector3.zero));
    }

    public void SetDestination(Vector3 newDestination, int frames = 40)
    {

            if (useLerp)
            {

                StartCoroutine(GoToGlobaPositionLerp(newDestination,frames));
            }
            else
            {

                StartCoroutine(GoToGlobaPosition(newDestination, frames));


            }

    }




    //IEnumerator GoToGlobaPositionLerp(Vector3 newDestination, int frames)
    //{

    //    isMoving = true;
    //    Vector3 origin = transform.position;

    //    float stepSize = 1f / frames;
    //    for (float interpolation = 0f; interpolation <= 1; interpolation += stepSize)
    //    {
    //        if (isMoving)
    //        {

    //            transform.position = Vector3.Lerp(origin, newDestination, Mathf.SmoothStep(0,1,interpolation));
    //            yield return null;
    //        }
    //    }

    //}

    IEnumerator GoToGlobaPositionLerp(Vector3 newDestination, int frames)
    {


        Vector3 origin = transform.position;

        float stepSize = 1f / frames;
        for (float interpolation = 0f; interpolation <= 1; interpolation += stepSize)
        {

            transform.position = Vector3.Lerp(origin, newDestination, Mathf.SmoothStep(0,1,interpolation));
            yield return null;
            
        }

    }


    IEnumerator GoToGlobaPosition(Vector3 newDestination, float frames)
    {

        float time = frames / 200;
        // TODO use time as smoothtime



        while (Vector3.Distance(transform.position, newDestination) > errorMargin)
        {
           
            transform.position = Vector3.SmoothDamp(transform.position, newDestination, ref vel, time);
            yield return null;
            
        }


        Debug.Log(Vector3.Distance(transform.position, newDestination));

    }




    //private void Update()
    //{

    //    if (isMoving && Vector3.Distance(transform.position, rememberedDestination) > accuracy)
    //    {
    //        transform.position = Vector3.SmoothDamp(transform.position, rememberedDestination, ref vel, smoothTime);
    //    }


    //}

}
