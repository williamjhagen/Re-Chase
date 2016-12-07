using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class Slide : Action
{

    Direction facing;

    public Slide(Character character, Direction dir) : base(character) { facing = dir; }
    int frame;

    public override void Begin()
    {
        character.Direction = facing;
        frame = 0;
        Vector2 temp = character.transform.position;
        temp.y -= .35f;
        character.transform.position = temp;
        character.State = State.Dodge;
    }

    /// <summary>
    /// Just keep walking
    /// </summary>
    /// <returns>always false</returns>
    public override bool Run()
    {
        if (frame < 0) return false;
        else if (frame < character.DashLength)
        {
            ++frame;
            Vector3 position = new Vector3(character.transform.position.x, character.transform.position.y, character.transform.position.z);
            position.x = position.x + (character.DashSpeed * Time.deltaTime) * (character.Direction == Direction.Left ? -1f : 1f);
            character.transform.position = position;
            return true;
        }
        else
        {
            Finish();
            return false;
        }

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
        if (frame < 0) return;
        frame = -1;
        Vector2 temp = character.transform.position;
        temp.y += .35f;
        character.transform.position = temp;
        character.State = State.Free;
    }
    public override void Interupt()
    {
        Finish();
    }

    protected override void PopulateChanges()
    {
        changes = new List<WorldStateChange>();
        changes.Add(facing == Direction.Left ? WorldStateChange.MoveLeft : WorldStateChange.MoveRight);
        changes.Add(WorldStateChange.MoveDown);
    }
}
