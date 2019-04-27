using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// TODO 刷新模式修改 根据数据刷新
/// </summary>

namespace PetMaze
{
    [CustomEditor(typeof(Map))]
    public class MapInspector : Editor
    {
        #region menber
        private Map _target;
        private Dictionary<int, Dictionary<int, GameObject>> _mapInstanceList;

        // 事件相关
        private GameObject _dragEvent = null;
        private Vector3 _mouseDownPos = Vector3.zero;
        private Vector3 _mouseUpPos = Vector3.zero;
        #endregion

        #region 基本函数
        private void Awake()
        {
            _target = (Map)target;
        }
        private void OnEnable()
        {
            _mapInstanceList = Map.Instance.MapInstanceList;
            EventWindow.DSelectEventIdCall += UpdateSelectEventInfo;
        }
        private void OnDisable()
        {
            EventWindow.DSelectEventIdCall -= UpdateSelectEventInfo;
        }
        #endregion

        #region 绘制
        #region 绘制属性
        //public override void OnInspectorGUI()
        //{
        //    EditorGUILayout.HelpBox("保存成功", MessageType.Info);
        //    // 绘制地图属性
        //    DrawMap();
        //}
        public void DrawMap()
        {
            EditorGUILayout.LabelField("地图属性", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            _target.Height = EditorGUILayout.IntField("行", Math.Max(0, _target.Height));
            _target.Width = EditorGUILayout.IntField("列", Math.Max(0, _target.Width));
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region 绘制场景
        private void OnSceneGUI()
        {
            EventHandheld();
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10f, 10f, 100f, 1000f));
            OnSceneCheckBtn();
            OnSceneCsvOpenBtn();
            OnSceneCsvSaveBtn();
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void OnSceneCheckBtn()
        {
            if (GUILayout.Button("检查配置文件是否合理"))
            {
                if (_target.CheckMapValid())
                    Debug.Log("检测通过");
                else
                    Debug.LogError("检测失败");
            }
        }
        private void OnSceneCsvOpenBtn()
        {
            if (GUILayout.Button("打开CSV文件"))
            {
                CsvData.Instance.FillCsv(_target.MapEventTypeList, _target.SavePath);
            }
        }
        private void OnSceneCsvSaveBtn()
        {
            if (GUILayout.Button("保存CSV文件"))
            {
                if (CsvData.Instance.Save(_target.MapEventTypeList, _target.SavePath))
                {
                    Debug.Log("保存成功");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("保存失败");
                }
            }
        }
        private void EventHandheld()
        {
            SceneView.currentDrawingSceneView.in2DMode = true;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Camera camera = SceneView.currentDrawingSceneView.camera;
            Vector2 mousePosition = Event.current.mousePosition;
            mousePosition.y = camera.pixelHeight - mousePosition.y;
            Vector3 wroldPosition = camera.ScreenToWorldPoint(mousePosition);
            Vector2 coordinate = Map.Instance.GetMapCoordinate(wroldPosition);

            // 事件相关
            EventType eventType = Event.current.type;
            if (eventType == EventType.MouseDown)
            {
                GameObject eventIns = GetEventIns(coordinate);
                if (eventIns != null) _dragEvent = eventIns;
                else _dragEvent = CreateEventIns(coordinate);
            }
            else if(eventType == EventType.MouseUp)
            {
                if (_dragEvent != null)
                {
                    _dragEvent = null;
                }
            }else if (eventType == EventType.MouseDrag)
            {
                if (_dragEvent != null)
                {
                    if (_target.IsPointValid((int)coordinate.x,(int)coordinate.y))
                    {
                        if (GetEventIns(coordinate) == null)
                        {
                            UpdateEventIns(_dragEvent, coordinate);
                            _dragEvent.transform.position = Map.Instance.GetWorldCoordinate(coordinate);
                            Repaint();
                        }
                    }
                }
            }
        }
        private GameObject GetEventIns(Vector2 coordinate)
        {
            int pointx = (int)coordinate.x;
            int pointy = (int)coordinate.y;
            if (!_target.IsPointValid(pointx, pointy))
            {
                return null;
            }
            return _mapInstanceList[pointx][pointy];
        }
        /// <summary>
        /// 可以将事件节点移出编辑范围
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="toCoordinate"></param>
        private void UpdateEventIns(GameObject ins, Vector2 toCoordinate)
        {
            int coorx = (int)toCoordinate.x;
            int coory = (int)toCoordinate.y;
            int pointx = -1;
            int pointy = -1;
            for(int i = 0;i< _mapInstanceList.Count; i++)
            {
                bool isBreak = false;
                for(int j = 0;j< _mapInstanceList[i].Count; j++)
                {
                    if (_mapInstanceList[i][j] == ins)
                    {
                        pointx = i;
                        pointy = j;
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }
            if (pointx != -1 && pointy != -1)
            {
                _mapInstanceList[pointx][pointy] = null;
            }
            _mapInstanceList[coorx][coory] = ins;
        }
        private GameObject CreateEventIns(Vector2 coordinate)
        {
            if (_target.SelectEventInfo == null)
            {
                Debug.LogError("请先选择一个事件");
                return null;
            }
            int pointx = (int)coordinate.x;
            int pointy = (int)coordinate.y;

            if (!_target.IsPointValid(pointx, pointy))
            {
                return null;
            }

            GameObject go = new GameObject(pointx + "X" + pointy);
            SpriteRenderer spRender = go.AddComponent<SpriteRenderer>();
            Texture2D tex = _target.SelectEventInfo.icon;
            spRender.sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f, 0.5f));
            go.transform.localScale = new Vector3(200 / tex.width, 200 / tex.height, 1);
            go.transform.position = Map.Instance.GetWorldCoordinate(coordinate);
            if (!_mapInstanceList.ContainsKey(pointx))
            {
                _mapInstanceList[pointx] = new Dictionary<int, GameObject>();
            }
            _mapInstanceList[pointx][pointy] = go;

            return go;
        }
        #endregion
        #endregion

        #region 帮助

        private bool CheckClick(Vector3 position)
        {
            bool isValid = false;



            return true;
        }
        private void UpdateSelectEventInfo(EventInfo eventInfo)
        {
            _target.UpdateSelectEventInfo(eventInfo);
        }
        #endregion
    }
}
