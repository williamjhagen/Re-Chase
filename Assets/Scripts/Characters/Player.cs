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
        List<GameInput> possibleInputs = inputManager.GetPossibleInputs(GetContext());
        Action ret = availableActions["Idle"];
        currentAnimationName = "Idle";
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
                && Array.Exists(requiredInputs, element => element.Equals(moveOption)))
            {
                //right and left have been handled, animation doesn't care
                currentAnimationName = pair.Key.Replace("Left", "").Replace("Right", "");
                return pair.Value;

            }
        }
        return ret;
    }
    
    /// <summary>
    /// For the player, this is static / hardcoded
    /// </summary>
    protected override void PopulateActions()
    {
        inputDict = new Dictionary<string, GameInput[]>();
        AddAction("Idle", new Idle(this));
        inputDict["Idle"] = new GameInput[] { GameInput.None };
        AddAction("WalkRight", new Walk(this, Direction.Right));
        inputDict["WalkRight"] = new GameInput[] { GameInput.SoftRight, GameInput.None };
        AddAction("WalkLeft", new Walk(this, Direction.Left));
        inputDict["WalkLeft"] = new GameInput[] { GameInput.SoftLeft, GameInput.None };
        AddAction("DashRight", new Dash(this, Direction.Right));
        inputDict["DashRight"] = new GameInput[] { GameInput.HardRight, GameInput.None };
        AddAction("DashLeft", new Dash(this, Direction.Left));
        inputDict["DashLeft"] = new GameInput[] { GameInput.HardLeft, GameInput.None };

    }


}
