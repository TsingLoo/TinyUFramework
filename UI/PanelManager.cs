using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using DG.Tweening;
#if false
using LitJson;
#endif

 namespace TinyUFramework {	
	public class PanelManager : Singleton<PanelManager>
	{
	    public const int LEFT_HIDE_VALUE = -1500;
	
	    //Unity Editor��<path to project folder>/Assets
	    readonly string panelPrefabPath = Application.dataPath + @"/Bundles/Resources/Prefabs/UI/Panel/";
	    readonly string jsonPath = Application.dataPath + @"/Bundles/Resources/Json/UIJson.json";
	
	    /// <summary>
	    /// ��¼������UIPanel����б�
	    /// </summary>
	    private List<UIPanelJson> AllPanelList;
	    //����һ������������ͣ���ʵ��������壬����������Ϲ��ص�BasePanel����ķ���
	    private Dictionary<eUIPanelType, BasePanel> panelDict;
	    //ʹ��ջ���洢��ǰ������������ʾ��Panel
	    private Stack<BasePanel> panelStack;
	
	    private Transform canvasTransform;
	    public Transform CanvasTransform
	    {
	        get
	        {
	            if (canvasTransform == null)
	                //ͨ�����ƻ�ȡCanvas�ϵ�Transform�����Բ�Ҫ��ͬ��Canvas
	                canvasTransform = GameObject.Find("MainCanvas").transform;
	            return canvasTransform;
	        }
	    }
	
	
	    public void InitPanelManager() 
	    {
#if false
	        RefreshUIPanelInfoJson();
#endif
	        GetPanelDictFromPrefabLoader();
	
	    }
	
#if false
	    public List<UIPanelJson> ReadJsonFile(string jsonPath)
	    {
	        //����Ҳ���UIJson�ļ������½�һ��Json�ļ���д�롰[]��
	        //������½�һ����Json�ļ���Jsonת���᷵��һ��null��Ҳ���Ǻ����list����null��֮��ʹ��list�Ĳ����ͻᱨһ����ָ�����
	        if (!File.Exists(jsonPath))
	        {
	            Debug.Log("[PanelManager]�Ҳ���" + jsonPath + "�ļ�");
	            File.WriteAllText(jsonPath, "[]");
	        }
	        List<UIPanelJson> list = JsonMapper.ToObject<List<UIPanelJson>>(File.ReadAllText(jsonPath));
	
	        return list;
	    }
	
	    //��UIPanel�б�����д��Json�ļ���
	    public void WriteJsonFile(string jsonPath, List<UIPanelJson> list)
	    {
	        string json = JsonMapper.ToJson(list);
	        File.WriteAllText(jsonPath, json);
	    }
	
	    /// <summary>
	    /// ���ô˷����Զ�ע�ᡢ��������Ԥ���塣
	    /// </summary>
	    public void RefreshUIPanelInfoJson()
	    {
	        Debug.Log("[PanelManager]��ʼ����ALLPanelList");
	        AllPanelList = ReadJsonFile(jsonPath);
	        //��ȡ�洢���prefeb���ļ��е���Ϣ
	        DirectoryInfo folder = new DirectoryInfo(panelPrefabPath);
	
	        //�����������prefab���ļ�����ÿһ��prefab�����֣���������ת��Ϊ��Ӧ��eUIPanelType�е�ö�٣�һ��ע��İ취��
	        //�ټ��UIPanelType�Ƿ���� ��List��,�����List�������path,�������List�������
	        foreach (FileInfo file in folder.GetFiles("*.prefab"))
	        {
	            //Debug.Log("[UIManager]The fileinfo is " + file);
	            //���Ԥ�����eUIPanelType�ǣ�Ӧ����Ԥ�����������ͳһ��Path��Type��
	            eUIPanelType type = (eUIPanelType)Enum.Parse(typeof(eUIPanelType), file.Name.Replace(".prefab", ""));
	            //Debug.Log("[UIManager]���Ԥ�����type�� " + type);
	            string path = @"Prefabs/UI/Panel/" + file.Name.Replace(".prefab", "");
	            //string path = panelPrefabPath + file.Name;
	            //string path = @"Prefabs/UI/Panel/" + file.Name;
	
	            UIPanelJson uIPanel = AllPanelList.SearchPanelForType(type);
	
	            if (uIPanel != null)//UIPanel�ڸ�List��,����pathֵ
	            {
	                Debug.Log("[PanelManager]" + type + "��List�У�����pathֵ: " + path);
	                uIPanel.UIPanelPath = path;
	            }
	            else
	            {
	                Debug.Log("[PanelManager]" + type + "����List�У����pathֵ: " + path);
	                UIPanelJson panel = new UIPanelJson
	                {
	                    UIPanelType = type,
	                    UIPanelPath = path
	                };
	                AllPanelList.Add(panel);
	            }
	        }
	
	        WriteJsonFile(jsonPath, AllPanelList);
	        //AssetDatabase.Refresh();
	        Debug.Log("[PanelManager]��������ALLPanelList");
	    }
#endif
	    private void GetPanelDictFromPrefabLoader() 
	    {
	        panelDict = PrefabLoader.Instance.GetPanelDict();
	    }
	
	    ///<summary>
	    ///���ô˷����������壬������
	    ///<summary>
	    /// <param name="type"></param>
	    public void OpenPanel(eUIPanelType type)
	    {
	        Debug.Log("[PanelManager]" + type + " is opened");
	        PushPanel(type);
	    }
	
	    /// <summary>
	    /// ���ô˷�����������,���ݲ���
	    /// </summary>
	    /// <param name="type"></param>
	    public void OpenPanel<T>(eUIPanelType type, T param = default(T)) where T : IPanelParams
	    {
	        Debug.Log("[PanelManager]" + type + " is opened");
	        BasePanel panel = GetPanel(type);
	        panel.SetPanelParam(param);
	        PushPanel(type);
	    }
	
	    public void StartLoading()
	    {
	        Debug.Log("[PanelManager]Begin to load");
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
	            Debug.Log($"[{nameof(PanelManager)}]There is no panel in stack");
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
	            Debug.Log($"[PanelManager]Failed to find {nameof(panelDict)}, create one");
	        }
	        //����չ������ͨ��type�����ֵ����Ӧ��BasePanel�����Ҳ����򷵻�null�������Extension����
	        BasePanel panel = panelDict.TryGetValue(type);
	
	        //�������ֵ���û���ҵ�
	        //ֻ��ȥjson����type��Ӧ��prefab��·��������
	        //�ټӽ��ֵ����Ա��´����ֵ��в���
	        if (panel == null)
	        {
	            //����չ������ͨ��Type�����б����Ӧ��UIPanel�����Ҳ����򷵻�null�������Extension����
	            string path = AllPanelList.SearchPanelForType(type).UIPanelPath;
	            if (path == null)
	                throw new Exception("[PanelManager]�Ҳ�����UIPanelType��Prefab");
	
	            if (Resources.Load(path) == null)
	                throw new Exception("[PanelManager]�Ҳ�����Path(" + path + ")��Prefab");
	            //ʵ����prefab
	            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
	            //�������ΪCanvas�������壬false��ʾ������worldPosition�����Ǹ���Canvas�������趨localPosition
	            instPanel.transform.SetParent(CanvasTransform, false);
	
	            panelDict.Add(type, instPanel.GetComponent<BasePanel>());
	
	            return instPanel.GetComponent<BasePanel>();
	        }
	
	        return panel;
	    }
	
	    //��ָ�����͵�panel��ջ,����ʾ�ڳ�����
	    public void PushPanel(eUIPanelType type)
	    {
	        if (panelStack == null)
	            panelStack = new Stack<BasePanel>();
	        //�ж�ջ���Ƿ�������panel,����,���ԭջ��panel�趨��״̬Ϊ��ͣ(OnPause)
	        if (panelStack.Count > 0)
	        {
	            BasePanel topPanel = panelStack.Peek();
	            topPanel.OnPause();
	        }
	
	
	        BasePanel panel = GetPanel(type);
	        panel.gameObject.SetActive(true);
	        //��ָ�����͵�panel��ջ���趨��״̬Ϊ���볡��(OnEnter)
	        panelStack.Push(panel);
	
	
	        if (panel == null)
	        {
	            Debug.LogWarning("[PanelManager]" + type + " δ������� " + typeof(BasePanel));
	            return;
	        }
	        panel.OnEnter();
	    }
	
	    //��ջ��panel��ջ,���ӳ�������ʧ
	    public void PopPanel()
	    {
	        if (panelStack == null)
	            panelStack = new Stack<BasePanel>();
	
	        //���ջ�Ƿ�Ϊ�գ���Ϊ����ֱ���˳�����
	        if (panelStack.Count <= 0) return;
	
	        //foreach (var e in panelStack)
	        //{
	        //    Debug.Log(e.transform.name);
	        //}
	
	
	        //��ջ��panel��ջ�����Ѹ�panel״̬��Ϊ�˳�����(OnExit)
	        BasePanel topPanel = panelStack.Pop();
	        Debug.Log("[PanelManager]" + topPanel.GetPanelType() + " is now popped");
	        topPanel.OnExit();
	
	        //�ٴμ���ջջ��Panel��ջ�Ƿ�Ϊ��
	        //��Ϊ�գ�ֱ���˳�����
	        //���Ϊ�գ�����µ�ջ��Panel״̬��Ϊ����(OnResume)
	        if (panelStack.Count <= 0) return;
	        BasePanel topPanel2 = panelStack.Peek();
	        Debug.Log($"[{nameof(PanelManager)}]The current top is : {topPanel2.name}");
	        topPanel2.OnResume();
	    }
	
	    public static void tweenHideGameObject(GameObject go, eDirection direction, float targetXvalue = LEFT_HIDE_VALUE)
	    {
	        switch ((int)direction)
	        {
	            case (int)eDirection.LEFT:
	                go.transform.DOMoveX(targetXvalue, 1f).SetEase(Ease.InOutCirc);
	                break;
	        }
	
	
	    }
	}
	
	public enum eDirection
	{
	    UP = 0,
	    DOWN,
	    LEFT,
	    RIGHT
	}
}
