using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base interface for all the UI params
/// </summary>
public interface IUIParams { }

public interface IPanelParams : IUIParams
{

}

public class PanelParams : IPanelParams
{

}

public class ConfirmPopupParam : PanelParams
{
    public string title;
    public string msg;
    public string confirmText;
    public string cancelText;
    public bool hasCancel;
    public UnityAction confirmMethod;
    public UnityAction cancelMethod;

    /// <summary>
    /// 确认窗口参数的构造方法。
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="msg">文本内容</param>
    /// <param name="confirmText">确认按钮（左起第一个按键）的文本</param>
    /// <param name="cancelText">取消按钮（左起第二个按键）的文本（默认为空）,若为空，则没有取消按钮</param>
    /// <param name="confirmMethod">点按确认按键会调用的方法</param>
    /// <param name="cancelMethod">点按取消按键会调用的方法（默认为空）</param>
    public ConfirmPopupParam(string title, string msg, string confirmText, string cancelText = null, UnityAction confirmMethod = null, UnityAction cancelMethod = null)
    {
        this.title = title;
        this.msg = msg;
        this.confirmText = confirmText;
        this.cancelText = cancelText;
        this.hasCancel = !String.IsNullOrEmpty(cancelText);
        this.confirmMethod = confirmMethod;
        this.cancelMethod = cancelMethod;

    }
}