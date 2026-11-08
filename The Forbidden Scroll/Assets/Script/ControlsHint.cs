using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class ControlsHint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(HideAfter10Seconds());
    }

    IEnumerator HideAfter10Seconds()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
