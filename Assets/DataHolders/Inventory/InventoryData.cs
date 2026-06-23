using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InventoryData", order = 2)]
public class InventoryData : MonoBehaviour
{
    public List<ItemData> itemsInInventory;
}
