using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {
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
                if(i > 3) {
                    aux.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void RemoveLife(int num) {
        _maxLife += num;
        _life += num;

        int done = num;
        for(int i = _maxLife +1; i  >= 0; i--) {
            if (_hearts[i].activeSelf) {
                if (done < 0) {
                    _hearts[i].SetActive(false);
                    done++;

                    GameObject aux = gameObject.transform.GetChild(0).gameObject;
                    aux.GetComponent<RectTransform>().sizeDelta = new Vector2(_hearts[i].GetComponent<RectTransform>().anchoredPosition.x + 10, 64.5f);
                }
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

    public void takeDamageUI(int num) {
        _life -= num;
        
        for (int i = _hearts.Count-1; i > _life-1; i--) {
            if(i >= 0) {
                if (_hearts[i].activeSelf) {
                    _hearts[i].GetComponent<Image>().color = Color.gray;
                }
            }

        }
    }

    public void NewHearts(int num) {
        _maxLife += num;
        _life += num;
        for (int i = 0; i < _hearts.Count; i++) {
            if(i+1 <= _maxLife) {
                _hearts[i].SetActive(true);
                GameObject aux = gameObject.transform.GetChild(0).gameObject;
                aux.GetComponent<RectTransform>().sizeDelta = new Vector2(_hearts[i].GetComponent<RectTransform>().anchoredPosition.x + 60, 64.5f);
            }
        }
    }

}
