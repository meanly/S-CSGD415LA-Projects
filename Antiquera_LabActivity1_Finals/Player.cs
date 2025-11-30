using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

namespace Antiquera_LabActivity1_Finals;

public class Player
{
    private Vector2 position;
    private Direction facingDirection;
    private PlayerState currentState;
    private float speed;
    private float walkSpeed = 100.0f;
    private float runSpeed = 200.0f;
    private float attackCooldown;
    private float attackCooldownTime = 0.5f;
    private bool isSprinting;
    private bool justStartedRangedAttack;

    // Animations
    private Dictionary<Direction, Animation> idleAnimations = null!;
    private Dictionary<Direction, Animation> walkAnimations = null!;
    private Dictionary<Direction, Animation> runAnimations = null!;
    private Dictionary<Direction, Animation> meleeAttackAnimations = null!;
    private Dictionary<Direction, Animation> rangedAttackAnimations = null!;

    private string assetPath = "Tiny Adventure Pack Plus/Character/Char_one";

    // Health (optional feature)
    public int MaxHealth { get; private set; } = 100;
    public int CurrentHealth { get; private set; } = 100;

    public Vector2 Position => position;
    public Direction FacingDirection => facingDirection;
    public PlayerState CurrentState => currentState;
    public float Speed => speed;
    public bool JustStartedRangedAttack => justStartedRangedAttack;

    public Player(Vector2 startPosition)
    {
        position = startPosition;
        facingDirection = Direction.Down;
        currentState = PlayerState.Idle;
        speed = walkSpeed;
        isSprinting = false;

        LoadAnimations();
    }

    private void LoadAnimations()
    {
        idleAnimations = new Dictionary<Direction, Animation>();
        walkAnimations = new Dictionary<Direction, Animation>();
        runAnimations = new Dictionary<Direction, Animation>();
        meleeAttackAnimations = new Dictionary<Direction, Animation>();
        rangedAttackAnimations = new Dictionary<Direction, Animation>();

        // Load idle animations
        idleAnimations[Direction.Down] = new Animation(
            new[] { $"{assetPath}/Idle/Char_idle_down.png" }, 0.2f);
        idleAnimations[Direction.Left] = new Animation(
            new[] { $"{assetPath}/Idle/Char_idle_left.png" }, 0.2f);
        idleAnimations[Direction.Right] = new Animation(
            new[] { $"{assetPath}/Idle/Char_idle_right.png" }, 0.2f);
        idleAnimations[Direction.Up] = new Animation(
            new[] { $"{assetPath}/Idle/Char_idle_up.png" }, 0.2f);

        // Load walk animations
        walkAnimations[Direction.Down] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_down.png" }, 0.15f);
        walkAnimations[Direction.Left] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_left.png" }, 0.15f);
        walkAnimations[Direction.Right] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_right.png" }, 0.15f);
        walkAnimations[Direction.Up] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_up.png" }, 0.15f);

        // Use walk animations for run but with faster frame time
        runAnimations[Direction.Down] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_down.png" }, 0.08f);
        runAnimations[Direction.Left] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_left.png" }, 0.08f);
        runAnimations[Direction.Right] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_right.png" }, 0.08f);
        runAnimations[Direction.Up] = new Animation(
            new[] { $"{assetPath}/Walk/Char_walk_up.png" }, 0.08f);

        // Load melee attack animations
        meleeAttackAnimations[Direction.Down] = new Animation(
            new[] { $"{assetPath}/Attack/Sword/Char_atk_down.png" }, 0.1f, false);
        meleeAttackAnimations[Direction.Left] = new Animation(
            new[] { $"{assetPath}/Attack/Sword/Char_atk_left.png" }, 0.1f, false);
        meleeAttackAnimations[Direction.Right] = new Animation(
            new[] { $"{assetPath}/Attack/Sword/Char_atk_right.png" }, 0.1f, false);
        meleeAttackAnimations[Direction.Up] = new Animation(
            new[] { $"{assetPath}/Attack/Sword/Char_atk_up.png" }, 0.1f, false);

        // Load ranged attack animations
        rangedAttackAnimations[Direction.Down] = new Animation(
            new[] { $"{assetPath}/Attack/Bow/Char_bow_down.png" }, 0.1f, false);
        rangedAttackAnimations[Direction.Left] = new Animation(
            new[] { $"{assetPath}/Attack/Bow/Char_bow_left.png" }, 0.1f, false);
        rangedAttackAnimations[Direction.Right] = new Animation(
            new[] { $"{assetPath}/Attack/Bow/Char_bow_right.png" }, 0.1f, false);
        rangedAttackAnimations[Direction.Up] = new Animation(
            new[] { $"{assetPath}/Attack/Bow/Char_bow_up.png" }, 0.1f, false);
    }

    public void Update(float deltaTime)
    {
        attackCooldown -= deltaTime;

        // Handle input
        HandleInput(deltaTime);

        // Update animations
        UpdateAnimations(deltaTime);

        // Check if attack animation finished
        if (currentState == PlayerState.AttackingMelee || currentState == PlayerState.AttackingRanged)
        {
            var attackAnim = currentState == PlayerState.AttackingMelee
                ? meleeAttackAnimations[facingDirection]
                : rangedAttackAnimations[facingDirection];

            if (attackAnim.IsFinished())
            {
                currentState = PlayerState.Idle;
                attackAnim.Reset();
                justStartedRangedAttack = false;
            }
        }
    }

    private void HandleInput(float deltaTime)
    {
        // Don't allow movement during attack
        if (currentState == PlayerState.AttackingMelee || currentState == PlayerState.AttackingRanged)
        {
            return;
        }

        Vector2 movement = Vector2.Zero;
        bool isMoving = false;

        // Check for sprint (using key code 340 for left shift, 344 for right shift)
        isSprinting = Raylib.IsKeyDown((KeyboardKey)340) || Raylib.IsKeyDown((KeyboardKey)344);
        speed = isSprinting ? runSpeed : walkSpeed;

        // Movement input (using key codes: W=87, S=83, A=65, D=68, Up=265, Down=264, Left=263, Right=262)
        if (Raylib.IsKeyDown((KeyboardKey)87) || Raylib.IsKeyDown((KeyboardKey)265))
        {
            movement.Y -= 1;
            facingDirection = Direction.Up;
            isMoving = true;
        }
        if (Raylib.IsKeyDown((KeyboardKey)83) || Raylib.IsKeyDown((KeyboardKey)264))
        {
            movement.Y += 1;
            facingDirection = Direction.Down;
            isMoving = true;
        }
        if (Raylib.IsKeyDown((KeyboardKey)65) || Raylib.IsKeyDown((KeyboardKey)263))
        {
            movement.X -= 1;
            facingDirection = Direction.Left;
            isMoving = true;
        }
        if (Raylib.IsKeyDown((KeyboardKey)68) || Raylib.IsKeyDown((KeyboardKey)262))
        {
            movement.X += 1;
            facingDirection = Direction.Right;
            isMoving = true;
        }

        // Normalize diagonal movement
        if (movement.Length() > 0)
        {
            movement = Vector2.Normalize(movement);
            position += movement * speed * deltaTime;
        }

        // Update state based on movement
        if (isMoving)
        {
            currentState = isSprinting ? PlayerState.Running : PlayerState.Walking;
        }
        else
        {
            currentState = PlayerState.Idle;
        }

        // Attack input (Space=32, E=69)
        if (attackCooldown <= 0)
        {
            if (Raylib.IsKeyPressed((KeyboardKey)32))
            {
                currentState = PlayerState.AttackingMelee;
                meleeAttackAnimations[facingDirection].Reset();
                attackCooldown = attackCooldownTime;
            }
            else if (Raylib.IsKeyPressed((KeyboardKey)69))
            {
                currentState = PlayerState.AttackingRanged;
                rangedAttackAnimations[facingDirection].Reset();
                attackCooldown = attackCooldownTime;
                justStartedRangedAttack = true;
            }
        }
    }

    private void UpdateAnimations(float deltaTime)
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                idleAnimations[facingDirection].Update(deltaTime);
                break;
            case PlayerState.Walking:
                walkAnimations[facingDirection].Update(deltaTime);
                break;
            case PlayerState.Running:
                runAnimations[facingDirection].Update(deltaTime);
                break;
            case PlayerState.AttackingMelee:
                meleeAttackAnimations[facingDirection].Update(deltaTime);
                break;
            case PlayerState.AttackingRanged:
                rangedAttackAnimations[facingDirection].Update(deltaTime);
                break;
        }
    }

    public void Draw()
    {
        Texture2D currentTexture;

        switch (currentState)
        {
            case PlayerState.Idle:
                currentTexture = idleAnimations[facingDirection].GetCurrentFrame();
                break;
            case PlayerState.Walking:
                currentTexture = walkAnimations[facingDirection].GetCurrentFrame();
                break;
            case PlayerState.Running:
                currentTexture = runAnimations[facingDirection].GetCurrentFrame();
                break;
            case PlayerState.AttackingMelee:
                currentTexture = meleeAttackAnimations[facingDirection].GetCurrentFrame();
                break;
            case PlayerState.AttackingRanged:
                currentTexture = rangedAttackAnimations[facingDirection].GetCurrentFrame();
                break;
            default:
                currentTexture = idleAnimations[facingDirection].GetCurrentFrame();
                break;
        }

        if (currentTexture.Id != 0)
        {
            // Scale up the player sprite (3x size for better visibility)
            float scale = 3.0f;
            Rectangle sourceRect = new Rectangle(0, 0, currentTexture.Width, currentTexture.Height);
            Rectangle destRect = new Rectangle(
                position.X,
                position.Y,
                currentTexture.Width * scale,
                currentTexture.Height * scale
            );
            Raylib.DrawTexturePro(currentTexture, sourceRect, destRect, Vector2.Zero, 0, new Color(255, 255, 255, 255));
        }
    }

    public Vector2 GetAttackDirection()
    {
        switch (facingDirection)
        {
            case Direction.Up:
                return new Vector2(0, -1);
            case Direction.Down:
                return new Vector2(0, 1);
            case Direction.Left:
                return new Vector2(-1, 0);
            case Direction.Right:
                return new Vector2(1, 0);
            default:
                return new Vector2(0, 1);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
    }

    public void ResetRangedAttackFlag()
    {
        justStartedRangedAttack = false;
    }

    public void Unload()
    {
        foreach (var anim in idleAnimations.Values) anim.Unload();
        foreach (var anim in walkAnimations.Values) anim.Unload();
        foreach (var anim in runAnimations.Values) anim.Unload();
        foreach (var anim in meleeAttackAnimations.Values) anim.Unload();
        foreach (var anim in rangedAttackAnimations.Values) anim.Unload();
    }
}

