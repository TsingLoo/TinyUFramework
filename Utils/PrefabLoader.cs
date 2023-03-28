using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PrefabLoader : SingletonForMonobehaviour<PrefabLoader>
{

    #region mono
    private void Awake()
    {
        InitPanelDict();
    }
    #endregion

    #region UIPanel 
    [Header("UIPanel")]
    [SerializeField] List<GameObject> Obj_AllPanelList;
    private Dictionary<eUIPanelType, BasePanel> panelDict;

    void InitPanelDict() 
    {
        panelDict = new Dictionary<eUIPanelType, BasePanel>(Obj_AllPanelList.Count);

        foreach (var go in Obj_AllPanelList)
        {
            BasePanel bPanel = go.GetComponent<BasePanel>();
            if (bPanel != null)
            {
                var obj_panel = Instantiate(go);
                obj_panel.SetActive(false);
                obj_panel.transform.SetParent(PanelManager.Instance.CanvasTransform, false);
                panelDict.Add(bPanel.panelType, obj_panel.GetComponent<BasePanel>());
            }
            else 
            {
                Debug.LogWarning($"[PrefabLoader]Falied to get{bPanel}");
            }
        }
    }

    public Dictionary<eUIPanelType, BasePanel> GetPanelDict() 
    {
        return panelDict;
    }

    #endregion

    #region Audio
    [Header("Audio")]
    public AudioClip error;
    public AudioClip bgm;
    public AudioClip wasted;
    public AudioClip buttonClick;
    public AudioClip buttonEnter;
    public AudioClip scoreUp;
    public AudioClip IPDPlaced;
    public AudioClip RemoveIPG;
    public AudioClip HitObject;
    public AudioClip HitD;
    public AudioClip IPDSpawn;
    public AudioClip Won;
    public AudioClip Pause;
    public AudioClip Resume;
    #endregion
}
