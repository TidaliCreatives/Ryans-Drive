using UnityEngine;

public class JuicyAnimation : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] bool doMove = false;
    [SerializeField] bool moveRight = false;
    [SerializeField] bool hasGlobalBoundaries = false;
    [SerializeField] float boundarieLeft = 50f;
    [SerializeField] float boundarieRight = 70f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float maxMoveDistance = 40f;
    [Space]

    [Header("Scaling")]
    [SerializeField] bool doScale = true;
    [SerializeField] float bounceTime = 14f;
    [SerializeField] float maxScaleAddition = 0.025f;
    bool positiveYScaling = true;
    float yScale = 1;
    [Space]

    [Header("Bounce")]
    [SerializeField] bool doBounce = false;
    [SerializeField] float elevationTime = 14f;
    [SerializeField] float maxElevation = 0.025f;
    [SerializeField] bool doRandomizeSeed = false;


    // General stuff
    Vector3 startPos;
    float newYPosition;
    bool isGoingUp = true;

    private void Start()
    {
        startPos = transform.position;

        if (doRandomizeSeed)
        {
            newYPosition = Random.Range(-1f, 1f) * maxElevation;
            transform.position = new Vector3(transform.position.x, startPos.y + newYPosition, transform.position.z);
            isGoingUp = transform.position.y <= startPos.y;
        }
    }

    private void Update()
    {
        if (doScale) { JuicyScale(); }

        if (doBounce) { JuicyBounce(); }

        if (doMove) { JuicyMove(); }
    }


    void JuicyScale()
    {
        if (positiveYScaling && transform.localScale.y < yScale + maxScaleAddition)
        {
            transform.localScale += new Vector3(0, maxScaleAddition * Time.deltaTime / bounceTime, 0);
        }
        else if (positiveYScaling && transform.localScale.y >= yScale + maxScaleAddition)
        {
            positiveYScaling = false;
        }
        else if (!positiveYScaling && transform.localScale.y > yScale - maxScaleAddition)
        {
            transform.localScale -= new Vector3(0, maxScaleAddition * Time.deltaTime / bounceTime, 0);
        }
        else if (!positiveYScaling && transform.localScale.y <= yScale - maxScaleAddition)
        {
            positiveYScaling = true;
        }
    }

    private void JuicyBounce()
    {
        if (isGoingUp && transform.position.y < startPos.y + maxElevation)
        {
            transform.position += new Vector3(0, 2f * maxElevation * Time.deltaTime / elevationTime, 0);
        }
        else if (isGoingUp && transform.position.y >= startPos.y + maxElevation)
        {
            isGoingUp = false;
        }
        else if (!isGoingUp && transform.position.y > startPos.y - maxElevation)
        {
            transform.position -= new Vector3(0, 2f * maxElevation * Time.deltaTime / elevationTime, 0);
        }
        else if (!isGoingUp && transform.position.y <= startPos.y - maxElevation)
        {
            isGoingUp = true;
        }
    }

    private void JuicyMove()
    {
        // Move
        transform.Translate((moveRight ? 1 : -1) * moveSpeed * Time.deltaTime * Vector3.right);

        // Reset x pos when reached global boundaries
        if (hasGlobalBoundaries)
        {
            if (transform.position.x < -Mathf.Abs(boundarieLeft)) // Math Abs is for edge case when boundarieLeft is negative
            {
                transform.Translate((-transform.position.x + boundarieRight) * Vector3.right);
            }
            else if (transform.position.x > boundarieRight)
            {
                transform.Translate((-transform.position.x - Mathf.Abs(boundarieLeft)) * Vector3.right); // Math Abs is for edge case when boundarieLeft is negative
            }
            return;
        }

        // Reset x pos when reached max distance
        if (Mathf.Abs(transform.position.x - startPos.x) > maxMoveDistance)
        {
            transform.position = new Vector3(startPos.x, transform.position.y, transform.position.z);
        }
    }
}