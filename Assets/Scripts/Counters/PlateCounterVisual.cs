using System;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField]
    private PlateCounter plateCounter;

    [SerializeField]
    private Transform plateVisualPrefab;

    [SerializeField]
    private Transform counterTopPoint;

    [SerializeField]
    private float plateOffsetY = .1f;

    private void Start()
    {
        plateCounter.OnPlateAmountChanged += PlateCounter_OnPlateAdded;
    }

    private void PlateCounter_OnPlateAdded(object sender, PlateCounter.OnPlateAddedEventArgs e)
    {
        ClearPlateVisuals();

        for (int i = 0; i < e.amount; i++)
        {
            Transform plateVisual = Instantiate(plateVisualPrefab, counterTopPoint);
            plateVisual.transform.localPosition = new(0, plateOffsetY * i, 0);
        }
    }

    private void ClearPlateVisuals()
    {
        foreach (Transform child in counterTopPoint)
        {
            Destroy(child.gameObject);
        }
    }
}
