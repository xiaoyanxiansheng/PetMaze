using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetMaze
{
    public static class CommonTools
    {
        /// <summary>
        /// 拷贝控件
        /// </summary>
        /// <param name="com"></param>
        /// <param name="toGo"></param>
        public static void PasteComponentToGameObject(Component com, GameObject toGo)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(com);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(toGo);
        }

        public static int GetStrTableIndex(List<string> table,string str)
        {
            int index = -1;

            for(int i =0;i< table.Count; i++)
            {
                if (table[i] == str)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
    }
}
