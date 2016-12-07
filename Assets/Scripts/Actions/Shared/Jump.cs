using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// literally do nothing.
/// Can be in air or on ground
/// Used so there is always an action running
/// </summary>
public class Jump : Action
{
    public Jump(Character character) : base(character) { }
    private float startPos;
    int frame = 0;

    public override void Begin()
    {
        character.State = State.StartUp;
        character.PositionState = PositionState.Ascending;
        startPos = character.transform.position.y;
        frame = 0;
    }

    public override void Finish()
    {
        return;
    }

    public override void Interupt()
    {
        Finish();
    }

    public override bool IsValid()
    {
        return character.State == State.Free && character.PositionState == PositionState.Grounded;
    }

    public override bool Run()
    {
        //6 frame startup
        ++frame;
        if(frame == 2)
        {
            
            character.State = State.Free;
            //Ascending state prevents ground clamping when starting the jump
            character.PositionState = PositionState.Aerial;
            //move the character
            character.ApplyForce( new Vector2(0, character.JumpSpeed));
            return true;
        }
        return false;
    }

    protected override void PopulateChanges()
    {
        changes = new List<WorldStateChange>();
        changes.Add(WorldStateChange.MoveUp);
    }
}
