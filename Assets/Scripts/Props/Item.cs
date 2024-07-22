using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public struct ItemValues {
        public int healthValue;
        public int armourValue;
        public int strenghtValue;
        public float speedValue;
    }

    public Sprite itemSprite;
    public string itemName;
    public float itemTime;
    public ItemValues itemValues;


    public void SetItemValues(int health, int armour, int strenght, int speed) {
        itemValues = new ItemValues();

        itemValues.healthValue = health;
        itemValues.armourValue = armour;
        itemValues.strenghtValue = strenght;
        itemValues.speedValue = speed;
    }
    public void SetItemSprite(Sprite spr) { itemSprite = spr; }
    public void SetItemName(string name) { itemName = name; }
    public void SetItemTime(float time) {  itemTime = time; }


    public ItemValues GetItemValues() { return  itemValues; }
    public Sprite GetItemSprite() { return  itemSprite; }
    public string GetItemName() { return itemName; }
    public float GetItemTime() { return itemTime; }


}
