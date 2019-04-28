using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
/// <summary>
/// http://www.360doc.com/content/18/0803/14/22877383_775399366.shtml
/// </summary>
namespace PetMaze
{
    #region 配置相关类
    [Serializable]
    public class MapEventSetting
    {
        public string Id = "";
        public string OpenPath = "";
        public int Width = 10;
        public int Height = 10;
        public string Theme = "";                                     // 主题   
        public int Layer = 1;                                      // 层级
        public bool IsFog = true;
    }
    [Serializable]
    public class AreaEventItemSetting
    {
        public string name = "";
    }
    [Serializable]
    public class AreaEventSetting
    {
        public AreaEventItemSetting[] AreaItems = new AreaEventItemSetting[4];
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
        public MapItem MapItemSetting = new MapItem();
        #endregion

        #region menber
        private string _openPath = "";
        private Color _selectColor = new Color(1, 0.62f, 0.14f);
        private Color _disSelectColor = new Color(0.5f, 0.5f, 0.5f);
        private static Map _instance;

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
            // 地块数据初始化
            MapItemSetting.InitEventList(GetCsvPath());
            MapItemSetting.CreateAllIns();
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
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
        #endregion

        #region 帮助函数
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
        public bool IsWorldCoordinateValid(Vector3 worldCoordinate)
        {
            return IsMapCoordinateValid(GetMapCoordinate(worldCoordinate));
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void InitMap()
        {
            // 初始化地图
            // 初始化区域
            // 初始化地块
        }
        /// <summary>
        /// 根据数据创建所有event对象
        /// </summary>
        public void RefreshMapItemInstanceList()
        {
            DeleteMapItemInstanceList();
            CreateMapItemInstaceList();
        }
        public void DeleteMapItemInstanceList()
        {

        }
        public void CreateMapItemInstaceList()
        {

        }

        public void UpdateSelectEventInfo(EventInfo eventInfo)
        {
            SelectEventInfo = eventInfo;
        }
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
        public EventItem GetEventItem(int pointx,int pointy)
        {
            if (!IsPointValid(pointx, pointy))
            {
                return null;
            }
            return MapItemSetting.GetEventItem(pointx, pointy);
        }
        public Vector2 GetEventItemPos(EventItem eventItem)
        {
            return MapItemSetting.GetEventItemPos(eventItem);
        }

        public string GetCsvPath()
        {
            if (MapEventSetting.OpenPath != "")
                return MapEventSetting.OpenPath;
            return MapSetting.Instance.TemplateCsvPath;
        }

        public string GetSavePath()
        {
            string path = "";

            if (MapEventSetting.Id != "")
                path = MapSetting.Instance.SavePath + MapEventSetting.Id + ".csv";

            return path;
        }
        #endregion
    }
}
