using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField]
    private Transform sizzlingParticles;

    [SerializeField]
    private Transform stoveOnVisual;

    [SerializeField]
    private StoveCounter stoveCounter;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool showVisual =
            e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;

        sizzlingParticles.gameObject.SetActive(showVisual);
        stoveOnVisual.gameObject.SetActive(showVisual);
    }
}
