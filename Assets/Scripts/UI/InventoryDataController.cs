using UnityEngine;

public class InventoryDataController : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Paneles de datos")]
    [SerializeField] private GameObject jaguarDataPanel;
    [SerializeField] private GameObject anacondaDataPanel;
    [SerializeField] private GameObject ranaDataPanel;
    [SerializeField] private GameObject delfinDataPanel;
    [SerializeField] private GameObject cordoncilloDataPanel;

    private void Start()
    {
        CloseAllDataPanels();
    }

    public void OpenJaguarData()
    {
        OpenDataPanel(jaguarDataPanel);
    }

    public void OpenAnacondaData()
    {
        OpenDataPanel(anacondaDataPanel);
    }

    public void OpenRanaData()
    {
        OpenDataPanel(ranaDataPanel);
    }

    public void OpenDelfinData()
    {
        OpenDataPanel(delfinDataPanel);
    }

    public void OpenCordoncilloData()
    {
        OpenDataPanel(cordoncilloDataPanel);
    }

    public void CloseAllDataPanels()
    {
        if (jaguarDataPanel != null) jaguarDataPanel.SetActive(false);
        if (anacondaDataPanel != null) anacondaDataPanel.SetActive(false);
        if (ranaDataPanel != null) ranaDataPanel.SetActive(false);
        if (delfinDataPanel != null) delfinDataPanel.SetActive(false);
        if (cordoncilloDataPanel != null) cordoncilloDataPanel.SetActive(false);
    }

    public void BackToInventory()
    {
        CloseAllDataPanels();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }
    }

    private void OpenDataPanel(GameObject panelToOpen)
    {
        CloseAllDataPanels();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (panelToOpen != null)
        {
            panelToOpen.SetActive(true);
        }
    }
}