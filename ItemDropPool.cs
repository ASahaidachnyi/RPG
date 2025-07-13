using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDropPool", menuName = "Items/Drop Pool")]
public class ItemDropPool : ScriptableObject
{
    public List<ItemData> commonItems;
    public List<ItemData> rareItems;
    public List<ItemData> uniqueItems;

    public List<ItemData> GetItemsByRarity(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => commonItems,
            ItemRarity.Rare => rareItems,
            ItemRarity.Unique => uniqueItems,
            _ => new List<ItemData>()
        };
    }
}
