using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

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
        // 动画相关
        private AnimBool fadeGroup1;
        private AnimBool fadeGroup2;
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

            // fadeGroup1 = new AnimBool(true);
            // fadeGroup1.valueChanged.AddListener(Repaint);
        }
        private void OnDisable()
        {
            EventWindow.DSelectEventIdCall -= UpdateSelectEventInfo;
        }
        #endregion

        #region 绘制
        #region 绘制属性
        public override void OnInspectorGUI()
        {
            // 绘制地图属性
            DrawMap();
        }
        public void DrawMap()
        {
            base.DrawDefaultInspector();

            // EditorGUILayout.PropertyField(serializedObject.FindProperty("MapEventSetting.Theme"));
            //EditorGUILayout.LabelField("地图属性", EditorStyles.boldLabel);
            //EditorGUILayout.BeginVertical("box");
            //_target.MapEventSetting.Size = EditorGUILayout.IntField("行", Math.Max(0, _target.MapEventSetting.Height));
            //_target.MapEventSetting.Size = EditorGUILayout.IntField("列", Math.Max(0, _target.MapEventSetting.Width));
            //EditorGUILayout.EndHorizontal();
            //GUIContent conten = new GUIContent();
            //conten.text = "aaa";
            //GUILayoutOption[] options = new GUILayoutOption[4];
            //options[0] = new GUILayoutOption();
            //EditorGUILayout.DropdownButton(conten,FocusType.Keyboard);


            /*if (EditorGUILayout.PropertyField(eventSetting))
            {
                EditorGUI.indentLevel++;
                eventSetting.arraySize = EditorGUILayout.DelayedIntField("Size", eventSetting.arraySize);
                int size = eventSetting.arraySize;
                for (int i = 0; i < size; i++)
                {
                    EditorGUILayout.PropertyField(eventSetting.GetArrayElementAtIndex(i));
                }
                EditorGUI.indentLevel--;
            }*/
            //fadeGroup.target = EditorGUILayout.Foldout(fadeGroup.target, "BeginFadeGroup");
            //if (EditorGUILayout.BeginFadeGroup(fadeGroup.faded))
            //{
            //    EditorGUI.indentLevel++;
            //    var eventSetting = serializedObject.FindProperty("AreaEventSetting.AreaItems");
            //    EditorGUILayout.PropertyField(eventSetting, true);
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("MapEventSetting"), true);
            //    this.selectOption = EditorGUILayout.IntPopup("IntPopup", (int)this.selectOption, new string[] { "01", "1", "2" }, new int[] { 0, 1, 2 });
            //    this.selectOption = EditorGUILayout.IntSlider("IntSlider", (int)this.selectOption, 0, 2);
            //    this.selectOption = EditorGUILayout.MaskField("MaskField", (int)this.selectOption, new string[] { "mask1", "mask2", "mask3" });
            //    this.selectOption = EditorGUILayout.Popup("Popup", (int)this.selectOption, new string[] { "s1", "s2", "s3" });
            //    EditorGUI.indentLevel--;
            //}
            //EditorGUILayout.EndFadeGroup();

            //serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 绘制场景
        private void OnSceneGUI()
        {
            EventHandleSetting();
            EventHandleHand();
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10f, 10f, 100f, 1000f));
            OnSceneDeleteBtn();
            GUILayout.Space(10);
            //OnSceneCheckBtn();
            GUILayout.Space(10);
            OnSceneCsvSaveBtn();
            OnSceneLoadCsvBtn();
            GUILayout.EndArea();
            Handles.EndGUI();
        }
        private void OnSceneDeleteBtn()
        {
            if (GUILayout.Button("清空"))
            {
                if (EditorUtility.DisplayDialog("确认", "是否清空?", "ok","cancel"))
                {
                    _target.MapItemSetting.DeleteAllIns();
                    Repaint();
                }
            }
        }
        private void OnSceneCheckBtn()
        {
            if (GUILayout.Button("检查"))
            {
                if (MapSetting.Instance.Path == "")
                {
                    Debug.LogError("保存路径没有设置");
                    return;
                }
                Debug.Log("配置检测通过");
            }
        }
        private void OnSceneLoadCsvBtn()
        {
            if (GUILayout.Button("Csv加载"))
            {
                if (_target.MapEventSetting.Id == "")
                {
                    Debug.LogError("请先设置地图Id后再点击LoadCsv");
                    return;
                }
                _target.ResetInitEventList();
                Debug.Log("加载成功");
            }
        }
        
        private void OnSceneCsvSaveBtn()
        {
            if (GUILayout.Button("CSV保存"))
            {
                if (_target.MapEventSetting.Id == "")
                {
                    Debug.LogError("请先设置地图Id后再点击保存");
                    return;
                }
                // 保存文件
                if (_target.SaveFiles())
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
            int pointx = (int)coordinate.x;
            int pointy = (int)coordinate.y;
            EventType eventType = Event.current.type;
            if (eventType == EventType.MouseDown)
            {
                EventItem eventItem = _target.GetEventItem(pointx, pointy);
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
                if (_dragEvent != null)
                {
                    //if (_dragEvent.PointX == pointx && _dragEvent.PointY == pointy)
                    //{
                    //    ClickEditor(_dragEvent);
                    //}
                    _dragEvent = null;
                }
            }
            else if (eventType == EventType.MouseDrag)
            {
                if (_dragEvent != null && _dragEvent.Ins != null)
                {
                    EventItem toEventItem = _target.GetEventItem(pointx, pointy);
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
