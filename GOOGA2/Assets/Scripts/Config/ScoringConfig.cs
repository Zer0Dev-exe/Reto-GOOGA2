using UnityEngine;
using System;
using System.Collections.Generic;

namespace GOOGAZ.Config
{
    /// <summary>
    /// Resultado de la evaluación de una receta
    /// </summary>
    [Serializable]
    public class RecipeEvaluation
    {
        public Recipe receta;
        public float puntuacionTotal;
        public float puntuacionNutriScore;
        public float puntuacionAdecuacion;
        public float puntuacionTemporalidad;
        public List<string> aciertos = new List<string>();
        public List<string> errores = new List<string>();
        public bool aprobado;
    }
    
    /// <summary>
    /// Criterios de puntuación para cada aspecto evaluado
    /// </summary>
    [Serializable]
    public class ScoringCriteria
    {
        [Header("Nutri-Score")]
        [Tooltip("Puntos por cada nivel de Nutri-Score (A=5, B=4, C=3, D=2, E=1)")]
        public float puntosNutriScoreA = 20f;
        public float puntosNutriScoreB = 16f;
        public float puntosNutriScoreC = 12f;
        public float puntosNutriScoreD = 8f;
        public float puntosNutriScoreE = 4f;
        
        [Header("Adecuación a la Situación")]
        [Tooltip("Puntos por cada propiedad nutricional requerida presente")]
        public float puntosPorPropiedadRequerida = 10f;
        
        [Tooltip("Penalización por cada propiedad prohibida presente")]
        public float penalizacionPorPropiedadProhibida = -15f;
        
        [Tooltip("Puntos bonus por usar ingredientes beneficiosos")]
        public float puntosPorIngredienteBeneficioso = 5f;
        
        [Tooltip("Penalización por usar ingredientes prohibidos")]
        public float penalizacionPorIngredienteProhibido = -10f;
        
        [Header("Temporalidad")]
        [Tooltip("Puntos máximos por usar 100% ingredientes de temporada")]
        public float puntosMaximosTemporalidad = 20f;
        
        [Tooltip("Porcentaje mínimo de ingredientes de temporada para obtener puntos")]
        [Range(0, 100)]
        public float porcentajeMinimoTemporalidad = 50f;
        
        [Header("Bonus y Penalizaciones")]
        [Tooltip("Bonus por completar el reto rápidamente (% del tiempo restante)")]
        [Range(0, 20)]
        public float bonusTiempo = 10f;
        
        [Tooltip("Bonus por usar receta de referencia exacta")]
        public float bonusRecetaReferencia = 15f;
        
        [Tooltip("Penalización por exceder calorías recomendadas")]
        public float penalizacionExcesoCalorias = -5f;
        
        [Header("Umbrales")]
        [Tooltip("Calorías máximas recomendadas por comida")]
        public float caloriasMaximasRecomendadas = 800f;
        
        [Tooltip("Puntuación mínima para aprobar")]
        [Range(0, 100)]
        public float puntuacionMinima = 60f;
    }
    
    /// <summary>
    /// ScriptableObject que gestiona el sistema de puntuación
    /// </summary>
    [CreateAssetMenu(fileName = "ScoringConfig", menuName = "GOOGAZ/Config/Scoring System", order = 4)]
    public class ScoringConfig : ScriptableObject
    {
        [Header("Criterios de Puntuación")]
        public ScoringCriteria criterios = new ScoringCriteria();
        
        [Header("Pesos de Evaluación")]
        [Tooltip("Peso del Nutri-Score en la puntuación final (%)")]
        [Range(0, 100)]
        public float pesoNutriScore = 40f;
        
        [Tooltip("Peso de la adecuación a la situación (%)")]
        [Range(0, 100)]
        public float pesoAdecuacion = 40f;
        
        [Tooltip("Peso de la temporalidad (%)")]
        [Range(0, 100)]
        public float pesoTemporalidad = 20f;
        
        [Header("Referencias")]
        public IngredientConfig ingredientDatabase;
        public RecipeConfig recipeDatabase;
        
        [Header("Feedback Visual")]
        [Tooltip("Color para puntuación excelente (>90)")]
        public Color colorExcelente = new Color(0.2f, 0.8f, 0.2f);
        
        [Tooltip("Color para puntuación buena (70-90)")]
        public Color colorBueno = new Color(0.4f, 0.7f, 0.3f);
        
        [Tooltip("Color para puntuación aceptable (60-70)")]
        public Color colorAceptable = new Color(0.8f, 0.8f, 0.2f);
        
        [Tooltip("Color para puntuación insuficiente (<60)")]
        public Color colorInsuficiente = new Color(0.8f, 0.2f, 0.2f);
        
        /// <summary>
        /// Evalúa una receta en el contexto de un escenario específico
        /// </summary>
        public RecipeEvaluation EvaluarReceta(Recipe receta, Scenario escenario, int mesActual, float tiempoRestante = 0f)
        {
            RecipeEvaluation evaluacion = new RecipeEvaluation
            {
                receta = receta
            };
            
            // 1. Evaluar Nutri-Score
            evaluacion.puntuacionNutriScore = EvaluarNutriScore(receta);
            
            // 2. Evaluar Adecuación a la Situación
            evaluacion.puntuacionAdecuacion = EvaluarAdecuacion(receta, escenario, evaluacion);
            
            // 3. Evaluar Temporalidad
            evaluacion.puntuacionTemporalidad = EvaluarTemporalidad(receta, mesActual, evaluacion);
            
            // 4. Calcular puntuación total ponderada
            float puntuacionBase = 
                (evaluacion.puntuacionNutriScore * pesoNutriScore / 100f) +
                (evaluacion.puntuacionAdecuacion * pesoAdecuacion / 100f) +
                (evaluacion.puntuacionTemporalidad * pesoTemporalidad / 100f);
            
            // 5. Aplicar bonus y penalizaciones adicionales
            float bonus = 0f;
            
            // Bonus por tiempo
            if (tiempoRestante > 0f)
            {
                bonus += criterios.bonusTiempo * (tiempoRestante / 100f);
                evaluacion.aciertos.Add($"Bonus por tiempo: +{bonus:F1} puntos");
            }
            
            // Bonus por receta de referencia
            if (receta.esRecetaReferencia && escenario.recetasReferenciaIds.Contains(receta.id))
            {
                bonus += criterios.bonusRecetaReferencia;
                evaluacion.aciertos.Add($"¡Usaste una receta de referencia! +{criterios.bonusRecetaReferencia} puntos");
            }
            
            // Bonus de la receta
            if (receta.puntosBonus > 0)
            {
                bonus += receta.puntosBonus;
                evaluacion.aciertos.Add($"Bonus de receta: +{receta.puntosBonus} puntos");
            }
            
            // Penalización por exceso de calorías
            if (receta.caloriasTotales > criterios.caloriasMaximasRecomendadas)
            {
                float exceso = receta.caloriasTotales - criterios.caloriasMaximasRecomendadas;
                float penalizacion = criterios.penalizacionExcesoCalorias * (exceso / 100f);
                bonus += penalizacion;
                evaluacion.errores.Add($"Exceso de calorías ({exceso:F0} kcal): {penalizacion:F1} puntos");
            }
            
            evaluacion.puntuacionTotal = Mathf.Clamp(puntuacionBase + bonus, 0f, 100f);
            evaluacion.aprobado = evaluacion.puntuacionTotal >= criterios.puntuacionMinima;
            
            return evaluacion;
        }
        
        /// <summary>
        /// Evalúa el Nutri-Score de la receta
        /// </summary>
        private float EvaluarNutriScore(Recipe receta)
        {
            float puntos = 0f;
            
            switch (receta.nutriScore)
            {
                case NutriScore.A:
                    puntos = criterios.puntosNutriScoreA;
                    break;
                case NutriScore.B:
                    puntos = criterios.puntosNutriScoreB;
                    break;
                case NutriScore.C:
                    puntos = criterios.puntosNutriScoreC;
                    break;
                case NutriScore.D:
                    puntos = criterios.puntosNutriScoreD;
                    break;
                case NutriScore.E:
                    puntos = criterios.puntosNutriScoreE;
                    break;
            }
            
            return puntos;
        }
        
        /// <summary>
        /// Evalúa la adecuación de la receta al escenario
        /// </summary>
        private float EvaluarAdecuacion(Recipe receta, Scenario escenario, RecipeEvaluation evaluacion)
        {
            float puntos = 0f;
            
            // Verificar propiedades nutricionales requeridas
            foreach (NutritionalProperty propiedad in Enum.GetValues(typeof(NutritionalProperty)))
            {
                if (propiedad == NutritionalProperty.None) continue;
                
                if (escenario.RequierePropiedad(propiedad))
                {
                    if (receta.TienePropiedad(propiedad))
                    {
                        puntos += criterios.puntosPorPropiedadRequerida;
                        evaluacion.aciertos.Add($"Propiedad requerida presente: {propiedad}");
                    }
                    else
                    {
                        evaluacion.errores.Add($"Falta propiedad requerida: {propiedad}");
                    }
                }
                
                // Verificar propiedades prohibidas
                if (escenario.PropiedadProhibida(propiedad))
                {
                    if (receta.TienePropiedad(propiedad))
                    {
                        puntos += criterios.penalizacionPorPropiedadProhibida;
                        evaluacion.errores.Add($"Propiedad prohibida presente: {propiedad}");
                    }
                }
            }
            
            // Verificar ingredientes beneficiosos y prohibidos
            if (ingredientDatabase != null)
            {
                foreach (string ingredienteId in receta.ingredientesIds)
                {
                    if (escenario.EsIngredienteBeneficioso(ingredienteId))
                    {
                        puntos += criterios.puntosPorIngredienteBeneficioso;
                        Ingredient ing = ingredientDatabase.GetIngredientById(ingredienteId);
                        if (ing != null)
                        {
                            evaluacion.aciertos.Add($"Ingrediente beneficioso: {ing.nombre}");
                        }
                    }
                    
                    if (escenario.EsIngredienteProhibido(ingredienteId))
                    {
                        puntos += criterios.penalizacionPorIngredienteProhibido;
                        Ingredient ing = ingredientDatabase.GetIngredientById(ingredienteId);
                        if (ing != null)
                        {
                            evaluacion.errores.Add($"Ingrediente prohibido: {ing.nombre}");
                        }
                    }
                }
            }
            
            return Mathf.Max(0f, puntos);
        }
        
        /// <summary>
        /// Evalúa la temporalidad de los ingredientes
        /// </summary>
        private float EvaluarTemporalidad(Recipe receta, int mesActual, RecipeEvaluation evaluacion)
        {
            if (ingredientDatabase == null || receta.ingredientesIds.Count == 0)
            {
                return 0f;
            }
            
            float porcentajeTemporalidad = receta.CalcularPorcentajeTemporalidad(ingredientDatabase, mesActual);
            
            if (porcentajeTemporalidad >= criterios.porcentajeMinimoTemporalidad)
            {
                float puntos = criterios.puntosMaximosTemporalidad * (porcentajeTemporalidad / 100f);
                evaluacion.aciertos.Add($"Temporalidad: {porcentajeTemporalidad:F0}% de ingredientes de temporada");
                return puntos;
            }
            else
            {
                evaluacion.errores.Add($"Baja temporalidad: solo {porcentajeTemporalidad:F0}% de ingredientes de temporada");
                return 0f;
            }
        }
        
        /// <summary>
        /// Obtiene el color de feedback según la puntuación
        /// </summary>
        public Color GetColorPorPuntuacion(float puntuacion)
        {
            if (puntuacion >= 90f) return colorExcelente;
            if (puntuacion >= 70f) return colorBueno;
            if (puntuacion >= 60f) return colorAceptable;
            return colorInsuficiente;
        }
        
        /// <summary>
        /// Obtiene el mensaje de feedback según la puntuación
        /// </summary>
        public string GetMensajePorPuntuacion(float puntuacion)
        {
            if (puntuacion >= 90f) return "¡EXCELENTE! Eres un maestro de la nutrición";
            if (puntuacion >= 80f) return "¡MUY BIEN! Gran conocimiento nutricional";
            if (puntuacion >= 70f) return "¡BIEN! Buen trabajo";
            if (puntuacion >= 60f) return "ACEPTABLE. Puedes mejorar";
            return "INSUFICIENTE. Revisa las recetas de referencia";
        }
        
        /// <summary>
        /// Valida que los pesos sumen 100%
        /// </summary>
        public bool ValidarPesos()
        {
            float total = pesoNutriScore + pesoAdecuacion + pesoTemporalidad;
            return Mathf.Approximately(total, 100f);
        }
        
        private void OnValidate()
        {
            if (!ValidarPesos())
            {
                Debug.LogWarning($"[ScoringConfig] Los pesos no suman 100%. Total: {pesoNutriScore + pesoAdecuacion + pesoTemporalidad}%");
            }
        }
    }
}
