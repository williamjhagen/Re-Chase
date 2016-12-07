using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class JumpThrow : AttackAction
{
    private GameObject kunai;
    int len = 25;
    int frame = 0;

    public JumpThrow(Character character) : base(character) { }

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
        frame = len + 1;
    }

    public override bool IsValid()
    {
        return character.State == State.Free && character.PositionState == PositionState.Aerial;
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
