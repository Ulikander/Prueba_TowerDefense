using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePlayUI : MonoBehaviour
{

    [Header("Player")]
    [SerializeField] Image imgCastleHP;
 
    [SerializeField] public Image ImgCurrentSelection;
    [SerializeField] public TextMeshProUGUI TextCurrentSelection;
    [SerializeField] Transform transUnitSelectorButtons;
    [HideInInspector] public UnitSelectorButton[] UnitSelectorButtons;

    [Header("Gameplay")]
    [SerializeField] TextMeshProUGUI textCurrency;
    [SerializeField] TextMeshProUGUI textWave;
    public RectTransform TransformUnitHpBars;
    [SerializeField] GameObject objResult;
    [SerializeField] TextMeshProUGUI textResult;

    /// <summary>
    /// Define the array of buttons for Selecting units.
    /// </summary>
    void Start()
    {
        UnitSelectorButtons = new UnitSelectorButton[transUnitSelectorButtons.childCount];

        for(int i = 0; i < transUnitSelectorButtons.childCount; i++)
        {
            UnitSelectorButtons[i] = transUnitSelectorButtons.GetChild(i).GetComponent<UnitSelectorButton>();
            UnitSelectorButtons[i].Index = i;
        }
    }

    /// <summary>
    /// Checks the castle hp and updates Unit Selector buttons / updates the selected unit UI/  Checks currency
    /// </summary>
    void Update()
    {
        textCurrency.text = $"$ {GamePlay.instance.Currency}";

       
        textWave.text = imgCastleHP.fillAmount <= 0 ? "Lost" : GamePlay.ActualWave == GamePlay.instance.WaveLimit ? "Won" : $"{GamePlay.ActualWave}";
        imgCastleHP.fillAmount = GamePlay.Castle.HP / (float)GamePlay.Castle.HP_Base;

        for( int i = 0; i < UnitSelectorButtons.Length; i++)
        {
            UnitPool _pool = null;
            bool _interactable = false;
            if(GamePlay.instance.AvailableAllyPools.TryGetValue(UnitSelectorButtons[i].UnitInfo.Unit, out _pool)) _interactable = _pool.AvailableUnits.Count > 0;

            int _unitsLeft = 0;
            if(_interactable && UnitSelectorButtons[i].UnitInfo.MaxUnits > 0)
            {
                int _unitsInField = _pool.TotalUnits - _pool.AvailableUnits.Count;
                print($"pool = {_pool.transform.parent.gameObject.name} - {_pool.Unit} |units in field = {_unitsInField}");
                _unitsLeft = UnitSelectorButtons[i].UnitInfo.MaxUnits - _unitsInField;
                if (_unitsLeft < 0) _unitsLeft = 0;
                if (_unitsLeft > UnitSelectorButtons[i].UnitInfo.MaxUnits) _unitsLeft = UnitSelectorButtons[i].UnitInfo.MaxUnits;
                _interactable = _unitsLeft > 0 && _pool.AvailableUnits.Count > 0;
            }

            UnitSelectorButtons[i].UpdateValues(GamePlay.instance.UnitSelected == i, _interactable, _unitsLeft);
        }

    }

    public void ShowResult(string _msg)
    {
        textResult.text = _msg;
        objResult.SetActive(true);
    }
}
