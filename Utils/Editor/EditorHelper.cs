using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TinyUFramework
{
    public class EditorHelper
{
    private static string filePath = Path.Combine(Path.GetTempPath(),$"{nameof(EditorHelper)}.xml");
    static string currentScene = "";
    #region  QuickMainLaunch
    [MenuItem("ScenePlay/RunMainScene _%F1")]
    private static void RunMainScene()
    {
        currentScene = EditorSceneManager.GetActiveScene().path;
        XDocument doc = new XDocument(
            new XElement("root",
                new XElement($"{nameof(currentScene)}", currentScene)
            )
        );
        doc.Save(filePath);
        
        Debug.Log($"[{nameof(EditorHelper)}][QuickMainLaunch] Previous scene is {currentScene}");
        EditorSceneManager.OpenScene("Assets/Scenes/main.unity");
        EditorApplication.EnterPlaymode();
    }
    
    [MenuItem("ScenePlay/LoadPreviousScene _%F2")]
    private static void LoadPreviousScene()
    {
        if (!EditorApplication.isPlaying) return;

        EditorApplication.playModeStateChanged += LoadPreviousSceneAfterPlayModeChanges;
        EditorApplication.ExitPlaymode();
    }

    private static void LoadPreviousSceneAfterPlayModeChanges(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Check if the XML file exists
            if (System.IO.File.Exists(filePath))
            {
                // Load the XML file
                XDocument doc = XDocument.Load(filePath);

                // Get the previous scene path
                string previousScene = doc.Element("root").Element($"{nameof(currentScene)}").Value;

                Debug.Log($"[{nameof(EditorHelper)}] Return to {previousScene}");

                if (!string.IsNullOrEmpty(previousScene))
                {
                    EditorSceneManager.OpenScene(previousScene);
                }
            }
            else
            {
                Debug.Log("No previous scene data found.");
            }

            // Unsubscribe from the event
            EditorApplication.playModeStateChanged -= LoadPreviousSceneAfterPlayModeChanges;
        }
    }
    
    #endregion
}
}
