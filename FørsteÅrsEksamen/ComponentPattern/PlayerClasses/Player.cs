using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.ComponentPattern.Weapons;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.Factory;
using ShamansDungeon.GameManagement;
using ShamansDungeon.LiteDB;
using ShamansDungeon.ObserverPattern;
using ShamansDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using ShamansDungeon.ComponentPattern.WorldObjects.PickUps;

namespace ShamansDungeon.ComponentPattern.PlayerClasses;

// Stefan
public abstract class Player : Character
{
    #region Properties
    public GameObject HandsGo;
    public GameObject MovementColliderGo;
    public Texture2D StatsScreen { get; protected set; }
    public Vector2 totalMovementInput, previousPosition;
    public Vector2 velocity { get; set; }

    protected List<IObserver> observers = new();

    public Potion ItemInInventory { get; set; }
    public bool HasUsedItem;

    public Collider movementCollider { get; set; } 

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
    private bool _hasPlayedDeathSound;

    #region Dash
    public bool CanDash = true;
    public float DashMaxMovePx = 300;
    public bool DashInput = false;
    public bool IsDashing { get; protected set; }
    public double DashCooldownTimer { get; protected set; }
    public double DashCooldown { get; protected set; } = 2f; 
    private double _dashTimeToCompleteTimer, _dashFinalLerpCooldown, _dashTimeToComplete = 0.1f; // The dash time it takes to move to the new position

    private Vector2 _previousTotalMovementInput;
    private Vector2 _dashMovementInput; // A normalized value

    private Vector2 _finalDashDirection; // So we know which direction and how much we can dash each direction 
    private Vector2 _dashStartPosition, _finalDashPosition;
    #endregion
    #endregion

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
        //NormalSpriteOffset = new(0, -70);

        base.Awake();

        //Collider.SetCollisionBox(15, 23, new Vector2(0, 40));

        //Weapon.StartPosOffset = new(40, 40);
        //Weapon.StartRelativeOffsetPos = new(0, -40);
        //Weapon.StartRelativePos = new(0, 80);

        DashCooldownTimer = DashCooldown;

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

        HasUsedItem = false;
        // Checks dash and dashes the direction if it can
        if (State != CharacterState.Dead)
        {
            CheckForMovement();
            PlayHeartBeatIfLow();
        }

        Weapon?.MoveWeaponAndAngle();

        switch (State)
        {
            case CharacterState.Idle:
                ResetRotationWhenIdle();
                break;

            case CharacterState.Moving:
                if (IsDashing) break;
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
        DashInput = false; // Resets the dash
    }
    private void PlayHeartBeatIfLow()
    {
        if (Health.NormalizedHealth > 0.25f) return; 
     
        GlobalSounds.PlaySound(SoundNames.LowHealthHeartbeat, 1, 0.5f, true, -0.4f, 0.2f);
    }
    private void ChangeScene()
    {
        _onDeadTimer += GameWorld.DeltaTime;
        if (!_hasPlayedDeathSound)
        {
            _hasPlayedDeathSound = true;
            GlobalSounds.PlaySound(SoundNames.ArcadeGameOver, 1, 1f);
        }

        if (_onDeadTimer >= _timeTillSceneChange)
        {
            _onDeadTimer = 0;
            SaveData.HasWon = false;
            DB.Instance.OnChangeSceneEnd();
        }
    }

    private void CheckForMovement()
    {
        if (CheckDash() || IsDashing) // If has begun dash or in the middle of dash.
        {
            SetState(CharacterState.Moving); // Maybe play the move 
            if (IsDashing)
            {
                UpdateIsDashingPosition();
            }
            return;
        }

        if (totalMovementInput != Vector2.Zero)
            SetState(CharacterState.Moving);
        else
            SetState(CharacterState.Idle);
    }

    protected override float GetWeaponAngle()
    {
        Vector2 mouseInUI = InputHandler.Instance.MouseOnUI;
        return (float)Math.Atan2(mouseInUI.Y, mouseInUI.X);
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
        AddRoom();
    }

    private void AddRoom()
    {
        // Updates the grid position
        GameObject cellGoUnderPlayer = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);
        if (cellGoUnderPlayer == null) return;
        GameObject.Transform.GridPosition = cellGoUnderPlayer.Transform.GridPosition;
        Cell cellUnderPlayer = cellGoUnderPlayer.GetComponent<Cell>();
        CollisionNr = cellUnderPlayer.CollisionNr;

        GridManager.Instance.AddVisitedRoomNumbers(cellUnderPlayer.RoomNr);
    }

    private bool IsPositionWalkable(Vector2 position)
    {
        GameObject gridCell = GridManager.Instance.GetCellAtPos(position);

        if (gridCell != null)
        {
            Cell cell = gridCell.GetComponent<Cell>();
            if (cell.CellWalkableType != CellWalkableType.NotValid && cell.CollisionNr != -1)
            {
                return true;
            }
        }
     
        return false;
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
    
    #region Dash
    // Now just need to make player locked into the dash
    // it will give health inmum
    /* Also make the player lerp from the start to end pos
     * Make the lerp based on how long the distance moved is compared to the maxDistance
     */

    private bool CheckDash()
    {
        if (!CanDash || IsDashing) return false;

        if (!DashInput)
        {
            if (DashCooldownTimer <= DashCooldown)
            {
                DashCooldownTimer += GameWorld.DeltaTime;
            }
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
            _dashTimeToCompleteTimer = 0;
            IsDashing = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateIsDashingPosition()
    {
        //TranslateMovement(inputDirection * distance);

        _dashTimeToCompleteTimer += GameWorld.DeltaTime; // How much

        double normalizedTimeToComplete = _dashTimeToCompleteTimer / _dashFinalLerpCooldown;

        if (normalizedTimeToComplete >= 1f) // We need to stop the lerp a little before, so it dosent get stuck inside stuff
        {
            ResetDash();
            return;
        }

        Health.CanTakeDamage = false;
        // Lerp the current direction with the end position
        Vector2 lerpDashPosition = Vector2.Lerp(_dashStartPosition, _finalDashPosition, (float)normalizedTimeToComplete);
        SetMovement(lerpDashPosition);
        AddRoom();
    }


    private void ResetDash()
    {
        // Resets dash
        DashInput = false;
        DashCooldownTimer = 0;
        _dashTimeToCompleteTimer = 0;
        IsDashing = false;
        Health.CanTakeDamage = true;
    }

    

    public void UpdateDash()
    {
        DashInput = DashCooldownTimer >= DashCooldown || IsDashing;
    }

    private bool CheckDashDirection(Vector2 inputDirection)
    {
        _dashMovementInput = inputDirection;

        bool hasMoved = false;

        float dashDistance = RayCastCheckDistanceToObstacle(movementCollider.CollisionBox, inputDirection, DashMaxMovePx, 1);

        if (dashDistance > 0f) // The collider can move
        {
            hasMoved = true;
            _finalDashDirection = dashDistance * inputDirection;
            OnDashMoved();
        }

        RotateCharacterOnMove(hasMoved);

        return hasMoved;
    }

    private void OnDashMoved()
    {
        _dashStartPosition = GameObject.Transform.Position;
        _finalDashPosition = GameObject.Transform.Position + _finalDashDirection;

        Vector2 positiveDashDirection = new Vector2(Math.Abs(_finalDashDirection.X), Math.Abs(_finalDashDirection.Y));

        float normalizedDifference = positiveDashDirection.Length() / DashMaxMovePx;
        _dashFinalLerpCooldown = Math.Max(_dashTimeToComplete * normalizedDifference, _dashTimeToComplete / 3); // We dont want the timer to be too fast
    }

    /// <summary>
    /// A raycast approach
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    private float RayCastCheckDistanceToObstacle(Vector2 startPos, Vector2 direction, float maxDistance, float rayCastStep = 10)
    {
        Vector2 currentPosition = startPos;
        float maxDistanceDirection = maxDistance * direction.Length();

        float distance = 0f;
        Vector2 step = new Vector2(rayCastStep * direction.X, rayCastStep * direction.Y);

        // Try first for the max distance
        Vector2 maxDistancePos = startPos + direction * maxDistance;
        if (IsPositionWalkable(maxDistancePos)) return maxDistance;

        float furthest = 0;

        // Now make the raycast
        while (distance < maxDistanceDirection)
        {
            currentPosition += step;
            distance += rayCastStep;

            // Check for collision at the new position
            if (!IsPositionWalkable(currentPosition)) continue;

            if (distance > furthest)
            {
                furthest = distance;
            }
        }

        // Found nothing
        return furthest;
    }

    private float RayCastCheckDistanceToObstacle(Rectangle rectangle, Vector2 direction, float maxDistance, float rayCastStep = 10)
    {
        float maxDistanceDirection = maxDistance * direction.Length();

        Vector2[] corners = new Vector2[]
        {
            new(rectangle.Left, rectangle.Top),
            new(rectangle.Right, rectangle.Top),
            new(rectangle.Right, rectangle.Bottom),
            new(rectangle.Left, rectangle.Bottom)
        };

        float distance = 0f;
        float furthest = 0f;
        Vector2 step = new Vector2(rayCastStep * direction.X, rayCastStep * direction.Y);

        // Try first for the max distance

        // Now make the raycast
        while (distance < maxDistanceDirection)
        {
            //currentPosition += step;
            distance += rayCastStep;

            // Adds the step to the corners too.
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] += step;
            }

            bool anyIsNotWalkable = false;

            // Check each corner of the CollisionBox
            foreach (Vector2 corner in corners)
            {
                if (!IsPositionWalkable(corner))
                {
                    anyIsNotWalkable = true;
                    break;
                }
            }

            if (anyIsNotWalkable)
            {
                continue;
            }

            // Have checked all corners, can now check if the distance if bigger than the furthest distance.
            if (distance > furthest)
            {
                furthest = distance;
                
                //step = new Vector2(MathF.Max(step.X * 0.5f, minRayCastStep), MathF.Max(step.Y * 0.5f, minRayCastStep));
            }
        }

        // Found nothing
        return furthest;
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
    #endregion
    
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