using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Kitchen Object List")]
public class KitchenObjectListSO : ScriptableObject
{
    public List<KitchenObjectSO> KitchenObjectSOList;
}
