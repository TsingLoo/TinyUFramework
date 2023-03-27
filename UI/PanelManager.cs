using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;
using DG.Tweening;


public class PanelManager : Singleton<PanelManager>
{
    public const int LEFT_HIDE_VALUE = -1500;

    //Unity Editor：<path to project folder>/Assets
    readonly string panelPrefabPath = Application.dataPath + @"/Bundles/Resources/Prefabs/UI/Panel/";
    readonly string jsonPath = Application.dataPath + @"/Bundles/Resources/Json/UIJson.json";

    /// <summary>
    /// 记录了所有UIPanel类的列表
    /// </summary>
    private List<UIPanelJson> AllPanelList;
    //开发一个给出面板类型，就实例化该面板，并返回面板上挂载的BasePanel组件的方法
    private Dictionary<eUIPanelType, BasePanel> panelDict;
    //使用栈来存储当前场景中正在显示的Panel
    private Stack<BasePanel> panelStack;

    private Transform canvasTransform;
    public Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
                //通过名称获取Canvas上的Transform，所以不要有同名Canvas
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

    public List<UIPanelJson> ReadJsonFile(string jsonPath)
    {
        //如果找不到UIJson文件，则新建一个Json文件并写入“[]”
        //如果仅新建一个空Json文件，Json转换会返回一个null，也就是后面的list等于null，之后使用list的操作就会报一个空指针错误。
        if (!File.Exists(jsonPath))
        {
            Debug.Log("[PanelManager]找不到" + jsonPath + "文件");
            File.WriteAllText(jsonPath, "[]");
        }
        List<UIPanelJson> list = JsonMapper.ToObject<List<UIPanelJson>>(File.ReadAllText(jsonPath));

        return list;
    }

    //将UIPanel列表内容写到Json文件中
    public void WriteJsonFile(string jsonPath, List<UIPanelJson> list)
    {
        string json = JsonMapper.ToJson(list);
        File.WriteAllText(jsonPath, json);
    }

    /// <summary>
    /// 利用此方法自动注册、更新面板的预制体。
    /// </summary>
    public void RefreshUIPanelInfoJson()
    {
        Debug.Log("[PanelManager]开始更新ALLPanelList");
        AllPanelList = ReadJsonFile(jsonPath);
        //读取存储面板prefeb的文件夹的信息
        DirectoryInfo folder = new DirectoryInfo(panelPrefabPath);

        //遍历储存面板prefab的文件夹里每一个prefab的名字，并把名字转换为对应的eUIPanelType中的枚举（一种注册的办法）
        //再检查UIPanelType是否存在 于List里,若存在List里则更新path,若不存在List里则加上
        foreach (FileInfo file in folder.GetFiles("*.prefab"))
        {
            //Debug.Log("[UIManager]The fileinfo is " + file);
            //这个预制体的eUIPanelType是（应当和预制体的名称相统一，Path即Type）
            eUIPanelType type = (eUIPanelType)Enum.Parse(typeof(eUIPanelType), file.Name.Replace(".prefab", ""));
            //Debug.Log("[UIManager]这个预制体的type是 " + type);
            string path = @"Prefabs/UI/Panel/" + file.Name.Replace(".prefab", "");
            //string path = panelPrefabPath + file.Name;
            //string path = @"Prefabs/UI/Panel/" + file.Name;

            UIPanelJson uIPanel = AllPanelList.SearchPanelForType(type);

            if (uIPanel != null)//UIPanel在该List中,更新path值
            {
                Debug.Log("[PanelManager]" + type + "在List中，更新path值: " + path);
                uIPanel.UIPanelPath = path;
            }
            else
            {
                Debug.Log("[PanelManager]" + type + "不在List中，添加path值: " + path);
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
        Debug.Log("[PanelManager]结束更新ALLPanelList");
    }

    private void GetPanelDictFromPrefabLoader() 
    {
        panelDict = PrefabLoader.Instance.GetPanelDict();
    }

    ///<summary>
    ///调用此方法打开相关面板，不传参
    ///<summary>
    /// <param name="type"></param>
    public void OpenPanel(eUIPanelType type)
    {
        Debug.Log("[PanelManager]" + type + " is opened");
        PushPanel(type);
    }

    /// <summary>
    /// 调用此方法打开相关面板,传递参数
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
        //【扩展方法】通过type查找字典里对应的BasePanel，若找不到则返回null，具体见Extension部分
        BasePanel panel = panelDict.TryGetValue(type);

        //在现有字典里没有找到
        //只能去json里找type对应的prefab的路径并加载
        //再加进字典里以便下次在字典中查找
        if (panel == null)
        {
            //【扩展方法】通过Type查找列表里对应的UIPanel，若找不到则返回null，具体见Extension部分
            string path = AllPanelList.SearchPanelForType(type).UIPanelPath;
            if (path == null)
                throw new Exception("[PanelManager]找不到该UIPanelType的Prefab");

            if (Resources.Load(path) == null)
                throw new Exception("[PanelManager]找不到该Path(" + path + ")的Prefab");
            //实例化prefab
            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            //把面板设为Canvas的子物体，false表示不保持worldPosition，而是根据Canvas的坐标设定localPosition
            instPanel.transform.SetParent(CanvasTransform, false);

            panelDict.Add(type, instPanel.GetComponent<BasePanel>());

            return instPanel.GetComponent<BasePanel>();
        }

        return panel;
    }

    //把指定类型的panel入栈,并显示在场景中
    public void PushPanel(eUIPanelType type)
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();
        //判断栈里是否有其他panel,若有,则把原栈顶panel设定其状态为暂停(OnPause)
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();
            topPanel.OnPause();
        }


        BasePanel panel = GetPanel(type);
        panel.gameObject.SetActive(true);
        //把指定类型的panel入栈并设定其状态为进入场景(OnEnter)
        panelStack.Push(panel);


        if (panel == null)
        {
            Debug.LogWarning("[PanelManager]" + type + " 未挂载组件 " + typeof(BasePanel));
            return;
        }
        panel.OnEnter();
    }

    //把栈顶panel出栈,并从场景中消失
    public void PopPanel()
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        //检查栈是否为空，若为空则直接退出方法
        if (panelStack.Count <= 0) return;

        //foreach (var e in panelStack)
        //{
        //    Debug.Log(e.transform.name);
        //}


        //把栈顶panel出栈，并把该panel状态设为退出场景(OnExit)
        BasePanel topPanel = panelStack.Pop();
        Debug.Log("[PanelManager]" + topPanel.GetPanelType() + " is now popped");
        topPanel.OnExit();

        //再次检查出栈栈顶Panel后栈是否为空
        //若为空，直接退出方法
        //若不为空，则把新的栈顶Panel状态设为继续(OnResume)
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