using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceOnHit : MonoBehaviour
{
    [System.Serializable]
    public class FaceAndReward
    {
        public Transform faceTransform;
        public GameObject rewardPrefab;
    }

    public ReceiveAttackHitbox receiveAttackHitbox;
    private Rigidbody _rb;
    private bool _wasHit = false;

    public FaceAndReward[] faceAndRewards;
    public GameObject healthPrefab;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    public void ReceiveKnockbackFrom(float force)
    {
        if (_wasHit)
            return;

        Vector3 flatDirection = transform.position - receiveAttackHitbox._previousHitPosition;
        flatDirection.y = 5.0f;
        flatDirection.Normalize();
        _rb.AddForceAtPosition(flatDirection * force * receiveAttackHitbox._previousHitMultiplier, _rb.position + Random.onUnitSphere, ForceMode.Impulse);
        _wasHit = true;

        StartCoroutine(WaitForSettleAndDoDice());
    }


    IEnumerator WaitForSettleAndDoDice()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject prefabToInstantiate = null;
        int faceIndex = -1;
        while (_rb.velocity.sqrMagnitude > 0.01f && _rb.angularVelocity.sqrMagnitude > 0.01f)
        {
            // Find the face that is most up
            float highestUpY = -1.0f;
            for (int i = 0; i < faceAndRewards.Length; i++)
            {
                var faceAndReward = faceAndRewards[i];
                if (highestUpY < -faceAndReward.faceTransform.forward.y)
                {
                    highestUpY = -faceAndReward.faceTransform.forward.y;
                    faceIndex = i;
                    prefabToInstantiate = faceAndReward.rewardPrefab;
                }
            }

            // Update the faces so they render/don't
            for (int i = 0; i < faceAndRewards.Length; i++)
                faceAndRewards[i].faceTransform.gameObject.SetActive(i == faceIndex);

            // Wait for next physics step
            yield return new WaitForFixedUpdate();
        }

        // Instantiate the assigned gameobject
        if (prefabToInstantiate != null)
            Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
        else
        {
            var stuff = Instantiate(healthPrefab, transform.position, Quaternion.identity).GetComponent<CollectableTriggerHandler>();
            stuff.amountOfHealth = faceIndex + 1;
            stuff.myParticleSystem.collision.SetPlane(0, GameObject.FindGameObjectWithTag("CollisionGround").transform);
        }

        // Show the symbol for a little bit (i.e. take away the dice mesh)
        Destroy(GetComponentInChildren<MeshFilter>().gameObject);

        yield return new WaitForSeconds(2.5f);

        // Finally, destroy me sempai!
        Destroy(gameObject);
    }
}
