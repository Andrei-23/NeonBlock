using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityMaterials : MonoBehaviour
{
    public static RarityMaterials Instance;

    public Material commonMaterial;
    public Material rareMaterial;
    public Material epicMaterial;
    public Material legendaryMaterial;
    public Material badMaterial;
    public Material defautMaterial;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Material GetRarityFrameMaterial(PieceData.Rarity rarity)
    {
        switch (rarity)
        {
            case PieceData.Rarity.Common: return commonMaterial;
            case PieceData.Rarity.Rare: return rareMaterial;
            case PieceData.Rarity.Epic: return epicMaterial;
            case PieceData.Rarity.Legendary: return legendaryMaterial;
            case PieceData.Rarity.Bad: return badMaterial;
            default: return defautMaterial;
        }
    }
}
