using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PetMaze
{
    public class EventWindow : EditorWindow
    {
        private static EventWindow _instance;
        private List<string> _tabNameList;
        private Dictionary<int , List<EventInfo>> _eventMap;
        private int _selectTab = 0;
        private EventInfo _selectEventInfo;
        private int ItemWidth = 60;
        private Vector2 _scrollViewPosition = Vector2.zero;

        public delegate void SelectEventIdCall(EventInfo eventInfo);
        public static event SelectEventIdCall DSelectEventIdCall;
        public static EventWindow Instance { get { return _instance; } set { _instance = value; } }

        public static void OpenWindow()
        {
            Instance = EditorWindow.GetWindow<EventWindow>();
            Instance.titleContent = new GUIContent("事件窗口");
        }

        private void InitEventMap()
        {
            _tabNameList = new List<string>();
            _tabNameList.Add("全部");
            List<string> fatherNameList = MapSetting.Instance.GetEventFatherNameList();
            foreach (string name in fatherNameList)
            {
                _tabNameList.Add(name);
            }

            _eventMap = new Dictionary<int, List<EventInfo>>();
            for(int i = 0; i < _tabNameList.Count; i++)
            {
                if (i == 0)
                {
                    _eventMap.Add(0,MapSetting.Instance.GetEventList());
                }
                else
                {
                    _eventMap.Add(i, MapSetting.Instance.EventList[i - 1].eventList);
                }
            }
        }

        void OnEnable()
        {
            InitEventMap();
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
            DrawScrollView();
        }

        #region 绘制
        private void DrawTabs()
        {
            _selectTab = GUILayout.Toolbar(_selectTab, _tabNameList.ToArray());
        }

        private void DrawScrollView()
        {
            List<EventInfo> eventList = _eventMap[_selectTab];
            if (eventList.Count == 0)
            {
                EditorGUILayout.HelpBox("此类型下面没有次数",MessageType.Info);
                return;
            }

            int index = -1;
            if (_selectEventInfo != null)
            {
                for (int j = 0; j < eventList.Count; j++)
                {
                    if (eventList[j] == _selectEventInfo)
                    {
                        index = j;
                        break;
                    }
                }
            }
            _scrollViewPosition = GUILayout.BeginScrollView(_scrollViewPosition);
            int colCapacity = Mathf.FloorToInt(position.width / ItemWidth);
            index = GUILayout.SelectionGrid(index, GetGUIContentFromEventList(), colCapacity, GetGUIStyle());
            GUILayout.EndScrollView();

            if (index >= 0)
            {
                _selectEventInfo = eventList[index];
                if (DSelectEventIdCall != null)
                {
                    DSelectEventIdCall(_selectEventInfo);
                }
            }
        }

        private GUIContent[] GetGUIContentFromEventList()
        {
            List<GUIContent> contentList = new List<GUIContent>();

            var eventList = _eventMap[_selectTab];
            for(int i = 0; i < eventList.Count; i++)
            {
                GUIContent content = new GUIContent();
                content.text = eventList[i].name;
                content.image = eventList[i].icon;
                contentList.Add(content);
            }

            return contentList.ToArray();
        }

        private GUIStyle GetGUIStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);

            style.alignment = TextAnchor.LowerCenter;
            style.imagePosition = ImagePosition.ImageAbove;
            style.fixedWidth = ItemWidth;
            style.fixedHeight = ItemWidth;

            return style;
        }
        #endregion
    }
}

