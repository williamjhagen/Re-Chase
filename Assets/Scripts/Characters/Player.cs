using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Player : Character {

    [SerializeField]
    private InputManager inputManager;
    private Dictionary<string, GameInput[]> inputDict;

    public void Start()
    {
        base.Start();
    }
    /// <summary>
    /// Query the input manager for stick and button inputs
    /// Determine based on that, and the player's current state what action should be taken
    /// </summary>
    /// <returns></returns>
    protected override Action GetAction()
    {
        UpdateAvailableActions();

        //run through possible inputs in priority order to select the best one
        //perhaps overly verbose -- TODO: simplify
        GameInput actionOption = inputManager.GetActionOption();
        GameInput moveOption = inputManager.GetMovementOption();

        //loop through each currently available action
        foreach(var pair in availableActions)
        {
            //if any action contains both return inputs
            GameInput[] requiredInputs = inputDict[pair.Key];
            if ( Array.Exists(requiredInputs, element => element.Equals(actionOption))
                && (Array.Exists(requiredInputs, element => element.Equals(moveOption)) || Array.Exists(requiredInputs, element => element.Equals(GameInput.AnyMovement))))
            {
                //right and left have been handled, animation doesn't care
                currentAnimationName = pair.Key.Replace("Left", "").Replace("Right", "");
                return pair.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// get this directly from the joystick
    /// </summary>
    protected override Vector2 GetDI()
    {
        //update fast falling state here
        fastFalling = inputManager.InputInBuffer(GameInput.HardDown);
        return inputManager.GetCurrentJoystick();
    }

    /// <summary>
    /// clear our input buffer when a new action is selected
    /// </summary>
    protected override void ActionSelected()
    {
        inputManager.ClearBuffer();
    }
    /// <summary>
    /// For the player, this is static / hardcoded
    /// </summary>
    protected override void PopulateActions()
    {
        inputDict = new Dictionary<string, GameInput[]>();
        AddAction("Idle", new Idle(this));
        inputDict["Idle"] = new GameInput[] { GameInput.None };
        AddAction("Float", new Float(this));
        inputDict["Float"] = new GameInput[] { GameInput.None };
        AddAction("WalkRight", new Walk(this, Direction.Right));
        inputDict["WalkRight"] = new GameInput[] { GameInput.SoftRight, GameInput.None };
        AddAction("WalkLeft", new Walk(this, Direction.Left));
        inputDict["WalkLeft"] = new GameInput[] { GameInput.SoftLeft, GameInput.None };
        AddAction("DashRight", new Dash(this, Direction.Right));
        inputDict["DashRight"] = new GameInput[] { GameInput.HardRight, GameInput.None };
        AddAction("DashLeft", new Dash(this, Direction.Left));
        inputDict["DashLeft"] = new GameInput[] { GameInput.HardLeft, GameInput.None };
        AddAction("Jump", new Jump(this));
        inputDict["Jump"] = new GameInput [] { GameInput.Jump, GameInput.AnyMovement};
        AddAction("SlideRight", new Slide(this, Direction.Right));
        inputDict["SlideRight"] = new GameInput[] { GameInput.R, GameInput.AnyMovement };
        AddAction("SlideLeft", new Slide(this, Direction.Left));
        inputDict["SlideLeft"] = new GameInput[] { GameInput.L, GameInput.AnyMovement };
        AddAction("DoubleJump", new DoubleJump(this));
        inputDict["DoubleJump"] = new GameInput[] { GameInput.Jump, GameInput.AnyMovement };
        AddAction("Attack", new Attack(this));
        inputDict["Attack"] = new GameInput[] { GameInput.Attack, GameInput.AnyMovement };
        AddAction("JumpAttack", new JumpAttack(this));
        inputDict["JumpAttack"] = new GameInput[] { GameInput.Attack, GameInput.AnyMovement };
        AddAction("Throw", new Throw(this));
        inputDict["Throw"] = new GameInput[] { GameInput.Special, GameInput.AnyMovement };
        AddAction("JumpThrow", new JumpThrow(this));
        inputDict["JumpThrow"] = new GameInput[] { GameInput.Special, GameInput.AnyMovement };

    }


}
