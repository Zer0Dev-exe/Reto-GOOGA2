using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace GOOGAZ
{
    /// <summary>
    /// Script para crear automáticamente la UI básica del juego
    /// Ejecutar en Unity: GameObject > GOOGAZ > Setup Basic UI
    /// </summary>
    public class UISetup
    {
        [MenuItem("GameObject/GOOGAZ/Setup Basic UI")]
        public static void CreateBasicUI()
        {
            // Crear Canvas principal
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Crear Panel de fondo
            GameObject panelGO = new GameObject("Background");
            panelGO.transform.SetParent(canvasGO.transform);
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.15f, 0.25f, 1f); // Azul oscuro
            
            RectTransform panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Crear Título
            GameObject titleGO = new GameObject("TitleText");
            titleGO.transform.SetParent(canvasGO.transform);
            TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "GOOGAZ";
            titleText.fontSize = 80;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.3f, 0.8f, 0.4f, 1f); // Verde
            titleText.fontStyle = FontStyles.Bold;
            
            RectTransform titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleRect.anchorMax = new Vector2(0.5f, 0.8f);
            titleRect.sizeDelta = new Vector2(800, 120);
            titleRect.anchoredPosition = Vector2.zero;
            
            // Crear Subtítulo
            GameObject subtitleGO = new GameObject("SubtitleText");
            subtitleGO.transform.SetParent(canvasGO.transform);
            TextMeshProUGUI subtitleText = subtitleGO.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "Juego de Nutrición";
            subtitleText.fontSize = 36;
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            
            RectTransform subtitleRect = subtitleGO.GetComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0.5f, 0.72f);
            subtitleRect.anchorMax = new Vector2(0.5f, 0.72f);
            subtitleRect.sizeDelta = new Vector2(600, 60);
            subtitleRect.anchoredPosition = Vector2.zero;
            
            // Crear texto de estado
            GameObject statusGO = new GameObject("StatusText");
            statusGO.transform.SetParent(canvasGO.transform);
            TextMeshProUGUI statusText = statusGO.AddComponent<TextMeshProUGUI>();
            statusText.text = "Inicializando sistema...";
            statusText.fontSize = 28;
            statusText.alignment = TextAlignmentOptions.Center;
            statusText.color = Color.white;
            
            RectTransform statusRect = statusGO.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0.5f, 0.5f);
            statusRect.anchorMax = new Vector2(0.5f, 0.5f);
            statusRect.sizeDelta = new Vector2(1000, 400);
            statusRect.anchoredPosition = Vector2.zero;
            
            // Crear botón de inicio
            GameObject buttonGO = new GameObject("StartButton");
            buttonGO.transform.SetParent(canvasGO.transform);
            Button button = buttonGO.AddComponent<Button>();
            Image buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.8f, 0.4f, 1f);
            
            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.2f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.2f);
            buttonRect.sizeDelta = new Vector2(300, 80);
            buttonRect.anchoredPosition = Vector2.zero;
            
            // Texto del botón
            GameObject buttonTextGO = new GameObject("Text");
            buttonTextGO.transform.SetParent(buttonGO.transform);
            TextMeshProUGUI buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
            buttonText.text = "INICIAR";
            buttonText.fontSize = 36;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            buttonText.fontStyle = FontStyles.Bold;
            
            RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            button.interactable = false; // Se activará cuando el GameManager esté listo
            
            // Crear GameManager
            GameObject gameManagerGO = new GameObject("GameManager");
            GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
            
            // Asignar referencias mediante reflexión
            var titleField = typeof(GameManager).GetField("titleText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var statusField = typeof(GameManager).GetField("statusText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var buttonField = typeof(GameManager).GetField("startButton", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (titleField != null) titleField.SetValue(gameManager, titleText);
            if (statusField != null) statusField.SetValue(gameManager, statusText);
            if (buttonField != null) buttonField.SetValue(gameManager, button);
            
            Debug.Log("✅ UI básica creada correctamente!");
            Debug.Log("📝 Ahora asigna los ScriptableObjects en el GameManager:");
            Debug.Log("   1. Selecciona el GameObject 'GameManager'");
            Debug.Log("   2. En el Inspector, arrastra los configs creados");
            
            Selection.activeGameObject = gameManagerGO;
        }
    }
}
