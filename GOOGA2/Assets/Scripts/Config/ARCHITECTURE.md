# Arquitectura del Sistema de Configuración GOOGAZ

```
┌─────────────────────────────────────────────────────────────────┐
│                         GOOGAZ - Sistema de Configuración        │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│   GameConfig.cs      │  ← Configuración global del juego
│  ┌────────────────┐  │
│  │ • Tiempo límite│  │
│  │ • Pesos score  │  │
│  │ • Audio config │  │
│  │ • Debug mode   │  │
│  └────────────────┘  │
└──────────────────────┘

         ↓ Usa

┌──────────────────────────────────────────────────────────────────┐
│                    BASES DE DATOS (ScriptableObjects)             │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌─────────────────────┐      ┌─────────────────────┐           │
│  │ IngredientConfig.cs │      │  RecipeConfig.cs    │           │
│  ├─────────────────────┤      ├─────────────────────┤           │
│  │ • Ingredientes[]    │◄─────┤ • Recetas[]         │           │
│  │ • Propiedades       │      │ • Ingredientes IDs  │           │
│  │ • Nutri-Score       │      │ • Valores nutri.    │           │
│  │ • Temporalidad      │      │ • Escenarios        │           │
│  └─────────────────────┘      └─────────────────────┘           │
│           ▲                            ▲                         │
│           │                            │                         │
│           │    ┌──────────────────────┐│                         │
│           │    │ ScenarioConfig.cs    ││                         │
│           │    ├──────────────────────┤│                         │
│           └────┤ • Escenarios[]       ││                         │
│                │ • Grupo objetivo     ││                         │
│                │ • Condición          ││                         │
│                │ • Requisitos nutri.  ││                         │
│                │ • Recetas referencia │┘                         │
│                └──────────────────────┘                          │
│                         ▲                                        │
└─────────────────────────┼────────────────────────────────────────┘
                          │
                          │ Referencia
                          │
┌─────────────────────────┴────────────────────────────────────────┐
│                    SISTEMAS DE JUEGO                              │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌─────────────────────┐      ┌─────────────────────┐           │
│  │ ScoringConfig.cs    │      │PlayerProgressConfig │           │
│  ├─────────────────────┤      ├─────────────────────┤           │
│  │ • Evaluar receta    │      │ • Estadísticas      │           │
│  │ • Criterios score   │      │ • Progreso escenas  │           │
│  │ • Pesos evaluación  │      │ • Logros            │           │
│  │ • Feedback visual   │      │ • Guardar/Cargar    │           │
│  └─────────────────────┘      └─────────────────────┘           │
│           │                            │                         │
└───────────┼────────────────────────────┼─────────────────────────┘
            │                            │
            ▼                            ▼
┌──────────────────────────────────────────────────────────────────┐
│                         GAME MANAGER                              │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  • Inicializar configs                                     │  │
│  │  • Gestionar flujo de juego                                │  │
│  │  • Fase de aprendizaje → Fase de cocina → Evaluación      │  │
│  │  • Actualizar progreso                                     │  │
│  └────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
```

## Flujo de Datos

```
INICIO DEL JUEGO
    │
    ├─► PlayerProgressConfig.CargarProgreso()
    │
    ├─► ScenarioConfig.GetUnlockedScenarios()
    │
    └─► Mostrar menú de escenarios
            │
            ▼
    SELECCIONAR ESCENARIO
            │
            ├─► RecipeConfig.GetRecipesForScenario()
            │
            └─► FASE DE APRENDIZAJE
                    │
                    └─► Mostrar recetas de referencia
                            │
                            ▼
                    FASE DE COCINA
                            │
                            ├─► Jugador selecciona ingredientes
                            │
                            └─► Jugador crea receta
                                    │
                                    ▼
                            EVALUACIÓN
                                    │
                                    ├─► ScoringConfig.EvaluarReceta()
                                    │       │
                                    │       ├─► Evaluar Nutri-Score
                                    │       ├─► Evaluar Adecuación
                                    │       └─► Evaluar Temporalidad
                                    │
                                    ├─► Mostrar feedback
                                    │
                                    └─► PlayerProgressConfig.ActualizarProgreso()
                                            │
                                            ├─► Guardar estadísticas
                                            ├─► Verificar logros
                                            └─► Desbloquear escenarios
```

## Relaciones entre Clases

```
GameConfig
    └─ (Configuración global independiente)

IngredientConfig
    └─ Ingredient[]
        ├─ IngredientType (enum)
        ├─ NutriScore (enum)
        ├─ Season (enum flags)
        └─ NutritionalProperty (enum flags)

RecipeConfig
    ├─ Recipe[]
    │   ├─ ingredientesIds → IngredientConfig
    │   └─ escenariosAdecuados → ScenarioConfig
    └─ [Referencia] → IngredientConfig

ScenarioConfig
    ├─ Scenario[]
    │   ├─ recetasReferenciaIds → RecipeConfig
    │   ├─ ingredientesBeneficiosos → IngredientConfig
    │   └─ ingredientesProhibidos → IngredientConfig
    ├─ [Referencia] → RecipeConfig
    └─ [Referencia] → IngredientConfig

ScoringConfig
    ├─ ScoringCriteria
    ├─ RecipeEvaluation (resultado)
    ├─ [Referencia] → IngredientConfig
    └─ [Referencia] → RecipeConfig

PlayerProgressConfig
    ├─ PlayerStats
    ├─ ScenarioProgress[]
    └─ [Referencia] → ScenarioConfig
```

## Enums Principales

```csharp
// Ingredientes
IngredientType: Verdura, Fruta, Proteina, Cereal, Lacteo, Legumbre, Fruto_Seco, Aceite, Especias

NutriScore: A(5), B(4), C(3), D(2), E(1)

Season (Flags): Enero, Febrero, Marzo, ..., Diciembre, TodoElAño

NutritionalProperty (Flags):
    - AntiEstres
    - Energetico
    - Proteico
    - Antioxidante
    - Hidratante
    - RicoEnFibra
    - BajoEnGrasa
    - RicoEnCalcio
    - RicoEnHierro
    - Omega3
    - VitaminaB
    - Magnesio
    - FacilDigestion

// Recetas
MealType: Desayuno, Comida, Cena, Snack

// Escenarios
TargetGroup: Embarazo, Adolescencia, Senectud, Deportista, Infantil, Adulto

Condition: Ninguna, Estres, Migrana, Deporte_Antes, Deporte_Despues, Anemia, Diabetes, Hipertension
```

## Métodos Clave por Clase

### IngredientConfig
- `GetIngredientById(string id)`
- `GetIngredientsByType(IngredientType tipo)`
- `GetSeasonalIngredients(int mes)`
- `GetIngredientsByProperty(NutritionalProperty prop)`

### RecipeConfig
- `GetRecipeById(string id)`
- `GetRecipesByMealType(MealType tipo)`
- `GetReferenceRecipes()`
- `GetRecipesForScenario(string escenarioId)`
- `ValidateRecipeIngredients()`

### ScenarioConfig
- `GetScenarioById(string id)`
- `GetScenariosByTargetGroup(TargetGroup grupo)`
- `GetUnlockedScenarios()`
- `GetScenariosInOrder()`

### ScoringConfig
- `EvaluarReceta(Recipe, Scenario, int mes, float tiempo)`
- `GetColorPorPuntuacion(float puntuacion)`
- `GetMensajePorPuntuacion(float puntuacion)`

### PlayerProgressConfig
- `InicializarProgreso()`
- `ActualizarProgresoEscenario(string id, RecipeEvaluation, float tiempo)`
- `GuardarProgreso()` / `CargarProgreso()`
- `GetPorcentajeCompletitud()`

## Orden de Creación Recomendado

1. **GameConfig** (independiente)
2. **IngredientConfig** (base de todo)
3. **RecipeConfig** (depende de Ingredient)
4. **ScenarioConfig** (depende de Recipe e Ingredient)
5. **ScoringConfig** (depende de Recipe e Ingredient)
6. **PlayerProgressConfig** (depende de Scenario)

## Validaciones Automáticas

Cada ScriptableObject valida sus datos en `OnValidate()`:

✓ IDs únicos (sin duplicados)
✓ Referencias válidas entre bases de datos
✓ Pesos de puntuación suman 100%
✓ Ingredientes existen en recetas
✓ Recetas existen en escenarios

---

**Nota:** Este sistema está diseñado para ser extensible. Puedes añadir nuevos ingredientes, recetas y escenarios sin modificar el código, solo creando nuevos datos en Unity.
