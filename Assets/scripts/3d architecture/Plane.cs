using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public List<NodeMono> NodesCurrentlyHeld;
    public Transform meshTransform;
    public float spacing;

    //TODO wszystko źle

    public Vector3 offerVacantLocalPosition(NodeMono node)
    {
        NodesCurrentlyHeld.Add(node);

        float xOffset = meshTransform.localScale.x / 2;

        float offeredX = meshTransform.localPosition .x - xOffset + NodesCurrentlyHeld.Count*spacing;
        float offeredY = meshTransform.localPosition.y;
        float offeredZ = meshTransform.localPosition.z;

        Vector3 offer =  new Vector3(offeredX, offeredY, offeredZ);
       

        return offer;
    }
}
