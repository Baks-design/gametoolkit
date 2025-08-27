using UnityEditor;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Persistence
{
    [CustomEditor(typeof(PersistenceManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var persistenceManager = (PersistenceManager)target;
            var gameName = persistenceManager.GameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game"))
                persistenceManager.NewGame();
            if (GUILayout.Button("Save Game"))
                persistenceManager.SaveGame();
            if (GUILayout.Button("Load Game"))
                persistenceManager.LoadGame(gameName);
            if (GUILayout.Button("Delete Game"))
                persistenceManager.DeleteGame(gameName);
        }
    }
}
