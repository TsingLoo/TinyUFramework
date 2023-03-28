using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : BasePanel
{
    [SerializeField] GameObject AudioControlPanel;   
    [SerializeField] GameObject ResumeBtn;
    [SerializeField] GameObject RestartBtn;

    [SerializeField] Image BackgroundImg;

    private void OnEnable()
    {
    }

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
        ResumeBtn.GetComponent<Button>().onClick.AddListener(()=> { MainController.Instance.ResumeGame();});
        RestartBtn.GetComponent<Button>().onClick.AddListener(() => { MainController.Instance.RestartGame();});
        //StartBtn.GetComponent<Button>().onClick.AddListener(OnClickStart);
        //HelpBtn.GetComponent<Button>().onClick.AddListener(MainController.Instance.OnClickHelp);
        //QuitBtn.GetComponent<Button>().onClick.AddListener(MainController.Instance.OnClickQuit);
    }

    void UnBindBtns()
    {
        ResumeBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        RestartBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        //MainController.Instance.resumeGame -= ResumeHandler;
        //StartBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        //HelpBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        //QuitBtn.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void ResumeHandler() 
    {
        //当前面板在显示之中，手动关闭
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
