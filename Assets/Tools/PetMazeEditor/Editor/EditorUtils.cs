using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PetMaze
{
    public static class EditorUtils
    {
        /// <summary>
        /// 创建木甲迷宫场景
        /// </summary>
        public static void CreateNewScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            RenderSettings.skybox = null;
            GameObject map = new GameObject("map");
            map.AddComponent<Map>();
        }
    }
}
