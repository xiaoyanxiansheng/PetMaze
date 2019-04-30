using UnityEditor;
using UnityEngine;

namespace PetMaze
{
    [CustomEditor(typeof(MapItemCom))]
    public class MapItemComInspector : Editor
    {

        void OnSceneGUI()
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10,10,100,1000));
            OnSceneGUIBackBackBtn();
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        void OnSceneGUIBackBackBtn()
        {
            if (GUILayout.Button("返回地图模式"))
            {
                Selection.activeGameObject = GameObject.FindObjectOfType<Map>().gameObject;
            }
        }
    }
}