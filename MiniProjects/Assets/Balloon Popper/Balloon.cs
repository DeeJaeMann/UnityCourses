using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField]
    private int clicksToPop = 5;

    [SerializeField]
    private float scaleIncreasePerClick = 0.1f;

    [SerializeField] private ScoreManager scoreManager;
    
    public void IncreaseSize()
    {
        // Debug.Log("Increase Balloon Size");
        transform.localScale += Vector3.one * scaleIncreasePerClick;
        
        clicksToPop--;
        
        if(clicksToPop == 0)
        {
            scoreManager.IncreaseScore(1);
            Destroy(gameObject);
        }
    }
}
