using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Image gamePlayingTimerImage;

    private void Update()
    {
        gamePlayingTimerImage.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
