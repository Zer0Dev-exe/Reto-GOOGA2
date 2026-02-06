# Guía de Migración: De NutritionGame.cs a NutritionGame2D.cs

## Pasos para cambiar al nuevo sistema 2D

### Opción 1: Reemplazar Completamente (Recomendado)

1. **En Unity:**
   - Abre tu escena principal
   - Encuentra el GameObject que tiene el script `NutritionGame.cs`
   - Elimina el componente `NutritionGame.cs`
   - Añade el componente `NutritionGame2D.cs`
   - Presiona Play

2. **El script se encargará de:**
   - Crear la cámara automáticamente
   - Configurar el HUD
   - Generar todos los sprites
   - Gestionar las interacciones

### Opción 2: Mantener Ambos (Para Comparar)

1. **Crear dos escenas:**
   - Escena 1: "GameScene_UI" con `NutritionGame.cs`
   - Escena 2: "GameScene_2D" con `NutritionGame2D.cs`

2. **Comparar:**
   - Ejecuta cada escena para ver las diferencias
   - Decide cuál prefieres

### Opción 3: Renombrar el Antiguo

1. **Renombrar archivo:**
   ```
   NutritionGame.cs → NutritionGame_OLD.cs
   ```

2. **En el script, cambiar:**
   ```csharp
   public class NutritionGame_OLD : MonoBehaviour
   ```

3. **Usar el nuevo:**
   - Añade `NutritionGame2D.cs` a tu GameObject

## Diferencias Clave en el Código

### Sistema de Coordenadas

**Antiguo (UI):**
```csharp
// Posiciones en anchors (0-1)
rect.anchorMin = new Vector2(0.5f, 0.5f);
rect.anchorMax = new Vector2(0.5f, 0.5f);
rect.sizeDelta = new Vector2(200, 100);
```

**Nuevo (2D):**
```csharp
// Posiciones en mundo 2D
transform.position = new Vector3(0, 0, 0);
transform.localScale = new Vector3(1, 1, 1);
```

### Creación de Elementos

**Antiguo (UI):**
```csharp
GameObject btnGO = new GameObject("Button");
Image img = btnGO.AddComponent<Image>();
Button btn = btnGO.AddComponent<Button>();
```

**Nuevo (2D):**
```csharp
GameObject spriteGO = new GameObject("Sprite");
SpriteRenderer sr = spriteGO.AddComponent<SpriteRenderer>();
BoxCollider2D collider = spriteGO.AddComponent<BoxCollider2D>();
```

### Detección de Clicks

**Antiguo (UI):**
```csharp
btn.onClick.AddListener(() => DoSomething());
```

**Nuevo (2D):**
```csharp
void OnMouseDown()
{
    DoSomething();
}
// O usando Raycast:
Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
```

## Ventajas del Nuevo Sistema

✅ **Aspecto de videojuego real** en lugar de página web
✅ **Animaciones fluidas** con física 2D
✅ **Partículas y efectos** visuales
✅ **Sprites personalizados** para cada ingrediente
✅ **Mejor rendimiento** (menos overhead de UI)
✅ **Más fácil de expandir** con mecánicas de juego

## Desventajas (y cómo mitigarlas)

❌ **Más complejo** → Documentación incluida
❌ **Requiere entender 2D** → Ejemplos claros en el código
❌ **Sprites procedurales básicos** → Fácil reemplazar con arte real

## Migración de Datos

Los datos del juego (escenarios, ingredientes) son **idénticos** en ambas versiones:

```csharp
private Scenario[] scenarios = new Scenario[] { ... };
private string[] availableIngredients = new string[] { ... };
```

Puedes copiar y pegar estos arrays entre versiones sin cambios.

## Checklist de Migración

- [ ] Backup de la escena actual
- [ ] Crear nueva escena o duplicar existente
- [ ] Añadir `NutritionGame2D.cs` al GameManager
- [ ] Eliminar `NutritionGame.cs` del GameManager
- [ ] Verificar que no hay errores en consola
- [ ] Probar el menú principal
- [ ] Probar la fase de aprendizaje
- [ ] Probar la fase de compra
- [ ] Probar los resultados
- [ ] Ajustar colores/tamaños si es necesario

## Solución de Problemas Comunes

### "No veo nada en pantalla"

**Solución:**
- Verifica que la cámara está en posición (0, 0, -10)
- Asegúrate de que es ortográfica
- Comprueba que el tamaño ortográfico es 5

### "Los sprites están muy grandes/pequeños"

**Solución:**
```csharp
// En NutritionGame2D.cs, ajusta las escalas:
sr.transform.localScale = new Vector3(0.5f, 0.5f, 1f); // Más pequeño
sr.transform.localScale = new Vector3(2f, 2f, 1f); // Más grande
```

### "Los clicks no funcionan"

**Solución:**
- Verifica que los objetos tienen Collider2D
- Asegúrate de que la cámara tiene el tag "MainCamera"
- Comprueba que no hay UI bloqueando los raycast

### "Los colores se ven mal"

**Solución:**
```csharp
// Ajusta en GetIngredientColor():
return new Color(1f, 0.5f, 0.0f); // RGB entre 0 y 1
```

## Próximos Pasos

Después de migrar exitosamente:

1. **Personaliza los sprites** en `SpriteGenerator.cs`
2. **Ajusta las animaciones** en `Ingredient2D.cs`
3. **Añade sonidos** (AudioSource.PlayClipAtPoint)
4. **Crea más efectos** de partículas
5. **Expande el juego** con nuevas mecánicas

---

¿Necesitas ayuda? Revisa el código - está bien comentado! 💪
