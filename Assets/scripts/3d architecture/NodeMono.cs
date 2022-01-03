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
    public string description;
    public DateTime due_on;
    public List<string> tags;


    public override string ToString()
    {
        return $" {type} { name}  { due_on.ToShortDateString()}";
    }
    public void Print()
    {
        Debug.Log(this.ToString());
    }
}

public class NodeMono : MonoBehaviour
{

    public NodeData data;

    public Text nameText;
    public Text tagsText;
    public Text typeText;
    public Text descriptionText;
    public Text dateText;

    public Collider nodeCollider;
    public RectTransform panelRect;

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


    public void UpdateDisplays()
    {
        typeText.text = data.type;
        nameText.text = data.name;
        descriptionText.text = data.description;
        dateText.gameObject.SetActive(false);


        UpdateColliderSize();
    }

    [ExposeMethodInEditor]
    public void UpdateColliderSize()
    {
        
        float width  = panelRect.GetWidth() + 40;
        float height = panelRect.GetHeight();
        float depth = 1f;

        nodeCollider.transform.position = panelRect.position;
        nodeCollider.transform.localScale = new Vector3(width, height, depth);
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




    void Start()
    {

  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //internal void OrientToFace(GameObject gameObject)
    //{
    //    throw new NotImplementedException();
    //}

    //public void MoveTo(Vector3 destinationWorldPosition, float speed= 0.1f)
    //{
    //    //StartCoroutine(MoveTowards(destinationWorldPosition, speed));
    //    StartCoroutine(MoveDamp(destinationWorldPosition, 0.3f));
    //}

    //IEnumerator MoveTowards(Vector3 destination, float step_size)
    //{

    //    Vector3 origin = transform.position;

        
    //    for (float pos = 0; pos < 1; pos += step_size)
    //    {

    //        transform.position = Vector3.Lerp(origin, destination, pos);
    //        yield return null;
    //    }
          
    //}

    //IEnumerator MoveDamp(Vector3 destination, float smoothTime, float accuracy=0.1f)
    //{
    //    Vector3 vel = Vector3.zero;

    //   while(Vector3.Distance(transform.position, destination) > accuracy)
    //    {
    //        transform.position = Vector3.SmoothDamp(transform.position, destination, ref vel, smoothTime);
    //        yield return null;
    //    }
    //}



    [ExposeMethodInEditor]
    public void Print()
    {
        Debug.Log($"{transform.position} {data.ToString()}");
    }
}
