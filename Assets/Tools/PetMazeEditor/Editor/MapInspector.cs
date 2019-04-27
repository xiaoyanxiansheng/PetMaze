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
            EventWindow.DSelectEventIdCall += UpdateSelectEventInfo;
            CreateEventInsAll();
            _target.transform.hideFlags = HideFlags.NotEditable;
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
            OnSceneRefreshMapBtn();
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
        private void OnSceneRefreshMapBtn()
        {
            if (GUILayout.Button("刷新整个地图"))
            {
                DeleteEventInsAll();
                CreateEventInsAll();
                Repaint();
            }
        }
        /// <summary>
        /// 事件相关
        /// </summary>
        private void EventHandheld()
        {
            Tools.current = Tool.None;
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
                int buttonIndex = Event.current.button;
                if (buttonIndex == 0)
                {
                    if (eventIns != null) _dragEvent = eventIns;
                    else _dragEvent = RefreshAddEventIns(coordinate);
                }else if (buttonIndex == 1)
                {
                    if (eventIns != null)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("删除"), false, ClickDelete, eventIns);
                        menu.AddItem(new GUIContent("编辑"), false, ClickEditor, eventIns);
                        menu.ShowAsContext();
                        Event.current.Use();
                    }
                }
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
                    RefreshUpdateCoordinate(GetEventInsPoint(_dragEvent), coordinate);
                }
            }
        }
        /// <summary>
        /// 改变
        /// </summary>
        /// <param name="preCoordinate"></param>
        /// <param name="coordinate"></param>
        private bool UpdateCoordinate(Vector2 preCoordinate,Vector2 coordinate)
        {
            if (GetEventIns(coordinate) != null)
            {
                return false;
            }
            int prePointx = (int)preCoordinate.x;
            int prePointy = (int)preCoordinate.y;
            if (!_target.IsPointValid(prePointx, prePointy))
            {
                return false;
            }
            int pointx = (int)coordinate.x;
            int pointy = (int)coordinate.y;
            if (!_target.IsPointValid(pointx, pointy))
            {
                return false;
            }
            _target.MapInstanceList[pointx][pointy] = _target.MapInstanceList[prePointx][prePointy];
            _target.MapInstanceList[prePointx][prePointy] = null;
            _target.MapInstanceList[pointx][pointy].transform.position = Map.Instance.GetWorldCoordinate(coordinate);
            return true;
        }
        private void RefreshUpdateCoordinate(Vector2 preCoordinate, Vector2 coordinate)
        {
            if (UpdateCoordinate(preCoordinate, coordinate))
            {
                Repaint();
            }
        }
        private void ClickDelete(object ob)
        {
            GameObject go = (GameObject)ob;
            RefreshDeleteEventIns(GetEventInsPoint(go));
        }
        private void ClickEditor(object ob)
        {

        }
        private GameObject GetEventIns(Vector2 coordinate)
        {
            int pointx = (int)coordinate.x;
            int pointy = (int)coordinate.y;
            if (!_target.IsPointValid(pointx, pointy))
            {
                return null;
            }
            return _target.MapInstanceList[pointx][pointy];
        }
        private Vector2 GetEventInsPoint(GameObject eventIns)
        {
            Vector2 vec = new Vector2(-1,-1);
            for (int i = 0; i < _target.MapInstanceList.Count; i++)
            {
                bool isBreak = false;
                for (int j = 0; j < _target.MapInstanceList[i].Count; j++)
                {
                    if (_target.MapInstanceList[i][j] == eventIns)
                    {
                        vec.x = i;
                        vec.y = j;
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }
            return vec;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="coordinate"></param>
        private void DeleteEventIns(Vector2 coordinate)
        {
            int coorx = (int)coordinate.x;
            int coory = (int)coordinate.y;
            if (_target.IsPointValid(coorx, coory))
            {
                GameObject go = _target.MapInstanceList[coorx][coory];
                if (go != null) DestroyImmediate(go);
                _target.MapInstanceList[coorx][coory] = null;
            }
        }
        /// <summary>
        /// 删除之后刷新
        /// </summary>
        /// <param name="coordinate"></param>
        private void RefreshDeleteEventIns(Vector2 coordinate)
        {
            DeleteEventIns(coordinate);
            Repaint();
        }
        private void DeleteEventInsAll()
        {
            foreach(Dictionary<int, GameObject> gos in _target.MapInstanceList.Values)
            {
                foreach(GameObject go in gos.Values)
                {
                    DestroyImmediate(go);
                }
            }
            Map.Instance.InitMapInstance();
        }
        /// <summary>
        /// 增加 
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        private GameObject AddEventIns(Vector2 coordinate,GameObject addIns = null)
        {
            GameObject eventIns = GetEventIns(coordinate);
            if (eventIns != null)
            {
                return null;
            }

            int pointx = (int)coordinate.x;
            int pointy = (int)coordinate.y;

            if (!_target.MapInstanceList.ContainsKey(pointx))
            {
                _target.MapInstanceList[pointx] = new Dictionary<int, GameObject>();
            }

            if (addIns == null)
            {
                addIns = CreateEventIns(coordinate);
            }

            _target.MapInstanceList[pointx][pointy] = addIns;

            return addIns;
        }
        /// <summary>
        /// 增加之后刷新
        /// </summary>
        /// <param name="coordinate"></param>
        private GameObject RefreshAddEventIns(Vector2 coordinate)
        {
            GameObject go = AddEventIns(coordinate);
            Repaint();
            return go;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
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
            go.transform.hideFlags = HideFlags.NotEditable;
            return go;
        }
        private void CreateEventInsAll()
        {
            // TODO 根据数据创建全部

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
