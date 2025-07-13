using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public ItemDropPool dropPool;

    [Range(0f, 100f)] public float commonChance = 50f;
    [Range(0f, 100f)] public float rareChance = 10f;
    [Range(0f, 100f)] public float uniqueChance = 5f;

    public void TryDropItem()
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= uniqueChance)
        {
            DropRandomItem(ItemRarity.Unique);
        }
        else if (roll <= uniqueChance + rareChance)
        {
            DropRandomItem(ItemRarity.Rare);
        }
        else if (roll <= uniqueChance + rareChance + commonChance)
        {
            DropRandomItem(ItemRarity.Common);
        }
        else
        {
            Debug.Log("No item dropped.");
        }
    }

    private void DropRandomItem(ItemRarity rarity)
    {
        var list = dropPool.GetItemsByRarity(rarity);
        if (list.Count == 0)
        {
            Debug.LogWarning($"No items available in the pool for rarity: {rarity}");
            return;
        }

        var selectedItem = list[Random.Range(0, list.Count)];
        Instantiate(selectedItem.prefab, transform.position, Quaternion.identity);
        Debug.Log($"Dropped: {selectedItem.itemName} ({rarity})");
    }
}
