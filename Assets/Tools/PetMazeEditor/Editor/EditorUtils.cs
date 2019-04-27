using System.Collections.Generic;
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

            string settingPath = "Assets/Tools/PetMazeEditor/Prefabs/MapSetting.prefab";
            MapSetting mapSetting = AssetDatabase.LoadAssetAtPath<MapSetting>(settingPath);
            if (mapSetting != null)
            {
                PasteComponentToGameObject(mapSetting, map);
            }
            else
            {
                Debug.LogError(settingPath+ " 路径下不存在MapSetting配置");
            }
        }

        public static List<T> GetAssets<T>(string path) where T : MonoBehaviour
        {
            T temp;
            string assetPath;
            GameObject asset;
            List<T> assetList = new List<T>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab",new string[] { path});
            for(int i = 0;i < guids.Length; i++)
            {
                assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                temp = asset.GetComponent<T>();
                if (temp)
                    assetList.Add(temp);
            }
            return assetList;
        }

        public static void PasteComponentToGameObject(Component com , GameObject toGo)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(com);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(toGo);
        }
    }
}
