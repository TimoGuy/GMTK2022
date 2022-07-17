using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealthTracker : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;
    public float waitAfterLimpSeconds = 0.5f;
    public int xpOnDefeat = 0;
    public ReceiveAttackHitbox receiveAttackHitbox;

    [Header("UI Elements")]
    public Text uiHealthText;
    public Image uiHealthBarBg;
    public Image uiHealthBarResponsive;

    private GameManagerScript _gameManagerScript;

    void Start()
    {
        _gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
        ChangeHealthRelative(0);
    }

    public void ChangeHealthRelative(int amountRelative)
    {
        health += (int)((float)amountRelative * receiveAttackHitbox._previousHitMultiplier);
        health = Mathf.Clamp(health, 0, maxHealth);

        CheckIfLimp();
        UpdateUI();
    }

    public void ReplenishHealthRelative(int amountRelative)
    {
        health += amountRelative;
        health = Mathf.Clamp(health, 0, maxHealth);

        CheckIfLimp();
        UpdateUI();
    }

    public void CheckIfLimp()
    {
        if (health > 0)
            return;

        var _rb = GetComponent<Rigidbody>();
        _rb.mass *= 0.5f;
        _rb.useGravity = false;
        _rb.maxAngularVelocity = 200.0f;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.angularVelocity = new Vector3(0.0f, 200.0f, 0.0f);

        foreach (var coll in GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }

        StartCoroutine(DestroyTimerCoroutine());
    }

    IEnumerator DestroyTimerCoroutine()
    {
        yield return new WaitForSeconds(waitAfterLimpSeconds);
        _gameManagerScript.ReportXPEarned(xpOnDefeat, transform.position);
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        if (uiHealthBarBg == null || uiHealthBarResponsive == null)
            return;

        uiHealthBarBg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)maxHealth + 4.0f);
        uiHealthBarResponsive.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)health);

        if (uiHealthText == null)
            return;

        uiHealthText.text = $"{health} / {maxHealth}";
    }
}
