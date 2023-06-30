using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class ClickRichText : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] TextMeshProUGUI text;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, pos, null);
        //int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, pos, Camera.main);
        if (linkIndex > -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            Debug.Log($"[{nameof(ClickRichText)}] {linkInfo.GetLinkText()}");
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}