using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class UnitBehaviour : MonoBehaviour
{
    [Header("Editor")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected Collider2D col;
    [SerializeField] SpriteRenderer spriteUnit, spriteSideDefiner, spriteHpBar;

    [Header("Stats")]
    public int Value;
    [SerializeField] protected int hpBase;
    public int HP_Base { get => hpBase; }
    [SerializeField] protected int powerBase;
    [SerializeField] protected float movSpeedBase;
    [SerializeField] protected float atkSpeedBase;
    public int HP;
    public int Power;
    public float MovSpeed;
    public float AtkSpeed;
    public float range;

    //should've been implemented in their own abstract class hehe
    [Header("Projectiles")]
    [SerializeField] GameObject[] projectiles;
    [SerializeField] protected int[] projectilesToSpawn;
    protected Dictionary<int,List<Projectile>> spawnedProjectiles = new Dictionary<int, List<Projectile>>();

    [Header("Interaction")]
    public bool IsHostile;

    /// <summary>
    /// The actual definition of the unit (if it is an archer/warrior/tower etc.)
    /// </summary>
    public UnitDefinition Unit;

    /// <summary>
    /// The type of the unit, as of a category to know what unit is this (single, tower, special) to calculate priority.
    /// </summary>
    public UnitType Type;

    /// <summary>
    /// Deactivated = The unit cannot be used.<br/><br/>
    /// Waiting = Default State, the behaviour here should tell the unit what to do next<br/><br/>
    /// MovingToPoint = The unit is moving and will not stop until it reaches its destination or die.<br/><br/>
    /// Attacking = The unit attacks, the behaviour should determine wheter it focuses the same target or changes depending on priority.<br/><br/>
    /// Dead = The unit is dead but still hasn't  became deactivated.
    /// </summary>
    public UnitState State;
    public int priority;
    [SerializeField] Vector3 pathVarianceMax;
    [SerializeField] protected float deathTime;
   

    private void Start()
    {

    }

    protected virtual void LateUpdate()
    {
        HpBarHandle();
        BehaviourHandle();
    }

    void HpBarHandle()
    {
        if(spriteHpBar == null) return;
        spriteHpBar.transform.parent.gameObject.SetActive(HP > 0);
        var _scale = spriteHpBar.transform.localScale;

        float _trueHpBase = hpBase * (IsHostile ? GamePlay.instance.DifficultyScalar : GamePlay.instance.PlayerUnitsScalar);
        _scale.x = HP / _trueHpBase;
        spriteHpBar.transform.localScale = _scale;
    }

    protected virtual void AnimationHandle()
    {
        spriteUnit.flipX = pathInvertDirection;
        if (anim == null) return;
        

        switch (State)
        {
            case UnitState.Waiting:
                anim.Play("Stand");
                break;
            case UnitState.MovingToPoint:
                anim.Play("Walk");
                break;
            case UnitState.ReachedEndPoint:
                anim.Play("Stand");
                break;
            case UnitState.Attacking:
                anim.Play("Attack");
                break;
            case UnitState.Dead:
                break;
            case UnitState.Deactivated:
                break;
        }
    }

    /// <summary>
    /// The function that handles what the unit does. Can be overriden.
    /// </summary>
    protected virtual void BehaviourHandle()
    {

        if (State != UnitState.Deactivated && HP <= 0) State = UnitState.Dead;
        switch (State)
        {
            case UnitState.Waiting:
                
                foreach(var _col in Physics2D.OverlapCircleAll(transform.localPosition, range))
                {
                    //print("_col: " + _col.name);
                    if (_col.tag != (IsHostile ? "Ally" : "Enemy")) continue;
                    attackTarget = _col.GetComponent<UnitBehaviour>();
                    State = UnitState.Attacking;
                    return;
                }

                attackTarget = null;
                State = UnitState.MovingToPoint;

                break;
            case UnitState.MovingToPoint:

                foreach (var _col in Physics2D.OverlapCircleAll(transform.localPosition, range))
                {
                    //print("_col: " + _col.name);
                    if (_col.tag != (IsHostile ? "Ally" : "Enemy")) continue;
                    State = UnitState.Waiting;
                    return;
                }

                MoveTowardsNextPoint();
                break;
            case UnitState.Attacking:
                Attack();
                break;
            case UnitState.Dead:
                Die();
                break;
            case UnitState.Deactivated:
            default:
                
                return;
        }

        AnimationHandle();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    #region Movement
    Vector3 pathVariance;
    [SerializeField] public int pathTargetId;
    [SerializeField] bool pathInvertDirection;

    /// <summary>
    /// Uses the PathPoints class to determine where to go next, has little variance in speeds and on the position they go when pathing. When the destination is reached, if hostile, it will attack the castle, if not, inverts its path.
    /// </summary>
    protected void MoveTowardsNextPoint()
    {
        int _trueTargetID = !pathInvertDirection ? pathTargetId : PathPoints.Points.Length - 1 - pathTargetId;
        if (Vector2.Distance(transform.position, PathPoints.Points[_trueTargetID].position + pathVariance) < 0.01f)
        {
            pathVariance = new Vector3(Random.Range(-pathVarianceMax.x, pathVarianceMax.x), Random.Range(-pathVarianceMax.y, pathVarianceMax.y), 0);
            pathTargetId++;
            if (pathTargetId == PathPoints.Points.Length)
            {
                pathTargetId = 0;
                if (!IsHostile) pathInvertDirection = !pathInvertDirection;
                else
                {
                    print("lel");
                    attackTarget = GamePlay.Castle;
                    State = UnitState.Attacking;
                    return;
                }

            }

            State = UnitState.Waiting;
            return;
        } 

        transform.position = Vector2.MoveTowards(transform.position, PathPoints.Points[_trueTargetID].position + pathVariance, MovSpeed * (Random.Range(0.8f, 1.1f)) * Time.deltaTime);
    }
    #endregion

    #region Projectiles Spawn
    public void SpawnProjectiles()
    {
        int _id = 0;
        foreach (var proj in projectiles)
        {
            spawnedProjectiles.Add(_id, new List<Projectile>());

            foreach (var q in projectilesToSpawn)
            {
                for(int i = 0; i < q; i++)
                {
                    var projSpwn = Instantiate(proj, transform);
                    projSpwn.gameObject.SetActive(false);
                    spawnedProjectiles[_id].Add(projSpwn.GetComponent<Projectile>());
                    
                }
            }
            _id++;
        }
    }

    #endregion

    #region Attack

    protected UnitBehaviour attackTarget;
    protected float attackDelayCont = 0;

    protected bool IsTargetCollidersNear
    {
        get
        {
            if (attackTarget == null) return false;
            float _dist = Physics2D.Distance(col, attackTarget.col).distance;
            //Debug.LogWarning("distance: " + _dist + "| range: " + range);
            return _dist <= range;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Attack()
    {
        if (!IsTargetCollidersNear)
        {
            State = UnitState.Waiting;
            return;
        }

        if (attackDelayCont > 0)
        {
            if (attackTarget.State == UnitState.Dead)
            {
                State = UnitState.Waiting;
                return;
            }

            attackDelayCont -= Time.deltaTime;
        }

        attackDelayCont = AtkSpeed;
        attackTarget.HP -= Power;
        if (attackTarget.State == UnitState.Dead || attackTarget.State == UnitState.Deactivated)
        {
            State = UnitState.Waiting;
            return;
        }
    }

    #endregion

    #region Dead
    protected float dieCont;

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Die()
    {
        if (dieCont == deathTime)
        {
            if(IsHostile) GamePlay.instance.Currency += Value;
            anim.Play("Die");
        }
        dieCont -= Time.deltaTime;
        if (dieCont <= 0) Deactivate();
    }

    #endregion

    #region Activation Handle

    Color colorRed = new Color(1, 0, 0, .32f);
    Color colorGreen = new Color(0, 1, 0, .32f);

    /// <summary>
    /// Makes the unit visible and gives it's stats values. It is positioned to target ID 0.<br/>You can omit the positioning by typing -1 as target ID.
    /// </summary>
    /// <param name="_targetId">The path ID where this unit will spawn.<br/>You can omit the positioning by typing -1 as target ID.</param>
    /// <param name="_ignoreInvert">If true, even if not hostile, it will spawn on a NOT overriden path ID</param>
    public void Activate(int _targetId = 0)
    {
        if (spriteSideDefiner != null) spriteSideDefiner.color = IsHostile ? colorRed : colorGreen;
        tag = IsHostile ? "Enemy" : "Ally";


        if (IsHostile)
        {
            HP = (int)(hpBase * GamePlay.instance.DifficultyScalar);
            Power = (int)(powerBase * GamePlay.instance.DifficultyScalar);
            MovSpeed = movSpeedBase * GamePlay.instance.DifficultyScalar;
            AtkSpeed = atkSpeedBase / GamePlay.instance.DifficultyScalar;
        }
        else
        {
            HP = (int)(hpBase * GamePlay.instance.PlayerUnitsScalar);
            Power = (int)(powerBase * GamePlay.instance.PlayerUnitsScalar);
            MovSpeed = movSpeedBase * GamePlay.instance.PlayerUnitsScalar;
            AtkSpeed = atkSpeedBase / GamePlay.instance.PlayerUnitsScalar;
        }
      
        if(_targetId >= 0)
        {
            pathTargetId = _targetId;
            pathInvertDirection = !IsHostile;
            transform.position = PathPoints.Points[pathTargetId].position;
            if (pathInvertDirection) pathTargetId = PathPoints.Points.Length - 1 - pathTargetId;

            //Debug.LogWarning($"Original: {pathTargetId} | True?: {_trueTargetID}");
        }

        
        State = UnitState.Waiting;

        dieCont = deathTime;
        attackDelayCont = 0;

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Deactivate()
    {
    
        if (IsHostile)
        {
            if (GamePlay.instance.AvailableEnemyPools.TryGetValue(Unit, out UnitPool _enemyPool))
            {
                _enemyPool.Enqueue(this);
                Debug.LogWarning($"Pool {_enemyPool.name} for {_enemyPool.transform.parent.name} / recieved {Unit} which was Hostile = {IsHostile}");
            }
        }
        else if (GamePlay.instance.AvailableAllyPools.TryGetValue(Unit, out UnitPool _allyPool))
        {
            _allyPool.Enqueue(this);
            Debug.LogWarning($"Pool {_allyPool.name} for {_allyPool.transform.parent.name} / recieved {Unit} which was Hostile = {IsHostile}");

        }

        State = UnitState.Deactivated;
        gameObject.SetActive(false);
    }
    #endregion
}
