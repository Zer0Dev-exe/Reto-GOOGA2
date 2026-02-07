using UnityEngine;

/// <summary>
/// Componente de ingrediente seleccionable en la tienda
/// </summary>
public class IngredientItem : MonoBehaviour
{
    public string ingredientName;
    public Color color;
    public System.Action<IngredientItem> onSelect;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        transform.localScale = originalScale * 1.2f;
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    private void OnMouseDown()
    {
        onSelect?.Invoke(this);
    }
}
