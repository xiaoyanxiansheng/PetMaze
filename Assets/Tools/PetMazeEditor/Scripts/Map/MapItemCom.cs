using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PetMaze
{
    [ExecuteInEditMode]
    public class MapItemCom : MonoBehaviour
    {

        public List<string> ValueList = new List<string>();

        public void InitValues(List<string> values)
        {
            ValueList = values;
        }

        void OnValidate()
        {
            Debug.Log(" OnValidate");
        }

        void OnDrawGizmosSelected()
        {
            Tools.current = Tool.None;
            if (Selection.activeGameObject == null || Selection.activeGameObject != gameObject)
                return;
            Color oldColor = Gizmos.color;
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f);
            float halfCellSize = Map.GridCellSize * 0.5f;
            Vector3 pos = transform.position;
            Gizmos.DrawLine(pos + new Vector3(-halfCellSize, -halfCellSize, 0), pos + new Vector3(halfCellSize, halfCellSize, 0));
            Gizmos.DrawLine(pos + new Vector3(-halfCellSize, halfCellSize, 0), pos + new Vector3(halfCellSize, -halfCellSize, 0));
            Gizmos.color = oldColor;
        }
    }
}
