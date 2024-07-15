using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBubble : MonoBehaviour
{
    private SpriteRenderer _background;
    private TextMeshPro _text;

    private void Awake() {
        _background = transform.Find("Background").GetComponent<SpriteRenderer>();
        _text = transform.Find("Text").GetComponent<TextMeshPro>();
    }
    
    private void Setup(string text) {
        _text.SetText(text);
    }

    private void Start() {
        Setup("yhep, this works");
    }
}
