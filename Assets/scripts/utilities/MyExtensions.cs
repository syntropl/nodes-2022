using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace ExtentionMethods
{
    public static class MyExtensions
    {

        public const long TicksInOneDay = 864000000000;
        public static void Print(this IEnumerable enumerable, string label = "")
        {

            int count = 0;
            foreach (object obj in enumerable) { count++; }

            Debug.Log("");
            Debug.Log("(" + count + " objects) " + label + ":");
            Debug.Log("{ ");

            int i = 0;
            foreach (object obj in enumerable)
            {
                Debug.Log("[" + i + "] " + obj.ToString() + ", ");
                i++;
            }
            Debug.Log(" }");

        }

        public static void Print(this MonoBehaviour mono)
        {

            if (mono.GetComponentInChildren<Text>() != null)
            {
                Debug.Log($"{mono.transform.position}   {mono.GetComponentInChildren<Text>().text}");
            }
            Debug.Log($"{mono.transform.position} { mono.name}  is on gamoebject: {mono.gameObject.name}");
        }


        public static void NullCheckLog(this MonoBehaviour thisMono, string varName, MonoBehaviour checkedMono)
        {
            if(checkedMono== null)
            {
                Debug.Log($"{thisMono.GetType()} NullReference:<color=red> {varName} is null</color>");
            }
        }


        //public static void MoveTowards(this MonoBehaviour thisMono, Vector3 destination, float smoothTime = 0.5f, float accuracy = 0.3f)
        //{
        //    MoveSmooth mover = thisMono.AddOrGetComponent<MoveSmooth>();
        //    mover.SetDestination(destination, smoothTime);
        //}

        public static void MoveTowards(this MonoBehaviour thisMono, Vector3 destination, int frames = 50)
        {
            MoveSmooth mover = thisMono.AddOrGetComponent<MoveSmooth>();
            mover.SetDestination(destination, frames);
        }

        public static void StopMoving(this MonoBehaviour thisMono)
        {
            MoveSmooth mover = thisMono.AddOrGetComponent<MoveSmooth>();
            mover.StopMovingNow();
        }


        public static void SuddenLookAt(this MonoBehaviour thisMono, Vector3 lookAtPostion)
        {
            thisMono.transform.rotation = Quaternion.LookRotation(-lookAtPostion);
        }


        public static void ObjectLookAt(this MonoBehaviour thisMono, Vector3 lookatPosition, float frames = 50f)
        {

            thisMono.StartCoroutine(RotateLerp(thisMono, lookatPosition, frames));
        }

        static IEnumerator RotateLerp(this MonoBehaviour thisMono, Vector3 lookatPosition, float frames)
        {

            Quaternion startRotation = thisMono.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(-lookatPosition);

            float stepSize = 1 / frames;
            for (float interpolation = 0; interpolation < 1; interpolation += stepSize)
            {

                thisMono.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, interpolation);
                yield return null;
            }
        }


        //TODO searching for inactive children nodes. still not working
        public static NodeMono[] GetChildrenNodes(this MonoBehaviour thisMono, bool includeInactive = true)
        {
            if (thisMono.GetComponentInChildren<NodeMono>(includeInactive))
            {
                return thisMono.GetComponentsInChildren<NodeMono>(includeInactive);
            }
            if (includeInactive)
            {
                Debug.Log($"{thisMono.gameObject.name} : no children nodes found");
            }
            else
            {
                Debug.Log($"{thisMono.gameObject.name} : no ACTIVE children nodes found");
            }

            return null;
        }

        public static NodeMono GetParentNode(this MonoBehaviour thisMono)
        {
            Transform candidate = thisMono.transform;

            while (candidate != null)
            {
                if (candidate.GetComponent<NodeMono>() == null)
                {
                    candidate = candidate.parent;
                }
                else
                {
                    return candidate.GetComponent<NodeMono>();
                }
            }
            return null;
            
        }

        public static Graph GetGraph(this MonoBehaviour thisMono)
        {

            Transform candidate = thisMono.transform; ;

            while (candidate != null)
            {
                //Debug.Log($"looking for graph component on : {candidate.gameObject.name}");
                if (candidate.GetComponent<Graph>() == null)
                {
                    candidate = candidate.parent;
                }
                else
                {
                    // Debug.Log($"graph found on{candidate.gameObject.name}");
                    return candidate.GetComponent<Graph>();
                }
            }
            Debug.Log("i've looked up the hierarchy and found no Graph component");
            return null;
        }




        public static void AddUnique(this List<NodeMono> thisList, NodeMono objectToAdd)
        {
            if (!thisList.Contains(objectToAdd)) { thisList.Add(objectToAdd); }
        }

        public static void AddUniqueRange(this List<NodeMono> thisList, IEnumerable<NodeMono> enumerable)
        {
            foreach(NodeMono objectToAdd in enumerable)
            {
                thisList.AddUnique(objectToAdd);
            }

        }

        public static void AddUnique(this List<string> thisList, string objectToAdd)
        {
            if (!thisList.Contains(objectToAdd)) { thisList.Add(objectToAdd); }
        }

        public static void AddUniqueRange(this List<string> thisList, IEnumerable<string> enumerable)
        {
            foreach (string objectToAdd in enumerable)
            {
                thisList.AddUnique(objectToAdd);
            }

        }

        public static Color RGBA(this Color input)
        {
            float r = input.r / 255;
            float g = input.g / 255;
            float b = input.b / 255;
            float a = input.a / 255;

            return new Color(r, g, b, a);
        }
    }
}


