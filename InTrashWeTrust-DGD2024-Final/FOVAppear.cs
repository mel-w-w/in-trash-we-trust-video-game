using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVAppear : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private FieldOfView FOV;
    public PlayerMovement player;

    private Coroutine visibilityCoroutine;
    public float visibilityDelay = 0.2f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        FOV = GetComponentInParent<FieldOfView>();
    }

    void Update()
    {
        bool canSeePlayer = FOV.CanSeePlayer;
        bool canSensePlayer = FOV.CanSensePlayer;
        bool isFighting = player.isFighting;
        bool isHiding = player.hiding;

        if (canSensePlayer || canSeePlayer)
        {
            SetVisibility(true);
        }
        else
        {
            if (visibilityCoroutine != null)
            {
                StopCoroutine(visibilityCoroutine);
            }
            visibilityCoroutine = StartCoroutine(DelayedVisibility(false, visibilityDelay));
        }

        if (isFighting || isHiding)
        {
            SetVisibility(false);
        }
    }

    // Coroutine to delay the visibility change
    private IEnumerator DelayedVisibility(bool visible, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetVisibility(visible);
    }

    // Method to set the visibility of the spriteRenderer
    private void SetVisibility(bool visible)
    {
        spriteRenderer.enabled = visible;
    }
}
