using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class UtilExtension
{
    public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        /// <summary>
        /// ��չ�ֵ����е�TryGetValue����
        /// ����ֱ��ͨ������key����value,��������ԭ����һ������boolֵ
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        TValue value;
        dict.TryGetValue(key, out value);

        return value;
    }

    /// <summary>
    /// ��չList��
    /// �����ֶ���ָ��UIPanelType��UIPanel,����UIPanel������
    /// </summary>
    /// <param name="list">UIPanel��List</param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// 
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
        fs.Seek(0, SeekOrigin.Begin);//�α�Ĳ��������п���
        byte[] bytes = new byte[fs.Length];//�����ֽڣ������洢��ȡ����ͼƬ�ֽ�
        try
        {
            fs.Read(bytes, 0, bytes.Length);//��ʼ��ȡ�����������trycatch��䣬��ֹ��ȡʧ�ܱ���

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        fs.Close();//�мǹر�

        int width = 2048;//ͼƬ�Ŀ��������������������ᵽ���������У�
        int height = 2048;//ͼƬ�ĸߣ�����˵�����⻰��pico��صĿ��������ﲻ�ܴ���4k��4k��Ȼ����ʾ�쳣����ʱ����pico��ʱ��ӦΪ����������˴����ԭ����Ϊ��������ͼ��6000*3600�����³����м���ͼ��ͺ����ˡ�����
        Texture2D texture = new Texture2D(width, height);
        if (texture.LoadImage(bytes))
        {
            Debug.Log("[IO] Load Image succesfully " + path);
            return texture;//�����ɵ�texture2d���أ�������͵õ����ⲿ��ͼƬ������ʹ����

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