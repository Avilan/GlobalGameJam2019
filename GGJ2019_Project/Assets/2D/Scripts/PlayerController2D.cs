using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController2D : PlayerController
{
    public float MoveSpeed;

    public bool UsePhysics;

    public GameObject BackpackItem;

    public List<GameObject> HouseInventory;

    /////

    private Rigidbody2D _rigidbody2D;

    private Collider2D _playerCollider;

    private Animator playerAnimator;

    private SpriteRenderer _spriteRenderer;

    private bool doFixedUpdate;

    protected override void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        base.Awake();
    }

    protected override void Start()
    {
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

        if (BackpackItem != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                var itemData = BackpackItem.GetComponent<InteriorItem2D>();

                HouseInventory.Add(BackpackItem);
                BackpackItem.transform.position = transform.position;
                BackpackItem.transform.position = transform.position + new Vector3(itemData.PlacementOffsetX, itemData.PlacementOffsetY, 0);
                BackpackItem = null;
                playerAnimator.SetBool("isCarryingItem", false);
            }
        }

        foreach (var item in HouseInventory.ToList())
        {
            var playerCollision = item.GetComponent<Collider2D>().bounds.Intersects(_playerCollider.bounds);
            var itemData = item.GetComponent<InteriorItem2D>();

            if (playerCollision)
            {
                itemData.EnableGlow();

                if (BackpackItem == null)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        HouseInventory.Remove(item);
                        BackpackItem = item;
                        item.transform.position = new Vector3(-20, -20, 0);
                        itemData.DisableGlow();
                        playerAnimator.SetBool("isCarryingItem", true);
                    }
                    else if (Input.GetKeyDown(keybinds.keyInteract))
                    {
                        // Activate calm for the item
                        // Music + reduced stress
                    }
                }
            }
            else
            {
                itemData.DisableGlow();
            }
        }
    }

    //TODO saving before level change. and loading on level entry (override LoadSettings but keep the base!!!)
    protected override void SaveBeforeLevelChange () {
        throw new System.NotImplementedException();
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