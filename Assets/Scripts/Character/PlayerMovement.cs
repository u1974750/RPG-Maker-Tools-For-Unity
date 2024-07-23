using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] int _maxHealth = 4;
    [SerializeField] float _speed = 5.0f;
    [SerializeField] int _attackDamage = 1;
    [SerializeField] int _armour = 0;
    

    private SpriteRenderer _spriteRenderer;
    private NPCController NPCInRange;
    private Animator _animController;
    private Animator _slashAnimator;
    private GameObject _slash;
    private GameObject _canvas;
    private GameObject item = null;
    private bool canAttack = true;
    private bool isInsideNPCRange = false;
    private bool hasItem = false;
    private bool usingItem = false;
    private float itemTime;
    private int currentHealth;
    Item.ItemValues modifiedStats = new Item.ItemValues();
    public Item.ItemValues currentStats;


    void Start()
    {
        _canvas = GameObject.Find("Canvas");
        currentHealth = _maxHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animController = GetComponent<Animator>();
        foreach( Transform child in transform) {
            if(child.gameObject.tag == "SlashAttack") {
                _slash = child.gameObject;

                _slashAnimator = _slash.GetComponent<Animator>();
            }
        }

        currentStats = new Item.ItemValues();
        currentStats.healthValue = _maxHealth;
        currentStats.armourValue = _armour;
        currentStats.speedValue = _speed;
        currentStats.strenghtValue = _attackDamage;

    }

    void Update()
    {
        Move();
        Attack();
        TakeItem();

        if(hasItem && Input.GetKey(KeyCode.Q)) { UseItem(); }
        if(usingItem) {
            itemTime -= Time.deltaTime;
            _canvas.transform.Find("Container").gameObject.transform.Find("item").gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetClockTime(itemTime);
            if(itemTime  < 0) {
                usingItem = false;
                _canvas.transform.Find("Container").transform.Find("item").gameObject.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                Sprite background = Resources.Load<Sprite>("background");
                _canvas.transform.Find("Container").gameObject.transform.Find("item").gameObject.GetComponent<UnityEngine.UI.Image>().sprite = background;

                currentStats.healthValue -= modifiedStats.healthValue;
                currentStats.armourValue -= modifiedStats.armourValue;
                currentStats.speedValue -= modifiedStats.speedValue;
                currentStats.strenghtValue -= modifiedStats.strenghtValue;

            }
        }
    }

    private void Move() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if(horizontalInput != 0f || verticalInput != 0f) {
            _animController.SetBool("Walk", true);

            if(horizontalInput > 0f) {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }

            Vector2 direction = new Vector2(horizontalInput, verticalInput);
            transform.Translate(direction * currentStats.speedValue * Time.deltaTime);

        }
        else {
            _animController.SetBool("Walk", false);
        }

    }

    private void Attack() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (canAttack) {
                _slash.SetActive(true);
                _slashAnimator.SetTrigger("Attack");
                canAttack = false;
            }
        }
    }

    private void TakeItem() {
        if (isInsideNPCRange) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (NPCInRange.HasItem()) {
                    item = NPCInRange.GiveItem();

                    if (item != null && item.GetComponent<Item>() != null) {
                        GameObject aux = _canvas.transform.Find("Container").gameObject;
                        aux = aux.transform.Find("item").gameObject;

                        aux.GetComponent<UnityEngine.UI.Image>().sprite = item.GetComponent<Item>().GetItemSprite();
                        
                        aux.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = " Q ";
                        hasItem = true;
                        itemTime = item.GetComponent<Item>().GetItemTime();
                    }

                }
            }
        }
    }

    private void UseItem() {
        usingItem = true;
        hasItem = false;
        modifiedStats = item.GetComponent<Item>().itemValues;
        
        if(modifiedStats.healthValue != 0) {
            Debug.Log("Enter");
            currentStats.healthValue += modifiedStats.healthValue;

            currentHealth += modifiedStats.healthValue;

            _canvas.GetComponent<CanvasController>().AddLife(modifiedStats.healthValue);


        }
        if (modifiedStats.armourValue != 0) {
            currentStats.armourValue += modifiedStats.armourValue;
        }
        if (currentStats.speedValue != 0) {
            currentStats.speedValue += modifiedStats.speedValue;
        }
        if (currentStats.strenghtValue != 0) {
            currentStats.strenghtValue += modifiedStats.strenghtValue;
        }

    }

    private string GetClockTime(float time) {
        string outMinutes, outSeconds;

        float minutes = Mathf.Floor(time / 60);
        float seconds = Mathf.RoundToInt(time % 60);

        outMinutes = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
        outSeconds = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();

        return outMinutes + ":" + outSeconds;
    }

    private IEnumerator ReturnColorWhite() {
        yield return new WaitForSeconds(0.05f);

        _spriteRenderer.color = Color.white;
    }

    private void UpdateCanvasHealth() {

    }

    private void Die() {
        //TODO: end game
    }

    public void NPCRangeInteract(bool dir, NPCController npc) {
        isInsideNPCRange = dir;
        NPCInRange = npc;
    }
    
    public void TakeDamage(bool isLeft, int damage) {
        if (isLeft) {
            transform.Translate(new Vector2(0.3f, 0));
        }
        else {
            transform.Translate(new Vector2(-0.3f, 0));
        }

        _spriteRenderer.color = Color.red;
        StartCoroutine(ReturnColorWhite());
        currentHealth -= damage - currentStats.armourValue;

        if (currentHealth <= 0) {
            Die();
        }
    }


    public void FinishAttack() {
        _slash.SetActive(false);
        canAttack = true;
    }
}
