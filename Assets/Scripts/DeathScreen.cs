using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour

{
   
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void Respawn()
    {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
