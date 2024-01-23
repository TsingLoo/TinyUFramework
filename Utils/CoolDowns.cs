using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Autohand;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Events;

namespace VEC.EolaneVR
{
    public static class CoolDowns
    {
        private static Dictionary<(string, object), float> cooldowns = new Dictionary<(string, object), float>();

        public static void StartCooldown(string methodName, float duration, object source = null)
        {
            cooldowns[(methodName, source)] = Time.time + duration;
        }

        public static bool IsCooldownComplete(string methodName, bool startCountDownIfTrue = true, float duration = 1f,
            object source = null)
        {
            if (!cooldowns.ContainsKey((methodName, source)) || Time.time >= cooldowns[(methodName, source)])
            {
                if (startCountDownIfTrue)
                {
                    StartCooldown(methodName, duration, source);
                }

                return true;
            }

            return false;
        }

        public static void TryExecute(UnityAction action, float duration, object source = null, UnityAction onCompleted = null)                     
        {
            if (IsCooldownComplete(GetKeyFromUnityAction(action), true, duration, source))
            {
                action.Invoke();
                onCompleted?.Invoke();
            }
        }

        
        public static void TryExecute<T>(UnityAction<T> action, T param, float duration, object source = null, UnityAction onCompleted = null)
        {
            if (IsCooldownComplete(GetKeyFromUnityAction(action), true, duration, source))
            {
                action.Invoke(param);
                onCompleted?.Invoke();
            }
        }

        public static void ResetCooldown(string methodName, object source = null)
        {
            if (cooldowns.ContainsKey((methodName, source)))
            {
                cooldowns.Remove((methodName, source));
            }
        }
        
        public static string GetKeyFromUnityAction(this UnityAction self)
        {
            if (self.GetMethodInfo() == null)
            {
                return self.GetHashCode().ToString();
            }
            else
            {
                return self.GetMethodInfo().Name;
            }
        }   
        
        public static string GetKeyFromUnityAction<T>(this UnityAction<T> self)
        {
            if (self.GetMethodInfo() == null)
            {
                return self.GetHashCode().ToString();
            }
            else
            {
                return self.GetMethodInfo().Name;
            }
        } 
    }

    public interface ICanCoolDown
    {
        public void CdFunction();
    }

    public interface ICanCoolDownT
    {
        public void CdFunction<T>(T param);
    }

    public interface ICanCoolDownHandTouchEvent: ICanCoolDown
    {

    }
    
    public interface ICanCoolDownTHandTouchEvent: ICanCoolDownT
    {

    }
    
    public static class CanCoolDownExtensions
    {
        public static void TryExecute(this ICanCoolDown self, float duration, object source = null, UnityAction onCompleted = null)
        {
            CoolDowns.TryExecute(self.CdFunction, duration, source, onCompleted);
        }

        public static void TryExecute<T>(this ICanCoolDownT self, T param, float duration, object source = null, UnityAction onCompleted = null)
        {
            CoolDowns.TryExecute(self.CdFunction, param, duration, source, onCompleted);
        }
        
        public static void TryExecute(this ICanCoolDownHandTouchEvent self, HandTouchEvent handTouchEvent,  float duration, object source = null, UnityAction onCompleted = null)
        {
            handTouchEvent.HandStartTouch.AddListener((h) =>
            {
                CoolDowns.TryExecute(self.CdFunction, duration, source, onCompleted);
            });
        }
        
        public static void TryExecute<T>(this ICanCoolDownTHandTouchEvent self, HandTouchEvent handTouchEvent, T param,  float duration, object source = null, UnityAction onCompleted = null)
        {
            handTouchEvent.HandStartTouch.AddListener((h) =>
            {
                CoolDowns.TryExecute(self.CdFunction, param, duration, source, onCompleted);
            });
        }
    }
}