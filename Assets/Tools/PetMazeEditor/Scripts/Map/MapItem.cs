
using UnityEngine;
/// <summary>
/// 地块数据
/// </summary>
namespace PetMaze
{
    public class MapItem : MonoBehaviour
    {
        [SerializeField] private int _pointX = 0;
        [SerializeField] private int _pointY = 0;
        [SerializeField] private EventWrap _mapEventWrap = new EventWrap();
    }
}
