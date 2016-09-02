using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// literally do nothing.
/// Can be in air or on ground
/// Used so there is always an action running
/// </summary>
public class Idle : Action
{
    public Idle(Character character) : base(character) { }

    public override void Begin()
    {
        inputs = new List<GameInput>();
        inputs.Add(GameInput.None);
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
        return character.State == State.Free;
    }

    public override bool Run()
    {
        return false;
    }
}
