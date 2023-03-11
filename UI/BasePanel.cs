using Unity.VisualScripting;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public eUIPanelType panelType;
    private CanvasGroup canvasGroup;
    protected IPanelParams panelParams;

    public void Awake()
    {

        canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

    }
    public virtual void OnEnter()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        //UIManager.Instance.PushPanel(GetPanelType());
    }

    public virtual void Set()
    {

    }

    public virtual void OnPause()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = false;
    }

    public virtual void OnResume()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void OnExit()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        //UIManager.Instance.PopPanel();
    }

    public eUIPanelType GetPanelType()
    {
        return panelType;
    }

    public void SetPanelParam(IPanelParams panelParams)
    {
        if (panelParams != null)
        {
            if (panelParams is IPanelParams)
            {
                this.panelParams = panelParams;
            }
            else
            {
                Debug.LogError("[UIManager]Params passed have wrong type! (" + panelParams.GetType() + " instead of " +
                               typeof(IUIParams) + ")");
                return;
            }
        }
        this.panelParams = panelParams;
    }
}