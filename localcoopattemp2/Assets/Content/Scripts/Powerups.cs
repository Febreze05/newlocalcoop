using GDD4500.LAB01;
using UnityEngine;
using System.Collections;

public class Powerups : MonoBehaviour
{
    public float speedBoost = 5f;       // Extra speed
    public float scaleIncrease = 1.5f;  // Multiplier for player size
    public float effectDuration = 5f;   // How long the effect lasts
    public float respawnTime = 10f;     // Time before item respawns


    private Collider itemCollider;
    private MeshRenderer itemRenderer;
    private ParticleSystem pickupEffect;

    private void Start()
    {
        itemCollider = GetComponent<Collider>();
        itemRenderer = GetComponent<MeshRenderer>();
        pickupEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int choice = Random.Range(0, 2); // 0 = speed, 1 = scale

            if (choice == 0)
            {
                StartCoroutine(TemporarySpeedBoost(other.GetComponent<PlayerMoveMechanic>()));
            }
            else
            {
                StartCoroutine(TemporarySizeIncrease(other.transform));
            }

            // Hide item while waiting to respawn
            itemRenderer.enabled = false;
            itemCollider.enabled = false;
            pickupEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            StartCoroutine(RespawnItem());
        }
    }

    private IEnumerator TemporarySpeedBoost(PlayerMoveMechanic player)
    {
        if (player != null)
        {
            player._MaxSpeed += speedBoost;
            player._Acceleration += speedBoost;
            yield return new WaitForSeconds(effectDuration);
            player._MaxSpeed -= speedBoost;
            player._Acceleration -= speedBoost;
        }
    }

    private IEnumerator TemporarySizeIncrease(Transform playerTransform)
    {
        Vector3 originalScale = playerTransform.localScale;
        playerTransform.localScale = originalScale * scaleIncrease;

        yield return new WaitForSeconds(effectDuration);

        playerTransform.localScale = originalScale;
    }

    private IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(respawnTime);

        itemRenderer.enabled = true;
        itemCollider.enabled = true;
        pickupEffect.Play();
    }
}
