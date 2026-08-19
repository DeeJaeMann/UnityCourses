using System;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    private bool hasPackage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Package"))
        {
            Debug.Log($"Package Trigger {other.name}");
            hasPackage = true;
        }
        else if (other.CompareTag("Customer") && hasPackage)
        {
            Debug.Log($"Customer Delivered {other.name}");
            hasPackage = false;
        }
    }
}
