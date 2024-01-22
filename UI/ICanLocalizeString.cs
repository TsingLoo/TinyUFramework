using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;

namespace VEC.EolaneVR
{
    public interface ICanLocalizeSmartString
    {
        public LocalizedString smartString { get; set; }
                
        LocalizedString.ChangeHandler LocalizeHandler { get; set; }
    }

    public static class CanLocalizeExtension
    {
        public static void BindLocalize(this ICanLocalizeSmartString self, TMP_Text tmp, UI_LocalizationKeys key,
            IList<object> arguments = null, string tableReference = "UI")
        {
            //self.L10n_textMeshPro = tmp;
            if (self.smartString != null)
            {
                Debug.LogWarning($"[{nameof(ICanLocalizeSmartString)}] Setting localize by Bind. Please use {nameof(SetLocalizeRefEnum)} instead");
            }

            self.smartString = new LocalizedString(tableReference, key.ToString());
            self.SetLocalizeRefEnum(key,arguments);
            self.LocalizeHandler = (value) =>
            {
                tmp.text = value;
                //Debug.Log($"[{nameof(BindLocalize)}] Set TMP to {value}");
            };
            self.smartString.StringChanged += self.LocalizeHandler;
        }
        
        public static void SetLocalizeRefEnum(this ICanLocalizeSmartString self, UI_LocalizationKeys key, IList<object> arguments = null,
            string tableReference = "UI")
        {
            //Debug.Log($"[{nameof(ICanLocalizeSmartString)}] Incoming key is {key.ToString()}");
            if (self.smartString == null)
            {
                Debug.LogError($"[{nameof(ICanLocalizeSmartString)}] {key}' smartString is null, Check if called in Awake?");
                return;
            }
            
            self.smartString.SetReference(tableReference, key.ToString());
            
            if (arguments != null)
            {
                self.smartString.Arguments = arguments;
            }
            self.smartString.RefreshString();
            //Debug.Log($"[{nameof(ICanLocalizeSmartString)}] Result string is {self.smartString}");
        }
        
        public static void UnBindLocalize(this ICanLocalizeSmartString self)
        {
            
            // Unsubscribe using the stored delegate
            if (self.LocalizeHandler != null)
            {
                self.smartString.StringChanged -= self.LocalizeHandler;
                //self.LocalizeHandler = null;
            }
        }
        
        
    }

    public class LocalizationStringEvent
    {
        public UI_LocalizationKeys key = UI_LocalizationKeys.NotGiven;
    }
}