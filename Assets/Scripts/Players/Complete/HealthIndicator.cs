using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealthIndicator : NetworkBehaviour
{
    [SerializeField]
    private Text informationText;       //Text element which will give information to the player
    
    public void UpdateHealth(int currentHealth)
    {
        informationText.text = currentHealth.ToString();
    }
}
