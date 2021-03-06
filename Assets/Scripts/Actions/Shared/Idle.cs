﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// literally do nothing.
/// Used for grounded
/// Used so there is always an action running
/// </summary>
public class Idle : Action
{
    public Idle(Character character) : base(character) { }

    public override void Begin()
    {
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
        return false;
    }

    protected override void PopulateChanges()
    {
        changes = new List<WorldStateChange>();
    }
}
