using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetMaze
{
    public static class CommonTools
    {
        public static void PasteComponentToGameObject(Component com, GameObject toGo)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(com);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(toGo);
        }
    }
}
