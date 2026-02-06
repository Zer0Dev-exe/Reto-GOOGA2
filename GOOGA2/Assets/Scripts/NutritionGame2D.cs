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
        "fresas", "arándanos", "manzana", "pera", "fruta",
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

        if (Input.GetMouseButtonDown(0) && currentPhase == GamePhase.Shopping)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                Ingredient2D ing = hit.collider.GetComponent<Ingredient2D>();
                if (ing != null) SelectIngredient(ing);
            }
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
        CreateStarryBackground();
        titleText.text = "<size=150><color=#FFD700>GOOGAZ</color></size>\n<size=40>EL RETO DE LA NUTRICIÓN</size>";
        instructionsText.text = "PRESIONA <b>ENTER</b> PARA EMPEZAR";
        instructionsText.color = new Color(1f, 0.9f, 0f);
        instructionsText.fontSize = 40;
    }

    private void ShowMenu()
    {
        ClearScene();
        currentPhase = GamePhase.Menu;
        CreateStarryBackground();
        titleText.text = "<size=80><color=#FFD700>SELECCIÓN DE MISIÓN</color></size>";
        instructionsText.text = "ELIGE TU PRÓXIMO RETO";
        instructionsText.color = Color.white;
        instructionsText.fontSize = 32;
        
        string[] labels = { "GIMNASIO", "INSTITUTO", "CASA ABUELOS" };
        Color[] colors = { new Color(0.1f, 0.8f, 0.4f), new Color(0.2f, 0.5f, 0.9f), new Color(0.9f, 0.4f, 0.2f) };
        for (int i = 0; i < scenarios.Length; i++)
        {
            int idx = i;
            CreateButton(labels[i], new Vector3(0, 1.0f - i * 1.5f, 0), colors[i], () => StartScenario(idx));
        }
    }

    private void ShowLearning()
    {
        ClearScene();
        currentPhase = GamePhase.Learning;
        CreateChalkboardBackground();
        
        Scenario s = scenarios[currentScenarioIndex];
        
        titleText.text = $"MISIÓN: {s.name.ToUpper()}";
        titleText.fontSize = 45;
        
        CreateLearningNote(s);

        instructionsText.text = "PRESIONA <b>ENTER</b> PARA IR A LA TIENDA";
        instructionsText.color = Color.white;
        instructionsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -420);
    }

    private void ShowShopping()
    {
        ClearScene();
        currentPhase = GamePhase.Shopping;
        CreateShopBackground();
        titleText.text = "TIENDA DE ALIMENTOS";
        instructionsText.text = "Haz clic en los ingredientes para tu cesta\nPresiona ENTER para finalizar";
        scoreText.text = "CESTA: 0";
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
        GameObject noteObj = new GameObject("LearningNote");
        noteObj.transform.SetParent(gameContainer.transform);
        noteObj.transform.position = new Vector3(0, 0, 5);
        SpriteRenderer sr = noteObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateBoxSprite(1000, 750, new Color(0.98f, 0.92f, 0.75f), true);
        sr.sortingOrder = 1;
        ScaleToFillScreen(noteObj, 0.75f);

        // Texto DENTRO de la nota con COLOR OSCURO para contraste
        GameObject noteTextObj = new GameObject("NoteText");
        noteTextObj.transform.SetParent(hudObject.transform, false);
        TextMeshProUGUI txt = noteTextObj.AddComponent<TextMeshProUGUI>();
        txt.color = new Color(0.15f, 0.1f, 0.05f); // Marrón casi negro
        txt.fontSize = 28;
        txt.alignment = TextAlignmentOptions.Top;
        txt.text = $"\n<size=45><B>LISTA DE LA COMPRA</B></size>\n\n" +
                   $"<size=26>{scenario.description}</size>\n\n" +
                   $"<align=left><indent=15%>" +
                   $"<B>OBJETIVOS:</B>\n" +
                   $"<color=#224422>• {string.Join("\n• ", scenario.requiredIngredients)}</color></indent></align>";
        
        RectTransform rt = txt.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(750, 550);
        rt.anchoredPosition = new Vector2(0, 30);
    }

    private void CreateShopkeeper()
    {
        shopkeeper = new GameObject("Shopkeeper");
        shopkeeper.transform.SetParent(gameContainer.transform);
        shopkeeper.transform.position = new Vector3(-6f, -1f, 0);
        SpriteRenderer sr = shopkeeper.AddComponent<SpriteRenderer>();
        sr.sprite = CreateShopkeeperSprite();
        sr.sortingOrder = 2;
        sr.transform.localScale = new Vector3(2f, 2f, 1f);
        shopkeeper.AddComponent<ShopkeeperAnimator>();
    }

    private void CreateShopShelves()
    {
        int cols = 6;
        float startX = -5.5f;
        float startY = 3.2f;
        
        // Crear estanterías visuales (líneas)
        for (int r = 0; r < 4; r++)
        {
            GameObject shelf = new GameObject("Shelf");
            shelf.transform.SetParent(gameContainer.transform);
            float shelfY = startY - r * 1.6f - 0.7f;
            shelf.transform.position = new Vector3(0, shelfY, 5);
            SpriteRenderer sr = shelf.AddComponent<SpriteRenderer>();
            sr.sprite = CreateBoxSprite(2400, 25, new Color(0.35f, 0.18f, 0.08f), false);
            sr.sortingOrder = 1;
            ScaleToFillScreen(shelf, 1.1f);
            shelf.transform.localScale = new Vector3(shelf.transform.localScale.x, 0.25f, 1);
        }

        for (int i = 0; i < availableIngredients.Length; i++)
        {
            int r = i / cols;
            int c = i % cols;
            CreateIngredient2D(availableIngredients[i], new Vector3(startX + c * 2.2f, startY - r * 1.6f, 0));
        }
    }

    private void CreateIngredient2D(string name, Vector3 pos)
    {
        GameObject obj = new GameObject($"Ing_{name}");
        obj.transform.SetParent(gameContainer.transform);
        obj.transform.position = pos;
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        Color color = GetIngredientColor(name);
        sr.sprite = SpriteGenerator.GenerateIngredientSprite(name, color);
        sr.sortingOrder = 10;
        sr.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        obj.AddComponent<CircleCollider2D>().radius = 0.6f;
        Ingredient2D ing = obj.AddComponent<Ingredient2D>();
        ing.ingredientName = name;
        ing.sprite = sr.sprite;
        ing.baseColor = color;
    }

    private void SelectIngredient(Ingredient2D ingredient)
    {
        if (!selectedIngredients.Contains(ingredient.ingredientName))
        {
            selectedIngredients.Add(ingredient.ingredientName);
            ingredient.OnClick();
            CreateSelectionParticles(ingredient.transform.position);
            GameObject iconObj = new GameObject("CartItem");
            iconObj.transform.SetParent(gameContainer.transform);
            SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
            sr.sprite = ingredient.sprite;
            sr.color = ingredient.baseColor;
            sr.sortingOrder = 20;
            iconObj.transform.position = new Vector3(7.5f, 3.5f - selectedIngredients.Count * 0.4f, 0);
            iconObj.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            scoreText.text = $"CESTA: {selectedIngredients.Count}";
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
            sr.sprite = SpriteGenerator.GenerateIngredientSprite(selectedIngredients[i], color);
            sr.sortingOrder = 5;
            sr.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            
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

    private Sprite CreateShopkeeperSprite()
    {
        Texture2D tex = new Texture2D(64, 64);
        tex.filterMode = FilterMode.Point;
        for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++) tex.SetPixel(x, y, Color.clear);
        for (int y = 10; y < 40; y++) for (int x = 20; x < 44; x++) tex.SetPixel(x, y, new Color(0.3f, 0.5f, 0.8f));
        for (int y = 40; y < 60; y++) for (int x = 22; x < 42; x++) tex.SetPixel(x, y, new Color(0.95f, 0.8f, 0.7f));
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 16f);
    }

    private void CreateStarryBackground()
    {
        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateSimpleBackground(new Color(0.05f, 0.05f, 0.15f));
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }

    private void CreateChalkboardBackground()
    {
        GameObject bg = new GameObject("BG_Chalk");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateSimpleBackground(new Color(0.15f, 0.25f, 0.2f));
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }

    private void CreateShopBackground()
    {
        GameObject bg = new GameObject("BG_Shop");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateSimpleBackground(new Color(0.8f, 0.7f, 0.6f));
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }

    private void ScaleToFillScreen(GameObject obj, float padding = 1.0f)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float height = mainCamera.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;

        obj.transform.localScale = new Vector3(
            (width / sr.sprite.bounds.size.x) * padding,
            (height / sr.sprite.bounds.size.y) * padding,
            1);
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
    private void OnMouseEnter() { transform.localScale = new Vector3(1.1f, 1.1f, 1); }
    private void OnMouseExit() { transform.localScale = Vector3.one; }
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
