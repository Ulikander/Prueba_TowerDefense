using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Advice : MonoBehaviour
{

    public static Advice instance;

    [SerializeField] GameObject prefabOnWorldText;
    [SerializeField] int onWorldTextCount;
    [SerializeField] float textSpeed, textDuration;
    [SerializeField] RectTransform transformCanvas;
    [SerializeField] TextMeshPro[] textOnWorld;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        textOnWorld = new TextMeshPro[onWorldTextCount];
        textOnWorldTimer = new float[onWorldTextCount];
        for (int i = 0; i < onWorldTextCount; i++)
        {
            textOnWorld[i] = Instantiate(prefabOnWorldText, transform).GetComponent<TextMeshPro>();
            textOnWorld[i].gameObject.SetActive(false);
        }
    }

    int _textId = 0;
    float[] textOnWorldTimer;
    void Update()
    {
        for(int i = 0; i < textOnWorld.Length; i++)
        {
            var _txt = textOnWorld[i];

            if (!_txt.gameObject.activeInHierarchy) continue;

            Vector3 _pos = _txt.transform.position;
            _pos.y += textSpeed * Time.unscaledTime;
            _txt.transform.position = _pos;

            textOnWorldTimer[i] += Time.deltaTime;

            if (textOnWorldTimer[i] >= textDuration)
            {
                textOnWorldTimer[i] = 0;
                _txt.gameObject.SetActive(false);
            }
        }
    }

    public static void ShowTextInWorld(Vector3 _position, string _msg)
    {
        print("advice called");
        instance.textOnWorld[instance._textId].text = _msg;
        instance.textOnWorld[instance._textId].transform.position = _position;
        instance.textOnWorld[instance._textId].gameObject.SetActive(true);
        instance._textId++;
        if (instance._textId >= instance.textOnWorld.Length) instance._textId = 0;
    }

}
