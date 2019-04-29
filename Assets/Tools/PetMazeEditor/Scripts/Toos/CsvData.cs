﻿using System.Collections.Generic;
using UnityEngine;

namespace PetMaze {
    public class CsvData
    {
        public List<string> keys = new List<string>();
        public Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

        public CsvData(string path)
        {
            CsvTools.Instance.FillCsv(data, path);
            keys = data["-3"];
        }

        public void FillKeys(Dictionary<string, List<string>> table)
        {
            string[] ks = new string[] { "-4","-1","-2","-1"};
            for(int i=0;i< ks.Length; i++)
            {
                table[i.ToString()] = data[ks[i]];
            }
        }

        public int GetIntValue(string key1, string key2)
        {
            int value = 0;
            int.TryParse(GetValue(key1, key2), out value);
            return value;
        }

        public string GetValue(string key1 , string key2)
        {
            if (!IsExist(key1))
            {
                return "";
            }
            List<string> lineData = data[key1];
            return GetValue(lineData, key2);
        }

        public string GetValue(List<string> lineData,string keyName)
        {
            string value = "";

            int index = CommonTools.GetStrTableIndex(keys, keyName);
            if (index != -1)
                value = lineData[index];

            return value;
        }

        /// <summary>
        /// 存在就修改 不存在就加入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lineData"></param>
        public void Modify(Dictionary<string, string> dicData)
        {
            List<string> list = new List<string>();
            for(int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                string value = "";
                if (dicData.ContainsKey(key))
                    value = dicData[key];
                list.Add(value);
            }
            data[dicData["ID"]] = list;
        }

        public bool IsExist(string key)
        {
            bool isExist = false;

            if (data.ContainsKey(key))
            {
                isExist = true;
            }

            return isExist;
        }
    }
}