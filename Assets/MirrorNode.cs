using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;

public class MirrorNode : NodeMono
{

    public NodeMono original;
    //public EdgeMono  DuplicateEdge;

    public string mirrorID;


    public void UpdateTraversableEdges()
    {

        // go through all  consideredEdges of original node
        // and duplicate it
        //foreach (EdgeMono consideredEdge in original.edgeMonos)
        //{
        //    if (FindSimilarEdges(consideredEdge) == null)
        //    {
        //        DuplicateEdge(consideredEdge);
        //    }

        //}
    }

  
    //

    //if (graph.mirrorsByUID.ContainsKey(data.uid))
    //{
    //    foreach (MirrorNode consideredMirror in graph.mirrorsByUID[data.uid])
    //    {
    //        string id = consideredMirror.mirrorID;
    //        bool isNotThis = id != this.mirrorID;
    //        bool isNotLinkedToThis = !this.mirrorsByMirrorID.ContainsKey(id);

    //        if (isNotThis && isNotLinkedToThis)
    //        {
    //            EdgeMono mirrorBridge = graph.LinkTwoMirrorNodes(this, consideredMirror);



    public EdgeMono[] FindSimilarEdges(EdgeMono comparedEdge)
    {
        List<EdgeMono> matches = new List<EdgeMono>();

        foreach (var kvp in edgeMonosByOtherNodeUID)
        {
            EdgeMono mirroredEdge = kvp.Value;
    
            if (mirroredEdge.IsThisSimilarTo(comparedEdge.data))
            {
                matches.Add(mirroredEdge);
            }
        }

        if (matches.Count > 0)
        {
            return matches.ToArray();
        }
        else
        {
            return null;
        }        
    }


    public void UpdatePanelStyles()
    {
        Dictionary<string, TextPanel> panelsOnOriginalByName = original.GetAllPanelsByName();
        Dictionary<string, TextPanel> myPanelsByName = this.GetAllPanelsByName();
        foreach(var kvp in panelsOnOriginalByName)
        {
            string name = kvp.Key;
            TextPanel originalPanel = kvp.Value;
            TextPanel myPanel = myPanelsByName[name];
            myPanel.MimicStyle(originalPanel);
        }
    }


    //TODO move this to graph
    void  DuplicateEdge(EdgeMono edgeToCopy)
    {

        Graph graph = this.GetGraph();

        bool isDirectional = edgeToCopy.data.isDirectional;
        string verb = edgeToCopy.data.verb;

        NodeMono newPair0 = null;
        NodeMono newPair1 = null;
        if (edgeToCopy.pairMono[0] == original)
        {
            //  consideredEdge points to the other node
            newPair0 = this;
            newPair1 = edgeToCopy.pairMono[1];
        }
        if (edgeToCopy.pairMono[1] == original)
        {
            //  consideredEdge points to the other node
            newPair0 = edgeToCopy.pairMono[0];
            newPair1 = this;
        }

        graph.LinkTwoNodes(newPair0, verb, newPair1, isDirectional);
    }










}
