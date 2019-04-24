using UnityEditor;

namespace PetMaze
{
    public static class MenuItems
    {
        [MenuItem("PetMaze/CreateNewScene")]
        public static void CreateNewScene()
        {
            EditorUtils.CreateNewScene();
        }
    }
}
