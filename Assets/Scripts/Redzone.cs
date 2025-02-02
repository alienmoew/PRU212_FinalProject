﻿using System.Collections;
using UnityEngine;

public class Redzone : MonoBehaviour
{
    public float shrinkRate; // How fast the redzone shrinks per second
    public float damageRate; // Time in seconds between damage ticks
    public int damageAmount; // Damage amount per tick
    public int numSegments; // Number of segments for the circle
    public float initialRadius; // Initial radius of the redzone
    public float shrinkInterval1; // Interval in seconds for first shrink step
    public float shrinkInterval2; // Interval in seconds for second shrink step
    public float finalRadius; // Final radius to shrink to

    private LineRenderer lineRenderer;
    private CircleCollider2D redzoneCollider;
    private bool playerInRedzone = false;
    private Coroutine damageCoroutine;
    private float elapsedTime = 0f;
    private float totalShrinkTime;

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

        totalShrinkTime = shrinkInterval1 + shrinkInterval2 * 2;
        StartCoroutine(ShrinkRedzone());
    }

    private void Update()
    {
        // Update the redzone timer UI every frame
        if (UIManager.Instance != null)
        {
            float remainingTime = GetRemainingTime();
            UIManager.Instance.UpdateRedzoneTimer(remainingTime);
        }
    }

    private IEnumerator ShrinkRedzone()
    {
        yield return ShrinkToRadius(initialRadius, shrinkInterval1, 60f);

        yield return ShrinkToRadius(60f, shrinkInterval2, 30f);

        yield return ShrinkToRadius(30f, shrinkInterval2, finalRadius);

        // Ensure radius does not go below finalRadius
        redzoneCollider.radius = finalRadius;
        DrawRedzone();
    }

    private IEnumerator ShrinkToRadius(float startRadius, float shrinkInterval, float targetRadius)
    {
        float currentRadius = startRadius;
        float shrinkTime = 0f;

        while (currentRadius > targetRadius)
        {
            float deltaTime = Time.deltaTime;
            currentRadius -= shrinkRate * deltaTime;
            redzoneCollider.radius = currentRadius;
            DrawRedzone();
            elapsedTime += deltaTime;
            shrinkTime += deltaTime;
            yield return null;
        }

        // Ensure radius does not go below targetRadius
        redzoneCollider.radius = Mathf.Max(currentRadius, targetRadius);
        DrawRedzone();

        // Wait for the remaining time of the shrink interval
        float remainingShrinkTime = shrinkInterval - shrinkTime;
        if (remainingShrinkTime > 0)
        {
            yield return new WaitForSeconds(remainingShrinkTime);
            elapsedTime += remainingShrinkTime;
        }
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

    public float GetRemainingTime()
    {
        return Mathf.Max(0, totalShrinkTime - elapsedTime);
    }
}
