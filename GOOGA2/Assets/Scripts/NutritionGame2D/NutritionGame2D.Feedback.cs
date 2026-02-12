using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// NutritionGame2D - Pantalla de Feedback detallado
/// Muestra análisis nutricional completo con desglose de ingredientes
/// y explicaciones de por qué cada alimento es bueno o malo para el escenario.
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
        tTitle.fontSize = 48;
        tTitle.color = new Color(1f, 0.85f, 0.4f);
        tTitle.alignment = TextAlignmentOptions.Center;
        tTitle.fontStyle = FontStyles.Bold;
        RectTransform rtT = titleObj.GetComponent<RectTransform>();
        rtT.anchorMin = new Vector2(0.5f, 1f);
        rtT.anchorMax = new Vector2(0.5f, 1f);
        rtT.anchoredPosition = new Vector2(0, -60);
        rtT.sizeDelta = new Vector2(800, 60);

        // === ESCENARIO ===
        GameObject scenarioObj = new GameObject("FeedScenario");
        scenarioObj.transform.SetParent(feedContainer.transform, false);
        TextMeshProUGUI tScen = scenarioObj.AddComponent<TextMeshProUGUI>();
        tScen.text = $"<color=#AACCFF>Escenario:</color> {s.name}";
        tScen.fontSize = 24;
        tScen.color = Color.white;
        tScen.alignment = TextAlignmentOptions.Center;
        RectTransform rtScen = scenarioObj.GetComponent<RectTransform>();
        rtScen.anchorMin = new Vector2(0.5f, 1f);
        rtScen.anchorMax = new Vector2(0.5f, 1f);
        rtScen.anchoredPosition = new Vector2(0, -115);
        rtScen.sizeDelta = new Vector2(700, 35);

        // === PUNTUACIÓN ===
        GameObject scoreObj = new GameObject("FeedScore");
        scoreObj.transform.SetParent(feedContainer.transform, false);
        TextMeshProUGUI tScore = scoreObj.AddComponent<TextMeshProUGUI>();
        tScore.text = $"Puntuación Final: <color={GetRatingColorHex(score)}>{score} / 100</color>";
        tScore.fontSize = 30;
        tScore.color = Color.white;
        tScore.alignment = TextAlignmentOptions.Center;
        tScore.fontStyle = FontStyles.Bold;
        RectTransform rtSc = scoreObj.GetComponent<RectTransform>();
        rtSc.anchorMin = new Vector2(0.5f, 1f);
        rtSc.anchorMax = new Vector2(0.5f, 1f);
        rtSc.anchoredPosition = new Vector2(0, -155);
        rtSc.sizeDelta = new Vector2(600, 40);

        // === SEPARADOR ===
        CreateSeparator(feedContainer.transform, new Vector2(0, -185), 650f);

        // === SCROLL VIEW para el contenido detallado ===
        GameObject scrollObj = new GameObject("FeedScrollView");
        scrollObj.transform.SetParent(feedContainer.transform, false);
        RectTransform rtScroll = scrollObj.AddComponent<RectTransform>();
        rtScroll.anchorMin = new Vector2(0.08f, 0f);
        rtScroll.anchorMax = new Vector2(0.92f, 1f);
        rtScroll.offsetMin = new Vector2(0, 100);   // margen inferior (botones)
        rtScroll.offsetMax = new Vector2(0, -200);   // margen superior (título, score)

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        Image scrollBg = scrollObj.AddComponent<Image>();
        scrollBg.color = new Color(0, 0, 0, 0.01f); // casi invisible, para raycast
        scrollObj.AddComponent<Mask>().showMaskGraphic = false;

        // Contenido del scroll
        GameObject contentObj = new GameObject("ScrollContent");
        contentObj.transform.SetParent(scrollObj.transform, false);
        RectTransform rtContent = contentObj.AddComponent<RectTransform>();
        rtContent.anchorMin = new Vector2(0, 1);
        rtContent.anchorMax = new Vector2(1, 1);
        rtContent.pivot = new Vector2(0.5f, 1);
        rtContent.anchoredPosition = Vector2.zero;

        scrollRect.content = rtContent;

        float yPos = 0f;
        float contentWidth = 750f;

        // --- SECCIÓN: Ingredientes Requeridos ---
        yPos = CreateDetailedIngredientSection(contentObj.transform, s,
            "INGREDIENTES ESENCIALES",
            "Estos alimentos son fundamentales para este escenario:",
            s.requiredIngredients,
            new Color(0.2f, 0.8f, 0.2f),   // color título
            new Color(0.15f, 0.25f, 0.15f), // color fondo tarjeta
            new Color(0.3f, 1f, 0.3f),      // color seleccionado
            new Color(0.5f, 0.5f, 0.5f),    // color no seleccionado
            yPos, contentWidth);

        yPos -= 15f;

        // --- SECCIÓN: Ingredientes Buenos ---
        yPos = CreateDetailedIngredientSection(contentObj.transform, s,
            "INGREDIENTES BENEFICIOSOS",
            "Estos alimentos aportan beneficios adicionales:",
            s.goodIngredients,
            new Color(0.5f, 0.7f, 1f),
            new Color(0.12f, 0.18f, 0.28f),
            new Color(0.6f, 0.85f, 1f),
            new Color(0.4f, 0.4f, 0.5f),
            yPos, contentWidth);

        yPos -= 15f;

        // --- SECCIÓN: Ingredientes Malos Seleccionados ---
        string[] selectedBad = selectedIngredients.Where(i => s.badIngredients.Contains(i)).ToArray();
        if (selectedBad.Length > 0)
        {
            yPos = CreateDetailedIngredientSection(contentObj.transform, s,
                "⚠ INGREDIENTES PERJUDICIALES SELECCIONADOS",
                "¡Estos alimentos son perjudiciales para este escenario!",
                selectedBad,
                new Color(1f, 0.4f, 0.3f),
                new Color(0.3f, 0.12f, 0.1f),
                new Color(1f, 0.5f, 0.4f),
                new Color(0.6f, 0.3f, 0.3f),
                yPos, contentWidth);
            yPos -= 15f;
        }

        // --- SECCIÓN: Ingredientes Malos NO seleccionados (info educativa) ---
        string[] notSelectedBad = s.badIngredients.Where(i => !selectedIngredients.Contains(i)).ToArray();
        if (notSelectedBad.Length > 0)
        {
            yPos = CreateDetailedIngredientSection(contentObj.transform, s,
                "EVITASTE CORRECTAMENTE",
                "¡Bien hecho! Estos alimentos debes evitar por estas razones:",
                notSelectedBad,
                new Color(0.6f, 0.9f, 0.5f),
                new Color(0.15f, 0.2f, 0.12f),
                new Color(0.7f, 0.9f, 0.6f),
                new Color(0.5f, 0.6f, 0.4f),
                yPos, contentWidth);
            yPos -= 15f;
        }

        // === SEPARADOR FINAL ===
        CreateSeparator(contentObj.transform, new Vector2(0, yPos), 650f);
        yPos -= 20f;

        // === CONSEJO NUTRICIONAL ===
        GameObject adviceObj = new GameObject("FeedAdvice");
        adviceObj.transform.SetParent(contentObj.transform, false);
        TextMeshProUGUI tAdvice = adviceObj.AddComponent<TextMeshProUGUI>();
        tAdvice.text = GetDetailedAdvice(s, score);
        tAdvice.fontSize = 20;
        tAdvice.color = new Color(0.9f, 0.9f, 0.8f);
        tAdvice.alignment = TextAlignmentOptions.Center;
        tAdvice.enableWordWrapping = true;
        RectTransform rtAdv = adviceObj.GetComponent<RectTransform>();
        rtAdv.anchorMin = new Vector2(0.5f, 1f);
        rtAdv.anchorMax = new Vector2(0.5f, 1f);
        rtAdv.anchoredPosition = new Vector2(0, yPos - 40f);
        rtAdv.sizeDelta = new Vector2(contentWidth, 100);
        yPos -= 140f;

        // Ajustar tamaño del content para scroll
        rtContent.sizeDelta = new Vector2(0, Mathf.Abs(yPos) + 20f);

        // === BOTONES (fuera del scroll) ===
        float btnY = 40f;
        CreateFeedbackButton(feedContainer.transform, "VOLVER A RESULTADOS", new Vector2(-250, btnY), new Color(0.5f, 0.5f, 0.6f), () => ShowResults());
        CreateFeedbackButton(feedContainer.transform, "MENÚ", new Vector2(250, btnY), new Color(0.8f, 0.4f, 0.2f), () => ShowMenu());
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

    /// <summary>
    /// Crea una sección detallada de ingredientes con tarjetas individuales
    /// que muestran el nombre, estado (seleccionado/no) y la explicación nutricional.
    /// </summary>
    private float CreateDetailedIngredientSection(Transform parent, Scenario s,
        string sectionTitle, string sectionSubtitle, string[] ingredients,
        Color titleColor, Color cardBgColor, Color selectedColor, Color missingColor,
        float yStart, float contentWidth)
    {
        float y = yStart;

        // Título de sección
        GameObject secLabel = new GameObject($"Feed_{sectionTitle}");
        secLabel.transform.SetParent(parent, false);
        TextMeshProUGUI tSec = secLabel.AddComponent<TextMeshProUGUI>();
        tSec.text = sectionTitle;
        tSec.fontSize = 22;
        tSec.color = titleColor;
        tSec.alignment = TextAlignmentOptions.Center;
        tSec.fontStyle = FontStyles.Bold;
        RectTransform rtSec = secLabel.GetComponent<RectTransform>();
        rtSec.anchorMin = new Vector2(0.5f, 1f);
        rtSec.anchorMax = new Vector2(0.5f, 1f);
        rtSec.anchoredPosition = new Vector2(0, y);
        rtSec.sizeDelta = new Vector2(contentWidth, 30);
        y -= 28f;

        // Subtítulo
        GameObject subLabel = new GameObject($"FeedSub_{sectionTitle}");
        subLabel.transform.SetParent(parent, false);
        TextMeshProUGUI tSub = subLabel.AddComponent<TextMeshProUGUI>();
        tSub.text = sectionSubtitle;
        tSub.fontSize = 16;
        tSub.color = new Color(0.7f, 0.7f, 0.75f);
        tSub.alignment = TextAlignmentOptions.Center;
        tSub.fontStyle = FontStyles.Italic;
        RectTransform rtSub = subLabel.GetComponent<RectTransform>();
        rtSub.anchorMin = new Vector2(0.5f, 1f);
        rtSub.anchorMax = new Vector2(0.5f, 1f);
        rtSub.anchoredPosition = new Vector2(0, y);
        rtSub.sizeDelta = new Vector2(contentWidth, 25);
        y -= 28f;

        // Tarjetas individuales por ingrediente
        for (int i = 0; i < ingredients.Length; i++)
        {
            string ingredient = ingredients[i];
            bool wasSelected = selectedIngredients.Contains(ingredient);

            // Obtener la razón/beneficio del diccionario
            string benefit = "";
            if (s.ingredientBenefits != null && s.ingredientBenefits.ContainsKey(ingredient))
            {
                benefit = s.ingredientBenefits[ingredient];
            }

            float cardHeight = string.IsNullOrEmpty(benefit) ? 35f : 65f;

            // Fondo de tarjeta
            GameObject card = new GameObject($"Card_{ingredient}");
            card.transform.SetParent(parent, false);
            Image cardImg = card.AddComponent<Image>();
            cardImg.color = cardBgColor;
            RectTransform rtCard = card.GetComponent<RectTransform>();
            rtCard.anchorMin = new Vector2(0.5f, 1f);
            rtCard.anchorMax = new Vector2(0.5f, 1f);
            rtCard.anchoredPosition = new Vector2(0, y - cardHeight / 2f);
            rtCard.sizeDelta = new Vector2(contentWidth - 20f, cardHeight);

            // Indicador de selección (barra lateral de color)
            GameObject indicator = new GameObject("Indicator");
            indicator.transform.SetParent(card.transform, false);
            Image indImg = indicator.AddComponent<Image>();
            indImg.color = wasSelected ? selectedColor : new Color(0.4f, 0.4f, 0.4f, 0.5f);
            RectTransform rtInd = indicator.GetComponent<RectTransform>();
            rtInd.anchorMin = new Vector2(0, 0);
            rtInd.anchorMax = new Vector2(0, 1);
            rtInd.anchoredPosition = new Vector2(3, 0);
            rtInd.sizeDelta = new Vector2(6, 0);

            // Nombre del ingrediente con marca
            GameObject nameObj = new GameObject("IngName");
            nameObj.transform.SetParent(card.transform, false);
            TextMeshProUGUI tName = nameObj.AddComponent<TextMeshProUGUI>();
            string mark = wasSelected ? "✓" : "✗";
            string nameColorHex = ColorUtility.ToHtmlStringRGB(wasSelected ? selectedColor : missingColor);
            tName.text = $"<color=#{nameColorHex}>{mark}</color>  <b>{ingredient.ToUpper()}</b>";
            tName.fontSize = 18;
            tName.color = wasSelected ? Color.white : new Color(0.65f, 0.65f, 0.65f);
            tName.alignment = TextAlignmentOptions.MidlineLeft;
            RectTransform rtName = nameObj.GetComponent<RectTransform>();
            rtName.anchorMin = new Vector2(0, 0.5f);
            rtName.anchorMax = new Vector2(0.45f, 1f);
            rtName.offsetMin = new Vector2(18, 2);
            rtName.offsetMax = new Vector2(0, -2);

            // Estado (Seleccionado / No seleccionado)
            GameObject statusObj = new GameObject("IngStatus");
            statusObj.transform.SetParent(card.transform, false);
            TextMeshProUGUI tStatus = statusObj.AddComponent<TextMeshProUGUI>();
            tStatus.text = wasSelected ? "SELECCIONADO" : "NO SELECCIONADO";
            tStatus.fontSize = 12;
            tStatus.color = wasSelected ? new Color(0.5f, 1f, 0.5f, 0.8f) : new Color(0.8f, 0.5f, 0.4f, 0.7f);
            tStatus.alignment = TextAlignmentOptions.MidlineRight;
            tStatus.fontStyle = FontStyles.Bold;
            RectTransform rtStatus = statusObj.GetComponent<RectTransform>();
            rtStatus.anchorMin = new Vector2(0.55f, 0.5f);
            rtStatus.anchorMax = new Vector2(1f, 1f);
            rtStatus.offsetMin = new Vector2(0, 2);
            rtStatus.offsetMax = new Vector2(-12, -2);

            // Razón / Beneficio (debajo del nombre)
            if (!string.IsNullOrEmpty(benefit))
            {
                GameObject reasonObj = new GameObject("IngReason");
                reasonObj.transform.SetParent(card.transform, false);
                TextMeshProUGUI tReason = reasonObj.AddComponent<TextMeshProUGUI>();
                tReason.text = benefit;
                tReason.fontSize = 14;
                tReason.color = new Color(0.85f, 0.85f, 0.75f, 0.9f);
                tReason.alignment = TextAlignmentOptions.MidlineLeft;
                tReason.enableWordWrapping = true;
                tReason.fontStyle = FontStyles.Italic;
                RectTransform rtReason = reasonObj.GetComponent<RectTransform>();
                rtReason.anchorMin = new Vector2(0, 0);
                rtReason.anchorMax = new Vector2(1, 0.5f);
                rtReason.offsetMin = new Vector2(18, 2);
                rtReason.offsetMax = new Vector2(-12, -2);
            }

            y -= (cardHeight + 5f);
        }

        return y;
    }

    /// <summary>
    /// Crea un botón para la pantalla de feedback con anchors en la parte inferior.
    /// </summary>
    private void CreateFeedbackButton(Transform parent, string label, Vector2 offset, Color bgColor, System.Action onClick)
    {
        GameObject btnObj = new GameObject($"FeedBtn_{label}");
        btnObj.transform.SetParent(parent, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = bgColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() => onClick());

        RectTransform rtBtn = btnObj.GetComponent<RectTransform>();
        rtBtn.anchorMin = new Vector2(0.5f, 0);
        rtBtn.anchorMax = new Vector2(0.5f, 0);
        rtBtn.anchoredPosition = offset;
        rtBtn.sizeDelta = new Vector2(300, 60);

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI tmpTxt = txtObj.AddComponent<TextMeshProUGUI>();
        tmpTxt.text = label;
        tmpTxt.fontSize = 22;
        tmpTxt.fontStyle = FontStyles.Bold;
        tmpTxt.color = Color.white;
        tmpTxt.alignment = TextAlignmentOptions.Center;
        RectTransform rtTxt = txtObj.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = Vector2.zero;
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
