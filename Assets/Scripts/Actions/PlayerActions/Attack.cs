using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Attack : AttackAction {

    public Attack(Character character) : base(character) { }
    int frame = 0;

    public override void Begin()
    {
        character.State = State.Attacking;
        frame = 0;
    }

    public override void Finish()
    {
        character.State = State.Free;
    }

    public override void Interupt()
    {
        frame = 50;
        Debug.Log("?");
    }

    public override bool IsValid()
    {
        return character.State == State.Free && character.PositionState == PositionState.Grounded;
    }

    public override bool Run()
    {
        ++frame;
        if (frame < 50) return true;
        else
        {
            Finish();
            return false;
        }
    }

    protected override void PopulateChanges()
    {
        changes = new List<WorldStateChange>();
        changes.Add(WorldStateChange.AttackClose);
    }
}
