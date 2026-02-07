using UnityEngine;

/// <summary>
/// Componente de botón de menú con hover y click en mundo 2D
/// </summary>
public class MenuButton : MonoBehaviour
{
    public System.Action onClick;
    public GameObject buttonText;
    public Color originalColor;
    public GameObject hoverGlow;
    private Vector3 basePos;
    private bool isHovered = false;

    void Start() { basePos = transform.position; }

    private void Update() {
        if (isHovered) {
            transform.position = basePos + new Vector3(0, Mathf.Sin(Time.time * 8) * 0.15f, 0);
        } else {
            transform.position = Vector3.Lerp(transform.position, basePos, Time.deltaTime * 10);
        }
    }

    private void OnMouseEnter() { 
        isHovered = true;
        if (hoverGlow != null) hoverGlow.SetActive(true);
    }
    private void OnMouseExit() { 
        isHovered = false;
        if (hoverGlow != null) hoverGlow.SetActive(false);
    }
    private void OnMouseDown() { onClick?.Invoke(); }
}
