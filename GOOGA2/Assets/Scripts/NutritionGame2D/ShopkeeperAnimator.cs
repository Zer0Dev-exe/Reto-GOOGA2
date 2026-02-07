using UnityEngine;

/// <summary>
/// Animación sutil de flotación del tendero
/// </summary>
public class ShopkeeperAnimator : MonoBehaviour
{
    private void Update() { transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.002f, 0); }
}
