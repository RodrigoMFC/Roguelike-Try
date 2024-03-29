using UnityEngine;

[RequireComponent(typeof(Fighter))]
public class HostileEnemy : AI
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private bool isFighting;
    private SpriteController spriteController;

    private void Start()
    {
        spriteController = GetComponentInChildren<SpriteController>();
    }

    private void OnValidate()
    {
        fighter = GetComponent<Fighter>();
        AStar = GetComponent<AStar>();
    }

    public void RunAI()
    {
        if (!fighter.Target)
        {
            fighter.Target = GameManager.instance.Actors[0];
        }
        else if (fighter.Target && !fighter.Target.IsAlive)
        {
            fighter.Target = null;
        }

        if (fighter.Target)
        {
            Vector3Int targetPosition = MapManager.instance.FloorMap.WorldToCell(fighter.Target.transform.position);
            if (isFighting || GetComponent<Actor>().FieldOfView.Contains(targetPosition))
            {
                if (!isFighting)
                {
                    isFighting = true;
                }

                float targetDistance = Vector3.Distance(transform.position, fighter.Target.transform.position);

                if (targetDistance <= 1.5f)
                {
                    Action.MeleeAction(GetComponent<Actor>(), fighter.Target);
                    return;
                }
                else
                { //If not in range, move towards target
                    MoveAlongPath(targetPosition);
                    Vector3Int vector3Int = MapManager.instance.FloorMap.WorldToCell(transform.position);
                    Actor actor = GetComponent<Actor>();
                    actor.movementCost = (MapManager.instance.TileIsGrass(vector3Int) || MapManager.instance.TileIsFire(vector3Int)) ? 1 : 0;
                    if (MapManager.instance.TileIsFire(vector3Int))
                    {
                        // Omae wa mou shindeiru
                        GetComponent<Fighter>().Hp = 0;

                    }

                    // update facing direction of sprite 
                    spriteController.SetSprite(facingDirection);
                    return;
                }
            }
        }

        Action.SkipAction();
    }
}