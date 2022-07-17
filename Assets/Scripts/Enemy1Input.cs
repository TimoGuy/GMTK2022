using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Input : MonoBehaviour
{
    public FlatRBMovement flatRBMovement;
    public AttackComponent attackComponent;

    [Header("Creep towards player mode")]
    public Transform playerTransform;
    public float movementSpeed = 0.25f;
    public AnimationCurve bobbingMotionY;
    public float bobbingMotionSpeed = 1.0f;
    private float _bobbingMotionSamplePos = 0.0f;

    [Header("Attack Mode")]
    public GameObject modelRef;
    private Renderer[] _modelRefRenderers;
    public Material flashMaterialRef;
    public float flashMaterialShowTime = 0.2f;
    public float attackTriggerRadius = 10.0f;
    public float attackWarmUpTime = 0.5f;
    private bool _doAttack = false;
    public float attackDoingTime = 1.0f;
    public float attackMovementSpeed = 4.0f;
    private Vector3 _tempSavedMovementDirectionForAttack;
    private Coroutine _flashAttackCoroutine = null;
    public GameObject hitBox;

    private int _mode = 1;       // 1: creep towards player;;;;; 2: attack player


    void Start()
    {
        _modelRefRenderers = modelRef.GetComponentsInChildren<Renderer>();
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void OnDisable()
    {
        if (_flashAttackCoroutine != null)
        {
            StopCoroutine(_flashAttackCoroutine);
            _flashAttackCoroutine = null;

            // Return to normal mode
            _doAttack = false;
            _mode = 1;
            hitBox.SetActive(false);

            // Reset materials if needed
            if (_savedMaterials.Count > 0)
            {
                for (int i = 0; i < _modelRefRenderers.Length; i++)
                {
                    var renderer = _modelRefRenderers[i];
                    renderer.material = _savedMaterials[i];
                }
                _savedMaterials.Clear();
            }
        }
    }


    void FixedUpdate()
    {
        Vector3 movementDirection = playerTransform.position - transform.position;
        float distanceFromPlayer = movementDirection.magnitude;
        movementDirection.Normalize();

        //
        // Big
        //
        switch (_mode)
        {
        case 1:
        {
            flatRBMovement.SendMovement(movementDirection, movementSpeed);

            if (distanceFromPlayer < attackTriggerRadius)
            {
                _mode = 2;
                _flashAttackCoroutine = StartCoroutine(FlashAttack());
            }
        }
        break;

        case 2:
        {
            if (_doAttack)
            {
                flatRBMovement.SendMovement(
                    _tempSavedMovementDirectionForAttack,
                    attackMovementSpeed
                );
            }
        }
        break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackTriggerRadius);
    }

    private List<Material> _savedMaterials = new List<Material>();
    IEnumerator FlashAttack()
    {
        _savedMaterials.Clear();
        for (int i = 0; i < _modelRefRenderers.Length; i++)
        {
            var renderer = _modelRefRenderers[i];
            _savedMaterials.Add(renderer.material);
            renderer.material = flashMaterialRef;
        }

        yield return new WaitForSeconds(flashMaterialShowTime);

        for (int i = 0; i < _modelRefRenderers.Length; i++)
        {
            var renderer = _modelRefRenderers[i];
            renderer.material = _savedMaterials[i];
        }
        _savedMaterials.Clear();

        yield return new WaitForSeconds(attackWarmUpTime);

        _tempSavedMovementDirectionForAttack = (playerTransform.position - transform.position).normalized;
        _doAttack = true;
        hitBox.SetActive(true);
        attackComponent.triggerAttack = true;

        yield return new WaitForSeconds(attackDoingTime);
        
        // Return to normal mode
        _doAttack = false;
        _mode = 1;
        hitBox.SetActive(false);
    }

    public void AbortAttackMovement()
    {
        _doAttack = false;
        flatRBMovement.SendMovement(
            _tempSavedMovementDirectionForAttack,
            0.0f
        );
        hitBox.SetActive(false);
    }
}
