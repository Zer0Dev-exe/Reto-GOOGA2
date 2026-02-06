using UnityEngine;

/// <summary>
/// Representa un ingrediente 2D con sprite y animación
/// </summary>
public class Ingredient2D : MonoBehaviour
{
    public string ingredientName;
    public Sprite sprite;
    public Color baseColor;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private float bobTimer = 0f;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        originalScale = transform.localScale;
    }
    
    private void Start()
    {
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        
        // Añadir un pequeño offset aleatorio para que no todos se muevan igual
        bobTimer = Random.Range(0f, Mathf.PI * 2f);
    }
    
    private void Update()
    {
        // Animación de "flotación" suave
        bobTimer += Time.deltaTime * 2f;
        float bobOffset = Mathf.Sin(bobTimer) * 0.05f;
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + bobOffset * 0.1f,
            transform.localPosition.z
        );
        
        // Pequeña rotación
        transform.localScale = originalScale * (1f + bobOffset * 0.1f);
    }
    
    public void OnHover()
    {
        // Efecto de hover
        transform.localScale = originalScale * 1.15f;
    }
    
    public void OnExit()
    {
        transform.localScale = originalScale;
    }
    
    public void OnClick()
    {
        // Animación de click
        StartCoroutine(ClickAnimation());
    }
    
    private System.Collections.IEnumerator ClickAnimation()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            transform.localScale = originalScale * scale;
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
}
