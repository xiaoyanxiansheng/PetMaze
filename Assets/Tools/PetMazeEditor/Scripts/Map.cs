using System;
using System.Collections.Generic;
using UnityEngine;

namespace PetMaze
{

    [Serializable]
    public class EventWrap
    {
        public List<EventType> _fixedEventKeyList = new List<EventType>();    // 固定事件列表 id
        public List<int> _fixedEventValueList = new List<int>();              // 固定事件列表 个数
        public List<EventList> _randomEventList = new List<EventList>();      // 随机事件列表 id列表
        public List<int> _randomEventProbList = new List<int>();              // 随机事件列表 概率
    }

    [Serializable]
    public class EventList
    {
        public List<EventType> EList = new List<EventType>();
    }

    public class Map : MonoBehaviour
    {

        [SerializeField] private int _size = 10;                                    // 大小
        [SerializeField] private Theme _theme = Theme.Default;                      // 主题
        [SerializeField] private int _layer = 1;                                    // 层级
        [SerializeField] private EventWrap _mapEventWrap = new EventWrap();         // 地图配置
        [SerializeField] private EventWrap[] _areaEventWrap = new EventWrap[4];     // 区域配置 TODO 大小不许改变

        private int _height = 10;
        private int _width = 10;
        private Color _selectColor = new Color(1, 0.62f, 0.14f);
        private Color _disSelectColor = new Color(0.5f, 0.5f, 0.5f);
        private static Map _instance;
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

        public int Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
                
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
                Height = _size;
                Width = _size;
            }
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        private void DrawBorderGizmo()
        {
            float disWidth = _width * GridCellSize;
            float disHeight = _height * GridCellSize;
            Vector3 startPos = transform.position;
            Gizmos.DrawLine(startPos, startPos + new Vector3(disWidth, 0, 0));
            Gizmos.DrawLine(startPos, startPos + new Vector3(0, disHeight, 0));
            Gizmos.DrawLine(startPos + new Vector3(0, disHeight, 0), startPos + new Vector3(disWidth, disHeight, 0));
            Gizmos.DrawLine(startPos + new Vector3(disWidth, 0, 0), startPos + new Vector3(disWidth, disHeight, 0));
        }
        /// <summary>
        /// 绘制网格线
        /// </summary>
        private void DrawGridLineGizmo()
        {
            Vector3 startPos = transform.position;
            for (int i = 1; i < _width; i++)
            {
                Gizmos.DrawLine(startPos + new Vector3(GridCellSize * i, 0, 0), startPos + new Vector3(GridCellSize * i, GridCellSize * _height, 0));
            }
            for (int i = 1; i < _height; i++)
            {
                Gizmos.DrawLine(startPos + new Vector3(0, GridCellSize * i, 0), startPos + new Vector3(GridCellSize * _width, GridCellSize * i, 0));
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// 绘制gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = _disSelectColor;
            DrawBorderGizmo();
            DrawGridLineGizmo();
            Gizmos.color = oldColor;
        }

        /// <summary>
        /// gizmos被选中
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = _selectColor;
            DrawBorderGizmo();
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
            mapCoordinate.x = Mathf.Ceil(diff.x / GridCellSize);
            mapCoordinate.y = Mathf.Ceil(diff.y / GridCellSize);

            return mapCoordinate;
        }

        /// <summary>
        /// 地图点转化成世界点
        /// </summary>
        /// <param name="mapCoordinate"></param>
        public Vector3 GetWorldCoordinate(Vector2 mapCoordinate)
        {
            Vector3 worldCoordinate = new Vector3(0, 0, 0);

            worldCoordinate.x = (mapCoordinate.x - 0.5f) * GridCellSize;
            worldCoordinate.y = (mapCoordinate.y - 0.5f) * GridCellSize;

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

            if (mapCoordinate.x >= 1 && mapCoordinate.x <= _width 
                && mapCoordinate.y >= 1 && mapCoordinate.y <= _height)
            {
                isValid = true;
            }

            return isValid;
        }
        public bool IsWorldCoordinateValid(Vector3 worldCoordinate)
        {
            return IsMapCoordinateValid(GetMapCoordinate(worldCoordinate));
        }
    }
}
