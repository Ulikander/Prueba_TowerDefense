using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{

    public static GamePlay instance;

    public GamePlayUI GameplayUI;

    [Header("Wave Control")]
    [SerializeField] WaveInfo[] availableWaves;
    [SerializeField] int currentWaves;
    public int WaveLimit;
    public static int ActualWave { get => instance.currentWaves + 1; }
    UnityTask waveLoop;
    bool AreEnemyUnitsAlive
    {
        get
        {
            foreach(var _pool in AvailableEnemyPools.Values) if (_pool.AvailableUnits.Count != _pool.TotalUnits) return true;
            return false;
        }
    }

    [Header("Currency")]
    public int Currency;
    [SerializeField] int currencyPerTime;
    [SerializeField] float currenyTimeDelay;


    [Header("Unit Related")]
    [SerializeField] UnitBehaviour unitCastle;
    public static UnitBehaviour Castle { get => instance.unitCastle; }
    /// <summary>
    /// Transform that contains all the UnitPool codes. These will be used to generate the pool dictionary.
    /// </summary>
    [SerializeField] Transform poolsAlly, poolsEnemy;
    /// <summary>
    /// Influences the enemy units.
    /// </summary>
    public float DifficultyScalar;
    /// <summary>
    /// Influences the ally units.
    /// </summary>
    public float PlayerUnitsScalar;
    /// <summary>
    /// The currently selected unit ID the player has. (Unit Selector Button)
    /// </summary>
    public int UnitSelected;


    public Dictionary<UnitDefinition, UnitPool> AvailableAllyPools;
    public Dictionary<UnitDefinition, UnitPool> AvailableEnemyPools;




    private void Awake()
    {
        instance = this;
    }

   /// <summary>
   /// Create the available ally/enemy pools. (Instantiated here)
   /// </summary>
    void Start()
    {
        InitializePools(poolsAlly, ref AvailableAllyPools, false);
        InitializePools(poolsEnemy, ref AvailableEnemyPools, true);

        waveLoop = new UnityTask(WaveLoop());
    }

    void InitializePools(Transform _origin, ref Dictionary<UnitDefinition, UnitPool> _dict, bool _isHostile)
    {
        _dict = new Dictionary<UnitDefinition, UnitPool>();
        for(int i = 0; i < _origin.childCount; i++)
        {
            var _pool = _origin.GetChild(i).GetComponent<UnitPool>();
            _dict.Add(_pool.Unit, _pool);
            _pool.InitializePool(_isHostile);
            
        }
    }

    
    void Update()
    {
        UnitSelectionValidator();
        CurrencyCounter();
    }

    #region Currency
    public static bool PlayerHasEnoughCurrency(int _cost) => instance.Currency >= _cost;
    string InfoPlaceableAt(UnitPlacement _placement)
    {
        switch (_placement)
        {
            case UnitPlacement.Spawnable:
                return "Spawn at Castle";
            case UnitPlacement.BuildPlace:
                return "Building Place";
            case UnitPlacement.Path:
                return "Main Path";
            case UnitPlacement.BuildPlaceAndPath:
                return "Build Area / Path";
            default:
                return "null";
        }
    }

    float _currencyCounter = 0;
    void CurrencyCounter()
    {
        if(_currencyCounter <= 0)
        {
            _currencyCounter = currenyTimeDelay;
            Currency += currencyPerTime;
            return;
        }
        _currencyCounter -= Time.deltaTime;
    }
    #endregion

    void UnitSelectionValidator()
    {
        if (Input.GetMouseButtonDown(1)) UnitSelected = -1;

        if (UnitSelected < 0)
        {
            GameplayUI.ImgCurrentSelection.sprite = General.SprBlank;
            GameplayUI.TextCurrentSelection.text = "";
            return;
        }

        var _info = GameplayUI.UnitSelectorButtons[UnitSelected].UnitInfo;
        GameplayUI.ImgCurrentSelection.sprite = _info.SpritePreview;
        GameplayUI.TextCurrentSelection.text = $"Placeable At:\n<size=80%>{InfoPlaceableAt(_info.PlaceableAt)}</size>";//\nMore Info:\n<size=80%>{_info.Info}</size>";
    }

    IEnumerator WaveLoop()
    {
        yield return new WaitForSeconds(5);

        //List<UnitBehaviour> _spawnedEnemies = new List<UnitBehaviour>();

        if (availableWaves.Length == 0)
        {
            Debug.LogError("NO WAVES!?");
            yield break;
        }

        NextWave:
        Debug.LogWarning("next wave");
        if (currentWaves == availableWaves.Length || currentWaves == WaveLimit)
        {
            Win();
            yield break;
        }

        WaveInfo _wave = availableWaves[currentWaves];
        int _currentLoop = 0;

        while (true)
        {
            Debug.LogWarning("while");
            yield return null;
            //_spawnedEnemies.Clear();

            //spawn chances

            for(int i = 0; i < _wave.Units.Length; i++)
            {
                Debug.LogWarning("for");
                yield return null;
                var _unitDef = _wave.Units[i];

                if (!AvailableEnemyPools.ContainsKey(_unitDef))
                {
                    Debug.LogError($"Wave tried to spawn from unexistent enemy pool: {_unitDef}");
                    continue;
                }

                int _count = Random.Range(_wave.UnitSpawnCountRange[i].x, _wave.UnitSpawnCountRange[i].y + 1);
                //Debug.LogWarning("count:" + _count);

                while (_count != 0)
                {
                    Debug.LogWarning("do");
                    if (AvailableEnemyPools[_unitDef].AvailableUnits.Count == 0)
                    {
                        Debug.LogError("Wave ran out of units from pool");
                        _count = 0;
                        yield return null;
                        continue;
                    }

                    yield return new WaitForSeconds(Random.Range(_wave.UnitTimeBetween[i].x, _wave.UnitTimeBetween[i].y));
                    var _unit = AvailableEnemyPools[_unitDef].Dequeue();
                    //_spawnedEnemies.Add(_unit);
                    _count--;
                }

                float _forcedCounter = Random.Range(_wave.TimeDelayRange.x, _wave.TimeDelayRange.y);
                while(_forcedCounter > 0f && AreEnemyUnitsAlive)
                {
                    _forcedCounter -= Time.deltaTime;
                    yield return null;
                }
            }

            Debug.LogWarning("after for");
            _currentLoop++;

            while (AreEnemyUnitsAlive) yield return null;

            if (_currentLoop > _wave.RepeatTimes)
            {
                currentWaves++;
                goto NextWave;
            }
        }
    }

    public void Win()
    {
        //score
        Debug.LogWarning("You won");

    }
    public void Lose()
    {
        waveLoop.Stop();
        General.GoToSceneAsync_MainMenu();
    }


}
