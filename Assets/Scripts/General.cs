using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class General : MonoBehaviour
{
    public static General instance;

    [Header("Scene Loader")]
    [SerializeField] float sceneLoad_alphaSpeed;
    /// <summary>
    /// 0 = Entire Object<br/>1 = Loading Anim
    /// </summary>
    [SerializeField] GameObject[] sceneLoad_Objs;
    [SerializeField] CanvasGroup sceneLoad_CanvasGroup;

    [Header("Sprites")]
    public Sprite sprBlank;
    public static Sprite SprBlank { get => instance.sprBlank; }

    [Header("Debug")]
    [SerializeField] GameObject debug_Obj;
    [SerializeField] CanvasGroup debug_availableOnGameplay;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        sceneLoad_Objs[0].SetActive(false);
        sceneLoad_CanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        DebugFunctions();
    }

    #region Async Load Scene

    //UnityTask sceneLoad_Task;
    bool sceneLoad_IsTaskActive { get => false; }//sceneLoad_Task != null && sceneLoad_Task.Running; }
    public static Scene CurrentScene { get => SceneManager.GetActiveScene(); }


    public static void GoToSceneAsync_MainMenu() => LoadSceneAsync(SceneID.MainMenu);
    public static void GoToSceneAsync_GamePlay() => LoadSceneAsync(SceneID.GamePlay);

    /// <summary>
    /// The scene loader, it does it asynchronously and creates a Fade-Out/Fade-In effect.<br/>You can use the "SceneID" class to get an ID.
    /// </summary>
    /// <param name="_sceneID">The name of the scene.<br/>You can use the SceneID class to get this.</param>
    public static void LoadSceneAsync(string _sceneID, Action _onLoad = null)
    {
        /**
        if(CurrentScene.name == _sceneID)
        {
            Debug.LogError("You should not try to load the same scene you are at.");
            return;
        }
        */
        /*
        SceneManager.LoadScene(_sceneID);
        return;
        */

        if (instance.sceneLoad_IsTaskActive)
        {
            Debug.LogError("You should not try to load more than one async scene at a time");
            return;
        }

        //sceneLoad_Task = new UnityTask(LoadSceneAsync_Enumerator(_sceneID, _onLoad));
        instance.StartCoroutine(instance.LoadSceneAsync_Enumerator(_sceneID, _onLoad));
    }

    /// Enumerator which executes the async scene load. It also handles the Fade in/out effect.
    IEnumerator LoadSceneAsync_Enumerator(string _sceneID, Action _onLoad)
    {
        var asyncScene = SceneManager.LoadSceneAsync(_sceneID);
        asyncScene.allowSceneActivation = false;

        /**
        AsyncError:
        yield return null;
        try
        {
            asyncScene.allowSceneActivation = false;
        }
        catch
        {
            goto AsyncError;
        }
        */

        sceneLoad_CanvasGroup.alpha = 0f;
        sceneLoad_Objs[0].SetActive(true);
        sceneLoad_Objs[1].SetActive(true);

        Debug.Log("before alpha to 1");
        while (sceneLoad_CanvasGroup.alpha != 1f)
        {
            yield return null;
            sceneLoad_CanvasGroup.alpha += sceneLoad_alphaSpeed * Time.unscaledDeltaTime;
            if (sceneLoad_CanvasGroup.alpha > 1f) sceneLoad_CanvasGroup.alpha = 1f;
            Debug.Log("alpha to 1");
        }

        Debug.Log("before progress < .9");
        while (asyncScene.progress < 0.9f)
        //while(CurrentScene.name != _sceneID)
        {
            yield return null;
            Debug.Log("progress < .9");
        }

        asyncScene.allowSceneActivation = true;

        /*
        Debug.Log("before ! isDone");
        while (!asyncScene.isDone)
        {
            yield return null;
            Debug.Log("! isDone");
        }
        */
        

        Debug.Log("before load obj false");
        sceneLoad_Objs[1].SetActive(false);

        Debug.Log("before onLoad null");
        if (_onLoad != null)
        {
            Retry:
            yield return null;
            try
            {
                _onLoad();
            }
            catch
            {
                goto Retry;
            }
        }

        Debug.Log("before alpha to 0");
        while (sceneLoad_CanvasGroup.alpha > 0f)
        {
            sceneLoad_CanvasGroup.alpha -= sceneLoad_alphaSpeed * Time.unscaledDeltaTime;
            if (sceneLoad_CanvasGroup.alpha < 0f) sceneLoad_CanvasGroup.alpha = 0f;
            Debug.Log("alpha to 0");
            yield return null;
        }

        sceneLoad_Objs[0].SetActive(false);
        //sceneLoad_Task = null;
        yield return null;
        Debug.Log("end");
    }


    #endregion

    #region Debug
    void DebugFunctions()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) debug_Obj.SetActive(!debug_Obj.activeInHierarchy);
        if (!debug_Obj.activeInHierarchy) return;

        debug_availableOnGameplay.interactable = CurrentScene.name == SceneID.GamePlay;

    }

    public void Debug_Button_GiveCurrency(int _q)
    {
        GamePlay.instance.Currency += _q;
    }

    public void Debug_Button_DeltatimeToggle()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    #endregion
}
