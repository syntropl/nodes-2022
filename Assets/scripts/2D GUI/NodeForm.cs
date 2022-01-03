using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtentionMethods;

public class NodeForm : MonoBehaviour
{
    public Graph graph;

    public Text label;
    public InputField nameField;
    public InputField descriptionField;
    public InputField typeField;
    public InputField tagsField;

    public Text message;

    bool showMessage = false;

    public List<Transform> navigableObjects;

    //public bool keepLookingAtCamera = true;



    // TODO: date selector



    //TODO tab and shift+tab navigation
    // getNavigableComponents to a list

    // TODO later: move below part to inputHandler
    // inputHandler - ? type ? elementInFocus
    // if tab blabla
    // if shift and tab - blabla



    //TODO 

    public NodeData NodeDataFromForm()
    {

        NodeData newData = new NodeData();
        newData.name = nameField.text;
        newData.description = descriptionField.text;
        newData.type = typeField.text;
        //newData.tags = ValidateTags(tagsField.text);
        
        return newData;
    }



    public void Commit()
    { 

        NodeDataFromForm().Print();
        graph.CreateNewNodeFromForm(this);
    }


    void Start()
    {
        //graph = this.GetGraph();
        //Debug.Log(graph.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (showMessage)
        {
            message.gameObject.SetActive(true);
        }

        //if (keepLookingAtCamera)
        //{
        //    this.SuddenLookAt(graph.camera.transform.position);
        //}

    }

}
