using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float followPercentage = 10f;
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = false;
    [SerializeField] bool followZ = false;

    Vector3 startPosition;

    private void Start()
    {
        // Save the starting position
        startPosition = transform.position;
    }

    private void Update()
    {
        // If follows are all false, do nothing
        if (!followX && !followY && !followZ)
        {
            return;
        }

        // Get this position
        Vector3 newPos = transform.position;

        // Get player position
        Vector3 playerPos = player.position;

        // Follow x coordinate of the player
        if (followX)
        {
            newPos.x = playerPos.x * (followPercentage / 100f);
        }
        else
        {
            newPos.x = 0;
        }

        // Follow y coordinate of the player
        if (followY)
        {
            newPos.y = playerPos.y * (followPercentage / 100f);
        }
        else
        {
            newPos.y = 0;
        }

        // Follow z coordinate of the player
        if (followZ)
        {
            newPos.z = playerPos.z * (followPercentage / 100f);
        }
        else
        {
            newPos.z = 0;
        }

        // Set new position
        transform.position = startPosition + newPos;
    }
}