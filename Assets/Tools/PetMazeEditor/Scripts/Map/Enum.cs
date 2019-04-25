/// <summary>
/// 一些枚举 和 常量
/// </summary>
using System.Collections.Generic;

namespace PetMaze
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EventType
    {
        a = 103,
    }
    /// <summary>
    /// 事件父类
    /// </summary>
    public enum FatherEventType
    {
        a = 1,
    }
    /// <summary>
    /// 主题
    /// </summary>
    public enum Theme
    {
        Default
    }

    public static class Enum
    {
        public static List<string> EventTypeList = null;       // 事件列表
        public static List<string> EventTypeFatherList = null; // 事件父类列表
        public static List<string> EventTypeFatherTrimList = null; // 事件父类列表
        public static List<string> ThemeTypeList = null;       // 主题列表

        public static void InitEnum()
        {
            GetEventTypeList();
            GetEventTypeFatherList();
            GetThemeTypeList();
        }

        public static List<string> GetEventTypeList()
        {
            if (EventTypeList == null || EventTypeList.Count == 0)
            {
                EventTypeList = GetEventList(1);
            }
            return EventTypeList;
        }

        public static List<string> GetEventTypeFatherList()
        {
            if (EventTypeFatherList == null || EventTypeFatherList.Count == 0)
            {
                EventTypeFatherList = GetEventList(2);
                EventTypeFatherTrimList = new List<string>();
                for(int i = 0;i< EventTypeFatherList.Count; i++)
                {
                    string value = EventTypeFatherList[i];
                    if (!EventTypeFatherTrimList.Contains(value))
                        EventTypeFatherTrimList.Add(value);
                }
            }
            return EventTypeFatherList;
        }

        public static List<string> GetEventTypeFatherTrimList()
        {
            if (EventTypeFatherTrimList == null || EventTypeFatherTrimList.Count == 0)
            {
                EventTypeFatherList = GetEventList(2);
                EventTypeFatherTrimList = new List<string>();
                for (int i = 0; i < EventTypeFatherList.Count; i++)
                {
                    string value = EventTypeFatherList[i];
                    if (!EventTypeFatherTrimList.Contains(value))
                        EventTypeFatherTrimList.Add(value);
                }
            }
            return EventTypeFatherTrimList;
        }

        public static List<string> GetEventList(int index)
        {
            List<string> eventCellList = new List<string>();
            Dictionary<int, List<string>> tempEventTypeList = new Dictionary<int, List<string>>();
            CsvData.Instance.FillCsv(tempEventTypeList, Map.Instance.EventTypePath);
            for (int i = 0; i < tempEventTypeList.Count; i++)
            {
                if (i > 3)
                {
                    for (int j = 0; j < tempEventTypeList[i].Count; j++)
                    {
                        if (j == index)
                        {
                            eventCellList.Add(tempEventTypeList[i][j]);
                        }
                    }
                }
            }
            return eventCellList;
        }

        public static void GetThemeTypeList()
        {
            
        }
    }
}
