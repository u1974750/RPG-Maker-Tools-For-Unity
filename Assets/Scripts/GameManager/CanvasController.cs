using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] Sprite _heartSprite;
    private int _life;
    private int _maxLife = 4;
    private List<GameObject> _hearts;

    private void Awake() {
        _life = _maxLife;
        _hearts = new List<GameObject>();
        GameObject aux = gameObject.transform.GetChild(0).gameObject;
        for (int i = 0; i < aux.transform.childCount; i++) {
            if (aux.transform.GetChild(i).name.Contains("Health")) {
                _hearts.Add(aux.transform.GetChild(i).gameObject);
            }
        }
    }

    public void RemoveLife(int num) {
        _maxLife += num;
        for (int i = num; i < 0; i++) {
            if (_hearts[_life - 1] != null) {
                _hearts[_life - 1].GetComponent<Image>().color = Color.gray;
                _life--;
            }
        }
    }

    public void ModifyHealthUI(int num) {
        if (num < 0) {
            RemoveLife(num);
        }
        else {
            NewHearts(num);
        }
    }

    public void NewHearts(int num) {

    }   

}
