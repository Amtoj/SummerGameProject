using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class creditsManager_Script : MonoBehaviour
{
    [SerializeField]
    private Object mainMenuScene;
    InputAction skipCredits;
    [SerializeField]
    PlayerInput playerInput;

    public bool skipEnable;
    private void Awake()
    {
        IEnumerator EnableSkipTimer()
        {
            skipEnable = false;
            yield return new WaitForSeconds(5);
            skipEnable = true;
        }
        StartCoroutine(EnableSkipTimer());
        skipCredits = playerInput.actions["Pause"];
    }
    private void OnEnable()
    {
        skipCredits.performed += ctx => load_MainMenu();
    }
    void load_MainMenu ()
    {
        Debug.Log(skipEnable);
        if (skipEnable)
        {
            Debug.Log(skipEnable);
            SceneManager.LoadScene(mainMenuScene.name, LoadSceneMode.Single);
        }
    }

    void enableSkip()
    {
        Debug.Log(skipEnable);
        skipEnable = true;
        Debug.Log(skipEnable);
    }
    void disableSkip()
    {
        Debug.Log(skipEnable);
        skipEnable = false;
        Debug.Log(skipEnable);
    }
}
