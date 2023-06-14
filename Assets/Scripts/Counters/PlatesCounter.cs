using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    [SerializeField] private float spawnPlateTimerMax = 4f;
    [SerializeField] private int platesSpawnMax = 5;

    private int platesSpawnedAmount;
    
    
    
    private float spawnPlateTimer;

    

    private void Update()
    {
        if (!IsServer) return;

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            //KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, this);
            spawnPlateTimer = 0;

            if (platesSpawnedAmount < platesSpawnMax)
            {
                SpawnPlateServerRpc();  // technically this doesn't need to be a ServerRpc since the server is calling it, but it's good practice to use ServerRpc for server->client communication
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // player is empty handed
            if (platesSpawnedAmount > 0)
            {
                // there is at least one available plate here

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc() => SpawnPlateClientRpc();


    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc() => InteractClientRpc();

    [ClientRpc]
    private void InteractClientRpc()
    {
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
