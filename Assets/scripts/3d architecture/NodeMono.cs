using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ExtentionMethods;

// TODO
// node.minimize

[System.Serializable]
public class NodeData
{
    public string uid;
    public string type;
    public string name;
    public string status;
    public string description;
    public DateTime due_on;
    public List<string> tags;

    [HideInInspector]
    public List<string> edgeDataReferences = new List<string>();

    public NodeData(string Uid = null, string Name = null, string Type = null, long due_date_as_TICKS = -23, string Description=null, List<string> Tags = null)
    {
        uid = Uid;
        name = Name;
        type = Type;
        description = Description;
        tags = Tags;
        if (due_date_as_TICKS >= 0) { due_on = new DateTime(due_date_as_TICKS); }
    }

    public override string ToString()
    {
        return $" {type} { name} {uid}";
    }
    public void Print()
    {
        Debug.Log(this.ToString());
    }
}

public class NodeMono : MonoBehaviour
{

    public TextPanel namePanel;
  
    public TextPanel tagsPanel;
    public TextPanel typePanel;
    public TextPanel descriptionPanel;
    public TextPanel datePanel;

    private NodeData _data;
    public NodeData data
    {
        get { return _data; }
        set { _data = value;}
    }
    //public List<EdgeMono> edgeMonos;

    public Dictionary<string, EdgeMono> edgeMonosByOtherNodeUID;


    public Dictionary<string, MirrorNode> mirrorsByMirrorID;
    public Dictionary<string, EdgeMono> mirrorBridgesByUID;


    public Collider nodeCollider;
    public Transform visibleNode;
    public RectTransform mainLayoutRect;

    public Color defaultPanelColor;
    public Color defaultTextColor;

    public Color highlightedPanelColor;
    public Color highlightedTextColor;

    public Color discretePanelColor;
    public Color discreteTextColor;

    
    public float layoutWidth
    {
        get { return mainLayoutRect.GetWidth(); }
        set { mainLayoutRect.sizeDelta = new Vector2(value, layoutHeight); }
    }

    public float layoutHeight
    {
        get { return mainLayoutRect.GetHeight(); }
        set { mainLayoutRect.sizeDelta = new Vector2(layoutWidth, value); }
    }

    //Properties for easy access
    private float posXlocal;
    public float PosXlocal
    {
        get { return transform.position.x; }
        set { transform.position = new Vector3(value, transform.position.y,transform.position.z); }
    }

    private float posYlocal;
    public float PosYlocal
    {
        get { return transform.position.y; }
        set { transform.position = new Vector3(transform.position.x, value, transform.position.z); }
    }

    private float posZlocal;
    public float PosZlocal
    {
        get { return transform.position.z; }
        set { transform.position = new Vector3(transform.position.x, transform.position.y, value); }
    }


    [ExposeMethodInEditor]
    public void PrintEdges()
    {

        Debug.Log($"{edgeMonosByOtherNodeUID.Count} edges of {this.name}");
        foreach(var kvp in edgeMonosByOtherNodeUID)
        {
            kvp.Value.Print();
        }
    }


    public void AddEdge(EdgeMono edgeMono)
    {


        string otherUID = edgeMono.GetOtherNode(this).data.uid;

        if (edgeMonosByOtherNodeUID.ContainsKey(otherUID))
        {
            Debug.Log($"edge{edgeMono.name} is already in {this.name} dictionary. there was an attempt to add it again");
            return;
        }

        if (edgeMonosByOtherNodeUID.ContainsValue(edgeMono))
        {
            Debug.Log($"edge{edgeMono.name} is already in {this.name} dictionary. there was an attempt to add it again");
            return;
        }

  

        if (edgeMono.GetOtherNode(this)) // null if edge does not remember this node
        {

            if(edgeMono.GetOtherNode(this) is MirrorNode)
            {
                Debug.Log("mirror node should never be passed here");
            }

            edgeMonosByOtherNodeUID.Add(otherUID, edgeMono);


            data.edgeDataReferences.Add(edgeMono.data.uid);


            //foreach(var kvp in mirrorsByMirrorID)
            //{
            //    MirrorNode mirror = kvp.Value;
            //    mirror.UpdateTraversableEdges();
            //}
        }

    }

    void Awake()
    {

   //     edgeMonos = new List<EdgeMono>();
        mirrorsByMirrorID = new Dictionary<string, MirrorNode>();
        mirrorBridgesByUID = new Dictionary<string, EdgeMono>();
        edgeMonosByOtherNodeUID = new Dictionary<string, EdgeMono>();

    }

    private void Update()
    {

        // only if i moved
        if (transform.hasChanged)
        {
            //only if ther are mirrors 
            if (mirrorBridgesByUID.Count > 0)
            {
                NegotiateMirrorScopes();
            }

            // update positions and rotations of active edges
            foreach (var kvp in edgeMonosByOtherNodeUID)
            {
                EdgeMono edge = kvp.Value;
                if (edge.isHidden == false)
                {
                    edge.UpdateTransform();
                }
            }

            // update positions and rotations of mirror bridges 
            foreach(var kvp in mirrorBridgesByUID)
            {
                kvp.Value.UpdateTransform();
            }

            //reset bool
            transform.hasChanged = false;
        }
    }

    private void OnEnable()
    {
        foreach (var kvp in edgeMonosByOtherNodeUID)
        {
            EdgeMono edge = kvp.Value;
            Debug.Log($"showing {edge.name}");
            edge.isHidden = false;
        }

    }

    private void OnDisable()
    {
        foreach (var kvp in edgeMonosByOtherNodeUID)
        {
            EdgeMono edge = kvp.Value;
            if (edge != null)
            {
                edge.isHidden = true;
            }

        }

    }
    public void OnDestroy()
    {
        if (this is MirrorNode == false)
        {

            // destroy all mirrors
            foreach (var kvp in mirrorsByMirrorID)
            {
                DestroyMirror(kvp.Key);
            }
        }

        else
        {
            NodeMono orig = ((MirrorNode)this).original;
            orig.mirrorsByMirrorID.Remove(((MirrorNode)this).mirrorID);
            foreach(var kvp in mirrorBridgesByUID)
            {
                Destroy(kvp.Value.gameObject);
            }
            orig.NegotiateMirrorScopes();
        }

        }


    

    //public List<NodeMono> GetDirectlyLinkedNodeMonos()
    //{ // returns only nodemonos directly connected by edgemono (exluding connections handled by mirrors)

    //    List<NodeMono> DirectlyLinked = new List<NodeMono>();
    //    foreach (var kvp in edgeMonosByOtherNodeUID)
    //    {
    //        EdgeMono edge = kvp.Value;
    //        DirectlyLinked.Add(edge.GetOtherNode(this));
    //    }
    //    return DirectlyLinked;
    //}



    public void UpdateDisplays()
    {
        typePanel.textString = data.type;
        namePanel.textString = data.name;
        if(data.tags == null) { tagsPanel.textString = ""; }
        else { tagsPanel.textString = data.tags.ToString(); }
        descriptionPanel.textString = data.description;
        datePanel.gameObject.SetActive(false);

        if(this is MirrorNode)
        {
            ((MirrorNode)this).UpdatePanelStyles();
        }


        //UpdateColliderSize();
    }


    ////TODO show incompleted tasks greyed out?
    //public void SetVisibility(float fraction = 1)
    //{
    //    if(fraction == 0) { visibleNode.gameObject.SetActive(false);}
    //    else
    //    {
    //        visibleNode.gameObject.SetActive(true);
    //        Text[] texts = visibleNode.GetComponentsInChildren<Text>();
    //        foreach(Text text in texts)
    //        {
    //            text.color = new Color(0, 0, 0, fraction);
    //        }
    //        // TODO alpha of renderer material?
    //    }

        
    //}


    //[ExposeMethodInEditor]
    //public void UpdateColliderSize()
    //{
        
    //    float layoutWidth  = mainLayoutRect.GetWidth() + 40;
    //    float layoutHeight = mainLayoutRect.GetHeight();
    //    float depth = 1f;

    //    nodeCollider.transform.position = mainLayoutRect.position;
    //    nodeCollider.transform.localScale = new Vector3(layoutWidth, layoutHeight, depth);
    //    // not implemented
    //}



    // 3D

    public void RotateXYToFace(Transform looker)
    {
        Quaternion rotation = Quaternion.LookRotation(transform.position- looker.position, Vector3.up);
        
        transform.rotation = rotation;
        //transform.LookAt(looker);

    }

    public void RotateXYtoAlignWith(Transform reference)
    {
        transform.rotation = reference.transform.rotation;
    }

    public void RotateYToFace(Transform looker)
    {
        float x = looker.position.x;
        float y = transform.position.y;
        float z = looker.position.z;
        Vector3 lookerPositionLeveled = new Vector3(x, y, z);

        
        Quaternion rotation = Quaternion.LookRotation(transform.position - lookerPositionLeveled, Vector3.up);

        transform.rotation = rotation;
        //transform.LookAt(looker);





    }

    // COLORS AND PANELS

    public void GreyOutVisibleNode()
    {
        List<TextPanel> allPanels = new List<TextPanel>() { namePanel, typePanel, tagsPanel, datePanel, descriptionPanel};

        foreach(TextPanel panel in allPanels)
        {
            DeEmphasizePanel(panel);
        }

        //visibleNode.GetComponentInChildren<Renderer>(true).enabled = false;
        // todo: object for performing operations on node's 3d renderers
    }

    [ExposeMethodInEditor]
    void HighlightNamePanel()
    {
        HighlightPanel(namePanel);
    }

    [ExposeMethodInEditor]
    void UnHighlightNamePanel()
    {
        ResetPanelColors(namePanel);
    }

    void DeEmphasizePanel(TextPanel panel)
    {
        panel.ApplyColors(discretePanelColor, discreteTextColor);
    }

    void ResetPanelColors(TextPanel panel)
    {
        panel.ApplyColors(defaultPanelColor, defaultTextColor);
    }

    void HighlightPanel(TextPanel panel)
    {
        panel.ApplyColors(highlightedPanelColor, highlightedTextColor);
    }

    [ExposeMethodInEditor]
    public void Minimize()
    {
        typePanel.gameObject.SetActive(false);
        descriptionPanel.gameObject.SetActive(false);
        datePanel.gameObject.SetActive(false);

    }

    [ExposeMethodInEditor]
    public void Maximize()
    {
        typePanel.gameObject.SetActive(true);
        descriptionPanel.gameObject.SetActive(true);
        datePanel.gameObject.SetActive(true);

    }

    public Dictionary<string, TextPanel> GetAllPanelsByName()
    {
        Dictionary<string, TextPanel> allPanelsByName = new Dictionary<string, TextPanel>();
        this.NullCheckLog("children Text Panels", GetComponentInChildren<TextPanel>());

        Debug.Log($"allPanelsByName.Count {allPanelsByName.Count}");
        foreach(TextPanel panel in GetComponentsInChildren<TextPanel>())
        {
            Debug.Log($"adding {panel.name}: {panel.textString}");
            allPanelsByName.Add(panel.name, panel);
        }


        return allPanelsByName;

    }



    // MIRROR HANDLING

    void NegotiateMirrorScopes()
    {
        // prevent from repeating this on every mirror. this should suffice
        if(this is MirrorNode)
        {
            ((MirrorNode)this).original.NegotiateMirrorScopes();
        }
        else
        {
            Graph graph = GetGraph();



            List<EdgeMono> edgeMonos = new List<EdgeMono>();
            Dictionary<NodeMono, EdgeMono> allEdgesByOtherNode = GetAllEdgesByOtherNodeMono();

            // populate candidates with this and all mirrors
            List<NodeMono> candidates = new List<NodeMono>() { this };
            foreach(var kvp in mirrorsByMirrorID)
            {
                candidates.Add(kvp.Value);
            }

            candidates.Print();


            // get closest mirror for every linked node in dict

            List<NodeMono> others = graph.AllNodeMonosLinkedTo(this);

            others.Print($"nodes connected to {this.name}");

            foreach (NodeMono other in others)
            {
                NodeMono closestCandidate = new NodeMono(); // new() only so that it compiles
                float smallestDistance = 1000000;
                foreach (NodeMono candidate in candidates)
                {
                    float distance = Vector3.Distance(candidate.transform.position, other.transform.position);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        closestCandidate = candidate;
                    }
                }

                //Debug.Log($"edgeMonosByOtherNodeUID[ {other.data.uid} ]   ({other.data.name})");

                //Debug.Log(edgeMonosByOtherNodeUID[other.data.uid]);

                //;
                //EdgeMono edgeToTake = edgeMonosByOtherNodeUID[other.data.uid];

                closestCandidate.TakeOverEdge(allEdgesByOtherNode[other]);


                //   mirrorsByClosestOther.Add(other, closestMirror);
            }

  

        }

    }


    public void TakeOverEdge(EdgeMono edge)
    {
        string thisID = data.uid;
        NodeMono[] pairMono = edge.pairMono;

        // only if this is actualy my edge
        // TODO edge.isThisNodeLinked(uid) // instead of if below
        if (pairMono[0].data.uid == thisID || pairMono[1].data.uid == thisID) 
        {
            NodeMono other = edge.GetOtherNode(GetGraph().GetNodeByUID(thisID));

            // decide which of pair is my mirror or me;
            if(pairMono[0].data.uid == thisID)
            {
                //remove edge from disconnecded nodeMono and pair to me
                pairMono[0].edgeMonosByOtherNodeUID.Remove(other.data.uid);
                edge.pairMono = new NodeMono[] { this, other };
            }
            else if (pairMono[1].data.uid == thisID)
            {
                //remove edge from disconnecded nodeMono and pair to me
                pairMono[1].edgeMonosByOtherNodeUID.Remove(other.data.uid);
                edge.pairMono = new NodeMono[] { other, this };
            }
            else
            {
                Debug.LogError($"{this.name} asked to take over edge that it should not: {edge.ToString()}");

            }



        }


    }


    [ExposeMethodInEditor]
    public void Print()
    {
        //Debug.Log($"{data.name} {transform.position} ");
        data.Print();
        //Graph graph = this.GetGraph();
        //List<string> connectedNodes = new List<string>();
        //foreach (var kvp in edgeMonosByOtherNodeUID)
        //{
        //    EdgeMono edge = kvp.Value;
        //    connectedNodes.Add(edge.GetOtherNode(this).name);
        //}
        //connectedNodes.Print($"{this.name}  connected nodes");
    }


    [ExposeMethodInEditor]
    public void PrintMirrorEdges()
    {
        Debug.Log($"{mirrorBridgesByUID.Values.Count} mirror edges of {this.name}");
        //foreach(var kvp in mirrorBridgesByUID)
        //{
        //    kvp.Value.Print();
        //}
    }

    public void MirrorAllLinkedNodes(NodeList2D list2D)
    {
        Graph graph = GetGraph();
        foreach(NodeMono linkedNode in graph.AllNodeMonosLinkedTo(this))
        {
            MirrorNode newMirror = graph.CreateMirror(linkedNode);
            list2D.AdoptNode(newMirror);
        }
    }


    public void DestroyMirror(string mirrorID)
    {
        Destroy(mirrorsByMirrorID[mirrorID].gameObject);
    }


    Graph GetGraph()
    {

        Transform candidate = this.transform; ;

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


    public Dictionary<NodeMono, EdgeMono> GetAllEdgesByOtherNodeMono()
    {
        Graph graph = GetGraph();
        Dictionary<NodeMono, EdgeMono> allEdgesByOtherNodeMono = new Dictionary<NodeMono, EdgeMono>();
        foreach (string edgeUID in data.edgeDataReferences)
        {
            EdgeMono edge = graph.edgesByUID[edgeUID];
            NodeMono otherMono = edge.GetOtherNode(this);
            allEdgesByOtherNodeMono.Add(otherMono, edge);

        }
        return allEdgesByOtherNodeMono;
    }
}
