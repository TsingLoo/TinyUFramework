using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Text;
using System;

namespace VEC.EolaneVR
{
    [InitializeOnLoad]
    public static class InitEditorHelper
    {
        static InitEditorHelper()
        {
            EditorHelper.GetEditorHelperXMLRoot();
        }
    }

    public class EditorHelper
    {
        #region ClearConsole
        public static void ClearConsole()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            System.Reflection.MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
            clearConsoleMethod.Invoke(new object(), null);
        }
        #endregion
        
        public static XElement GetEditorHelperXMLRoot()
        {
            XDocument doc;

            // Check if the XML file exists
            if (System.IO.File.Exists(filePath))
            {
                doc = XDocument.Load(filePath);
            }
            else
            {
                doc = new XDocument(
                    new XElement("root",
                        new XElement("QuickMainSceneLaunch",
                            new XElement($"targetScene", "Assets/Scenes/Office_Light.unity"),
                            new XElement($"{nameof(currentScene)}", currentScene)
                        )
                        // new XElement(nameof(RunPythonAfterBuild),
                        //     new XElement($"enable", 0),
                        //     new XElement($"pythonScriptFolderPath", "Assets/Path/To/Your/Folder/"),
                        //     new XElement($"scriptName", "ForBuild.py"),
                        //     new XElement($"version", "0.0.0"),
                        //     new XElement($"args", "")
                        // )
                    )
                );
                doc.Save(filePath);
            }

            return doc.Root;
        }


        public static void SetElementValue(string elementName, string newValue)
        {
            XElement root = GetEditorHelperXMLRoot();

            // Find the element with the specified name
            XElement elementToUpdate = root.Descendants().FirstOrDefault(e => e.Name.LocalName == elementName);

            if (elementToUpdate != null)
            {
                // Update the element value
                elementToUpdate.Value = newValue;

                // Save the changes back to the file
                XDocument doc = new XDocument(root);
                doc.Save(filePath);

                Console.WriteLine($"Element '{elementName}' updated with value '{newValue}'.");
            }
            else
            {
                Console.WriteLine($"Element '{elementName}' not found.");
            }
        }

        static DirectoryInfo dataDirectoryInfo = new DirectoryInfo(Application.dataPath);

        static string filePath = Path.Combine(dataDirectoryInfo.Parent.ToString(), "UserSettings",
            $"{nameof(EditorHelper)}.xml");

        #region QuickMainSceneLaunch

        static string currentScene = "";

        [MenuItem("ScenePlay/RunMainScene _%F1")]
        private static void RunMainScene()
        {
            if (EditorApplication.isPlaying) return;

            currentScene = EditorSceneManager.GetActiveScene().path;
            GetEditorHelperXMLRoot().Element("QuickMainSceneLaunch").Element($"{nameof(currentScene)}").Value = currentScene;

            Debug.Log($"[{nameof(EditorHelper)}][QuickMainLaunch] Previous scene is {currentScene}");
            EditorSceneManager.OpenScene(GetEditorHelperXMLRoot().Element("QuickMainSceneLaunch").Element($"targetScene")
                .Value);
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
            Debug.Log($"[{nameof(EditorHelper)}] {filePath}");


            if (state == PlayModeStateChange.EnteredEditMode)
            {
                // Check if the XML file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Load the XML file
                    XDocument doc = XDocument.Load(filePath);

                    // Get the previous scene path
                    string previousScene = GetEditorHelperXMLRoot().Element("QuickMainSceneLaunch")
                        .Element($"{nameof(currentScene)}").Value;

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

    #region ICanSetStyleEditor

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class CanStyleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Check if the target object implements ICanSetStyleEditor
            if (target is ICanSetStyleEditor ui)
            {
                // Draw the default inspector
                DrawDefaultInspector();

                if (GUILayout.Button("Set Style Now"))
                {
                    ui.SetStyleEditor();
                }
            }
            else
            {
                // If not, just draw the default inspector
                DrawDefaultInspector();
            }
        }
    }

    #endregion

    #region EnumGeneratorFromCSVFileWindow

    public class EnumGeneratorFromCSVFileWindow : EditorWindow
    {
        private bool enUSSelected = false;
        private bool zhCNSelected = false;
        
        [MenuItem("Tools/Localization Asset Generator")]
        public static void ShowWindow()
        {
            GetWindow<EnumGeneratorFromCSVFileWindow>("Localization Asset Generator");
        }

        private string csvFilePath = "Assets/Localization/UI.csv";
        private string outputDirectory = "Assets/Scripts/Utility";
        private string namespaceForEnum = "VEC.EolaneVR";
        private string filenamePostfix = "_LocalizationKeys";

        void OnGUI()
        {

            // CSV File Path
            EditorGUILayout.BeginHorizontal();
            csvFilePath = EditorGUILayout.TextField("CSV File Path:", csvFilePath);
            if (GUILayout.Button("Browse CSV"))
            {
                csvFilePath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
            }

            EditorGUILayout.EndHorizontal();
         
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Generate Enum", EditorStyles.boldLabel);
            // Output Directory
            EditorGUILayout.BeginHorizontal();
            outputDirectory = EditorGUILayout.TextField("Output Directory:", outputDirectory);
            if (GUILayout.Button("Browse Output"))
            {
                string path = EditorUtility.OpenFolderPanel("Select Output Directory", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    outputDirectory = path.Replace(Application.dataPath, "Assets"); // Convert to relative path
                }
            }

            EditorGUILayout.EndHorizontal();

            // Fields for customization
            namespaceForEnum = EditorGUILayout.TextField("Namespace:", namespaceForEnum);
            filenamePostfix = EditorGUILayout.TextField("Filename Postfix:", filenamePostfix);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("TTS", EditorStyles.boldLabel);
            // Toggles for language selection
            EditorGUILayout.BeginHorizontal();
            enUSSelected = EditorGUILayout.Toggle("English (en-US)", enUSSelected);
            zhCNSelected = EditorGUILayout.Toggle("Chinese (zh-CN)", zhCNSelected);
            EditorGUILayout.EndHorizontal();
            
            
            if (GUILayout.Button("Run"))
            {
                if (!string.IsNullOrEmpty(csvFilePath))
                {
                    GenerateEnumFromCSVFile(csvFilePath);
                }
                else
                {
                    Debug.LogError("Please select a valid CSV file.");
                }
            }
        }

        void GenerateEnumFromCSVFile(string filePath)
        {
            var csvData = File.ReadAllText(filePath);
            var keys = ExtractKeysFromCSV(csvData);

            // Ensure "NoGiven" is not a key in the CSV
            if (keys.Contains("NotGiven"))
            {
                string errorMsg =
                    "The key 'NotGiven' is reserved and should not be present in the CSV. Please remove or rename this key. Please also make sure you have updated the .csv after modifying Localization Table.";

                bool userClickedOk = EditorUtility.DisplayDialog("Error Generating Enum", errorMsg, "OK");
                Debug.LogError(errorMsg);
                if (userClickedOk)
                {
                    EditorApplication.ExecuteMenuItem("Window/Asset Management/Localization Tables");
                }

                return;
            }

            // Validate keys
            foreach (var key in keys)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-zA-Z][a-zA-Z0-9]*$"))
                {
                    string errorMsg =
                        $"Key '{key}' is not a valid C# identifier. Keys should only contain letters and numbers and must not start with a number. Please also make sure you have updated the .csv after modifying Localization Table.";

                    bool userClickedOk = EditorUtility.DisplayDialog("Error Generating Enum", errorMsg, "OK");
                    Debug.LogError(errorMsg);
                    if (userClickedOk)
                    {
                        EditorApplication.ExecuteMenuItem("Window/Asset Management/Localization Tables");
                    }

                    return;
                }
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string enumName = $"{fileName}{filenamePostfix}";
            string outputPath = $"{outputDirectory}/{enumName}.cs";

            StringBuilder enumTemplate =
                new StringBuilder("// This file is auto-generated by the Localization Enum Generator tool.\n");
            enumTemplate.AppendLine(
                "// Manual modifications will be overwritten. Please use the Tools/Localization Enum Generator from CSV File for generation instead.\n");
            enumTemplate.AppendLine($"namespace {namespaceForEnum}\n{{\n    public enum {enumName}\n    {{");
            // Add NotGiven as the first enum value
            enumTemplate.AppendLine("        NotGiven = 0,");
            // Add all keys except the last one with a comma
            for (int i = 0; i < keys.Count - 1; i++)
            {
                enumTemplate.AppendLine($"        {keys[i]},");
            }

            // Add the last key without a comma
            if (keys.Count > 0)
            {
                enumTemplate.AppendLine($"        {keys[keys.Count - 1]}");
            }

            enumTemplate.AppendLine("    }\n}");


            File.WriteAllText(outputPath, enumTemplate.ToString());
            AssetDatabase.Refresh();
            Debug.Log($"Generated {enumName} at: {outputPath}");
        }
        
        void GenerateTTSFromCSVFile(string filePath)
        {
            var csvData = File.ReadAllText(filePath);
            var allLines = ExtractLinesFromCSV(csvData);

            foreach (var line in allLines)
            {
                // Assuming columns are: Key, Id, Chinese (Simplified)(zh-Hans), English(en), French(fr)
                var chineseText = line[2].Trim('"');
                var englishText = line[3].Trim('"');

                if (zhCNSelected && !string.IsNullOrWhiteSpace(chineseText))
                {
                    //ProcessTextForTTS(chineseText, "zh-CN");
                }

                if (enUSSelected && !string.IsNullOrWhiteSpace(englishText))
                {
                    //ProcessTextForTTS(englishText, "en-US");
                }

                // Add conditions for other languages if necessary
            }
        }

        List<string> ExtractKeysFromCSV(string csv)
        {
            var allLines = ExtractLinesFromCSV(csv);
            List<string> keys = new List<string>();

            foreach (var line in allLines)
            {
                // Extract the key from the first column of each line
                keys.Add(line[0].Trim('"'));
            }

            return keys;
        }
        
        List<string[]> ExtractLinesFromCSV(string csv)
        {
            var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<string[]> allLines = new List<string[]>();

            int i = 0;
            while (i < lines.Length)
            {
                var combinedLine = lines[i];
                while (combinedLine.Count(c => c == '"') % 2 == 1 && i + 1 < lines.Length)
                {
                    i++;
                    combinedLine += " " + lines[i];
                }

                allLines.Add(combinedLine.Split(','));
                i++;
            }

            // Remove the header
            allLines.RemoveAt(0);

            return allLines;
        }

    }

    #endregion
    
    #region TTS

    public abstract class TTS_Engine
    {
        public abstract string ProcessTextForTTS(string text, string languageCode);
    }

    public class SystemSpeechTTS_Engine : TTS_Engine
    {
        public override string ProcessTextForTTS(string text, string languageCode)
        {
            // using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            // {
            //     // Select voice based on language code
            //     synthesizer.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, new System.Globalization.CultureInfo(languageCode));
            //
            //     string outputFilePath = Path.Combine("Path/To/Output/Directory", $"{Guid.NewGuid()}.wav");
            //
            //     // Configure the synthesizer to save to a file
            //     synthesizer.SetOutputToWaveFile(outputFilePath);
            //
            //     // Synthesize the text to speech
            //     synthesizer.Speak(text);
            //
            //     return outputFilePath; // Returns the path to the generated audio file
            // }

            throw new System.Exception("SystemSpeechTTS_Engine is not implemented yet.");
        }
    }
    
    #endregion
}