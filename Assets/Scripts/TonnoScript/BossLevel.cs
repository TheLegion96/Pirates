using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel : MonoBehaviour
{
    [SerializeField]
    static public GameObject[] TNT;
    [SerializeField]
    private GameObject BossNotStatic;

    static int MaxCounter;
    public int Counter = 0;
    public Player player;

    private bool levelFinished = false;

    // Use this for initialization
    void Start()
    {
        TNT = GameObject.FindGameObjectsWithTag("Soda");
        MaxCounter = TNT.Length;

    }

    //  private float nextActionTime = 0.0f;
    // public float period = 0.1f;

    void Update()
    {
        if (MaxCounter <= Counter)
        {
            if (!levelFinished)
            {
                levelFinished = true;
                Counter = 0;
                //Destroy(BossNotStatic.gameObject);
                BossNotStatic.GetComponent<Animator>().SetTrigger("Die");

                StartCoroutine(FinishWithDelay());

            }
        }
        else
            CannonTriggered();
    }

    public IEnumerator FinishWithDelay()
    {
        player.enabled = false;
        yield return new WaitForSeconds(1);
        player.LevelFinished();
        yield return null;
    }

    public void CannonTriggered()
    {
        Counter = 0;
        for (int i = 0; i < TNT.Length; i++)
        {
            if (TNT[i] == null)
            {
                Counter++;
            }
        }
    }
}
