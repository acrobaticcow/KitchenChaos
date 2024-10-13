using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI recipeDeliveredCountText;

    [SerializeField]
    private Button RetryButton;

    [SerializeField]
    private Button MainMenuButton;

    private void Awake()
    {
        RetryButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        MainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArg e)
    {
        Debug.Log("Event fire:" + e.state);
        if (e.state == GameManager.State.GameOver)
        {
            Debug.Log("Game is over");
            Show();
        }
        else
            Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        recipeDeliveredCountText.text = DeliveryManager
            .Instance.GetDeliveredRecipeCount()
            .ToString();
    }
}
