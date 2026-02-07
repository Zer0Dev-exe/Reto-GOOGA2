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
    
    // Referencias a contenedores de HUD para controlar visibilidad por fase
    private GameObject titleHudObj;
    private GameObject scoreHudObj;
    private GameObject instructionsHudObj;
    
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
        if (hudCanvas != null)
        {
            // Limpiar elementos persistentes de Resultados o Botones dinámicos del HUD
            Transform resCont = hudCanvas.transform.Find("ResultsContainer");
            if (resCont) Destroy(resCont.gameObject);
            
            Transform btnT = hudCanvas.transform.Find("Btn_Terminar");
            if (btnT) Destroy(btnT.gameObject);
            
            Transform btnA = hudCanvas.transform.Find("Btn_Aceptar");
            if (btnA) Destroy(btnA.gameObject);
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

        // TITULO Y FONDO
        // TITULO Y FONDO
        titleHudObj = new GameObject("TitleHud");
        titleHudObj.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtTitleObj = titleHudObj.AddComponent<RectTransform>();
        // ANCLAJE SUPERIOR CENTRAL (TOP CENTER)
        rtTitleObj.anchorMin = new Vector2(0, 1);
        rtTitleObj.anchorMax = new Vector2(1, 1);
        rtTitleObj.pivot = new Vector2(0.5f, 1);
        rtTitleObj.anchoredPosition = new Vector2(0, 0); // Pegado arriba
        rtTitleObj.sizeDelta = new Vector2(0, 120); // Alto fijo, ancho estirado

        // Fondo (Hijo del objeto título)
        GameObject titleBg = new GameObject("TitleBG");
        titleBg.transform.SetParent(titleHudObj.transform, false);
        Image bgImg = titleBg.AddComponent<Image>();
        bgImg.color = new Color(0,0,0,0.6f);
        RectTransform rtBg = titleBg.GetComponent<RectTransform>();
        rtBg.anchorMin = Vector2.zero;
        rtBg.anchorMax = Vector2.one; // Estira para llenar el padre
        rtBg.sizeDelta = Vector2.zero;
        
        // Texto (Hijo del objeto título, después del fondo para que se pinte encima)
        // Texto (Hijo del objeto título)
        GameObject txtObj = new GameObject("TitleText");
        txtObj.transform.SetParent(titleHudObj.transform, false);
        titleText = txtObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 48; // Base size
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.95f, 0.6f); // Dorado claro
        titleText.fontStyle = FontStyles.Bold;
        
        RectTransform trt = titleText.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one; 
        trt.sizeDelta = Vector2.zero; // Fill parent
        trt.offsetMin = new Vector2(0, 10); // Padding visual
        trt.offsetMax = new Vector2(0, -10);

        // INSTRUCCIONES ESTÁTICAS (Panel inferior)
        instructionsHudObj = new GameObject("InstructionsHud");
        instructionsHudObj.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtInstrObj = instructionsHudObj.AddComponent<RectTransform>();
        rtInstrObj.anchorMin = Vector2.zero;
        rtInstrObj.anchorMax = Vector2.one;
        rtInstrObj.sizeDelta = Vector2.zero;
        
        // Fondo ELIMINADO por petición del usuario ("no deberia salir la barra de abajo")
        // Solo texto con outline para legibilidad
        
        // Texto de instrucciones
        GameObject txtInstrObj = new GameObject("InstrText");
        txtInstrObj.transform.SetParent(instructionsHudObj.transform, false);
        instructionsText = txtInstrObj.AddComponent<TextMeshProUGUI>();
        instructionsText.fontSize = 28;
        instructionsText.alignment = TextAlignmentOptions.Center;
        instructionsText.color = Color.white;
        instructionsText.fontStyle = FontStyles.Bold;
        instructionsText.enableWordWrapping = true;
        
        // Outline para que se lea sin fondo
        instructionsText.outlineWidth = 0.2f;
        instructionsText.outlineColor = new Color(0,0,0,1f);
        
        RectTransform instrRt = instructionsText.GetComponent<RectTransform>();
        instrRt.anchorMin = new Vector2(0, 0);
        instrRt.anchorMax = new Vector2(1, 0.1f); // 10% inferior
        instrRt.offsetMin = new Vector2(20, 10);
        instrRt.offsetMax = new Vector2(-20, 0);

        // PUNTUACIÓN / CESTA
        scoreHudObj = new GameObject("ScoreHud");
        scoreHudObj.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtScoreObj = scoreHudObj.AddComponent<RectTransform>();
        rtScoreObj.anchorMin = new Vector2(1, 1);
        rtScoreObj.anchorMax = new Vector2(1, 1);
        rtScoreObj.anchoredPosition = new Vector2(-150, -50);
        rtScoreObj.sizeDelta = new Vector2(300, 60); // Ancho suficiente para horizontal
        
        // Fondo Puntuación
        GameObject scoreBg = new GameObject("ScoreBG");
        scoreBg.transform.SetParent(scoreHudObj.transform, false);
        Image sBg = scoreBg.AddComponent<Image>();
        sBg.color = new Color(0,0,0,0.7f);
        RectTransform rtSBg = scoreBg.GetComponent<RectTransform>();
        rtSBg.anchorMin = Vector2.zero;
        rtSBg.anchorMax = Vector2.one;
        rtSBg.sizeDelta = Vector2.zero;

        // Texto Puntuación
        GameObject txtScoreObj = new GameObject("ScoreText");
        txtScoreObj.transform.SetParent(scoreHudObj.transform, false);
        scoreText = txtScoreObj.AddComponent<TextMeshProUGUI>();
        scoreText.fontSize = 32;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.color = new Color(0.5f, 1f, 0.5f); // Verde
        
        RectTransform srt = scoreText.GetComponent<RectTransform>();
        srt.anchorMin = Vector2.zero;
        srt.anchorMax = Vector2.one;
        srt.sizeDelta = Vector2.zero;
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

    private void ShowLearning()
    {
        ClearScene(); 
        currentPhase = GamePhase.Learning;
        
        // OCULTAR HUD QUE MOLADE (Título y Score)
        if(titleHudObj) titleHudObj.SetActive(false); // Para que no tape el papel
        if(scoreHudObj) scoreHudObj.SetActive(false);
        if(instructionsHudObj) instructionsHudObj.SetActive(true);

        CreateChalkboardBackground();
        CreateLearningNote(scenarios[currentScenarioIndex]);
        
        // Instructions for this phase
        instructionsText.text = "Pulsa ENTER para comenzar la compra";
    }

    private void ShowIntro()
    {
        ClearScene();
        currentPhase = GamePhase.Intro;
        
        // HUD Config
        if(titleHudObj) titleHudObj.SetActive(true);
        if(scoreHudObj) scoreHudObj.SetActive(false); 
        if(instructionsHudObj) instructionsHudObj.SetActive(true);

        GameObject bgLayer = new GameObject("Intro_BG");
        bgLayer.transform.SetParent(gameContainer.transform);
        bgLayer.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bgLayer.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/intro_bg.png");
        if (s != null) sr.sprite = s;
        else CreateStarryBackground(); 

        ScaleToFillScreen(bgLayer, 1.0f);

        titleText.text = ""; // Logo already in background but we keep TitleHud active for consistent look? No, maybe hide it.
        // Actually, if Intro BG has logo, hide TitleHud.
        if(titleHudObj) titleHudObj.SetActive(false);
        
        instructionsText.text = "PRESIONA <size=50><B><color=#FFD700>ENTER</color></B></size> PARA EMPEZAR";
        instructionsText.color = Color.white;
    }

    private void ShowMenu()
    {
        ClearScene(); 
        
        // SEGURIDAD EXTRA: Asegurar que no queda nada del HUD de resultados
        if (hudCanvas != null)
        {
            Transform resCont = hudCanvas.transform.Find("ResultsContainer");
            if (resCont) Destroy(resCont.gameObject);
        }

        currentPhase = GamePhase.Menu;
        
        // Configurar HUD
        if(titleHudObj) titleHudObj.SetActive(true);
        if(scoreHudObj) scoreHudObj.SetActive(false); 
        if(instructionsHudObj) instructionsHudObj.SetActive(true);

        CreateSelectionBackground(); // Fix: Ensure background is created

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



    private void ShowShopping()
    {
        ClearScene();
        currentPhase = GamePhase.Shopping;
        
        // MOSTRAR HUD COMPLETO
        if(titleHudObj) titleHudObj.SetActive(true);
        if(scoreHudObj) scoreHudObj.SetActive(true);
        if(instructionsHudObj) instructionsHudObj.SetActive(true);

        CreateShopBackground();
        
        titleText.text = "<color=#FFEAB0>EL MERCADO GOOGAZ</color>";
        titleText.fontSize = 65;
        
        instructionsText.text = "Selecciona los ingredientes más saludables para completar tu misión.";
        instructionsText.color = new Color(0.9f, 0.9f, 0.9f);
        instructionsText.fontSize = 28;
        instructionsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -440);

        // BOTÓN TERMINAR (ROJO/NARANJA)
        GameObject btnEnd = new GameObject("Btn_Terminar");
        btnEnd.transform.SetParent(hudCanvas.transform, false);
        Image imgEnd = btnEnd.AddComponent<Image>();
        imgEnd.color = new Color(0.9f, 0.3f, 0.2f); // Rojo anaranjado
        Button btn = btnEnd.AddComponent<Button>();
        btn.onClick.AddListener(()=> ShowResults());
        RectTransform rtEnd = btnEnd.GetComponent<RectTransform>();
        rtEnd.anchorMin = new Vector2(0.5f, 0);
        rtEnd.anchorMax = new Vector2(0.5f, 0);
        rtEnd.anchoredPosition = new Vector2(0, 80);
        rtEnd.sizeDelta = new Vector2(300, 80);
        
        GameObject txtEnd = new GameObject("Text");
        txtEnd.transform.SetParent(btnEnd.transform, false);
        TextMeshProUGUI tmpEnd = txtEnd.AddComponent<TextMeshProUGUI>();
        tmpEnd.text = "TERMINAR";
        tmpEnd.color = Color.white;
        tmpEnd.fontSize = 32;
        tmpEnd.fontStyle = FontStyles.Bold;
        tmpEnd.alignment = TextAlignmentOptions.Center;
        txtEnd.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        txtEnd.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        txtEnd.GetComponent<RectTransform>().anchorMax = Vector2.one;

        // Estilo de Cesta (Score)
        scoreText.text = "CESTA: 0";
        scoreText.color = new Color(1f, 1f, 1f); // Blanco para contrastar con el marrón del mostrador
        scoreText.fontSize = 40;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.outlineWidth = 0.2f; // Outline para resaltar
        scoreText.outlineColor = Color.black;
        
        // Lo ponemos ABAJO A LA IZQUIERDA (Sobre el mostrador de Oak)
        RectTransform srt = scoreText.GetComponent<RectTransform>();
        srt.anchorMin = Vector2.zero;
        srt.anchorMax = Vector2.one;
        srt.offsetMin = Vector2.zero;
        srt.offsetMax = Vector2.zero;
        
        // Asegurar que el contenedor Scores esté activo y posicionado
        scoreHudObj.SetActive(true); // Activar
        
        // Reposicionar el contenedor del Score sobre el mostrador
        RectTransform scoreContRT = scoreHudObj.GetComponent<RectTransform>();
        // Anclaje abajo-izquierda
        scoreContRT.anchorMin = Vector2.zero;
        scoreContRT.anchorMax = Vector2.zero;
        scoreContRT.pivot = new Vector2(0.5f, 0.5f);
        // Posición ajustada a ojo para coincidir con (-6.5, -2.5) en UI
        // (-6.5 en world es muy a la izquierda). En UI 1920x1080, x=0 es borde izquierdo.
        // Vamos a probar con (150, 150)
        // Vamos a probar con (150, 150)
        scoreContRT.anchoredPosition = new Vector2(250, 150); 
        scoreContRT.sizeDelta = new Vector2(300, 100);
        
        // ICONO DE BOLSA DE LA COMPRA
        // ICONO DE BOLSA DE LA COMPRA (Recrear siempre para evitar referencias rotas)
        Transform existingIcon = scoreHudObj.transform.Find("BagIcon");
        if (existingIcon) Destroy(existingIcon.gameObject);

        GameObject iconObj = new GameObject("BagIcon");
        iconObj.transform.SetParent(scoreHudObj.transform, false);
        Image img = iconObj.AddComponent<Image>();
        img.sprite = CreateShoppingBagSprite();
        img.preserveAspect = true;
        RectTransform rtIcon = iconObj.GetComponent<RectTransform>();
        rtIcon.anchorMin = new Vector2(0, 0.5f);
        rtIcon.anchorMax = new Vector2(0, 0.5f);
        rtIcon.pivot = new Vector2(0, 0.5f);
        rtIcon.anchoredPosition = new Vector2(20, 0);
        rtIcon.sizeDelta = new Vector2(80, 80);
        
        iconObj.SetActive(true);

        // Ajustar texto para que no solape el icono
        scoreText.text = "x 0";
        scoreText.alignment = TextAlignmentOptions.Left;
        
        // Reutilizamos la referencia srt
        srt.offsetMin = new Vector2(110, 0); // Padding izquierdo por el icono
        srt.offsetMax = Vector2.zero;
        
        // IMPORTANTE: Activar el objeto de texto (que ClearScene desactiva)
        if(scoreText) scoreText.gameObject.SetActive(true);

        CreateShopkeeper();
        CreateShopShelves();
    }

    private void ShowResults()
    {
        ClearScene(); // Limpia gameContainer (mundo)
        
        // Limpieza explícita del HUD dinámico de Fases anteriores (botones, etc de Shopping)
        Transform oldBtnT = hudCanvas.transform.Find("Btn_Terminar");
        if (oldBtnT) Destroy(oldBtnT.gameObject);
        Transform oldBtnA = hudCanvas.transform.Find("Btn_Aceptar");
        if (oldBtnA) Destroy(oldBtnA.gameObject);

        currentPhase = GamePhase.Results;
        
        // OCULTAR HUD ESTÁNDAR COMPLETO
        if(titleHudObj) titleHudObj.SetActive(false);
        if(scoreHudObj) scoreHudObj.SetActive(false);
        if(instructionsHudObj) instructionsHudObj.SetActive(false);

        // CREAR CONTENEDOR DE RESULTADOS (Para fácil limpieza luego)
        GameObject resContainer = new GameObject("ResultsContainer");
        resContainer.transform.SetParent(hudCanvas.transform, false);
        // Hacemos que ocupe toda la pantalla
        RectTransform rtRes = resContainer.AddComponent<RectTransform>();
        rtRes.anchorMin = Vector2.zero;
        rtRes.anchorMax = Vector2.one;
        rtRes.sizeDelta = Vector2.zero;
        
        // FONDO DE RESULTADOS (Panel UI Semitransparente oscuro)
        GameObject pnl = new GameObject("PanelBG");
        pnl.transform.SetParent(resContainer.transform, false);
        Image imgPnl = pnl.AddComponent<Image>();
        imgPnl.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        RectTransform rtPnl = pnl.GetComponent<RectTransform>();
        rtPnl.anchorMin = new Vector2(0.1f, 0.1f);
        rtPnl.anchorMax = new Vector2(0.9f, 0.9f); // Margen del 10%
        
        Scenario s = scenarios[currentScenarioIndex];
        int score = CalculateScore(s);
        
        // TÍTULO RESULTADOS
        GameObject titleObj = new GameObject("ResTitle");
        titleObj.transform.SetParent(resContainer.transform, false);
        TextMeshProUGUI tTitle = titleObj.AddComponent<TextMeshProUGUI>();
        tTitle.text = "RESUMEN DE MISIÓN";
        tTitle.fontSize = 65;
        tTitle.color = new Color(1f, 0.8f, 0.4f); // Dorado suave
        tTitle.alignment = TextAlignmentOptions.Top;
        tTitle.fontStyle = FontStyles.Bold;
        RectTransform rtT = titleObj.GetComponent<RectTransform>();
        rtT.anchorMin = new Vector2(0.5f, 1f);
        rtT.anchorMax = new Vector2(0.5f, 1f);
        rtT.anchoredPosition = new Vector2(0, -150); // Bajado un poco
        rtT.sizeDelta = new Vector2(800, 100);

        // PUNTUACIÓN
        GameObject scObj = new GameObject("ResScore");
        scObj.transform.SetParent(resContainer.transform, false);
        TextMeshProUGUI tSc = scObj.AddComponent<TextMeshProUGUI>();
        tSc.text = $"PUNTUACIÓN: <color={GetRatingColorHex(score)}>{score}</color>";
        tSc.fontSize = 55;
        tSc.color = Color.white;
        tSc.alignment = TextAlignmentOptions.Top;
        RectTransform rtSc = scObj.GetComponent<RectTransform>();
        rtSc.anchorMin = new Vector2(0.5f, 1f);
        rtSc.anchorMax = new Vector2(0.5f, 1f);
        rtSc.anchoredPosition = new Vector2(0, -250);
        rtSc.sizeDelta = new Vector2(600, 80);

        // FEEDBACK
        GameObject fbObj = new GameObject("ResFeedback");
        fbObj.transform.SetParent(resContainer.transform, false);
        TextMeshProUGUI tFb = fbObj.AddComponent<TextMeshProUGUI>();
        tFb.text = GetFeedback(s, score);
        tFb.fontSize = 32;
        tFb.color = new Color(0.9f, 0.9f, 0.9f);
        tFb.alignment = TextAlignmentOptions.Center;
        RectTransform rtFb = fbObj.GetComponent<RectTransform>();
        rtFb.anchorMin = new Vector2(0.5f, 0.5f);
        rtFb.anchorMax = new Vector2(0.5f, 0.5f);
        rtFb.anchoredPosition = new Vector2(0, 0);
        rtFb.sizeDelta = new Vector2(800, 200);

        // BOTONES DE ACCIÓN (Reintentar / Menú)
        CreateResultButton(resContainer.transform, "REINTENTAR", new Vector2(-200, -350), new Color(0.3f, 0.6f, 1f), () => StartScenario(currentScenarioIndex));
        CreateResultButton(resContainer.transform, "MENÚ", new Vector2(200, -350), new Color(0.8f, 0.4f, 0.2f), () => ShowMenu());
    }
    
    private void CreateResultButton(Transform parent, string label, Vector2 pos, Color c, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject("Btn_"+label);
        btnObj.transform.SetParent(parent, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = c;
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(action);
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(250, 70);
        
        GameObject txtObj = new GameObject("Txt");
        txtObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI tm = txtObj.AddComponent<TextMeshProUGUI>();
        tm.text = label;
        tm.fontSize = 28;
        tm.color = Color.white;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        RectTransform rtT = txtObj.GetComponent<RectTransform>();
        rtT.anchorMin = Vector2.zero;
        rtT.anchorMax = Vector2.one;
        rtT.sizeDelta = Vector2.zero;
    }
    
    private string GetRatingColorHex(int score) {
        if(score >= 10) return "#00FF00";
        if(score >= 5) return "#FFFF00";
        return "#FF0000";
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
        
        // 4. BOTÓN ACEPTAR (MISION) - Reemplaza al texto de "Press Enter" y la barra
        if(instructionsHudObj) instructionsHudObj.SetActive(false); // Ocultar HUD normal aquí

        GameObject btnObj = new GameObject("Btn_Aceptar");
        btnObj.transform.SetParent(hudCanvas.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.8f, 0.2f); // Verde
        
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() => ShowShopping());

        RectTransform rtBtn = btnObj.GetComponent<RectTransform>();
        rtBtn.anchorMin = new Vector2(0.5f, 0);
        rtBtn.anchorMax = new Vector2(0.5f, 0);
        rtBtn.anchoredPosition = new Vector2(0, 80); // Un poco elevado del borde
        rtBtn.sizeDelta = new Vector2(300, 80);

        // Texto del botón
        GameObject txtBtn = new GameObject("Text");
        txtBtn.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI tmpBtn = txtBtn.AddComponent<TextMeshProUGUI>();
        tmpBtn.text = "ACEPTAR";
        tmpBtn.color = Color.white;
        tmpBtn.fontSize = 36;
        tmpBtn.fontStyle = FontStyles.Bold;
        tmpBtn.alignment = TextAlignmentOptions.Center;
        txtBtn.GetComponent<RectTransform>().sizeDelta = Vector2.zero; // Fill
        txtBtn.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        txtBtn.GetComponent<RectTransform>().anchorMax = Vector2.one;
        
        // Y su fondo también
        Transform instrBg = hudCanvas.transform.Find("InstrBG");
        if(instrBg != null) {
            instrBg.SetAsLastSibling();
            instrBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -480);
        }
    }

    private void CreateShopkeeper()
    {
        // El mostrador (ajustado para que no tape los pies si Oak se mueve)
        GameObject counter = new GameObject("Counter");
        counter.transform.SetParent(gameContainer.transform);
        counter.transform.position = new Vector3(-6.5f, -2.5f, 2); // Más abajo
        SpriteRenderer csr = counter.AddComponent<SpriteRenderer>();
        csr.sprite = CreateBoxSprite(400, 250, new Color(0.25f, 0.15f, 0.1f), true);
        csr.sortingOrder = 10; // Delante del personaje
        counter.transform.localScale = new Vector3(1f, 1f, 1f);

        shopkeeper = new GameObject("Shopkeeper");
        shopkeeper.transform.SetParent(gameContainer.transform);
        shopkeeper.transform.position = new Vector3(-6.5f, -1.5f, 1); // BAJADO de -0.5f a -1.5f para que no flote
        SpriteRenderer sr = shopkeeper.AddComponent<SpriteRenderer>();
        // CAMBIO: Usar el Sprite de Oak en lugar del generado procedimentalmente
        sr.sprite = CreateCocineroOakSprite(); 
        sr.sortingOrder = 5; // Detrás del mostrador
        
        // Ajuste de escala más natural para el sprite de Oak (que suele ser más alto/detallado)
        if (sr.sprite != null && sr.sprite.texture.width > 64) {
            sr.transform.localScale = new Vector3(1.3f, 1.3f, 1f); // MUCHO MAS GRANDE como pidió el usuario
            sr.transform.localPosition += new Vector3(0, 0.5f, 0); // Ajuste vertical por el tamaño
        } else {
            sr.transform.localScale = new Vector3(2.0f, 2.0f, 1f); // Escalar si es pixel art pequeño o fallback
        }
        
        shopkeeper.AddComponent<ShopkeeperAnimator>();
    }

    private void CreateShopShelves()
    {
        // En lugar de crear estanterías geométricas, usamos las del fondo para posicionar ingredientes
        float[] shelfHeights = { 1.8f, 0.1f, -1.5f, -3.2f };
        int cols = 6;
        float startX = -3.5f; 
        
        int ingredientIndex = 0;
        // CARGA DE SPRITES: Intentar cargar la hoja de sprites completa primero
        Sprite[] atlasSprites = LoadAndSliceAtlas();
        
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
            
            // Lógica de Sprites: Atlas > Individual > Procedural (Fallback muy básico)
            if (atlasSprites != null && ingredientIndex < atlasSprites.Length) {
                sr.sprite = atlasSprites[ingredientIndex];
            } else {
                sr.sprite = CreateIngredientSprite(ingredientName, color);
            }
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
            // Etiqueta de precio/nombre MEJORADA (Fondo oscuro, texto claro)
            GameObject label = new GameObject("PriceLabel");
            label.transform.SetParent(item.transform, false);
            label.transform.localPosition = new Vector3(0, -0.7f, -0.1f);
            
            // Fondo oscuro redondeado para mejor contraste
            SpriteRenderer labelBg = label.AddComponent<SpriteRenderer>();
            labelBg.sprite = CreateBoxSprite(140, 40, new Color(0.1f, 0.1f, 0.1f, 0.9f), true);
            
            // Ajustar escala del label inversamente al padre para mantener tamaño constante
            float pScale = item.transform.localScale.x;
            if (pScale == 0) pScale = 1;
            label.transform.localScale = new Vector3(0.015f / pScale, 0.015f / pScale, 1);
            
            // Texto dentro del label (usando TextMeshPro hijo para nitidez)
            // Texto dentro del label (usando TextMeshPro hijo para nitidez)
            GameObject txtObj = new GameObject("Txt");
            txtObj.transform.SetParent(label.transform, false);
            txtObj.transform.localPosition = new Vector3(0, 0, -0.1f); // Un poco más cerca de cámara para evitar Z-fighting
            TextMeshPro labelTxt = txtObj.AddComponent<TextMeshPro>();
            labelTxt.text = ingredientName.ToUpper();
            labelTxt.fontSize = 5; // Resolución interna ajustada
            labelTxt.alignment = TextAlignmentOptions.Center;
            labelTxt.color = Color.white;
            // IMPORTANTE: Sorting Order para que se vea sobre el fondo del label y world items
            labelTxt.GetComponent<MeshRenderer>().sortingOrder = 20; 
            
            labelTxt.rectTransform.sizeDelta = new Vector2(14, 4); // Coordenadas locales pequeñas

            item.AddComponent<BoxCollider2D>();
            IngredientItem ii = item.AddComponent<IngredientItem>();
            ii.ingredientName = ingredientName;
            ii.color = color;
            ii.onSelect = (ing) => {
                if (!selectedIngredients.Contains(ing.ingredientName)) {
                    selectedIngredients.Add(ing.ingredientName);
                    scoreText.text = $"x {selectedIngredients.Count}";
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
            filenameNoExt.Replace(" ", "") + "*", // 3. sin espacios (queso fresco -> quesofresco)
            "*" + filenameNoExt.Replace(" ", "") + "*", // 4. contiene nombre sin espacios
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
            // Búsqueda insensible a mayúsculas/minúsculas para Linux
            // Convertimos el patrón de glob a algo manejable manualmente
            string patternClean = pattern.Replace("*", "");
            bool start = pattern.StartsWith("*");
            bool end = pattern.EndsWith("*");
            
            // 1. Buscar en Sprites/Food
            string foodPath = System.IO.Path.Combine(Application.dataPath, "Sprites", "Food");
            if (System.IO.Directory.Exists(foodPath))
            {
                var files = System.IO.Directory.GetFiles(foodPath);
                foreach(var f in files) {
                    string fname = System.IO.Path.GetFileNameWithoutExtension(f);
                    if (IsMatch(fname, patternClean, start, end))
                        return LoadSpriteFromFiles(new string[]{f});
                }
            }

            // 2. Buscar en todo Assets (Más lento pero necesario si no fallamos)
            // Limitamos a la carpeta Recursos para no morir escaneando
            string resPath = System.IO.Path.Combine(Application.dataPath, "Resources");
            if (System.IO.Directory.Exists(resPath))
            {
                var files = System.IO.Directory.GetFiles(resPath, "*", System.IO.SearchOption.AllDirectories);
                foreach(var f in files) {
                    string fname = System.IO.Path.GetFileNameWithoutExtension(f);
                    if (IsMatch(fname, patternClean, start, end))
                        return LoadSpriteFromFiles(new string[]{f});
                }
            }
            
            return null;
        } 
        catch { return null; }
    }
    
    private bool IsMatch(string filename, string pattern, bool startWild, bool endWild) {
        if (startWild && endWild) return filename.IndexOf(pattern, System.StringComparison.OrdinalIgnoreCase) >= 0;
        if (startWild) return filename.EndsWith(pattern, System.StringComparison.OrdinalIgnoreCase);
        if (endWild) return filename.StartsWith(pattern, System.StringComparison.OrdinalIgnoreCase);
        return filename.Equals(pattern, System.StringComparison.OrdinalIgnoreCase);
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

    private Sprite[] LoadAndSliceAtlas()
    {
        Sprite atlas = LoadLocalSprite("Ingredients/food_icons_atlas.png"); // Nombre fijo del asset generado
        if (atlas == null) return null;

        Texture2D tex = atlas.texture;
        tex.filterMode = FilterMode.Point; // Pixel Art nítido
        
        int cols = 7;
        int rows = 4;
        int total = cols * rows;
        Sprite[] sprites = new Sprite[total];

        int w = tex.width / cols;
        int h = tex.height / rows;

        for (int i = 0; i < total; i++)
        {
            int c = i % cols;
            int r = rows - 1 - (i / cols); // Lectura de arriba a abajo (Coordenadas UV empiezan abajo)
            
            // Creación del recorte
            Rect rect = new Rect(c * w, r * h, w, h);
            sprites[i] = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), 16f);
        }
        return sprites;
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
            
            // Limpiar elementos de Resultados y Botones dinámicos
            Transform tResCont = hudCanvas.transform.Find("ResultsContainer"); if(tResCont) Destroy(tResCont.gameObject);
            Transform tBtnEnd = hudCanvas.transform.Find("Btn_Terminar"); if(tBtnEnd) Destroy(tBtnEnd.gameObject);
            Transform tBtnAcc = hudCanvas.transform.Find("Btn_Aceptar"); if(tBtnAcc) Destroy(tBtnAcc.gameObject);
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
        // 0. Ruta directa confiable (si existe)
        Sprite s = LoadLocalSprite("Characters/cocinero_oak.png");
        
        // 1. Si falla, intentar buscar variantes recursivas
        if (s == null) s = FindSpriteRecursive("cocinero_oak");
        if (s == null) s = FindSpriteRecursive("oak");
        if (s == null) s = FindSpriteRecursive("chef");
        
        if (s != null) {
            s.texture.filterMode = FilterMode.Point;
            return s;
        }

        // 2. Fallback VISIBLE (Cuadrado blanco/gris con forma básica)
        int w=128, h=256;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color bodyColor = new Color(0.9f, 0.9f, 0.9f); // Blanco sucio
        Color skinColor = new Color(1f, 0.8f, 0.6f);
        
        for(int y=0; y<h; y++) {
            for(int x=0; x<w; x++) {
                if (y > 100) tex.SetPixel(x, y, bodyColor); // Cuerpo
                else if (y > 60) tex.SetPixel(x,y, skinColor); // Cabeza
                else tex.SetPixel(x,y, Color.clear);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(0.5f,0.5f));
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

    private Sprite CreateShoppingBagSprite()
    {
        int w=32, h=32;
        Texture2D t = new Texture2D(w, h);
        t.filterMode = FilterMode.Point;
        Color clear = Color.clear;
        Color bagColor = new Color(0.8f, 0.6f, 0.4f); // Papel kraft
        Color shadow = new Color(0.6f, 0.4f, 0.2f);
        
        for(int y=0; y<h; y++) {
            for(int x=0; x<w; x++) {
                t.SetPixel(x, y, clear);
                // Cuerpo de la bolsa
                if(x >= 6 && x <= 26 && y >= 4 && y <= 24) {
                    t.SetPixel(x, y, bagColor);
                    // Sombrita lateral
                    if(x == 6 || x == 26 || y == 4) t.SetPixel(x,y, shadow); 
                }
                // Asas
                if ((x >= 10 && x <= 12 && y > 24 && y < 29) || (x >= 20 && x <= 22 && y > 24 && y < 29) || (x>=10 && x<=22 && y>=28 && y<=29)) {
                    if(!(x > 12 && x < 20 && y < 28)) // Hueco del asa
                        t.SetPixel(x, y, shadow);
                }
            }
        }
        t.Apply();
        return Sprite.Create(t, new Rect(0,0,w,h), new Vector2(0.5f,0.5f));
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
        if (ingredient == "tomate" || ingredient == "fresas" || ingredient == "manzana" || ingredient.Contains("fruta")) return new Color(0.9f, 0.2f, 0.2f);
        if (ingredient == "calabaza" || ingredient == "zanahoria" || ingredient == "boniato") return new Color(1f, 0.5f, 0.1f);
        if (ingredient == "plátano" || ingredient == "banana" || ingredient == "patata") return new Color(1f, 0.9f, 0.2f);
        if (ingredient == "queso" || ingredient.Contains("queso") || ingredient == "yogurt" || ingredient == "leche") return new Color(0.95f, 0.95f, 1f);
        if (ingredient == "almendras" || ingredient == "nueces") return new Color(0.6f, 0.4f, 0.3f);
        if (ingredient == "pera") return new Color(0.7f, 0.8f, 0.2f);
        if (ingredient == "arándanos") return new Color(0.4f, 0.2f, 0.7f);
        return new Color(0.6f, 0.6f, 0.6f); // GREY (Default)
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
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        transform.localScale = originalScale * 1.2f; // Efecto hover
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    private void OnMouseDown()
    {
        onSelect?.Invoke(this);
    }
}
