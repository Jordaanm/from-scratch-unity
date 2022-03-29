using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MultiScreenMenu
{
    private static bool IS_OPEN = false;

    public bool IsOpen() {
        return IS_OPEN;
    }

    public override void OpenMenu() {
        IS_OPEN = true;
        base.OpenMenu();
    }

    public override void CloseMenu() {
        IS_OPEN = false;
        base.CloseMenu();
    }
}
