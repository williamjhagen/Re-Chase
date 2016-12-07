using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// Model for all acting characters
/// </summary>
public abstract class Character : MonoBehaviour{

    #region Variables
    //attached components that will be used later
    SpriteRenderer spriteRenderer;
    //the controller that handle sprite animation
    SpriteController spriteController;
    //various states governing what is happening to the character
    protected Direction direction;
    protected State state;
    protected PositionState positionState;
    protected VulnState vulnState;
    //list of ALL actions available to the player
    //children don't need to know what is possible, just available this frame
    [SerializeField]
    private Dictionary<string, Action> actions;
    //list of actions the character can currently do
    protected Dictionary<string, Action> availableActions;
    //the action currently being run by the character
    protected Action currentAction;
    //the name of the animation currently being played
    //passed to the spriteController to begin the animation
    protected string currentAnimationName;

    //health
    [SerializeField]
    public int maxHealth;
    private int health;

    //movement variables
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float jumpLength;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float fallSpeed;
    [NonSerialized]
    public bool fastFalling = false;
    [SerializeField]
    private float airSpeed;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashLength;
    private float speedModifier = 1;
    [SerializeField]
    private int numAirJumps = 1;
    public int jumpsRemaining;
    //TODO: Adjust knockback via percent?
    private int percent;
    private Vector2 velocity;
    #endregion

    #region Init
    public void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteController = GetComponentInChildren<SpriteController>();
        actions = new Dictionary<string, Action>();
        availableActions = new Dictionary<string, Action>();
        PopulateActions();
        Init();
    }
    protected void Init()
    {
        health = maxHealth;
        jumpsRemaining = numAirJumps;
        direction = Direction.Right;
        state = State.Free;
        vulnState = VulnState.Neutral;
        positionState = PositionState.Grounded;
        UpdateAvailableActions();
        currentAction = GetAction();
        velocity = new Vector2();
        StartCoroutine("Act");
        StartCoroutine("AerialMovement");
    }
    #endregion

    #region Methods
    /// <summary>
    /// do the thing
    /// if the thing is done, get something else to do
    /// </summary>
    protected IEnumerator Act()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            if(!currentAction.Run())
            {
                Action newAction = GetAction();
                if (newAction != null)
                {
                    if (newAction != currentAction)
                        spriteController.PlayAnimation(currentAnimationName);
                    currentAction = newAction;
                    currentAction.Begin();
                    ActionSelected();
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// handles how the character moves in the air
    /// whether they jumped or were hit
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AerialMovement()
    {
        while(true)
        {
            if(positionState == PositionState.Aerial)
            {
                Vector2  di = GetDI();
                di.y = 0;
                Vector2 temp = transform.position;
                ///di
                //Debug.Log(di * airSpeed * Time.deltaTime);
                velocity += di * airSpeed * Time.deltaTime;
                //gravity
                velocity.y -= FallSpeed * Time.deltaTime;

                velocity *= .9f;

                temp += velocity;
                transform.position = temp;
            }
            if(state != State.Dodge && currentAction.GetType() != typeof(Attack)) CheckGrounded();
            yield return null;
        }
    }

    /// <summary>
    /// updates internal list of available actions given current state
    /// </summary>
    protected void UpdateAvailableActions()
    {
        availableActions.Clear();
        foreach(var action in actions)
        {
            if (action.Value.IsValid()) availableActions[action.Key] = action.Value;
        }
    }
    /// <summary>
    /// Coroutine that handles modifying a character's base speed
    /// </summary>
    /// <param name="modifier">Percentage speed to be additively applied</param>
    /// <param name="time">time in seconds for the modifier to last</param>
    public IEnumerator AddSpeedMod(float modifier, float time)
    {
        speedModifier += modifier;

        yield return new WaitForSeconds(time);

        speedModifier -= modifier;
    }

    /// <summary>
    /// returns any applicable information about the character's state in the world
    /// </summary>
    public CharacterContext GetContext()
    {
        return new CharacterContext(direction, positionState, state, vulnState, jumpsRemaining);
    }

    /// <summary>
    /// raycast down to check if the character is standing on the ground
    /// </summary>
    /// <returns>whether or not the character is standing</returns>
    public void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1.2f);
        if(hit.point != Vector2.zero)
        {
            positionState = PositionState.Grounded;
            jumpsRemaining = numAirJumps;
            Vector2 temp = transform.position;
            temp.y = hit.point.y + 1.2f;
            transform.position = temp;
            fastFalling = false;
            if (currentAction.IsValid() == false) currentAction.Interupt();
        }
        else if(positionState == PositionState.Grounded && state == State.Free)
        {
            positionState = PositionState.Aerial;
        }

    }

    /// <summary>
    /// Can be used to initiate fast falling, knockback, etc
    /// </summary>
    /// <param name="force">the force vector</param>
    /// <param name="knockback">If it is a knockback, account for percent damage</param>
    public void ApplyForce(Vector2 force, bool knockback = false)
    {
        //TODO: percent based knockback
        velocity.y += force.y;
        velocity.x += force.x;
    }

    /// <summary>
    /// stops vertical momentum, used primarily for double jumping
    /// </summary>
    public void ClearForceVertical()
    {
        velocity.y = 0;
    }

    /// <summary>
    /// actions is a write only list. enforced here.
    /// </summary>
    /// <param name="action"></param>
    protected void AddAction(string key, Action action)
    {
        actions[key] = action; 
    }

    /// <summary>
    /// event fired to children to say an action was successfully ran
    /// </summary>
    abstract protected void ActionSelected();

    /// <summary>
    /// Figure out what action needs to be run
    /// NPCS will rely on an NPC manager
    /// Player will rely on inputs
    /// </summary>
    /// <returns></returns>
    abstract protected Action GetAction();

    /// <summary>
    /// The player can always have directional influence
    /// determine what this should be
    /// </summary>
    /// <returns>A vector 2 representing the direction the character move</returns>
    abstract protected Vector2 GetDI();

    /// <summary>
    /// Children are responsible for handling which actions they have access too
    /// </summary>
    abstract protected void PopulateActions();
    #endregion

    #region Properties
    public int Health
    {
        get { return health; }
        set
        {
            if (health - value >= 0) health = 0;
            else health -= value;
        }
    }
    

    public float WalkSpeed
    {
        get
        {
            return walkSpeed * speedModifier;
        }
    }

    public float RunSpeed
    {
        get
        {
            return runSpeed * speedModifier;
        }
    }

    public float JumpLength
    {
        get
        {
            return jumpLength;
        }
    }

    public float FallSpeed
    {
        get
        {
            return fastFalling ? fallSpeed * 2 : fallSpeed;
        }
    }

    public float DashSpeed
    {
        get
        {
            return dashSpeed * speedModifier;
        }
    }

    public float DashLength
    {
        get
        {
            return dashLength;
        }
    }

    public float SpeedMod
    {
        get
        {
            return speedModifier;
        }
    }

    public float AirSpeed
    {
        get
        {
            return airSpeed * speedModifier;
        }
    }

    public Direction Direction
    {
        get { return spriteRenderer.flipX ? Direction.Left : Direction.Right; }
        set
        {
            direction = value;
            if (value == Direction.Left) spriteRenderer.flipX = true;
            else if (value == Direction.Right) spriteRenderer.flipX = false;
        }
    }

    public PositionState PositionState
    {
        get
        {
            return positionState;
        }
        set
        {
            positionState = value;
        }
    }

    public VulnState VulnState
    {
        get
        {
            return vulnState;
        }
        set
        {
            vulnState = value;
        }
    }

    public State State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }

    public float JumpSpeed
    {
        get
        {
            return jumpSpeed;
        }
        
    }

    #endregion


}
public struct CharacterContext
{
    public CharacterContext(Direction dir, PositionState posState, State state, VulnState vulnState, int remainingJumps)
    {
        this.remainingJumps = remainingJumps;
        this.dir = dir;
        this.posState = posState;
        this.state = state;
        this.vulnState = vulnState;
    }
    public int remainingJumps;
    public Direction dir;
    public PositionState posState;
    public State state;
    public VulnState vulnState;
}
#region States
public enum Direction
{
    Left,
    Right
}
public enum PositionState
{
    Grounded,
    Aerial,
    Ascending,
    InVehicle
}

public enum State
{
    Free,
    Hitstun,
    Grabbed,
    Tumble,
    Attacking,
    StartUp,
    Smashing,
    Shield,
    Dodge,
    Grab
}

public enum VulnState
{
    Neutral,
    Invulnerable,
    Armored
}


public enum WorldStateChange
{
    MoveLeft,
    MoveRight,
    MoveDown,
    MoveUp,
    AttackClose,
    AttackFar,
}
#endregion

