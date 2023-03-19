using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

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

    private float fryingTimer;
    private float burningTimer;
    private State state;
    private FryingRecipeSO fryingRecipeSO;
    private FryingRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer += Time.deltaTime;
                if (fryingTimer > fryingRecipeSO.FriyingTimerMax)
                {
                    // fried
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.Output, this);

                    state = State.Fried;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = state});
                    
                    burningTimer = 0f;
                    burningRecipeSO = GetFryingRecipeSOWithInput(fryingRecipeSO.Output);
                }

                break;
            case State.Fried:
                burningTimer += Time.deltaTime;
                if (burningTimer > burningRecipeSO.FriyingTimerMax)
                {
                    // burned
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(burningRecipeSO.Output, this);

                    state = State.Burned;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = state});
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
                player.GetKitchenObject().SetKitchenObjectParent(this);
                fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                state = State.Frying;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = state});
                fryingTimer = 0f;
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                // player is not carrying something   
                // pick up the object
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = state});
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
}
