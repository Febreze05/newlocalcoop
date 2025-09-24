using GDD4500.LAB01;
using UnityEngine;
using System.Collections;

public class Powerups : MonoBehaviour
{
    #region Variables
    // this just sets how much speed the player gains from the boost
    public float speedBoost = 5f;

    // this just sets the multiplier for how much the player grows
    public float scaleIncrease = 1.5f;

    // this just sets how long the powerup effect will last
    public float effectDuration = 5f;

    // this just sets how long before the item respawns after being picked up
    public float respawnTime = 10f;

    // this just references the item’s collider
    private Collider itemCollider;

    // this just references the item’s renderer (visual appearance)
    private MeshRenderer itemRenderer;

    // this just references the pickup’s particle effect
    private ParticleSystem pickupEffect;
    #endregion

    #region Unity Methods
    private void Start()
    {
        // this just grabs the collider on this object
        itemCollider = GetComponent<Collider>();

        // this just grabs the mesh renderer on this object
        itemRenderer = GetComponent<MeshRenderer>();

        // this just grabs the particle system attached to a child object
        pickupEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // this just checks if the thing colliding is the player
        if (other.CompareTag("Player"))
        {
            // this just randomly decides which effect to give: 0 = speed, 1 = scale
            int choice = Random.Range(0, 2);

            if (choice == 0)
            {
                // this just starts the speed boost coroutine
                StartCoroutine(TemporarySpeedBoost(other.GetComponent<PlayerMoveMechanic>()));
            }
            else
            {
                // this just starts the size increase coroutine
                StartCoroutine(TemporarySizeIncrease(other.transform));
            }

            // this just hides the item while it is waiting to respawn
            itemRenderer.enabled = false;
            itemCollider.enabled = false;

            // this just stops the pickup particle effect
            pickupEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // this just starts the respawn timer
            StartCoroutine(RespawnItem());
        }
    }
    #endregion

    #region Powerup Coroutines
    private IEnumerator TemporarySpeedBoost(PlayerMoveMechanic player)
    {
        if (player != null)
        {
            // this just increases player movement stats
            player._MaxSpeed += speedBoost;
            player._Acceleration += speedBoost;

            // this just waits for the effect duration
            yield return new WaitForSeconds(effectDuration);

            // this just resets player stats back to normal
            player._MaxSpeed -= speedBoost;
            player._Acceleration -= speedBoost;
        }
    }

    private IEnumerator TemporarySizeIncrease(Transform playerTransform)
    {
        // this just saves the player’s original size
        Vector3 originalScale = playerTransform.localScale;

        // this just makes the player bigger
        playerTransform.localScale = originalScale * scaleIncrease;

        // this just waits for the effect duration
        yield return new WaitForSeconds(effectDuration);

        // this just resets the player’s size
        playerTransform.localScale = originalScale;
    }

    private IEnumerator RespawnItem()
    {
        // this just waits until the item should respawn
        yield return new WaitForSeconds(respawnTime);

        // this just re enables visuals and collider
        itemRenderer.enabled = true;
        itemCollider.enabled = true;

        // this just restarts the pickup effect
        pickupEffect.Play();
    }
    #endregion
}
