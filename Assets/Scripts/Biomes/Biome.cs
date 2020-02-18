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


    public Biome GetSubBiome(float biome_, float elevation_, float temperature_, ref float bias_, Biome parentBiome = null)
    {
        
        System.Func<float, float, float, float> getSubOffset = (pMin, pMax, cMod) => (pMin + (pMax - pMin) * cMod);

        float tBiomeMin = minBiomeVal;
        float tBiomeMax = maxBiomeVal;
        float tElevationMin = minElevation;
        float tElevationMax = maxElevation;
        float tTemperatureMin = minTemperature;
        float tTemperatureMax = maxTemperature;

        if(parentBiome != null)
        {
            tBiomeMin = getSubOffset(parentBiome.minBiomeVal, parentBiome.maxBiomeVal, minBiomeVal);
            tBiomeMax = getSubOffset(parentBiome.minBiomeVal, parentBiome.maxBiomeVal, maxBiomeVal);

            tElevationMin = getSubOffset(parentBiome.minElevation, parentBiome.maxElevation, minElevation);
            tElevationMax = getSubOffset(parentBiome.minElevation, parentBiome.maxElevation, maxElevation);

            tTemperatureMin = getSubOffset(parentBiome.minTemperature, parentBiome.maxTemperature, minTemperature);
            tTemperatureMax = getSubOffset(parentBiome.minTemperature, parentBiome.maxTemperature, maxTemperature);
        }

        if(biome_ < tBiomeMin || biome_ > tBiomeMax ||
            elevation_ < tElevationMin || elevation_ > tElevationMax ||
                temperature_ < tTemperatureMin || temperature_ > tTemperatureMax)
                    return null;

        float biomeBias = (tBiomeMax - tBiomeMin) + (biome_ * 2);
        float elevationBias = (tElevationMax - tElevationMin) + (elevation_ * 2);
        float temperatureBias = (tTemperatureMax - tTemperatureMax) + (temperature_ * 2);
        float bias = biomeBias + elevationBias + temperatureBias;

        if(bias <= bias_)
        {
            Biome sub = null;
            foreach(Biome b in subBiomes)
            {
                Biome temp = b.GetSubBiome(biome_, elevation_, temperature_, ref bias, b);
                sub = temp == null ? sub : temp;
            }
            if(sub != null) return sub;
        }
        else
        {
            return null;
        }
        return this;
    }
}
