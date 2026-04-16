using UnityEngine;
using UnityEngine.InputSystem;

public class BalloonClicker : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Debug.Log("Mouse clicked");
            Vector2 mousePosition = Mouse.current.position.value;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Debug.Log("Hit something");
                Balloon balloon = hit.collider.GetComponent<Balloon>();

                balloon?.IncreaseSize();
            }
        }
    }
}
