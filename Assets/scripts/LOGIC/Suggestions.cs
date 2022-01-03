using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Suggestions : MonoBehaviour
{

    InputField thisField;




    private void Start()
    {
        if(GetComponent<InputField>() == null) { Debug.LogError($"Suggestions Component cannot find Input Field component on {gameObject.name}"); }
        thisField = GetComponent<InputField>();
    }
}
