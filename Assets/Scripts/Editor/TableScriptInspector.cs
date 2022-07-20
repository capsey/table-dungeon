using TableDungeon.Maze;
using UnityEditor;
using UnityEngine;

namespace TableDungeon.Editor
{
    [CustomEditor(typeof(TableScript))]
    public class TableScriptInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (TableScript) target;
            
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Generate new"))
            {
                script.Generate();
            }
            if (GUILayout.Button("Open whole map"))
            {
                script.OpenEverything();
            }
            GUI.enabled = true;
        }
    }
}