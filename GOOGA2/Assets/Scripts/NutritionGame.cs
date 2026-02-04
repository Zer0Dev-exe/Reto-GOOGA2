using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Juego completo de nutrición GOOGAZ
/// Implementa los 3 escenarios del README con sistema de puntuación
/// </summary>
public class NutritionGame : MonoBehaviour
{
    // Datos del juego
    private int currentScenarioIndex = 0;
    private List<string> selectedIngredients = new List<string>();
    private GamePhase currentPhase = GamePhase.Menu;
    
    // UI References
    private Canvas mainCanvas;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI descriptionText;
    private GameObject menuPanel;
    private GameObject learningPanel;
    private GameObject cookingPanel;
    private GameObject resultsPanel;
    
    private enum GamePhase
    {
        Menu,
        Learning,
        Cooking,
        Results
    }
    
    // Datos de escenarios
    private Scenario[] scenarios = new Scenario[]
    {
        new Scenario
        {
            name = "Embarazo y Deporte",
            description = "Necesidades nutricionales antes y después de realizar actividad física",
            learningRecipes = new string[]
            {
                "🍞 Desayuno: Tostada (queso cottage + fruta)",
                "🥣 Desayuno: Bowl avena templada (manzana + nueces)",
                "🥗 Comida: Bowl de quinoa (verduras + pollo)",
                "🍲 Comida: Crema de calabaza y zanahoria",
                "🐟 Cena: Puré de boniato con merluza al vapor",
                "🍽️ Cena: Filete de salmón con puré y verduras"
            },
            requiredIngredients = new string[] { "quinoa", "pollo", "calabaza", "salmón", "boniato", "verduras" },
            goodIngredients = new string[] { "avena", "nueces", "fruta", "queso", "merluza", "zanahoria" },
            badIngredients = new string[] { "frituras", "azúcar", "alcohol", "procesados" }
        },
        new Scenario
        {
            name = "Adolescencia y Estrés",
            description = "Combatir el estrés a través de la alimentación",
            learningRecipes = new string[]
            {
                "🥣 Desayuno: Bowl de avena (leche + fresas + arándanos + almendras)",
                "🍞 Desayuno: Tostada integral (queso fresco + membrillo + nueces)",
                "🍲 Comida: Crema ligera con merluza",
                "🫘 Comida: Guiso de lentejas (calabaza + zanahoria)",
                "🥗 Cena: Ensalada de verano",
                "🐟 Cena: Rodaballo al vapor con verduras"
            },
            requiredIngredients = new string[] { "avena", "fresas", "arándanos", "almendras", "lentejas", "merluza" },
            goodIngredients = new string[] { "nueces", "queso fresco", "calabaza", "zanahoria", "verduras", "rodaballo" },
            badIngredients = new string[] { "café", "energéticas", "azúcar", "frituras" }
        },
        new Scenario
        {
            name = "Senectud - Gestión de Migraña",
            description = "Alimentación para la tercera edad con gestión de migraña",
            learningRecipes = new string[]
            {
                "🥣 Desayuno: Porridge de avena con fruta",
                "🥤 Desayuno: Batido suave (pera + yogurt proteico + avena)",
                "🍅 Comida: Gazpacho",
                "🍲 Comida: Sopa de verduras",
                "🍗 Cena: Pollo guisado (calabaza + boniato)",
                "🍳 Cena: Tortilla francesa (calabacín + tomate de temporada)"
            },
            requiredIngredients = new string[] { "avena", "pera", "yogurt", "verduras", "pollo", "calabaza" },
            goodIngredients = new string[] { "fruta", "tomate", "calabacín", "boniato", "sopa" },
            badIngredients = new string[] { "queso curado", "chocolate", "vino", "embutidos", "cítricos" }
        }
    };
    
    // Ingredientes disponibles
    private string[] availableIngredients = new string[]
    {
        "avena", "quinoa", "arroz", "pasta",
        "pollo", "salmón", "merluza", "rodaballo",
        "lentejas", "garbanzos",
        "calabaza", "zanahoria", "tomate", "calabacín", "verduras",
        "fresas", "arándanos", "manzana", "pera", "fruta",
        "almendras", "nueces",
        "queso", "queso fresco", "yogurt",
        "boniato", "patata"
    };
    
    private void Start()
    {
        CreateEventSystem();
        CreateUI();
        ShowMenu();
        
        // Cargar fuente pixel art si existe (Opcional, usa default si no)
        // TMP_FontAsset pixelFont = Resources.Load<TMP_FontAsset>("Fonts/PixelFont");
    }
    
    private void CreateEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }
    
    private void CreateUI()
    {
        // Canvas principal
        GameObject canvasGO = new GameObject("GameCanvas");
        mainCanvas = canvasGO.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Ya no creamos fondo global aquí, cada fase tiene su fondo
        
        // Crear paneles
        menuPanel = CreatePanel(canvasGO.transform, "MenuPanel");
        learningPanel = CreatePanel(canvasGO.transform, "LearningPanel");
        cookingPanel = CreatePanel(canvasGO.transform, "CookingPanel");
        resultsPanel = CreatePanel(canvasGO.transform, "ResultsPanel");
    }
    
    private GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        panel.SetActive(false);
        return panel;
    }
    
    private void ShowMenu()
    {
        currentPhase = GamePhase.Menu;
        HideAllPanels();
        menuPanel.SetActive(true);
        
        foreach (Transform child in menuPanel.transform)
            Destroy(child.gameObject);
        
        // Fondo con imagen
        CreateBackgroundImage(menuPanel.transform, "Images/Backgrounds/menu_bg");
        
        // Título con efecto de brillo y tamaño ajustado
        GameObject titleBox = CreateFancyTitleBox(menuPanel.transform);
        RectTransform tRect = titleBox.GetComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0.2f, 0.7f); // Subir un poco y hacerlo más estrecho
        tRect.anchorMax = new Vector2(0.8f, 0.9f);
        
        // Título principal del juego con sombra
        CreateTextWithShadow(titleBox.transform, "GOOGAZ", new Vector2(0.5f, 0.5f), new Vector2(800, 120), 100, 
            new Color(1f, 0.95f, 0.2f), FontStyles.Bold, new Color(0.8f, 0.4f, 0f));
        
        // Botones de Escenarios
        float yPos = 0.5f;
        string[] labels = { "GIMNASIO (Embarazo)", "INSTITUTO (Estrés)", "CASA ABUELOS (Migraña)" };
        
        for (int i = 0; i < scenarios.Length; i++)
        {
            int index = i;
            CreateRetroButton(menuPanel.transform, labels[i], new Color(0.3f, 0.7f, 0.4f), new Vector2(0.5f, yPos), new Vector2(500, 70), () => StartScenario(index));
            yPos -= 0.15f;
        }
    }
    
    private void StartScenario(int scenarioIndex)
    {
        currentScenarioIndex = scenarioIndex;
        selectedIngredients.Clear();
        ShowLearningPhase();
    }
    
    private void ShowLearningPhase()
    {
        currentPhase = GamePhase.Learning;
        HideAllPanels();
        learningPanel.SetActive(true);
        
        foreach (Transform child in learningPanel.transform)
            Destroy(child.gameObject);
        
        Scenario scenario = scenarios[currentScenarioIndex];
        
        // Fondo con imagen de aula
        CreateBackgroundImage(learningPanel.transform, "Images/Backgrounds/learning_bg");
        
        // Título de la Misión
        CreateText(learningPanel.transform, $"MISIÓN: {scenario.name}", new Vector2(0.5f, 0.9f), new Vector2(1000, 80), 40, Color.white, FontStyles.Bold);
        
        // Nota de Papel con sombra
        GameObject paperShadow = new GameObject("PaperShadow");
        paperShadow.transform.SetParent(learningPanel.transform, false);
        Image pShadowImg = paperShadow.AddComponent<Image>();
        pShadowImg.color = new Color(0f, 0f, 0f, 0.4f);
        RectTransform pShadowRect = paperShadow.GetComponent<RectTransform>();
        pShadowRect.anchorMin = new Vector2(0.15f, 0.25f);
        pShadowRect.anchorMax = new Vector2(0.85f, 0.8f);
        pShadowRect.anchoredPosition = new Vector2(5, -5);
        
        GameObject paper = new GameObject("PaperNote");
        paper.transform.SetParent(learningPanel.transform, false);
        Image pImg = paper.AddComponent<Image>();
        pImg.color = new Color(0.98f, 0.96f, 0.88f, 1f); // Color papel más cálido
        
        // Borde de la nota
        Outline paperOutline = paper.AddComponent<Outline>();
        paperOutline.effectColor = new Color(0.7f, 0.6f, 0.4f);
        paperOutline.effectDistance = new Vector2(3, -3);
        
        RectTransform pRect = paper.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.15f, 0.25f);
        pRect.anchorMax = new Vector2(0.85f, 0.8f);
        
        // Texto Recetas con Formato Rico (Estilo Nota Real)
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("<align=center><color=#8B4513><size=32><b>📋 MENÚ RECOMENDADO</b></size></color></align>\n\n");
        sb.Append("<line-height=150%>"); // Más interlineado para legibilidad
        
        foreach (string recipe in scenario.learningRecipes)
        {
            // Formatear cabeceras (Desayuno, Comida, Cena)
            string line = recipe;
            if (line.Contains(":"))
            {
                string[] parts = line.Split(':');
                // Parte 1 (Ej: Desayuno) en Rojo Oscuro y Negrita
                // Parte 2 (La receta) en Azul Tinta
                line = $"<color=#8B0000><b>• {parts[0]}:</b></color> <color=#1a1a2e>{parts[1]}</color>";
            }
            else
            {
                 line = $"• {line}";
            }
            sb.AppendLine(line);
        }
                             
        var tmp = CreateText(paper.transform, sb.ToString(), new Vector2(0.5f, 0.5f), Vector2.zero, 22, new Color(0.1f, 0.1f, 0.2f), FontStyles.Normal);
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.lineSpacing = 20; // Espaciado extra
        tmp.richText = true; // Asegurar que interpreta los colores
        tmp.margin = new Vector4(50, 40, 50, 40);
        RectTransform tr = tmp.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.sizeDelta = Vector2.zero;
        
        // Botones
        CreateRetroButton(learningPanel.transform, "¡IR A LA TIENDA!", new Color(0.9f, 0.8f, 0.2f), new Vector2(0.5f, 0.12f), new Vector2(400, 70), ShowCookingPhase);
        CreateRetroButton(learningPanel.transform, "Atrás", new Color(0.7f, 0.7f, 0.7f), new Vector2(0.1f, 0.1f), new Vector2(120, 50), ShowMenu);
    }
    
    private void ShowCookingPhase()
    {
        currentPhase = GamePhase.Cooking;
        HideAllPanels();
        cookingPanel.SetActive(true);
        
        foreach (Transform child in cookingPanel.transform)
            Destroy(child.gameObject);
        
        Scenario scenario = scenarios[currentScenarioIndex];
        
        // Fondo con imagen de tienda
        CreateBackgroundImage(cookingPanel.transform, "Images/Backgrounds/shop_bg");
        
        // Mostrador
        GameObject counterPanel = CreatePanel(cookingPanel.transform, "CounterPanel");
        counterPanel.SetActive(true); // ¡IMPORTANTE ACTIVARLO!
        RectTransform counterRect = counterPanel.GetComponent<RectTransform>();
        counterRect.anchorMin = new Vector2(0, 0.65f);
        counterRect.anchorMax = new Vector2(1, 1);
        counterRect.offsetMin = Vector2.zero;
        counterRect.offsetMax = Vector2.zero;
        
        CreateImage(counterPanel.transform, new Color(0.55f, 0.4f, 0.3f, 1f), new Vector2(0, 0), new Vector2(1, 0.2f));
        
        // Caja de Diálogo Superior
        GameObject dialogBox = CreateDialogBox(counterPanel.transform, 
            $"¡Hola! Necesito ayuda con: <color=#FFFF00>{scenario.name}</color>\n\n" +
            $"<size=24><i>{scenario.description}</i></size>");
        RectTransform dialogRect = dialogBox.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.1f, 0.25f);
        dialogRect.anchorMax = new Vector2(0.9f, 0.9f);
        
        // Estanterías
        GameObject shelvesPanel = CreatePanel(cookingPanel.transform, "ShelvesPanel");
        shelvesPanel.SetActive(true); // ¡IMPORTANTE ACTIVARLO!
        RectTransform shelvesRect = shelvesPanel.GetComponent<RectTransform>();
        shelvesRect.anchorMin = new Vector2(0, 0);
        shelvesRect.anchorMax = new Vector2(0.7f, 0.65f);
        
        CreateText(shelvesPanel.transform, "PRODUCTOS DISPONIBLES", new Vector2(0.5f, 0.95f), new Vector2(400, 40), 24, Color.white, FontStyles.Bold);
        
        // Grid Productos (Rediseñado para 5 columnas)
        float startX = 0.12f;
        float startY = 0.82f;
        int cols = 5; // Más columnas para que quepan
        float xStep = 0.19f;
        float yStep = 0.16f;
        
        int itemIndex = 0;
        foreach (string ingredient in availableIngredients)
        {
            int r = itemIndex / cols;
            int c = itemIndex % cols;
            
            float x = startX + (c * xStep);
            float y = startY - (r * yStep);
            
            // Solo dibujar si estamos dentro del panel (limitamos filas si hay demasiados)
            if (y > 0.1f)
            {
                CreateShopItem(shelvesPanel.transform, ingredient, new Vector2(x, y));
            }
            itemIndex++;
        }
        
        // Cesta Compra
        GameObject cartPanel = CreateDialogBox(cookingPanel.transform, "");
        RectTransform cartRect = cartPanel.GetComponent<RectTransform>();
        cartRect.anchorMin = new Vector2(0.72f, 0.1f);
        cartRect.anchorMax = new Vector2(0.98f, 0.65f);
        
        CreateText(cartPanel.transform, "TU CESTA", new Vector2(0.5f, 0.92f), new Vector2(200, 40), 28, Color.white, FontStyles.Bold);
        
        GameObject cartItemsContainer = new GameObject("CartItemsContainer");
        cartItemsContainer.transform.SetParent(cartPanel.transform, false);
        RectTransform containerRect = cartItemsContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.05f, 0.15f);
        containerRect.anchorMax = new Vector2(0.95f, 0.85f);
        
        currentCartContainer = cartItemsContainer;
        UpdateCartDisplay();
        
        // Botones Acción
        CreateRetroButton(cookingPanel.transform, "¡Comprar!", new Color(0.2f, 0.8f, 0.2f), new Vector2(0.85f, 0.05f), new Vector2(200, 60), EvaluateRecipe);
        CreateRetroButton(cookingPanel.transform, "Volver", new Color(0.8f, 0.3f, 0.3f), new Vector2(0.1f, 0.05f), new Vector2(150, 50), ShowLearningPhase);
    }

    private GameObject currentCartContainer;

    private GameObject CreateDialogBox(Transform parent, string text)
    {
        GameObject boxGO = new GameObject("DialogBox", typeof(RectTransform));
        boxGO.transform.SetParent(parent, false);
        
        // Sombra del recuadro
        GameObject shadowGO = new GameObject("Shadow");
        shadowGO.transform.SetParent(boxGO.transform, false);
        Image shadowImg = shadowGO.AddComponent<Image>();
        shadowImg.color = new Color(0f, 0f, 0f, 0.5f);
        RectTransform shadowRect = shadowGO.GetComponent<RectTransform>();
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        shadowRect.sizeDelta = new Vector2(8, -8);
        shadowRect.anchoredPosition = new Vector2(4, -4);
        
        // Borde dorado (capa intermedia)
        GameObject borderGO = new GameObject("Border");
        borderGO.transform.SetParent(boxGO.transform, false);
        Image borderImg = borderGO.AddComponent<Image>();
        borderImg.color = new Color(1f, 0.85f, 0.3f);
        RectTransform borderRect = borderGO.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(6, -6);
        
        // Fondo con gradiente azul oscuro a morado
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(boxGO.transform, false);
        Image boxImg = bgGO.AddComponent<Image>();
        boxImg.color = new Color(0.15f, 0.25f, 0.55f, 0.95f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Texto
        if (!string.IsNullOrEmpty(text))
        {
            TextMeshProUGUI tmp = CreateText(bgGO.transform, text, new Vector2(0.5f, 0.5f), Vector2.zero, 32, Color.white, FontStyles.Normal);
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.margin = new Vector4(30, 30, 30, 30);
            
            RectTransform textRect = tmp.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = new Vector2(-40, -40);
        }
        
        return boxGO;
    }

    private void CreateShopItem(Transform parent, string ingredient, Vector2 anchorPos)
    {
        GameObject itemGO = new GameObject($"Item_{ingredient}", typeof(RectTransform));
        itemGO.transform.SetParent(parent, false);
        
        RectTransform rect = itemGO.GetComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.sizeDelta = new Vector2(90, 110); // Más alto para el texto
        
        // Slot Background (Estilo Flat)
        Image bg = itemGO.AddComponent<Image>();
        bg.color = new Color(0.95f, 0.95f, 0.9f); // Blanco hueso
        
        // Icono (Color procedural)
        GameObject iconGO = new GameObject("Icon", typeof(RectTransform));
        iconGO.transform.SetParent(itemGO.transform, false);
        Image iconImg = iconGO.AddComponent<Image>();
        iconImg.sprite = GeneratePixelSprite(GetIngredientColor(ingredient));
        
        RectTransform iconRect = iconGO.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.7f);
        iconRect.anchorMax = new Vector2(0.5f, 0.7f);
        iconRect.sizeDelta = new Vector2(50, 50);
        
        // Nombre del producto debajo (Pequeño pero legible)
        CreateText(itemGO.transform, ingredient, new Vector2(0.5f, 0.25f), new Vector2(85, 40), 16, Color.black, FontStyles.Normal);
        
        // Botón invisible que cubre todo
        Button btn = itemGO.AddComponent<Button>();
        btn.targetGraphic = bg;
        
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 1f, 0.9f); // Verde suave al pasar ratón
        colors.pressedColor = new Color(0.7f, 0.9f, 0.7f);
        colors.selectedColor = Color.white;
        colors.colorMultiplier = 1f;
        btn.colors = colors;
        
        btn.onClick.AddListener(() => AddIngredientToCart(ingredient));
    }


    private void AddIngredientToCart(string ingredient)
    {
        if (!selectedIngredients.Contains(ingredient))
        {
            selectedIngredients.Add(ingredient);
            UpdateCartDisplay();
        }
    }

    private void UpdateCartDisplay()
    {
        if (currentCartContainer == null) return;
        
        foreach (Transform child in currentCartContainer.transform)
            Destroy(child.gameObject);
            
        float yPos = 0;
        foreach (string item in selectedIngredients)
        {
            CreateCartItem(currentCartContainer.transform, item, yPos);
            yPos -= 40;
        }
    }
    
    private void CreateCartItem(Transform parent, string item, float yOffset)
    {
        GameObject itemGO = new GameObject($"Cart_{item}");
        itemGO.transform.SetParent(parent, false);
        RectTransform rect = itemGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, yOffset);
        rect.sizeDelta = new Vector2(0, 35);
        
        TextMeshProUGUI text = CreateText(itemGO.transform, $"> {item}", new Vector2(0.5f, 0.5f), Vector2.zero, 20, Color.white, FontStyles.Normal);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        text.alignment = TextAlignmentOptions.Left;
        text.margin = new Vector4(10, 0, 0, 0);
    }
    
    private void CreateRetroButton(Transform parent, string text, Color color, Vector2 pos, Vector2 size, System.Action onClick)
    {
        GameObject btnGO = new GameObject($"Btn_{text}", typeof(RectTransform));
        btnGO.transform.SetParent(parent, false);
        
        // Sombra muy sutil y pequeña (casi invisible, estilo flat)
        GameObject shadowGO = new GameObject("Shadow");
        shadowGO.transform.SetParent(btnGO.transform, false);
        Image shadowImg = shadowGO.AddComponent<Image>();
        shadowImg.color = new Color(0f, 0f, 0f, 0.3f);
        RectTransform shadowRect = shadowGO.GetComponent<RectTransform>();
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        shadowRect.sizeDelta = new Vector2(2, -2); // Sombra mínima
        shadowRect.anchoredPosition = new Vector2(2, -2);
        
        // Borde fino negro
        GameObject borderGO = new GameObject("Border");
        borderGO.transform.SetParent(btnGO.transform, false);
        Image borderImg = borderGO.AddComponent<Image>();
        borderImg.color = Color.black;
        RectTransform borderRect = borderGO.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(2, -2); // Borde más fino (2px en lugar de 4px)
        
        // Fondo del botón plano
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(btnGO.transform, false);
        Image img = bgGO.AddComponent<Image>();
        img.color = color;
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = img;
        
        // Colores de interacción (Flat Design)
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f); // Brillo sutil
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Oscurecer al presionar
        colors.selectedColor = Color.white;
        colors.colorMultiplier = 1f;
        btn.colors = colors;
        
        btn.onClick.AddListener(() => onClick());
        
        RectTransform rect = btnGO.GetComponent<RectTransform>();
        rect.anchorMin = pos;
        rect.anchorMax = pos;
        rect.sizeDelta = size;
        
        // Texto plano sin sombra (o sombra mínima si es necesario contraste)
        // Usamos CreateText normal en lugar de CreateTextWithShadow para un look limpio
        CreateText(bgGO.transform, text, new Vector2(0.5f, 0.5f), size, 24, Color.white, FontStyles.Bold);
    }

    // Auxiliares rápidos
    private void CreateFullScreenPanel(Transform parent, Color color)
    {
        GameObject p = new GameObject("BG_Fill");
        p.transform.SetParent(parent, false);
        Image img = p.AddComponent<Image>();
        img.color = color;
        RectTransform r = p.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }
    
    private void CreateImage(Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject("Img");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = anchorMin;
        r.anchorMax = anchorMax;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }
    
    private void CreateBackgroundImage(Transform parent, string backgroundType)
    {
        GameObject bgGO = new GameObject("BG_Image");
        bgGO.transform.SetParent(parent, false);
        bgGO.transform.SetAsFirstSibling();
        
        Image img = bgGO.AddComponent<Image>();
        
        // Generar fondo procedural según el tipo
        Sprite bgSprite = null;
        
        if (backgroundType.Contains("menu"))
        {
            bgSprite = GenerateStarryNightBackground();
        }
        else if (backgroundType.Contains("learning"))
        {
            bgSprite = GenerateChalkboardBackground();
        }
        else if (backgroundType.Contains("shop"))
        {
            bgSprite = GenerateShopBackground();
        }
        
        if (bgSprite != null)
        {
            img.sprite = bgSprite;
            img.color = Color.white;
        }
        else
        {
            // Fallback
            img.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        }
        
        RectTransform r = bgGO.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }
    

    
    private Sprite GenerateChalkboardBackground()
    {
        int width = 512;
        int height = 512;
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;
        
        Color[] pixels = new Color[width * height];
        Color baseColor = new Color(0.15f, 0.25f, 0.2f); // Verde pizarra
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Textura con ruido
                float noise = Random.value * 0.05f;
                pixels[y * width + x] = baseColor + new Color(noise, noise, noise, 0);
            }
        }
        
        // Marco de madera (bordes)
        Color woodColor = new Color(0.4f, 0.25f, 0.15f);
        
        // Borde superior e inferior
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = woodColor;
                pixels[(height - 1 - y) * width + x] = woodColor;
            }
        }
        
        // Bordes laterales
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < height; y++)
            {
                pixels[y * width + x] = woodColor;
                pixels[y * width + (width - 1 - x)] = woodColor;
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    private Sprite GenerateShopBackground()
    {
        int width = 512;
        int height = 512;
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;
        
        Color[] pixels = new Color[width * height];
        Color floorColor = new Color(0.8f, 0.7f, 0.6f); // Beige claro
        Color wallColor = new Color(0.85f, 0.75f, 0.65f); // Beige más claro
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Pared arriba, suelo abajo
                if (y > height * 0.6f)
                {
                    pixels[y * width + x] = wallColor;
                }
                else
                {
                    pixels[y * width + x] = floorColor;
                }
                
                // Ruido sutil
                float noise = Random.value * 0.03f;
                pixels[y * width + x] += new Color(noise, noise, noise, 0);
            }
        }
        
        // Baldosas del suelo
        Color tileLineColor = new Color(0.6f, 0.5f, 0.4f);
        for (int y = 0; y < (int)(height * 0.6f); y += 64)
        {
            for (int x = 0; x < width; x++)
            {
                if (y < height)
                    pixels[y * width + x] = tileLineColor;
            }
        }
        
        for (int x = 0; x < width; x += 64)
        {
            for (int y = 0; y < (int)(height * 0.6f); y++)
            {
                pixels[y * width + x] = tileLineColor;
            }
        }
        
        // Estantería simple en la pared
        Color shelfColor = new Color(0.5f, 0.35f, 0.2f);
        for (int y = (int)(height * 0.7f); y < (int)(height * 0.75f); y++)
        {
            for (int x = 50; x < width - 50; x++)
            {
                pixels[y * width + x] = shelfColor;
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    private void CreateStar(Transform parent, Vector2 position, float size)
    {
        GameObject starGO = new GameObject("Star");
        starGO.transform.SetParent(parent, false);
        
        Image img = starGO.AddComponent<Image>();
        img.color = new Color(1f, 1f, 0.8f, Random.Range(0.3f, 0.9f));
        
        RectTransform r = starGO.GetComponent<RectTransform>();
        r.anchorMin = position;
        r.anchorMax = position;
        r.sizeDelta = new Vector2(size, size);
    }
    
    // Generador de Sprite "Procedural" (Crea un cuadradito de color con borde como si fuera pixel art básico)
    private Sprite GeneratePixelSprite(Color baseColor)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point; // Importante para look pixel
        
        Color[] colors = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Borde
                if (x < 4 || x >= size-4 || y < 4 || y >= size-4)
                    colors[y * size + x] = new Color(0,0,0,1); // Negro
                else
                    colors[y * size + x] = baseColor;
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    private void CreateIngredientButton(Transform parent, string ingredient, Vector2 anchorPos, GameObject selectedPanel)
    {
        // Tarjeta principal con sombra
        GameObject cardGO = new GameObject($"Card_{ingredient}");
        cardGO.transform.SetParent(parent, false);
        
        RectTransform cardRect = cardGO.AddComponent<RectTransform>();
        cardRect.anchorMin = anchorPos;
        cardRect.anchorMax = anchorPos;
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(160, 90);
        
        // Sombra
        GameObject shadowGO = new GameObject("Shadow");
        shadowGO.transform.SetParent(cardGO.transform, false);
        Image shadow = shadowGO.AddComponent<Image>();
        shadow.color = new Color(0f, 0f, 0f, 0.2f);
        RectTransform shadowRect = shadowGO.GetComponent<RectTransform>();
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        shadowRect.sizeDelta = new Vector2(6, -6);
        shadowRect.anchoredPosition = new Vector2(3, -3);
        
        // Botón con gradiente
        GameObject btnGO = new GameObject("Button");
        btnGO.transform.SetParent(cardGO.transform, false);
        
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = GetIngredientColor(ingredient);
        
        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        btn.colors = colors;
        
        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = Vector2.zero;
        btnRect.anchorMax = Vector2.one;
        btnRect.sizeDelta = Vector2.zero;
        
        // Icono emoji grande
        string emoji = GetIngredientEmoji(ingredient);
        CreateText(btnGO.transform, emoji, new Vector2(0.5f, 0.65f), new Vector2(80, 40), 32, Color.white, FontStyles.Normal);
        
        // Nombre del ingrediente
        TextMeshProUGUI nameText = CreateTextComponent(btnGO.transform, ingredient, 14, new Color(1f, 1f, 1f, 0.95f), FontStyles.Bold);
        nameText.raycastTarget = false;
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0.4f);
        nameRect.sizeDelta = Vector2.zero;
        
        btn.onClick.AddListener(() => AddIngredient(ingredient, selectedPanel));
    }
    
    private void AddIngredient(string ingredient, GameObject selectedPanel)
    {
        if (!selectedIngredients.Contains(ingredient))
        {
            selectedIngredients.Add(ingredient);
            UpdateSelectedIngredientsDisplay(selectedPanel);
        }
    }
    
    private void UpdateSelectedIngredientsDisplay(GameObject selectedPanel)
    {
        foreach (Transform child in selectedPanel.transform)
            Destroy(child.gameObject);
        
        if (selectedIngredients.Count == 0)
        {
            TextMeshProUGUI placeholderText = CreateTextComponent(selectedPanel.transform, 
                "🍽️ Selecciona ingredientes para tu plato...", 18, 
                new Color(0.5f, 0.5f, 0.5f, 0.7f), FontStyles.Italic);
            placeholderText.alignment = TextAlignmentOptions.Center;
            return;
        }
        
        // Crear lista con tarjetas pequeñas
        float yPos = 20f;
        foreach (string ingredient in selectedIngredients)
        {
            GameObject itemGO = new GameObject($"Item_{ingredient}");
            itemGO.transform.SetParent(selectedPanel.transform, false);
            
            RectTransform itemRect = itemGO.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0, 1);
            itemRect.anchorMax = new Vector2(1, 1);
            itemRect.pivot = new Vector2(0.5f, 1);
            itemRect.anchoredPosition = new Vector2(0, -yPos);
            itemRect.sizeDelta = new Vector2(-40, 50);
            
            // Fondo de la tarjeta
            Image bg = itemGO.AddComponent<Image>();
            bg.color = new Color(1f, 1f, 1f, 0.8f);
            
            // Emoji
            string emoji = GetIngredientEmoji(ingredient);
            CreateText(itemGO.transform, emoji, new Vector2(0.15f, 0.5f), new Vector2(40, 40), 24, Color.black, FontStyles.Normal);
            
            // Nombre
            TextMeshProUGUI nameText = CreateText(itemGO.transform, ingredient, new Vector2(0.6f, 0.5f), 
                new Vector2(200, 40), 16, new Color(0.2f, 0.2f, 0.2f, 1f), FontStyles.Bold);
            nameText.alignment = TextAlignmentOptions.Left;
            
            yPos += 60f;
        }
    }
    
    private void EvaluateRecipe()
    {
        if (selectedIngredients.Count == 0)
        {
            Debug.Log("Selecciona al menos un ingrediente!");
            return;
        }
        
        ShowResults();
    }
    
    // Generador de Sprite "Procedural" o Carga de Resources
    private Sprite GeneratePixelSprite(Color baseColor, string ingredientName = "")
    {
        // 1. Intentar cargar desde Resources si tiene nombre
        if (!string.IsNullOrEmpty(ingredientName))
        {
            Sprite loaded = Resources.Load<Sprite>($"Images/Ingredients/{ingredientName}");
            if (loaded != null) return loaded;
        }

        // 2. Si no, generar proceduralmente
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point; // Importante para look pixel
        
        Color[] colors = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Borde negro grueso
                bool isBorder = x < 4 || x >= size-4 || y < 4 || y >= size-4;
                // Brillo arriba izquierda
                bool isShine = !isBorder && x > 8 && x < 20 && y > size-20 && y < size-8;
                
                if (isBorder)
                    colors[y * size + x] = new Color(0.1f, 0.1f, 0.1f, 1f);
                else if (isShine)
                    colors[y * size + x] = Color.Lerp(baseColor, Color.white, 0.5f);
                else
                    colors[y * size + x] = baseColor;
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private void ShowResults()
    {
        currentPhase = GamePhase.Results;
        HideAllPanels();
        resultsPanel.SetActive(true);
        
        foreach (Transform child in resultsPanel.transform)
            Destroy(child.gameObject);
        
        Scenario scenario = scenarios[currentScenarioIndex];
        int score = CalculateScore(scenario);
        
        // Fondo con gradiente oscuro estrellado
        CreateFullScreenPanel(resultsPanel.transform, new Color(0.1f, 0.05f, 0.15f, 1f));
        
        // Estrellas decorativas
        for (int i = 0; i < 20; i++)
        {
            CreateStar(resultsPanel.transform, 
                new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f)),
                Random.Range(3f, 8f));
        }
        
        // Panel Central Resultados
        GameObject mainBox = CreateDialogBox(resultsPanel.transform, "");
        RectTransform rect = mainBox.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.1f);
        rect.anchorMax = new Vector2(0.9f, 0.9f);
        
        // Título Estilo RPG
        CreateText(mainBox.transform, "RESULTADO DE LA COMPRA", new Vector2(0.5f, 0.9f), new Vector2(600, 60), 40, Color.yellow, FontStyles.Bold);
        
        // Score
        CreateText(mainBox.transform, $"{score}/100", new Vector2(0.5f, 0.75f), new Vector2(400, 80), 80, GetRatingColor(score), FontStyles.Bold);
        CreateText(mainBox.transform, GetRating(score), new Vector2(0.5f, 0.65f), new Vector2(500, 50), 30, Color.white, FontStyles.Bold);
        
        // Feedback en caja de texto interior
        GameObject feedbackBox = CreatePanel(mainBox.transform, "Feedback");
        Image fbImg = feedbackBox.AddComponent<Image>();
        fbImg.color = new Color(0, 0, 0, 0.5f);
        RectTransform fbRect = feedbackBox.GetComponent<RectTransform>();
        fbRect.anchorMin = new Vector2(0.1f, 0.3f);
        fbRect.anchorMax = new Vector2(0.9f, 0.55f);
        
        // Texto Feedback con tamaño explícito relative al padre (0,0 size delta significa que llena el padre si anchors están estirados, pero aquí es dentro de un rect transform fijo)
        // Mejor usamos un tamaño fijo seguro
        var fbText = CreateText(feedbackBox.transform, GetFeedback(scenario, score), new Vector2(0.5f, 0.5f), new Vector2(0, 0), 24, Color.white, FontStyles.Italic);
        RectTransform fbTextRect = fbText.GetComponent<RectTransform>();
        fbTextRect.anchorMin = Vector2.zero;
        fbTextRect.anchorMax = Vector2.one;
        fbTextRect.sizeDelta = new Vector2(-40, -40); // Margen
            
        // Botones Retro
        CreateRetroButton(mainBox.transform, "Nueva Compra", new Color(0.2f, 0.6f, 0.8f), new Vector2(0.3f, 0.15f), new Vector2(250, 60), () => StartScenario(currentScenarioIndex));
        CreateRetroButton(mainBox.transform, "Salir a la Calle", new Color(0.8f, 0.4f, 0.2f), new Vector2(0.7f, 0.15f), new Vector2(250, 60), ShowMenu);
    }
    
    private int CalculateScore(Scenario scenario)
    {
        int score = 0;
        
        // Puntos por ingredientes requeridos (40 puntos)
        int requiredCount = selectedIngredients.Count(i => scenario.requiredIngredients.Contains(i));
        score += (requiredCount * 40) / scenario.requiredIngredients.Length;
        
        // Puntos por ingredientes buenos (40 puntos)
        int goodCount = selectedIngredients.Count(i => scenario.goodIngredients.Contains(i));
        score += (goodCount * 40) / scenario.goodIngredients.Length;
        
        // Penalización por ingredientes malos (-20 puntos cada uno)
        int badCount = selectedIngredients.Count(i => scenario.badIngredients.Contains(i));
        score -= badCount * 20;
        
        // Bonus por variedad (20 puntos si tiene al menos 5 ingredientes)
        if (selectedIngredients.Count >= 5)
            score += 20;
        
        return Mathf.Clamp(score, 0, 100);
    }
    
    private string GetRating(int score)
    {
        if (score >= 90) return "¡EXCELENTE! 🌟";
        if (score >= 75) return "¡MUY BIEN! ⭐";
        if (score >= 60) return "BIEN ✓";
        if (score >= 40) return "PUEDE MEJORAR";
        return "NECESITA MEJORAR";
    }
    
    private Color GetRatingColor(int score)
    {
        if (score >= 75) return new Color(0.2f, 0.7f, 0.3f, 1f); // Verde
        if (score >= 50) return new Color(0.8f, 0.7f, 0.2f, 1f); // Amarillo
        return new Color(0.8f, 0.3f, 0.2f, 1f); // Rojo
    }
    
    private string GetFeedback(Scenario scenario, int score)
    {
        if (score >= 75)
            return "¡Fantástico! Has creado un plato muy adecuado para esta situación. " +
                   "Los ingredientes seleccionados cubren perfectamente las necesidades nutricionales.";
        
        if (score >= 50)
            return "Buen trabajo, pero hay margen de mejora. Revisa las recetas de referencia " +
                   "para identificar ingredientes clave que podrías haber incluido.";
        
        return "Tu plato necesita ajustes. Te recomendamos revisar la fase de aprendizaje " +
               "y prestar atención a los ingredientes específicos para esta situación.";
    }
    
    private void HideAllPanels()
    {
        menuPanel.SetActive(false);
        learningPanel.SetActive(false);
        cookingPanel.SetActive(false);
        resultsPanel.SetActive(false);
    }
    
    // Métodos auxiliares para crear UI
    private TextMeshProUGUI CreateText(Transform parent, string text, Vector2 anchorPos, Vector2 size, 
        float fontSize, Color color, FontStyles style)
    {
        GameObject textGO = new GameObject($"Text_{text.Substring(0, Mathf.Min(10, text.Length))}");
        textGO.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.raycastTarget = false;
        
        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        
        return tmp;
    }
    
    private TextMeshProUGUI CreateTextWithShadow(Transform parent, string text, Vector2 anchorPos, Vector2 size, 
        float fontSize, Color color, FontStyles style, Color shadowColor)
    {
        // Crear sombra primero
        GameObject shadowGO = new GameObject($"Shadow_{text.Substring(0, Mathf.Min(10, text.Length))}");
        shadowGO.transform.SetParent(parent, false);
        
        TextMeshProUGUI shadowTmp = shadowGO.AddComponent<TextMeshProUGUI>();
        shadowTmp.text = text;
        shadowTmp.fontSize = fontSize;
        shadowTmp.alignment = TextAlignmentOptions.Center;
        shadowTmp.color = shadowColor;
        shadowTmp.fontStyle = style;
        shadowTmp.raycastTarget = false;
        
        RectTransform shadowRect = shadowGO.GetComponent<RectTransform>();
        shadowRect.anchorMin = anchorPos;
        shadowRect.anchorMax = anchorPos;
        shadowRect.pivot = new Vector2(0.5f, 0.5f);
        shadowRect.sizeDelta = size;
        shadowRect.anchoredPosition = new Vector2(6, -6);
        
        // Crear texto principal
        TextMeshProUGUI mainText = CreateText(parent, text, anchorPos, size, fontSize, color, style);
        mainText.transform.SetAsLastSibling(); // Asegurar que esté encima
        
        return mainText;
    }
    
    private GameObject CreateFancyTitleBox(Transform parent)
    {
        GameObject boxGO = new GameObject("FancyTitleBox", typeof(RectTransform));
        boxGO.transform.SetParent(parent, false);
        
        // Sombra suave en lugar de bloque sólido
        GameObject shadowGO = new GameObject("Shadow");
        shadowGO.transform.SetParent(boxGO.transform, false);
        Image shadowImg = shadowGO.AddComponent<Image>();
        shadowImg.color = new Color(0f, 0f, 0f, 0.4f);
        RectTransform shadowRect = shadowGO.GetComponent<RectTransform>();
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        shadowRect.sizeDelta = new Vector2(10, -10);
        shadowRect.anchoredPosition = new Vector2(5, -5);
        
        // Borde fino elegante
        GameObject borderGO = new GameObject("Border");
        borderGO.transform.SetParent(boxGO.transform, false);
        Image borderImg = borderGO.AddComponent<Image>();
        borderImg.color = new Color(1f, 0.8f, 0.2f); // Dorado
        RectTransform borderRect = borderGO.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(4, -4);
        
        // Fondo semi-transparente y oscuro para resaltar el texto sin bloquear todo
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(boxGO.transform, false);
        Image boxImg = bgGO.AddComponent<Image>();
        boxImg.color = new Color(0.05f, 0.05f, 0.1f, 0.85f); // Casi negro azulado
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        return boxGO;
    }

    private Sprite GenerateStarryNightBackground()
    {
        int width = 320; // Reducir resolución para look más retro
        int height = 180;
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point; // Pixel Perfect (Nítido)
        
        Color[] pixels = new Color[width * height];
        
        // Gradiente de cielo nocturno limpio
        Color topColor = new Color(0.05f, 0.05f, 0.15f); // Azul muy oscuro
        Color bottomColor = new Color(0.1f, 0.05f, 0.25f); // Morado oscuro
        
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            // Dithering simple para evitar bandas feas
            float dither = (Random.value > 0.5f) ? 0.01f : -0.01f;
            Color rowColor = Color.Lerp(bottomColor, topColor, t);
            
            for (int x = 0; x < width; x++)
            {
                Color c = rowColor;
                c.r += dither; c.g += dither; c.b += dither;
                pixels[y * width + x] = c;
            }
        }
        
        // Estrellas (Píxeles individuales nítidos)
        int starCount = 100;
        for (int i = 0; i < starCount; i++)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
            
            float brightness = Random.Range(0.6f, 1f);
            Color starColor = new Color(1f, 1f, 0.9f, brightness);
            
            pixels[y * width + x] = starColor;
            
            // Algunas estrellas más grandes (cruz)
            if (Random.value > 0.9f)
            {
                Color faintColor = new Color(1f, 1f, 1f, brightness * 0.4f);
                if (x+1 < width) pixels[y * width + (x + 1)] = faintColor;
                if (x-1 > 0) pixels[y * width + (x - 1)] = faintColor;
                if (y+1 < height) pixels[(y + 1) * width + x] = faintColor;
                if (y-1 > 0) pixels[(y - 1) * width + x] = faintColor;
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    private TextMeshProUGUI CreateTextComponent(Transform parent, string text, float fontSize, 
        Color color, FontStyles style)
    {
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.raycastTarget = false;
        
        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        return tmp;
    }
    
    private void CreateButton(Transform parent, string text, Vector2 anchorPos, Vector2 size, 
        Color color, System.Action onClick)
    {
        GameObject btnGO = new GameObject($"Btn_{text}");
        btnGO.transform.SetParent(parent, false);
        
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = color;
        
        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        btn.colors = colors;
        
        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = anchorPos;
        btnRect.anchorMax = anchorPos;
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = size;
        
        TextMeshProUGUI btnText = CreateTextComponent(btnGO.transform, text, 28, Color.white, FontStyles.Bold);
        btnText.raycastTarget = false;
        
        btn.onClick.AddListener(() => onClick());
    }
    
    private void CreateIngredientResultCard(Transform parent, string ingredient, Vector2 anchorPos)
    {
        GameObject cardGO = new GameObject($"ResultCard_{ingredient}");
        cardGO.transform.SetParent(parent, false);
        
        RectTransform cardRect = cardGO.AddComponent<RectTransform>();
        cardRect.anchorMin = anchorPos;
        cardRect.anchorMax = anchorPos;
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(120, 80);
        
        // Fondo de la tarjeta
        Image bg = cardGO.AddComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.9f);
        
        // Emoji grande
        string emoji = GetIngredientEmoji(ingredient);
        CreateText(cardGO.transform, emoji, new Vector2(0.5f, 0.6f), new Vector2(60, 40), 28, Color.black, FontStyles.Normal);
        
        // Nombre
        TextMeshProUGUI nameText = CreateText(cardGO.transform, ingredient, new Vector2(0.5f, 0.25f), 
            new Vector2(110, 30), 12, new Color(0.3f, 0.3f, 0.3f, 1f), FontStyles.Normal);
        nameText.alignment = TextAlignmentOptions.Center;
    }
    
    // Métodos para mejorar la presentación visual
    private string GetIngredientEmoji(string ingredient)
    {
        switch (ingredient.ToLower())
        {
            case "avena": return "🌾";
            case "quinoa": return "🌿";
            case "arroz": return "🍚";
            case "pasta": return "🍝";
            case "pollo": return "🍗";
            case "salmón": return "🐟";
            case "merluza": return "🐠";
            case "rodaballo": return "🐡";
            case "lentejas": return "🫘";
            case "garbanzos": return "🫘";
            case "calabaza": return "🎃";
            case "zanahoria": return "🥕";
            case "tomate": return "🍅";
            case "calabacín": return "🥒";
            case "verduras": return "🥬";
            case "fresas": return "🍓";
            case "arándanos": return "🫐";
            case "manzana": return "🍎";
            case "pera": return "🍐";
            case "fruta": return "🍊";
            case "almendras": return "🌰";
            case "nueces": return "🥜";
            case "queso": return "🧀";
            case "queso fresco": return "🧀";
            case "yogurt": return "🥛";
            case "boniato": return "🍠";
            case "patata": return "🥔";
            default: return "🍽️";
        }
    }
    
    private Color GetIngredientColor(string ingredient)
    {
        // Cereales - Amarillos/Marrones
        if (ingredient == "avena" || ingredient == "arroz" || ingredient == "arroz integral" || ingredient == "pasta" || ingredient == "pan integral" || ingredient == "pan")
            return new Color(0.85f, 0.7f, 0.4f, 1f);
        
        // Proteínas - Rojos/Naranjas/Rosas
        if (ingredient == "pollo" || ingredient == "salmón" || ingredient == "pescado" || ingredient == "merluza" || ingredient == "ternera")
            return new Color(1f, 0.6f, 0.6f, 1f);
        if (ingredient == "tofu" || ingredient == "huevo")
            return new Color(0.95f, 0.9f, 0.8f, 1f); // Blanco/Crema
        
        // Legumbres - Marrones
        if (ingredient == "lentejas" || ingredient == "garbanzos")
            return new Color(0.6f, 0.4f, 0.2f, 1f);
        
        // Verduras - Verdes
        if (ingredient.Contains("verdura") || ingredient == "espinacas" || ingredient == "brocoli" || ingredient == "lechuga" || ingredient == "acelgas")
            return new Color(0.2f, 0.6f, 0.2f, 1f);
        if (ingredient == "zanahoria" || ingredient == "calabaza" || ingredient == "boniato")
            return new Color(1f, 0.5f, 0.0f, 1f); // Naranja
        if (ingredient == "tomate" || ingredient == "pimiento")
            return new Color(0.9f, 0.2f, 0.2f, 1f); // Rojo
        if (ingredient == "patata")
            return new Color(0.9f, 0.8f, 0.5f, 1f); // Amarillo pálido
        
        // Frutas
        if (ingredient == "fresas" || ingredient == "manzana" || ingredient == "cerezas")
            return new Color(0.9f, 0.3f, 0.3f, 1f); // Rojo
        if (ingredient == "platano" || ingredient == "limón")
            return new Color(1f, 0.9f, 0.2f, 1f); // Amarillo fuerte
        if (ingredient == "naranja" || ingredient == "mandarina")
            return new Color(1f, 0.6f, 0.0f, 1f); // Naranja
        if (ingredient == "pera" || ingredient == "uvas")
            return new Color(0.6f, 0.8f, 0.3f, 1f); // Verde claro
        
        // Frutos secos
        if (ingredient == "almendras" || ingredient == "nueces")
            return new Color(0.7f, 0.5f, 0.3f, 1f);
        
        // Lácteos - Blanco azulado
        if (ingredient == "queso" || ingredient == "queso fresco" || ingredient == "yogurt" || ingredient == "leche")
            return new Color(0.9f, 0.95f, 1f, 1f);
            
        // Default (Gris para debug, no invisible)
        return new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    
    // Clase para almacenar datos de escenarios
    private class Scenario
    {
        public string name;
        public string description;
        public string[] learningRecipes;
        public string[] requiredIngredients;
        public string[] goodIngredients;
        public string[] badIngredients;
    }
}
