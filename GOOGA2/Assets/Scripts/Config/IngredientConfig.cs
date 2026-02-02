using UnityEngine;
using System;
using System.Collections.Generic;

namespace GOOGAZ.Config
{
    /// <summary>
    /// Tipos de ingredientes disponibles en el juego
    /// </summary>
    public enum IngredientType
    {
        Verdura,
        Fruta,
        Proteina,
        Cereal,
        Lacteo,
        Legumbre,
        Fruto_Seco,
        Aceite,
        Especias
    }
    
    /// <summary>
    /// Clasificación Nutri-Score (de A=mejor a E=peor)
    /// </summary>
    public enum NutriScore
    {
        A = 5,  // Excelente
        B = 4,  // Bueno
        C = 3,  // Aceptable
        D = 2,  // Pobre
        E = 1   // Muy pobre
    }
    
    /// <summary>
    /// Meses del año para control de temporalidad
    /// </summary>
    [Flags]
    public enum Season
    {
        None = 0,
        Enero = 1 << 0,
        Febrero = 1 << 1,
        Marzo = 1 << 2,
        Abril = 1 << 3,
        Mayo = 1 << 4,
        Junio = 1 << 5,
        Julio = 1 << 6,
        Agosto = 1 << 7,
        Septiembre = 1 << 8,
        Octubre = 1 << 9,
        Noviembre = 1 << 10,
        Diciembre = 1 << 11,
        TodoElAño = ~0
    }
    
    /// <summary>
    /// Propiedades nutricionales especiales de los ingredientes
    /// </summary>
    [Flags]
    public enum NutritionalProperty
    {
        None = 0,
        AntiEstres = 1 << 0,        // Magnesio, omega-3, vitaminas B
        Energetico = 1 << 1,         // Carbohidratos complejos
        Proteico = 1 << 2,           // Alto en proteínas
        Antioxidante = 1 << 3,       // Vitaminas A, C, E
        Hidratante = 1 << 4,         // Alto contenido de agua
        RicoEnFibra = 1 << 5,        // Fibra dietética
        BajoEnGrasa = 1 << 6,        // Bajo contenido graso
        RicoEnCalcio = 1 << 7,       // Para huesos
        RicoEnHierro = 1 << 8,       // Para anemia
        Omega3 = 1 << 9,             // Ácidos grasos omega-3
        VitaminaB = 1 << 10,         // Complejo B
        Magnesio = 1 << 11,          // Mineral anti-estrés
        FacilDigestion = 1 << 12     // Fácil de digerir
    }
    
    /// <summary>
    /// Configuración de un ingrediente individual
    /// </summary>
    [Serializable]
    public class Ingredient
    {
        [Header("Información Básica")]
        public string id;
        public string nombre;
        [TextArea(2, 4)]
        public string descripcion;
        public Sprite icono;
        
        [Header("Clasificación")]
        public IngredientType tipo;
        public NutriScore nutriScore = NutriScore.C;
        
        [Header("Temporalidad")]
        [Tooltip("Meses en los que este ingrediente está de temporada")]
        public Season temporada = Season.TodoElAño;
        
        [Header("Propiedades Nutricionales")]
        public NutritionalProperty propiedades = NutritionalProperty.None;
        
        [Header("Valores Nutricionales (por 100g)")]
        [Range(0, 1000)]
        public float calorias;
        [Range(0, 100)]
        public float proteinas;
        [Range(0, 100)]
        public float carbohidratos;
        [Range(0, 100)]
        public float grasas;
        [Range(0, 100)]
        public float fibra;
        
        [Header("Gameplay")]
        [Tooltip("¿Este ingrediente está desbloqueado desde el inicio?")]
        public bool desbloqueadoInicio = true;
        
        /// <summary>
        /// Verifica si el ingrediente está de temporada en un mes específico
        /// </summary>
        public bool EstaDeTemporada(int mes)
        {
            if (mes < 1 || mes > 12) return false;
            Season mesActual = (Season)(1 << (mes - 1));
            return (temporada & mesActual) != 0 || temporada == Season.TodoElAño;
        }
        
        /// <summary>
        /// Verifica si el ingrediente tiene una propiedad nutricional específica
        /// </summary>
        public bool TienePropiedad(NutritionalProperty propiedad)
        {
            return (propiedades & propiedad) != 0;
        }
        
        /// <summary>
        /// Obtiene el valor numérico del Nutri-Score
        /// </summary>
        public int GetNutriScoreValue()
        {
            return (int)nutriScore;
        }
    }
    
    /// <summary>
    /// ScriptableObject que contiene la base de datos de ingredientes
    /// </summary>
    [CreateAssetMenu(fileName = "IngredientDatabase", menuName = "GOOGAZ/Config/Ingredient Database", order = 3)]
    public class IngredientConfig : ScriptableObject
    {
        [Header("Base de Datos de Ingredientes")]
        [Tooltip("Lista de todos los ingredientes disponibles en el juego")]
        public List<Ingredient> ingredientes = new List<Ingredient>();
        
        /// <summary>
        /// Busca un ingrediente por su ID
        /// </summary>
        public Ingredient GetIngredientById(string id)
        {
            return ingredientes.Find(i => i.id == id);
        }
        
        /// <summary>
        /// Obtiene todos los ingredientes de un tipo específico
        /// </summary>
        public List<Ingredient> GetIngredientsByType(IngredientType tipo)
        {
            return ingredientes.FindAll(i => i.tipo == tipo);
        }
        
        /// <summary>
        /// Obtiene ingredientes que están de temporada en un mes específico
        /// </summary>
        public List<Ingredient> GetSeasonalIngredients(int mes)
        {
            return ingredientes.FindAll(i => i.EstaDeTemporada(mes));
        }
        
        /// <summary>
        /// Obtiene ingredientes con una propiedad nutricional específica
        /// </summary>
        public List<Ingredient> GetIngredientsByProperty(NutritionalProperty propiedad)
        {
            return ingredientes.FindAll(i => i.TienePropiedad(propiedad));
        }
        
        /// <summary>
        /// Obtiene ingredientes con un Nutri-Score mínimo
        /// </summary>
        public List<Ingredient> GetIngredientsByMinNutriScore(NutriScore minScore)
        {
            return ingredientes.FindAll(i => i.nutriScore >= minScore);
        }
        
        private void OnValidate()
        {
            // Verificar IDs duplicados
            HashSet<string> ids = new HashSet<string>();
            foreach (var ingrediente in ingredientes)
            {
                if (!string.IsNullOrEmpty(ingrediente.id))
                {
                    if (!ids.Add(ingrediente.id))
                    {
                        Debug.LogWarning($"[IngredientConfig] ID duplicado encontrado: {ingrediente.id}");
                    }
                }
            }
        }
    }
}
