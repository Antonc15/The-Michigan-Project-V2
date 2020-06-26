using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    [Header("Floating Text")]
    public GameObject floatingText;
    public float textOffSetY;
    public float textSpreadPlane = 1f;
    public float textSpreadY = 0.3f;

    [Header("Settings")]
    public Target body;

    [Header("Health")]
    public Slider healthBar;
    public int health;
    public bool isHead;

    int maxHealth;
    Vector3 hitPos;

    bool executeHeadText;
    float minDam;
    float maxDam;
    bool criticalHit;

    void Awake()
    {
        maxHealth = health;

        if(isHead && body == null)
        {
            body = transform.parent.GetComponent<Target>();
        }

        if(!isHead && healthBar != null)
            healthBar.gameObject.SetActive(false);
    }
    public void TakeDamage(int amount)
    {
        if (!isHead)
        {
            health -= amount;


            if (floatingText != null)
                ShowFloatingText(amount);

            if (health <= 0f)
            {
                Die();
            }
        }
        else
        {
            body.executeHeadText = true;
            body.TakeDamage(amount * 2);
        }

        float healthFloat = health;
        float maxHealthFloat = maxHealth;

        if (!isHead && healthBar != null && !healthBar.gameObject.activeSelf)
            healthBar.gameObject.SetActive(true);

        if (!isHead && healthBar != null)
            healthBar.value = healthFloat / maxHealthFloat;

    }

    public void HitPosition(Vector3 hitLoc)
    {
        if (isHead)
        {
            body.HitPosition(hitLoc);
        }
        else
        {
            hitPos = hitLoc;
        }
    }

    public void AssignColor(float minDamage, float maxDamage, bool isCritical)
    {
        criticalHit = isCritical;

        if (!criticalHit)
        {
            minDam = minDamage;
            maxDam = maxDamage;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void ShowFloatingText(int damage)
    {
        Vector3 location;
        location = new Vector3(transform.position.x + Random.Range(-textSpreadPlane, textSpreadPlane), hitPos.y + Random.Range(textSpreadY, textSpreadY), transform.position.z + Random.Range(-textSpreadPlane, textSpreadPlane));

        GameObject text = Instantiate(floatingText, location, Quaternion.identity);
        text.transform.parent = GameObject.Find("DamageTextHolder").transform;
        text.GetComponent<TextMesh>().text = damage.ToString();

        if (!executeHeadText)
        {
            if (criticalHit)
            {
                int fontSize = text.GetComponent<TextMesh>().fontSize;

                text.GetComponent<TextMesh>().color = text.GetComponent<DamageText>().criticalHitColor;
                text.GetComponent<TextMesh>().fontSize = Mathf.RoundToInt(fontSize * 1.5f);
            }
            else
            {
                if (minDam == maxDam)
                {
                    text.GetComponent<TextMesh>().color = text.GetComponent<DamageText>().maxDamageColor;
                }
                else
                {
                    text.GetComponent<TextMesh>().color = Color32.Lerp(text.GetComponent<DamageText>().minDamageColor, text.GetComponent<DamageText>().maxDamageColor, (damage - minDam) / (maxDam - minDam));
                }
            }
        }
        else
        {
            //handles the size of text
            int fontSize = text.GetComponent<TextMesh>().fontSize;
            text.GetComponent<TextMesh>().fontSize = Mathf.RoundToInt(fontSize * 1.5f);

            //handles the color of text
            text.GetComponent<TextMesh>().color = text.GetComponent<DamageText>().headshotHitColor;

            //handles bold italisism
            text.GetComponent<TextMesh>().fontStyle = FontStyle.BoldAndItalic;

            executeHeadText = false;
        }
    }
}
