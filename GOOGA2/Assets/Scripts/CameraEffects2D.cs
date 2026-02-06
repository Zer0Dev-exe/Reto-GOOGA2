using UnityEngine;

/// <summary>
/// Efectos de cámara para el juego 2D
/// Añade shake, zoom y transiciones suaves
/// </summary>
public class CameraEffects2D : MonoBehaviour
{
    private Camera cam;
    private Vector3 originalPosition;
    private float originalSize;
    
    // Shake
    private float shakeIntensity = 0f;
    private float shakeDuration = 0f;
    private float shakeTimer = 0f;
    
    // Zoom
    private float targetSize;
    private float zoomSpeed = 2f;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        originalPosition = transform.position;
        originalSize = cam.orthographicSize;
        targetSize = originalSize;
    }
    
    private void Update()
    {
        // Aplicar shake
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            
            transform.position = originalPosition + new Vector3(x, y, 0);
            
            if (shakeTimer <= 0f)
            {
                transform.position = originalPosition;
            }
        }
        
        // Aplicar zoom suave
        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }
    }
    
    /// <summary>
    /// Sacudir la cámara
    /// </summary>
    public void Shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }
    
    /// <summary>
    /// Hacer zoom
    /// </summary>
    public void ZoomTo(float size, float speed = 2f)
    {
        targetSize = size;
        zoomSpeed = speed;
    }
    
    /// <summary>
    /// Resetear zoom
    /// </summary>
    public void ResetZoom()
    {
        targetSize = originalSize;
    }
    
    /// <summary>
    /// Seguir un objetivo suavemente
    /// </summary>
    public void FollowTarget(Vector3 targetPosition, float smoothSpeed = 5f)
    {
        Vector3 desiredPosition = new Vector3(targetPosition.x, targetPosition.y, originalPosition.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        originalPosition = transform.position;
    }
}
