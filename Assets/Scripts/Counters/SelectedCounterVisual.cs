using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField]
    private BaseCounter baseCounter;

    [SerializeField]
    private GameObject[] visualGameObjects;

    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(
        object sender,
        Player.OnSelectedCounterChangedEventArgs e
    )
    {
        if (e.selectedCounter == baseCounter)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        foreach (var obj in visualGameObjects)
        {
            obj.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (var obj in visualGameObjects)
        {
            obj.SetActive(false);
        }
    }
}
