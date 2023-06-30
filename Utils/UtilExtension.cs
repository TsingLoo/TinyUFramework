using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

 namespace TinyUFramework {	
	public static class UtilExtension
	{
	    public static string FixedLengthString(int value , int length = 2)
	    {
	        return value.ToString().PadLeft(length, '0');
	    }
	
#if false
	    public static void HighlightElement(this TMP_Text element, Color highlightColor)
	    {
	        Color previousColor = new Color(element.color.r, element.color.g, element.color.b, element.color.a);
	        //Color previousColor = element.color;
	        Tweener OutT = element.DOColor(previousColor, 0.7f).SetId($"{element.transform.name}{highlightColor}OUT").SetAutoKill(false);
	        element.DOColor(highlightColor, 1f).SetId($"{element.transform.name}{highlightColor}IN").SetAutoKill(false).OnComplete(() => {
	            OutT.Play();
	        });
	    }
#endif
	
	    public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
	    {
	        TValue value;
	        dict.TryGetValue(key, out value);
	
	        return value;
	    }
	
	    public static UIPanelJson SearchPanelForType(this List<UIPanelJson> list, eUIPanelType type)
	    {
	        foreach (var item in list)
	        {
	            if (item.UIPanelType == type)
	                return item;
	        }
	
	        return null;
	    }
	
	    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
	    {
	        T component = obj.GetComponent<T>();
	        if (component == null)
	        {
	            return obj.AddComponent<T>();
	        }
	
	        return component;
	    }
	
	    public static void SafeSetActive(UnityEngine.Object obj, bool active)
	    {
	        if (obj != null)
	        {
	            if (obj is GameObject)
	            {
	                ((GameObject)obj).SetActive(active);
	            }
	            else if (obj is Component)
	            {
	                ((GameObject)obj).gameObject.SetActive(active);
	            }
	        }
	    }
	
	    public static char ValidateInt(string text, int charIndex, char charToValidate)
	    {
	        if ((charToValidate >= '0' && charToValidate <= '9'))
	        {
	            return charToValidate;
	        }
	
	        return char.MinValue;
	    }
	
	    public static char ValidateFloat(string text, int charIndex, char charToValidate)
	    {
	        if ((charToValidate >= '0' && charToValidate <= '9') || (charIndex > 0 && charToValidate == '.'))
	        {
	            return charToValidate;
	        }
	        return char.MinValue;
	    }
	
	    public static void InputIntAndSave(TMP_InputField input, string key = nameof(Input), int defaultValue = 2)
	    {
	        if (PlayerPrefs.HasKey(key))
	        {
	            input.text = PlayerPrefs.GetInt(key).ToString();
	        }
	        else
	        {
	            input.text = defaultValue.ToString();
	            PlayerPrefs.SetInt(key, int.Parse(input.text));
	        }
	
	        input.onValidateInput += ValidateInt;
	        input.onValueChanged.AddListener((value) =>
	        {
	            if (string.IsNullOrEmpty(value))
	            {
	                value = 1.ToString();
	            }
	            else if (int.Parse(value) < 1)
	            {
	                input.text = 1.ToString();
	                value = 1.ToString();
	            }
	        });
	        input.onEndEdit.AddListener((value) =>
	        {
	            PlayerPrefs.SetInt(key, int.Parse(value));
	        });
	    }
	
	    public static void InputFloatAndSave(TMP_InputField input, string key = nameof(Input), float defaultValue = 2.8f)
	    {
	        if (PlayerPrefs.HasKey(key))
	        {
	            input.text = PlayerPrefs.GetFloat(key).ToString();
	        }
	        else
	        {
	            input.text = defaultValue.ToString();
	            PlayerPrefs.SetFloat(key, float.Parse(input.text));
	        }
	
	        input.onValidateInput += ValidateFloat;
	        input.onValueChanged.AddListener((value) =>
	        {
	            if (string.IsNullOrEmpty(value))
	            {
	                value = 1.ToString();
	            }
	            else if (float.Parse(value) < 1)
	            {
	                input.text = 1.ToString();
	                value = 1.ToString();
	            }
	        });
	        input.onEndEdit.AddListener((value) =>
	        {
	            PlayerPrefs.SetFloat(key, float.Parse(value));
	        });
	    }
	
	    public static Texture2D LoadTextureByIO(string path)
	    {
	        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
	        fs.Seek(0, SeekOrigin.Begin);
	        byte[] bytes = new byte[fs.Length];
	        try
	        {
	            fs.Read(bytes, 0, bytes.Length);
	
	        }
	        catch (Exception e)
	        {
	            Debug.Log(e);
	        }
	        fs.Close();
	        int height = 2048;
	        int width = 2048;
	        Texture2D texture = new Texture2D(width, height);
	        if (texture.LoadImage(bytes))
	        {
	            Debug.Log("[IO] Load Image succesfully " + path);
	            return texture;
	        }
	        else
	        {
	            Debug.Log("[IO] Failed to load Image" + path);
	            return null;
	        }
	    }
	
	    public static Sprite TextureToSprite(Texture2D tex)
	    {
	        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
	        return sprite;
	    }
	
	
	}
}
