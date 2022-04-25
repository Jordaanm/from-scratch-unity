using System.Collections;
using System.Collections.Generic;
using SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private VisualElement root;
    public SaveDataCarrier saveDataCarrier;
    
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        BindButtons();
    }

    private void BindButtons()
    {
        var newGame = root.Q<Button>("btn-new");
        newGame.RegisterCallback<ClickEvent>(NewGameClick);
        
        var quitGame = root.Q<Button>("btn-quit");
        quitGame.RegisterCallback<ClickEvent>(QuitGame);
    }

    private void QuitGame(ClickEvent evt)
    {
        Application.Quit();
    }

    private void NewGameClick(ClickEvent evt)
    {
        //Todo: Set Preset New Game Save Data
        
        
        DontDestroyOnLoad(saveDataCarrier.gameObject);
        saveDataCarrier.loadGameOnSceneLoad = true;
        SceneManager.LoadScene(1);
    }
    
}
