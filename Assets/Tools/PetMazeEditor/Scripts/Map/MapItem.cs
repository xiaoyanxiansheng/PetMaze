
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

        public int PointX
        {
            get
            {
                return _pointX;
            }

            set
            {
                _pointX = value;
            }
        }

        public int PointY
        {
            get
            {
                return _pointY;
            }

            set
            {
                _pointY = value;
            }
        }
    }
}
