using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// NutritionGame2D - Pantalla de Feedback detallado
/// Muestra análisis nutricional completo con desglose de ingredientes
/// </summary>
public partial class NutritionGame2D
{
    private void ShowFeedback()
    {
        ClearScene();
        currentPhase = GamePhase.Feedback;

        // OCULTAR HUD ESTÁNDAR
        if (titleHudObj) titleHudObj.SetActive(false);
        if (scoreHudObj) scoreHudObj.SetActive(false);
        if (instructionsHudObj) instructionsHudObj.SetActive(false);

        Scenario s = scenarios[currentScenarioIndex];
        int score = CalculateScore(s);

        // FONDO
        GameObject bg = new GameObject("Feedback_BG");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
        bgSr.sprite = GenerateSimpleBackground(new Color(0.08f, 0.08f, 0.12f));
        bgSr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);

        // CONTENEDOR PRINCIPAL
        GameObject feedContainer = new GameObject("FeedbackContainer");
        feedContainer.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtFeed = feedContainer.AddComponent<RectTransform>();
        rtFeed.anchorMin = Vector2.zero;
        rtFeed.anchorMax = Vector2.one;
        rtFeed.sizeDelta = Vector2.zero;

        // PANEL DE FONDO
        GameObject pnl = new GameObject("FeedbackPanelBG");
        pnl.transform.SetParent(feedContainer.transform, false);
        Image imgPnl = pnl.AddComponent<Image>();
        imgPnl.color = new Color(0.12f, 0.12f, 0.18f, 0.95f);
        RectTransform rtPnl = pnl.GetComponent<RectTransform>();
        rtPnl.anchorMin = new Vector2(0.05f, 0.05f);
        rtPnl.anchorMax = new Vector2(0.95f, 0.95f);

        // === TÍTULO ===
        GameObject titleObj = new GameObject("FeedTitle");
        titleObj.transform.SetParent(feedContainer.transform, false);
        TextMeshProUGUI tTitle = titleObj.AddComponent<TextMeshProUGUI>();
        tTitle.text = "ANÁLISIS NUTRICIONAL";
        tTitle.fontSize = 55;
        tTitle.color = new Color(1f, 0.85f, 0.4f);
        tTitle.alignment = TextAlignmentOptions.Center;
        tTitle.fontStyle = FontStyles.Bold;
        RectTransform rtT = titleObj.GetComponent<RectTransform>();
        rtT.anchorMin = new Vector2(0.5f, 1f);
        rtT.anchorMax = new Vector2(0.5f, 1f);
        rtT.anchoredPosition = new Vector2(0, -80);
        rtT.sizeDelta = new Vector2(800, 80);

        // === ESCENARIO ===
        GameObject scenarioObj = new GameObject("FeedScenario");
        scenarioObj.transform.SetParent(feedContainer.transform, false);
        TextMeshProUGUI tScen = scenarioObj.AddComponent<TextMeshProUGUI>();
        tScen.text = $"<color=#AACCFF>Escenario:</color> {s.name}";
        tScen.fontSize = 28;
        tScen.color = Color.white;
        tScen.alignment = TextAlignmentOptions.Center;
        RectTransform rtScen = scenarioObj.GetComponent<RectTransform>();
        rtScen.anchorMin = new Vector2(0.5f, 1f);
        rtScen.anchorMax = new Vector2(0.5f, 1f);
        rtScen.anchoredPosition = new Vector2(0, -145);
        rtScen.sizeDelta = new Vector2(700, 40);

        // === PUNTUACIÓN ===
        GameObject scoreObj = new GameObject("FeedScore");
        scoreObj.transform.SetParent(feedContainer.transform, false);
        TextMeshProUGUI tScore = scoreObj.AddComponent<TextMeshProUGUI>();
        tScore.text = $"Puntuación Final: <color={GetRatingColorHex(score)}>{score} / 100</color>";
        tScore.fontSize = 36;
        tScore.color = Color.white;
        tScore.alignment = TextAlignmentOptions.Center;
        tScore.fontStyle = FontStyles.Bold;
        RectTransform rtSc = scoreObj.GetComponent<RectTransform>();
        rtSc.anchorMin = new Vector2(0.5f, 1f);
        rtSc.anchorMax = new Vector2(0.5f, 1f);
        rtSc.anchoredPosition = new Vector2(0, -190);
        rtSc.sizeDelta = new Vector2(600, 50);

        // === SEPARADOR ===
        CreateSeparator(feedContainer.transform, new Vector2(0, -225), 650f);

        // === DESGLOSE DE INGREDIENTES ===
        float yPos = -265f;

        // --- Ingredientes requeridos ---
        yPos = CreateIngredientSection(feedContainer.transform, "INGREDIENTES REQUERIDOS",
            s.requiredIngredients, new Color(0.3f, 1f, 0.3f), new Color(0.5f, 0.5f, 0.5f), yPos);

        // --- Ingredientes buenos ---
        yPos = CreateIngredientSection(feedContainer.transform, "INGREDIENTES BUENOS",
            s.goodIngredients, new Color(0.6f, 0.85f, 1f), new Color(0.4f, 0.4f, 0.5f), yPos);

        // --- Ingredientes malos seleccionados ---
        string[] selectedBad = selectedIngredients.Where(i => s.badIngredients.Contains(i)).ToArray();
        if (selectedBad.Length > 0)
        {
            yPos -= 10f;
            GameObject badLabel = new GameObject("FeedBadLabel");
            badLabel.transform.SetParent(feedContainer.transform, false);
            TextMeshProUGUI tBad = badLabel.AddComponent<TextMeshProUGUI>();
            tBad.text = "⚠ INGREDIENTES PERJUDICIALES SELECCIONADOS";
            tBad.fontSize = 22;
            tBad.color = new Color(1f, 0.4f, 0.3f);
            tBad.alignment = TextAlignmentOptions.Center;
            tBad.fontStyle = FontStyles.Bold;
            RectTransform rtBad = badLabel.GetComponent<RectTransform>();
            rtBad.anchorMin = new Vector2(0.5f, 1f);
            rtBad.anchorMax = new Vector2(0.5f, 1f);
            rtBad.anchoredPosition = new Vector2(0, yPos);
            rtBad.sizeDelta = new Vector2(700, 30);
            yPos -= 30f;

            string badList = string.Join("  •  ", selectedBad.Select(i => i.ToUpper()));
            GameObject badItems = new GameObject("FeedBadItems");
            badItems.transform.SetParent(feedContainer.transform, false);
            TextMeshProUGUI tBadItems = badItems.AddComponent<TextMeshProUGUI>();
            tBadItems.text = badList;
            tBadItems.fontSize = 20;
            tBadItems.color = new Color(1f, 0.6f, 0.5f);
            tBadItems.alignment = TextAlignmentOptions.Center;
            RectTransform rtBadItems = badItems.GetComponent<RectTransform>();
            rtBadItems.anchorMin = new Vector2(0.5f, 1f);
            rtBadItems.anchorMax = new Vector2(0.5f, 1f);
            rtBadItems.anchoredPosition = new Vector2(0, yPos);
            rtBadItems.sizeDelta = new Vector2(700, 30);
            yPos -= 35f;
        }

        // === SEPARADOR ===
        CreateSeparator(feedContainer.transform, new Vector2(0, yPos), 650f);
        yPos -= 15f;

        // === CONSEJO NUTRICIONAL ===
        GameObject adviceObj = new GameObject("FeedAdvice");
        adviceObj.transform.SetParent(feedContainer.transform, false);
        TextMeshProUGUI tAdvice = adviceObj.AddComponent<TextMeshProUGUI>();
        tAdvice.text = GetDetailedAdvice(s, score);
        tAdvice.fontSize = 22;
        tAdvice.color = new Color(0.9f, 0.9f, 0.8f);
        tAdvice.alignment = TextAlignmentOptions.Center;
        tAdvice.enableWordWrapping = true;
        RectTransform rtAdv = adviceObj.GetComponent<RectTransform>();
        rtAdv.anchorMin = new Vector2(0.5f, 1f);
        rtAdv.anchorMax = new Vector2(0.5f, 1f);
        rtAdv.anchoredPosition = new Vector2(0, yPos - 30f);
        rtAdv.sizeDelta = new Vector2(750, 80);

        // === BOTONES ===
        float btnY = -480f;
        CreateResultButton(feedContainer.transform, "VOLVER A RESULTADOS", new Vector2(-250, btnY), new Color(0.5f, 0.5f, 0.6f), () => ShowResults());
        CreateResultButton(feedContainer.transform, "MENÚ", new Vector2(250, btnY), new Color(0.8f, 0.4f, 0.2f), () => ShowMenu());
    }

    // === HELPERS DE FEEDBACK ===

    private void CreateSeparator(Transform parent, Vector2 pos, float width)
    {
        GameObject sep = new GameObject("Separator");
        sep.transform.SetParent(parent, false);
        Image sepImg = sep.AddComponent<Image>();
        sepImg.color = new Color(1f, 0.85f, 0.4f, 0.3f);
        RectTransform rtSep = sep.GetComponent<RectTransform>();
        rtSep.anchorMin = new Vector2(0.5f, 1f);
        rtSep.anchorMax = new Vector2(0.5f, 1f);
        rtSep.anchoredPosition = pos;
        rtSep.sizeDelta = new Vector2(width, 2);
    }

    private float CreateIngredientSection(Transform parent, string title, string[] ingredients,
        Color selectedColor, Color missingColor, float yStart)
    {
        float y = yStart;

        // Título de sección
        GameObject secLabel = new GameObject($"Feed_{title}");
        secLabel.transform.SetParent(parent, false);
        TextMeshProUGUI tSec = secLabel.AddComponent<TextMeshProUGUI>();
        tSec.text = title;
        tSec.fontSize = 20;
        tSec.color = new Color(1f, 0.95f, 0.7f);
        tSec.alignment = TextAlignmentOptions.Center;
        tSec.fontStyle = FontStyles.Bold;
        RectTransform rtSec = secLabel.GetComponent<RectTransform>();
        rtSec.anchorMin = new Vector2(0.5f, 1f);
        rtSec.anchorMax = new Vector2(0.5f, 1f);
        rtSec.anchoredPosition = new Vector2(0, y);
        rtSec.sizeDelta = new Vector2(700, 30);
        y -= 30f;

        // Lista de ingredientes con estado
        string items = "";
        int selected = 0;
        for (int i = 0; i < ingredients.Length; i++)
        {
            bool wasSelected = selectedIngredients.Contains(ingredients[i]);
            if (wasSelected) selected++;

            string colorHex = wasSelected
                ? ColorUtility.ToHtmlStringRGB(selectedColor)
                : ColorUtility.ToHtmlStringRGB(missingColor);
            string mark = wasSelected ? "✓" : "✗";

            items += $"<color=#{colorHex}>{mark} {ingredients[i].ToUpper()}</color>";
            if (i < ingredients.Length - 1) items += "    ";
        }

        GameObject itemsObj = new GameObject($"FeedItems_{title}");
        itemsObj.transform.SetParent(parent, false);
        TextMeshProUGUI tItems = itemsObj.AddComponent<TextMeshProUGUI>();
        tItems.text = items;
        tItems.fontSize = 18;
        tItems.color = Color.white;
        tItems.alignment = TextAlignmentOptions.Center;
        tItems.enableWordWrapping = true;
        RectTransform rtItems = itemsObj.GetComponent<RectTransform>();
        rtItems.anchorMin = new Vector2(0.5f, 1f);
        rtItems.anchorMax = new Vector2(0.5f, 1f);
        rtItems.anchoredPosition = new Vector2(0, y);
        rtItems.sizeDelta = new Vector2(750, 35);
        y -= 30f;

        // Contador
        GameObject countObj = new GameObject($"FeedCount_{title}");
        countObj.transform.SetParent(parent, false);
        TextMeshProUGUI tCount = countObj.AddComponent<TextMeshProUGUI>();
        tCount.text = $"({selected}/{ingredients.Length} seleccionados)";
        tCount.fontSize = 16;
        tCount.color = new Color(0.7f, 0.7f, 0.7f);
        tCount.alignment = TextAlignmentOptions.Center;
        RectTransform rtCount = countObj.GetComponent<RectTransform>();
        rtCount.anchorMin = new Vector2(0.5f, 1f);
        rtCount.anchorMax = new Vector2(0.5f, 1f);
        rtCount.anchoredPosition = new Vector2(0, y);
        rtCount.sizeDelta = new Vector2(400, 25);
        y -= 30f;

        return y;
    }

    private string GetDetailedAdvice(Scenario s, int score)
    {
        int reqCount = selectedIngredients.Count(i => s.requiredIngredients.Contains(i));
        int goodCount = selectedIngredients.Count(i => s.goodIngredients.Contains(i));
        int badCount = selectedIngredients.Count(i => s.badIngredients.Contains(i));

        string advice = "";

        if (score >= 75)
        {
            advice = "¡Excelente trabajo! Has seleccionado la mayoría de ingredientes esenciales. ";
            if (badCount == 0)
                advice += "Además, has evitado todos los ingredientes perjudiciales. ¡Nutrición perfecta!";
            else
                advice += $"Sin embargo, has incluido {badCount} ingrediente(s) perjudicial(es). ¡Intenta evitarlos la próxima vez!";
        }
        else if (score >= 50)
        {
            advice = $"Buen intento. Has acertado {reqCount} de {s.requiredIngredients.Length} ingredientes esenciales. ";
            if (goodCount > 0)
                advice += $"También incluiste {goodCount} ingrediente(s) beneficioso(s). ";
            advice += "Revisa la fase de aprendizaje para mejorar tu selección.";
        }
        else
        {
            advice = $"Necesitas mejorar. Solo acertaste {reqCount} de {s.requiredIngredients.Length} ingredientes esenciales. ";
            if (badCount > 0)
                advice += $"Incluiste {badCount} ingrediente(s) perjudicial(es) que restan puntos. ";
            advice += "Te recomendamos volver a estudiar las necesidades nutricionales del escenario.";
        }

        return advice;
    }
}
