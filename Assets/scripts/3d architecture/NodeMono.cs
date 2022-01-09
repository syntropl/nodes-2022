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
    public List<EdgeData> edgeDataList = new List<EdgeData>();

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
        return $" {type} { name}";
    }
    public void Print()
    {
        Debug.Log(this.ToString());
        List<string> linkedNodes = new List<string>();
        foreach (EdgeData edge in edgeDataList)
        {
            linkedNodes.Add(edge.OtherThan(this).name);
        }
        linkedNodes.Print("linked nodes:");
    }
}

public class NodeMono : MonoBehaviour
{

    public TextPanel namePanel;


    public TextPanel tagsPanel;
    public TextPanel typePanel;
    public TextPanel descriptionPanel;
    public TextPanel datePanel;


    public NodeData data;
    public List<EdgeMono> edgeMonos;




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
        edgeMonos.Print();
    }


    public void AddEdge(EdgeMono edgeMono)
    {
        Debug.Log($"attempting to add edge to list on node {this.name}");

        if (edgeMono.GetOtherNode(this)) // null if edge does not remember this node
        {
            edgeMonos.Add(edgeMono);
            edgeMonos.Print($"edgemonos on {this.name}");

            //data.edgeDataList.Add(edgeMono.data);
            //data.edgeDataList.Print($"edgeDataList on { this.name}");
        }

    }

    void Awake()
    {

        edgeMonos = new List<EdgeMono>();

    }

    private void OnEnable()
    {
        foreach (EdgeMono edge in edgeMonos)
        {
            Debug.Log($"showing {edge.name}");
            edge.isHidden = false;
        }

    }

    private void OnDisable()
    {
        foreach (EdgeMono edge in edgeMonos)
        {
            if(edge != null)
            {
                Debug.Log($"hiding {edge.name}");
                edge.isHidden = true;
            }

        }

    }



    public void UpdateDisplays()
    {
        typePanel.textString = data.type;
        namePanel.textString = data.name;
        if(data.tags == null) { tagsPanel.textString = ""; }
        else { tagsPanel.textString = data.tags.ToString(); }
        descriptionPanel.textString = data.description;
        datePanel.gameObject.SetActive(false);


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



    [ExposeMethodInEditor]
    public void Print()
    {
        Debug.Log($"{data.name} {transform.position} ");
        data.Print();
    }
}
