using UnityEngine;

/// <summary>
/// Controlador de partículas simples con gravedad
/// </summary>
public class ParticleController : MonoBehaviour
{
    public Vector2 velocity;
    public float lifetime;
    private float t = 0;
    
    private void Update()
    {
        t += Time.deltaTime;
        if (t > lifetime) { Destroy(gameObject); return; }
        transform.position += (Vector3)velocity * Time.deltaTime;
        velocity.y -= 9.8f * Time.deltaTime;
    }
}
