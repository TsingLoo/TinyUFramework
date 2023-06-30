using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using LitJson;

 namespace TinyUFramework {	
	public class PanelManager : Singleton<PanelManager>
	{
	    const string UIJson = nameof(UIJson);
	    static string jsonResourcesPath = Path.Combine("Json",UIJson);
	    
#if UNITY_EDITOR   
	    static string prePathInEditor = Path.Combine("Bundles");
#endif
	    
	    readonly string panelPrefabPath = Path.Combine(Application.dataPath
#if UNITY_EDITOR
	        , prePathInEditor    
#endif
	        ,"Resources", "Prefabs", "UI", "Panel");
	    
	    readonly string jsonFullPath = Path.Combine(Application.dataPath
#if UNITY_EDITOR
	        , prePathInEditor 
#endif
	        ,"Resources", jsonResourcesPath);
	    
	    
	
	    private List<UIPanelJson> AllPanelList;
	    private Dictionary<eUIPanelType, BasePanel> panelDict;
	    private Stack<BasePanel> panelStack;
	
	    private Transform canvasTransform;
	    public Transform CanvasTransform
	    {
	        get
	        {
	            if (canvasTransform == null)
	                canvasTransform = GameObject.Find("MainCanvas").transform;
	            return canvasTransform;
	        }
	    }
	
	
	    public void InitPanelManager() 
	    {
	        RefreshUIPanelInfoJson();
	    }
	
	    public List<UIPanelJson> ReadJsonFile()
	    {
	        if (Resources.Load(jsonResourcesPath) == null)
	        {
	            File.WriteAllText( $"{jsonFullPath}.json", "[]");
	        }
	        
	        TextAsset jsonText = Resources.Load(Path.Combine("Json", UIJson)) as TextAsset;
	        List<UIPanelJson> list = JsonMapper.ToObject<List<UIPanelJson>>(jsonText.ToString());
	        return list;
	    }
	    
	    public void WriteJsonFile(string jsonPath, List<UIPanelJson> list)
	    {
	        string json = JsonMapper.ToJson(list);
	        File.WriteAllText(jsonPath, json);
	    }
	    public void RefreshUIPanelInfoJson()
	    {
	        AllPanelList = ReadJsonFile();
#if UNITY_EDITOR
	        DirectoryInfo folder = new DirectoryInfo(panelPrefabPath);
	        
	        foreach (FileInfo file in folder.GetFiles("*.prefab"))
	        {
	            eUIPanelType type = (eUIPanelType)Enum.Parse(typeof(eUIPanelType), file.Name.Replace(".prefab", ""));
	            string path =  Path.Combine("Prefabs","UI","Panel", file.Name.Replace(".prefab", ""));
	            UIPanelJson uIPanel = AllPanelList.SearchPanelForType(type);
	
	            if (uIPanel != null)
	            {
	                uIPanel.UIPanelPath = path;
	            }
	            else
	            {
	                UIPanelJson panel = new UIPanelJson
	                {
	                    UIPanelType = type,
	                    UIPanelPath = path
	                };
	                AllPanelList.Add(panel);
	            }
	        }
	
	        WriteJsonFile($"{jsonFullPath}.json", AllPanelList);
	        //AssetDatabase.Refresh();
#endif
	    }
	
	    public void OpenPanel(eUIPanelType type)
	    {
	        Debug.Log($"[PanelManager] {type} is opened");
	        PushPanel(type);
	    }
	
	    public void OpenPanel<T>(eUIPanelType type, T param = default(T)) where T : IPanelParams
	    {
	        Debug.Log($"[PanelManager] {type} is opened with {param}");
	        BasePanel panel = GetPanel(type);
	        panel.SetPanelParam(param);
	        PushPanel(type);
	    }
	
	    public void StartLoading()
	    {
	        Debug.Log("[PanelManager] Begin to load");
	        OpenPanel(eUIPanelType.LoadingPanel);
	        foreach (BasePanel bp in panelStack)
	        {
	            bp.gameObject.GetComponent<CanvasGroup>().interactable = false;
	        }
	    }
	
	    public void FinishLoading()
	    {
	        PopPanel();
	        if (panelStack == null)
	        {
	            return;
	        }
	        //Debug.Log("[UIManager]Finish loading: " + panelStack.Count);
	        foreach (BasePanel bp in panelStack)
	        {
	            if (bp.gameObject.TryGetComponent<LoadingPanel>(out LoadingPanel lp))
	            {
	
	            }
	            Debug.Log(bp.gameObject.name);
	            bp.gameObject.GetComponent<CanvasGroup>().interactable = true;
	        }
	    }
	
	    public BasePanel GetTopPanel() 
	    {
	        if (panelStack == null || panelStack.Count <= 0)
	        {
	            Debug.Log($"[{nameof(PanelManager)}] There is no panel in stack");
	            return null;
	        }
	        else
	        {
	            return panelStack.Peek();
	        }
	    }
	
	
	    public BasePanel GetPanel(eUIPanelType type)
	    {
	        if (panelDict == null)
	        {
	            panelDict = new Dictionary<eUIPanelType, BasePanel>();
	            Debug.Log($"[PanelManager] Failed to find {nameof(panelDict)}, create one");
	        }
	
	        BasePanel panel = panelDict.TryGetValue(type);
	
	
	        if (panel == null)
	        {
	            string path = AllPanelList.SearchPanelForType(type).UIPanelPath;
	            if (path == null)
	                throw new Exception($"[PanelManager] Failed to get path by {nameof(AllPanelList)}");
	
	            if (Resources.Load(path) == null)
	                throw new Exception($"[PanelManager] Failed to get obj by {nameof(Resources.Load)}");
	            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
	            instPanel.transform.SetParent(CanvasTransform, false);
	
	            panelDict.Add(type, instPanel.GetComponent<BasePanel>());
	            return instPanel.GetComponent<BasePanel>();
	        }
	
	        return panel;
	    }
	
	
	    public void PushPanel(eUIPanelType type)
	    {
	        if (panelStack == null)
	        {
	            panelStack = new Stack<BasePanel>(); 
	        }
	
	
	        if (panelStack.Count > 0)
	        {
	            BasePanel topPanel = panelStack.Peek();
	            topPanel.OnPause();
	        }
	
	
	        BasePanel panel = GetPanel(type);
	        panel.gameObject.SetActive(true);
	        panelStack.Push(panel);
	
	
	        if (panel == null)
	        {
	            Debug.LogWarning($"[{nameof(PanelManager)}]");
	            return;
	        }
	        panel.OnEnter();
	    }
	    
	    
	    public void PopPanel()
	    {
	        if (panelStack == null)
	            panelStack = new Stack<BasePanel>();
	        
	        if (panelStack.Count <= 0) return;
	
	        //foreach (var e in panelStack)
	        
	        BasePanel topPanel = panelStack.Pop();
	        Debug.Log($"{nameof(PanelManager)} {topPanel.GetPanelType()} is now popped");
	        topPanel.OnExit();
	        
	        if (panelStack.Count <= 0) return;
	        BasePanel topPanel2 = panelStack.Peek();
	        Debug.Log($"[{nameof(PanelManager)}] The current top is : {topPanel2.name}");
	        topPanel2.OnResume();
	    }
	}
}
