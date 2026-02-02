using UnityEngine;

namespace GOOGAZ.Config
{
    /// <summary>
    /// Configuración general del juego GOOGAZ
    /// Contiene parámetros globales y ajustes del sistema de juego
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GOOGAZ/Config/Game Config", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [Header("Configuración General")]
        [Tooltip("Nombre del juego")]
        public string gameName = "GOOGAZ";
        
        [Tooltip("Versión actual del juego")]
        public string version = "1.0.0";
        
        [Header("Configuración de Gameplay")]
        [Tooltip("Tiempo límite para completar cada reto (en segundos, 0 = sin límite)")]
        [Range(0, 600)]
        public float timeLimit = 180f;
        
        [Tooltip("¿Mostrar fase de aprendizaje antes del reto?")]
        public bool showLearningPhase = true;
        
        [Tooltip("Número de recetas a mostrar en la fase de aprendizaje")]
        [Range(1, 10)]
        public int recipesToShowInLearning = 3;
        
        [Header("Sistema de Puntuación")]
        [Tooltip("Puntuación mínima para aprobar un reto")]
        [Range(0, 100)]
        public int minimumPassingScore = 60;
        
        [Tooltip("Puntuación máxima posible")]
        public int maximumScore = 100;
        
        [Header("Pesos de Criterios de Puntuación")]
        [Tooltip("Peso del Nutri-Score en la puntuación final (%)")]
        [Range(0, 100)]
        public float nutriScoreWeight = 40f;
        
        [Tooltip("Peso de la adecuación a la situación (%)")]
        [Range(0, 100)]
        public float situationAdequacyWeight = 40f;
        
        [Tooltip("Peso de la temporalidad de ingredientes (%)")]
        [Range(0, 100)]
        public float seasonalityWeight = 20f;
        
        [Header("Configuración de UI")]
        [Tooltip("Duración de las animaciones de feedback (segundos)")]
        public float feedbackAnimationDuration = 2f;
        
        [Tooltip("¿Mostrar puntuación en tiempo real?")]
        public bool showRealtimeScore = true;
        
        [Header("Audio")]
        [Tooltip("Volumen maestro del juego")]
        [Range(0f, 1f)]
        public float masterVolume = 0.8f;
        
        [Tooltip("Volumen de efectos de sonido")]
        [Range(0f, 1f)]
        public float sfxVolume = 0.7f;
        
        [Tooltip("Volumen de música")]
        [Range(0f, 1f)]
        public float musicVolume = 0.5f;
        
        [Header("Desarrollo y Debug")]
        [Tooltip("Modo debug activado")]
        public bool debugMode = false;
        
        [Tooltip("¿Saltar fase de aprendizaje en modo debug?")]
        public bool skipLearningInDebug = false;
        
        /// <summary>
        /// Valida que los pesos de puntuación sumen 100%
        /// </summary>
        public bool ValidateScoreWeights()
        {
            float total = nutriScoreWeight + situationAdequacyWeight + seasonalityWeight;
            return Mathf.Approximately(total, 100f);
        }
        
        private void OnValidate()
        {
            // Validar que los pesos sumen 100%
            if (!ValidateScoreWeights())
            {
                Debug.LogWarning($"[GameConfig] Los pesos de puntuación no suman 100%. Total actual: {nutriScoreWeight + situationAdequacyWeight + seasonalityWeight}%");
            }
        }
    }
}
