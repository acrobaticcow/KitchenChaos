using UnityEngine;

[CreateAssetMenu()]
public class BurnedRecipeSO : ScriptableObject
{
    public KitchenObjectsSO input;
    public KitchenObjectsSO output;
    public float burnedTimerMax;
}
