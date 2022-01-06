using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;
using UnityEngine.UI;

public class NodeList2D : MonoBehaviour 
{


    public int layout = 1;
    public Text labelText;

    public int LabelSize
    {
        get { return labelText.fontSize; }
        set { labelText.fontSize = value; }
    }

    public float LabelWidth
    {
        get { return labelText.rectTransform.GetWidth(); }
    }

    float labelXoffset = 0;
    private bool _labelsShiftedLeft;
    public bool labelsShiftedLeft
    {
        get { return _labelsShiftedLeft; }
        set
        {
            if (value==true) { labelXoffset = -100; }
            else { labelXoffset = 0; }
            _labelsShiftedLeft = value;
        }
    }

    float marginBelowLabel = 0f;
    
    float spacing = 20f;

    public bool anchorOnFirstNode = true;

    // TODO naucz sie uzywac sort by 


    public void AdoptNodes(List<NodeMono> nodes)
    {
        foreach(NodeMono node in nodes)
        {
            AdoptNode(node);
            //TODO hide edges with children
        }
        //UpdatePositions();
    }

    [ExposeMethodInEditor]
    public void PrintNodes()
    {
        this.GetChildrenNodes().Print();
    }


    public void AdoptNode(NodeMono node)
    {
        node.transform.SetParent(this.transform);
        node.transform.localRotation = Quaternion.identity;

//        Debug.Log($"{node.name} is now child of {node.transform.parent}");
        UpdatePositions();
    }


    void LineUpRow()
    {


        NodeMono[] activeNodes = this.GetChildrenNodes(false);
        float xPosRelative = 0;
        float yPosRelative = 0f;
        float labelYoffset = labelText.rectTransform.GetHeight() + marginBelowLabel;

        if (anchorOnFirstNode)
        {
            labelText.transform.localPosition = new Vector3(labelXoffset, labelYoffset, 0);
        }
        else
        {

            yPosRelative = -1 * labelYoffset;
        }

        if (activeNodes != null)
        {
            foreach (NodeMono node in activeNodes)
            {
                Vector3 localPos = new Vector3(xPosRelative, yPosRelative, 0);
                Vector3 newGlobalPosition = transform.TransformPoint(localPos);

                node.MoveTowards(newGlobalPosition, 50);
                xPosRelative = xPosRelative + spacing + node.labelPanelRect.GetWidth();

            }
        }
        //else
        //{
        //    Debug.Log($"{gameObject.name} has no active children nodes");
        //}

    }

    void LineUpColumn()
    {


        NodeMono[] activeNodes = this.GetChildrenNodes(false);

        float labelYoffset = labelText.rectTransform.GetHeight() + marginBelowLabel;
        float yPosRelative = 0f;


        if (anchorOnFirstNode)
        {
            labelText.transform.localPosition = new Vector3(0,labelYoffset,0);
        }
        else
        {
            
            yPosRelative = -1 * labelYoffset;
        }
        

        if (activeNodes != null)
        {
            foreach (NodeMono node in activeNodes)
            {
                Vector3 newGlobalPosition = transform.TransformPoint(new Vector3(0, yPosRelative, 0));
                node.MoveTowards(newGlobalPosition);
                yPosRelative = yPosRelative - node.labelPanelRect.GetHeight() + spacing;

                //Debug.Log($"newGlobalPosition = {newGlobalPosition}");
            }
        }

    }


    [ExposeMethodInEditor]
    public void UpdatePositions()
    {
        switch (layout)
        {
            case 0: //COLUMN
                //Debug.Log("column not implemented");
                LineUpColumn();
                break;

            case 1: //ROW
                LineUpRow();
                break;

            case 3: //POLYGON

                //TODO
                Debug.LogError("polygon node list not implemented");

                break;
        }
    }

}
