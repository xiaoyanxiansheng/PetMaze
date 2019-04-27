using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PetMaze
{
    [CustomEditor(typeof(Map))]
    public class MapInspector : Editor
    {
        private Map _target;
        private Dictionary<int, Dictionary<int, GameObject>> _mapInstanceList;
        private EventInfo _selectEventInfo;

        private void Awake()
        {
            _target = (Map)target;

            InitMapInstance();

            EventWindow.DSelectEventIdCall += UpdateSelectEventInfo;
        }

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
            GUILayout.BeginArea(new Rect(10f,10f,100f,1000f));
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
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            SceneView.currentDrawingSceneView.in2DMode = true;

            Camera camera = SceneView.currentDrawingSceneView.camera;
            Vector2 mousePosition = Event.current.mousePosition;
            mousePosition.y = camera.pixelHeight - mousePosition.y;
            Vector3 wroldPosition = camera.ScreenToWorldPoint(mousePosition);
            CreateEvent(wroldPosition);
        }
        #endregion

        #region 帮助
        private void InitMapInstance()
        {
            _mapInstanceList = new Dictionary<int, Dictionary<int, GameObject>>();
        }

        private void CreateEvent(Vector3 position)
        {
            if (_selectEventInfo == null)
            {
                Debug.LogError("请先选择一个事件");
                return;
            }
            if (!CheckClick(position))
            {
                Debug.LogError("点击非法");
                return;
            }
            if (GetInstance(position) != null)
            {
                Debug.LogError("请先删除当前位置事件");
                return;
            }
            Vector2 pos = Map.Instance.GetMapCoordinate(position);
            Vector2 mapCoordinate = Map.Instance.GetMapCoordinate(pos);
            GameObject go = new GameObject();
            go.transform.position = Map.Instance.GetWorldCoordinate(mapCoordinate);
            int pointx = (int)mapCoordinate.x;
            int pointy = (int)mapCoordinate.y;
            if (!_mapInstanceList.ContainsKey(pointx))
            {
                _mapInstanceList[pointx] = new Dictionary<int, GameObject>();
            }
            _mapInstanceList[pointx][pointy] = go;
        }

        private GameObject GetInstance(Vector3 pos)
        {
            GameObject go = null;

            Vector2 mapCoordinate = Map.Instance.GetMapCoordinate(pos);
            int pointx = (int)mapCoordinate.x;
            int pointy = (int)mapCoordinate.y;
            if (pointx <= _mapInstanceList.Count)
            {
                if (pointy <= _mapInstanceList[pointx].Count)
                {
                    go = _mapInstanceList[pointx][pointy];
                }
            }

            return go;
        }

        private bool CheckClick(Vector3 position)
        {
            bool isValid = false;



            return true;
        }

        private void UpdateSelectEventInfo(EventInfo eventInfo)
        {
            _selectEventInfo = eventInfo;
            Debug.Log("UpdateSelectEventInfo" + eventInfo.name);
        }
        #endregion
    }
}
