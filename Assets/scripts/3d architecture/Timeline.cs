using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ExtentionMethods;

// if this gets slow - remove redundant use of GetChildrenNodes and store them inside AdoptNodes();

public class Timeline : MonoBehaviour
{

    //TIME

    public TimeFrame timeFrame;
    const long ticksInADay = 864000000000;

    // SPACE
    public MeshRenderer mesh;


    public Transform testStart;
    public Transform testEnd;
    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;


    public bool useDayLists = true;
    float nodeSize = 350f; // when useDayLists == false

    [ExposeMethodInEditor]
    void PrintChildrenNodes()
    {
        this.GetChildrenNodes().Print();
    }


    public void AdoptNodes(List<NodeMono> nodesToAdopt)
    {
        RemoveUndatedNodes(nodesToAdopt);
        NodeMono[] children = this.GetChildrenNodes();
        if (children!= null) { nodesToAdopt.AddRange(children);}
        UpdateTimeFrameToFit(nodesToAdopt);
        UpdateDimensions();

        foreach(NodeMono node in nodesToAdopt)
        { node.transform.SetParent(this.transform); }

        if (useDayLists) { CreateAndPopulateDayLists(); }
        else { DistributeChildrenNodesOnZaxis(); }
    }


    void UpdateDimensions()
    {
        float timeAxisLength = nodeSize * timeFrame.DaySpan;
        startLocalPosition = new Vector3(0, 0, 0);
        endLocalPosition = new Vector3(0, 0, timeAxisLength);

        if (mesh)
        {
            mesh.transform.localScale = new Vector3(2, 2, timeAxisLength);
            mesh.transform.localPosition = new Vector3(0, 0, timeAxisLength / 2);
            if (testStart) { testStart.localPosition = startLocalPosition; }
            if (testEnd) { testEnd.localPosition = endLocalPosition; }
        }

    }

    void CreateAndPopulateDayLists()
    {
        NodeMono[] nodes = this.GetChildrenNodes();
        for (int dayIndex = 0; dayIndex <= timeFrame.DaySpan; dayIndex++)
        {

            //REPRESENTING EACH DAY OF THE TIMELINE'S DAYSPAN AS TIMEFRAME

            long ticksFromFrameStart = dayIndex * ticksInADay;
            DateTime dayStart = new DateTime(timeFrame.start.Ticks + ticksFromFrameStart);
            DateTime dayEnd = new DateTime(dayStart.Ticks + ticksInADay - 1);
            TimeFrame dayFrame = new TimeFrame(dayStart, dayEnd);

            // FINDING NODES WITH DUE DATE SET TO CURRENTLY CONSIDERED DATE

            List<NodeMono> dayNodes = new List<NodeMono>();

            foreach (NodeMono node in nodes)
            {
                if (node.data.due_on.Date == dayStart.Date)
                {
                    dayNodes.Add(node);
                }
            }

            // CREATING AND POPULATING node lists FOR EACH NON-EMPTY DAY

            if (dayNodes.Count > 0)
            {
                NodeList2D dayList = AddOrGetDayList(dayFrame, dayNodes);
                dayList.isOnAxis = true;    // this will offset label to the left

            }

        }
    }

    NodeList2D AddOrGetDayList(TimeFrame dayFrame, List<NodeMono> dayNodes)
    {
        string label = dayFrame.DateString;
        NodeList2D[] dayLists = GetComponentsInChildren<NodeList2D>();
        foreach(NodeList2D dayList in dayLists)
        {
            if (dayList.labelText.text == dayFrame.DateString)
            {
                dayList.AdoptNodes(dayNodes);
                return dayList;
            }
        }

        Vector3 position = GetDateGlobalPosition(dayFrame.start);

        return this.GetGraph().CreateNewNodeList(label, position, dayNodes, 1, this.transform,true);
    } 

    void DistributeChildrenNodesOnZaxis()
    {
        NodeMono[] nodes = this.GetChildrenNodes();
        if (nodes== null) { return; }

        foreach(NodeMono node in nodes)
        {
            node.MoveTowards(GetDateGlobalPosition(node.data.due_on));
        }
    
    }

    Vector3 GetDateGlobalPosition(DateTime dateTime)
    {
        long deltaTicks = dateTime.Ticks - timeFrame.start.Ticks;
        double lerpZ = (double)deltaTicks / (double)timeFrame.Ticks;

        Vector3 endGlobalPos = transform.TransformPoint(endLocalPosition);

        return Vector3.Lerp(transform.position, endGlobalPos, (float)lerpZ);

    }


    void UpdateTimeFrameToFit(List<NodeMono> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            NodeMono node = nodes[i];
            DateTime date = node.data.due_on;
            if (i == 0) { timeFrame = new TimeFrame(date, date); }
            else if (date < timeFrame.start) { timeFrame.start = date; }
            else if (date > timeFrame.end) { timeFrame.end = date; }
        }
    }

    List<NodeMono> RemoveUndatedNodes(List<NodeMono> nodeList)
    {

        List<NodeMono> undatedNodes = new List<NodeMono>();
        foreach (NodeMono node in nodeList)
        {
            if (!(node.data.due_on.Ticks > 0))
            {
                undatedNodes.Add(node);
            }
        }
        foreach (NodeMono undated in undatedNodes)
        {
            nodeList.Remove(undated);
        }
        Debug.Log($"{nodeList.Count} nodes with dates found");
        return nodeList;
    }
}
