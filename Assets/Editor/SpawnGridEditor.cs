using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnGrid))]
public class SpawnGridEditor : Editor
{
    public bool showTiles = false;
    public override void OnInspectorGUI()
    {
        SpawnGrid spawnGrid = (SpawnGrid)target;
        
        showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");
        if(showTiles)
        {
            EditorGUI.indentLevel++;

            for(int i = 0; i < spawnGrid.Hexatiles.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                spawnGrid.Hexatiles[i].tilePrefab = (GameObject)EditorGUILayout.ObjectField(spawnGrid.Hexatiles[i].tilePrefab, typeof(GameObject), false, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 2f)});
                spawnGrid.Hexatiles[i].heightOffset = EditorGUILayout.Slider(spawnGrid.Hexatiles[i].heightOffset, 0f, 1f);
                if(GUILayout.Button("X"))
                {
                    PerlinTile[] pt = new PerlinTile[spawnGrid.Hexatiles.Length - 1];
                    for(int j = 0; j < i; j++)
                    {
                        pt[j] = spawnGrid.Hexatiles[j];
                    }
                    for(int j = i + 1; j < pt.Length; j++)
                    {
                        pt[j-1] = spawnGrid.Hexatiles[j];
                    }
                    spawnGrid.Hexatiles = pt;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel = 0;

            if(GUILayout.Button("Add tile"))
            {
                PerlinTile[] pt = new PerlinTile[spawnGrid.Hexatiles.Length + 1];
                for(int i = 0; i < spawnGrid.Hexatiles.Length; i++)
                {
                    pt[i] = spawnGrid.Hexatiles[i];
                }
                spawnGrid.Hexatiles = pt;
            }
        }


        base.OnInspectorGUI();
    }
}
