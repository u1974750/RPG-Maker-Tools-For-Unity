using TMPro;
using UnityEngine;

public class NPCController : MonoBehaviour {
    public string dialogText;
    public GameObject item;
    private GameObject dialogBubble;
    private TextMeshPro dialogTextComponent;
    private SpriteRenderer dialogBubbleSprite;
    private bool hasItem = true;


    // Start is called before the first frame update
    void Awake() {
        dialogBubble = transform.Find("DialogBubble").gameObject;
        if (dialogBubble != null) {
            GameObject child = dialogBubble.transform.Find("Background").gameObject;
            dialogBubbleSprite = child.GetComponent<SpriteRenderer>();

            dialogTextComponent = child.transform.Find("Text").gameObject.GetComponent<TextMeshPro>();

        }
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerMovement>().NPCRangeInteract(true, this);
            if (dialogText != "") {
                dialogBubble.SetActive(true);
                dialogTextComponent.text = dialogText;

                dialogBubbleSprite.size = new Vector2(
                                              dialogTextComponent.gameObject.GetComponent<RectTransform>().sizeDelta.x + 0.5f,
                                              dialogTextComponent.gameObject.GetComponent<RectTransform>().sizeDelta.y + 0.5f);

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            dialogBubble.SetActive(false);
            collision.GetComponent<PlayerMovement>().NPCRangeInteract(false, this);
        }
    }

    public GameObject GiveItem() {
        hasItem = false;
        return item;
    }

    public bool HasItem() {
        return hasItem;
    }

}
