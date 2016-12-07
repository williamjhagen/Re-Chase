using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NPC : Character
{
    /// <summary>
    /// the other npc or player
    /// </summary>
    [SerializeField]
    public Character opponent;


    //all personality variables should be constrained to -1 to 1

    //how frequently the character attacks
    [SerializeField]
    private float aggression;
    //how often the character jumps
    [SerializeField]
    private float bounciness;
    //how quickly the character moves, and for how long
    [SerializeField]
    private float energy;
    //how worried the character is about getting hit
    [SerializeField]
    private float caution;
    //ranged vs melee attacks
    [SerializeField]
    private float closer;
    //randomness
    [SerializeField]
    private float spontaneity;
    Vector2 DI;

    const float IMPORTANCE_URGENT = 3f;
    const float IMPORTANCE_HIGH = .75f;
    const float IMPORTANCE_MEDIUM = .25f;
    const float IMPORTANCE_LOW = .1f;

    //state things
    private Vector2 distanceFromEnemy;
    int attacksRecently = 1;


    WorldState currentState;
    WorldState goalState;

    bool moving = false;

    public void Start()
    {
        //currentState.distanceFromEnemy = opponent.transform.position - transform.position;
        StartCoroutine("RecoverStamina");
        base.Start();
    }
    /// <summary>
    /// Query the input manager for stick and button inputs
    /// Determine based on that, and the player's current state what action should be taken
    /// </summary>
    /// <returns></returns>
    protected override Action GetAction()
    {
        UpdateAvailableActions();
        Action action = null;
        float best = -2f;
        UpdateState();
        foreach (var item in availableActions)
        {
            float score = Evaluate(item.Value);
            if (score > best)
            {
                action = item.Value;
                best = score;
                currentAnimationName = item.Key.Replace("Left", "").Replace("Right", "");
            }
        }
        if (action != null)
        {
            var actionType = action.GetType();
            if (action is AttackAction)
            {
                currentState.attacking = true;
                //direction = PreferredDirection();
            }
            else
                currentState.attacking = false;

            if (actionType != typeof(Dash) && action.GetType() != typeof(Float) && action.GetType() != typeof(Idle))
            {
                currentState.stamina = 0;
            }
            
        }
        return action;
    }


    private float Evaluate(Action action)
    {
        float eval = 0f;
        List<WorldStateChange> changes = action.StateChange();
        foreach (WorldStateChange change in changes)
        {
            switch (change)
            {
                case WorldStateChange.AttackClose:
                    if (goalState.MeleeRange && currentState.MeleeRange && goalState.attacking)
                        eval += IMPORTANCE_HIGH;
                    if (currentState.FarRange || currentState.MidRange)
                        eval -= IMPORTANCE_URGENT;
                    if (direction != currentState.DirectionOfOpponent)
                        eval -= IMPORTANCE_URGENT;
                    if (currentState.Tired)
                        eval -= IMPORTANCE_URGENT;
                    break;
                case WorldStateChange.AttackFar:
                    if (goalState.FarRange && currentState.FarRange && goalState.attacking)
                        eval += IMPORTANCE_HIGH;
                    if (goalState.MidRange && currentState.MidRange && goalState.attacking)
                        eval += IMPORTANCE_HIGH;
                    if (currentState.MeleeRange)
                        eval -= IMPORTANCE_URGENT;
                    if (direction != currentState.DirectionOfOpponent)
                        eval -= IMPORTANCE_URGENT;
                    if (currentState.Tired)
                        eval -= IMPORTANCE_URGENT;
                    break;
                case WorldStateChange.MoveDown:
                    if (currentState.stamina > 1)
                        eval += IMPORTANCE_LOW;
                    if (currentState.Tired)
                        eval -= IMPORTANCE_URGENT;
                    break;
                case WorldStateChange.MoveLeft:
                    if (PreferredDirection() == Direction.Left)
                        eval += IMPORTANCE_LOW;
                    else
                        eval -= IMPORTANCE_HIGH;
                    break;
                case WorldStateChange.MoveRight:
                    if (PreferredDirection() == Direction.Right)
                        eval += IMPORTANCE_LOW;
                    else
                        eval -= IMPORTANCE_HIGH;
                    break;
                case WorldStateChange.MoveUp:
                    if (goalState.Above && !currentState.Above)
                        eval += IMPORTANCE_MEDIUM;
                    else
                        eval -= IMPORTANCE_LOW;
                    if (currentState.Tired)
                        eval -= IMPORTANCE_HIGH;
                    if (direction != PreferredDirection())
                        eval -= IMPORTANCE_MEDIUM;
                    
                    break;
            }
            
        }

        if (action.GetType() == typeof(Slide) && currentState.Tired)
            eval -= IMPORTANCE_URGENT;
        if (   changes.Count == 0
            && direction == currentState.DirectionOfOpponent && currentState.Tired
            && currentState.MeleeRange == goalState.MeleeRange
            && currentState.MidRange == goalState.MidRange
            && currentState.FarRange == goalState.FarRange
            )
            eval += IMPORTANCE_HIGH;
        {
            float rand = (spontaneity + 1.1f) / 4f;
            return eval + Random.Range(-rand, rand);
        }
    }

    public void UpdateGoal()
    {
        Vector2 tarDist = new Vector2();
        //min distance should be  determined by the closer variable
        float maxDist = (1f - (closer + 1f) / 2f) * 10f;
        //min distance dominated by caution
        float minDist = (1f - (caution + 1) / 2f) * 2f;
        tarDist.x = Random.Range(minDist, maxDist);
        tarDist.y = ((bounciness + 1f) / 2f) * Random.Range(0, .3f);
        bool attacking = ((aggression+1f)/2f  > Random.Range(0f,1f));
        goalState = new WorldState(tarDist, attacking);
    }

    private void UpdateState()
    {
        currentState.distanceFromEnemy = opponent.transform.position - transform.position;
        if (currentState == goalState) UpdateGoal();
    }

    private Direction PreferredDirection()
    {
        //return (currentState.distanceFromEnemy.x > goalState.distanceFromEnemy.x) ? Direction.Right : Direction.Left;
        return (transform.position.x < TargetPosition().x) ? Direction.Right : Direction.Left;
    }

    private Vector2 TargetPosition()
    {
        Vector2 target = opponent.transform.position;
        if (currentState.DirectionOfOpponent == Direction.Left)
            target.x += goalState.distanceFromEnemy.x;
        else
            target.x -= goalState.distanceFromEnemy.x;
        target.y += goalState.distanceFromEnemy.y + 1.1f;
        return target;
    }
    /// <summary>
    /// get this directly from the joystick
    /// </summary>
    protected override Vector2 GetDI()
    {
        //TODO: AI Controlled DI
        Vector2 v2pos = transform.position;
        return (TargetPosition() - v2pos).normalized / 2f;
    }

    IEnumerator RecoverStamina()
    {
        while (true)
        {
            currentState.stamina += (energy + 2f) / 2f * Time.deltaTime;
            //currentState.stamina = Mathf.Max(currentState.stamina, 0f);
            yield return null;
        }
    }
    /// <summary>
    /// clear our input buffer when a new action is selected
    /// </summary>
    protected override void ActionSelected()
    {
        return;
    }
    /// <summary>
    /// For the player, this is static / hardcoded
    /// </summary>
    protected override void PopulateActions()
    {

        AddAction("Idle", new Idle(this));
        AddAction("Float", new Float(this));
        AddAction("DashRight", new Dash(this, Direction.Right));
        AddAction("DashLeft", new Dash(this, Direction.Left));
        AddAction("Jump", new Jump(this));
        AddAction("SlideRight", new Slide(this, Direction.Right));
        AddAction("SlideLeft", new Slide(this, Direction.Left));
        AddAction("DoubleJump", new DoubleJump(this));
        AddAction("Attack", new Attack(this));
        AddAction("JumpAttack", new JumpAttack(this));
        AddAction("Throw", new Throw(this));
        AddAction("JumpThrow", new JumpThrow(this));
    }

    public float Aggression
    {
        get
        {
            return aggression;
        }
        set
        {
            aggression = value;
            if (aggression > 1f) aggression = 1f;
            if (aggression < -1f) aggression = -1f;
        }
    }
    public float Spontaneity
    {
        get
        {
            return spontaneity;
        }
        set
        {
            spontaneity = value;
            if (spontaneity > 1f) spontaneity = 1f;
            if (spontaneity < -1f) spontaneity = -1f;
        }
    }
    public float Closer
    {
        get
        {
            return closer;
        }
        set
        {
            closer = value;
            if (closer > 1f) closer = 1f;
            if (closer < -1f) closer = -1f;
        }
    }
    public float Caution
    {
        get
        {
            return caution;
        }
        set
        {
            caution = value;
            if (caution > 1f) caution = 1f;
            if (caution < -1f) caution = -1f;
        }
    }
    public float Bounciness
    {
        get
        {
            return bounciness;
        }
        set
        {
            bounciness = value;
            if (bounciness > 1f) bounciness = 1f;
            if (bounciness < -1f) bounciness = -1f;
        }
    }
    public float Energy
    {
        get
        {
            return energy;
        }
        set
        {
            energy = value;
            if (energy > 1f) energy = 1f;
            if (energy < -1f) energy = -1f;
        }
    }

    struct WorldState
    {
        public Vector2 distanceFromEnemy;
        public float stamina;
        public bool attacking;

        public const float MELEE_RANGE = 2f;
        public const float FAR_RANGE = 10f;

        public bool MeleeRange { get { return distanceFromEnemy.magnitude <= MELEE_RANGE; } }
        public bool MidRange { get { return distanceFromEnemy.magnitude > MELEE_RANGE && distanceFromEnemy.magnitude < FAR_RANGE; } }
        public bool FarRange { get { return distanceFromEnemy.magnitude >= FAR_RANGE; } }
        public bool Tired { get { return stamina < 1; } }
        public bool Above { get { return distanceFromEnemy.y > .1f; } }
        public Direction DirectionOfOpponent { get { return distanceFromEnemy.x > 0 ? Direction.Right : Direction.Left; } }
        public Direction DirectionAwayFromOpponent { get { return distanceFromEnemy.x > 0 ? Direction.Left : Direction.Right; } }


        public WorldState(Vector2 distanceFromEnemy, bool attacking, float stamina = 1f)
        {
            this.distanceFromEnemy = distanceFromEnemy;
            this.attacking = attacking;
            this.stamina = stamina;
        }

        public static bool operator ==(WorldState ws1, WorldState ws2)
        {
            return ws1.MeleeRange == ws2.MeleeRange && ws1.MidRange == ws2.MidRange && ws1.FarRange == ws2.FarRange && ws1.attacking == ws2.attacking;
        }
        public static bool operator !=(WorldState ws1, WorldState ws2)
        {
            return !(ws1 == ws2);
        }
    }
}
