using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int selectedWeapon = 0;

    Interact playerInteract;
    [HideInInspector]
    public bool canSwitch;

    void Start()
    {
        SelectWeapon();
        playerInteract = GameObject.Find("Player").GetComponent<Interact>();
        canSwitch = true;
    }
    void Update()
    {

        int previousSelectedWeapon = selectedWeapon;

        if (!playerInteract.carrying && canSwitch)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (selectedWeapon >= transform.childCount - 1)
                {
                    selectedWeapon = 0;
                }
                else
                {
                    selectedWeapon++;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (selectedWeapon <= 0)
                {
                    selectedWeapon = transform.childCount - 1;
                }
                else
                {
                    selectedWeapon--;
                }
            }
        }

        if(previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }

    }

    void SelectWeapon()
    {

        int i = 0;

        foreach(Transform weapon in transform)
        {
            if(i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
