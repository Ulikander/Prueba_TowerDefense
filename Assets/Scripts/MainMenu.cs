using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField inputWaves, inputEnemy, inputAlly, inputCurrency;

    private void Update()
    {
        if(inputWaves.text != "")
        {
            int _waves = int.Parse(inputWaves.text);
            if (_waves > 5) inputWaves.text = "5";
            if (_waves <= 0) inputWaves.text = "";
        }

        if(inputEnemy.text != "")
        {
            float _enemy = float.Parse(inputEnemy.text);
            if (_enemy < 0.24f) inputEnemy.text = "";
        }

        if (inputAlly.text != "")
        {
            float _ally = float.Parse(inputAlly.text);
            if (_ally < 0.24f) inputAlly.text = "";
        }
    }

    public void Button_StartTheGame()
    {
        int _waves = inputWaves.text == "" ? 2 : int.Parse(inputWaves.text);
        float _enemy = inputEnemy.text == "" ? 0.85f : float.Parse(inputEnemy.text);
        float _ally = inputAlly.text == "" ? 1f : float.Parse(inputAlly.text);
        int _currency = inputCurrency.text == "" ? 0 : int.Parse(inputCurrency.text);

        General.LoadSceneAsync(SceneID.GamePlay, delegate
        {
            GamePlay.instance.WaveLimit = _waves;
            GamePlay.instance.DifficultyScalar = _enemy;
            GamePlay.instance.PlayerUnitsScalar = _ally;
            GamePlay.instance.Currency = _currency;
        });
    }
}
