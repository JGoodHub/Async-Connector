using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Async.Connector.Editor
{

    [InitializeOnLoad]
    public static class SceneAutoLoader
    {

        // Static constructor binds a playmode-changed callback.
        static SceneAutoLoader()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        // Menu items to select the "master" scene and control whether or not to load it.
        [MenuItem("File/Scene Autoload/Select Master Scene...")]
        private static void SelectMasterScene()
        {
            string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
            masterScene = masterScene.Replace(Application.dataPath, "Assets"); //project relative instead of absolute path
            if (!string.IsNullOrEmpty(masterScene))
            {
                MasterScene = masterScene;
                LoadMasterOnPlay = true;
            }
        }

        [MenuItem("File/Scene Autoload/Load 'MAGLoad' scene On Play", true)]
        private static bool ShowLoadMasterOnPlay()
        {
            return !LoadMasterOnPlay;
        }

        [MenuItem("File/Scene Autoload/Load 'MAGLoad' scene On Play")]
        private static void EnableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = true;
        }

        [MenuItem("File/Scene Autoload/Don't Load 'MAGLoad' scene On Play", true)]
        private static bool ShowDontLoadMasterOnPlay()
        {
            return LoadMasterOnPlay;
        }

        [MenuItem("File/Scene Autoload/Don't Load 'MAGLoad' scene On Play")]
        private static void DisableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = false;
        }

        // Play mode change callback handles the scene load/reload.
        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (!LoadMasterOnPlay)
            {
                return;
            }

            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed play -- autoload master scene.
                PreviousScene = EditorSceneManager.GetActiveScene().path;
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    try
                    {
                        EditorSceneManager.OpenScene(MasterScene);
                    }
                    catch
                    {
                        Debug.LogError($"error: scene not found: {MasterScene}");
                        EditorApplication.isPlaying = false;
                    }
                }
                else
                {
                    // User cancelled the save operation -- cancel play as well.
                    EditorApplication.isPlaying = false;
                }
            }

            // isPlaying check required because cannot OpenScene while playing
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed stop -- reload previous scene.
                try
                {
                    EditorSceneManager.OpenScene(PreviousScene);
                }
                catch
                {
                    Debug.LogError($"error: scene not found: {PreviousScene}");
                }
            }
        }

        // Properties are remembered as editor preferences.
        private const string cEditorPrefLoadMasterOnPlay = "SceneAutoLoader.LoadMasterOnPlay";
        private const string cEditorPrefMasterScene = "SceneAutoLoader.MasterScene";
        private const string cEditorPrefPreviousScene = "SceneAutoLoader.PreviousScene";

        private static bool LoadMasterOnPlay
        {
            get => EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, true);
            set => EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value);
        }

        private static string MasterScene
        {
            get => EditorPrefs.GetString(cEditorPrefMasterScene, "Master.unity");
            set => EditorPrefs.SetString(cEditorPrefMasterScene, value);
        }

        private static string PreviousScene
        {
            get => EditorPrefs.GetString(cEditorPrefPreviousScene, EditorSceneManager.GetActiveScene().path);
            set => EditorPrefs.SetString(cEditorPrefPreviousScene, value);
        }

    }

}