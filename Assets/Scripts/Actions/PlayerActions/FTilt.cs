using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FTilt : Action {

    public FTilt(Character character) : base(character) { }

    public override void Begin()
    { 
        inputs.Add(GameInput.SoftRight);
        inputs.Add(GameInput.Attack);
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
        throw new NotImplementedException();
    }
    
}
