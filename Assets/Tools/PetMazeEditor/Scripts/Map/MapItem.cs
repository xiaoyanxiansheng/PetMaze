
using UnityEngine;
/// <summary>
/// 地块数据
/// </summary>
namespace PetMaze
{
    public class MapItem : MonoBehaviour
    {
        #region 配置
        [SerializeField] private int _eventType = 0;
        [SerializeField] private int _eventFatherType = 0;
        [SerializeField] private EventWrap _mapEventWrap = new EventWrap();
        #endregion

        private int _pointX = 0;
        private int _pointY = 0;
    }
}
