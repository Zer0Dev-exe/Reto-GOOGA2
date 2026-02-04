# 🚀 Guía de Inicio Rápido - GOOGAZ

## ¿Ves solo una pantalla azul en Unity?

¡No te preocupes! Acabamos de crear el sistema de configuración. Ahora necesitas seguir estos pasos para ver el juego funcionando:

---

## 📋 Pasos para configurar Unity

### 1️⃣ Instalar TextMeshPro (si no lo tienes)

Si Unity te muestra un error sobre `TMPro`, necesitas importar TextMeshPro:

1. Ve a **Window > TextMeshPro > Import TMP Essential Resources**
2. Haz clic en **Import**

---

### 2️⃣ Crear los ScriptableObjects de Configuración

Necesitas crear los archivos de configuración en Unity:

#### a) GameConfig
1. En el **Project panel**, haz clic derecho en `Assets`
2. Selecciona **Create > GOOGAZ > Config > Game Config**
3. Nómbralo `GameConfig`

#### b) IngredientDatabase
1. Clic derecho en `Assets`
2. **Create > GOOGAZ > Config > Ingredient Database**
3. Nómbralo `IngredientDatabase`

#### c) RecipeDatabase
1. Clic derecho en `Assets`
2. **Create > GOOGAZ > Config > Recipe Database**
3. Nómbralo `RecipeDatabase`
4. En el Inspector, arrastra `IngredientDatabase` al campo **Ingredient Database**

#### d) ScenarioDatabase
1. Clic derecho en `Assets`
2. **Create > GOOGAZ > Config > Scenario Database**
3. Nómbralo `ScenarioDatabase`
4. Arrastra `RecipeDatabase` e `IngredientDatabase` a sus campos

#### e) ScoringConfig
1. Clic derecho en `Assets`
2. **Create > GOOGAZ > Config > Scoring System**
3. Nómbralo `ScoringConfig`
4. Arrastra `RecipeDatabase` e `IngredientDatabase`

#### f) PlayerProgressConfig
1. Clic derecho en `Assets`
2. **Create > GOOGAZ > Config > Player Progress**
3. Nómbralo `PlayerProgressConfig`
4. Arrastra `ScenarioDatabase`

---

### 3️⃣ Crear la UI Básica

Hay dos opciones:

#### Opción A: Automática (Recomendada)
1. En Unity, ve al menú **GameObject > GOOGAZ > Setup Basic UI**
2. Esto creará automáticamente toda la UI y el GameManager

#### Opción B: Manual
1. Abre la escena `SampleScene`
2. Crea un GameObject vacío llamado `GameManager`
3. Añade el componente `GameManager` (script)
4. Crea la UI manualmente (Canvas, textos, botones)

---

### 4️⃣ Asignar las Configuraciones al GameManager

1. Selecciona el GameObject **GameManager** en la jerarquía
2. En el **Inspector**, verás campos vacíos para las configuraciones
3. Arrastra cada ScriptableObject creado a su campo correspondiente:
   - **Game Config** → `GameConfig`
   - **Ingredient DB** → `IngredientDatabase`
   - **Recipe DB** → `RecipeDatabase`
   - **Scenario DB** → `ScenarioDatabase`
   - **Scoring System** → `ScoringConfig`
   - **Player Progress** → `PlayerProgressConfig`

---

### 5️⃣ ¡Ejecutar el Juego!

1. Presiona el botón **Play** ▶️ en Unity
2. Deberías ver:
   - Título "GOOGAZ"
   - Estado del sistema
   - Botón "INICIAR"

---

## 🎨 Personalizar el Juego

### Añadir Ingredientes

1. Selecciona `IngredientDatabase` en el Project panel
2. En el Inspector, expande **Ingredientes**
3. Aumenta el **Size** para añadir nuevos ingredientes
4. Configura cada ingrediente:
   - **ID**: `tomate` (único, snake_case)
   - **Nombre**: `Tomate`
   - **Tipo**: `Verdura`
   - **Nutri Score**: `A`
   - **Temporada**: Selecciona los meses
   - **Propiedades**: Marca las casillas relevantes

### Añadir Recetas

1. Selecciona `RecipeDatabase`
2. Añade recetas en el array **Recetas**
3. Configura:
   - **ID**: `ensalada_tomate`
   - **Nombre**: `Ensalada de Tomate`
   - **Tipo Comida**: `Comida`
   - **Ingredientes IDs**: Añade IDs de ingredientes (ej: `tomate`, `lechuga`)

### Crear Escenarios

1. Selecciona `ScenarioDatabase`
2. Añade escenarios basados en el README:
   - **Embarazo y Deporte**
   - **Adolescencia y Estrés**
   - **Senectud (Migraña)**

---

## 🐛 Solución de Problemas

### Error: "TMPro namespace not found"
- Importa TextMeshPro: **Window > TextMeshPro > Import TMP Essential Resources**

### Error: "GameConfig no asignado"
- Crea los ScriptableObjects siguiendo el paso 2
- Asígnalos al GameManager (paso 4)

### La pantalla sigue azul
- Verifica que la escena `SampleScene` esté abierta
- Asegúrate de haber creado la UI (paso 3)
- Revisa la consola de Unity para ver mensajes de error

### No aparece el menú "GOOGAZ" en GameObject
- El script `UISetup.cs` debe estar en la carpeta `Assets/Scripts/Editor/`
- Espera a que Unity compile los scripts
- Si no aparece, reinicia Unity

---

## 📚 Próximos Pasos

1. **Poblar las bases de datos** con ingredientes y recetas del README
2. **Crear los 3 escenarios** principales
3. **Implementar la UI de juego** (fase de aprendizaje, cocina, evaluación)
4. **Añadir gráficos** (sprites para ingredientes, recetas, etc.)
5. **Implementar el sistema de puntuación** visual

---

## 🎯 Estructura del Proyecto

```
Assets/
├── Scripts/
│   ├── Config/
│   │   ├── GameConfig.cs
│   │   ├── IngredientConfig.cs
│   │   ├── RecipeConfig.cs
│   │   ├── ScenarioConfig.cs
│   │   ├── ScoringConfig.cs
│   │   └── PlayerProgressConfig.cs
│   ├── Editor/
│   │   └── UISetup.cs
│   └── GameManager.cs
├── Scenes/
│   └── SampleScene.unity
└── [ScriptableObjects aquí]
    ├── GameConfig.asset
    ├── IngredientDatabase.asset
    ├── RecipeDatabase.asset
    ├── ScenarioDatabase.asset
    ├── ScoringConfig.asset
    └── PlayerProgressConfig.asset
```

---

## 💡 Consejos

- **Guarda frecuentemente** tu escena y proyecto (Ctrl+S)
- **Revisa la consola** para ver mensajes del sistema
- **Lee los comentarios** en el código para entender cada parte
- **Consulta README.md** en la carpeta Config para más detalles

---

¡Listo para empezar a crear tu juego de nutrición! 🎮🥗
