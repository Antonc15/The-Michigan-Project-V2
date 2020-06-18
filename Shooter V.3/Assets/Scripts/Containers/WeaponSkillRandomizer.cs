using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class WeaponSkillRandomizer : MonoBehaviour
{

    void Awake()
    {
        int chance = 0;

        //assigning the skill amount
        chance = Random.Range(0,100);

        if (chance < 1)
        {
            for(int i = 0; i < 3; i++)
            {

            }
        }
        else if (chance < 10)
        {
            for (int i = 0; i < 2; i++)
            {

            }
        }
        else if (chance < 50)
        {

        }

        Destroy(this);

    }
}
