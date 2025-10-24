using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public List<GameObject> AllTrafficPaths;


    private IEnumerator Start()
    {
        foreach (var trafficPath in AllTrafficPaths)
        {
            trafficPath.SetActive(true);
            yield return new WaitForSeconds(5);
        }
    }
}
