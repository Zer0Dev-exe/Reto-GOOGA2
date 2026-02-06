using UnityEngine;

/// <summary>
/// Sistema de partículas 2D personalizado
/// Crea efectos visuales sin usar el sistema de partículas de Unity
/// </summary>
public class ParticleSystem2D : MonoBehaviour
{
    [System.Serializable]
    public class Particle
    {
        public GameObject gameObject;
        public Vector2 velocity;
        public float lifetime;
        public float maxLifetime;
        public Color startColor;
        public Color endColor;
        public float startSize;
        public float endSize;
        public bool useGravity;
    }
    
    private System.Collections.Generic.List<Particle> particles = new System.Collections.Generic.List<Particle>();
    
    private void Update()
    {
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            Particle p = particles[i];
            
            if (p.lifetime <= 0f)
            {
                if (p.gameObject != null)
                    Destroy(p.gameObject);
                particles.RemoveAt(i);
                continue;
            }
            
            p.lifetime -= Time.deltaTime;
            
            // Mover
            if (p.gameObject != null)
            {
                p.gameObject.transform.position += (Vector3)p.velocity * Time.deltaTime;
                
                // Gravedad
                if (p.useGravity)
                {
                    p.velocity.y -= 9.8f * Time.deltaTime;
                }
                
                // Interpolar color
                float t = 1f - (p.lifetime / p.maxLifetime);
                SpriteRenderer sr = p.gameObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.Lerp(p.startColor, p.endColor, t);
                    
                    // Interpolar tamaño
                    float size = Mathf.Lerp(p.startSize, p.endSize, t);
                    p.gameObject.transform.localScale = new Vector3(size, size, 1f);
                }
            }
        }
    }
    
    /// <summary>
    /// Emitir partículas en una posición
    /// </summary>
    public void Emit(Vector3 position, int count, ParticleConfig config)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject particleObj = new GameObject("Particle");
            particleObj.transform.position = position;
            
            SpriteRenderer sr = particleObj.AddComponent<SpriteRenderer>();
            sr.sprite = config.sprite;
            sr.color = config.startColor;
            sr.sortingOrder = 100;
            
            Particle p = new Particle
            {
                gameObject = particleObj,
                velocity = GetRandomVelocity(config),
                lifetime = config.lifetime,
                maxLifetime = config.lifetime,
                startColor = config.startColor,
                endColor = config.endColor,
                startSize = config.startSize,
                endSize = config.endSize,
                useGravity = config.useGravity
            };
            
            particles.Add(p);
        }
    }
    
    /// <summary>
    /// Emitir partículas en explosión
    /// </summary>
    public void EmitExplosion(Vector3 position, int count, float force, Color color, Sprite sprite)
    {
        ParticleConfig config = new ParticleConfig
        {
            sprite = sprite,
            startColor = color,
            endColor = new Color(color.r, color.g, color.b, 0f),
            startSize = 0.3f,
            endSize = 0.1f,
            lifetime = 0.5f,
            minVelocity = force * 0.5f,
            maxVelocity = force,
            useGravity = true
        };
        
        Emit(position, count, config);
    }
    
    /// <summary>
    /// Emitir partículas en lluvia
    /// </summary>
    public void EmitRain(Vector3 position, int count, Color color, Sprite sprite)
    {
        ParticleConfig config = new ParticleConfig
        {
            sprite = sprite,
            startColor = color,
            endColor = new Color(color.r, color.g, color.b, 0f),
            startSize = 0.2f,
            endSize = 0.1f,
            lifetime = 1f,
            minVelocity = 1f,
            maxVelocity = 2f,
            useGravity = true,
            directionMin = new Vector2(-0.2f, -1f),
            directionMax = new Vector2(0.2f, -0.5f)
        };
        
        Emit(position, count, config);
    }
    
    /// <summary>
    /// Emitir partículas flotantes
    /// </summary>
    public void EmitFloating(Vector3 position, int count, Color color, Sprite sprite)
    {
        ParticleConfig config = new ParticleConfig
        {
            sprite = sprite,
            startColor = color,
            endColor = new Color(color.r, color.g, color.b, 0f),
            startSize = 0.3f,
            endSize = 0.2f,
            lifetime = 1.5f,
            minVelocity = 0.5f,
            maxVelocity = 1.5f,
            useGravity = false,
            directionMin = new Vector2(-0.5f, 0.5f),
            directionMax = new Vector2(0.5f, 1f)
        };
        
        Emit(position, count, config);
    }
    
    private Vector2 GetRandomVelocity(ParticleConfig config)
    {
        Vector2 direction;
        
        if (config.directionMin != Vector2.zero || config.directionMax != Vector2.zero)
        {
            direction = new Vector2(
                Random.Range(config.directionMin.x, config.directionMax.x),
                Random.Range(config.directionMin.y, config.directionMax.y)
            ).normalized;
        }
        else
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        
        float speed = Random.Range(config.minVelocity, config.maxVelocity);
        return direction * speed;
    }
    
    [System.Serializable]
    public class ParticleConfig
    {
        public Sprite sprite;
        public Color startColor = Color.white;
        public Color endColor = new Color(1f, 1f, 1f, 0f);
        public float startSize = 0.3f;
        public float endSize = 0.1f;
        public float lifetime = 1f;
        public float minVelocity = 1f;
        public float maxVelocity = 3f;
        public bool useGravity = false;
        public Vector2 directionMin = Vector2.zero;
        public Vector2 directionMax = Vector2.zero;
    }
}
