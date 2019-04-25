using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PetMaze
{
    public class EventWindow : EditorWindow
    {

        private static EventWindow _instance;
        private int _selectIndex = 0;

        public static void OpenWindow()
        {
            _instance = EditorWindow.GetWindow<EventWindow>();
            _instance.titleContent = new GUIContent("事件窗口");
        }

        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        void OnDestroy()
        {

        }

        void OnGUI()
        {
            DrawTabs();
        }

        private void DrawTabs()
        {
            EditorGUILayout.HelpBox("aaa", MessageType.Info);
            _selectIndex = GUILayout.Toolbar(_selectIndex, Enum.GetEventTypeFatherTrimList().ToArray());
            Debug.Log(_selectIndex);
        }
    }
}

