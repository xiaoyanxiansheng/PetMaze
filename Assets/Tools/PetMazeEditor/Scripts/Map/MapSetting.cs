﻿using System.Collections.Generic;
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
        // 大小根据表格指定不能手动更改大小
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
        public static string MazeTemplateEventId = "100000";
        public static string MazeMapCsvName = "MazeMap";
        public static string MazeThemeCsvName = "MazeTheme";
        public static string MazeElement = "MazeElement";
        public string Path = "D:/xyj/PetMazeEditor/PetMaze/Assets/Tools/PetMazeEditor/TestFile/";
        
        // 事件列表
        [SerializeField] private List<EventFatherInfo> _eventList = new List<EventFatherInfo>();
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

        [ContextMenu("InitData")]
        public void InitData()
        {
            CsvData csvData = new CsvData(GetMapElementPath());
            List<string> mainKeyList = csvData.GetMainKeyList();
            Dictionary<string, EventFatherInfo> fatherEventList = new Dictionary<string, EventFatherInfo>();
            foreach (string id in mainKeyList)
            {
                string fatherId = csvData.GetValue(id, "type");
                // 填充father信息
                if (!fatherEventList.ContainsKey(fatherId))
                {
                    EventFatherInfo tempFatherInfo = new EventFatherInfo();
                    tempFatherInfo.id = fatherId;
                    EventFatherInfo setFatherInfo = GetEventFatherInfo(fatherId);
                    if (setFatherInfo != null)
                    {
                        tempFatherInfo.name = setFatherInfo.name;
                    }
                    fatherEventList[fatherId] = tempFatherInfo;
                    fatherEventList[fatherId].eventList = new List<EventInfo>();
                }

                // 填充eventList信息
                EventInfo tempEventInfo = new EventInfo();
                tempEventInfo.id = id;
                EventInfo setEventInfo = GetEventInfo(fatherId, id);
                if (setEventInfo != null)
                {
                    tempEventInfo.name = setEventInfo.name;
                    tempEventInfo.icon = setEventInfo.icon;
                }
                fatherEventList[fatherId].eventList.Add(tempEventInfo);
            }
            _eventList.Clear();
            foreach(EventFatherInfo fInfo in fatherEventList.Values)
            {
                _eventList.Add(fInfo);
            }
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

        public EventFatherInfo GetEventFatherInfo(string fatherId)
        {
            EventFatherInfo fatherInfo = null;

            for (int i = 0; i < _eventList.Count; i++)
            {
                if (_eventList[i].id == fatherId)
                {
                    fatherInfo = _eventList[i];
                }
            }

            return fatherInfo;
        }
        public EventInfo GetEventInfo(string fatherId,string eventId)
        {
            EventFatherInfo fatherInfo = GetEventFatherInfo(fatherId);
            if (fatherInfo != null)
            {
                foreach (EventInfo eInfo in fatherInfo.eventList)
                {
                    if (eInfo.id == eventId)
                    {
                        return eInfo;
                    }
                }
            }
            return null;
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

        public EventInfo GetEventInfo(string id)
        {
            for (int i = 0; i < _eventList.Count; i++)
            {
                for (int j = 0; j < _eventList[i].eventList.Count; j++)
                {
                    if (_eventList[i].eventList[j].id == id)
                    {
                        return _eventList[i].eventList[j];
                    }
                }
            }

            return null;
        }

        public string GetEventName(string id)
        {
            string name = "";

            EventInfo info = GetEventInfo(id);
            if (info != null)
                name = info.name;

            return name;
        }
        #endregion

        #region 路径相关
        public string GetMapCsvPath()
        {
            return Path + MazeMapCsvName + ".csv";
        }
        public string GetMapElementPath()
        {
            return Path + MazeElement + ".csv";
        }
        public string GetMapThemePath()
        {
            return Path + MazeThemeCsvName + ".csv";
        }
        #endregion
    }
}
