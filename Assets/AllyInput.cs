using System.Collections;
using UnityEngine;

public class AllyInput : MonoBehaviour
{
    public float decidingTimeInterval = 1.0f;
    public float maxTurnSpeed = 90.0f;
    public float attackRangeRadius = 5.0f;
    public float homingSpeed = 5.0f;
    public float attackTime = 1.0f;
    private float _attackTimeElapsed = 0.0f;
    private bool _attackModeLock = false;
    public AnimationCurve attackCurve;
    private float _currentFacingAngle;
    public GameManagerScript gameManagerScript;
    public FlatRBMovement flatRBMovement;
    private Vector3 _focusPosition;
    private Rigidbody _rb;

    void Start()
    {
        var currentFacingDirection = transform.forward;
        currentFacingDirection.y = 0.0f;
        currentFacingDirection.Normalize();
        _currentFacingAngle = Mathf.Atan2(currentFacingDirection.x, currentFacingDirection.z) * Mathf.Rad2Deg;

        _rb = GetComponent<Rigidbody>();

        StartCoroutine(AllyAI());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRangeRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _focusPosition);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0.0f, _currentFacingAngle, 0.0f) * Vector3.forward * 2.0f);
        }
    }

    IEnumerator AllyAI()
    {
        while (true)
        {
            // Refresh surroundings
            var spawnedEnemies = gameManagerScript.GetListOfSpawnedEnemies();

            float decidingTime = 0.0f;
            while (decidingTime < decidingTimeInterval)
            {
                decidingTime += Time.deltaTime;

                //
                // HOMING MODE
                //

                // Find enemy position that's closest distance wise, and most in line with the current moving direction
                Vector3 currentFacingDirection = Quaternion.Euler(0.0f, _currentFacingAngle, 0.0f) * Vector3.forward;

                bool first = true;
                Vector3 easiestToGetToPos = new Vector3();      float easiestToGetToDotProduct = -1.0f;
                Vector3 closestPos = new Vector3();             float closestDistance = -1.0f;
                foreach (var spawnedEnemy in spawnedEnemies)
                {
                    if (spawnedEnemy == null)
                        continue;

                    Vector3 deltaPosition = spawnedEnemy.transform.position - transform.position;
                    deltaPosition.y = 0.0f;

                    float distance = deltaPosition.magnitude;
                    if (first || distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPos = spawnedEnemy.transform.position;
                    }

                    float dotProduct = Vector3.Dot(currentFacingDirection, deltaPosition.normalized);
                    if (first || dotProduct > easiestToGetToDotProduct)
                    {
                        easiestToGetToDotProduct = dotProduct;
                        easiestToGetToPos = spawnedEnemy.transform.position;
                    }

                    first = false;
                }

                // See if enemy is in range, and attack if so. If not, home in towards the one whose most in the eyeline of the ally.
                if (_attackModeLock || (!first && closestDistance < attackRangeRadius))
                {
                    _attackModeLock = true;

                    // Switch to attack mode to closest enemy
                    _focusPosition = closestPos;
                    _attackTimeElapsed += Time.deltaTime / attackTime;

                    _rb.position = new Vector3(_rb.position.x, attackCurve.Evaluate(_attackTimeElapsed), _rb.position.z);

                    // Exit
                    if (_attackTimeElapsed > 1.0f)
                    {
                        _attackModeLock = false;
                        _attackTimeElapsed = 0.0f;
                    }
                }
                else
                {
                    // Home towards easiest to get to enemy
                    _focusPosition = easiestToGetToPos;

                    Vector3 deltaPosition = _focusPosition - transform.position;
                    deltaPosition.y = 0.0f;
                    deltaPosition.Normalize();

                    float targetFacingAngle = Mathf.Atan2(deltaPosition.x, deltaPosition.z) * Mathf.Rad2Deg;

                    _currentFacingAngle = Mathf.MoveTowardsAngle(_currentFacingAngle, targetFacingAngle, maxTurnSpeed * Time.deltaTime);

                    flatRBMovement.SendMovement(Quaternion.Euler(0.0f, _currentFacingAngle, 0.0f) * Vector3.forward, homingSpeed);
                }

                // Wait a frame
                yield return new WaitForEndOfFrame();
            }
        }
    }

    void Update()
    {
        
    }
}
