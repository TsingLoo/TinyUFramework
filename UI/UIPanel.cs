using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eUIPanelType
{
    BasePanel = 0,
    SettingPanel,
    ConfirmPopupPanel,
    StartMenu,
    PauseMenu,
    LoadingPanel,
    ConfigPanel,
    HUDPanel,
    HelpPanel,
    AboutPanel
}

public class UIPanelJson
{
    public eUIPanelType UIPanelType;
    public string UIPanelPath;

    public override string ToString()
    {
        return ("The eUIPanelType is " + UIPanelType + " UIPanelPath is " + UIPanelPath);
    }
}
