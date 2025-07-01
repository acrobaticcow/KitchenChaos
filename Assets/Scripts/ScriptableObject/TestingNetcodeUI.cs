using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
	[SerializeField]
	private Button startHostButton;

	[SerializeField]
	private Button startClientButton;

	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update() { }

	private void Awake()
	{
		startHostButton.onClick.AddListener(() =>
		{
			Debug.Log("Host");
			NetworkManager.Singleton.StartHost();
			GameManager.Instance.SetIsLocalPlayerReady(true);
			Hide();
		});
		startClientButton.onClick.AddListener(() =>
		{
			Debug.Log("Client");
			NetworkManager.Singleton.StartClient();
			GameManager.Instance.SetIsLocalPlayerReady(true);
			Hide();
		});
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
