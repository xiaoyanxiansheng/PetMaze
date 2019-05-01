using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
/// <summary>
/// http://www.360doc.com/content/18/0803/14/22877383_775399366.shtml
/// https://blog.csdn.net/tom_221x/article/details/79437561
/// </summary>
namespace PetMaze
{
    #region 配置相关类
    [Serializable]
    public class MapEventSetting
    {
        // 地图ID
        public string Id = MapSetting.MazeTemplateEventId;
        // 地图尺寸
        [Header("改变大小之后要重新保存再加载一下")]
        public int Size = 10;
        // 主题
        public string Theme = "";
        // 是否开启上帝视角(驱散迷雾)
        public bool IsFog = true;
        // 权重
        public int Weight = 0;
        // 范围事件(和策划确定只需要统计第一个出现的事件)
        public List<EventRange> RangeEventList = new List<EventRange>();
        // 随机时间
        public List<EventRandom> RandomEventList = new List<EventRandom>();

        public int Width { get { return Size; } }
        public int Height { get { return Size; } }
    }
    /// <summary>
    /// 区域配置
    /// </summary>
    [Serializable]
    public class AreaEventItemSetting
    {
        public List<EventRange> RangeEventList = new List<EventRange>();
    }
    /// <summary>
    /// 区域事件配置
    /// </summary>
    [Serializable]
    public class AreaEventSetting
    {
        public AreaEventItemSetting[] AreaItems = new AreaEventItemSetting[4];
    }
    /// <summary>
    /// 事件上下限配置 如果不配置就不限制
    /// </summary>
    [Serializable]
    public class EventRange
    {
        public string Name = "";
        public string Id = "";
        public string Param = "";
        public int Min = 0;
        public int Max = 0;
    }
    /// <summary>
    /// 随机事件配置
    /// </summary>
    [Serializable]
    public class EventRandom
    {
        public List<EventRandomItem> EventList = new List<EventRandomItem>();
        public int Random = 0;
    }
    [Serializable]
    public class EventRandomItem
    {
        public string Name = "";
        public string Id = "";
        public string Param = "";
    }
    #endregion

    [ExecuteInEditMode]
    public class Map : MonoBehaviour
    {
        #region 面板配置属性
        // 地图配置
        public MapEventSetting MapEventSetting = new MapEventSetting();
        // 区域配置
        public AreaEventSetting AreaEventSetting = new AreaEventSetting();
        // 地块配置
        [NonSerialized]
        public MapItem MapItemSetting = new MapItem();
        #endregion

        #region menber
        private const int _mapRangeEventCount = 4;
        private const int _mapRandomEventCount = 4;
        private const int _mapRandomEventItenCount = 4;
        private Color _selectColor = new Color(1, 0.62f, 0.14f);
        private Color _disSelectColor = new Color(0.5f, 0.5f, 0.5f);
        private static Map _instance;
        private CsvData mapCsvData;

        // 保存的选择对象
        [NonSerialized] public EventInfo SelectEventInfo;

        public const float GridCellSize = 2f;
        public static Map Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<Map>();
                return _instance;
            }
        }
        #endregion

        #region 基本函数
        private void Awake()
        {
            ResetInitEventList();
        }
        private void OnEnable()
        {
            
        }
        private void OnDisable()
        {
            
        }
        private void OnValidate()
        {
            RefreshNames();
        }
        private void RefreshNames()
        {
            foreach(EventRange eventItem in MapEventSetting.RangeEventList)
            {
                eventItem.Name = MapSetting.Instance.GetEventName(eventItem.Id);
            }
            foreach (EventRandom eventRandom in MapEventSetting.RandomEventList)
            {
                foreach(EventRandomItem eventRandomItem in eventRandom.EventList)
                {
                    eventRandomItem.Name = MapSetting.Instance.GetEventName(eventRandomItem.Id);
                }
            }
            foreach(AreaEventItemSetting areaSetting in AreaEventSetting.AreaItems)
            {
                foreach(EventRange eventRange in areaSetting.RangeEventList)
                {
                    eventRange.Name = MapSetting.Instance.GetEventName(eventRange.Id);
                }
            }
        }
        /// <summary>
        /// 绘制gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            DrawGridLineGizmo();
        }
        /// <summary>
        /// gizmos被选中
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            DrawBorderGizmo();
            DrawFourAreaGizmo();
        }
        #endregion

        #region 绘制地图
        /// <summary>
        /// 边框
        /// </summary>
        private void DrawBorderGizmo()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = _selectColor;
            float disWidth = MapEventSetting.Width * GridCellSize;
            float disHeight = MapEventSetting.Height * GridCellSize;
            Vector3 startPos = transform.position;
            Gizmos.DrawLine(startPos, startPos + new Vector3(disWidth, 0, 0));
            Gizmos.DrawLine(startPos, startPos + new Vector3(0, disHeight, 0));
            Gizmos.DrawLine(startPos + new Vector3(0, disHeight, 0), startPos + new Vector3(disWidth, disHeight, 0));
            Gizmos.DrawLine(startPos + new Vector3(disWidth, 0, 0), startPos + new Vector3(disWidth, disHeight, 0));
            Gizmos.color = oldColor;
        }
        /// <summary>
        /// 网格
        /// </summary>
        private void DrawGridLineGizmo()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = _disSelectColor;
            Vector3 startPos = transform.position;
            for (int i = 0; i <= MapEventSetting.Width; i++)
            {
                Gizmos.DrawLine(startPos + new Vector3(GridCellSize * i, 0, 0), startPos + new Vector3(GridCellSize * i, GridCellSize * MapEventSetting.Height, 0));
            }
            for (int i = 0; i <= MapEventSetting.Height; i++)
            {
                Gizmos.DrawLine(startPos + new Vector3(0, GridCellSize * i, 0), startPos + new Vector3(GridCellSize * MapEventSetting.Width, GridCellSize * i, 0));
            }
            Gizmos.color = oldColor;
        }
        /// <summary>
        /// 区域
        /// </summary>
        private void DrawFourAreaGizmo()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = _selectColor;
            float disWidth = MapEventSetting.Width * GridCellSize;
            float disHeight = MapEventSetting.Height * GridCellSize;
            float halfDisWidth = disWidth * 0.5f;
            float halfDisHeight = disWidth * 0.5f;
            Vector3 startPos = transform.position;
            Gizmos.DrawLine(startPos + new Vector3(0, halfDisHeight,0), startPos + new Vector3(disWidth, halfDisHeight, 0));
            Gizmos.DrawLine(startPos + new Vector3(halfDisWidth, 0, 0), startPos + new Vector3(halfDisWidth, disHeight, 0));
            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 世界点转化成地图点
        /// </summary>
        /// <param name="worldCoordinate"></param>
        /// <returns></returns>
        public Vector2 GetMapCoordinate(Vector3 worldCoordinate)
        {
            Vector2 mapCoordinate = new Vector2(0, 0);

            Vector3 diff = worldCoordinate - transform.position;
            mapCoordinate.x = Mathf.Floor(diff.x / GridCellSize);
            mapCoordinate.y = Mathf.Floor(diff.y / GridCellSize);

            return mapCoordinate;
        }
        /// <summary>
        /// 地图点转化成世界点
        /// </summary>
        /// <param name="mapCoordinate"></param>
        public Vector3 GetWorldCoordinate(Vector2 mapCoordinate)
        {
            Vector3 worldCoordinate = new Vector3(0, 0, 0);

            worldCoordinate.x = (mapCoordinate.x + 0.5f) * GridCellSize;
            worldCoordinate.y = (mapCoordinate.y + 0.5f) * GridCellSize;

            return transform.position + worldCoordinate;
        }
        /// <summary>
        /// 自动捕捉位置
        /// </summary>
        /// <param name="tran"></param>
        public void AutoCatchMapCoordinate(Transform tran)
        {
            Vector2 mapCoordinate = GetMapCoordinate(tran.position);
            tran.position = GetWorldCoordinate(mapCoordinate);
        }
        /// <summary>
        /// 检测地图点是否合法
        /// </summary>
        /// <param name="mapCoordinate"></param>
        /// <returns></returns>
        public bool IsMapCoordinateValid(Vector2 mapCoordinate)
        {
            bool isValid = false;

            if (mapCoordinate.x >= 1 && mapCoordinate.x <= MapEventSetting.Width
                && mapCoordinate.y >= 1 && mapCoordinate.y <= MapEventSetting.Height)
            {
                isValid = true;
            }

            return isValid;
        }
        /// <summary>
        /// 检测世界点是否合法
        /// </summary>
        /// <param name="worldCoordinate"></param>
        /// <returns></returns>
        public bool IsWorldCoordinateValid(Vector3 worldCoordinate)
        {
            return IsMapCoordinateValid(GetMapCoordinate(worldCoordinate));
        }
        #endregion

        #region 帮助函数
        /// <summary>
        /// 点击选择事件窗口的事件会通知更新
        /// </summary>
        /// <param name="eventInfo"></param>
        public void UpdateSelectEventInfo(EventInfo eventInfo)
        {
            SelectEventInfo = eventInfo;
        }
        /// <summary>
        /// 位置是否合法
        /// </summary>
        /// <param name="pointx">从0开始</param>
        /// <param name="pointy">从0开始</param>
        /// <returns></returns>
        public bool IsPointValid(int pointx, int pointy)
        {
            bool isValid = false;

            int maxHeight = MapEventSetting.Height;
            int maxWidth = MapEventSetting.Width;
            if (pointx >= 0 && pointx < maxWidth && pointy >= 0 && pointy < maxHeight)
            {
                isValid = true;
            }

            return isValid;
        }
        /// <summary>
        /// 获取事件
        /// </summary>
        /// <param name="pointx"></param>
        /// <param name="pointy"></param>
        /// <returns></returns>
        public EventItem GetEventItem(int pointx,int pointy)
        {
            if (!IsPointValid(pointx, pointy))
            {
                return null;
            }
            return MapItemSetting.GetEventItem(pointx, pointy);
        }
        /// <summary>
        /// 获取事件的索引
        /// </summary>
        /// <param name="eventItem"></param>
        /// <returns></returns>
        public Vector2 GetEventItemPos(EventItem eventItem)
        {
            return MapItemSetting.GetEventItemPos(eventItem);
        }

        #region 文件相关
        /// <summary>
        /// Csv保存路径
        /// </summary>
        /// <returns></returns>
        public string GetSaveScvPath()
        {
            string path = path = MapSetting.Instance.Path + MapEventSetting.Id + ".csv";
            path = path.Replace('\\', '/');
            return path;
        }
        /// <summary>
        /// csv的打开路劲
        /// </summary>
        /// <returns></returns>
        public string GetOpenCsvPath()
        {
            string path = path = MapSetting.Instance.Path + MapEventSetting.Id + ".csv";
            path = path.Replace('\\', '/');
            return path;
        }
        /// <summary>
        /// 层级配置路径
        /// </summary>
        /// <returns></returns>
        public string GetMapCsvPath()
        {
            string path = MapSetting.Instance.GetMapCsvPath();
            path = path.Replace('\\', '/');
            return path;
        }
        /// <summary>
        /// 保存所有文件
        /// </summary>
        /// <returns></returns>
        public bool SaveFiles()
        {
            return SaveFilesEvent() && SaveFilesMap();
        }
        /// <summary>
        /// 保存事件文件
        /// </summary>
        /// <returns></returns>
        public bool SaveFilesEvent()
        {
            return MapItemSetting.SaveCsv(GetSaveScvPath());
        }
        /// <summary>
        /// 保存区域文件
        /// </summary>
        /// <returns></returns>
        public bool SaveFilesArea()
        {
            //var list = MapItemSetting.GetEventTrimList();
            //CsvTools.Instance.Save(list, GetSaveScvPath());
            return true;
        }
        /// <summary>
        /// 保存层级文件
        /// </summary>
        /// <returns></returns>
        public bool SaveFilesMap()
        {
            Dictionary<string, string> dicData = new Dictionary<string, string>();
            #region 地图属性
            dicData["ID"] = MapEventSetting.Id;
            dicData["Size"] = MapEventSetting.Size.ToString();
            dicData["Theme"] = MapEventSetting.Theme;
            dicData["IsFog"] = (MapEventSetting.IsFog ? 1 : 0).ToString();
            dicData["Weight"] = MapEventSetting.Weight.ToString();
            // 随机事件
            FillFileMapRandomEvent(dicData);
            // 范围事件
            FillFileMapRangeEvent(dicData,MapEventSetting.RangeEventList,"Event");
            #endregion
            #region 区域属性
            // 范围事件
            for(int i = 0;i< AreaEventSetting.AreaItems.Length; i++)
            {
                FillFileMapRangeEvent(dicData, AreaEventSetting.AreaItems[i].RangeEventList, "EventArea"+(i+1));
            }
            #endregion

            mapCsvData.Modify(dicData);
            return CsvTools.Instance.Save(mapCsvData.data, GetMapCsvPath());
        }
        /// <summary>
        /// 填充范围事件
        /// </summary>
        /// <param name="dicData"></param>
        /// <param name="rangeEventList"></param>
        /// <param name="key"></param>
        private void FillFileMapRangeEvent(Dictionary<string, string> dicData, List<EventRange> rangeEventList,string key)
        {
            for(int i = 0;i< rangeEventList.Count; i++)
            {
                EventRange e = rangeEventList[i];
                int index = (i + 1);
                dicData[key + "Id" + index] = e.Id;
                dicData[key + "Param" + index] = e.Param;
                dicData[key + "Min" + index] = e.Min.ToString();
                dicData[key + "Max" + index] = e.Max.ToString();
            }
        }
        /// <summary>
        /// 填充随机事件
        /// </summary>
        /// <param name="dicData"></param>
        /// <param name="randomEventList"></param>
        private void FillFileMapRandomEvent(Dictionary<string, string> dicData)
        {
            for (int i = 0; i < MapEventSetting.RandomEventList.Count; i++)
            {
                int index = (i + 1);
                dicData["EventRandom"+ index] = MapEventSetting.RandomEventList[i].Random.ToString();
                List<EventRandomItem> eventList = MapEventSetting.RandomEventList[i].EventList;
                for(int j = 0; j < eventList.Count; j++)
                {
                    dicData["EventRandom" + index + "Event" + (j + 1)] = eventList[j].Id;
                    dicData["EventRandom" + index + "Param" + (j + 1)] = eventList[j].Param;
                }
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 重置数据和表现
        /// </summary>
        public void ResetInitEventList()
        {
            // 数据
            InitMapCsvData();
            InitMapSetting();
            // 表现
            MapItemSetting.DeleteAllIns();
            MapItemSetting.InitEventList(GetOpenCsvPath());
            MapItemSetting.CreateAllIns();
        }
        private void InitMapCsvData()
        {
            if (mapCsvData != null)
                return;
            mapCsvData = new CsvData(GetMapCsvPath());
        }
        public void InitMapSetting()
        {
            MapEventSetting.Size = GetMapCsvIntValue("Size");
            MapEventSetting.Theme = GetMapCsvValue("Theme");
            MapEventSetting.IsFog = GetMapCsvValue("IsFog") == "1" ? true : false;
            MapEventSetting.Weight = GetMapCsvIntValue("Size");
            // 初始化随机事件
            InitMapRandomEventSetting();
            // 初始化范围事件
            InitMapRangeEventSetting(MapEventSetting.RangeEventList, "Event");
            // 初始化区域范围事件
            for (int i = 0; i < 4; i++)
            {
                AreaEventSetting.AreaItems[i] = new AreaEventItemSetting();
                InitMapRangeEventSetting(AreaEventSetting.AreaItems[i].RangeEventList, "EventArea" + (i + 1));
            }
        }
        public void InitMapRangeEventSetting(List<EventRange> rangeEventList, string key)
        {
            rangeEventList.Clear();
            for (int i = 0;i< _mapRangeEventCount; i++)
            {
                int index = (i + 1);
                string id = GetMapCsvValue(key + "Id" + index);
                if (id != "")
                {
                    EventRange e = new EventRange();
                    e.Id = GetMapCsvValue(key + "Id" + index);
                    e.Param = GetMapCsvValue(key + "Param" + index);
                    e.Min = GetMapCsvIntValue(key + "Min" + index);
                    e.Max = GetMapCsvIntValue(key + "Max" + index);
                    e.Name = MapSetting.Instance.GetEventName(e.Id);
                    rangeEventList.Add(e);
                }
            }
        }
        public void InitMapRandomEventSetting()
        {
            MapEventSetting.RandomEventList.Clear();
            for (int i = 0; i < _mapRandomEventCount; i++)
            {
                int index = (i + 1);
                int Random = GetMapCsvIntValue("EventRandom" + index);
                if (Random > 0)
                {
                    EventRandom randomEvent = new EventRandom();
                    randomEvent.Random = Random;
                    randomEvent.EventList = new List<EventRandomItem>();
                    for (int j = 0; j < _mapRandomEventItenCount; j++)
                    {
                        EventRandomItem item = new EventRandomItem();
                        item.Id = GetMapCsvValue("EventRandom" + index + "Event" + (j + 1));
                        item.Param = GetMapCsvValue("EventRandom" + index + "Param" + (j + 1));
                        item.Name = MapSetting.Instance.GetEventName(item.Id);
                        if (item.Id != "")
                            randomEvent.EventList.Add(item);
                    }
                    MapEventSetting.RandomEventList.Add(randomEvent);
                }
            }
        }
        public string GetMapCsvValue(string key)
        {
            return mapCsvData.GetValue(MapEventSetting.Id,key);
        }
        public int GetMapCsvIntValue(string key)
        {
            return mapCsvData.GetIntValue(MapEventSetting.Id, key);
        }
        #endregion
        #endregion
    }
}
