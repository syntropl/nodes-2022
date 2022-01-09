using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtentionMethods;

//TODO TEST CASE idk what when task is in multiple projects

public class ReadAsanaProject : MonoBehaviour
{
    public TextAsset asanaProjectJson;
    public AsanaTask[] asanaTasks;
    AsanaProjectMembership asanaProjectMembership;
    List<string[]> mentions;
    public Vector3 projectGlobalPosition;

    Graph graph;

    private void Start()
    {
        

        ImportTasks();


        //ConnectTasksToProjects();

        foreach (AsanaTask task in asanaTasks)
        {
            ConnectTaskToProjectsAndSections(task);
        }



        RefreshListsHook();
    }



    [ExposeMethodInEditor]
    public void ConnectTaskToProjectsAndSections(AsanaTask task)
    {
        string projectUID = "";
        List<NodeMono> sectionNodes = new List<NodeMono>();
        foreach (AsanaMembership memberhsip in task.memberships)
        {

            // this if maybye later?
            if (memberhsip.project != null)
            {
                NodeList2D projectList = AddNodeToGroup2D(task.gid, memberhsip.project.gid, memberhsip.project.name, "project", 1);
                projectList.LabelSize = 80;
                projectList.labelText.rectTransform.sizeDelta = new Vector2(700, 300);
                //TODO copy type string from the list nodemono
                // btw nodepanel position is wrong
                projectUID = memberhsip.project.gid;

            }

            if (memberhsip.section != null)
                
            {
                NodeList2D sectionList = AddNodeToGroup2D(task.gid, memberhsip.section.gid, memberhsip.section.name, "section",1);
                sectionList.LabelSize = 40;
                sectionList.labelText.rectTransform.sizeDelta = new Vector2(300, 300);
                //TODO copy type string from the list nodemono
                // btw nodepanel position is wrong
                sectionNodes.Add(graph.GetNodeByUID(memberhsip.section.gid));

                NodeList2D projectList = AddNodeToGroup2D(memberhsip.section.gid, projectUID, "ERROR", "project", 0);
            }


            // TODO if task is subtask, put it on column list

            // TODO if task has tag, link to or create tag node



        }


        
    }

   
    NodeList2D AddNodeToGroup2D(string nodeUID, string groupUID, string groupName, string groupType, int layout)
    {

        // TODO this method should be in graph

        NodeMono taskNode = graph.GetNodeByUID(nodeUID);
        NodeData groupData = new NodeData(groupUID, groupName,groupType);
        
        NodeMono groupNode = graph.LinkToExistingNodeOrCreateNew(taskNode, "is in", groupData).pairMono[1];
        groupNode.mainLayoutRect.transform.position = new Vector3(-100,0,0);
        NodeList2D groupList;
        if(groupNode.GetComponentInChildren<NodeList2D>() == null)
        {
            groupList = graph.CreateNewNodeList(groupData.name, projectGlobalPosition, null,layout, groupNode.transform, false);
            groupList.transform.localPosition = Vector3.zero;
            groupList.transform.localRotation = Quaternion.identity;
            groupNode.visibleNode.gameObject.SetActive(false);
        }
        else
        {
            groupList = groupNode.GetComponentInChildren<NodeList2D>();
        }

//        Debug.Log($"groupList.AdoptNode(taskNode){groupList.labelText.text} {taskNode}");
        groupList.AdoptNode(taskNode);

        return groupList;
       
    }

    //THis WORKS.  // THIS WILL BE SOON OBSOLETE
    [ExposeMethodInEditor]
    void ConnectTasksToProjects()
    {
        foreach (AsanaTask task in asanaTasks)
        {
            foreach (AsanaMembership memberhsip in task.memberships)
            {
                if (memberhsip.project != null)
                {
                    NodeData projectNodeData = new NodeData();
                    projectNodeData.name = memberhsip.project.name;
                    projectNodeData.uid = memberhsip.project.gid;
                    NodeMono taskNode = graph.GetNodeByUID(task.gid);
                    NodeMono projectNode = graph.LinkToExistingNodeOrCreateNew(taskNode, "is in", projectNodeData, true).pairMono[1];
                    projectNode.mainLayoutRect.gameObject.SetActive(false);
                    NodeList2D projectList;

                    if (projectNode.GetComponentInChildren<NodeList2D>() == null)
                    {
                        projectList = graph.CreateNewNodeList(projectNodeData.name, projectGlobalPosition, null, 1, projectNode.transform, true);
                        projectList.transform.localPosition = Vector3.zero;
                        projectList.transform.localRotation = Quaternion.identity;
                    }
                    else
                    {
                        projectList = projectNode.GetComponentInChildren<NodeList2D>();
                    }
                    projectList.AdoptNode(taskNode);
                    projectList.UpdatePositions();


                }
            }
        }
    }




   
    [ExposeMethodInEditor]
    public void ImportTasks() 
    {

        asanaTasks = JsonUtility.FromJson<AsanaData>(asanaProjectJson.text).data;
        mentions = new List<string[]>();

        if (!GetComponent<Graph>()) { Debug.Log("graph component not found"); }
        graph = GetComponent<Graph>();
        

        foreach (AsanaTask task in asanaTasks)
        {

            // INSTANTIATE TASK
            NodeMono taskNode = InstantiateTask(task);

            // ADD TASK DESCRIPTION
            if (task.notes != null)
            {
                // checking for mentions of other tasks inside task description
                string asanaLinkRoot = "https://app.asana.com/0/0/";
                if (task.notes.Contains(asanaLinkRoot))
                {
                    int start = task.notes.IndexOf(asanaLinkRoot) + asanaLinkRoot.Length;
                    Debug.Log($"{task.name} task.notes: {task.notes}");

                    string mentionUID = task.notes.Substring(start, 16); // if asana ever changes length of gids this will break;
                    string[] protoEdge = { mentionUID, "is referenced by", taskNode.data.uid };

                    mentions.Add(protoEdge);

                }
            }

            if (task.assignee.gid != null)
            {
                // INSTANTIATE USER as node

                Debug.Log($"instantiating user -  {task.assignee.name}");
                NodeMono userNode = InstantiateUser(task.assignee);
                graph.LinkTwoNodes(taskNode, "is assigned to:", userNode, true);
            }
            
            if (task.tags != null)
            {
            // INSTANTIATE TAGS as nodes
                foreach (AsanaTag tag in task.tags)
                {
                    NodeMono tagNode;
                    if(graph.GetNodesByName(tag.name, "tag").Length > 0)
                    {
                        NodeMono[] similarTagNodes = graph.GetNodesByName(tag.name, "tag");
                        if (similarTagNodes.Length > 1) { similarTagNodes.Print($"tagNodes found named>{tag.name}<"); }
                        tagNode = similarTagNodes[0];
                    }
                    else
                    {
                        tagNode = InstantiateTag(tag);              
                    }
                    graph.LinkTwoNodes(taskNode, "is tagged", tagNode, true);
                }

            }
        }

        // INSTANTIATE MENTIONS  as edges
        foreach(string[] mention in mentions)
        {

            NodeMono callerNode = graph.GetNodeByUID(mention[2]);
            string verb = mention[1];
            NodeMono mentionedNode = graph.GetNodeByUID(mention[0]);



            if (mentionedNode == null) { Debug.Log($"while importing task: {callerNode.data.name} I extracted a mention of <color=red>{mention[2]}</color> but do not recognize anything with this UID. check parsing during import"); }
            else
            {

                if (callerNode != null)
                {
                    Debug.Log($"<color=blue>!!! {callerNode.name}</color> mentions something");
                    string mentionURL = @"https://app.asana.com/0/0/" + mention[0];


                    string correctedDescription = callerNode.data.description.Replace(mentionURL,$"// go to:{mentionedNode.data.name}//");

                    Debug.Log($"mentionURL: {mentionURL}");
                    Debug.Log($"mentionedNode.data.name and uid: {mentionedNode.data.name} {mentionedNode.data.uid}");

                    Debug.Log($"callerNode.data.name and uid: {callerNode.data.name} {callerNode.data.uid}");
                    Debug.Log($"callerNode.data.description: {callerNode.data.description}");
                    callerNode.data.description = correctedDescription;
                    Debug.Log($"after correction task description is : {callerNode.data.description}");

                    //TODO  make it bold?
                    callerNode.UpdateDisplays();


                    graph.LinkTwoNodes(mentionedNode, "is referenced by", callerNode, true);
                }
                else
                {
                    Debug.Log($": node lost: node <color=red>{mention[2]}</color>  was supposed to mention {mention[0]}");
                }

            }
        }

    }

    NodeMono InstantiateTask(AsanaTask task)
    {
        //task.Print();

        NodeData data = new NodeData();
        data.name = task.name;
        data.type = "task";
        data.uid = task.gid;
        data.description = task.notes;
        data.status = task.completed ? "completed" : "incomplete";
        if (task.due_on.Length > 9)
            //data.due_on = System.DateTime.Parse(task.due_on.Substring(0, 10));
            data.due_on = System.DateTime.Parse(task.due_on, null, System.Globalization.DateTimeStyles.RoundtripKind);
        NodeMono taskNode = graph.CreateOrGetNode(data);

        if(data.status == "completed") { taskNode.GreyOutVisibleNode(); }
        if (task.subtasks.Length > 0)
        {
            foreach (AsanaTask subtask in task.subtasks)
            {
                NodeMono subtaskNode = InstantiateTask(subtask);
                NodeList2D projectList = AddNodeToGroup2D(subtaskNode.data.uid, taskNode.data.uid, $"{taskNode.name} subtasks", "_", 0);
                graph.LinkTwoNodes(subtaskNode, "is a subtask", taskNode, true);
            }
        }

        return taskNode;
    }


    NodeMono InstantiateUser(AsanaUser user)
    {
        NodeData data = new NodeData();
        data.name = user.name;
        data.type = "Person";
        data.uid = user.gid;

        Debug.Log($"user data created ({data.name}) ({data.uid})");

        return graph.CreateOrGetNode(data);
    }

    NodeMono InstantiateTag(AsanaTag tag)
    {
        NodeData data = new NodeData();
        data.name = tag.name;
        data.type = "tag";
        data.uid = tag.gid; 

        //Debug.Log($"data created {data.name}  {data.uid}");

        return graph.CreateOrGetNode(data);
    }

    [ExposeMethodInEditor]
    void RefreshListsHook()
    {
        Debug.Log("refreshing nodelists on events not implemented");

        foreach (NodeList2D list in GetComponentsInChildren<NodeList2D>())
        {
            list.UpdatePositions();
        }
    }

}

[System.Serializable]
public class AsanaTask
{
    public string gid;
    public string name;
    public string due_on;
    public string notes;
    public string permalink_url;
    public bool completed;
    public string completed_at;
    public AsanaTask parent;
    public AsanaUser assignee;
    public AsanaTask[] subtasks;

    public AsanaTag[] tags;
    public AsanaMembership[] memberships;

    public void Print()
    {
        // this method shows how to get to task data such as:
        // subtasks
        // assignee
        // tags
        // project and section in which this is located
        // parent of task (not tested)


        Debug.Log("__________________");

        string section = "";
        if (memberships != null)
        {
            foreach(AsanaMembership membership in memberships)
            {
                if (membership.section != null)
                {
                    section = membership.section.name;
                    Debug.Log(section);
                }
            }
            Debug.Log("__________________");

        }

        
        if(parent!= null)
        {
            Debug.Log("child of: " + parent.name);
        }


        //string proj = "";
        //if(memberships != null)
        //{
        //    foreach (AsanaMembership membership in memberships)
        //    {
        //        if (membership.project != null)
        //        {
        //            proj = membership.project.name;
        //        }
        //    }

        //}

        Debug.Log(name);
        if (assignee.name != null)
        {
            Debug.Log("assigned to " + assignee.name);
        }

        if (completed)
        {
            Debug.Log("completed at " + completed_at);
        }
        else
        {
            Debug.Log("not completed");
        }
        if (tags.Length > 0)
        {
            string tagString = "tags:";
            
            foreach (AsanaTag tag in tags)
            {
                tagString = tagString + tag.name; 
            }
            
            Debug.Log(tagString);
        }
        if (subtasks.Length > 0)
        {
            Debug.Log("subtasks:");
            foreach(AsanaTask subtask in subtasks)
            {
                Debug.Log("        [ ] " + subtask.name );
                //subtask.Print();
                
            }
        }


    }

}

[System.Serializable]
public class AsanaUser 
{
    public string gid;
    public string name;
}



[System.Serializable]
public class AsanaMembership
{   
    public AsanaProjectMembership project;
    public AsanaSectionMembership section;
}

[System.Serializable]
public class AsanaData
{
    public AsanaTask[] data;
}

[System.Serializable]
public class AsanaTag
{
    public string gid;
    public string name;
}

[System.Serializable]
public class AsanaProjectMembership
{
    public string gid;
    public string name;
}

[System.Serializable]
public class AsanaSectionMembership
{
    public string gid;
    public string name;
}


