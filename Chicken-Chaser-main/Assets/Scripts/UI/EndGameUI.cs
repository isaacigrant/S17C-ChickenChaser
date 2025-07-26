using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private Transform endGameCam;
    [SerializeField] private Transform endCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endGameCam.gameObject.SetActive(false);
        endCanvas.gameObject.SetActive(false);
        //SettingsManager.SaveFile.onLookSenseChanged += OnLookSenseChanged;
        PlayerChicken.OnPlayerEscaped += OnEndGame;
    }

    public void OnEndGame(Vector3 val) 
    {
        
        endGameCam.gameObject.SetActive(true);
        endCanvas.gameObject.SetActive(true);
    }

}
