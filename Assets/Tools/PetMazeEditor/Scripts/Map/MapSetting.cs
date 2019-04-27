using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 通用配置 和 具体地图无关
/// </summary>
namespace PetMaze
{
    [Serializable]
    public class EventInfo
    {
        public string name;
        public string id;
        public Texture2D icon;
    }
    [Serializable]
    public class EventFatherInfo
    {
        public string name;
        public string id;
        public List<EventInfo> eventList = new List<EventInfo>();
    }
    [Serializable]
    public class ThemeInfo
    {
        public string name;
        public string id;
        public Color color;
    }
    public class MapSetting : MonoBehaviour
    {
        #region 配置
        // 事件列表
        [SerializeField] private List<EventFatherInfo> _eventList = new List<EventFatherInfo>();
        // 主题列表
        [SerializeField] private List<ThemeInfo> _themeList = new List<ThemeInfo>();
        #endregion

        #region 初始化相关
        private static MapSetting _instance = null;

        public static MapSetting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<MapSetting>();
                return _instance;
            }
        }

        public List<EventFatherInfo> EventList { get { return _eventList; } set { _eventList = value; } }
        #endregion

        #region 主题相关
        public ThemeInfo GetDefaultTheme()
        {
            return GetTheme(0);
        }
        public ThemeInfo GetTheme(int index)
        {
            if (index > _themeList.Count)
            {
                return null;
            }
            return _themeList[index];
        }
        public List<ThemeInfo> GetThemeList()
        {
            return _themeList;
        }
        public List<string> GetThemeNameList()
        {
            List<string> themeNameList = new List<string>();

            for (int i = 0; i < _themeList.Count; i++)
            {
                themeNameList.Add(_themeList[i].name);
            }

            return themeNameList;
        }
        #endregion

        #region 事件相关
        public List<EventInfo> GetEventList()
        {
            List<EventInfo> eventInfos = new List<EventInfo>();

            for(int i = 0;i< _eventList.Count; i++)
            {
                for(int j = 0; j< _eventList[i].eventList.Count; j++)
                {
                    EventInfo eventInfo = _eventList[i].eventList[j];
                    if (!eventInfos.Contains(eventInfo))
                    {
                        eventInfos.Add(eventInfo);
                    }
                }
            }

            return eventInfos;
        }

        public List<EventInfo> GetEventList(string fatherId)
        {
            List<EventInfo> eventInfos = new List<EventInfo>();

            for (int i = 0; i < _eventList.Count; i++)
            {
                if (_eventList[i].id == fatherId)
                {
                    for (int j = 0; j < _eventList[i].eventList.Count; j++)
                    {
                        EventInfo eventInfo = _eventList[i].eventList[j];
                        if (!eventInfos.Contains(eventInfo))
                        {
                            eventInfos.Add(eventInfo);
                        }
                    }
                }
            }

            return eventInfos;
        }

        public List<string> GetEventFatherNameList()
        {
            List<string> nameList = new List<string>();

            foreach(EventFatherInfo eventFather in _eventList)
            {
                nameList.Add(eventFather.name);
            }

            return nameList;
        }

        public string GetFatherId(int index)
        {
            string id = "";

            if (index < _eventList.Count)
            {
                id = _eventList[index].id;
            }

            return id;
        }
        #endregion
    }
}
