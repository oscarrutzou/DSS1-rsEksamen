using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.Weapons;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.ObserverPattern;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.PlayerClasses;

// Stefan
public abstract class Player : Character
{
    public GameObject HandsGo;
    public GameObject MovementColliderGo;

    public Vector2 totalMovementInput, previousPosition;
    public Vector2 velocity { get; set; }

    protected List<IObserver> observers = new();

    public Potion ItemInInventory { get; set; }

    private Collider movementCollider;

    public WeaponTypes WeaponType; 
    public ClassTypes ClassType;

    private double _onDeadTimer;
    private readonly double _timeTillSceneChange = 3f;
    private static SoundNames[] _playerHit = new SoundNames[]
    {
        SoundNames.PlayerHit1,
        SoundNames.PlayerHit2,
        SoundNames.PlayerHit3,
        SoundNames.PlayerHit4,
        SoundNames.PlayerHit5,
        SoundNames.PlayerHit6,
        SoundNames.PlayerHit7,
        SoundNames.PlayerHit8,
    };  

    public Player(GameObject gameObject) : base(gameObject)
    {
        Speed = 150;
    }

    public Player(GameObject gameObject, GameObject handsGo, GameObject movementColliderGo) : base(gameObject)
    {
        Speed = 150;
        this.HandsGo = handsGo;
        this.MovementColliderGo = movementColliderGo;
    }

    public override void Awake()
    {
        base.Awake();

        CharacterHitHurt = _playerHit;

        movementCollider = MovementColliderGo.GetComponent<Collider>();
        Collider.SetCollisionBox(15, 24, new Vector2(0, 30));

        Cell cellUnderPlayer = SetStartCollisionNr();
        if (cellUnderPlayer == null) return;
        GridManager.Instance.AddVisitedRoomNumbers(cellUnderPlayer.RoomNr);
    }

    public override void Start()
    {
        SpriteRenderer.SetLayerDepth(LayerDepth.Player);
        SetState(CharacterState.Idle);
    }

    public override void Update()
    {
        base.Update();

        // Checks dash and dashes the direction if it can
        if (State != CharacterState.Dead && !CheckDash())
        {
            CheckForMovement();
        }

        Weapon?.MoveWeaponAndAngle();

        switch (State)
        {
            case CharacterState.Idle:
                ResetRotationWhenIdle();
                break;

            case CharacterState.Moving:
                Move(totalMovementInput);
                break;

            case CharacterState.Attacking:
                ResetRotationWhenIdle();
                break;

            case CharacterState.Dead:
                ResetRotationWhenIdle();
                ChangeScene();
                break;
        }

        if (totalMovementInput != Vector2.Zero) _previousTotalMovementInput = totalMovementInput;

        totalMovementInput = Vector2.Zero;
    }

    private void ChangeScene()
    {
        _onDeadTimer += GameWorld.DeltaTime;
        if (_onDeadTimer >= _timeTillSceneChange)
        {
            _onDeadTimer = 0;
            SaveData.HasWon = false;
            DB.Instance.OnChangeSceneEnd();
        }
    }

    private void CheckForMovement()
    {
        if (totalMovementInput != Vector2.Zero)
            SetState(CharacterState.Moving);
        else
            SetState(CharacterState.Idle);
    }

    #region Movement

    public void AddInput(Vector2 input) // 0, 1 / 0, -1 / 1,0 / -1, 0
    {
        // Ensure the input vector is not a zero vector, will cause Nan/Nan in the vector
        if (input != Vector2.Zero)
        {
            // Add the normalized input to the total movement
            totalMovementInput += input;
            
            // If the input is the opposite then the input can turn into Vector.Zero
            if (totalMovementInput != Vector2.Zero)
                totalMovementInput.Normalize();
        }
    }

    public void Move(Vector2 input)
    {
        InitializeMovement();

        if (input != Vector2.Zero)
        {
            ProcessInput(input);
        }

        UpdateDirection();

        if (GridManager.Instance.CurrentGrid == null) return; // Player can't walk if there is no grid.

        TryMoveInBothDirections();

        UpdatePositionAndNotify();


    }

    public bool CanDash = true;
    public float DashMaxMovePx = 200;
    public bool Dash = false;

    private double _dashTimer, _dashCooldown = 1f;

    private Vector2 _previousTotalMovementInput;

    // Now just need to make player locked into the dash
    // it will give health inmum
    /* Also make the player lerp from the start to end pos
     * Make the lerp based on how long the distance moved is compared to the maxDistance
     */

    private bool CheckDash()
    {
        if (!CanDash) return false;

        if (!Dash)
        {
            _dashTimer += GameWorld.DeltaTime;
            return false;
        }

        Vector2 input;
        if (totalMovementInput == Vector2.Zero)
        {
            // Check previous movement input.
            if (_previousTotalMovementInput != Vector2.Zero)
            {
                input = _previousTotalMovementInput;
            }
            else
            {
                input = new Vector2(0, 1); // Sets the default direction to right, if none of the buttons have been pressed
            }
        }
        else
        {
            // Use total movement input
            input = totalMovementInput;
        }

        // If dashed true
        if (CheckDashDirection(input))
        {
            // Resets dash
            Dash = false;
            _dashTimer = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateDash()
    {
        Dash = _dashTimer >= _dashCooldown;
    }

    private bool CheckDashDirection(Vector2 inputDirection)
    {
        // Separate the movement into X and Y components
        Vector2 xMovement = new Vector2(inputDirection.X, 0);
        Vector2 yMovement = new Vector2(0, inputDirection.Y);

        bool hasMoved = false;

        //Try moving along the X axis
        if (xMovement.X != 0f)
        {
            // Update the previous position after a successful move
            if (TryDashMove(xMovement))
            {
                hasMoved = true;
            }
        }

        // Try moving along the Y axis
        if (yMovement.Y != 0f)
        {
            // Update the previous position after a successful move
            if (TryDashMove(yMovement))
            {
                hasMoved = true;
            }
        }

        RotateCharacterOnMove(hasMoved);

        return hasMoved;
    }
    private bool TryDashMove(Vector2 inputDirection)
    {
        // Get the CollisionBox
        Rectangle collisionBox = movementCollider.CollisionBox;

        // Check each corner of the CollisionBox
        Vector2[] corners = new Vector2[]
        {
            new(collisionBox.Left, collisionBox.Top),
            new(collisionBox.Right, collisionBox.Top),
            new(collisionBox.Right, collisionBox.Bottom),
            new(collisionBox.Left, collisionBox.Bottom)
        };

        float distance = DashMaxMovePx;

        foreach (Vector2 corner in corners)
        { 
            // Find max distance for each corner,
            // the min distance is the final movement
            float maxCornerDashDistance = FindMaxDistance(corner, inputDirection, DashMaxMovePx);
            if (maxCornerDashDistance > 0f)
            {
                // You cant move in that direction
                if (distance > maxCornerDashDistance)
                {
                    distance = maxCornerDashDistance; // Set the max distance it moves to the smallest calse
                }
            }
            else
            {
                return false;
            }
        }

        // If all corners are in valid cells, then move the correct distance
        TranslateMovement(inputDirection * distance);

        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

    private void InitializeMovement()
    {
        velocity = Vector2.Zero;
        previousPosition = GameObject.Transform.Position;
    }

    private void ProcessInput(Vector2 input)
    {
        velocity = BaseMath.SafeNormalize(input);
        Direction = velocity;
    }

    private void TryMoveInBothDirections()
    {
        velocity *= Speed * (float)GameWorld.DeltaTime;

        // Separate the movement into X and Y components
        Vector2 xMovement = new Vector2(velocity.X, 0);
        Vector2 yMovement = new Vector2(0, velocity.Y);

        bool hasMoved = false;
        //Try moving along the X axis
        if (xMovement.X != 0f && TryMove(xMovement))
        {
            // Update the previous position after a successful move
            previousPosition = GameObject.Transform.Position;
            hasMoved = true;
        }

        // Try moving along the Y axis
        if (yMovement.Y != 0f && TryMove(yMovement))
        {
            // Update the previous position after a successful move
            previousPosition = GameObject.Transform.Position;
            hasMoved = true;
        }

        RotateCharacterOnMove(hasMoved);
    }

    private void UpdatePositionAndNotify()
    {
        SetMovement(GameObject.Transform.Position); // So we set the other gameobjects (Hands, Movement Collider...)

        // Updates the grid position
        GameObject cellGoUnderPlayer = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);
        if (cellGoUnderPlayer == null) return;
        GameObject.Transform.GridPosition = cellGoUnderPlayer.Transform.GridPosition;
        Cell cellUnderPlayer = cellGoUnderPlayer.GetComponent<Cell>();
        CollisionNr = cellUnderPlayer.CollisionNr;

        GridManager.Instance.AddVisitedRoomNumbers(cellUnderPlayer.RoomNr);
    }

    /// <summary>
    /// A raycast approach
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    private float CheckDistanceToObstacle(Vector2 direction, float maxDistance)
    {
        Vector2 currentPosition = GameObject.Transform.Position;
        Vector2 step = direction * Speed * (float)GameWorld.DeltaTime; 
        float distance = 0f;

        // First check max distance, then half and make 

        while (distance < maxDistance)
        {
            currentPosition += step;
            distance += step.Length();

            // Check for collision at the new position
            if (!IsPositionWalkable(currentPosition))
            {
                return distance;
            }
        }

        return maxDistance;
    }




    /// <summary>
    /// Binary search to find the max distance. Only on player dash.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="maxDistance"></param>
    /// <param name="minStepDistance"></param>
    /// <returns></returns>
    private float FindMaxDistance(Vector2 startPos, Vector2 direction, float maxDistance, float minStepDistance = 10)
    {
        float low = 0f;
        float high = maxDistance;
        float mid = 0f;

        // Checks if the max Distance is already the max direction
        Vector2 maxDistancePos = startPos + direction * maxDistance;
        if (IsPositionWalkable(maxDistancePos)) return maxDistance;

        while (high - low > minStepDistance)
        {
            mid = (low + high) / 2;
            Vector2 testPosition = startPos + direction * mid;

            if (IsPositionWalkable(testPosition))
            {
                low = mid; // No collision, try further
            }
            else
            {
                high = mid; // Collision, try closer
            }
        }

        return low;
    }
    private bool IsPositionWalkable(Vector2 position)
    {
        GameObject gridCell = GridManager.Instance.GetCellAtPos(position);
        return gridCell != null && gridCell.GetComponent<Cell>().CellWalkableType != CellWalkableType.NotValid;
    }

    private bool TryMove(Vector2 movement)
    {
        // Translate the GameObject, and movement collider.
        TranslateMovement(movement);

        // Get the CollisionBox
        Rectangle collisionBox = movementCollider.CollisionBox;

        // Check each corner of the CollisionBox
        Vector2[] corners = new Vector2[]
        {
            new(collisionBox.Left, collisionBox.Top),
            new(collisionBox.Right, collisionBox.Top),
            new(collisionBox.Right, collisionBox.Bottom),
            new(collisionBox.Left, collisionBox.Bottom)
        };

        foreach (Vector2 corner in corners)
        {
            if (!IsPositionWalkable(corner))
            {
                // If any corner is in an invalid cell, revert the movement on both player and
                SetMovement(previousPosition);
                return false;
            }
        }

        // If all corners are in valid cells, the movement is successful
        return true;
    }

    /// <summary>
    /// Adds the movement to the gameobject
    /// </summary>
    /// <param name="movement"></param>
    protected void TranslateMovement(Vector2 movement)
    {
        GameObject.Transform.Translate(movement);
        MovementColliderGo.Transform.Position = GameObject.Transform.Position;
        HandsGo.Transform.Position = GameObject.Transform.Position;
        Weapon.MoveWeaponAndAngle();
    }

    /// <summary>
    /// Sets the position of the object to the position.
    /// </summary>
    /// <param name="position"></param>
    protected void SetMovement(Vector2 position)
    {
        GameObject.Transform.Position = position;
        MovementColliderGo.Transform.Position = GameObject.Transform.Position;
        HandsGo.Transform.Position = GameObject.Transform.Position;
        Weapon.MoveWeaponAndAngle();
        GameWorld.Instance.WorldCam.Position = GameObject.Transform.Position; //Sets the new position of the world cam
    }

    #endregion Movement

    #region Item

    public bool CanPickUpItem(Potion item)
    {
        if (ItemInInventory == null)
        {
            ItemInInventory = item;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UseItem()
    {
        if (ItemInInventory == null) return;

        ItemInInventory.Use();
    }

    #endregion Item
}