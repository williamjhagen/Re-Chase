using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class Action{
    //the character using this action
    protected Character character;
    protected List<GameInput> inputs;

    public Action(Character character)
    {
        this.character = character;
    }


    /// <summary>
    /// run setup for the action
    /// </summary>
    public abstract void Begin();
    
    /// <summary>
    /// The primary running code for the action
    /// updates hitboxes / hurtboxes, etc.
    /// </summary>
    /// <returns>returns true if the action has successfully completed</returns>
    public abstract bool Run();

    /// <summary>
    /// executes when the action completes its normal cycle
    /// </summary>
    public abstract void Finish();

    /// <summary>
    /// to be called if the action finishes prematurely 
    /// </summary>
    public abstract void Interupt();

    /// <summary>
    /// Determine if this action is currently available based on 
    /// the character's current state
    /// </summary>
    /// <returns></returns>
    public abstract bool IsValid();

    /// <summary>
    /// return a list of inputs required for a player to execute this action
    /// null implies the player can't initiate the action
    /// </summary>
    public List<GameInput> GetInputs()
    {
        return new List<GameInput>(inputs);
    }
}
