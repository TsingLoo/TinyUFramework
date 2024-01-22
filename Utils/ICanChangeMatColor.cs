using DG.Tweening;
using UnityEngine;

namespace VEC.EolaneVR
{
    public interface ICanChangeMatColor
    {
    }

    public static class CanChangeColorExtension
    {
        public static void ChangeMatColor(this ICanChangeMatColor self, Color color)
        {
            var mono = self as MonoBehaviour;

            // Ensure the object implementing ICanChangeMatColor is a MonoBehaviour
            if (mono == null)
            {
                Debug.LogWarning($"[{nameof(ICanChangeMatColor)}] {nameof(ICanChangeMatColor)} must be implemented by a {nameof(MonoBehaviour)}.");
                return;
            }

            var meshRenderer = mono.GetComponent<MeshRenderer>();

            // Ensure a MeshRenderer component is present
            if (meshRenderer != null)
            {
                Debug.Log($"[{nameof(ICanChangeMatColor)}] Changing material color to {color}");


                
                meshRenderer.material.DOColor(color, "_BaseColor", 0.5f);
            }
            else
            {
                Debug.LogWarning($"[{nameof(ICanChangeMatColor)}] MeshRenderer component not found.");
            }
        }
    }
}