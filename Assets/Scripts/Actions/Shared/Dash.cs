using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class Dash : Action
{ 

    Direction facing;
    int frame = 0;

    public Dash(Character character, Direction dir) : base(character) { facing = dir; }

    public override void Begin()
    {
        character.Direction = Facing;
        frame = 0;
    }

    /// <summary>
    /// Just keep walking
    /// </summary>
    /// <returns>always false</returns>
    public override bool Run()
    {
        ++frame;
        Vector3 position = new Vector3(character.transform.position.x, character.transform.position.y, character.transform.position.z);
        position.x = position.x + (character.RunSpeed * Time.deltaTime) * (character.Direction == Direction.Left ? -1f : 1f);
        character.transform.position = position;
        
        return frame < 5;
    }

    public override bool IsValid()
    {
        return (character.PositionState == PositionState.Grounded && character.State == State.Free);
    }

    /// <summary>
    /// nothing to see here
    /// </summary>
    public override void Finish()
    {
        frame = 0;
        return;
    }
    public override void Interupt()
    {
        Finish();
    }

    protected override void PopulateChanges()
    {
        changes = new List<WorldStateChange>();
        changes.Add(facing == Direction.Left ? WorldStateChange.MoveLeft : WorldStateChange.MoveRight);
    }


    public Direction Facing
    {
        get
        {
            return facing;
        }
    }
}
