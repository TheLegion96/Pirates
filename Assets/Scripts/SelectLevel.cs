using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SelectLevel : MonoBehaviour
{

    private int index = 0;

    void Start()
    {

    }

    void Update()
    {

        // Navigation
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (index < 3)
            {
                index++;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (index > 0)
            {
                index--;
            }
        }

        // Load levels
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (index == 0)
            {
                //
            }
            else if (index == 1)
            {
                //
            }
            else if (index == 2)
            {
                //
            }
            else if (index == 3)
            {
                //
            }
        }

    }
}
