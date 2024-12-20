using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField]
    private GameObject hasProgressGameObject;
    private IHasProgress hasProgress;

    [SerializeField]
    private Image barImage;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null)
            Debug.LogError(
                hasProgressGameObject + "doesn't have componenet that implement IHasProgress"
            );

        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;

        barImage.fillAmount = 0f;

        Hide();
    }

    private void HasProgress_OnProgressChanged(
        object sender,
        IHasProgress.ProgressChangedEventArgs e
    )
    {
        barImage.fillAmount = e.ProgressNormalized;

        if (barImage.fillAmount == 0f || barImage.fillAmount == 1f)
            Hide();
        else
            Show();
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
