using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Script de prueba para verificar que los botones funcionan
/// </summary>
public class TestButton : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== TEST BUTTON INICIADO ===");
        
        // Destruir cualquier Canvas existente
        Canvas[] existingCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas c in existingCanvases)
        {
            Debug.Log($"Destruyendo Canvas existente: {c.gameObject.name}");
            Destroy(c.gameObject);
        }
        
        // Crear EventSystem si no existe
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            EventSystem eventSystem = eventSystemGO.AddComponent<EventSystem>();
            StandaloneInputModule inputModule = eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("✅ EventSystem creado con StandaloneInputModule!");
        }
        else
        {
            Debug.Log("EventSystem ya existe");
        }
        
        // Esperar un frame antes de crear la UI
        StartCoroutine(CreateUIAfterFrame());
    }
    
    private System.Collections.IEnumerator CreateUIAfterFrame()
    {
        yield return null; // Esperar un frame
        
        Debug.Log("Creando UI...");
        
        // Crear Canvas
        GameObject canvasGO = new GameObject("MainCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Asegurar que esté al frente
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        
        Debug.Log("✅ Canvas creado");
        
        // Fondo
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.15f, 0.25f, 1f);
        
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Título
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "GOOGAZ";
        titleText.fontSize = 80;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.3f, 0.8f, 0.4f, 1f);
        titleText.fontStyle = FontStyles.Bold;
        titleText.raycastTarget = false;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(800, 150);
        titleRect.anchoredPosition = Vector2.zero;
        
        // Mensaje
        GameObject msgGO = new GameObject("Message");
        msgGO.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI msgText = msgGO.AddComponent<TextMeshProUGUI>();
        msgText.text = "JUEGO FUNCIONANDO!\n\nJuego de Nutricion\n\nPresiona el boton para empezar";
        msgText.fontSize = 32;
        msgText.alignment = TextAlignmentOptions.Center;
        msgText.color = Color.white;
        msgText.raycastTarget = false;
        
        RectTransform msgRect = msgGO.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.pivot = new Vector2(0.5f, 0.5f);
        msgRect.sizeDelta = new Vector2(1000, 300);
        msgRect.anchoredPosition = Vector2.zero;
        
        Debug.Log("✅ Textos creados");
        
        // Botón
        GameObject btnGO = new GameObject("StartButton");
        btnGO.transform.SetParent(canvasGO.transform, false);
        
        // Primero la imagen
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.3f, 0.8f, 0.4f, 1f);
        btnImage.raycastTarget = true; // IMPORTANTE: El botón debe recibir raycast
        
        // Luego el botón
        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        
        // Configurar colores
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 1f);
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.selectedColor = new Color(1f, 1f, 1f, 1f);
        colors.colorMultiplier = 1f;
        btn.colors = colors;
        
        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.3f);
        btnRect.anchorMax = new Vector2(0.5f, 0.3f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
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
        btnText.raycastTarget = false; // El texto NO debe bloquear clicks
        
        RectTransform btnTextRect = btnTextGO.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.pivot = new Vector2(0.5f, 0.5f);
        btnTextRect.sizeDelta = Vector2.zero;
        btnTextRect.anchoredPosition = Vector2.zero;
        
        Debug.Log("✅ Botón creado");
        
        // Listener del botón
        btn.onClick.AddListener(() => {
            Debug.Log("🎉🎉🎉 ¡¡¡BOTON PRESIONADO!!! 🎉🎉🎉");
            msgText.text = "¡JUEGO INICIADO!\n\nEscenario: Adolescencia y Estres\n\nCrea recetas saludables para ayudar a Maria";
            btnText.text = "¡FUNCIONANDO!";
            btnImage.color = new Color(0.2f, 0.6f, 0.3f, 1f);
        });
        
        Debug.Log("✅✅✅ TODO CREADO CORRECTAMENTE ✅✅✅");
        Debug.Log($"Canvas activo: {canvas.gameObject.activeInHierarchy}");
        Debug.Log($"Botón activo: {btn.gameObject.activeInHierarchy}");
        Debug.Log($"EventSystem presente: {FindObjectOfType<EventSystem>() != null}");
    }
}
