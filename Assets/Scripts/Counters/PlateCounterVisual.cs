using System;
using System.Collections;
using System.Collections.Generic;
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
    private List<Transform> plateVisualPrefabList;

    private void Awake()
    {
        plateVisualPrefabList = new();
    }

    private void Start()
    {
        plateCounter.OnPlateAdded += PlateCounter_OnPlateAdded;
        plateCounter.OnPlateRemoved += PlateCounter_OnPlateRemoved;
    }

    private void PlateCounter_OnPlateRemoved(object sender, EventArgs e)
    {
        int i = plateVisualPrefabList.Count - 1;
        Destroy(plateVisualPrefabList[i].gameObject);
        plateVisualPrefabList.RemoveAt(i);
    }

    private void PlateCounter_OnPlateAdded(object sender, EventArgs e)
    {
        Transform plateVisual = Instantiate(plateVisualPrefab, counterTopPoint);
        plateVisualPrefabList.Add(plateVisual);
        int i = plateVisualPrefabList.Count - 1;
        plateVisual.transform.localPosition = new(0, plateOffsetY * i, 0);
    }
}
