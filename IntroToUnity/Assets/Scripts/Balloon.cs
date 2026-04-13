using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField]
    private float increaseScaleAmount = 0.1f;

    [SerializeField]
    private int clicksToPop = 5;

    public void IncreaseSize()
    {
        // Debug.Log("Increase Size");
        transform.localScale += Vector3.one * increaseScaleAmount;
        clicksToPop--;

        if (clicksToPop == 0)
        {
            Destroy(gameObject);
        }
    }
}
