# Sistema de Configuración GOOGAZ

Este directorio contiene los archivos de configuración principales del juego GOOGAZ. Todos los archivos son **ScriptableObjects** de Unity que permiten crear y gestionar datos del juego de forma visual en el editor.

## 📁 Archivos de Configuración

### 1. **GameConfig.cs**
**Propósito:** Configuración general del juego y parámetros globales.

**Cómo crear:**
1. En Unity: `Assets > Create > GOOGAZ > Config > Game Config`
2. Configurar parámetros como:
   - Tiempo límite de retos
   - Pesos de puntuación (Nutri-Score, Adecuación, Temporalidad)
   - Configuración de audio
   - Modo debug

**Validaciones automáticas:**
- Verifica que los pesos de puntuación sumen 100%

---

### 2. **IngredientConfig.cs**
**Propósito:** Base de datos de ingredientes con propiedades nutricionales.

**Cómo crear:**
1. En Unity: `Assets > Create > GOOGAZ > Config > Ingredient Database`
2. Añadir ingredientes con:
   - Información básica (nombre, descripción, icono)
   - Tipo de ingrediente (Verdura, Fruta, Proteína, etc.)
   - Nutri-Score (A-E)
   - Temporada (meses en los que está disponible)
   - Propiedades nutricionales (Anti-estrés, Energético, etc.)
   - Valores nutricionales (calorías, proteínas, etc.)

**Enums importantes:**
- `IngredientType`: Verdura, Fruta, Proteina, Cereal, Lacteo, Legumbre, Fruto_Seco, Aceite, Especias
- `NutriScore`: A (mejor) a E (peor)
- `Season`: Meses del año (con flags para múltiples meses)
- `NutritionalProperty`: Propiedades especiales (AntiEstres, Energetico, Proteico, etc.)

**Métodos útiles:**
```csharp
// Buscar ingrediente por ID
Ingredient ing = ingredientDB.GetIngredientById("tomate");

// Obtener ingredientes de temporada
List<Ingredient> temporada = ingredientDB.GetSeasonalIngredients(6); // Junio

// Filtrar por propiedad
List<Ingredient> antiEstres = ingredientDB.GetIngredientsByProperty(NutritionalProperty.AntiEstres);
```

---

### 3. **RecipeConfig.cs**
**Propósito:** Base de datos de recetas con sus ingredientes y propiedades.

**Cómo crear:**
1. En Unity: `Assets > Create > GOOGAZ > Config > Recipe Database`
2. Asignar referencia a `IngredientConfig`
3. Añadir recetas con:
   - Información básica (nombre, descripción, imagen)
   - Tipo de comida (Desayuno, Comida, Cena)
   - IDs de ingredientes que la componen
   - Escenarios para los que es adecuada
   - Valores nutricionales totales

**Métodos útiles:**
```csharp
// Buscar receta
Recipe receta = recipeDB.GetRecipeById("bowl_quinoa");

// Recetas de referencia para aprendizaje
List<Recipe> referencias = recipeDB.GetReferenceRecipes();

// Recetas para un escenario
List<Recipe> recetas = recipeDB.GetRecipesForScenario("embarazo_deporte");
```

---

### 4. **ScenarioConfig.cs**
**Propósito:** Configuración de escenarios de juego (retos).

**Cómo crear:**
1. En Unity: `Assets > Create > GOOGAZ > Config > Scenario Database`
2. Asignar referencias a `RecipeConfig` e `IngredientConfig`
3. Crear escenarios con:
   - Grupo objetivo (Embarazo, Adolescencia, Senectud, etc.)
   - Condición (Estrés, Migraña, Deporte, etc.)
   - Situación y notas clave
   - Propiedades nutricionales requeridas/prohibidas
   - Recetas de referencia
   - Ingredientes beneficiosos/prohibidos

**Ejemplo de escenario:**
```
ID: "adolescencia_estres"
Grupo: Adolescencia
Condición: Estrés
Propiedades Requeridas: AntiEstres | Magnesio | VitaminaB
Recetas Referencia: ["bowl_avena_fresas", "crema_merluza", "ensalada_verano"]
```

**Métodos útiles:**
```csharp
// Obtener escenario
Scenario escenario = scenarioDB.GetScenarioById("adolescencia_estres");

// Escenarios desbloqueados
List<Scenario> desbloqueados = scenarioDB.GetUnlockedScenarios();

// Escenarios por dificultad
List<Scenario> faciles = scenarioDB.GetScenariosByDifficulty(1);
```

---

### 5. **ScoringConfig.cs**
**Propósito:** Sistema de puntuación y evaluación de recetas.

**Cómo crear:**
1. En Unity: `Assets > Create > GOOGAZ > Config > Scoring System`
2. Asignar referencias a `IngredientConfig` y `RecipeConfig`
3. Configurar:
   - Criterios de puntuación
   - Pesos de evaluación
   - Bonus y penalizaciones
   - Colores de feedback

**Uso principal:**
```csharp
// Evaluar una receta
RecipeEvaluation eval = scoringConfig.EvaluarReceta(
    receta, 
    escenario, 
    mesActual: 6,  // Junio
    tiempoRestante: 45f
);

// Resultados
Debug.Log($"Puntuación: {eval.puntuacionTotal}");
Debug.Log($"Aprobado: {eval.aprobado}");
Debug.Log($"Aciertos: {string.Join(", ", eval.aciertos)}");
Debug.Log($"Errores: {string.Join(", ", eval.errores)}");

// Feedback visual
Color color = scoringConfig.GetColorPorPuntuacion(eval.puntuacionTotal);
string mensaje = scoringConfig.GetMensajePorPuntuacion(eval.puntuacionTotal);
```

**Criterios de evaluación:**
1. **Nutri-Score** (40%): Calidad nutricional objetiva
2. **Adecuación** (40%): Cumplimiento de requisitos del escenario
3. **Temporalidad** (20%): Uso de ingredientes de temporada

---

### 6. **PlayerProgressConfig.cs**
**Propósito:** Gestión del progreso del jugador y guardado.

**Cómo crear:**
1. En Unity: `Assets > Create > GOOGAZ > Config > Player Progress`
2. Asignar referencia a `ScenarioConfig`

**Funcionalidades:**
```csharp
// Inicializar progreso
progressConfig.InicializarProgreso();

// Actualizar después de completar escenario
progressConfig.ActualizarProgresoEscenario(
    escenarioId: "adolescencia_estres",
    evaluacion: eval,
    tiempoEmpleado: 120f
);

// Guardar/Cargar
progressConfig.GuardarProgreso();
progressConfig.CargarProgreso();

// Estadísticas
float completitud = progressConfig.GetPorcentajeCompletitud();
string tiempo = progressConfig.GetTiempoJuegoFormateado();
```

**Sistema de logros:**
- `primera_victoria`: Aprobar el primer escenario
- `maestro_nutricional`: Completar todos los escenarios
- `perfeccionista`: Obtener puntuación perfecta (100)
- `explorador_culinario`: Descubrir 10 recetas

---

## 🎮 Flujo de Uso Recomendado

### 1. Configuración Inicial
```
1. Crear GameConfig
2. Crear IngredientConfig y añadir todos los ingredientes
3. Crear RecipeConfig (referenciar IngredientConfig) y añadir recetas
4. Crear ScenarioConfig (referenciar Recipe e Ingredient) y crear escenarios
5. Crear ScoringConfig (referenciar Recipe e Ingredient)
6. Crear PlayerProgressConfig (referenciar Scenario)
```

### 2. En el GameManager
```csharp
public class GameManager : MonoBehaviour
{
    [Header("Configuración")]
    public GameConfig gameConfig;
    public IngredientConfig ingredientDB;
    public RecipeConfig recipeDB;
    public ScenarioConfig scenarioDB;
    public ScoringConfig scoringSystem;
    public PlayerProgressConfig playerProgress;
    
    void Start()
    {
        // Cargar progreso del jugador
        playerProgress.CargarProgreso();
        
        // Obtener escenarios desbloqueados
        List<Scenario> disponibles = new List<Scenario>();
        foreach (var escenario in scenarioDB.escenarios)
        {
            var progreso = playerProgress.GetProgresoEscenario(escenario.id);
            if (progreso != null && progreso.desbloqueado)
            {
                disponibles.Add(escenario);
            }
        }
    }
}
```

### 3. Flujo de Juego
```csharp
// 1. Fase de Aprendizaje
void MostrarFaseAprendizaje(Scenario escenario)
{
    List<Recipe> recetasReferencia = new List<Recipe>();
    foreach (string recetaId in escenario.recetasReferenciaIds)
    {
        Recipe receta = recipeDB.GetRecipeById(recetaId);
        if (receta != null) recetasReferencia.Add(receta);
    }
    // Mostrar recetas al jugador...
}

// 2. Fase de Juego (Cocinar)
void EvaluarRecetaJugador(Recipe recetaSeleccionada, Scenario escenario)
{
    int mesActual = DateTime.Now.Month;
    float tiempoRestante = CalcularTiempoRestante();
    
    RecipeEvaluation eval = scoringSystem.EvaluarReceta(
        recetaSeleccionada,
        escenario,
        mesActual,
        tiempoRestante
    );
    
    // Mostrar feedback
    MostrarResultados(eval);
    
    // Actualizar progreso
    float tiempoEmpleado = gameConfig.timeLimit - tiempoRestante;
    playerProgress.ActualizarProgresoEscenario(
        escenario.id,
        eval,
        tiempoEmpleado
    );
}
```

---

## 🔍 Validaciones Automáticas

Todos los ScriptableObjects incluyen validaciones en `OnValidate()`:

- **GameConfig**: Verifica que pesos de puntuación sumen 100%
- **IngredientConfig**: Detecta IDs duplicados
- **RecipeConfig**: Valida que ingredientes existan en la base de datos
- **ScenarioConfig**: Valida recetas e ingredientes referenciados
- **ScoringConfig**: Verifica que pesos de evaluación sumen 100%

---

## 📊 Ejemplo de Datos: Escenario "Adolescencia y Estrés"

```csharp
// Ingredientes beneficiosos
- Avena (AntiEstres, RicoEnFibra, Magnesio)
- Fresas (Antioxidante, VitaminaC)
- Almendras (AntiEstres, Magnesio, Omega3)
- Merluza (Proteico, Omega3, BajoEnGrasa)
- Lentejas (Proteico, RicoEnFibra, RicoEnHierro)

// Recetas de referencia
1. Bowl de avena (leche + fresas + arándanos + almendras)
2. Crema ligera con merluza
3. Guiso de lentejas (calabaza + zanahoria)
4. Ensalada de verano
5. Rodaballo al vapor con verduras

// Propiedades requeridas
- AntiEstres
- Magnesio
- VitaminaB
- Omega3

// Nutri-Score mínimo: B
```

---

## 🚀 Próximos Pasos

1. **Crear los ScriptableObjects** en Unity siguiendo el orden recomendado
2. **Poblar las bases de datos** con los ingredientes y recetas del README
3. **Configurar los 3 escenarios** principales:
   - Embarazo y Deporte
   - Adolescencia y Estrés
   - Senectud (Migraña)
4. **Ajustar el sistema de puntuación** según balance de juego
5. **Implementar el GameManager** que use estos configs
6. **Crear la UI** para mostrar recetas, ingredientes y feedback

---

## 📝 Notas Importantes

- Todos los IDs deben ser únicos y en formato `snake_case`
- Las referencias entre configs deben asignarse en el Inspector de Unity
- El sistema de guardado usa `PlayerPrefs` por defecto (puede cambiarse a JSON)
- Los colores de feedback son personalizables en `ScoringConfig`
- La temporalidad usa el mes actual del sistema (`DateTime.Now.Month`)

---

**¡Listo para empezar a crear contenido para GOOGAZ!** 🎮🥗
