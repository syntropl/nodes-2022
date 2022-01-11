using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtentionMethods;

public class TextPanel : MonoBehaviour
{

    Image panel;
    public Text text;

    public string textString
    {
        get { return text.text; }
        set
        {
            text.text = value;
            UpdateTextTransform();
            UpdatePanelHeight();        
        }
    }

    float textLeftMargin = 5;
    //float textRightMargin= 5;
    float textBottomMargin = 10;
    float textTopMargin= 5;


    public void MimicStyle(TextPanel original)
    {
        panel.color = original.panel.color;
        text.color = original.text.color;

    }



    public void ApplyColors(Color panelColor, Color textColor)
    {
        panel.color = panelColor;
        text.color = textColor;
    }


    float fontToWorldSizeFactor = 1;
    float FontHeight
    {
        get {
            Debug.Log($"fontToWorldSizeFactor = {fontToWorldSizeFactor}");
            return text.fontSize * fontToWorldSizeFactor; }

        set { text.fontSize = (int)(value / fontToWorldSizeFactor); }
    }



    [ExposeMethodInEditor]
    public void UpdatePanelHeight()
    {
        LayoutElement panelLayoutElement = panel.GetComponent<LayoutElement>();
        float textHeight = text.preferredHeight;
        panelLayoutElement.preferredHeight = textHeight + textBottomMargin + textTopMargin;
        if(textString == "") { panelLayoutElement.preferredHeight = 0; }
    }


    [ExposeMethodInEditor]
    public void UpdateTextTransform()
    {   
        RectTransform rect = text.rectTransform;
        rect.sizeDelta = new Vector2(rect.GetWidth(), text.preferredHeight);
        text.rectTransform.localPosition = new Vector2(textLeftMargin, -1 * textTopMargin);
    }



    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        if(text == null) { Debug.Log(this.name + " text component not found in child"); }

        panel = GetComponent<Image>();
        if (panel == null) { Debug.Log(this.name + " panel component not found in child"); }
    }

}
