using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;

public class ListingNode : NodeMono
{
    public int layout = 1;
    float labelPanelXoffset = 0;

    private bool _labelsShiftedLeft;
    public bool labelsShiftedLeft
    {
        get { return _labelsShiftedLeft; }
        set
        {
            float newX;
            float y = labelPanelRect.localPosition.y;
            float z = labelPanelRect.localPosition.z;
            

            if (value == true)
            {
                newX = -1 * labelWidth;
                Vector3 newPosition = new Vector3(newX, y, z);
                labelPanelRect.localPosition = newPosition;
            }
            else
            {
                newX = GetComponentInChildren<Canvas>().GetComponent<RectTransform>().GetWidth()/2;
                Vector3 newPosition = new Vector3(newX, y, z);
                labelPanelRect.localPosition = newPosition;

            }
                _labelsShiftedLeft = value;
        }
    }

    public RectTransform listRect;
    public float listWidth
    {
        get { return listRect.GetWidth(); }
        set { listRect.sizeDelta = new Vector2(value, listHeight); }
    }

    public float listHeight
    {
        get { return listRect.GetHeight(); }
        set { listRect.sizeDelta = new Vector2(listWidth, value); }
    }

    float marginBelowLabel = 0f;
    float childrenSpacing = 20f;

    //    public bool anchorOnFirstNode = true;

    //    // TODO naucz sie uzywac sort by 


    public void AdoptNodes(List<NodeMono> nodes)
    {
        foreach (NodeMono node in nodes)
        {
            AdoptNode(node);
            //TODO hide edges with children
        }
        //UpdatePositions();
    }

    public void AdoptNode(NodeMono node)
    {
        node.transform.SetParent(this.transform);
        node.transform.localRotation = Quaternion.identity;

        Debug.Log($"{node.name} is now child of {node.transform.parent}");
        UpdatePositions();
    }



    /// <summary>
    /// /
    /// </summary>
    public List<NodeMono> testAdoptList;

  
    private void Start()
    {
        testAdoptList = new List<NodeMono>();

    }

    [ExposeMethodInEditor]
    public void testAdopt()
    {
        AdoptNodes(testAdoptList);
    }


    /// <summary>
    /// /////
    /// </summary>
    ///

    void LineUpRow()
    {


        NodeMono[] activeNodes = this.GetChildrenNodes(false);
        float xPosRelative = 0;
        float yPosRelative = 0f;
        //float labelYoffset = labelText.rectTransform.GetHeight() + marginBelowLabel;

        //if (anchorOnFirstNode)
        //{
        //    labelText.transform.localPosition = new Vector3(labelPanelXoffset, labelYoffset, 0);
        //}
        //else
        //{

        //    yPosRelative = -1 * labelYoffset;
        //}

        if (activeNodes != null)
        {
            foreach (NodeMono node in activeNodes)
            {
                Vector3 localPos = new Vector3(xPosRelative, yPosRelative, 0);
                Vector3 newGlobalPosition = transform.TransformPoint(localPos);

                node.MoveTowards(newGlobalPosition, 50);
                xPosRelative = xPosRelative + childrenSpacing + node.labelPanelRect.GetWidth();

            }
        }
        //else
        //{
        //    Debug.Log($"{gameObject.name} has no active children nodes");
        //}

    }

    void LineUpColumn()
    {


        //NodeMono[] activeNodes = this.GetChildrenNodes(false);

        //float labelYoffset = labelText.rectTransform.GetHeight() + marginBelowLabel;
        //float yPosRelative = 0f;


        //if (anchorOnFirstNode)x
        //{
        //    labelText.transform.localPosition = new Vector3(0, labelYoffset, 0);
        //}
        //else
        //{

        //    yPosRelative = -1 * labelYoffset;
        //}


        //if (activeNodes != null)
        //{
        //    foreach (NodeMono node in activeNodes)
        //    {
        //        Vector3 newGlobalPosition = transform.TransformPoint(new Vector3(0, yPosRelative, 0));
        //        node.MoveTowards(newGlobalPosition);
        //        yPosRelative = yPosRelative - node.panelRect.GetHeight() + spacing;

        //        //Debug.Log($"newGlobalPosition = {newGlobalPosition}");
        //    }
        //}

    }


    [ExposeMethodInEditor]
    public void UpdatePositions()
    {
        switch (layout)
        {
            case 0: 
                LineUpColumn();
                break;

            case 1: 
                LineUpRow();
                break;

            case 3: //POLYGON

                //TODO
                Debug.LogError("polygon node list not implemented");

                break;
        }
    }


    [ExposeMethodInEditor]
    public void PrintNodes()
    {
        this.GetChildrenNodes().Print();
    }
}
