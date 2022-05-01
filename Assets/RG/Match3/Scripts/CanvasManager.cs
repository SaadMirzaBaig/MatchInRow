using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;


    private void Awake() {

        restartButton.onClick.AddListener(RestartGame);
    }



    private void RestartGame() {

        BroadcastManager.ClearGrid?.Invoke();
        BroadcastManager.InitializeGame?.Invoke();
    }
}
