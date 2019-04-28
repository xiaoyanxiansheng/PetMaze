using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 地块数据 这里作为数据和实体的桥梁
/// </summary>
namespace PetMaze
{
    [Serializable]
    public class EventItem
    {
        public int PointX = 0;
        public int PointY = 0;
        public List<string> ValueList = new List<string>();

        public GameObject Ins;
    }

    [ExecuteInEditMode]
    [Serializable]
    public class MapItem
    {
        #region menber
        public List<List<string>> KeyList = new List<List<string>>();
        public List<EventItem> EventList = new List<EventItem>();
        #endregion

        #region 数据层
        /// <summary>
        /// 舒适化所有数据
        /// </summary>
        public void InitEventList(string path)
        {
            if (path.Trim() == "")
            {
                Debug.LogError("打开文件路径为空 " + path);
                return;
            }
            if (!File.Exists(path))
            {
                Debug.LogError("打开文件不存在 " + path);
                return;
            }
            EventList.Clear();
            Dictionary<string, List<string>> csvData = new Dictionary<string, List<string>>();
            CsvTools.Instance.FillCsv(csvData, path);
            int width = Map.Instance.MapEventSetting.Width;
            int height = Map.Instance.MapEventSetting.Height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    EventItem evetnItem = new EventItem();
                    EventList.Add(evetnItem);
                    evetnItem.PointX = i;
                    evetnItem.PointY = j;

                    int index = i * width + j + 1;
                    if (csvData.ContainsKey(index.ToString()))
                    {
                        evetnItem.ValueList.Clear();
                        List<string> eventInfo = csvData[index.ToString()];
                        for(int ei = 0; ei < CsvTools.EventListCount; ei++)
                        {
                            string eId = eventInfo[ei + CsvTools.CellOffset + 1];
                            if (eId.Trim() != "")
                            {
                                evetnItem.ValueList.Add(eId);
                            }
                        }
                    }
                }
            }
            // key
            KeyList.Clear();
            List<string> keys = new List<string> { "id", "ID", "int" };
            foreach(string key in keys)
            {
                KeyList.Add(csvData[key]);
            }
        }
        #endregion

        #region 实体层
        /// <summary>
        /// 移动实体
        /// </summary>
        /// <param name="eventItem"></param>
        /// <param name="toEventItem"></param>
        public bool MoveIns(EventItem eventItem , EventItem toEventItem)
        {
            if (eventItem == null || toEventItem == null)
            {
                return false;
            }
            if(eventItem.PointX == toEventItem.PointX && eventItem.PointY == toEventItem.PointY)
            {
                return false;
            }
            if (toEventItem.Ins != null)
            {
                return false;
            }
            GameObject ins = eventItem.Ins;
            toEventItem.ValueList = eventItem.ValueList;
            toEventItem.Ins = ins;
            eventItem.ValueList = new List<string>();
            eventItem.Ins = null;

            if (toEventItem.Ins != null)
            {
                toEventItem.Ins.transform.position = Map.Instance.GetWorldCoordinate(new Vector2(toEventItem.PointX, toEventItem.PointY));
                SetInsName(toEventItem);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 卸载所有实体
        /// </summary>
        public void DeleteAllIns()
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                DeleteIns(EventList[i]);
            }
        }
        public void DeleteIns(EventItem eventItem)
        {
            if (eventItem == null || eventItem.Ins == null)
                return;
            GameObject.DestroyImmediate(eventItem.Ins);
            eventItem.Ins = null;
            eventItem.ValueList.Clear();
        }
        /// <summary>
        /// 创建所有实体
        /// </summary>
        public void CreateAllIns()
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                EventItem item = EventList[i];
                CreateIns(item);
            }
        }
        public void AddEventList(EventItem eventItem,string id)
        {
            if (eventItem == null) return;
            int addIndex = 0;
            for(int i = 0; i < eventItem.ValueList.Count; i++)
            {
                if (eventItem.ValueList[i].Trim() == "")
                {
                    addIndex = i;
                    break;
                }
            }
            eventItem.ValueList.Add(id);
        }
        public void AddIns(EventItem eventItem, string id)
        {
            AddEventList(eventItem, id);
            CreateIns(eventItem);
        }
        public void CreateIns(EventItem eventItem)
        {
            if (eventItem == null) return;
            List<string> eventList = eventItem.ValueList;
            if (eventList == null || eventList.Count == 0) return;

            string eventId = eventList[0];
            EventInfo eventInfo = MapSetting.Instance.GetEventInfo(eventId);
            if (eventInfo == null)
            {
                Debug.LogError("本地配置和表格配置不匹配 错误事件Id: " + eventId);
                return;
            }
            GameObject go = new GameObject();
            SpriteRenderer spRender = go.AddComponent<SpriteRenderer>();
            Texture2D tex = eventInfo.icon;
            spRender.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            go.transform.localScale = new Vector3(200 / tex.width, 200 / tex.height, 1);
            go.transform.position = Map.Instance.GetWorldCoordinate(new Vector2(eventItem.PointX, eventItem.PointY));
            go.transform.hideFlags = HideFlags.HideInHierarchy;
            go.transform.parent = Map.Instance.transform;
            eventItem.Ins = go;
            SetInsName(eventItem);
            MapItemCom com = go.AddComponent<MapItemCom>();
            com.InitValues(eventItem.ValueList);
        }
        /// <summary>
        /// 根据行列坐标获取事件
        /// </summary>
        /// <param name="pointx"></param>
        /// <param name="pointy"></param>
        /// <returns></returns>
        public EventItem GetEventItem(int pointx, int pointy)
        {
            EventItem rItem = null;

            for(int i = 0;i< EventList.Count; i++)
            {
                EventItem item = EventList[i];
                if (item.PointX == pointx && item.PointY == pointy)
                {
                    rItem = item;
                    break;
                }
            }

            return rItem;
        }
        /// <summary>
        /// 获取event索引
        /// </summary>
        /// <param name="eventItem"></param>
        /// <returns></returns>
        public Vector2 GetEventItemPos(EventItem eventItem)
        {
            Vector2 posVec = Vector2.zero;

            for (int i = 0; i < EventList.Count; i++)
            {
                EventItem item = EventList[i];
                if (item == eventItem)
                {
                    posVec.x = item.PointX;
                    posVec.y = item.PointY;
                    break;
                }
            }

            return posVec;
        }
        /// <summary>
        /// 设置实体名字
        /// </summary>
        /// <param name="eventItem"></param>
        public void SetInsName(EventItem eventItem)
        {
            if (eventItem == null || eventItem.Ins == null)
                return;
            eventItem.Ins.name = eventItem.PointX + "X" + eventItem.PointY;
        }
        /// <summary>
        /// 得到去零的数据 为了节约内存
        /// </summary>
        /// <returns></returns>
        public Dictionary<int,List<string>> GetEventTrimList()
        {
            Dictionary<int, List<string>> eventTrimList = new Dictionary<int, List<string>>();

            // 加入key
            int lineCount = KeyList[0].Count;
            for(int i = 0; i < KeyList.Count; i++)
            {
                eventTrimList[i] = KeyList[i];
            }

            int index = eventTrimList.Count;
            int width = Map.Instance.MapEventSetting.Width;
            int height = Map.Instance.MapEventSetting.Height;
            // 加入数据
            for (int i = 0; i < EventList.Count; i++)
            {
                EventItem eventItem = EventList[i];
                if (eventItem.ValueList.Count > 0 && eventItem.Ins != null)
                {
                    List<string> values = new List<string>();
                    values.Add("");
                    values.Add((eventItem.PointX * width + eventItem.PointY + 1).ToString());
                    foreach(string v in eventItem.ValueList)
                    {
                        values.Add(v);
                    }
                    for(int j = values.Count;j< lineCount; j++)
                    {
                        values.Add("");
                    }
                    eventTrimList[index] = values;
                    index++;
                }
            }

            return eventTrimList;
        }
        #endregion
    }
}
