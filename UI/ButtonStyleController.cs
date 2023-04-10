using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TinyUFramework
{
    [RequireComponent(typeof(Button))]
    public class ButtonStyleController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {

        Color NormalColor = SaveDataManager.ThemeColor_G_Normal;
        Color HighlightColor = SaveDataManager.ThemeColor_G_Highlight;

        void OnEnable()
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
                Debug.Log($"[{nameof(ButtonStyleController)}]'s style has been changed");
                styleCB.normalColor = NormalColor;
            }

            if (rawCB.highlightedColor == ColorBlock.defaultColorBlock.highlightedColor)
            {
                styleCB.highlightedColor = HighlightColor;
            }

            btn.colors = styleCB;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundLoaderManager.Instance.AddFxSound(PrefabLoader.Instance.buttonEnter);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundLoaderManager.Instance.AddFxSound(PrefabLoader.Instance.buttonClick);
        }
    }
}
