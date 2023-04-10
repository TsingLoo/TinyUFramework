using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

 namespace TinyUFramework {	
	public class PauseMenu : BasePanel
	{
	    [SerializeField] GameObject AudioControlPanel;   
	    [SerializeField] GameObject ResumeBtn;
	    [SerializeField] GameObject RestartBtn;
	    [SerializeField] GameObject HomeBtn;
	 
	    [SerializeField] Image BackgroundImg;
	
	
	    public override void OnEnter()
	    {
	        BindBtns();
	        AudioControlPanel.SetActive(true);
	        base.OnEnter();
	        BackgroundImg.DOColor(new Color(1f, 1f, 1f, 0.7f), 0.4f).SetEase(Ease.OutCubic).SetUpdate(true);
	    }
	
	    public override void OnExit()
	    {
	        AudioControlPanel.SetActive(false);
	        UnBindBtns();
	        BackgroundImg.DOColor(new Color(0f, 0f, 0f, 0f), 0.2f).SetUpdate(true);
	        base.OnExit();
	    }
	
	
	    void BindBtns()
	    {
	        //MainController.Instance.resumeGame += ResumeHandler;
	        //ResumeBtn.GetComponent<Button>().onClick.AddListener(()=> { MainController.Instance.ResumeGame();});
	        //RestartBtn.GetComponent<Button>().onClick.AddListener(() => { MainController.Instance.RestartGame();});
	        HomeBtn.GetComponent<Button>().onClick.AddListener(() => {
	            PanelManager.Instance.PopPanel();
	            if (PanelManager.Instance.GetTopPanel().GetPanelType() == eUIPanelType.HUDPanel)
	            {
	                PanelManager.Instance.PopPanel();
	                //MainController.Instance.ExitPlay();
	            }
	            DOTween.Kill("StartToRespawnIPD");
	            PanelManager.Instance.OpenPanel(eUIPanelType.StartMenu);
	        });
	        //StartBtn.GetComponent<Button>().onClick.AddListener(OnClickStart);
	        //HelpBtn.GetComponent<Button>().onClick.AddListener(MainController.Instance.OnClickHelp);
	        //QuitBtn.GetComponent<Button>().onClick.AddListener(MainController.Instance.OnClickQuit);
	    }
	
	    void UnBindBtns()
	    {
	        ResumeBtn.GetComponent<Button>().onClick.RemoveAllListeners();
	        RestartBtn.GetComponent<Button>().onClick.RemoveAllListeners();
	        HomeBtn.GetComponent<Button>().onClick.RemoveAllListeners();
	        //MainController.Instance.resumeGame -= ResumeHandler;
	        //StartBtn.GetComponent<Button>().onClick.RemoveAllListeners();
	        //HelpBtn.GetComponent<Button>().onClick.RemoveAllListeners();
	        //QuitBtn.GetComponent<Button>().onClick.RemoveAllListeners();
	    }
	
	    void ResumeHandler() 
	    {
	        //��ǰ�������ʾ֮�У��ֶ��ر�
	        if (canvasGroup.alpha == 1)
	        {
	            Debug.LogWarning ($"[{nameof(PauseMenu)}]Error Show, invisible by hand");
	            canvasGroup.alpha = 0;
	            canvasGroup.blocksRaycasts = false;
	        }
	    }
	
	    void OnClickQuit()
	    {
	        PanelManager.Instance.OpenPanel(eUIPanelType.ConfirmPopupPanel, new ConfirmPopupParam(
	                "CONFIRM", "Confirm to QUIT?", "QUIT", "CANCEL", () =>
	                {
#if UNITY_EDITOR
	                    UnityEditor.EditorApplication.isPlaying = false;
#else
	        Application.Quit();
#endif 
	                }
	            ));
	    }
	
	
	
	
	}
}
