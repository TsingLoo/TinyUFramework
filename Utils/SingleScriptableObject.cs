using UnityEngine;
using Path = System.IO.Path;

 namespace TinyUFramework {	
	public class SingleScriptableObject<T> : ScriptableObject where T : ScriptableObject
	{
	    private static string scriptableObjectPath = @"ScriptableObject";
	    private static T instance;
	    public static T Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
	                instance = Resources.Load<T>(Path.Join(scriptableObjectPath, typeof(T).Name));
	            }
	            if (instance == null)
	            {
	                instance = CreateInstance<T>();
	            }
	            if (string.IsNullOrEmpty(instance.name))
	            {
	                Debug.LogWarning($"Failed to get {nameof(ScriptableObject)}:{typeof(T).Name}");
	            }
	
	            return instance;
	        }
	    }
	
	    public static T GetSO(string soName)
	    {
	        var so = Resources.Load<T>(Path.Join(scriptableObjectPath, soName));
	        if (string.IsNullOrEmpty(so.name))
	        {
	            Debug.LogWarning($"Failed to get {nameof(ScriptableObject)}:{soName}");
	        }
	        return so;
	    }
	}
}
