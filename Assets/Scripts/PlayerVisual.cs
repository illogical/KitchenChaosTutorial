using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{

    [SerializeField] private MeshRenderer headMeshRender;
    [SerializeField] private MeshRenderer bodyMeshRender;

    private Material material;

    private void Awake()
    {
        material = new Material(headMeshRender.material);
        headMeshRender.material = material;
        bodyMeshRender.material = material;
    }

    public void SetPlayerColor(Color color)
    {
        material.color = color;
    }   
}
