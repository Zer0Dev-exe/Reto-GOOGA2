using UnityEngine;

/// <summary>
/// Ejemplo de cómo integrar todos los sistemas 2D
/// Este script muestra cómo usar CameraEffects2D y ParticleSystem2D
/// </summary>
public class GameEffectsExample : MonoBehaviour
{
    private CameraEffects2D cameraEffects;
    private ParticleSystem2D particleSystem;
    
    private void Start()
    {
        // Obtener o añadir componentes
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            cameraEffects = mainCam.GetComponent<CameraEffects2D>();
            if (cameraEffects == null)
            {
                cameraEffects = mainCam.gameObject.AddComponent<CameraEffects2D>();
            }
        }
        
        // Crear sistema de partículas
        GameObject particleObj = new GameObject("ParticleSystem");
        particleSystem = particleObj.AddComponent<ParticleSystem2D>();
    }
    
    // ============================================
    // EJEMPLOS DE USO
    // ============================================
    
    /// <summary>
    /// Ejemplo 1: Efecto al seleccionar ingrediente
    /// </summary>
    public void OnIngredientSelected(Vector3 position)
    {
        // Shake suave de cámara
        if (cameraEffects != null)
        {
            cameraEffects.Shake(0.1f, 0.2f);
        }
        
        // Partículas doradas flotantes
        if (particleSystem != null)
        {
            Sprite particleSprite = CreateSimpleCircleSprite();
            particleSystem.EmitFloating(
                position, 
                15, 
                new Color(1f, 0.9f, 0.2f), 
                particleSprite
            );
        }
    }
    
    /// <summary>
    /// Ejemplo 2: Efecto al completar compra
    /// </summary>
    public void OnPurchaseComplete(Vector3 position)
    {
        // Shake más fuerte
        if (cameraEffects != null)
        {
            cameraEffects.Shake(0.3f, 0.5f);
        }
        
        // Explosión de confeti
        if (particleSystem != null)
        {
            Sprite particleSprite = CreateSimpleCircleSprite();
            
            // Múltiples colores
            Color[] colors = {
                Color.red,
                Color.yellow,
                Color.green,
                Color.blue,
                Color.magenta
            };
            
            foreach (Color color in colors)
            {
                particleSystem.EmitExplosion(
                    position,
                    10,
                    5f,
                    color,
                    particleSprite
                );
            }
        }
    }
    
    /// <summary>
    /// Ejemplo 3: Efecto al obtener puntuación alta
    /// </summary>
    public void OnHighScore(int score)
    {
        if (score >= 90)
        {
            // Zoom dramático
            if (cameraEffects != null)
            {
                cameraEffects.ZoomTo(4f, 3f);
                
                // Volver al zoom normal después de 2 segundos
                Invoke("ResetCameraZoom", 2f);
            }
            
            // Lluvia de estrellas
            if (particleSystem != null)
            {
                Sprite starSprite = CreateStarSprite();
                
                for (int i = 0; i < 5; i++)
                {
                    Vector3 pos = new Vector3(
                        Random.Range(-8f, 8f),
                        6f,
                        0
                    );
                    
                    particleSystem.EmitRain(
                        pos,
                        20,
                        Color.yellow,
                        starSprite
                    );
                }
            }
        }
    }
    
    /// <summary>
    /// Ejemplo 4: Efecto al cambiar de escena
    /// </summary>
    public void OnSceneTransition(Vector3 targetPosition)
    {
        if (cameraEffects != null)
        {
            // Seguir suavemente al objetivo
            StartCoroutine(FollowTargetCoroutine(targetPosition));
        }
    }
    
    private System.Collections.IEnumerator FollowTargetCoroutine(Vector3 target)
    {
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cameraEffects.FollowTarget(target, 5f);
            yield return null;
        }
    }
    
    private void ResetCameraZoom()
    {
        if (cameraEffects != null)
        {
            cameraEffects.ResetZoom();
        }
    }
    
    // ============================================
    // UTILIDADES PARA CREAR SPRITES SIMPLES
    // ============================================
    
    private Sprite CreateSimpleCircleSprite()
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        
        Color[] pixels = new Color[size * size];
        int center = size / 2;
        int radius = size / 2 - 1;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                
                if (dist < radius)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 20f);
    }
    
    private Sprite CreateStarSprite()
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        
        Color[] pixels = new Color[size * size];
        
        // Inicializar transparente
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        // Dibujar estrella simple (cruz + X)
        int center = size / 2;
        
        // Línea vertical
        for (int y = 2; y < size - 2; y++)
        {
            pixels[y * size + center] = Color.white;
        }
        
        // Línea horizontal
        for (int x = 2; x < size - 2; x++)
        {
            pixels[center * size + x] = Color.white;
        }
        
        // Diagonales
        for (int i = 3; i < size - 3; i++)
        {
            pixels[i * size + i] = Color.white;
            pixels[i * size + (size - 1 - i)] = Color.white;
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 20f);
    }
}

// ============================================
// EJEMPLO DE INTEGRACIÓN CON NutritionGame2D
// ============================================

/*
Para integrar estos efectos en NutritionGame2D.cs:

1. Añadir campos privados:
   private CameraEffects2D cameraEffects;
   private ParticleSystem2D particleSystem;
   private GameEffectsExample effectsExample;

2. En Start(), después de SetupCamera():
   cameraEffects = mainCamera.gameObject.AddComponent<CameraEffects2D>();
   
   GameObject particleObj = new GameObject("ParticleSystem");
   particleSystem = particleObj.AddComponent<ParticleSystem2D>();
   
   effectsExample = gameObject.AddComponent<GameEffectsExample>();

3. En SelectIngredient(), después de ingredient.OnClick():
   effectsExample.OnIngredientSelected(ingredient.transform.position);

4. En ShowResults(), al calcular el score:
   if (score >= 90)
   {
       effectsExample.OnHighScore(score);
   }

5. En EvaluateRecipe(), antes de ShowResults():
   effectsExample.OnPurchaseComplete(new Vector3(7f, -2f, 0)); // Posición de la cesta
*/
