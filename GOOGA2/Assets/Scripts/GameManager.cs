using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GOOGAZ.Config;

namespace GOOGAZ
{
    /// <summary>
    /// Gestor principal del juego GOOGAZ
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private IngredientConfig ingredientDB;
        [SerializeField] private RecipeConfig recipeDB;
        [SerializeField] private ScenarioConfig scenarioDB;
        [SerializeField] private ScoringConfig scoringSystem;
        [SerializeField] private PlayerProgressConfig playerProgress;
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Button startButton;
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            Debug.Log("=== GOOGAZ - Inicializando ===");
            
            // Verificar configuraciones
            if (gameConfig == null)
            {
                Debug.LogWarning("GameConfig no asignado. Crea uno en: Assets > Create > GOOGAZ > Config > Game Config");
                UpdateStatus("⚠️ Falta GameConfig\nCrea uno en:\nAssets > Create > GOOGAZ > Config > Game Config");
                return;
            }
            
            if (ingredientDB == null)
            {
                Debug.LogWarning("IngredientConfig no asignado.");
                UpdateStatus("⚠️ Falta IngredientConfig\nCrea uno en:\nAssets > Create > GOOGAZ > Config > Ingredient Database");
                return;
            }
            
            if (recipeDB == null)
            {
                Debug.LogWarning("RecipeConfig no asignado.");
                UpdateStatus("⚠️ Falta RecipeConfig\nCrea uno en:\nAssets > Create > GOOGAZ > Config > Recipe Database");
                return;
            }
            
            if (scenarioDB == null)
            {
                Debug.LogWarning("ScenarioConfig no asignado.");
                UpdateStatus("⚠️ Falta ScenarioConfig\nCrea uno en:\nAssets > Create > GOOGAZ > Config > Scenario Database");
                return;
            }
            
            // Todo configurado correctamente
            Debug.Log($"✓ {gameConfig.gameName} v{gameConfig.version}");
            Debug.Log($"✓ Ingredientes: {ingredientDB.ingredientes.Count}");
            Debug.Log($"✓ Recetas: {recipeDB.recetas.Count}");
            Debug.Log($"✓ Escenarios: {scenarioDB.escenarios.Count}");
            
            UpdateStatus($"✅ Sistema inicializado correctamente!\n\n" +
                        $"📊 Ingredientes: {ingredientDB.ingredientes.Count}\n" +
                        $"🍳 Recetas: {recipeDB.recetas.Count}\n" +
                        $"🎯 Escenarios: {scenarioDB.escenarios.Count}\n\n" +
                        $"Presiona 'Iniciar' para comenzar");
            
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartGame);
                startButton.interactable = true;
            }
        }
        
        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log(message);
        }
        
        private void OnStartGame()
        {
            Debug.Log("🎮 Iniciando juego...");
            UpdateStatus("🎮 Iniciando juego...\n\n(Aquí irá el menú de escenarios)");
            
            // Aquí irá la lógica para mostrar el menú de escenarios
            if (scenarioDB != null && scenarioDB.escenarios.Count > 0)
            {
                var escenario = scenarioDB.escenarios[0];
                UpdateStatus($"📖 Escenario: {escenario.nombre}\n\n{escenario.descripcion}\n\n{escenario.situacion}");
            }
        }
        
    }
}
