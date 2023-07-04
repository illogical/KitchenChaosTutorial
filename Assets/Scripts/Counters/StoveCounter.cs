using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State State;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private FryingRecipeSO fryingRecipeSO;
    private FryingRecipeSO burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimerOnValueChanged;
        burningTimer.OnValueChanged += BurningTimerOnValueChanged;
        state.OnValueChanged += StateOnValueChanged;
    }

    private void Update()
    {
        if (!IsServer) return;

        switch (state.Value)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer.Value += Time.deltaTime;

                if (fryingTimer.Value > fryingRecipeSO.FriyingTimerMax)
                {
                    // fried
                    KitchenGameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.Output, this);

                    state.Value = State.Fried;
                    burningTimer.Value = 0f;
                    SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(fryingRecipeSO.Output));
                }

                break;
            case State.Fried:
                burningTimer.Value += Time.deltaTime;

                if (burningTimer.Value > burningRecipeSO.FriyingTimerMax)
                {
                    // burned
                    KitchenGameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());
                    KitchenObject.SpawnKitchenObject(burningRecipeSO.Output, this);

                    state.Value = State.Burned;
                }
                break;
            case State.Burned:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (HasKitchenObject())
        {

        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // there is no object on the counter

            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                // player is carrying something
                // put the item to be fried on the empty counter
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);

                InteractPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
            }
        }
        else
        {
            // there is an object here

            if (!player.HasKitchenObject())
            {
                // player is not carrying something   
                // pick up the object
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStateIdleServerRpc();
            }
            else
            {
                // player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();

                        SetStateIdleServerRpc();
                    }
                }
            }

        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        return GetFryingRecipeSOWithInput(kitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        return GetFryingRecipeSOWithInput(kitchenObjectSO)?.Output;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (var fryingRecipeSo in fryingRecipeSOArray)
        {
            if (fryingRecipeSo.Input == kitchenObjectSO)
            {
                return fryingRecipeSo;
            }
        }

        return null;
    }

    private void FryingTimerOnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.FriyingTimerMax : 1f; // in case recipeSO is not initialized yet

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = fryingTimer.Value / fryingTimerMax
        });
    }

    private void BurningTimerOnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.FriyingTimerMax : 1f; // in case recipeSO is not initialized yet

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = burningTimer.Value / burningTimerMax
        });
    }

    private void StateOnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = state.Value });

        if (state.Value == State.Burned || state.Value == State.Idle)
        {
            // hide the progress bar
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                ProgressNormalized = 0f
            });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        state.Value = State.Frying;
        fryingTimer.Value = 0f;
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    } 

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        fryingRecipeSO = GetFryingRecipeSOWithInput(KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex));
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        burningRecipeSO = GetFryingRecipeSOWithInput(KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex));
    }
}
