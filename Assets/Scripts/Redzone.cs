using System.Collections;
using UnityEngine;

public class Redzone : MonoBehaviour
{
    public float shrinkRate = 1f; // How fast the redzone shrinks per second
    public float damageRate = 1f; // Time in seconds between damage ticks
    public int damageAmount = 10; // Damage amount per tick
    public int numSegments = 100; // Number of segments for the circle
    public float initialRadius = 20f; // Initial radius of the redzone
    public float shrinkInterval1 = 15f; // Interval in seconds for first shrink step
    public float shrinkInterval2 = 10f; // Interval in seconds for second shrink step
    public float finalRadius = 5f; // Final radius to shrink to

    private LineRenderer lineRenderer;
    private CircleCollider2D redzoneCollider;
    private bool playerInRedzone = false;
    private Coroutine damageCoroutine;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        redzoneCollider = GetComponent<CircleCollider2D>();

        redzoneCollider.radius = initialRadius;

        lineRenderer.positionCount = numSegments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.1f; // Width of the line
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Choose material for the line
        lineRenderer.startColor = Color.red; // Color of the line
        lineRenderer.endColor = Color.red;

        StartCoroutine(ShrinkRedzone());
    }

    private IEnumerator ShrinkRedzone()
    {
        // First shrink to 15f
        yield return ShrinkToRadius(initialRadius, shrinkInterval1, 15f);

        // Second shrink to 10f
        yield return ShrinkToRadius(15f, shrinkInterval2, 10f);

        // Third shrink to 5f (final radius)
        yield return ShrinkToRadius(10f, shrinkInterval2, finalRadius);

        // Ensure radius does not go below finalRadius
        redzoneCollider.radius = finalRadius;
        DrawRedzone();

        yield break;
    }

    private IEnumerator ShrinkToRadius(float startRadius, float shrinkInterval, float targetRadius)
    {
        float currentRadius = startRadius;

        while (currentRadius > targetRadius)
        {
            currentRadius -= shrinkRate * Time.deltaTime;
            redzoneCollider.radius = currentRadius;
            DrawRedzone();
            yield return null;
        }

        // Ensure radius does not go below targetRadius
        redzoneCollider.radius = Mathf.Max(currentRadius, targetRadius);
        DrawRedzone();

        yield return new WaitForSeconds(shrinkInterval);
    }

    private void DrawRedzone()
    {
        float deltaTheta = (2f * Mathf.PI) / numSegments; // Angle between points on the circle
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = redzoneCollider.radius * Mathf.Cos(theta); // X coordinate of the point
            float y = redzoneCollider.radius * Mathf.Sin(theta); // Y coordinate of the point

            lineRenderer.SetPosition(i, new Vector3(x, y, 0)); // Set coordinates for each point on the circle
            theta += deltaTheta; // Increase angle to compute the next point on the circle
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRedzone = true;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRedzone = false;
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DealDamage(other.GetComponent<PlayerAimWeapon>()));
            }
        }
    }

    private IEnumerator DealDamage(PlayerAimWeapon player)
    {
        while (!playerInRedzone)
        {
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
            yield return new WaitForSeconds(damageRate);
        }
    }
}
