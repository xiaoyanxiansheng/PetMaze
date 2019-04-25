using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 参考 https://blog.csdn.net/eazey_wj/article/details/78821193
/// </summary>
namespace PetMaze
{
    public class CsvData
    {
        #region 私有变量
        private static CsvData _instance;
        #endregion

        #region 私有方法
        private void FillAll(Dictionary<int, List<string>>  csvMap, string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                csvMap[i] = new List<string>();
                FillOne(csvMap[i], lines[i]);
            }
        }
        private void FillOne(List<string> fill, string s)
        {
            string[] splitStr = s.Split(',');
            for (int j = 0; j < splitStr.Length; j++)
            {
                fill.Add(splitStr[j]);
            }
        }
        private string[] GetFileStr(Dictionary<int, List<string>> csvMap)
        {
            string[] linesStr = new string[csvMap.Count];
            for (int i = 0; i < csvMap.Count; i++){
                linesStr[i] = "";
                for (int j = 0;j < csvMap[i].Count; j++)
                {
                    if (j == 0)
                    {
                        linesStr[i] += csvMap[i][j];
                    }
                    else
                    {
                        linesStr[i] += "," + csvMap[i][j];
                    }
                }
            }
            return linesStr;
        }
        private bool IsValid(int x, int y)
        {
            return true;
        }
        private bool IsPathValid(string path)
        {
            return path.Trim() != "";
        }
        #endregion

        #region 增删改存开
        public static CsvData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CsvData();
                return _instance;
            }
        }

        public void FillCsv(Dictionary<int, List<string>> csvMap, string path)
        {
            if (!IsPathValid(path))
            {
                Debug.LogError("路径不合法 : " + path);
                return;
            }
            if (!File.Exists(path))
            {
                Debug.LogError("文件不存在 : " + path);
                return;
            }
            string[] lines = File.ReadAllLines(path);
            FillAll(csvMap, lines);
        }

        public bool Save(Dictionary<int, List<string>> csvMap, string path)
        {
            if (!IsPathValid(path))
            {
                Debug.LogError("路径不合法 : " + path);
                return false;
            }

            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
                fs.Dispose();
            }

            string[] fileStr = GetFileStr(csvMap);
            File.WriteAllLines(path, fileStr);
            return true;
        }
        #endregion

        #region 测试
        public string GetLogString(Dictionary<int, List<string>> csvMap)
        {
            string logStr = "";

            for(int i = 0; i < csvMap.Count; i++)
            {
                for(int j = 0; j < csvMap[i].Count; j++)
                {
                    logStr += "_" + csvMap[i][j];
                }
                logStr += "\n";
            }

            return logStr;
        }

        public void Log(Dictionary<int, List<string>> csvMap)
        {
            Debug.Log(GetLogString(csvMap));
        }
        #endregion
    }
}
