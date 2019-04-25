using System;
using UnityEngine;
using UnityEditor;

namespace PetMaze
{
    [CustomEditor(typeof(Map))]
    public class MapInspector : Editor
    {
        private Map _target;

        private void Awake()
        {
            _target = (Map)target;
        }

        //public override void OnInspectorGUI()
        //{
        //    EditorGUILayout.HelpBox("保存成功", MessageType.Info);
        //    // 绘制地图属性
        //    DrawMap();
        //}

        #region 绘制场景
        private void OnSceneGUI()
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10f,10f,100f,1000f));
            OnSceneCheckBtn();
            OnSceneCsvOpenBtn();
            OnSceneCsvSaveBtn();
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void OnSceneCheckBtn()
        {
            if (GUILayout.Button("检查配置文件是否合理"))
            {
                if (_target.CheckMapValid())
                    Debug.Log("检测通过");
                else
                    Debug.LogError("检测失败");
            }
        }
        private void OnSceneCsvOpenBtn()
        {
            if (GUILayout.Button("打开CSV文件"))
            {
                CsvData.Instance.FillCsv(_target.MapEventTypeList, _target.SavePath);
            }
        }
        private void OnSceneCsvSaveBtn()
        {
            if (GUILayout.Button("保存CSV文件"))
            {
                if (CsvData.Instance.Save(_target.MapEventTypeList, _target.SavePath))
                {
                    Debug.Log("保存成功");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("保存失败");
                }
            }
        }
        
        #endregion



        public void DrawMap()
        {
            EditorGUILayout.LabelField("地图属性",EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            _target.Height = EditorGUILayout.IntField("行", Math.Max(0, _target.Height));
            _target.Width = EditorGUILayout.IntField("列", Math.Max(0, _target.Width));
            EditorGUILayout.EndHorizontal();
        }
    }
}
