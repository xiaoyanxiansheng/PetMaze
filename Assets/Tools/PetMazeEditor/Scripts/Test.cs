using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetMaze
{
    [ExecuteInEditMode]
    public class Test : MonoBehaviour
    {

        private void Update()
        {
            Map.Instance.AutoCatchMapCoordinate(transform);
        }

        private void OnDrawGizmos()
        {
            Color oldColor = Gizmos.color;

            Gizmos.color = Map.Instance.IsWorldCoordinateValid(transform.position) ? Color.green : Color.gray;
            Gizmos.DrawCube(transform.position,Vector3.one * Map.GridCellSize);

            Gizmos.color = oldColor;
        }
    }
}
