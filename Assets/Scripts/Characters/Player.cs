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
        inputDict = new Dictionary<string, GameInput[]>();
        PopulateActions();
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
        AddAction("Idle", new Idle(this));
        inputDict["Idle"] = new GameInput[] { GameInput.None };
        AddAction("Walk", new Walk(this));
        inputDict["Walk"] = new GameInput[] { GameInput.SoftLeft, GameInput.SoftRight, GameInput.None };
        
    }


}
