# GOOGAZ - Juego de Nutrición 2D

## 🎮 Cambios Principales: De HTML/UI a Juego 2D Real

### ¿Qué ha cambiado?

**ANTES:**
- ❌ Interfaz basada en Canvas UI (parecía HTML)
- ❌ Emojis como "sprites"
- ❌ Botones planos sin vida
- ❌ Sin animaciones ni efectos visuales

**AHORA:**
- ✅ Juego 2D real con sprites
- ✅ Sprites procedurales pixel art para cada ingrediente
- ✅ Personajes 2D animados (vendedor, jugador)
- ✅ Partículas y efectos visuales
- ✅ Animaciones de flotación, hover y selección
- ✅ Fondos 2D con diferentes escenarios

## 📁 Archivos Nuevos

### Scripts Principales

1. **NutritionGame2D.cs** - Script principal del juego 2D
   - Gestiona todas las fases del juego
   - Crea sprites procedurales para fondos
   - Maneja la interacción con ingredientes
   - Sistema de partículas para efectos visuales

2. **SpriteGenerator.cs** - Generador de sprites procedurales
   - Crea sprites únicos para cada tipo de ingrediente
   - Diferentes formas: granos, pescados, verduras, frutas, etc.
   - Estilo pixel art con bordes negros
   - Colores apropiados para cada alimento

3. **Ingredient2D.cs** - Componente para ingredientes
   - Animación de flotación suave
   - Efectos de hover (agrandar al pasar el ratón)
   - Animación de click
   - Gestión de sprites

## 🎯 Cómo Usar

### Configuración en Unity

1. **Crear una escena nueva o usar la existente**

2. **Añadir el script principal:**
   - Crea un GameObject vacío llamado "GameManager"
   - Arrastra el script `NutritionGame2D.cs` al GameObject
   - El script creará automáticamente la cámara y el HUD si no existen

3. **Ejecutar el juego:**
   - Presiona Play en Unity
   - Verás el menú principal con 3 botones de escenarios
   - Los botones son sprites 2D, no UI Canvas

### Controles

**Menú Principal:**
- Haz clic en cualquier escenario para comenzar

**Fase de Aprendizaje:**
- Lee las recetas en la nota
- Presiona **ESPACIO** para ir a la tienda

**Fase de Compra:**
- Haz clic en los ingredientes para añadirlos a tu cesta
- Verás partículas doradas cuando selecciones un ingrediente
- Los ingredientes seleccionados aparecen en la cesta (derecha)
- Presiona **ENTER** para finalizar y ver resultados

**Resultados:**
- Presiona **R** para reintentar el mismo escenario
- Presiona **M** para volver al menú principal

## 🎨 Características Visuales

### Sprites Procedurales

Cada ingrediente tiene su propio sprite generado proceduralmente:

- **Granos** (avena, quinoa, arroz): Forma ovalada con textura de granos
- **Pescados** (salmón, merluza): Forma de pez con escamas y ojo
- **Verduras redondas** (tomate, calabaza): Círculo con brillo y tallo
- **Zanahoria**: Forma triangular con hojas verdes
- **Frutas**: Círculo con brillo y hoja
- **Frutos secos**: Forma ovalada con textura
- **Lácteos**: Cubo con agujeros (queso) o forma simple

### Animaciones

- **Flotación**: Todos los ingredientes flotan suavemente
- **Hover**: Se agrandan al pasar el ratón
- **Click**: Animación de pulso al seleccionar
- **Partículas**: Efectos dorados al seleccionar ingredientes
- **Vendedor**: Animación de idle (respiración)

### Fondos

- **Menú**: Cielo estrellado con gradiente morado-azul
- **Aprendizaje**: Pizarra verde oscuro
- **Tienda**: Suelo de baldosas y pared beige

## 🔧 Personalización

### Cambiar Colores de Ingredientes

Edita el método `GetIngredientColor()` en `NutritionGame2D.cs`:

```csharp
private Color GetIngredientColor(string ingredient)
{
    if (ingredient == "tomate")
        return new Color(0.9f, 0.2f, 0.2f); // Rojo
    // ... más ingredientes
}
```

### Modificar Sprites

Edita `SpriteGenerator.cs` para cambiar cómo se dibujan los ingredientes:

```csharp
private static void DrawFish(Color[] pixels, int size, Color baseColor)
{
    // Modifica aquí la forma del pescado
}
```

### Ajustar Animaciones

En `Ingredient2D.cs`, modifica los parámetros:

```csharp
bobTimer += Time.deltaTime * 2f; // Velocidad de flotación
float bobOffset = Mathf.Sin(bobTimer) * 0.05f; // Amplitud
```

## 🎮 Diferencias con la Versión Anterior

| Aspecto | Versión Antigua (UI) | Versión Nueva (2D) |
|---------|---------------------|-------------------|
| Ingredientes | Emojis en botones UI | Sprites 2D animados |
| Interacción | Click en botones | Raycast 2D en sprites |
| Fondos | Colores planos | Sprites procedurales |
| Animaciones | Ninguna | Flotación, hover, partículas |
| Personajes | Ninguno | Vendedor animado |
| Estilo | HTML/Web | Videojuego 2D |

## 🚀 Próximas Mejoras Sugeridas

1. **Sonidos**: Añadir efectos de sonido al seleccionar ingredientes
2. **Música**: Música de fondo para cada fase
3. **Más animaciones**: Transiciones entre escenas
4. **Sprites artísticos**: Reemplazar sprites procedurales con arte dibujado
5. **Jugador**: Añadir un personaje jugador que se mueva
6. **Diálogos**: Sistema de diálogos con el vendedor
7. **Power-ups**: Elementos especiales en la tienda
8. **Minijuegos**: Pequeños desafíos al seleccionar ingredientes

## 📝 Notas Técnicas

- **Resolución**: El juego usa una cámara ortográfica con tamaño 5
- **Sorting Order**: Fondos (-10), Objetos (1-3), Partículas (10)
- **Filtro de Texturas**: FilterMode.Point para estilo pixel art
- **Colliders**: CircleCollider2D para ingredientes, BoxCollider2D para botones
- **Canvas**: Solo se usa para el HUD (texto), no para el juego principal

## 🐛 Solución de Problemas

**Los ingredientes no responden a clicks:**
- Asegúrate de que tienen un Collider2D
- Verifica que la cámara es la Main Camera

**Los sprites se ven borrosos:**
- Verifica que FilterMode está en Point
- Ajusta el PPU (Pixels Per Unit) en los sprites

**Las animaciones van muy rápido/lento:**
- Ajusta los multiplicadores de Time.deltaTime en los scripts

## 👨‍💻 Autor

Juego creado para el proyecto GOOGAZ - Educación Nutricional

---

¡Disfruta del juego! 🎮🥗
