using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;

    public event EventHandler OnPickedUpSomething;
    public event EventHandler<OnSelectedChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;
    

    public bool IsWalking() => isWalking;
    private bool isWalking;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public static void ResetStaticData() => OnAnyPlayerSpawned = null;

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }


        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        // only the server should listen for this event
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
        }        
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInteractOnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInputOnOnInteractAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.ColorId));
    }


    private void GameInteractOnInteractAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying())
        {
            // block actions until game starts
            return;
        }
        
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void GameInputOnOnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying())
        {
            // block actions until game starts
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }
    
    void Update()
    {
        if(IsOwner) //local player only
        {
            HandleMovement();
            HandleInteractions();
        }

    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }
        
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) //more compact null check
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        bool canMove = !Physics.BoxCast(
            transform.position, 
            Vector3.one * playerRadius, 
            moveDir, 
            Quaternion.identity, 
            moveDistance,
            collisionsLayerMask);

        // allow a player to slide against a wall by moving diagonally. Split x and z inputs.
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized; 
            canMove = (moveDir.x < -0.5f || moveDir.x > 0.5f) && !Physics.BoxCast(
                transform.position,
                Vector3.one * playerRadius,
                moveDirX,
                Quaternion.identity,
                moveDistance,
                collisionsLayerMask);

            if (canMove)
            {
                // can move on the x
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -0.5f || moveDir.z > 0.5f) && !Physics.BoxCast(
                    transform.position,
                    Vector3.one * playerRadius,
                    moveDirZ,
                    Quaternion.identity,
                    moveDistance,
                    collisionsLayerMask);

                if (canMove)
                {
                    // can move on the z
                    moveDir = moveDirZ;
                }
            }
        }
        
        if (canMove)
        {
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }
        
        isWalking = moveDir != Vector3.zero;
        
        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() => kitchenObjectHoldPoint;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject is not null)
        {
            OnPickedUpSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public bool HasKitchenObject() => kitchenObject != null;

    public KitchenObject GetKitchenObject() => kitchenObject;

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public NetworkObject GetNetworkObject() => NetworkObject;

    private void NetworkManagerOnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId && HasKitchenObject())
        {
            // Can only destroy kitchen object from the server
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

}
