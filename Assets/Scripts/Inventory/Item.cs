using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public uint identifier;
    public string name;
    public float weight;
    public Sprite icon;
    public ItemType type;

    public enum ItemType
    {
        Upgrade,
        Potion,
        Weapon
    }
}
