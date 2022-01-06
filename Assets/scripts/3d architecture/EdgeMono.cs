using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;

[System.Serializable]
public class EdgeData
{
    public string uid;
    public string verb;
    public NodeData[] pair;
    public bool isDirectional;

    public EdgeData(NodeData[] nodeDataPair, string verbDescription, bool isDirectionalBool = false, string UID = null)
    {
        verb = verbDescription;
        pair= nodeDataPair;
        isDirectional = isDirectionalBool;

        if (UID == null)
        {
            Debug.Log("you forgot to generate uid:  (on mono) GetGraph().MakeNewUID()");
        }
        else
        {
            uid = UID;
        }
    }

    public NodeData OtherThan(NodeData thisNodeData)
    {
        if (thisNodeData.uid == pair[0].uid) { return pair[1]; }
        if (thisNodeData.uid == pair[1].uid) { return pair[0]; }
        return null;
    }

    public override string ToString()
    {

        //return "edge print not implemented";
        if (pair == null ^ pair[0] == null ^ pair[1] == null)
        {
            return null;
        }
        else
        {
            string arrow = "";
            if (isDirectional) { arrow = ">"; }
            return $"{pair[0].name} - {verb} - {arrow} {pair[1].name} ";
        }

    }

    public void Print()
    {
        Debug.Log(this.ToString());
    }
}


public class EdgeMono : MonoBehaviour
{
    public  EdgeData data;


    public bool isHidden;

    public Transform cylinder;

    public NodeMono[] pairMono;
    //public NodeMono[] pairMono;


    public void Set(NodeMono pairMono0, string verb, NodeMono pairMono1, bool isDirectional, string uid)
    {
        this.NullCheckLog("pairMono0", pairMono0 );
        this.NullCheckLog("pairMono1", pairMono1);

        pairMono = new NodeMono[] { pairMono0, pairMono1 };
        data.pair = new NodeData[] { pairMono0.data, pairMono1.data };
        data.verb = verb;
        data.uid = uid;
        data.isDirectional = isDirectional;
    }

    public float thickness = 0.2f;

    public void UpdateTransform()
    {
        Vector3 pos0 = pairMono[0].transform.position;
        Vector3 pos1 = pairMono[1].transform.position;
       
        transform.position = pos0;

        Vector3 rotation = pos1 - pos0;
        if (rotation != Vector3.zero)
        {
            transform.localRotation = Quaternion.LookRotation(rotation, Vector3.up);
        }
        


        float distance = Vector3.Distance(pos0, pos1);
        cylinder.transform.localScale = new Vector3(thickness, distance / 2, thickness);
        cylinder.transform.localPosition = new Vector3(0, 0, distance / 2);

    }



    public NodeMono getOtherNode(NodeMono thisNode)
    {
        if (thisNode == pairMono[0]) { return pairMono[1]; }
        else if (thisNode == pairMono[1]) { return pairMono[0]; }
        else
            Debug.LogError("Node I'm not connected to just asked me about the other");
            return null;
    }

    public void UpdateLabel()
    {
        string dataString = data.ToString();
        if(dataString != null)
        {
            gameObject.name = data.verb;
            //TODO world space label

            //            Print();
        }

        
    }



    private void Update()
    {
        /// ALL THIS SHOULD BE DONE BY EVENTS ?
        /// 
        if (pairMono != null && pairMono[0] != null && pairMono[1] != null)
        {
            if (pairMono[0].gameObject.activeSelf && pairMono[1].gameObject.activeSelf)
            {
                isHidden = false;
            }
            else
            {
                isHidden = true;
            }

        }
        // ///

        if (isHidden)
        {
            cylinder.gameObject.SetActive(false);
        }
        else
        {
            cylinder.gameObject.SetActive(true);
            UpdateTransform();
        }

  
        
    }

    public override string ToString()
    {
        return data.ToString();
    }

    [ExposeMethodInEditor]
    public void Print()
    {
        Debug.Log(this.ToString());
        Debug.Log($"{pairMono[0].transform.position} {pairMono[1].transform.position}");
    }




}
