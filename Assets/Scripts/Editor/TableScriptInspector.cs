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
            if (GUILayout.Button("Generate new"))
            {
                var script = (TableScript) target;
                script.Generate();
            }
        }
    }
}