using System.Collections;
using System.Collections.Generic;
using FromScratch.Player;
using UI;
using UnityEngine;

public class PauseMenu : MultiScreenMenu
{
    private static bool IS_OPEN = false;
    private readonly FromScratchPlayer player;

    public PauseMenu(FromScratchPlayer player) : base()
    {
        this.player = player;
    }
    
    public bool IsOpen() {
        return IS_OPEN;
    }

    public override void OpenMenu() {
        IS_OPEN = true;
        base.OpenMenu();
        player.character.DisableControls();
        Time.timeScale = 0f;
    }

    public override void CloseMenu() {
        IS_OPEN = false;
        base.CloseMenu();
        player.character.EnableControls();
        Time.timeScale = 1f;
    }
}
