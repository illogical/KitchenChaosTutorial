using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Frying Recipe")]
public class FryingRecipeSO : ScriptableObject
{
    public KitchenObjectSO Input;
    public KitchenObjectSO Output;
    public float FriyingTimerMax;
}
