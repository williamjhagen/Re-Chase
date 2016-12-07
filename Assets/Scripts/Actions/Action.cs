using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class Action{
    //the character using this action
    protected Character character;
    protected string name;
    protected List<WorldStateChange> changes;

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
    /// <returns>returns true if the action is running</returns>
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
    ///  
    /// </summary>
    /// <returns>a list of changes to the world state</returns>
    public List<WorldStateChange> StateChange()
    {
        if (changes == null) PopulateChanges();
        return changes;
    }

    /// <summary>
    /// only called once, populates the action's changes to the world state
    /// </summary>
    protected abstract void PopulateChanges();

    /// <summary>
    /// returns name, used to sync with animation system
    /// </summary>
    public string Name { get { return name; } }

}
