using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LevelScriptableFile))]
public class LevelScriptableFileEditor : Editor
{
    private int levelsAmmount = 1;
    private int nodesPerLevel = 3;
    private int nodesMaxRange = 5;
    private Grid grid;

    public override void OnInspectorGUI()
    {
        levelsAmmount = EditorGUILayout.IntField("Levels Amount", levelsAmmount);
        nodesPerLevel = EditorGUILayout.IntField("Nodes per level", nodesPerLevel);
        nodesMaxRange = EditorGUILayout.IntField("Nodes max range", nodesMaxRange);
        grid = EditorGUILayout.ObjectField("Grid reference", grid, typeof(Grid)) as Grid;

        EditorGUILayout.Space(15);

        if (GUILayout.Button("Create levels"))
        {
            if(!grid)
            {
                Debug.LogError("Grid reference necessary");
                return;
            }

            var levelFile = (LevelScriptableFile)target;
            var package = new LevelPackage();

            for (int i = 0; i < levelsAmmount; i++)
            {
                var level = LevelGenerator.CreateNewLevel(nodesPerLevel, nodesMaxRange, grid.gridSize);
                package.Add(level);
            }

            levelFile.levels.Add(package);
        }

        EditorGUILayout.Space(30);

        base.OnInspectorGUI();

    }

    private void OnEnable()
    {
        grid = FindObjectOfType<Grid>();
    }

}
