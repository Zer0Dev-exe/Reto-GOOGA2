using UnityEngine;
using System;
using System.Collections.Generic;

namespace GOOGAZ.Config
{
    /// <summary>
    /// Tipo de comida del día
    /// </summary>
    public enum MealType
    {
        Desayuno,
        Comida,
        Cena,
        Snack
    }
    
    /// <summary>
    /// Representa una receta completa con sus ingredientes y propiedades
    /// </summary>
    [Serializable]
    public class Recipe
    {
        [Header("Información Básica")]
        public string id;
        public string nombre;
        [TextArea(3, 6)]
        public string descripcion;
        public Sprite imagen;
        
        [Header("Clasificación")]
        public MealType tipoComida;
        
        [Header("Ingredientes")]
        [Tooltip("IDs de los ingredientes que componen esta receta")]
        public List<string> ingredientesIds = new List<string>();
        
        [Header("Propiedades de la Receta")]
        [Tooltip("Propiedades nutricionales que aporta esta receta")]
        public NutritionalProperty propiedadesNutricionales = NutritionalProperty.None;
        
        [Tooltip("Nutri-Score calculado de la receta")]
        public NutriScore nutriScore = NutriScore.C;
        
        [Header("Adecuación a Escenarios")]
        [Tooltip("IDs de los escenarios para los que esta receta es adecuada")]
        public List<string> escenariosAdecuados = new List<string>();
        
        [Header("Valores Nutricionales Totales")]
        [Range(0, 2000)]
        public float caloriasTotales;
        [Range(0, 200)]
        public float proteinasTotales;
        [Range(0, 300)]
        public float carbohidratosTotales;
        [Range(0, 100)]
        public float grasasTotales;
        
        [Header("Temporalidad")]
        [Tooltip("¿Esta receta usa principalmente ingredientes de temporada?")]
        public bool usaIngredientesTemporada = true;
        
        [Header("Dificultad y Tiempo")]
        [Range(1, 5)]
        [Tooltip("Dificultad de preparación (1=muy fácil, 5=muy difícil)")]
        public int dificultad = 2;
        
        [Tooltip("Tiempo de preparación en minutos")]
        public int tiempoPreparacion = 30;
        
        [Header("Gameplay")]
        [Tooltip("¿Esta receta es de referencia en la fase de aprendizaje?")]
        public bool esRecetaReferencia = false;
        
        [Tooltip("Puntos bonus por preparar esta receta correctamente")]
        [Range(0, 20)]
        public int puntosBonus = 0;
        
        /// <summary>
        /// Verifica si la receta es adecuada para un escenario específico
        /// </summary>
        public bool EsAdecuadaParaEscenario(string escenarioId)
        {
            return escenariosAdecuados.Contains(escenarioId);
        }
        
        /// <summary>
        /// Verifica si la receta tiene una propiedad nutricional específica
        /// </summary>
        public bool TienePropiedad(NutritionalProperty propiedad)
        {
            return (propiedadesNutricionales & propiedad) != 0;
        }
        
        /// <summary>
        /// Calcula el porcentaje de ingredientes de temporada
        /// </summary>
        public float CalcularPorcentajeTemporalidad(IngredientConfig ingredientDB, int mesActual)
        {
            if (ingredientesIds.Count == 0) return 0f;
            
            int ingredientesTemporada = 0;
            foreach (string id in ingredientesIds)
            {
                Ingredient ing = ingredientDB.GetIngredientById(id);
                if (ing != null && ing.EstaDeTemporada(mesActual))
                {
                    ingredientesTemporada++;
                }
            }
            
            return (float)ingredientesTemporada / ingredientesIds.Count * 100f;
        }
    }
    
    /// <summary>
    /// ScriptableObject que contiene la base de datos de recetas
    /// </summary>
    [CreateAssetMenu(fileName = "RecipeDatabase", menuName = "GOOGAZ/Config/Recipe Database", order = 2)]
    public class RecipeConfig : ScriptableObject
    {
        [Header("Base de Datos de Recetas")]
        [Tooltip("Lista de todas las recetas disponibles en el juego")]
        public List<Recipe> recetas = new List<Recipe>();
        
        [Header("Referencias")]
        [Tooltip("Referencia a la base de datos de ingredientes")]
        public IngredientConfig ingredientDatabase;
        
        /// <summary>
        /// Busca una receta por su ID
        /// </summary>
        public Recipe GetRecipeById(string id)
        {
            return recetas.Find(r => r.id == id);
        }
        
        /// <summary>
        /// Obtiene todas las recetas de un tipo de comida específico
        /// </summary>
        public List<Recipe> GetRecipesByMealType(MealType tipo)
        {
            return recetas.FindAll(r => r.tipoComida == tipo);
        }
        
        /// <summary>
        /// Obtiene las recetas de referencia para la fase de aprendizaje
        /// </summary>
        public List<Recipe> GetReferenceRecipes()
        {
            return recetas.FindAll(r => r.esRecetaReferencia);
        }
        
        /// <summary>
        /// Obtiene recetas adecuadas para un escenario específico
        /// </summary>
        public List<Recipe> GetRecipesForScenario(string escenarioId)
        {
            return recetas.FindAll(r => r.EsAdecuadaParaEscenario(escenarioId));
        }
        
        /// <summary>
        /// Obtiene recetas que contienen un ingrediente específico
        /// </summary>
        public List<Recipe> GetRecipesWithIngredient(string ingredientId)
        {
            return recetas.FindAll(r => r.ingredientesIds.Contains(ingredientId));
        }
        
        /// <summary>
        /// Obtiene recetas con una propiedad nutricional específica
        /// </summary>
        public List<Recipe> GetRecipesByProperty(NutritionalProperty propiedad)
        {
            return recetas.FindAll(r => r.TienePropiedad(propiedad));
        }
        
        /// <summary>
        /// Obtiene recetas con un Nutri-Score mínimo
        /// </summary>
        public List<Recipe> GetRecipesByMinNutriScore(NutriScore minScore)
        {
            return recetas.FindAll(r => r.nutriScore >= minScore);
        }
        
        /// <summary>
        /// Valida que todos los ingredientes de las recetas existan en la base de datos
        /// </summary>
        public bool ValidateRecipeIngredients()
        {
            if (ingredientDatabase == null)
            {
                Debug.LogError("[RecipeConfig] No hay referencia a IngredientDatabase");
                return false;
            }
            
            bool todosValidos = true;
            foreach (var receta in recetas)
            {
                foreach (string ingredienteId in receta.ingredientesIds)
                {
                    if (ingredientDatabase.GetIngredientById(ingredienteId) == null)
                    {
                        Debug.LogWarning($"[RecipeConfig] Receta '{receta.nombre}' contiene ingrediente inválido: {ingredienteId}");
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
            foreach (var receta in recetas)
            {
                if (!string.IsNullOrEmpty(receta.id))
                {
                    if (!ids.Add(receta.id))
                    {
                        Debug.LogWarning($"[RecipeConfig] ID duplicado encontrado: {receta.id}");
                    }
                }
            }
            
            // Validar ingredientes si hay referencia
            if (ingredientDatabase != null)
            {
                ValidateRecipeIngredients();
            }
        }
    }
}
