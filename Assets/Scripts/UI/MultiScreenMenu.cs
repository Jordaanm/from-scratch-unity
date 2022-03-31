using System.Collections;
using System.Collections.Generic;
using AssetReferences;
using UI;
using UnityEngine;
using UnityEngine.UIElements;
public class MultiScreenMenu : FullscreenMenuHost.FullScreenMenu
{
    public interface Submenu
    {
        string GetTitle();
        string GetID();
        VisualElement GetRoot();
        void OnOpen();
        void OnClose();
        void SetIsActive(bool state);
    }

    protected List<Submenu> m_submenus = new List<Submenu>();
    protected Submenu m_activeSubmenu = null;
    protected Submenu m_previousSubmenu = null;
    protected int m_currentSubmenuIndex = 0;

    #region UI References
    protected static VisualTreeAsset treeAsset;
    protected const string treeAssetKey = "multiscreen";
    protected VisualElement m_veRoot;
    protected Label m_lblMainTitle;
    protected Label m_lblPrevTitle;
    protected Label m_lblNextTitle;
    protected VisualElement m_veMain;
    #endregion

    public Submenu CurrentSubmenu {
        get {
            return GetSubmenuAtIndex(m_currentSubmenuIndex);
        }
    }

    public MultiScreenMenu AddSubmenu(Submenu menu) {
        m_submenus.Add(menu);
        //TODO: Update UI
        return this;
    }

    public MultiScreenMenu RemoveMenu(Submenu menu) {
        m_submenus.Remove(menu);
        //TODO: Update UI;
        return this;
    }

    public virtual void OpenMenu() {
        m_currentSubmenuIndex = 0;
        SetActiveSubmenu(CurrentSubmenu);
    }

    public virtual void CloseMenu() {

    }

    public virtual void NextMenu(int offset = 1) {
        m_currentSubmenuIndex = GetRelativeIndex(offset);
        SetActiveSubmenu(GetSubmenuAtIndex(m_currentSubmenuIndex));
    }

    public void OpenTo(string id) {
        int index = m_submenus.FindIndex(x => x.GetID() == id);
        if(index != -1) {
            m_currentSubmenuIndex = index;
            SetActiveSubmenu(CurrentSubmenu);
        }
    }

    #region FullScreenMenu contract

    public VisualElement GetRoot() {
        if (m_veRoot == null) {
            BuildUI();
        }

        return m_veRoot;
    }


    #endregion

    virtual protected void BuildUI() {
        if(treeAsset == null) {
            treeAsset = VisualTreeAssetReference.Instance.GetAsset(treeAssetKey);
        }

        m_veRoot = treeAsset.CloneTree().Q(null, "root");
        m_lblMainTitle = m_veRoot.Q<Label>("title__main");
        m_lblPrevTitle = m_veRoot.Q<Label>("title__prev");
        m_lblNextTitle = m_veRoot.Q<Label>("title__next");
        m_veMain = m_veRoot.Q("main");
    }

    private void SetActiveSubmenu(Submenu submenu) {
        if(m_activeSubmenu != null) {
            //DEACTIVATE
            m_activeSubmenu.OnClose();
            m_activeSubmenu.SetIsActive(false);
            m_activeSubmenu.GetRoot().RemoveFromHierarchy();
        }

        m_previousSubmenu = m_activeSubmenu;
        m_activeSubmenu = submenu;
        m_activeSubmenu.SetIsActive(true);
        m_veMain.Add(m_activeSubmenu.GetRoot());
        m_activeSubmenu.OnOpen();
        UpdateTitle();
    }

    private void UpdateTitle() {
        string title = CurrentSubmenu?.GetTitle();
        string prevTitle = GetSubmenuAtIndex(m_currentSubmenuIndex - 1)?.GetTitle();
        string nextTitle = GetSubmenuAtIndex(m_currentSubmenuIndex + 1)?.GetTitle();

        m_lblMainTitle.text = title;
        m_lblNextTitle.text = nextTitle;
        m_lblPrevTitle.text = prevTitle;
    }

    private Submenu GetSubmenuAtIndex(int index) {
        //Normalise index
        index = GetRelativeIndex(index, 0);

        if (m_submenus == null || index >= m_submenus.Count) {
            return null;
        } else {
            return m_submenus[index];
        }
    }

    private int GetRelativeIndex(int offset) {
        return GetRelativeIndex(m_currentSubmenuIndex, offset);
    }

    private int GetRelativeIndex(int current, int offset) {
        //Edge case, avoid divide by zero
        int count = m_submenus.Count;
        if (count == 0) { return 0; }
        int basic = current + offset;

        while (basic < 0) {
            basic += count;
        }

        return basic % count;
    }
}
