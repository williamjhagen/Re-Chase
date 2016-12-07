using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Throw : AttackAction
{
    int frame = 0;
    int len = 40;
    public Throw(Character character) : base(character) { }

    public override void Begin()
    {
        character.State = State.Attacking;
        frame = 1;
    }

    public override void Finish()
    {
        character.State = State.Free;
    }

    public override void Interupt()
    {
        frame = len + 1;
    }

    public override bool IsValid()
    {
        return frame > 0 || character.State == State.Free && character.PositionState == PositionState.Grounded;
    }

    public override bool Run()
    {
        ++frame;
        if (frame == len / 2)
        {
            GameObject t = GameObject.Instantiate(Resources.Load("Kunai")) as GameObject;
            t.GetComponent<Kunai>().player = character;
            return true;
        }
        if (frame < len) return true;

        Finish();
        return false;
    }
    protected override void PopulateChanges()
    {
        changes = new List<WorldStateChange>();
        changes.Add(WorldStateChange.AttackFar);
    }
}
