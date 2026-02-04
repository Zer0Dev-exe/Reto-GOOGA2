using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Juego simple que funciona AL DARLE AL PLAY
/// </summary>
public class SimpleGame : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== INICIANDO GOOGAZ ===");
        
        // Crear EventSystem si no existe (NECESARIO para que funcionen los botones)
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("EventSystem creado!");
        }
        
        // Crear Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Fondo
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.15f, 0.25f, 1f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Título
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(canvasGO.transform);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "GOOGAZ";
        titleText.fontSize = 80;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.3f, 0.8f, 0.4f, 1f);
        titleText.fontStyle = FontStyles.Bold;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(800, 150);
        titleRect.anchoredPosition = Vector2.zero;
        
        // Mensaje
        GameObject msgGO = new GameObject("Message");
        msgGO.transform.SetParent(canvasGO.transform);
        TextMeshProUGUI msgText = msgGO.AddComponent<TextMeshProUGUI>();
        msgText.text = "JUEGO FUNCIONANDO!\n\nJuego de Nutricion\n\nPresiona el boton para empezar";
        msgText.fontSize = 32;
        msgText.alignment = TextAlignmentOptions.Center;
        msgText.color = Color.white;
        RectTransform msgRect = msgGO.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.sizeDelta = new Vector2(1000, 300);
        msgRect.anchoredPosition = Vector2.zero;
        
        // Botón
        GameObject btnGO = new GameObject("Button");
        btnGO.transform.SetParent(canvasGO.transform);
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.3f, 0.8f, 0.4f, 1f);
        Button btn = btnGO.AddComponent<Button>();
        
        // Configurar transición del botón
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.3f, 0.8f, 0.4f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.9f, 0.5f, 1f);
        colors.pressedColor = new Color(0.2f, 0.6f, 0.3f, 1f);
        colors.selectedColor = new Color(0.3f, 0.8f, 0.4f, 1f);
        btn.colors = colors;
        
        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.3f);
        btnRect.anchorMax = new Vector2(0.5f, 0.3f);
        btnRect.sizeDelta = new Vector2(400, 100);
        btnRect.anchoredPosition = Vector2.zero;
        
        // Texto del botón
        GameObject btnTextGO = new GameObject("ButtonText");
        btnTextGO.transform.SetParent(btnGO.transform, false);
        TextMeshProUGUI btnText = btnTextGO.AddComponent<TextMeshProUGUI>();
        btnText.text = "INICIAR JUEGO";
        btnText.fontSize = 32;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;
        btnText.fontStyle = FontStyles.Bold;
        btnText.raycastTarget = false; // IMPORTANTE: El texto no debe bloquear los clicks
        
        RectTransform btnTextRect = btnTextGO.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.sizeDelta = Vector2.zero;
        btnTextRect.anchoredPosition = Vector2.zero;
        
        // Listener del botón
        btn.onClick.AddListener(() => {
            Debug.Log("¡¡¡BOTON PRESIONADO!!!");
            msgText.text = "JUEGO INICIADO!\n\nEscenario: Adolescencia y Estres\n\nCrea recetas saludables para ayudar a Maria";
            btnText.text = "¡FUNCIONANDO!";
        });
        
        Debug.Log("=== GOOGAZ CREADO CORRECTAMENTE ===");
    }
}
