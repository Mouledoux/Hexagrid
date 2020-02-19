using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Biome))]
public class BiomeEditor : Editor
{
    public bool showSubBiomes = false;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        Biome parentBiome = (Biome)target;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Biome Tile", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 5f)});
        parentBiome.biomeTile = (GameObject)EditorGUILayout.ObjectField(parentBiome.biomeTile, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Biome Material", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 5f)});
        parentBiome.biomeMaterial = (Material)EditorGUILayout.ObjectField(parentBiome.biomeMaterial, typeof(Material), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Biome Decoration", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 5f)});
        parentBiome.biomeDeco = (GameObject)EditorGUILayout.ObjectField(parentBiome.biomeDeco, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField($"Biome", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 5f)});
            parentBiome.minBiomeVal = EditorGUILayout.FloatField(parentBiome.minBiomeVal, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 16)});
            parentBiome.maxBiomeVal = EditorGUILayout.FloatField(parentBiome.maxBiomeVal, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 16f)});
            EditorGUILayout.MinMaxSlider(ref parentBiome.minBiomeVal, ref parentBiome.maxBiomeVal, 0, 1);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField($"Elevation", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 5f)});
            parentBiome.minElevation = EditorGUILayout.FloatField(parentBiome.minElevation, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 16)});
            parentBiome.maxElevation = EditorGUILayout.FloatField(parentBiome.maxElevation, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 16)});
            EditorGUILayout.MinMaxSlider(ref parentBiome.minElevation, ref parentBiome.maxElevation, 0, 1);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField($"Temperature", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 5f)});
            parentBiome.minTemperature = EditorGUILayout.FloatField(parentBiome.minTemperature, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 16)});
            parentBiome.maxTemperature = EditorGUILayout.FloatField(parentBiome.maxTemperature, new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 16)});
            EditorGUILayout.MinMaxSlider(ref parentBiome.minTemperature, ref parentBiome.maxTemperature, 0, 1);
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();

        
        showSubBiomes = EditorGUILayout.Foldout(showSubBiomes, "Sub-Biomes");
        //if(showSubBiomes)
        {
            EditorGUI.indentLevel++;
            if(parentBiome.subBiomes != null)
            {
                for(int i = 0; i < parentBiome.subBiomes.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        parentBiome.subBiomes[i] = (Biome)EditorGUILayout.ObjectField(parentBiome.subBiomes[i], typeof(Biome), false,
                            new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 2f)});
                        parentBiome.subBiomes[i] = CheckForBiomeLoop(parentBiome) != null ? null : parentBiome.subBiomes[i];
                        

                        if(GUILayout.Button("Remove Sub-Biome"))
                        {
                            Biome[] sb = new Biome[parentBiome.subBiomes.Length - 1];
                            for(int j = 0; j < i; j++)
                            {
                                sb[j] = parentBiome.subBiomes[j];
                            }
                            for(int j = i; j < sb.Length; j++)
                            {
                                sb[j] = parentBiome.subBiomes[j+1];
                            }
                            parentBiome.subBiomes = sb;
                            return;
                        }
                    }
                    EditorGUILayout.EndHorizontal();


                    if(parentBiome.subBiomes[i] != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField($"Biome Range", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 4)});
                            EditorGUILayout.MinMaxSlider(ref parentBiome.subBiomes[i].minBiomeVal, ref parentBiome.subBiomes[i].maxBiomeVal, 0f, 1f);
                        }
                        EditorGUILayout.EndHorizontal();
                        

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField($"Elevation Range", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 4f)});
                            EditorGUILayout.MinMaxSlider(ref parentBiome.subBiomes[i].minElevation, ref parentBiome.subBiomes[i].maxElevation, 0f, 1f);
                        }
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField($"Temperature Range", new GUILayoutOption[] {GUILayout.Width(EditorGUIUtility.currentViewWidth / 4f)});
                            EditorGUILayout.MinMaxSlider(ref parentBiome.subBiomes[i].minTemperature, ref parentBiome.subBiomes[i].maxTemperature, 0f, 1f);
                        }
                        EditorGUILayout.EndHorizontal();
                    
                        EditorGUILayout.Space();
                    }
                }
            }

            if(GUILayout.Button("Add New Sub-Biome"))
            {
                Biome[] sb = parentBiome.subBiomes == null ? new Biome[1] : new Biome[parentBiome.subBiomes.Length + 1];
                for(int i = 0; i < parentBiome.subBiomes.Length; i++)
                {
                    sb[i] = parentBiome.subBiomes[i];
                }
                parentBiome.subBiomes = sb;
            }
        }
    }

    public Biome CheckForBiomeLoop(Biome rootBiome_)
    {
        if(rootBiome_.subBiomes.Length < 1) return null;

        List<Biome> subCircleOpen = new List<Biome>();
        List<Biome> subCircleClosed = new List<Biome>();

        foreach(Biome b in rootBiome_.subBiomes)
        {
            if(b != null)subCircleOpen.Add(b);
        }
        
        foreach(Biome b in subCircleOpen)
        {
            if(subCircleClosed.Contains(b) || b == rootBiome_)
            {
                Debug.LogWarning($"Cannot loop sub-biomes. {b.name} is a duplicate sub-biome.");
                return b;
            }

            foreach(Biome s in b.subBiomes)
                if(s != null) subCircleClosed.Add(s);
            
            subCircleClosed.Add(b);
        }

        return null;
    }
}
