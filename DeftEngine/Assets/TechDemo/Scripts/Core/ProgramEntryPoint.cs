using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deft;
using Deft.UI;

public class ProgramEntryPoint : MonoBehaviour
{
    public Canvas CanvasPrefab;
    public List<UIPanel> PanelPrefabs;

    private GameAdmin gameAdmin;

    void Awake()
    {
        gameAdmin = new GameObject("GameAdmin").AddComponent<GameAdmin>();
        var uiView = gameAdmin.AddManager<UIView>("UIView");

        foreach (UIPanel panel in PanelPrefabs)
            uiView.AddPanel(CanvasPrefab, panel, panel.GetType().Name);
    }

    void Start()
    {
        gameAdmin.GetManager<UIView>().GetPanel<MainMenu>().IsVisible = true;

        Destroy(gameObject);
    }
}
