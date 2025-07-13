using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemRarity rarity;
    public GameObject prefab; // Якщо треба спаунити об’єкт
}
