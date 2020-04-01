using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Panel currentPanel = null;
    public Panel afterConfig = null;

    private List<Panel> panelHistory = new List<Panel>();

    // Start is called before the first frame update
    private void Start()
    {
        SetupPanels();
    }

    private void SetupPanels()
    {
        Panel[] panels = GetComponentsInChildren<Panel>();

        foreach(Panel panel in panels)
        {
            panel.Setup(this);
        }

        currentPanel.Show();
    }

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))//Button primaryhandtrigger
        {
            GoToPrevious();
        }
    }

    public void GoToPrevious()
    {
        if(panelHistory.Count == 0)
        {
            OVRManager.PlatformUIConfirmQuit();
            return;
        }

        int lastIndex = panelHistory.Count - 1;
        SetCurrent(panelHistory[lastIndex]);
        panelHistory.RemoveAt(lastIndex);
    }

    public void SetCurrentWithHistory(Panel newPanel)
    {
        if(currentPanel.gameObject.name !="Panel_config")
        {
            panelHistory.Add(currentPanel);
            SetCurrent(newPanel);
        }
    }

    public void AfterConfig()
    {
        Debug.Log("in after config");
        SetCurrent(afterConfig);
    }

    private void SetCurrent(Panel newPanel)
    {
        currentPanel.Hide();

        currentPanel = newPanel;
        currentPanel.Show();
    }
}
