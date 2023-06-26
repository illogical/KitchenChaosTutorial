using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Recipe List")]
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> RecipeSOList;
}
