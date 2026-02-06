using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Juego de nutrición GOOGAZ en 2D real con sprites
/// </summary>
public class NutritionGame2D : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public Camera mainCamera;
    
    [Header("Prefabs y Recursos")]
    public GameObject ingredientPrefab;
    public GameObject particlePrefab;
    
    [Header("UI Canvas (solo para HUD)")]
    public Canvas hudCanvas;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI instructionsText;
    
    // Estado del juego
    private GamePhase currentPhase = GamePhase.Menu;
    private int currentScenarioIndex = 0;
    private List<string> selectedIngredients = new List<string>();
    private GameObject gameContainer; 
    private GameObject hudObject;
    
    private GameObject player;
    private GameObject shopkeeper;
    private GameObject currentBackground;

    private enum GamePhase { Intro, Menu, Learning, Shopping, Results }

    // --- SISTEMA DE AUTO-INICIO (ONE CLICK) ---
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindObjectOfType<NutritionGame2D>() == null)
        {
            GameObject go = new GameObject("★ GOOGAZ_MANAGER");
            go.AddComponent<NutritionGame2D>();
        }
    }

    private void Start()
    {
        #if UNITY_EDITOR
        // Forzar cambio a la pestaña Game para que veas el resultado 2D
        EditorApplication.ExecuteMenuItem("Window/General/Game");
        #endif
        InitializeGame();
    }

    private void InitializeGame()
    {
        CleanupOldSystem();
        EnsureEventSystem();
        SetupCamera();
        SetupHUD();
        ShowIntro();
    }

    private void CleanupOldSystem()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            string typeName = script.GetType().Name;
            if (typeName == "NutritionGame" || typeName == "SimpleGame" || typeName == "BasicUISetup" || typeName == "SimpleUICreator")
            {
                if (script != this) script.enabled = false;
            }
        }

        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            if (canvas.name.Contains("GameCanvas") || canvas.name.Contains("Canvas"))
            {
                if (canvas.gameObject.name != "HUD_Canvas") canvas.gameObject.SetActive(false);
            }
        }
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private void SetupCamera()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCamera = camObj.AddComponent<Camera>();
            mainCamera.tag = "MainCamera";
        }
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5f;
        mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        mainCamera.transform.position = new Vector3(0, 0, -10);
    }

    private void SetupHUD()
    {
        if (hudCanvas != null) return;

        GameObject canvasObj = new GameObject("HUD_Canvas");
        hudCanvas = canvasObj.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hudCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        hudObject = new GameObject("HUD_Dynamic_Content");
        hudObject.transform.SetParent(hudCanvas.transform, false);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(hudCanvas.transform, false);
        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 70;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = new Color(1f, 1f, 0.9f);
        titleText.enableWordWrapping = false;
        RectTransform titleRt = titleText.GetComponent<RectTransform>();
        titleRt.anchoredPosition = new Vector2(0, 350);
        titleRt.sizeDelta = new Vector2(1200, 200);

        GameObject instrObj = new GameObject("Instructions");
        instrObj.transform.SetParent(hudCanvas.transform, false);
        instructionsText = instrObj.AddComponent<TextMeshProUGUI>();
        instructionsText.fontSize = 32;
        instructionsText.alignment = TextAlignmentOptions.Center;
        instructionsText.color = Color.white;
        instructionsText.enableWordWrapping = false;
        RectTransform instrRt = instructionsText.GetComponent<RectTransform>();
        instrRt.anchoredPosition = new Vector2(0, -350);
        instrRt.sizeDelta = new Vector2(1200, 100);

        GameObject scoreObj = new GameObject("ScoreHud");
        scoreObj.transform.SetParent(hudCanvas.transform, false);
        scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
        scoreText.fontSize = 40;
        scoreText.alignment = TextAlignmentOptions.TopRight;
        scoreText.color = Color.yellow;
        RectTransform srt = scoreText.GetComponent<RectTransform>();
        srt.anchorMin = srt.anchorMax = new Vector2(1, 1);
        srt.anchoredPosition = new Vector2(-100, -100);
        srt.sizeDelta = new Vector2(600, 100);
    }

    private Scenario[] scenarios = new Scenario[]
    {
        new Scenario {
            name = "Embarazo y Deporte",
            description = "Necesidades nutricionales antes y después de realizar actividad física",
            requiredIngredients = new string[] { "quinoa", "pollo", "calabaza", "salmón", "boniato", "verduras" },
            goodIngredients = new string[] { "avena", "nueces", "fruta", "queso", "merluza", "zanahoria" },
            badIngredients = new string[] { "frituras", "azúcar", "alcohol", "procesados" }
        },
        new Scenario {
            name = "Adolescencia y Estrés",
            description = "Combatir el estrés a través de la alimentación",
            requiredIngredients = new string[] { "avena", "fresas", "arándanos", "almendras", "lentejas", "merluza" },
            goodIngredients = new string[] { "nueces", "queso fresco", "calabaza", "zanahoria", "verduras", "rodaballo" },
            badIngredients = new string[] { "café", "energéticas", "azúcar", "frituras" }
        },
        new Scenario {
            name = "Senectud - Gestión de Migraña",
            description = "Alimentación para la tercera edad con gestión de migraña",
            requiredIngredients = new string[] { "avena", "pera", "yogurt", "verduras", "pollo", "calabaza" },
            goodIngredients = new string[] { "fruta", "tomate", "calabacín", "boniato", "sopa" },
            badIngredients = new string[] { "queso curado", "chocolate", "vino", "embutidos", "cítricos" }
        }
    };

    private string[] availableIngredients = new string[]
    {
        "avena", "quinoa", "arroz", "pasta",
        "pollo", "salmón", "merluza", "rodaballo",
        "lentejas", "garbanzos",
        "calabaza", "zanahoria", "tomate", "calabacín", "verduras",
        "fresas", "arándanos", "manzana", "pera", "plátano", "fruta",
        "almendras", "nueces",
        "queso", "queso fresco", "yogurt",
        "boniato", "patata"
    };

    private void Update()
    {
        // Forzar actualización de posiciones de texto sobre botones 2D
        if (gameContainer != null)
        {
            foreach (Transform t in gameContainer.transform)
            {
                MenuButton mb = t.GetComponent<MenuButton>();
                if (mb != null && mb.buttonText != null)
                {
                    Vector3 screenPos = mainCamera.WorldToScreenPoint(t.position);
                    mb.buttonText.transform.position = screenPos;
                }
            }
        }

        if (currentPhase == GamePhase.Intro && Input.GetKeyDown(KeyCode.Return))
            ShowMenu();
        else if (currentPhase == GamePhase.Learning && Input.GetKeyDown(KeyCode.Return))
            ShowShopping();
        else if (currentPhase == GamePhase.Shopping && Input.GetKeyDown(KeyCode.Return))
            ShowResults();
        else if (currentPhase == GamePhase.Results)
        {
            if (Input.GetKeyDown(KeyCode.R)) StartScenario(currentScenarioIndex);
            if (Input.GetKeyDown(KeyCode.M)) ShowMenu();
        }
    }

    private void StartScenario(int index)
    {
        currentScenarioIndex = index;
        selectedIngredients.Clear();
        ShowLearning();
    }

    private void ShowIntro()
    {
        ClearScene();
        currentPhase = GamePhase.Intro;
        
        GameObject bgLayer = new GameObject("Intro_BG");
        bgLayer.transform.SetParent(gameContainer.transform);
        bgLayer.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bgLayer.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/intro_bg.png");
        if (s != null) sr.sprite = s;
        else CreateStarryBackground(); 

        ScaleToFillScreen(bgLayer, 1.0f);

        titleText.text = ""; // Logo already in background
        instructionsText.text = "PRESIONA <size=50><B><color=#FFD700>ENTER</color></B></size> PARA EMPEZAR";
        instructionsText.color = Color.white;
        instructionsText.fontSize = 35;
        
        // Efecto de sombra para el texto
        instructionsText.outlineWidth = 0.2f;
        instructionsText.outlineColor = Color.black;

        // Fondo oscuro para el texto de instrucciones (SOLICITADO POR USUARIO)
        GameObject instrBg = new GameObject("Intro_InstrBG");
        instrBg.transform.SetParent(hudCanvas.transform, false);
        // Asegurar que esté detrás del texto
        instrBg.transform.SetSiblingIndex(instructionsText.transform.GetSiblingIndex()); 
        
        Image img = instrBg.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.85f);
        RectTransform rt = instrBg.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1000, 120);
        rt.anchoredPosition = new Vector2(0, -350);
    }

    private void ShowMenu()
    {
        ClearScene();
        currentPhase = GamePhase.Menu;
        // Calibración precisa de escala y altura (Senior Polish)
        // Posiciones un poco más altas para dejar aire al suelo
        CreateCharacterSelectionButton(1, "Characters/adolescencia.png", scenarios[1].name.ToUpper(), new Vector3(-6, 0, 0), 7.0f, new Color(0.4f, 1f, 0.4f)); 
        CreateCharacterSelectionButton(0, "Characters/embarazo.png", scenarios[0].name.ToUpper(), new Vector3(0, 0, 0), 7.2f, new Color(0.4f, 0.8f, 1f)); 
        CreateCharacterSelectionButton(2, "Characters/senectud.png", scenarios[2].name.ToUpper(), new Vector3(6, 0, 0), 7.2f, new Color(1f, 0.85f, 0.4f));
    }

    private void CreateSelectionBackground()
    {
        // Fondo: Gradiente de estudio más contrastado
        GameObject bg = new GameObject("SelectionBG");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        int w = 2, h = 64;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Bilinear;
        Color topC = new Color(0.05f, 0.05f, 0.1f);   // Casi negro azulado
        Color botC = new Color(0.12f, 0.08f, 0.15f);  // Púrpura muy oscuro
        for (int y = 0; y < h; y++) {
            Color t = Color.Lerp(botC, topC, (float)y / h);
            tex.SetPixel(0, y, t); tex.SetPixel(1, y, t);
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 10f);
        ScaleToFillScreen(bg, 1.0f, true);
        sr.sortingOrder = -10;

        // Suelo estilizado (ocupa el tercio inferior)
        GameObject floor = new GameObject("SelectionFloor");
        floor.transform.SetParent(gameContainer.transform);
        // Posicionado para que el borde superior esté abajo
        floor.transform.position = new Vector3(0, -7, 9); 
        SpriteRenderer srFloor = floor.AddComponent<SpriteRenderer>();
        srFloor.sprite = CreateBoxSprite(20, 20, new Color(0.03f, 0.03f, 0.05f, 1f), false);
        // Escalado manual para cubrir el ancho y parte del alto
        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        floor.transform.localScale = new Vector3(screenWidth * 2, 8, 1);
        srFloor.sortingOrder = -9;
    }

    private Sprite CreateSelectionLightSprite()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size);
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(size/2, size/2));
                float alpha = Mathf.Clamp01(1.0f - (dist / (size/2)));
                tex.SetPixel(x, y, new Color(1, 1, 1, alpha * alpha));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private void CreateCharacterSelectionButton(int index, string spritePath, string label, Vector3 pos, float targetH, Color glowColor)
    {
        GameObject charObj = new GameObject($"Char_{index}");
        charObj.transform.SetParent(gameContainer.transform);
        charObj.transform.position = pos;
        
        SpriteRenderer sr = charObj.AddComponent<SpriteRenderer>();
        Sprite s = LoadLocalSprite(spritePath);
        if (s != null) {
            s.texture.filterMode = FilterMode.Point; // ¡CRUCIAL para que el píxel art no se vea borroso!
            sr.sprite = s;
            float h = s.bounds.size.y;
            float scale = targetH / h;
            charObj.transform.localScale = new Vector3(scale, scale, 1);
        }
        sr.sortingOrder = 5;

        // Base circular (Plataforma de luz)
        GameObject pedestal = new GameObject("Pedestal");
        pedestal.transform.SetParent(charObj.transform, false);
        // Calculamos la posición base del personaje
        float spriteBottomOffset = (sr.sprite != null) ? sr.sprite.bounds.min.y * charObj.transform.localScale.y : -targetH/2;
        pedestal.transform.position = charObj.transform.position + new Vector3(0, spriteBottomOffset, 0.5f);
        pedestal.transform.localScale = new Vector3(4.0f, 1.2f, 1);
        SpriteRenderer psr = pedestal.AddComponent<SpriteRenderer>();
        psr.sprite = CreateSelectionLightSprite(); // El mismo gradiente circular
        psr.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.2f);
        psr.sortingOrder = 4;

        // Borde de color (Glow mejorado)
        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(charObj.transform, false);
        glow.transform.localPosition = Vector3.zero;
        SpriteRenderer gsr = glow.AddComponent<SpriteRenderer>();
        gsr.sprite = sr.sprite;
        gsr.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.6f);
        gsr.sortingOrder = 6; // Por delante para efecto aura
        glow.transform.localScale = Vector3.one * 1.05f;
        glow.SetActive(false);

        // Click logic
        charObj.AddComponent<BoxCollider2D>().size = sr.sprite != null ? sr.sprite.bounds.size : Vector2.one;
        MenuButton mb = charObj.AddComponent<MenuButton>();
        mb.onClick = () => StartScenario(index);
        mb.hoverGlow = glow;

        // UI Label: Eliminado fondo feo, añadido borde al texto (Outline)
        GameObject labelBg = new GameObject($"LabelBg_{index}");
        labelBg.transform.SetParent(hudObject.transform, false);
        // Sin imagen de fondo (transparente)
        
        GameObject textObj = new GameObject($"LabelText_{index}");
        textObj.transform.SetParent(labelBg.transform, false);
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 22;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = new Color(1f, 0.9f, 0.7f);
        txt.fontStyle = FontStyles.Bold;
        txt.enableWordWrapping = true;
        
        // BORDE DEL TEXTO (OUTLINE) PARA QUE SE LEA SIN CAJA
        txt.outlineWidth = 0.3f;
        txt.outlineColor = new Color(0,0,0,1f); // Borde negro fuerte
        
        // Sombra suave para separar del fondo
        txt.fontSharedMaterial.EnableKeyword("UNDERLAY_ON");
        txt.fontSharedMaterial.SetFloat("_UnderlayOffsetX", 1f);
        txt.fontSharedMaterial.SetFloat("_UnderlayOffsetY", -1f);
        txt.fontSharedMaterial.SetFloat("_UnderlayDilate", 1f);
        txt.fontSharedMaterial.SetFloat("_UnderlaySoftness", 0.5f);
        
        RectTransform rtb = labelBg.AddComponent<RectTransform>();
        rtb.sizeDelta = new Vector2(300, 80);
        
        RectTransform rtText = txt.GetComponent<RectTransform>();
        rtText.sizeDelta = new Vector2(280, 70);
        
        // Posicionamiento HUD vinculado al mundo
        Vector3 screenPos = mainCamera.WorldToScreenPoint(pos + Vector3.down * 4.2f);
        rtb.position = screenPos;
    }

    private void ShowLearning()
    {
        ClearScene();
        currentPhase = GamePhase.Learning;
        CreateChalkboardBackground();
        
        Scenario s = scenarios[currentScenarioIndex];
        
        // ELIMINADO EL TÍTULO DEL HUD PARA EVITAR SOLAPAMIENTO Y LIMPIAR LA VISTA
        titleText.text = ""; 
        
        CreateLearningNote(s);

        instructionsText.text = "PRESIONA <color=#FFD700><b>ENTER</b></color> PARA ACEPTAR LA MISIÓN";
        instructionsText.color = Color.white;
        instructionsText.fontSize = 32; // Un poco más grande
        
        // Fondo oscuro "Cuadrado" para que se vea bien
        GameObject instrBg = new GameObject("InstrBG");
        instrBg.transform.SetParent(hudCanvas.transform, false);
        
        Image img = instrBg.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.95f); // Casi opaco
        RectTransform rt = instrBg.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(900, 80);
        rt.anchoredPosition = new Vector2(0, -480);
        
        // Aseguramos que la instrucción de ENTER quede por encima del overlay
        instructionsText.transform.SetAsLastSibling();
        
        // Movemos las instrucciones más abajo para que no tapen el papel
        RectTransform rtInstr = instructionsText.GetComponent<RectTransform>();
        rtInstr.anchoredPosition = new Vector2(0, -480);
    }

    private void ShowShopping()
    {
        ClearScene();
        currentPhase = GamePhase.Shopping;
        CreateShopBackground();
        
        titleText.text = "<color=#FFEAB0>EL MERCADO GOOGAZ</color>";
        titleText.fontSize = 65;
        
        instructionsText.text = "Selecciona los ingredientes más saludables para completar tu misión.";
        instructionsText.color = new Color(0.9f, 0.9f, 0.9f);
        instructionsText.fontSize = 28;
        instructionsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -440);

        scoreText.text = "CESTA: 0";
        scoreText.color = Color.white;
        scoreText.fontSize = 45;
        scoreText.alignment = TextAlignmentOptions.Center;
        RectTransform srt = scoreText.GetComponent<RectTransform>();
        srt.anchorMin = srt.anchorMax = new Vector2(0.5f, 1f);
        srt.anchoredPosition = new Vector2(0, -140);
        scoreText.gameObject.SetActive(true);

        CreateShopkeeper();
        CreateShopShelves();
    }

    private void ShowResults()
    {
        ClearScene();
        currentPhase = GamePhase.Results;
        CreateStarryBackground();
        
        Scenario s = scenarios[currentScenarioIndex];
        int score = CalculateScore(s);
        
        // Título centralizado
        titleText.text = "RESUMEN DE MISIÓN";
        titleText.fontSize = 70;
        titleText.color = Color.white;
        
        // Fondo visual para los resultados (Pantalla casi completa)
        GameObject panel = new GameObject("ResultsPanel");
        panel.transform.SetParent(gameContainer.transform);
        panel.transform.position = new Vector3(0, 0, 5);
        SpriteRenderer sr = panel.AddComponent<SpriteRenderer>();
        sr.sprite = CreateBoxSprite(1200, 900, new Color(0, 0, 0, 0.85f), true);
        sr.sortingOrder = 1;
        ScaleToFillScreen(panel, 0.9f); // Llenar el 90% de la pantalla

        instructionsText.text = $"<size=50>{GetFeedback(s, score)}</size>\n\n<color=#AAAAAA><size=28>Presiona <b>R</b> para reintentar o <b>M</b> para menú</size></color>";
        instructionsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -350);

        scoreText.text = $"Puntuación: <size=80>{score}</size>";
        scoreText.color = GetRatingColor(score);
        scoreText.alignment = TextAlignmentOptions.Center;
        RectTransform srt = scoreText.GetComponent<RectTransform>();
        srt.anchorMin = srt.anchorMax = new Vector2(0.5f, 0.5f);
        srt.anchoredPosition = new Vector2(0, 250);
        scoreText.gameObject.SetActive(true);

        DisplaySelectedIngredients();
    }

    private void CreateButton(string label, Vector3 pos, Color color, System.Action action)
    {
        GameObject btnObj = new GameObject($"Btn_{label}");
        btnObj.transform.SetParent(gameContainer.transform);
        btnObj.transform.position = pos;
        
        SpriteRenderer sr = btnObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateBoxSprite(600, 140, color, true);
        sr.sortingOrder = 5;
        
        btnObj.AddComponent<BoxCollider2D>().size = new Vector2(6, 1.4f);
        MenuButton mb = btnObj.AddComponent<MenuButton>();
        mb.onClick = action;
        mb.originalColor = color;
        
        // Crear texto en HUD
        GameObject textObj = new GameObject($"Text_{label}");
        textObj.transform.SetParent(hudObject.transform, false);
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 42;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        txt.fontStyle = FontStyles.Bold;
        txt.enableWordWrapping = false;
        txt.raycastTarget = false;
        
        RectTransform rt = txt.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 100);
        
        mb.buttonText = textObj;
    }

    private void CreateLearningNote(Scenario scenario)
    {
        // 1. Fondo oscuro de enfoque (Overlay) - AHORA ESTRUCTURA UI
        GameObject overlay = new GameObject("UI_Overlay");
        overlay.transform.SetParent(hudCanvas.transform, false);
        // Ponerlo al principio para que esté detrás de todo en el HUD
        overlay.transform.SetSiblingIndex(0); 
        
        Image imgOverlay = overlay.AddComponent<Image>();
        imgOverlay.color = new Color(0,0,0,0.85f);
        RectTransform rtOverlay = overlay.GetComponent<RectTransform>();
        rtOverlay.anchorMin = Vector2.zero;
        rtOverlay.anchorMax = Vector2.one; // Estirar a toda la pantalla
        rtOverlay.sizeDelta = Vector2.zero;

        // 2. La Hoja de Papel (UI Image) - DESPLAZADA A LA DERECHA
        GameObject noteObj = new GameObject("UI_MissionPaper");
        noteObj.transform.SetParent(hudCanvas.transform, false);
        noteObj.transform.SetSiblingIndex(1);
        
        Image imgPaper = noteObj.AddComponent<Image>();
        imgPaper.sprite = CreateBoxSprite(800, 900, new Color(0.96f, 0.96f, 0.94f), true);
        
        RectTransform rtPaper = noteObj.GetComponent<RectTransform>();
        rtPaper.anchoredPosition = new Vector2(350, 20); // Desplazado a la derecha
        rtPaper.sizeDelta = new Vector2(700, 850); // Un poco más estrecho

        // 2.5 EL COCINERO OAK (UI Image) - A LA IZQUIERDA
        GameObject oakObj = new GameObject("UI_Boss");
        oakObj.transform.SetParent(hudCanvas.transform, false);
        oakObj.transform.SetSiblingIndex(2); // Por encima del papel si se solapan
        
        Image imgOak = oakObj.AddComponent<Image>();
        imgOak.sprite = CreateCocineroOakSprite();
        imgOak.preserveAspect = true;
        
        RectTransform rtOak = oakObj.GetComponent<RectTransform>();
        rtOak.anchorMin = new Vector2(0.5f, 0.5f);
        rtOak.anchorMax = new Vector2(0.5f, 0.5f);
        rtOak.anchoredPosition = new Vector2(-450, -50); // A la izquierda
        rtOak.sizeDelta = new Vector2(500, 800);

        // 3. El Clip metálico superior (UI Image)
        GameObject clip = new GameObject("UI_MetalClip");
        clip.transform.SetParent(noteObj.transform, false);
        Image imgClip = clip.AddComponent<Image>();
        imgClip.sprite = CreateBoxSprite(300, 60, new Color(0.3f, 0.35f, 0.4f), true);
        
        RectTransform rtClip = clip.GetComponent<RectTransform>();
        rtClip.anchoredPosition = new Vector2(0, 430); // Ajustado al nuevo alto
        rtClip.sizeDelta = new Vector2(300, 60);

        // 4. Contenido del Texto (Ajustado tamaños para que quepa perfecto)
        GameObject noteTextObj = new GameObject("MissionContent");
        noteTextObj.transform.SetParent(noteObj.transform, false);
        TextMeshProUGUI txt = noteTextObj.AddComponent<TextMeshProUGUI>();
        txt.alignment = TextAlignmentOptions.TopLeft;
        txt.enableWordWrapping = true;
        
        // Construimos el string con estilo limpio y jerarquía
        string content = "";
        
        // Cabecera Institucional
        content += $"<align=center><size=32><B><color=#2C3E50>CONFIDENCIAL</color></B></size></align>\n";
        content += $"<align=center><size=14><color=#7F8C8D>• MINISTERIO DE NUTRICIÓN GOOGAZ •</color></size></align>\n";
        content += $"<align=center><size=24><color=#BDC3C7>_______________________________________</color></size></align>\n\n";

        // Perfil del Paciente
        content += $"<size=18><color=#95A5A6>SUJETO DE ESTUDIO:</color></size>\n";
        content += $"<size=28><B><color=#E67E22>{scenario.name.ToUpper()}</color></B></size>\n\n";

        // Objetivo
        content += $"<size=18><color=#95A5A6>OBJETIVO CLÍNICO:</color></size>\n";
        content += $"<size=22><color=#34495E>{scenario.description}</color></size>\n\n";
        content += $"<align=center><size=24><color=#BDC3C7>_______________________________________</color></size></align>\n\n";

        // Lista de Compra (Diseño Checklist Visual)
        content += $"<size=22><B><color=#27AE60>PROTOCOLOS NUTRICIONALES:</color></B></size>\n";
        content += $"<color=#2C3E50><line-height=130%>";
        foreach(var item in scenario.requiredIngredients)
        {
            content += $"  <b>[ ]</b> <size=26>{item.ToUpper()}</size>\n"; 
        }
        content += $"</color>";

        // Nota: He quitado el sello de texto para ponerlo como objeto UI abajo

        txt.text = content;
        
        // Ajustamos la configuración física del texto para el nuevo ancho
        RectTransform rtTxt = txt.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero; rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = new Vector2(-80, -100); // Margen interno más amplio
        rtTxt.anchoredPosition = new Vector2(0, -20);
        
        // 5. SELLO VISUAL "PRIORIDAD ALTA" (Elemento UI Rotado)
        GameObject stampObj = new GameObject("UI_Stamp");
        stampObj.transform.SetParent(noteObj.transform, false);
        
        // Caja Roja del Sello
        Image stampImg = stampObj.AddComponent<Image>();
        stampImg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f); // Rojo sólido
        RectTransform rtStamp = stampObj.GetComponent<RectTransform>();
        rtStamp.sizeDelta = new Vector2(280, 70);
        rtStamp.anchoredPosition = new Vector2(180, -350); // Esquina inferior derecha
        rtStamp.localRotation = Quaternion.Euler(0, 0, 15); // Rotado
        
        // Texto del Sello
        GameObject stampTxtObj = new GameObject("StampText");
        stampTxtObj.transform.SetParent(stampObj.transform, false);
        TextMeshProUGUI stampTxt = stampTxtObj.AddComponent<TextMeshProUGUI>();
        stampTxt.text = "PRIORIDAD ALTA";
        stampTxt.fontSize = 28;
        stampTxt.fontStyle = FontStyles.Bold;
        stampTxt.alignment = TextAlignmentOptions.Center;
        stampTxt.color = Color.white;
        stampTxtObj.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 70);
        
        // Aseguramos que la instrucción de ENTER quede por encima del overlay
        instructionsText.transform.SetAsLastSibling();
        
        // Movemos las instrucciones más abajo para que no tapen el papel
        RectTransform rtInstr = instructionsText.GetComponent<RectTransform>();
        rtInstr.anchoredPosition = new Vector2(0, -480);
        
        // Y su fondo también
        Transform instrBg = hudCanvas.transform.Find("InstrBG");
        if(instrBg != null) {
            instrBg.SetAsLastSibling();
            instrBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -480);
        }
    }

    private void CreateShopkeeper()
    {
        // El mostrador
        GameObject counter = new GameObject("Counter");
        counter.transform.SetParent(gameContainer.transform);
        counter.transform.position = new Vector3(-6.5f, -2f, 2);
        SpriteRenderer csr = counter.AddComponent<SpriteRenderer>();
        csr.sprite = CreateBoxSprite(400, 300, new Color(0.25f, 0.15f, 0.1f), true);
        csr.sortingOrder = 4;
        counter.transform.localScale = new Vector3(1f, 1f, 1f);

        shopkeeper = new GameObject("Shopkeeper");
        shopkeeper.transform.SetParent(gameContainer.transform);
        shopkeeper.transform.position = new Vector3(-6.5f, -0.5f, 1);
        SpriteRenderer sr = shopkeeper.AddComponent<SpriteRenderer>();
        sr.sprite = CreateShopkeeperSprite();
        sr.sortingOrder = 3;
        sr.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
        shopkeeper.AddComponent<ShopkeeperAnimator>();
    }

    private void CreateShopShelves()
    {
        // En lugar de crear estanterías geométricas, usamos las del fondo para posicionar ingredientes
        float[] shelfHeights = { 1.8f, 0.1f, -1.5f, -3.2f };
        int cols = 6;
        float startX = -3.5f; 
        
        int ingredientIndex = 0;
        foreach (var ingredientName in availableIngredients)
        {
            int row = ingredientIndex / cols;
            int col = ingredientIndex % cols;
            if (row >= shelfHeights.Length) break;

            Vector3 pos = new Vector3(startX + col * 1.8f, shelfHeights[row], 0);
            
            GameObject item = new GameObject($"Item_{ingredientName}");
            item.transform.SetParent(gameContainer.transform);
            item.transform.position = pos;
            
            Color color = GetIngredientColor(ingredientName);
            SpriteRenderer sr = item.AddComponent<SpriteRenderer>();
            
            // Generación o Carga de Sprite
            sr.sprite = CreateIngredientSprite(ingredientName, color);
            sr.sortingOrder = 5;

            // AJUSTE DE ESCALA:
            // Si es un sprite cargado (no el procedural "default"), lo escalamos para que tenga un tamaño uniforme
            if (sr.sprite.name != "DefaultCircle")
            {
                float maxSize = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
                if (maxSize > 0)
                {
                    // Queremos que ocupen aprox 1.4 unidades de mundo
                    float targetSize = 1.4f;
                    float scaleFactor = targetSize / maxSize;
                    item.transform.localScale = Vector3.one * scaleFactor;
                }
            }
            else
            {
                // Escala para el círculo procedural base
                item.transform.localScale = Vector3.one * 1.2f;
            }

            // Etiqueta de precio/nombre
            GameObject label = new GameObject("PriceLabel");
            label.transform.SetParent(item.transform, false);
            // Ajustamos posición de la etiqueta relativa al tamaño visual
            label.transform.localPosition = new Vector3(0, -0.6f, -0.1f); 
            label.AddComponent<SpriteRenderer>().sprite = CreateBoxSprite(120, 40, new Color(1,1,1,0.8f), true);
            // Invertimos la escala del padre para que la etiqueta se vea siempre igual
            float parentScale = item.transform.localScale.x;
            label.transform.localScale = new Vector3(0.012f / parentScale, 0.012f / parentScale, 1);

            item.AddComponent<BoxCollider2D>();
            IngredientItem ii = item.AddComponent<IngredientItem>();
            ii.ingredientName = ingredientName;
            ii.color = color;
            ii.onSelect = (ing) => {
                if (!selectedIngredients.Contains(ing.ingredientName)) {
                    selectedIngredients.Add(ing.ingredientName);
                    scoreText.text = $"CESTA: {selectedIngredients.Count}";
                    CreateParticles(pos, ing.color);
                    // Feedback visual
                    ing.transform.localScale *= 1.2f;
                }
            };

            ingredientIndex++;
        }
    }

    private Sprite CreateIngredientSprite(string name, Color c)
    {
        // 1. INTENTAR CARGAR SPRITE PERSONALIZADO (Robustez mejorada)
        // Normalizamos el nombre: minúsculas y sin tildes
        string simpleName = RemoveAccents(name.ToLower());
        
        // Búsqueda inteligente recursiva en todo Assets si no está en la ruta estándar
        Sprite s = FindSpriteRecursive(simpleName);
        
        if (s != null)
        {
            s.name = simpleName;
            return s;
        }

        // 2. FALLBACK VISUAL:
        // Si no hay imagen, creamos un círculo con el NOMBRE escrito para que se sepa qué es
        return CreateFallbackSprite(name, c);
    }

    private Sprite FindSpriteRecursive(string filenameNoExt)
    {
        // Mapa de correcciones manuales (para errores de nombrado comunes)
        if (filenameNoExt == "boniato") filenameNoExt = "bonito"; // Corrección para archivo mal nombrado
        
        string[] searchPatterns = new string[] 
        { 
            filenameNoExt + "*",          // 1. coincidencia exacta o prefijo (avena -> avena-removebg)
            "*" + filenameNoExt + "*",    // 2. contiene el nombre (fruta -> mixFruta)
        };

        // Búsqueda de archivos
        foreach (var pattern in searchPatterns)
        {
            Sprite s = TryFindSprite(pattern);
            if (s != null) return s;
        }

        // 3. Intento en singular (fresas -> fresa)
        if (filenameNoExt.EndsWith("s"))
        {
            string singular = filenameNoExt.Substring(0, filenameNoExt.Length - 1);
            // Probamos patrones con el singular
            if (TryFindSprite(singular + "*") != null) return TryFindSprite(singular + "*");
            if (TryFindSprite("*" + singular + "*") != null) return TryFindSprite("*" + singular + "*");
        }

        return null;
    }

    private Sprite TryFindSprite(string pattern)
    {
        try 
        {
            // Buscamos primero en la carpeta específica Food para ser más rápidos
            string foodPath = System.IO.Path.Combine(Application.dataPath, "Sprites", "Food");
            if (System.IO.Directory.Exists(foodPath))
            {
                string[] files = System.IO.Directory.GetFiles(foodPath, pattern);
                Sprite s = LoadSpriteFromFiles(files);
                if (s != null) return s;
            }

            // Si falla, búsqueda profunda (Fallback)
            string[] allFiles = System.IO.Directory.GetFiles(Application.dataPath, pattern, System.IO.SearchOption.AllDirectories);
            return LoadSpriteFromFiles(allFiles);
        } 
        catch { return null; }
    }

    private Sprite LoadSpriteFromFiles(string[] files)
    {
        foreach (string f in files)
        {
            if (f.EndsWith(".meta")) continue;
            string ext = System.IO.Path.GetExtension(f).ToLower();
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                byte[] data = System.IO.File.ReadAllBytes(f);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(data))
                {
                    tex.filterMode = FilterMode.Point; // Cambiar a Bilinear si las imágenes son HD
                    tex.Apply();
                    
                    // Importante: Crear sprite con pivote central
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
            }
        }
        return null;
    }

    private Sprite CreateFallbackSprite(string name, Color c)
    {
        int size = 64; // Más grande para que quepa texto
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        
        // Fondo transparente
        for(int i=0; i<size*size; i++) tex.SetPixel(i%size, i/size, Color.clear);
        
        // Círculo base
        float cx = size/2, cy = size/2, r = size/2 - 2;
        for(int y=0; y<size; y++)
        {
            for(int x=0; x<size; x++)
            {
                if(Vector2.Distance(new Vector2(x,y), new Vector2(cx,cy)) <= r)
                {
                    tex.SetPixel(x, y, c);
                }
                else if(Vector2.Distance(new Vector2(x,y), new Vector2(cx,cy)) <= r + 2)
                {
                    tex.SetPixel(x, y, new Color(0,0,0,0.8f));
                }
            }
        }
        
        // "Escribir" la inicial (muy rudimentario, dibujando píxeles)
        // Solo dibujamos la primera letra en blanco en el centro
        Color textColor = Color.white;
        char letter = char.ToUpper(name[0]);
        DrawLetter(tex, letter, (int)cx, (int)cy, textColor);

        tex.Apply();
        Sprite fallback = Sprite.Create(tex, new Rect(0,0,size,size), new Vector2(0.5f,0.5f), 16f);
        fallback.name = "Fallback_" + name;
        return fallback;
    }

    private void DrawLetter(Texture2D tex, char l, int cx, int cy, Color c)
    {
        // Matriz de puntos muy básica 5x7
        int[,] dotMatrix = GetCharMatrix(l);
        int scale = 4;
        int w = 5 * scale;
        int h = 7 * scale;
        int startX = cx - w/2;
        int startY = cy - h/2;

        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (dotMatrix[6-y, x] == 1) // Invertir Y porque textura va de abajo a arriba
                {
                    for(int sy=0; sy<scale; sy++)
                        for(int sx=0; sx<scale; sx++)
                            tex.SetPixel(startX + x*scale + sx, startY + y*scale + sy, c);
                }
            }
        }
    }

    private int[,] GetCharMatrix(char c)
    {
        // Fallback default (cuadrado sólido)
        int[,] matrix = new int[7,5];
        for(int y=0; y<7; y++) for(int x=0; x<5; x++) matrix[y,x] = 1;
        
        // Definiciones básicas
        if (c == 'A') return new int[,] {{0,1,1,1,0},{1,0,0,0,1},{1,0,0,0,1},{1,1,1,1,1},{1,0,0,0,1},{1,0,0,0,1},{1,0,0,0,1}};
        if (c == 'P') return new int[,] {{1,1,1,1,0},{1,0,0,0,1},{1,0,0,0,1},{1,1,1,1,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0}};
        if (c == 'Q') return new int[,] {{0,1,1,1,0},{1,0,0,0,1},{1,0,0,0,1},{1,0,0,0,1},{1,0,1,0,1},{1,0,0,1,0},{0,1,1,0,1}};
        if (c == 'L') return new int[,] {{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,1,1,1,1}};
        
        return matrix;
    }

    private string RemoveAccents(string text)
    {
        return text.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n").Replace("ü", "u");
    }

    private void CreateParticles(Vector3 pos, Color color)
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject p = new GameObject("Particle");
            p.transform.SetParent(gameContainer.transform);
            p.transform.position = pos;
            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = CreateBoxSprite(8, 8, color, false);
            sr.sortingOrder = 100;
            p.transform.localScale = Vector3.one * Random.Range(0.5f, 1.0f);
            
            ParticleController pc = p.AddComponent<ParticleController>();
            pc.velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(2f, 5f));
            pc.lifetime = 0.6f;
        }
    }

    private void DisplaySelectedIngredients()
    {
        float startX = -4.5f;
        float startY = 1.0f;
        int cols = 5;
        for (int i = 0; i < selectedIngredients.Count; i++)
        {
            int r = i / cols;
            int c = i % cols;
            GameObject obj = new GameObject($"Res_{selectedIngredients[i]}");
            obj.transform.SetParent(gameContainer.transform);
            obj.transform.position = new Vector3(startX + c * 2.2f, startY - r * 2.0f, 0);
            
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            Color color = GetIngredientColor(selectedIngredients[i]);
            sr.sprite = CreateIngredientSprite(selectedIngredients[i], color);
            sr.sortingOrder = 5;
            
            // AJUSTE DE ESCALA (Igual que en la tienda):
            // Si es un sprite cargado o fallback textual, lo normalizamos
            if (sr.sprite != null)
            {
                float maxSize = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
                if (maxSize > 0)
                {
                    // En la pantalla de resultados queremos que sean un poco más pequeños para que quepan bien
                    float targetSize = 1.0f; 
                    float scaleFactor = targetSize / maxSize;
                    obj.transform.localScale = Vector3.one * scaleFactor;
                }
            }
            else
            {
                obj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            
            // Etiqueta bajo el ingrediente
            GameObject textObj = new GameObject($"Label_{i}");
            textObj.transform.SetParent(hudObject.transform, false);
            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = selectedIngredients[i].ToUpper();
            label.fontSize = 20;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.enableWordWrapping = false;
            
            // Poner el texto justo debajo del icono 2D
            Vector3 screenPos = mainCamera.WorldToScreenPoint(obj.transform.position + Vector3.down * 1.0f);
            textObj.transform.position = screenPos;
        }
    }

    private void ClearScene()
    {
        if (gameContainer != null) DestroyImmediate(gameContainer);
        gameContainer = new GameObject("Game_Content");
        if (hudObject != null) { foreach (Transform t in hudObject.transform) Destroy(t.gameObject); }
        else {
            hudObject = new GameObject("HUD_Dynamic_Content");
            if (hudCanvas != null) hudObject.transform.SetParent(hudCanvas.transform, false);
        }

        // Limpieza IMPORTANTE de elementos UI persistentes de la fase anterior
        if (hudCanvas != null) {
            Transform tOver = hudCanvas.transform.Find("UI_Overlay"); if(tOver) Destroy(tOver.gameObject);
            Transform tPaper = hudCanvas.transform.Find("UI_MissionPaper"); if(tPaper) Destroy(tPaper.gameObject);
            Transform tBoss = hudCanvas.transform.Find("UI_Boss"); if(tBoss) Destroy(tBoss.gameObject);
            Transform tInstr = hudCanvas.transform.Find("InstrBG"); if(tInstr) Destroy(tInstr.gameObject);
            Transform tIntroInstr = hudCanvas.transform.Find("Intro_InstrBG"); if(tIntroInstr) Destroy(tIntroInstr.gameObject);
        }

        if (titleText != null) titleText.text = "";
        if (instructionsText != null) instructionsText.text = "";
        if (scoreText != null) scoreText.gameObject.SetActive(false);
    }

    private Sprite CreateBoxSprite(int w, int h, Color color, bool border)
    {
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color[] pix = new Color[w * h];
        for (int i = 0; i < pix.Length; i++) pix[i] = color;
        if (border)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x < 6 || x > w - 6 || y < 6 || y > h - 6) pix[y * w + x] = Color.black;
                }
            }
        }
        tex.SetPixels(pix);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateCocineroOakSprite()
    {
        // INTENTO DE CARGAR EL SPRITE REAL DEL USUARIO
        // El usuario debe guardar la imagen como "cocinero_oak.png" en Assets/Resources/Characters o la carpeta de carga
        Sprite s = LoadLocalSprite("Characters/cocinero_oak.png");
        if (s != null) {
            s.texture.filterMode = FilterMode.Point;
            return s;
        }

        // Fallback transparente si no está la imagen aún (para no mostrar el "monigote de minecraft")
        Texture2D tex = new Texture2D(128, 256);
        return Sprite.Create(tex, new Rect(0,0,128,256), new Vector2(0.5f,0.5f));
    }

    private Sprite CreateShopkeeperSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        
        // Limpiar transparente
        for(int i=0; i<size*size; i++) tex.SetPixel(i%size, i/size, Color.clear);

        Color skin = new Color(0.85f, 0.65f, 0.55f);
        Color hair = new Color(0.2f, 0.15f, 0.1f);
        Color apron = new Color(0.3f, 0.5f, 0.3f);
        Color shirt = Color.white;

        // Cabeza
        DrawPixelRect(tex, 24, 40, 16, 16, skin);
        // Cabello/Gorra
        DrawPixelRect(tex, 22, 52, 20, 6, hair);
        // Ojos
        tex.SetPixel(28, 48, Color.black); tex.SetPixel(36, 48, Color.black);
        // Cuerpo
        DrawPixelRect(tex, 20, 15, 24, 25, shirt);
        // Delantal
        DrawPixelRect(tex, 22, 15, 20, 20, apron);
        // Brazos
        DrawPixelRect(tex, 16, 25, 6, 12, skin);
        DrawPixelRect(tex, 42, 25, 6, 12, skin);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 10f);
    }

    private void DrawPixelRect(Texture2D tex, int x, int y, int w, int h, Color c)
    {
        for (int i = x; i < x + w; i++)
            for (int j = y; j < y + h; j++)
                tex.SetPixel(i, j, c);
    }

    private void CreateStarryBackground()
    {
        GameObject bg = new GameObject("BG_Sky");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/space_bg.png");
        if (s != null) {
            sr.sprite = s;
        } else {
            // Fallback procedimental si no existe la imagen
            int w = 256; int h = 256;
            Texture2D tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            Color topColor = new Color(0.02f, 0.05f, 0.15f);
            Color botColor = new Color(0.05f, 0.02f, 0.05f);
            for (int y = 0; y < h; y++) {
                Color rowColor = Color.Lerp(botColor, topColor, (float)y / h);
                for (int x = 0; x < w; x++) {
                    Color c = rowColor;
                    if (Random.value > 0.997f) c = Color.white;
                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 10f);
        }
        
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }

    private void CreateChalkboardBackground()
    {
        GameObject bg = new GameObject("BG_Chalk");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/learning_bg.png");
        if (s != null) {
            sr.sprite = s;
        } else {
            int w = 128; int h = 128;
            Texture2D tex = new Texture2D(w, h);
            Color chalkBase = new Color(0.12f, 0.2f, 0.15f);
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    float noise = Random.Range(-0.02f, 0.02f);
                    tex.SetPixel(x, y, chalkBase + new Color(noise, noise, noise));
                }
            }
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 10f);
        }
        
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }

    private void CreateShopBackground()
    {
        GameObject bg = new GameObject("BG_Market_Full");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/market_bg.png");
        if (s != null) {
            sr.sprite = s;
        } else {
            // Fallback al anterior sistema de pared/suelo
            sr.sprite = GenerateSimpleBackground(new Color(0.8f, 0.7f, 0.6f));
        }
        
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }

    private Sprite LoadLocalSprite(string path)
    {
        string fullPath = System.IO.Path.Combine(Application.dataPath, "Sprites", path);
        if (System.IO.File.Exists(fullPath))
        {
            byte[] data = System.IO.File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(data))
            {
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }
        }
        return null;
    }

    private void ScaleToFillScreen(GameObject obj, float padding = 1.0f, bool preserveAspect = true)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        if (preserveAspect)
        {
            float scaleX = screenWidth / spriteWidth;
            float scaleY = screenHeight / spriteHeight;
            float finalScale = Mathf.Max(scaleX, scaleY) * padding;
            obj.transform.localScale = new Vector3(finalScale, finalScale, 1);
        }
        else
        {
            obj.transform.localScale = new Vector3(
                (screenWidth / spriteWidth) * padding,
                (screenHeight / spriteHeight) * padding,
                1);
        }
    }

    private Sprite GenerateSimpleBackground(Color c)
    {
        Texture2D t = new Texture2D(16, 16);
        for (int i = 0; i < 256; i++) t.SetPixel(i % 16, i / 16, c);
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 1f);
    }

    private void CreateSelectionParticles(Vector3 pos)
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject p = new GameObject("P");
            p.transform.position = pos;
            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = GenerateSimpleBackground(Color.yellow);
            sr.sortingOrder = 100;
            p.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            ParticleController pc = p.AddComponent<ParticleController>();
            pc.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f));
            pc.lifetime = 0.5f;
        }
    }

    private Color GetIngredientColor(string ingredient)
    {
        ingredient = ingredient.ToLower();
        if (ingredient == "avena" || ingredient == "arroz" || ingredient == "pasta" || ingredient == "quinoa") return new Color(0.9f, 0.8f, 0.5f);
        if (ingredient == "pollo" || ingredient == "pavo") return new Color(1f, 0.8f, 0.7f);
        if (ingredient == "salmón" || ingredient == "trucha") return new Color(1f, 0.6f, 0.5f);
        if (ingredient == "merluza" || ingredient == "rodaballo") return new Color(0.9f, 0.9f, 1f);
        if (ingredient == "lentejas" || ingredient == "garbanzos") return new Color(0.7f, 0.4f, 0.2f);
        if (ingredient.Contains("verdura") || ingredient == "calabacín" || ingredient == "brócoli") return new Color(0.3f, 0.7f, 0.3f);
        if (ingredient == "tomate" || ingredient == "fresas" || ingredient == "manzana") return new Color(0.9f, 0.2f, 0.2f);
        if (ingredient == "calabaza" || ingredient == "zanahoria" || ingredient == "boniato") return new Color(1f, 0.5f, 0.1f);
        if (ingredient == "plátano" || ingredient == "banana") return new Color(1f, 0.9f, 0.2f);
        if (ingredient == "queso" || ingredient == "yogurt" || ingredient == "leche") return new Color(0.95f, 0.95f, 1f);
        if (ingredient == "almendras" || ingredient == "nueces") return new Color(0.6f, 0.4f, 0.3f);
        return new Color(0.6f, 0.6f, 0.6f);
    }

    private int CalculateScore(Scenario scenario)
    {
        int score = 0;
        int req = selectedIngredients.Count(i => scenario.requiredIngredients.Contains(i));
        score += (req * 50) / scenario.requiredIngredients.Length;
        int good = selectedIngredients.Count(i => scenario.goodIngredients.Contains(i));
        score += (good * 30) / scenario.goodIngredients.Length;
        int bad = selectedIngredients.Count(i => scenario.badIngredients.Contains(i));
        score -= bad * 20;
        return Mathf.Clamp(score, 0, 100);
    }

    private Color GetRatingColor(int score)
    {
        if (score >= 75) return Color.green;
        if (score >= 50) return Color.yellow;
        return Color.red;
    }

    private string GetFeedback(Scenario s, int score)
    {
        if (score >= 75) return "¡Excelente elección!";
        if (score >= 50) return "Buen intento, revisa las recetas.";
        return "Nivel de nutrición bajo.";
    }

    private class Scenario
    {
        public string name;
        public string description;
        public string[] requiredIngredients;
        public string[] goodIngredients;
        public string[] badIngredients;
    }
}

// COMPONENTES AUXILIARES
public class MenuButton : MonoBehaviour
{
    public System.Action onClick;
    public GameObject buttonText;
    public Color originalColor;
    public GameObject hoverGlow;
    private Vector3 basePos;
    private bool isHovered = false;

    void Start() { basePos = transform.position; }

    private void Update() {
        if (isHovered) {
            // Animación suave de flotación sin romper los píxeles (sin escala)
            transform.position = basePos + new Vector3(0, Mathf.Sin(Time.time * 8) * 0.15f, 0);
        } else {
            transform.position = Vector3.Lerp(transform.position, basePos, Time.deltaTime * 10);
        }
    }

    private void OnMouseEnter() { 
        isHovered = true;
        if (hoverGlow != null) hoverGlow.SetActive(true);
    }
    private void OnMouseExit() { 
        isHovered = false;
        if (hoverGlow != null) hoverGlow.SetActive(false);
    }
    private void OnMouseDown() { onClick?.Invoke(); }
}

public class ShopkeeperAnimator : MonoBehaviour
{
    private void Update() { transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.002f, 0); }
}

public class ParticleController : MonoBehaviour
{
    public Vector2 velocity;
    public float lifetime;
    private float t = 0;
    private void Update()
    {
        t += Time.deltaTime;
        if (t > lifetime) { Destroy(gameObject); return; }
        transform.position += (Vector3)velocity * Time.deltaTime;
        velocity.y -= 9.8f * Time.deltaTime;
    }
}

public class IngredientItem : MonoBehaviour
{
    public string ingredientName;
    public Color color;
    public System.Action<IngredientItem> onSelect;

    private void OnMouseDown()
    {
        onSelect?.Invoke(this);
    }
}
