using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;
using UnityEngine.UI;

public class NodeList2D : MonoBehaviour
{
    public int layout = 1;
    public Text labelText;

    float labelXoffset = 0;
    private bool _isOnAxis;
    public bool isOnAxis
    {
        get { return _isOnAxis; }
        set
        {
            if (value==true) { labelXoffset = -100; }
            else { labelXoffset = 0; }
            _isOnAxis = value;
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
        UpdatePositions();
    }


    void LineUpRow()
    {


        NodeMono[] nodes = this.GetChildrenNodes();
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


        foreach (NodeMono node in nodes)
        {
            Vector3 localPos = new Vector3(xPosRelative, yPosRelative, 0);
            Vector3 newGlobalPosition = transform.TransformPoint(localPos);

            node.MoveTowards(newGlobalPosition, 50);
            xPosRelative = xPosRelative + spacing + node.panelRect.GetWidth();

        }
    }

    void LineUpColumn()
    {


        NodeMono[] nodes = this.GetChildrenNodes();

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
        

        if (nodes != null)
        {
            foreach (NodeMono node in nodes)
            {
                Vector3 newGlobalPosition = transform.TransformPoint(new Vector3(0, yPosRelative, 0));
                node.MoveTowards(newGlobalPosition);
                yPosRelative = yPosRelative - node.panelRect.GetHeight() + spacing;

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
