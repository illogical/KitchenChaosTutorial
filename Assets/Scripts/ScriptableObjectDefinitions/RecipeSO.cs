using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
       public string RecipeName;
       public List<KitchenObjectSO> KitchenObjectSOList;
}
