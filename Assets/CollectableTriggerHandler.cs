using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableTriggerHandler : MonoBehaviour
{
    public int amountOfHealth = 10;
    public ParticleSystem myParticleSystem;

    private bool _canCollect = false;


    void Start()
    {
        var burst = myParticleSystem.emission.GetBurst(0);
        burst.count = amountOfHealth;
        myParticleSystem.emission.SetBurst(0, burst);

        StartCoroutine(DictateLifetime());
    }


    void OnTriggerStay(Collider collider)
    {
        if (!_canCollect)
            return;

        if (collider.gameObject.tag != "Player")
            return;

        collider.gameObject.SendMessage("ReplenishHealthRelative", amountOfHealth);
        Destroy(gameObject);
    }


    IEnumerator DictateLifetime()
    {
        _canCollect = false;
        yield return new WaitForSeconds(1.0f);
        _canCollect = true;
        yield return new WaitForSeconds(59.0f);
        Destroy(gameObject);
    }
}
