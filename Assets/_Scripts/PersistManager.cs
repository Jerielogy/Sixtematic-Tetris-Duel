using UnityEngine;

public class PersistManager : MonoBehaviour
{
    void Awake()
    {
        // This is the universal command that replaces the hidden DDOL checkbox.
        DontDestroyOnLoad(gameObject);
    }
}