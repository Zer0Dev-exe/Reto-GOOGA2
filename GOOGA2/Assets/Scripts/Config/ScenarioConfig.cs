using UnityEngine;
using System;
using System.Collections.Generic;

namespace GOOGAZ.Config
{
    /// <summary>
    /// Colectivos o grupos poblacionales objetivo
    /// </summary>
    public enum TargetGroup
    {
        Embarazo,
        Adolescencia,
        Senectud,
        Deportista,
        Infantil,
        Adulto
    }
    
    /// <summary>
    /// Condiciones o situaciones específicas a resolver
    /// </summary>
    public enum Condition
    {
        Ninguna,
        Estres,
        Migrana,
        Deporte_Antes,
        Deporte_Despues,
        Anemia,
        Diabetes,
        Hipertension
    }
    
    /// <summary>
    /// Representa un escenario de juego completo
    /// </summary>
    [Serializable]
    public class Scenario
    {
        [Header("Identificación")]
        public string id;
        public string nombre;
        [TextArea(3, 6)]
        public string descripcion;
        
        [Header("Colectivo y Condición")]
        public TargetGroup grupoObjetivo;
        public Condition condicion;
        
        [Header("Contexto del Reto")]
        [TextArea(2, 5)]
        [Tooltip("Situación específica que se presenta al jugador")]
        public string situacion;
        
        [TextArea(3, 8)]
        [Tooltip("Notas clave que el jugador debe tener en cuenta")]
        public string notasClave;
        
        [Header("Requisitos Nutricionales")]
        [Tooltip("Propiedades nutricionales necesarias para resolver este escenario")]
        public NutritionalProperty propiedadesRequeridas = NutritionalProperty.None;
        
        [Tooltip("Propiedades que deben evitarse en este escenario")]
        public NutritionalProperty propiedadesProhibidas = NutritionalProperty.None;
        
        [Tooltip("Nutri-Score mínimo recomendado")]
        public NutriScore nutriScoreMinimo = NutriScore.B;
        
        [Header("Recetas de Referencia")]
        [Tooltip("IDs de las recetas que se muestran en la fase de aprendizaje")]
        public List<string> recetasReferenciaIds = new List<string>();
        
        [Header("Ingredientes")]
        [Tooltip("IDs de ingredientes especialmente beneficiosos para este escenario")]
        public List<string> ingredientesBeneficiosos = new List<string>();
        
        [Tooltip("IDs de ingredientes que deben evitarse en este escenario")]
        public List<string> ingredientesProhibidos = new List<string>();
        
        [Header("Temporalidad")]
        [Tooltip("¿Este escenario requiere uso de ingredientes de temporada?")]
        public bool requiereTemporalidad = true;
        
        [Tooltip("Peso de la temporalidad en la puntuación (%)")]
        [Range(0, 50)]
        public float pesoTemporalidad = 20f;
        
        [Header("Dificultad")]
        [Range(1, 5)]
        [Tooltip("Nivel de dificultad del escenario (1=fácil, 5=muy difícil)")]
        public int nivelDificultad = 2;
        
        [Header("Visuales")]
        public Sprite imagenEscenario;
        public Sprite iconoColectivo;
        public Color colorTema = Color.white;
        
        [Header("Orden y Progresión")]
        [Tooltip("Orden en que aparece este escenario en el juego")]
        public int ordenProgresion = 0;
        
        [Tooltip("¿Este escenario está desbloqueado desde el inicio?")]
        public bool desbloqueadoInicio = true;
        
        [Tooltip("IDs de escenarios que deben completarse antes de desbloquear este")]
        public List<string> escenariosRequeridos = new List<string>();
        
        /// <summary>
        /// Verifica si el escenario requiere una propiedad nutricional específica
        /// </summary>
        public bool RequierePropiedad(NutritionalProperty propiedad)
        {
            return (propiedadesRequeridas & propiedad) != 0;
        }
        
        /// <summary>
        /// Verifica si una propiedad está prohibida en este escenario
        /// </summary>
        public bool PropiedadProhibida(NutritionalProperty propiedad)
        {
            return (propiedadesProhibidas & propiedad) != 0;
        }
        
        /// <summary>
        /// Verifica si un ingrediente es beneficioso para este escenario
        /// </summary>
        public bool EsIngredienteBeneficioso(string ingredienteId)
        {
            return ingredientesBeneficiosos.Contains(ingredienteId);
        }
        
        /// <summary>
        /// Verifica si un ingrediente está prohibido en este escenario
        /// </summary>
        public bool EsIngredienteProhibido(string ingredienteId)
        {
            return ingredientesProhibidos.Contains(ingredienteId);
        }
    }
    
    /// <summary>
    /// ScriptableObject que contiene la configuración de escenarios
    /// </summary>
    [CreateAssetMenu(fileName = "ScenarioDatabase", menuName = "GOOGAZ/Config/Scenario Database", order = 1)]
    public class ScenarioConfig : ScriptableObject
    {
        [Header("Base de Datos de Escenarios")]
        [Tooltip("Lista de todos los escenarios disponibles en el juego")]
        public List<Scenario> escenarios = new List<Scenario>();
        
        [Header("Referencias")]
        [Tooltip("Referencia a la base de datos de recetas")]
        public RecipeConfig recipeDatabase;
        
        [Tooltip("Referencia a la base de datos de ingredientes")]
        public IngredientConfig ingredientDatabase;
        
        /// <summary>
        /// Busca un escenario por su ID
        /// </summary>
        public Scenario GetScenarioById(string id)
        {
            return escenarios.Find(s => s.id == id);
        }
        
        /// <summary>
        /// Obtiene escenarios por grupo objetivo
        /// </summary>
        public List<Scenario> GetScenariosByTargetGroup(TargetGroup grupo)
        {
            return escenarios.FindAll(s => s.grupoObjetivo == grupo);
        }
        
        /// <summary>
        /// Obtiene escenarios por condición
        /// </summary>
        public List<Scenario> GetScenariosByCondition(Condition condicion)
        {
            return escenarios.FindAll(s => s.condicion == condicion);
        }
        
        /// <summary>
        /// Obtiene escenarios desbloqueados desde el inicio
        /// </summary>
        public List<Scenario> GetUnlockedScenarios()
        {
            return escenarios.FindAll(s => s.desbloqueadoInicio);
        }
        
        /// <summary>
        /// Obtiene escenarios ordenados por progresión
        /// </summary>
        public List<Scenario> GetScenariosInOrder()
        {
            List<Scenario> ordenados = new List<Scenario>(escenarios);
            ordenados.Sort((a, b) => a.ordenProgresion.CompareTo(b.ordenProgresion));
            return ordenados;
        }
        
        /// <summary>
        /// Obtiene escenarios por nivel de dificultad
        /// </summary>
        public List<Scenario> GetScenariosByDifficulty(int dificultad)
        {
            return escenarios.FindAll(s => s.nivelDificultad == dificultad);
        }
        
        /// <summary>
        /// Valida que todas las recetas de referencia existan
        /// </summary>
        public bool ValidateScenarioRecipes()
        {
            if (recipeDatabase == null)
            {
                Debug.LogError("[ScenarioConfig] No hay referencia a RecipeDatabase");
                return false;
            }
            
            bool todosValidos = true;
            foreach (var escenario in escenarios)
            {
                foreach (string recetaId in escenario.recetasReferenciaIds)
                {
                    if (recipeDatabase.GetRecipeById(recetaId) == null)
                    {
                        Debug.LogWarning($"[ScenarioConfig] Escenario '{escenario.nombre}' contiene receta inválida: {recetaId}");
                        todosValidos = false;
                    }
                }
            }
            
            return todosValidos;
        }
        
        /// <summary>
        /// Valida que todos los ingredientes beneficiosos/prohibidos existan
        /// </summary>
        public bool ValidateScenarioIngredients()
        {
            if (ingredientDatabase == null)
            {
                Debug.LogError("[ScenarioConfig] No hay referencia a IngredientDatabase");
                return false;
            }
            
            bool todosValidos = true;
            foreach (var escenario in escenarios)
            {
                // Validar ingredientes beneficiosos
                foreach (string ingredienteId in escenario.ingredientesBeneficiosos)
                {
                    if (ingredientDatabase.GetIngredientById(ingredienteId) == null)
                    {
                        Debug.LogWarning($"[ScenarioConfig] Escenario '{escenario.nombre}' contiene ingrediente beneficioso inválido: {ingredienteId}");
                        todosValidos = false;
                    }
                }
                
                // Validar ingredientes prohibidos
                foreach (string ingredienteId in escenario.ingredientesProhibidos)
                {
                    if (ingredientDatabase.GetIngredientById(ingredienteId) == null)
                    {
                        Debug.LogWarning($"[ScenarioConfig] Escenario '{escenario.nombre}' contiene ingrediente prohibido inválido: {ingredienteId}");
                        todosValidos = false;
                    }
                }
            }
            
            return todosValidos;
        }
        
        private void OnValidate()
        {
            // Verificar IDs duplicados
            HashSet<string> ids = new HashSet<string>();
            foreach (var escenario in escenarios)
            {
                if (!string.IsNullOrEmpty(escenario.id))
                {
                    if (!ids.Add(escenario.id))
                    {
                        Debug.LogWarning($"[ScenarioConfig] ID duplicado encontrado: {escenario.id}");
                    }
                }
            }
            
            // Validar referencias si existen
            if (recipeDatabase != null)
            {
                ValidateScenarioRecipes();
            }
            
            if (ingredientDatabase != null)
            {
                ValidateScenarioIngredients();
            }
        }
    }
}
