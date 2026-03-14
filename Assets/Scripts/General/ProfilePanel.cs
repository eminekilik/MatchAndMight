using UnityEngine;

public class ProfilePanel : MonoBehaviour
{
    public GameObject profilePanel;

    void Start()
    {
        profilePanel.SetActive(false);
    }

    public void OpenPanel()
    {
        profilePanel.SetActive(true);
    }

    public void ClosePanel()
    {
        profilePanel.SetActive(false);
    }
}
