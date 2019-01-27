using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController2D : PlayerController
{
    public float MoveSpeed;

    public bool UsePhysics;

    public GameObject BackpackItem;

    public List<GameObject> HouseInventory;

    public GameObject Exit;

    /////

    public GameObject ItemPlant;

    public GameObject ItemPainting;

    public GameObject ItemFireplace;

    public GameObject ItemChair;

    public GameObject ItemDrawing;

    public float MinutesToNoStress;

    public GameObject StressVisuals;

    /////
    
    private Rigidbody2D _rigidbody2D;

    private Collider2D _playerCollider;

    private Animator playerAnimator;

    private SpriteRenderer _spriteRenderer;

    private Dictionary<ItemType, GameObject> ItemMapping;

    private List<GameObject> activatedItems;

    private bool doFixedUpdate;

    protected override void Awake()
    {
        ////TESTING
        //GameState.Reset();
        //StressLevel = 1.0f;

        activatedItems = new List<GameObject>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        base.Awake();
    }

    protected override void Start()
    {
        ItemMapping = new Dictionary<ItemType, GameObject>
        {
            { ItemType.CHAIR, ItemChair },
            { ItemType.DRAWING, ItemDrawing },
            { ItemType.PAINTING, ItemPainting },
            { ItemType.PLANT, ItemPlant },
            { ItemType.STOVE, ItemFireplace }
        };

        if (IsHoldingItem)
        {
            switch (HeldItemType)
            {
                case ItemType.CHAIR:
                    BackpackItem = ItemChair;
                    break;

                case ItemType.DRAWING:
                    BackpackItem = ItemDrawing;
                    break;

                case ItemType.PAINTING:
                    BackpackItem = ItemPainting;
                    break;

                case ItemType.PLANT:
                    BackpackItem = ItemPlant;
                    break;

                case ItemType.STOVE:
                    BackpackItem = ItemFireplace;
                    break;
            }

            IsHoldingItem = false;
            playerAnimator.SetBool("isCarryingItem", true);
        }

        //// TODO: TEMP
        //// Add drawing as placed item
        //var itemDrawingState = GameState.GetStateForItem(ItemType.DRAWING);
        //itemDrawingState.itemLocation = GameState.ItemLocation.INDOORS;
        //itemDrawingState.indoorPosition = ItemDrawing.transform.position;
        //GameState.SetStateForItem(ItemType.DRAWING, itemDrawingState);

        // Load placed items
        foreach (var item in ItemMapping)
        {
            var itemState = GameState.GetStateForItem(item.Key);
            if (itemState.itemLocation == GameState.ItemLocation.INDOORS && itemState.indoorPosition != Vector2.zero)
            {
                item.Value.GetComponent<InteriorItem2D>().ShowAt(itemState.indoorPosition);
                //item.Value.transform.position = itemState.indoorPosition;
                HouseInventory.Add(item.Value);
            }
        }

        base.Start();
    }

    protected override void ExecuteUpdate(bool hasFocus, bool pauseMenuOpen)
    {
        if (pauseMenuOpen || !hasFocus)
        {
            doFixedUpdate = false;
            return;
        }

        doFixedUpdate = true;

        var isOverExit = Exit.GetComponent<Collider2D>().bounds.Intersects(_playerCollider.bounds);
        if (isOverExit)
        {
            if (BackpackItem == null && Input.GetKeyDown(keybinds.keyActivate))
            {
                SceneManager.LoadScene("OutDoorWorld");
            }
            return;
        }

        var initialHouseInventory = HouseInventory.ToList();

        if (BackpackItem != null)
        {
            if (Input.GetKeyDown(keybinds.keyInteract))
            {
                var itemData = BackpackItem.GetComponent<InteriorItem2D>();

                var dropPosition = transform.position + new Vector3(itemData.PlacementOffsetX, itemData.PlacementOffsetY, 0);
                itemData.ShowAt(dropPosition);

                var itemType = ItemMapping.Single(m => m.Value == BackpackItem).Key;
                var newState = GameState.GetStateForItem(itemType);
                newState.indoorPosition = dropPosition;
                GameState.SetStateForItem(itemType, newState);

                if (activatedItems.Any(i => i == BackpackItem))
                {
                    itemData.EnableGlow();
                }

                HouseInventory.Add(BackpackItem);
                BackpackItem = null;
                playerAnimator.SetBool("isCarryingItem", false);
            }
        }

        foreach (var item in initialHouseInventory)
        {
            var playerCollision = item.GetComponent<Collider2D>().bounds.Intersects(_playerCollider.bounds);
            var itemData = item.GetComponent<InteriorItem2D>();

            if (playerCollision)
            {
                if (Input.GetKeyDown(keybinds.keyActivate) && activatedItems.Contains(item))
                {
                    itemData.SetVolume(0.0f);
                    itemData.DisableGlow();
                    activatedItems.Remove(item);
                }
                else
                {
                    if (BackpackItem == null)
                    {
                        if (Input.GetKeyDown(keybinds.keyInteract))
                        {
                            itemData.SetVolume(0.0f);
                            itemData.DisableGlow();
                            activatedItems.Remove(item);

                            HouseInventory.Remove(item);
                            BackpackItem = item;
                            //item.transform.position = new Vector3(-20, -20, 0);
                            //item.GetComponent<Renderer>().enabled = false;
                            itemData.Hide();
                            itemData.DisableGlow();
                            playerAnimator.SetBool("isCarryingItem", true);
                        }
                        else if (Input.GetKeyDown(keybinds.keyActivate) && !activatedItems.Contains(item))
                        {
                            // Activate calm for the item
                            // Music + reduced stress
                            activatedItems.Add(item);
                            MinutesToNoStress = MinutesToNoStress / 1.3f;
                            itemData.SetVolume(1.0f);
                            itemData.EnableGlow();
                        }
                    }
                }

                //itemData.EnableGlow();
            }
            else
            {
                //itemData.DisableGlow();
            }
        }
    }

    //not needed here but it's still in because i can't be arsed to change another script to actually cast to firstpersonplayercontroller...
    public override void SaveBeforeLevelChange () {
        throw new System.NotImplementedException();
    }

    protected override void ManageStressLevel ()
    {
        var secondsToNoStress = MinutesToNoStress * 60;
        var stressPerSecond = 1f / secondsToNoStress;
        StressLevel -= stressPerSecond * Time.deltaTime;
        //StressVisuals.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, StressLevel);

        StressVisuals.GetComponent<StressVisuals>().StressLevel = StressLevel;
    }

    void FixedUpdate()
    {
        if (!doFixedUpdate)
        {
            return;
        }

        if (UsePhysics)
        {
            HandleMovementWithPhysics();
        }
        else
        {
            HandleMovementWithoutPhysics();
        }
    }

    void HandleAnimationChanges(float moveDirection)
    {
        if (moveDirection > 0)
        {
            _spriteRenderer.flipX = false;
            playerAnimator.SetBool("isWalking", true);
        }
        else if (moveDirection < 0)
        {
            _spriteRenderer.flipX = true;
            playerAnimator.SetBool("isWalking", true);
        }
        else
        {
            playerAnimator.SetBool("isWalking", false);
        }

    }

    int GetMovementInput()
    {
        return Input.GetKey(keybinds.keyLeft) ? -1 : Input.GetKey(keybinds.keyRight) ? 1 : 0;
    }

    void HandleMovementWithPhysics()
    {
        //var horizontalInput = Input.GetAxis("Horizontal");
        //var moveDirection = horizontalInput > 0 ? 1 : horizontalInput < 0 ? -1 : 0;

        var moveDirection = GetMovementInput();
        var movement = new Vector2(moveDirection * MoveSpeed * 60 * Time.deltaTime, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = movement;

        HandleAnimationChanges(moveDirection);
    }

    void HandleMovementWithoutPhysics()
    {
        //var horizontalInput = GetInputHorizontal();
        //var moveDirection = horizontalInput > 0 ? 1 : horizontalInput < 0 ? -1 : 0;

        var moveDirection = GetMovementInput();
        var movement = moveDirection * MoveSpeed * Time.deltaTime;
        _rigidbody2D.MovePosition(new Vector2(_rigidbody2D.position.x + movement, _rigidbody2D.position.y));

        HandleAnimationChanges(moveDirection);
    }
}