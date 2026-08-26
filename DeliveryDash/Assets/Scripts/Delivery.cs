using System;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    private bool _hasPackage;
    [SerializeField] private float delay = 0.5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Package") && !_hasPackage)
        {
            Debug.Log($"Package Trigger {other.name}");
            GetComponent<ParticleSystem>().Play();
            Destroy(other.gameObject, delay);
            _hasPackage = true;
        }
        else if (other.CompareTag("Customer") && _hasPackage)
        {
            Debug.Log($"Customer Delivered {other.name}");
            GetComponent<ParticleSystem>().Stop();
            _hasPackage = false;
        }
    }
}
