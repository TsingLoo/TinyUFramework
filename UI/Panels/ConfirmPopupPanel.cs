using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

 namespace TinyUFramework {	
	public class ConfirmPopupPanel : BasePanel
	{
    #region Inspector
	    [SerializeField] private TextMeshProUGUI lb_title;
	    [SerializeField] private TextMeshProUGUI lb_message;
	    [SerializeField] private TextMeshProUGUI lb_confirmButton;
	    [SerializeField] private Button btn_confirm;
	    [SerializeField] private TextMeshProUGUI lb_cancelButton;
	    [SerializeField] private Button btn_cancel;
	    [SerializeField] private GameObject obj_cancelButton;
	    private UnityAction confirmMethod;
	    //private Action OnConfirmMethod;
	    private UnityAction cancelMethod;
    #endregion
	    /*
	    protected override void OnParamsSet()
	    {
	        titleLabel.text = Properties.Title;
	        messageLabel.text = Properties.Message;
	        confirmButtonLabel.text = Properties.ConfirmButtonText;
	        cancelButtonLabel.text = Properties.CancelButtonText;
	        cancelButtonObject.SetActive(Properties.CancelAction != null);
	    }
	    */
	
	    public override void OnEnter()
	    {
	        base.OnEnter();
	        //MainController.Instance.isShowingPopup= true;
	
	        ConfirmPopupParam param = (ConfirmPopupParam)base.panelParams;
	        lb_title.text = param.title;
	        lb_message.text = param.msg;
	        lb_message.color = param.msgColor;
	        lb_confirmButton.text = param.confirmText;
	        lb_cancelButton.text = param.cancelText;
	        this.confirmMethod = param.confirmMethod;
	        this.cancelMethod = param.cancelMethod;
	        btn_confirm.onClick.AddListener(OnConfirm);
	        btn_cancel.onClick.AddListener(OnCancelMethod);
	        UtilExtension.SafeSetActive(obj_cancelButton, param.hasCancel);
	    }
	
	    public void OnConfirm()
	    {
	        Debug.Log($"[{nameof(ConfirmPopupPanel)}]Confirm is pressed");
	        PanelManager.Instance.PopPanel();
	        confirmMethod?.Invoke();
	    }
	
	    public void OnCancelMethod()
	    {
	        cancelMethod?.Invoke();
	        PanelManager.Instance.PopPanel();
	    }
	
	    public override void OnExit()
	    {
	        base.OnExit();
	        //MainController.Instance.isShowingPopup = false;
	        btn_confirm.onClick.RemoveAllListeners();
	        btn_cancel.onClick.RemoveAllListeners();
	    }
	}
}
