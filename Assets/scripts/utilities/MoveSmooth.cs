using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSmooth : MonoBehaviour
{

    public Vector3 rememberedDestination;
    public float smoothTime = 0.2f;
    public float errorMargin = 0.1f;

    public bool continueMoving = false;

    public static Vector3 vel;// = Vector3.zero;

    [ExposeMethodInEditor]
    public void StopMovingNow()
    {
        SetDestination(transform.position, 0);
       
    }

    [ExposeMethodInEditor]
    public void NewRandomMove()
    {
       
        SetDestination(zExtensionsRandom.RandomizeXY(Vector3.zero));
    }

    public void SetDestination(Vector3 newDestination, int frames = 40)
    {
        continueMoving = false;
        StartCoroutine(GoToGlobaPosition(newDestination, frames));

    }

    IEnumerator GoToGlobaPosition(Vector3 newDestination, int frames)
    {

        continueMoving = true;
        Vector3 origin = transform.position;

        float stepSize = 1f / frames;
        for (float interpolation = 0f; interpolation <= 1; interpolation += stepSize)
        {
            if (continueMoving)
            {

                transform.position = Vector3.Lerp(origin, newDestination, interpolation);
                yield return null;
            }
        }

    }

    //IEnumerator GoToGlobaPosition(Vector3 newDestination, float frames)
    //{
    //    // TODO use time as smoothtime
    //    continueMoving = true;

    //    while (Vector3.Distance(transform.position, newDestination) > errorMargin)
    //    {
    //        if (continueMoving)
    //        {
    //            transform.position = Vector3.SmoothDamp(transform.position, newDestination, ref vel, 0.1f);
    //            yield return null;
    //        }
    //    }

    //    Debug.Log(Vector3.Distance(transform.position, newDestination));

    //}




    //private void Update()
    //{

    //    if (continueMoving && Vector3.Distance(transform.position, rememberedDestination) > accuracy)
    //    {
    //        transform.position = Vector3.SmoothDamp(transform.position, rememberedDestination, ref vel, smoothTime);
    //    }


    //}

}
