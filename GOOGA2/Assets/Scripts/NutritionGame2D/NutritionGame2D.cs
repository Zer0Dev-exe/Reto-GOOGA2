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
/// Clase principal: campos, inicialización, flujo de juego y Update
/// </summary>
public partial class NutritionGame2D : MonoBehaviour
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
            
            Transform tResCont = hudCanvas.transform.Find("ResultsContainer"); if(tResCont) Destroy(tResCont.gameObject);
            Transform tBtnEnd = hudCanvas.transform.Find("Btn_Terminar"); if(tBtnEnd) Destroy(tBtnEnd.gameObject);
            Transform tBtnAcc = hudCanvas.transform.Find("Btn_Aceptar"); if(tBtnAcc) Destroy(tBtnAcc.gameObject);
        }

        if (titleText != null) titleText.text = "";
        if (instructionsText != null) instructionsText.text = "";
        if (scoreText != null) scoreText.gameObject.SetActive(false);
    }
}
