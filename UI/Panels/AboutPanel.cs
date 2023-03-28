using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutPanel : BasePanel
{
    [SerializeField] GameObject BackBtn;

    void OnEnable()
    {
        BackBtn.GetComponent<Button>().onClick.AddListener(OnClickBackBtn);
    }

    void OnClickBackBtn() 
    {
        PanelManager.Instance.PopPanel();
        PanelManager.Instance.OpenPanel(eUIPanelType.StartMenu);
    }
}


