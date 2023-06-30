using UnityEngine;

 namespace TinyUFramework {	
	public class SingletonForMonobehaviour <T> where T : MonoBehaviour
	{
	    public static MonoBehaviour Instance
	    {
	        get
	        {
	            return Nested.instance;
	        }
	    }
	
	    class Nested
	    {
	        static Nested() { }
	
	        internal static readonly MonoBehaviour instance = new MonoBehaviour();
	    }
	}
}
