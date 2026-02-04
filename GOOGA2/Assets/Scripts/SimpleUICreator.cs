using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GOOGAZ
{
    /// <summary>
    /// Script simplificado para crear la UI básica
    /// Adjunta este script a un GameObject vacío y presiona Play
    /// </summary>
    public class SimpleUICreator : MonoBehaviour
    {
        private void Start()
        {
            CreateUI();
        }
        
        private void CreateUI()
        {
            // Verificar si ya existe un Canvas
            Canvas existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas != null)
            {
                Debug.Log("Ya existe un Canvas en la escena");
                return;
            }
            
            // Crear Canvas principal
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Crear Panel de fondo con gradiente
            GameObject panelGO = new GameObject("Background");
            panelGO.transform.SetParent(canvasGO.transform, false);
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0.15f, 0.2f, 0.3f, 1f);
            
            RectTransform panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Crear Título
            CreateText(canvasGO.transform, "TitleText", "GOOGAZ", 
                new Vector2(0.5f, 0.75f), new Vector2(800, 120), 72, 
                new Color(0.4f, 0.9f, 0.5f, 1f), TextAlignmentOptions.Center, FontStyles.Bold);
            
            // Crear Subtítulo
            CreateText(canvasGO.transform, "SubtitleText", "Juego de Nutrición Interactivo", 
                new Vector2(0.5f, 0.68f), new Vector2(600, 60), 32, 
                new Color(0.8f, 0.8f, 0.8f, 1f), TextAlignmentOptions.Center, FontStyles.Normal);
            
            // Crear texto de estado
            TextMeshProUGUI statusText = CreateText(canvasGO.transform, "StatusText", 
                "🎮 Bienvenido a GOOGAZ\n\nPara comenzar:\n1. Crea los ScriptableObjects de configuración\n2. Asígnalos al GameManager\n3. Presiona el botón INICIAR", 
                new Vector2(0.5f, 0.45f), new Vector2(1000, 400), 26, 
                Color.white, TextAlignmentOptions.Center, FontStyles.Normal);
            
            // Crear botón de inicio
            GameObject buttonGO = CreateButton(canvasGO.transform, "StartButton", "INICIAR", 
                new Vector2(0.5f, 0.2f), new Vector2(300, 80), 
                new Color(0.4f, 0.9f, 0.5f, 1f));
            
            Button button = buttonGO.GetComponent<Button>();
            button.interactable = false;
            
            // Crear GameManager si no existe
            GameManager existingGM = FindObjectOfType<GameManager>();
            if (existingGM == null)
            {
                GameObject gameManagerGO = new GameObject("GameManager");
                GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
                
                // Intentar asignar referencias usando reflexión
                try
                {
                    var statusField = typeof(GameManager).GetField("statusText", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var buttonField = typeof(GameManager).GetField("startButton", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (statusField != null) statusField.SetValue(gameManager, statusText);
                    if (buttonField != null) buttonField.SetValue(gameManager, button);
                    
                    Debug.Log("✅ UI y GameManager creados correctamente!");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"No se pudieron asignar referencias automáticamente: {e.Message}");
                }
            }
            
            Debug.Log("✅ UI básica creada! Ahora crea los ScriptableObjects de configuración.");
            
            // Destruir este script después de crear la UI
            Destroy(this);
        }
        
        private TextMeshProUGUI CreateText(Transform parent, string name, string text, 
            Vector2 anchorPos, Vector2 size, float fontSize, Color color, 
            TextAlignmentOptions alignment, FontStyles style)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            
            TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = color;
            tmp.fontStyle = style;
            
            RectTransform rect = textGO.GetComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            
            return tmp;
        }
        
        private GameObject CreateButton(Transform parent, string name, string buttonText, 
            Vector2 anchorPos, Vector2 size, Color color)
        {
            GameObject buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            
            Button button = buttonGO.AddComponent<Button>();
            Image buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = color;
            
            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
            buttonRect.anchorMin = anchorPos;
            buttonRect.anchorMax = anchorPos;
            buttonRect.sizeDelta = size;
            buttonRect.anchoredPosition = Vector2.zero;
            
            // Texto del botón
            CreateText(buttonGO.transform, "Text", buttonText, 
                new Vector2(0.5f, 0.5f), size, 32, Color.white, 
                TextAlignmentOptions.Center, FontStyles.Bold);
            
            return buttonGO;
        }
    }
}
