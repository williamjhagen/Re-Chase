﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Walk : Action {

    public Walk(Character character) : base(character) { }

    public override void Begin()
    {
        inputs = new List<GameInput>();
        inputs.Add(GameInput.SoftRight);
    }

    /// <summary>
    /// Just keep walking
    /// </summary>
    /// <returns>always false</returns>
    public override bool Run()
    {
        Vector3 position  = new Vector3(character.transform.position.x, character.transform.position. y, character.transform.position.z);
        position.x = position.x + (character.WalkSpeed * Time.deltaTime) * (character.Direction == Direction.Left ? -1f : 1f);
        character.transform.position = position;
        //always return true
        return true;
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
        return;
    }
    public override void Interupt()
    {
        Finish();
    }
}