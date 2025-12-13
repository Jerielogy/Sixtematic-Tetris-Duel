using UnityEngine;

// This object will hold the player's identity for the entire session.
public class PlayerIdentity : MonoBehaviour
{
    // Static instance for easy access from anywhere (Singleton Pattern)
    public static PlayerIdentity Instance;

    // Public properties to store the data
    public string PlayerUsername { get; private set; } = "Player1";
    public string TeamRepresenting { get; private set; } = "Arcade Solo"; // Default for Arcade

    void Awake()
    {
        // Enforce only one instance of this manager exists
        if (Instance == null)
        {
            Instance = this;
            // CRITICAL: Keeps this object alive when moving from Main Menu to Arcade Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this to update the player's name when they type it in the UI
    public void SetIdentity(string username, string team)
    {
        PlayerUsername = username;
        TeamRepresenting = team;
        Debug.Log($"Identity Set: {PlayerUsername} ({TeamRepresenting})");
    }
}