# 🎮 Configuración Paso a Paso en Unity

## 📋 Requisitos Previos

- Unity 2020.3 o superior
- TextMeshPro instalado (se instala automáticamente al usar el script)

## 🚀 Configuración Rápida (5 minutos)

### Paso 1: Preparar la Escena

1. **Abrir Unity**
   - Abre tu proyecto GOOGA2

2. **Crear o Abrir Escena**
   - Si tienes una escena existente: Ábrela
   - Si no: `File > New Scene` y guárdala como "GameScene"

3. **Limpiar la Escena (Opcional)**
   - Elimina objetos innecesarios
   - Mantén solo la cámara principal si existe

### Paso 2: Configurar el GameManager

1. **Crear GameObject Vacío**
   ```
   Hierarchy > Click derecho > Create Empty
   Nombre: "GameManager"
   ```

2. **Añadir Script Principal**
   ```
   Selecciona GameManager
   Inspector > Add Component
   Busca: "NutritionGame2D"
   Click en el script
   ```

3. **Verificar Configuración**
   ```
   En el Inspector deberías ver:
   - Main Camera: (None) ← Se auto-configura
   - Ingredient Prefab: (None) ← Opcional
   - Particle Prefab: (None) ← Opcional
   - Hud Canvas: (None) ← Se auto-crea
   - Title Text: (None) ← Se auto-crea
   - Score Text: (None) ← Se auto-crea
   - Instructions Text: (None) ← Se auto-crea
   ```

### Paso 3: ¡Ejecutar!

1. **Presiona Play** ▶️

2. **Deberías Ver:**
   - Fondo estrellado
   - Título "GOOGAZ" arriba
   - 3 botones de escenarios
   - Instrucciones abajo

3. **Si algo falla:**
   - Revisa la consola (Ctrl+Shift+C)
   - Verifica que los scripts están en `Assets/Scripts/`
   - Asegúrate de que no hay errores de compilación

## 🎨 Configuración Avanzada (Opcional)

### Añadir Efectos de Cámara

1. **Seleccionar Main Camera**
   ```
   Hierarchy > Main Camera
   ```

2. **Añadir Componente**
   ```
   Inspector > Add Component
   Busca: "CameraEffects2D"
   ```

3. **Configurar (Opcional)**
   ```
   Puedes ajustar:
   - Shake Intensity
   - Zoom Speed
   ```

### Añadir Sistema de Partículas

1. **Crear GameObject**
   ```
   Hierarchy > Create Empty
   Nombre: "ParticleManager"
   ```

2. **Añadir Componente**
   ```
   Inspector > Add Component
   Busca: "ParticleSystem2D"
   ```

### Añadir Efectos Integrados

1. **Seleccionar GameManager**

2. **Añadir Componente**
   ```
   Inspector > Add Component
   Busca: "GameEffectsExample"
   ```

3. **Esto añadirá automáticamente:**
   - Shake de cámara al seleccionar
   - Partículas doradas
   - Efectos especiales

## 🔧 Configuración Manual de Cámara (Si es necesario)

Si la cámara no se configura automáticamente:

1. **Seleccionar Main Camera**

2. **Configurar Proyección**
   ```
   Inspector > Camera
   - Projection: Orthographic
   - Size: 5
   - Position: (0, 0, -10)
   - Background: Color oscuro
   ```

3. **Verificar Tag**
   ```
   Tag: MainCamera
   ```

## 📱 Configuración de Canvas (Si es necesario)

Si el HUD no aparece:

1. **Crear Canvas**
   ```
   Hierarchy > UI > Canvas
   Nombre: "HUD Canvas"
   ```

2. **Configurar Canvas**
   ```
   Inspector > Canvas
   - Render Mode: Screen Space - Overlay
   ```

3. **Añadir Canvas Scaler**
   ```
   Inspector > Canvas Scaler
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080
   ```

4. **Arrastrar al Script**
   ```
   Selecciona GameManager
   Arrastra el Canvas al campo "Hud Canvas"
   ```

## 🎯 Verificación de Configuración

### Checklist Completo

- [ ] GameManager existe en la escena
- [ ] NutritionGame2D.cs está adjunto
- [ ] No hay errores en la consola
- [ ] Al presionar Play aparece el menú
- [ ] Los botones responden a clicks
- [ ] El fondo estrellado es visible
- [ ] El texto del HUD es legible

### Jerarquía Esperada (Después de Play)

```
Hierarchy:
├── GameManager (NutritionGame2D)
├── Main Camera (CameraEffects2D - opcional)
├── ParticleManager (ParticleSystem2D - opcional)
├── HUD Canvas
│   ├── Title
│   ├── Instructions
│   └── Score
├── Background_Starry
├── Button_GIMNASIO
├── Button_INSTITUTO
└── Button_CASA ABUELOS
```

## 🐛 Solución de Problemas

### Problema: "NullReferenceException"

**Causa:** Falta algún componente
**Solución:**
```
1. Detén el juego
2. Verifica que todos los scripts están en Assets/Scripts/
3. Recompila (Ctrl+R)
4. Vuelve a ejecutar
```

### Problema: "No se ve nada"

**Causa:** Cámara mal configurada
**Solución:**
```
1. Selecciona Main Camera
2. Position: (0, 0, -10)
3. Projection: Orthographic
4. Size: 5
5. Clear Flags: Solid Color
```

### Problema: "Los botones no responden"

**Causa:** Falta EventSystem
**Solución:**
```
1. Hierarchy > Create > UI > Event System
2. Vuelve a ejecutar
```

### Problema: "Sprites borrosos"

**Causa:** Configuración de calidad
**Solución:**
```
1. Edit > Project Settings > Quality
2. Anti Aliasing: Disabled
3. Texture Quality: Full Res
```

## 📊 Configuración de Build (Para Exportar)

### Windows

```
File > Build Settings
- Platform: Windows
- Architecture: x86_64
- Target: Standalone
Build
```

### WebGL

```
File > Build Settings
- Platform: WebGL
- Compression Format: Gzip
Switch Platform
Build
```

### Android

```
File > Build Settings
- Platform: Android
- Minimum API Level: 21
- Target API Level: 30
Build
```

## 🎨 Personalización Post-Configuración

### Cambiar Resolución de Referencia

```csharp
// En NutritionGame2D.cs, método SetupHUD()
scaler.referenceResolution = new Vector2(1280, 720); // HD
// o
scaler.referenceResolution = new Vector2(2560, 1440); // 2K
```

### Cambiar Tamaño de Cámara

```csharp
// En NutritionGame2D.cs, método SetupCamera()
mainCamera.orthographicSize = 7f; // Más zoom out
// o
mainCamera.orthographicSize = 3f; // Más zoom in
```

### Cambiar Color de Fondo

```csharp
// En NutritionGame2D.cs, método SetupCamera()
mainCamera.backgroundColor = new Color(0.2f, 0.1f, 0.3f); // Morado oscuro
```

## 📝 Notas Importantes

1. **TextMeshPro**
   - La primera vez que uses TMP, Unity te pedirá importar recursos
   - Click en "Import TMP Essentials"
   - Esto es normal y solo pasa una vez

2. **Rendimiento**
   - El juego genera sprites proceduralmente
   - Esto puede tardar un momento la primera vez
   - Luego es muy rápido

3. **Compatibilidad**
   - Funciona en Unity 2020.3+
   - Compatible con URP y Built-in
   - Funciona en todas las plataformas

## 🎓 Próximos Pasos

Después de configurar:

1. **Juega el juego completo**
   - Prueba los 3 escenarios
   - Verifica que todo funciona

2. **Personaliza**
   - Cambia colores en `GetIngredientColor()`
   - Modifica sprites en `SpriteGenerator.cs`
   - Ajusta animaciones en `Ingredient2D.cs`

3. **Expande**
   - Añade más ingredientes
   - Crea nuevos escenarios
   - Implementa nuevas mecánicas

## ✅ Configuración Completada

Si todo funciona:

✅ Menú principal visible
✅ Botones interactivos
✅ Fase de aprendizaje funcional
✅ Tienda con ingredientes
✅ Sistema de puntuación
✅ Resultados mostrados

**¡Felicidades! Tu juego 2D está listo.** 🎉

---

**¿Problemas?** Revisa la sección de solución de problemas
**¿Dudas?** Consulta README_2D.md
**¿Listo para más?** Experimenta con los efectos! 🚀
