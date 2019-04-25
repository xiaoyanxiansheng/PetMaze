using UnityEditor;

namespace PetMaze
{
    public static class MenuItems
    {
        [MenuItem("PetMaze/新地图")]
        public static void CreateNewScene()
        {
            EditorUtils.CreateNewScene();
        }

        [MenuItem("PetMaze/事件窗口 %_e")]
        public static void OpenEventWindow()
        {
            EventWindow.OpenWindow();
        }
    }
}
