using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBiome", menuName = "Biome/Biome")]
public class Biome : ScriptableObject
{
    public GameObject biomeTile;


    [Space]
    [Range(0f, 1f)]
    public float minBiomeVal;
    [Range(0f, 1f)]
    public float maxBiomeVal;

    public float averageBiome => (minBiomeVal + maxBiomeVal) / 2f;



    [Space]
    [Range(0f, 1f)]
    public float minElevation;
    [Range(0f, 1f)]
    public float maxElevation;

    public float averageElevation => (minElevation + maxElevation) / 2f;



    [Space]
    [Range(0f, 1f)]
    public float minTemperature;
    [Range(0f, 1f)]
    public float maxTemperature;

    public float averageTemperature => (minTemperature + maxTemperature) / 2f;



    [Space]
    public Biome[] subBiomes;


    public Biome(float minBiome_ = 0f, float maxBiome_ = 1f,
        float minElevation_ = 0f, float maxElevation_ = 1f,
        float minTemperature_ = 0f, float maxTemperature_ = 1f)
    {
        minBiomeVal = minBiome_;
        maxBiomeVal = maxBiome_;

        minElevation = minElevation_;
        maxElevation = maxElevation_;

        minTemperature = minTemperature_;
        maxTemperature = maxTemperature_;
    }


    public void Initialize()
    {
        for(int i = 0; i < subBiomes.Length; i++)
        {
            if(subBiomes[i].minBiomeVal < minBiomeVal) subBiomes[i].minBiomeVal = minBiomeVal;
            if(subBiomes[i].maxBiomeVal > maxBiomeVal) subBiomes[i].maxBiomeVal = maxBiomeVal;

            if(subBiomes[i].minElevation < minElevation) subBiomes[i].minElevation = minElevation;
            if(subBiomes[i].maxElevation > maxElevation) subBiomes[i].maxElevation = maxElevation;

            if(subBiomes[i].minTemperature < minTemperature) subBiomes[i].minTemperature = minTemperature;
            if(subBiomes[i].maxTemperature > maxTemperature) subBiomes[i].maxTemperature = maxTemperature;
        }
    }

    public Biome GetSubBiome(float biome_, float elevation_, float temperature_, float bias_ = float.MaxValue)
    {
        Initialize();

        if(biome_ < minBiomeVal || biome_ > maxBiomeVal ||
            elevation_ < minElevation || elevation_ > maxElevation ||
                temperature_ < minTemperature || temperature_ > maxTemperature)
                    return null;

        float biomeBias = (maxBiomeVal - minBiomeVal) + (biome_ * 2);
        float elevationBias = (maxElevation - minElevation) + (elevation_ * 2);
        float temperatureBias = (maxTemperature - minTemperature) + (temperature_ * 2);
        float bias = biomeBias + elevationBias + temperatureBias;

        if(bias <= bias_)
        {
            foreach(Biome b in subBiomes)
            {
                Biome sub = b.GetSubBiome(biome_, elevation_, temperature_, bias);
                if(sub != null)
                {
                    return sub.GetSubBiome(biome_, elevation_, temperature_, bias);
                }
            }
        }
        else
        {
            return null;
        }
        return this;
    }
}
