using UnityEngine;
using UnityEngine.UI;

namespace GOOGAZ
{
    /// <summary>
    /// Versión ultra-simple sin TextMeshPro
    /// </summary>
    public class BasicUISetup : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("=== Creando UI básica ===");
            
            // Verificar si ya existe Canvas
            if (FindObjectOfType<Canvas>() != null)
            {
                Debug.Log("Ya existe un Canvas");
                Destroy(gameObject);
                return;
            }
            
            // Crear Canvas
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Crear panel de fondo
            GameObject panel = new GameObject("Background");
            panel.transform.SetParent(canvasGO.transform, false);
            
            Image img = panel.AddComponent<Image>();
            img.color = new Color(0.2f, 0.3f, 0.4f, 1f);
            
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            
            // Crear texto simple
            GameObject textGO = new GameObject("WelcomeText");
            textGO.transform.SetParent(canvasGO.transform, false);
            
            Text text = textGO.AddComponent<Text>();
            text.text = "GOOGAZ - Juego de Nutrición\n\nUI Creada Correctamente!\n\nAhora crea los ScriptableObjects:\n1. GameConfig\n2. IngredientDatabase\n3. RecipeDatabase\n4. ScenarioDatabase";
            text.fontSize = 32;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = new Vector2(0.5f, 0.5f);
            textRT.anchorMax = new Vector2(0.5f, 0.5f);
            textRT.sizeDelta = new Vector2(800, 400);
            textRT.anchoredPosition = Vector2.zero;
            
            // Crear botón
            GameObject buttonGO = new GameObject("StartButton");
            buttonGO.transform.SetParent(canvasGO.transform, false);
            
            Button button = buttonGO.AddComponent<Button>();
            Image btnImg = buttonGO.AddComponent<Image>();
            btnImg.color = new Color(0.3f, 0.8f, 0.3f, 1f);
            
            RectTransform btnRT = buttonGO.GetComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(0.5f, 0.3f);
            btnRT.anchorMax = new Vector2(0.5f, 0.3f);
            btnRT.sizeDelta = new Vector2(200, 60);
            btnRT.anchoredPosition = Vector2.zero;
            
            // Texto del botón
            GameObject btnTextGO = new GameObject("Text");
            btnTextGO.transform.SetParent(buttonGO.transform, false);
            
            Text btnText = btnTextGO.AddComponent<Text>();
            btnText.text = "INICIAR";
            btnText.fontSize = 24;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.color = Color.white;
            btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
            btnTextRT.anchorMin = Vector2.zero;
            btnTextRT.anchorMax = Vector2.one;
            btnTextRT.sizeDelta = Vector2.zero;
            
            button.onClick.AddListener(() => {
                Debug.Log("Botón presionado! Aquí irá el menú de escenarios");
                text.text = "¡Botón funcionando!\n\nAhora necesitas:\n1. Crear los ScriptableObjects\n2. Asignarlos al GameManager\n3. Añadir contenido (ingredientes, recetas, escenarios)";
            });
            
            Debug.Log("✅ UI creada correctamente!");
            Debug.Log("Detén el juego (Stop) y guarda la escena (Ctrl+S)");
            
            // Auto-destruir este script
            Destroy(this);
        }
    }
}
