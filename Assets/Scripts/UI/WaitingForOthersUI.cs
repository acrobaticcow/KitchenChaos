using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOthersUI : MonoBehaviour
{
    private void Start()
    {
        Hide();
        GameManager.Instance.OnLocalPlayerReadyChanged +=
            WaitingForOthersUI_OnLocalPlayerReadyChanged;
    }

    private void WaitingForOthersUI_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.GetIsLocalPlayerReady())
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
