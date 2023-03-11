using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eUIPanelType
{
    BasePanel = 0,
    SettingPanel,
    ConfirmPopupPanel,
    LaunchPanel,
    LoadingPanel,
    ConfigPanel
}

public class UIPanel
{
    public eUIPanelType UIPanelType;
    public string UIPanelPath;

    public override string ToString()
    {
        return ("The eUIPanelType is " + UIPanelType + " UIPanelPath is " + UIPanelPath);
    }
}
