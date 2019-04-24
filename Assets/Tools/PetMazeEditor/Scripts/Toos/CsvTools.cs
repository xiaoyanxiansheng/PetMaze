using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 参考 https://blog.csdn.net/eazey_wj/article/details/78821193
/// </summary>
namespace PetMaze
{
    public class CsvData
    {
        private static CsvData _instance;

        private List<string> _keyList = new List<string>();
        private List<List<string>> _valueList = new List<List<string>>();

        public static CsvData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CsvData();
                return _instance;
            }
        }

        public void Open(string path)
        {

        }

        public void Create()
        {

        }

        public void Save()
        {

        }

        public void Modify(int x, int y,string value)
        {
            if (IsValid(x, y))
                return;
            _valueList[x][y] = value;
        }

        public bool IsValid(int x, int y)
        {
            bool isValid = false;

            if (y >= 0 && y < _keyList.Count)
                if (x >= 0 && x < _valueList.Count && y < _valueList[x].Count)
                    isValid = true;

            return isValid;
        }
    }
}
