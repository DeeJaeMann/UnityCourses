using UnityEngine;
using UnityEngine.InputSystem;

public class BalloonClicker : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Debug.Log("Mouse Clicked!");
            Vector2 mousePosition = Mouse.current.position.value;
            // Expensive operation, find preferred method
            Ray ray = Camera.main!.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Debug.Log($"Hit: {hit.transform.name}");
                // Expensive operation, find preferred method
                Balloon balloon = hit.collider.GetComponent<Balloon>();

                balloon?.IncreaseSize();
            }
        }
    }
}
