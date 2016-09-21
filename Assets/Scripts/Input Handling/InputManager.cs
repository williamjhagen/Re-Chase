using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class InputManager : MonoBehaviour {

    
    //ranges for reading movement inputs
    private float cStickMin = .6f;
    private float lowRange = .001f;
    //different sensitivities for each direction
    //gamecube controllers suck
    private float highRangeRight = .7f;
    private float highRangeLeft = .6f;
    private float highRangeUp = .6f;
    private float highRangeDown = .6f;
    [SerializeField]
    private Buffer buffer;

    public bool DoAction(Action action)
    {
        
        return false;

    }

    public GameInput GetActionOption()
    {
        int len = buffer.Count;
        //readability over efficiency
        //buffer should be short given human limits
        if (buffer.Contains(GameInput.Special)) return GameInput.Special;
        if (buffer.Contains(GameInput.Attack)) return GameInput.Attack;
        if (buffer.Contains(GameInput.Jump)) return GameInput.Jump;
        if (buffer.Contains(GameInput.Grab)) return GameInput.Grab;
        if (buffer.Contains(GameInput.Shield)) return GameInput.Shield;

        return GameInput.None;
    }

    //hard > soft
    //up > right > left > down (character usually wants to go right), ducking is w/e
    public GameInput GetMovementOption()
    {
        if (buffer.Contains(GameInput.HardUp)) return GameInput.HardUp;
        if (buffer.Contains(GameInput.HardRight)) return GameInput.HardRight;
        if (buffer.Contains(GameInput.HardLeft)) return GameInput.HardLeft;
        if (buffer.Contains(GameInput.HardDown)) return GameInput.HardDown;
        if (buffer.Contains(GameInput.SoftUp)) return GameInput.SoftUp;
        if (buffer.Contains(GameInput.SoftRight)) return GameInput.SoftRight;
        if (buffer.Contains(GameInput.SoftLeft)) return GameInput.SoftLeft;
        if (buffer.Contains(GameInput.SoftDown)) return GameInput.SoftDown;

        return GameInput.None;
    }

    public List<GameInput> GetPossibleInputs(CharacterContext context)
    {
        List<GameInput> ret = new List<GameInput>();

        if (context.state == State.Free)
        {
            ret.Add(GameInput.Special);
            ret.Add(GameInput.Attack);
            ret.Add(GameInput.Shield);

            if (context.posState == PositionState.Grounded)
            {
                ret.Add(GameInput.Grab);
            }
            else if (context.posState == PositionState.Aerial)
            {
                if (context.remainingJumps > 0)
                {
                    ret.Add(GameInput.Jump);
                }
            }
        }
        return ret;
    }
    /// <summary>
    /// read inputs once per frame
    /// this is update instead of a coroutine because we want it called before inputs are processed
    /// to increase responsiveness by 1 frame
    /// NOTE:  this method should ONLY gather information
    /// leave decisions to other methods
    /// </summary>
    void Update()
    {
        //BUG: buttons not being read?
        //get action inputs
        if (Input.GetButtonDown("s")) buffer.RegisterInput(GameInput.Special);
        if (Input.GetButtonDown("a")) buffer.RegisterInput(GameInput.Attack);
        if (Input.GetButtonDown("s")) buffer.RegisterInput(GameInput.Shield);
        if (Input.GetButtonDown("z")) buffer.RegisterInput(GameInput.Grab);
        if (Input.GetButtonDown("jump")) buffer.RegisterInput(GameInput.Jump);
        //c stick inputs
        if (Input.GetAxis("C Horizontal") > cStickMin)
        {
            buffer.RegisterInput(GameInput.HardRight);
            buffer.RegisterInput(GameInput.Attack);
        }
        if (Input.GetAxis("C Horizontal") < -cStickMin)
        {
            buffer.RegisterInput(GameInput.HardLeft);
            buffer.RegisterInput(GameInput.Attack);
        }
        if (Input.GetAxis("C Vertical") > cStickMin)
        {
            buffer.RegisterInput(GameInput.HardUp);
            buffer.RegisterInput(GameInput.Attack);
        }
        if (Input.GetAxis("C Vertical") < -cStickMin)
        {
            buffer.RegisterInput(GameInput.HardDown);
            buffer.RegisterInput(GameInput.Attack);
        }
        /* movement controls are more complicated, and differ depending on keyboard vs joystick
         * see individual comments for details on implementations
         */
        if (Input.GetAxis("Vertical") > lowRange && Input.GetAxis("Vertical") < highRangeUp)
            buffer.RegisterInput(GameInput.SoftUp);
        if (Input.GetAxis("Vertical") > highRangeUp)
            buffer.RegisterInput(GameInput.HardUp);
        if (Input.GetAxis("Vertical") < -lowRange && Input.GetAxis("Vertical") > -highRangeLeft)
            buffer.RegisterInput(GameInput.SoftDown);
        if (Input.GetAxis("Vertical") < -highRangeDown)
            buffer.RegisterInput(GameInput.HardDown);
        if (Input.GetAxis("Horizontal") > lowRange && Input.GetAxis("Horizontal") < highRangeRight)
            buffer.RegisterInput(GameInput.SoftRight);
        if (Input.GetAxis("Horizontal") > highRangeRight)
            buffer.RegisterInput(GameInput.HardRight);
        if (Input.GetAxis("Horizontal") < -lowRange && Input.GetAxis("Horizontal") > -highRangeLeft) 
            buffer.RegisterInput(GameInput.SoftLeft);
        if (Input.GetAxis("Horizontal") < -highRangeLeft)
            buffer.RegisterInput(GameInput.HardLeft);
        //TODO: Dpad?
    }
}

/// <summary>
/// This is what the player can extract from the buffer
/// </summary>
public struct ActionStruct
{
    public GameAction action;
    public GameDirection direction;

    public ActionStruct(GameAction action = GameAction.Idle, GameDirection direction = GameDirection.None)
    {
        this.action = action;
        this.direction = direction;
    }
}

/// <summary>
/// cardinal directions to determine action directions
/// </summary>
public enum GameDirection
{
    None,
    Left,
    Right,
    Up,
    Down
}

public enum GameAction
{
    Idle,
    Tilt,
    Smash,
    DashAttack,
    Dash,
    Dodge,
    Shield,
    Walk,
    Grab,
}

public enum GameInput
{
    None,
    Attack,
    Special,
    Shield,
    Grab,
    Jump,
    SoftUp,
    HardUp,
    SoftLeft,
    HardDown,
    SoftDown,
    SoftRight,
    HardRight,
    HardLeft,
}
