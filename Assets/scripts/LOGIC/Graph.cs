using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;
using System.Linq;

public class Graph : MonoBehaviour
{

    public GraphMetadata metadata;


    public List<NodeMono> createdNodes;
    public List<EdgeMono> createdEdges;

    public Dictionary<string, NodeMono> nodesByUID;
    public Dictionary<string, EdgeMono> edgesByUID;

    public Transform edgesContainer;
    public Transform nodesContainer;
    //public Plane newNodePlane; //TODO this should be a nodeList
    public NodeList2D newNodesList2D;

    public GameObject nodeTemplate;
    public GameObject nodeFormTemplate;
    public GameObject edgeTemplate;
    public GameObject nodeListTemplate;

    public Vector3 newNodesListPosition = new Vector3(-425, 290, 380);
    public Vector3 newNodesListRotation = new Vector3(0, -66, 0);


    public CameraContainer camera;

    //public NodeForm nodeEditor;

    public GameObject timelineTemplate;

    private void Awake()
    {
        nodesByUID = new Dictionary<string, NodeMono>();
        edgesByUID = new Dictionary<string, EdgeMono>();

        createdNodes = new List<NodeMono>();
        newNodesList2D = CreateNewNodeList("new nodes", new Vector3(-100, -200, 200), null, 1, nodesContainer);
        newNodesList2D.transform.position = newNodesListPosition;
        newNodesList2D.transform.localEulerAngles = newNodesListRotation;

        Debug.Log(newNodesList2D);
        nodeTemplate.gameObject.SetActive(false);
        nodeFormTemplate.gameObject.SetActive(false);
        //

    }

    private void Update()
    {


        //RotateAllYTowardsCamera();

        if (Time.frameCount == 100)
        {
            // LATE START DEBUG


            TimelineTest();


        }
    }

    [ExposeMethodInEditor]
    void PrintHiddenNodes()
    {
        List<NodeMono> inactiveNodes = new List<NodeMono>();
        foreach (NodeMono node in this.GetChildrenNodes(true))
        {
            if (node.gameObject.activeSelf)
            {
                inactiveNodes.Add(node);
            }
        }

        inactiveNodes.Print();
    }



    [ExposeMethodInEditor]
    public void TimelineTest()
    {
        List<NodeMono> allNodes = nodesByUID.Values.ToList<NodeMono>();
        CreateTimeline(null, new Vector3(0, 0, 0), allNodes);
        newNodesList2D.UpdatePositions();
    }

    public EdgeMono LinkToExistingNodeOrCreateNew(NodeMono originNode, string verb, NodeData nodeDataTolink, bool isDirectional = false)
    {

        NodeMono destinationNode = GetNodeByUID(nodeDataTolink.uid);


        if (destinationNode == null)
        {
            destinationNode = CreateOrGetNode(nodeDataTolink);
        }
        //        Debug.Log($"destination node found. Linking: {originNode.data.name} - {destinationNode.data.name}");

        return LinkTwoNodes(originNode, verb, destinationNode, isDirectional);

    }

    public EdgeMono LinkTwoNodes(NodeMono node0, string verb, NodeMono node1, bool isDirectional = false)
    {
        GameObject newGo = Instantiate(edgeTemplate, edgesContainer);
        newGo.SetActive(true);
        newGo.name = $"-[{verb}]-";
        if (newGo.GetComponent<EdgeMono>() == null) { Debug.Log("New Edge is missing an EdgeMono component"); }
        EdgeMono edgeMono = newGo.GetComponent<EdgeMono>();

        string uid = MakeNewUID($"({verb})");

        edgeMono.Set(node0, verb, node1, isDirectional, uid); 
        edgeMono.data = new EdgeData(new NodeData[]{ node0.data, node1.data}, verb, isDirectional,uid);

        // TODO check for existing edge like this
        // at node0 looka at every edge and for each
        //          if node on other end is node1
        //              if verb is the same
        //                  show warning, compare two verbs, ask for confirmation;


        edgesByUID.Add(uid, edgeMono);

        return edgeMono;
    }


    //public EdgeMono CreateEmptyEdge(NodeMono node0, NodeMono node1)
    //{
    //    GameObject newGo = Instantiate(edgeTemplate, edgesContainer);
    //    newGo.SetActive(true);
    //    newGo.name = "emtpy edge";
    //    if(newGo.GetComponent<EdgeMono>()== null) { Debug.Log("New Edge is missing an EdgeMono component"); }
    //    EdgeMono edge = newGo.GetComponent<EdgeMono>();
    //    edge.pairMono = new NodeMono[] { node0, node1 };
    //    return edge;
    //}

    public void CreateNewNodeFromForm(NodeForm form)
    {

        NodeMono newNode = CreateOrGetNode(form.NodeDataFromForm());
        Debug.Log(newNode.ToString());
        Destroy(form.gameObject);
    }


    public NodeMono CreateOrGetNode(NodeData data, NodeList2D location = null) // rename to CreateOrGet
    {

        
        NodeMono match = GetNodeByUID(data.uid);
        if (match)
        {
            //TODO alert user, perhaps ask.
            //if i get uid from asana or generate and it matches an existing node it wont be created

            return match;
        }

        if(data.uid == null)
        {
            data.uid = MakeNewUID();
        }
        

        // instantiate and position new empty node
        if(location == null) { location = newNodesList2D; }
        GameObject newGo = Instantiate(nodeTemplate, location.transform);
        newGo.name = "○ " + data.name;
        newGo.SetActive(true);
        NodeMono newNode = newGo.AddOrGetComponent<NodeMono>();
        newNode.data = data;
        newNode.UpdateDisplays();

        //newGo.transform.localPosition = newNodePlane.offerVacantLocalPosition(newNode);
        //newGo.transform.localRotation = Quaternion.identity;
        location.AdoptNode(newNode);

        //TODO created nodes should be completely substituted by nodesByUID
      //  createdNodes.Add(newNode);


        nodesByUID.Add(newNode.data.uid, newNode);

        return newNode;
        // position the node on timeline
    
    }

    public Timeline CreateTimeline(TimeFrame timeframe, Vector3 position, List<NodeMono> nodesToAdopt = null)
    {
        GameObject newGO = Instantiate(timelineTemplate, nodesContainer);
        newGO.transform.position = position;

        //TODO how to decide timeline orientation?
        newGO.transform.localRotation = Quaternion.identity;

        newGO.name = "---> TIMELINE";
        newGO.SetActive(true);

        Timeline timeline = newGO.AddOrGetComponent<Timeline>();
        timeline.timeFrame = timeframe;
        

        if (nodesToAdopt != null) { timeline.AdoptNodes(nodesToAdopt); }

        return timeline;
    }


    public NodeList2D CreateNewNodeList(string label, Vector3 globalPosition, List<NodeMono> nodes=null, int layout = 1, Transform parent = null, bool labelsShiftedLeft = false)
    {
        if (parent == null) { parent = this.transform; }
        GameObject newGO = Instantiate(nodeListTemplate, parent);
        newGO.transform.position = globalPosition;
        newGO.transform.localRotation = Quaternion.identity;
        newGO.name = "[ ] " + label;
        newGO.SetActive(true);

        if (!newGO.GetComponent<NodeList2D>()) { Debug.Log("NodeList2D component not found on instantiated template GameObject"); }
        NodeList2D nodeList = newGO.GetComponent<NodeList2D>();
        nodeList.labelText.text = label;
        nodeList.layout = layout;
        nodeList.labelsShiftedLeft = labelsShiftedLeft;

        if (nodes != null)
        {
            nodeList.AdoptNodes(nodes);
        }
        return nodeList;
    }

    public void showNewNodeForm()
    {

        Vector3 globalPosition = Vector3.zero;//transform.TransformPoint(camera.transform.localPosition + new Vector3(0, 0, 50));
        NodeForm form = makeNodeForm(globalPosition);
        form.MoveTowards(camera.hudPlane.transform.position);
        form.ObjectLookAt(camera.transform.position);
        form.transform.parent = camera.hudPlane.transform;


        
       
    }

    private NodeForm makeNodeForm(Vector3 globalPosition)
    {
        GameObject newGO = Instantiate(nodeFormTemplate, this.transform);
        newGO.name = "nodeEditor";
        newGO.transform.position = globalPosition;
        newGO.SetActive(true);

        if (newGO.GetComponent<NodeForm>() == null) { Debug.Log("nodeForm component not found"); }
        NodeForm form = newGO.GetComponent<NodeForm>();
        form.graph = this;
        return form;
    }


    public NodeMono GetNodeByUID(string uid)
    {
     


        if (nodesByUID.ContainsKey(uid))
        {
            return nodesByUID[uid];
        }
        else { return null; }


        //List<NodeMono> matches = new List<NodeMono>();

        //    //foreach(NodeMono node in createdNodes)
        //    //{
        //    //    if(node.data.uid == uid)
        //    //    {
        //    //        matches.Add(node);             
        //    //    }
        //    //}

        //    if (matches.Count == 0)
        //    {
        //        return null;
        //    }
        //    if (matches.Count > 1)
        //    {
        //        Debug.LogError($"somehow ... id {uid} appears in {matches.Count} nodes");
        //        matches.Print();

        //    }
        //    return matches[0];   
     }

    public EdgeMono GetEdgeByUID(string uid)
    {
            if (edgesByUID.ContainsKey(uid))
            {
                return edgesByUID[uid];
            }
            else { return null; }
        }


    public NodeMono[] GetNodesByName(string name, string type=null)
    {
        List<NodeMono> matches = new List<NodeMono>();

        foreach (var kvp in nodesByUID)
        {
            NodeMono node = kvp.Value;
            if(node.data.name == name)
            {
                if (type !=null)
                {
                    if(node.data.type == type)
                    {
                        matches.Add(node);
                    }
                }
                else
                {
                    matches.Add(node);
                }      
            }
        }
        return matches.ToArray();
    }


    //public EdgeMono GetEdgeByUID(string uid)
    //{
    //    List<EdgeMono> matches = new List<EdgeMono>();

    //    foreach(EdgeMono edge in createdEdges)
    //    {
    //        if(edge.data.uid == uid)
    //        {
    //            matches.Add(edge);
    //        }
    //    }
    //    if (matches.Count == 0)
    //    {
    //        return null;
    //    }
    //    if (matches.Count > 1)
    //    {
    //        Debug.LogError($"somehow ... id {uid} appears in {matches.Count} edges");
    //        matches.Print();
    //    }

    //    return matches[0];

    //}

    public string MakeNewUID(string readablePart = "")
    {
        string proposition = null;
        bool uniquenessConfirmed = false;

        while (!uniquenessConfirmed)
        {
            proposition =  GenrateRandomNumberString(15) + readablePart;
            uniquenessConfirmed = isThisIDunique(proposition);            
        }
        return proposition;
    }


    public bool isThisIDunique(string id)
    {
        if(id.Length<10 ^ id == null) { return false; }
        if (GetNodeByUID(id) != null) { return false; }
        if (GetEdgeByUID(id) != null) { return false; }
        return true;
    }

    string GenrateRandomNumberString(int length)
    {
        string output = "";
        for(int i = 0; i < length; i += 1)
        {
            output = output + Random.Range(0, 9).ToString();
        }
        if (output.Length != length) { Debug.LogError("code generates wrong number of numbers"); }
        return output;
    }


    //public bool isIDunique(string id)
    //{
    //    foreach(NodeMono node in createdNodes)
    //    {
    //        if(node.data.uid == id)
    //        {
    //            Debug.Log("conflict detected. use different id" + node.data.name);
    //            return false;
    //        }
    //    }

    //    //TODO the same for edges

    //    return true;
    //}



    [ExposeMethodInEditor]
    public void minimizeAll()
    {

        foreach (var kvp in nodesByUID)
        {
            kvp.Value.Minimize();
        }

        //        foreach(NodeMono node in createdNodes)
        //        {
        //            node.Minimize();
        ////            node.UpdateColliderSize();
        //        }
    }

    [ExposeMethodInEditor]
    public void maximizeAll()
    {

        foreach(var kvp in nodesByUID)
        {
            kvp.Value.Maximize();
        }

 //       foreach(NodeMono node in createdNodes)
 //       {
 //           node.Maximize();
 ////           node.UpdateColliderSize();
 //       }
    }


    // move this to new visual preferences script?
    [ExposeMethodInEditor]
    public void resetAllCanvasResolutions()
    {

        UnityEngine.UI.CanvasScaler[] canvases = GetComponentsInChildren<UnityEngine.UI.CanvasScaler>();
        foreach(UnityEngine.UI.CanvasScaler canvas in canvases)
        {
            canvas.dynamicPixelsPerUnit = 5;
        }


    }

}
