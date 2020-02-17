// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;

// [CustomEditor(typeof(SpawnGrid))]
// public class SpawnGridEditor : Editor
// {
//     public bool showTiles = false;
//     public override void OnInspectorGUI()
//     {
//         SpawnGrid spawnGrid = (SpawnGrid)target;
        
//         showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");
//         if(showTiles)
//         {
//             EditorGUI.indentLevel++;

//             for(int i = 0; i < spawnGrid.perlinTiles.Length; i++)
//             {
//                 EditorGUILayout.BeginHorizontal();
                
//                 spawnGrid.perlinTiles[i].tilePrefab = (GameObject)EditorGUILayout.ObjectField(spawnGrid.perlinTiles[i].tilePrefab, typeof(GameObject), false, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 3f)});
                
//                 spawnGrid.perlinTiles[i].elevation =      (PerlinTile.Elevations)EditorGUILayout.EnumPopup(spawnGrid.perlinTiles[i].elevation);
//                 spawnGrid.perlinTiles[i].tempreature =    (PerlinTile.Tempreatures)EditorGUILayout.EnumPopup(spawnGrid.perlinTiles[i].tempreature);
//                 spawnGrid.perlinTiles[i].biome =          (PerlinTile.Biomes)EditorGUILayout.EnumPopup(spawnGrid.perlinTiles[i].biome);

//                 if(GUILayout.Button("X"))
//                 {
//                     PerlinTile[] pt = new PerlinTile[spawnGrid.perlinTiles.Length - 1];
//                     for(int j = 0; j < i; j++)
//                     {
//                         pt[j] = spawnGrid.perlinTiles[j];
//                     }
//                     for(int j = i + 1; j < pt.Length; j++)
//                     {
//                         pt[j] = spawnGrid.perlinTiles[j];
//                     }
//                     spawnGrid.perlinTiles = pt;
//                 }
//                 EditorGUILayout.EndHorizontal();
//             }

//             EditorGUI.indentLevel = 0;

//             if(GUILayout.Button("Add tile"))
//             {
//                 PerlinTile[] pt = new PerlinTile[spawnGrid.perlinTiles.Length + 1];
//                 for(int i = 0; i < spawnGrid.perlinTiles.Length; i++)
//                 {
//                     pt[i] = spawnGrid.perlinTiles[i];
//                 }
//                 spawnGrid.perlinTiles = pt;
//             }
//         }


//         base.OnInspectorGUI();
//     }
// }
