using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonStyleController : MonoBehaviour
{

    Color NormalColor = SaveDataManager.ThemeColor_G_Normal;
    Color HighlightColor = SaveDataManager.ThemeColor_G_Highlight;

    private void Start()
    {
        ChangeBtnStyle();      
    }

    void ChangeBtnStyle() 
    {
        Button btn = gameObject.GetComponent<Button>();
        ColorBlock rawCB = btn.colors;
        ColorBlock styleCB = rawCB;
        if (rawCB.normalColor == ColorBlock.defaultColorBlock.normalColor)
        {
            styleCB.normalColor = NormalColor;
        }


        if (rawCB.highlightedColor == ColorBlock.defaultColorBlock.highlightedColor)
        { 
            styleCB.highlightedColor = HighlightColor;
        }


        btn.colors = styleCB;
    }
}
