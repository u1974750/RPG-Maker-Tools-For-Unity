using UnityEngine;

public class Item : MonoBehaviour {
    public struct ItemValues {
        public int healthValue;
        public int armourValue;
        public int strenghtValue;
        public float speedValue;
    }

    [Header("UI Properties")]
    public Sprite itemSprite;
    public string itemName;
    public float itemTime;

    [Space(10)]
    [Header("Item Properties")]
    public int speedValue;
    public int healthValue;
    public int armourValue;
    public int strenghtValue;

    public void SetItemValues(int health, int armour, int strenght, int speed) {

        healthValue = health;
        armourValue = armour;
        strenghtValue = strenght;
        speedValue = speed;
    }
    public void SetItemSprite(Sprite spr) { itemSprite = spr; }
    public void SetItemName(string name) { itemName = name; }
    public void SetItemTime(float time) { itemTime = time; }


    public Sprite GetItemSprite() { return itemSprite; }
    public string GetItemName() { return itemName; }
    public float GetItemTime() { return itemTime; }


    public int GetSpeed() { return speedValue; }
    public int GetHealth() { return healthValue; }
    public int GetStrenght() { return strenghtValue; }
    public int GetArmour() { return armourValue; }


}
