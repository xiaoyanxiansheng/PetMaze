using System;
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
        //    // 绘制地图属性
        //    DrawMap();
        //}

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
