using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Recipe")]
public class RecipeSO : ScriptableObject
{
       public string RecipeName;
       public List<KitchenObjectSO> KitchenObjectSOList;
}
