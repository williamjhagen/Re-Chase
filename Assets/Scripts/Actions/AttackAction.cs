using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AttackAction : Action
{
   public AttackAction(Character character) : base(character) { }
}

