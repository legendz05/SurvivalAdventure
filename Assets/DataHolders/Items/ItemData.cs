using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int maxStack;
    public string itemID;

    public Sprite itemIcon;
    public Mesh itemMesh;
    public Material itemMaterial;

    public void OnValidate()
    {
        itemID = "ID_" + itemName;
    }
}
