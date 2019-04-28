﻿using System;
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
        private EventItem _dragEvent = null;
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
            _target.MapEventSetting.Height = EditorGUILayout.IntField("行", Math.Max(0, _target.MapEventSetting.Height));
            _target.MapEventSetting.Width = EditorGUILayout.IntField("列", Math.Max(0, _target.MapEventSetting.Width));
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region 绘制场景
        private void OnSceneGUI()
        {
            EventHandleSetting();
            EventHandleHand();
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
                if (MapSetting.Instance.TemplateCsvPath == "")
                {
                    Debug.LogError("Csv模板没有设置");
                    return;
                }
                Debug.Log("配置检测通过");
            }
        }
        private void OnSceneCsvOpenBtn()
        {
            if (GUILayout.Button("打开CSV文件"))
            {
                //CsvData.Instance.FillCsv(_target.MapEventTypeList, _target.SavePath);
            }
        }
        private void OnSceneCsvSaveBtn()
        {
            if (GUILayout.Button("保存CSV文件"))
            {
                var list = _target.MapItemSetting.GetEventTrimList();
                if (CsvTools.Instance.Save(list, _target.GetSavePath()))
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
                ResetAllIns();
                Repaint();
            }
        }
        #region 事件相关
        private void EventHandleSetting()
        {
            Tools.current = Tool.None;
            SceneView.currentDrawingSceneView.in2DMode = true;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        private void EventHandleHand()
        {
            // 事件相关
            Camera camera = SceneView.currentDrawingSceneView.camera;
            Vector2 mousePosition = Event.current.mousePosition;
            mousePosition.y = camera.pixelHeight - mousePosition.y;
            Vector3 wroldPosition = camera.ScreenToWorldPoint(mousePosition);
            Vector2 coordinate = Map.Instance.GetMapCoordinate(wroldPosition);
            EventType eventType = Event.current.type;
            if (eventType == EventType.MouseDown)
            {
                EventItem eventItem = _target.GetEventItem((int)coordinate.x, (int)coordinate.y);
                int buttonIndex = Event.current.button;
                if (buttonIndex == 0)
                {
                    _dragEvent = eventItem;
                    if (eventItem != null && eventItem.Ins == null)
                    {
                        if (_target.SelectEventInfo != null)
                        {
                            _target.MapItemSetting.AddIns(eventItem, _target.SelectEventInfo.id);
                            Repaint();
                        }
                        else
                            Debug.LogError("请先选择一个事件");
                    }
                }
                else if (buttonIndex == 1)
                {
                    OpenEditorMenus(eventItem);
                }
            }
            else if (eventType == EventType.MouseUp)
            {
                if (_dragEvent != null) _dragEvent = null;
            }
            else if (eventType == EventType.MouseDrag)
            {
                if (_dragEvent != null && _dragEvent.Ins != null)
                {
                    EventItem toEventItem = _target.GetEventItem((int)coordinate.x, (int)coordinate.y);
                    if (_target.MapItemSetting.MoveIns(_dragEvent, toEventItem))
                        _dragEvent = toEventItem;
                }
            }
        }
        #endregion
        #region 编辑菜单
        private void OpenEditorMenus(EventItem eventItem)
        {
            if (eventItem == null || eventItem.Ins == null)
            {
                return;
            }
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("删除"), false, ClickDelete, eventItem);
            menu.AddItem(new GUIContent("编辑"), false, ClickEditor, eventItem);
            menu.ShowAsContext();
            Event.current.Use();
        }
        private void ClickDelete(object ob)
        {
            EventItem eventItem = (EventItem)ob;
            _target.MapItemSetting.DeleteIns(eventItem);
            Repaint();
        }
        private void ClickEditor(object ob)
        {
            Selection.activeGameObject = ((EventItem)(ob)).Ins;
        }
        #endregion

        /// <summary>
        /// 重刷整个地图实体
        /// </summary>
        private void ResetAllIns()
        {
            _target.MapItemSetting.DeleteAllIns();
            _target.MapItemSetting.CreateAllIns();
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
