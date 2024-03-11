using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
sealed class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    private SpriteController spriteController;

    [SerializeField] private bool moveKeyHeld; //read-only
    
    private LadderObjectManager ladderObjectManager;


    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        spriteController = GetComponentInChildren<SpriteController>();
        ladderObjectManager = GameObject.Find("LadderManager").GetComponent<LadderObjectManager>(); // spaguetti but f it we ball
    }
    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Player.Disable();
    }

    void Controls.IPlayerActions.OnMovement(InputAction.CallbackContext context)
    {
        if (context.started)
            moveKeyHeld = true;
        else if (context.canceled)
            moveKeyHeld = false;
    }

    void Controls.IPlayerActions.OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
            Action.EscapeAction();
    }

    public void OnView(InputAction.CallbackContext context)
    {
        if (context.performed)
            UIManager.instance.ToggleMessageHistory();
    }

    private void FixedUpdate()
    {
        if (!UIManager.instance.IsMessageHistoryOpen)
        {
            if (GameManager.instance.IsPlayerTurn && moveKeyHeld && GetComponent<Actor>().IsAlive && !GetComponent<GhostAbility>().GhostAbilityAvailable)
            {   
                Debug.Log(!GetComponent<GhostAbility>().GhostAbilityAvailable);
                MovePlayer();
            }
           
            

        }
    }

    private void MovePlayer()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Vector3 futurePosition = transform.position + (Vector3)roundedDirection;
        spriteController.SetSprite(roundedDirection);
        Debug.Log(transform.position);

        //update player position for ladder algorithm
        ladderObjectManager.updatePlayerPosition(new Vector3(futurePosition.x-0.5f, futurePosition.y-0.5f, 0));

        if (IsValidPosition(futurePosition))
        
            moveKeyHeld = Action.BumpAction(GetComponent<Actor>(), roundedDirection); //If we bump into an entity, moveKeyHeld is set to false.

        
    }

    private bool IsValidPosition(Vector3 futurePosition)
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(futurePosition);
        if (!MapManager.instance.InBounds(gridPosition.x, gridPosition.y) || !MapManager.instance.FloorMap.HasTile(gridPosition) || futurePosition == transform.position)
            return false;

        return
            // In bounds of the known maximum coordinates
            MapManager.instance.InBounds(gridPosition.x, gridPosition.y)
            // FloorMap has a tile there
         && MapManager.instance.FloorMap.HasTile(gridPosition)
            // There's no collision for the future position (aka is ground)
         && MapManager.instance.FloorMap.GetColliderType(gridPosition) == Tile.ColliderType.None
            // We actually move to a new position
         && futurePosition != transform.position;
    }
}