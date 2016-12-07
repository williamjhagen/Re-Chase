using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// literally do nothing.
/// Can be in air or on ground
/// Used so there is always an action running
/// </summary>
public class DoubleJump : Action
{
    public DoubleJump(Character character) : base(character) { }

    public override void Begin()
    {
        character.ClearForceVertical();
        character.ApplyForce(new Vector2(0, character.JumpSpeed * .75f));
        character.fastFalling = false;
        --character.jumpsRemaining;
        //character can change directions
        float xAxis = Input.GetAxis("Horizontal");
        if ( xAxis < -.4f)
        {
            character.Direction = Direction.Left;
        }
        else if(xAxis > .4f)
        {
            character.Direction = Direction.Right;
        }
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
        return character.State == State.Free && character.PositionState == PositionState.Aerial && character.jumpsRemaining > 0;
    }

    public override bool Run()
    {
        return false;
    }

    protected override void PopulateChanges()
    {
        changes = new List<WorldStateChange>();
        changes.Add(WorldStateChange.MoveUp);
    }
}
