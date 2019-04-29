using System.Text;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 注意点：表格key的顺序不能修改
/// 参考 https://blog.csdn.net/eazey_wj/article/details/78821193
/// </summary>
namespace PetMaze
{
    public class CsvTools
    {
        public const int EventListCount = 5;
        public const int RowOffset = 4;
        public const int CellOffset = 1;

        #region 私有变量
        private static CsvTools _instance;
        #endregion

        #region 私有方法
        private void FillAll(Dictionary<string, List<string>>  csvMap, string[] lines)
        {
            List<string> keys = GetKeys();
            int keysVount = keys.Count;
            for (int i = 0; i < lines.Length; i++)
            {
                List<string> list = new List<string>();
                FillOne(list, lines[i]);
                string index = list[CellOffset];
                if (i < keysVount)
                    index = keys[i];
                csvMap[index] = list;
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
        private string[] GetFileStr(Dictionary<string, List<string>> csvMap)
        {
            List<string> strs = new List<string>();
            foreach(string key in csvMap.Keys)
            {
                string str = "";
                string dian = "";
                List<string> values = csvMap[key];
                foreach(string s in values)
                {
                    str += dian + s;
                    dian = ",";
                }
                strs.Add(str);
            }
            return strs.ToArray();
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
        public static CsvTools Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CsvTools();
                return _instance;
            }
        }

        public void FillCsv(Dictionary<string, List<string>> csvMap, string path)
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

        public bool Save(Dictionary<string, List<string>> csvMap, string path)
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

        public static List<string> GetKeys()
        {
            return new List<string>() { "-4","-3","-2","-1"};
        }

        public static List<string> InitLineData(List<string> data)
        {
            List<string> list = new List<string>();
            list.Add("");
            list.AddRange(data);
            return list;
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
