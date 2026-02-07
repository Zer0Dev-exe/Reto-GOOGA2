using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// NutritionGame2D - Fase de Resultados y puntuación
/// </summary>
public partial class NutritionGame2D
{
    private void ShowResults()
    {
        ClearScene();
        
        // Limpieza explícita del HUD dinámico de fases anteriores
        Transform oldBtnT = hudCanvas.transform.Find("Btn_Terminar");
        if (oldBtnT) Destroy(oldBtnT.gameObject);
        Transform oldBtnA = hudCanvas.transform.Find("Btn_Aceptar");
        if (oldBtnA) Destroy(oldBtnA.gameObject);

        currentPhase = GamePhase.Results;
        
        // OCULTAR HUD ESTÁNDAR COMPLETO
        if(titleHudObj) titleHudObj.SetActive(false);
        if(scoreHudObj) scoreHudObj.SetActive(false);
        if(instructionsHudObj) instructionsHudObj.SetActive(false);

        // CREAR CONTENEDOR DE RESULTADOS
        GameObject resContainer = new GameObject("ResultsContainer");
        resContainer.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtRes = resContainer.AddComponent<RectTransform>();
        rtRes.anchorMin = Vector2.zero;
        rtRes.anchorMax = Vector2.one;
        rtRes.sizeDelta = Vector2.zero;
        
        // FONDO DE RESULTADOS
        GameObject pnl = new GameObject("PanelBG");
        pnl.transform.SetParent(resContainer.transform, false);
        Image imgPnl = pnl.AddComponent<Image>();
        imgPnl.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        RectTransform rtPnl = pnl.GetComponent<RectTransform>();
        rtPnl.anchorMin = new Vector2(0.1f, 0.1f);
        rtPnl.anchorMax = new Vector2(0.9f, 0.9f);
        
        Scenario s = scenarios[currentScenarioIndex];
        int score = CalculateScore(s);
        
        // TÍTULO RESULTADOS
        GameObject titleObj = new GameObject("ResTitle");
        titleObj.transform.SetParent(resContainer.transform, false);
        TextMeshProUGUI tTitle = titleObj.AddComponent<TextMeshProUGUI>();
        tTitle.text = "RESUMEN DE MISIÓN";
        tTitle.fontSize = 65;
        tTitle.color = new Color(1f, 0.8f, 0.4f);
        tTitle.alignment = TextAlignmentOptions.Top;
        tTitle.fontStyle = FontStyles.Bold;
        RectTransform rtT = titleObj.GetComponent<RectTransform>();
        rtT.anchorMin = new Vector2(0.5f, 1f);
        rtT.anchorMax = new Vector2(0.5f, 1f);
        rtT.anchoredPosition = new Vector2(0, -150);
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
            
            if (sr.sprite != null)
            {
                float maxSize = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
                if (maxSize > 0)
                {
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
            
            Vector3 screenPos = mainCamera.WorldToScreenPoint(obj.transform.position + Vector3.down * 1.0f);
            textObj.transform.position = screenPos;
        }
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
}
