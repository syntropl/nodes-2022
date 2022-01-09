using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ExtentionMethods;

// TODO
// node.minimize

//[System.Serializable]
//public class NodeData
//{
//    public string uid;
//    public string type;
//    public string name;
//    public string status;
//    public string description;
//    public DateTime due_on;
//    public List<string> tags;

//    public List<EdgeData> edgeDataList = new List<EdgeData>();

//    public NodeData(string Uid = null, string Name = null, string Type = null, long due_date_as_TICKS = -23, string Description=null, List<string> Tags = null)
//    {
//        uid = Uid;
//        name = Name;
//        type = Type;
//        description = Description;
//        tags = Tags;
//        if (due_date_as_TICKS >= 0) { due_on = new DateTime(due_date_as_TICKS); }
//    }

//    public override string ToString()
//    {
//        return $" {type} { name}  { due_on.ToShortDateString()}";
//    }
//    public void Print()
//    {
//        Debug.Log(this.ToString());
//        List<string> linkedNodes = new List<string>();
//        foreach (EdgeData edge in edgeDataList)
//        {
//            linkedNodes.Add(edge.OtherThan(this).name);
//        }
//        linkedNodes.Print("linked nodes:");
//    }
//}

public class NodeMonoBackup : MonoBehaviour
{

    //public TextPanel namePanel;
    public Text nameText;


    public Text tagsText;
    public Text typeText;
    public Text descriptionText;
    public Text dateText;





    public NodeData data;
    public List<EdgeMono> edgeMonos;



    // panels for highlighting specific texts
    public Image namePanel;
    public Image typePanel;
    // more panels ?

    public Collider nodeCollider;
    public Transform visibleNode;
    public RectTransform labelPanelRect;

    public Color defaultPanelColor;
    public Color defaultTextColor;

    public Color highlightedPanelColor;
    public Color highlightedTextColor;

    public Color discretePanelColor;
    public Color discreteTextColor;

    

    public float labelWidth
    {
        get { return labelPanelRect.GetWidth(); }
        set { labelPanelRect.sizeDelta = new Vector2(value, labelHeight); }
    }

    public float labelHeight
    {
        get { return labelPanelRect.GetHeight(); }
        set { labelPanelRect.sizeDelta = new Vector2(labelWidth, value); }
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
    void HighlightNamePanel()
    {
        HighlightPanel(namePanel);
    }

    [ExposeMethodInEditor]
    void UnHighlightNamePanel()
    {
        ResetPanelColors(namePanel);
    }

    void DeEmphasizePanel(Image panel)
    {
        Text text = panel.GetComponentInChildren<Text>();
        SetPanelColors(panel, discretePanelColor, text, discreteTextColor);
    }
    
    void ResetPanelColors(Image panel)
    {
        Text text = panel.GetComponentInChildren<Text>();
        SetPanelColors(panel, defaultPanelColor, text, defaultTextColor);

    }

    void HighlightPanel(Image panel)
    {
        Text text = panel.GetComponentInChildren<Text>();
        SetPanelColors(panel, highlightedPanelColor, text, highlightedTextColor);
    }


    void SetPanelColors(Image panel, Color panelColor, Text text, Color textColor)
    {
        panel.color = panelColor;
        text.color = textColor;
        Debug.Log(panel.name + " " + panel.color);
        Debug.Log(text.name + " " + text.color);
    }


    void Start()
    {

        edgeMonos = new List<EdgeMono>();
    }



    public void UpdateDisplays()
    {
        typeText.text = data.type;
        nameText.text = data.name;
        descriptionText.text = data.description;
        dateText.gameObject.SetActive(false);


        UpdateColliderSize();
    }


    //TODO show incompleted tasks greyed out?
    public void SetVisibility(float fraction = 1)
    {
        if(fraction == 0) { visibleNode.gameObject.SetActive(false);}
        else
        {
            visibleNode.gameObject.SetActive(true);
            Text[] texts = visibleNode.GetComponentsInChildren<Text>();
            foreach(Text text in texts)
            {
                text.color = new Color(0, 0, 0, fraction);
            }
            // TODO alpha of renderer material?
        }

        
    }


    [ExposeMethodInEditor]
    public void UpdateColliderSize()
    {
        
        float labelWidth  = labelPanelRect.GetWidth() + 40;
        float labelHeight = labelPanelRect.GetHeight();
        float depth = 1f;

        nodeCollider.transform.position = labelPanelRect.position;
        nodeCollider.transform.localScale = new Vector3(labelWidth, labelHeight, depth);
        // not implemented
    }

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

    public void Minimize()
    {
        typeText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        dateText.gameObject.SetActive(false);

    }

    public void Maximize()
    {
        typeText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        dateText.gameObject.SetActive(true);

    }



    [ExposeMethodInEditor]
    public void Print()
    {
        Debug.Log($"{data.name} {transform.position} ");
        data.Print();
    }
}
