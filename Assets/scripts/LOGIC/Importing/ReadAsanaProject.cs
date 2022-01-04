using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO TEST CASE idk what when task is in multiple projects

public class ReadAsanaProject : MonoBehaviour
{
    public TextAsset asanaProjectJson;
    public AsanaTask[] asanaTasks;
    AsanaProjectMembership asanaProjectMembership;
    //public AsanaUser[] asanaUsers;

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
        List<NodeMono> sectionNodes = new List<NodeMono>();
        foreach (AsanaMembership memberhsip in task.memberships)
        {
            if (memberhsip.project != null)
            {

                NodeList2D projectList = AddTaskToGroup(task.gid, memberhsip.project.gid, memberhsip.project.name, "project", 1);
                projectList.LabelSize = 80;
                //TODO copy type string from the list nodemono
                // btw nodepanel position is wrong

            }

            if (memberhsip.section != null)
                
            {
                NodeList2D sectionList = AddTaskToGroup(task.gid, memberhsip.section.gid, memberhsip.section.name, "section",1);
                sectionList.LabelSize = 40;
                //TODO copy type string from the list nodemono
                // btw nodepanel position is wrong
                sectionNodes.Add(graph.GetNodeByUID(memberhsip.section.gid));
            }


        }


        
    }

   
    NodeList2D AddTaskToGroup(string taskUID, string groupUID, string groupName, string groupType, int layout)
    {

        // TODO this method should be in graph

        NodeMono taskNode = graph.GetNodeByUID(taskUID);
        NodeData groupData = new NodeData(groupUID, groupName,groupType);
        
        NodeMono groupNode = graph.LinkToExistingNodeOrCreateNew(taskNode, "is in", groupData).pairMono[1];
        groupNode.panelRect.transform.position = new Vector3(-100,0,0);
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

        Debug.Log($"groupList.AdoptNode(taskNode){groupList.labelText.text} {taskNode}");
        groupList.AdoptNode(taskNode);

        return groupList;
       
    }

    //THis WORKS
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
                    projectNode.panelRect.gameObject.SetActive(false);
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




    // THIS WILL BE SOON OBSOLETE
    [ExposeMethodInEditor]
    public void ImportTasks() 
    {
        asanaTasks = JsonUtility.FromJson<AsanaData>(asanaProjectJson.text).data;

        if (!GetComponent<Graph>()) { Debug.Log("graph component not found"); }
        graph = GetComponent<Graph>();


        foreach (AsanaTask task in asanaTasks)
        {
            NodeMono taskNode = InstantiateTask(task);
            if (task.assignee.gid != null)
            {
                NodeMono userNode = InstantiateUser(task.assignee);
                graph.LinkTwoNodes(taskNode, "is assigned to:", userNode, true);
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
        if (task.due_on.Length > 9)
            //data.due_on = System.DateTime.Parse(task.due_on.Substring(0, 10));
            data.due_on = System.DateTime.Parse(task.due_on, null, System.Globalization.DateTimeStyles.RoundtripKind);
        NodeMono taskNode = graph.CreateNode(data);
        if (task.subtasks.Length > 0)
        {
            foreach (AsanaTask subtask in task.subtasks)
            {
                NodeMono subtaskNode = InstantiateTask(subtask);
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

        Debug.Log($"data created {data.name}  {data.uid}");

        return graph.CreateNode(data);
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
            string tagString = "tags: ";
            {
                foreach (AsanaTag tag in tags)
                {
                    tagString = tagString + tag.name; 
                }
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
public class AsanaUser // ? assignee?
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


