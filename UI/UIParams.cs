using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

 namespace TinyUFramework {	
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
	    public Color msgColor;
	    public string confirmText;
	    public string cancelText;
	    public bool hasCancel;
	    public UnityAction confirmMethod;
	    public UnityAction cancelMethod;
	
	    /// <summary>
	    /// ȷ�ϴ��ڲ����Ĺ��췽����
	    /// </summary>
	    /// <param name="title">����</param>
	    /// <param name="msg">�ı�����</param>
	    /// <param name="confirmText">ȷ�ϰ�ť�������һ�����������ı�</param>
	    /// <param name="cancelText">ȡ����ť������ڶ������������ı���Ĭ��Ϊ�գ�,��Ϊ�գ���û��ȡ����ť</param>
	    /// <param name="confirmMethod">�㰴ȷ�ϰ�������õķ���</param>
	    /// <param name="cancelMethod">�㰴ȡ����������õķ�����Ĭ��Ϊ�գ�</param>
	    public ConfirmPopupParam(string title, string msg ,Color msgColor,  string confirmText, string cancelText = null, UnityAction confirmMethod = null, UnityAction cancelMethod = null)
	    {
	        this.title = title;
	        this.msg = msg;
	        this.msgColor = msgColor;
	        this.confirmText = confirmText;
	        this.cancelText = cancelText;
	        this.hasCancel = !String.IsNullOrEmpty(cancelText);
	        this.confirmMethod = confirmMethod;
	        this.cancelMethod = cancelMethod;
	    }
	
	    public ConfirmPopupParam(string title, string msg, string confirmText, string cancelText = null, UnityAction confirmMethod = null, UnityAction cancelMethod = null)
	    {
	        this.title = title;
	        this.msg = msg;
	        this.msgColor = Color.black;
	        this.confirmText = confirmText;
	        this.cancelText = cancelText;
	        this.hasCancel = !String.IsNullOrEmpty(cancelText);
	        this.confirmMethod = confirmMethod;
	        this.cancelMethod = cancelMethod;
	    }
	}
}
