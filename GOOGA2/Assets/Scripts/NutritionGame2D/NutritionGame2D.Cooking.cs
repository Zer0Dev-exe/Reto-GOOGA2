using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NutritionGame2D - Pantalla intermedia de coccion
/// </summary>
public partial class NutritionGame2D
{
    private void ShowCooking()
    {
        ClearScene();
        currentPhase = GamePhase.Cooking;
        cookingProgress = 0f;
        cookingComplete = false;

        if (titleHudObj) titleHudObj.SetActive(true);
        if (scoreHudObj) scoreHudObj.SetActive(false);
        if (instructionsHudObj) instructionsHudObj.SetActive(true);

        titleText.text = "<color=#FFDD88>COCINANDO...</color>";
        titleText.fontSize = 60;
        instructionsText.text = "PASO 1: MANTEN ESPACIO O CLICK PARA LLENAR LA BARRA\nPASO 2: AL 100% PASA AL RESUMEN";
        instructionsText.color = new Color(0.95f, 0.95f, 0.95f);
        instructionsText.fontSize = 22;
        RectTransform instrRt = instructionsText.GetComponent<RectTransform>();
        instrRt.anchoredPosition = Vector2.zero;
        SetInstructionsPanelAlpha(0.55f);

        // Fondo simple de cocina
        GameObject bg = new GameObject("Cooking_BG");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
        bgSr.sprite = GenerateSimpleBackground(new Color(0.12f, 0.1f, 0.1f));
        bgSr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);

        // Sarten: Mejorada para cubrir el area de alimentos
        GameObject pan = new GameObject("Cooking_Pan");
        pan.transform.SetParent(gameContainer.transform);
        pan.transform.position = new Vector3(0, -0.2f, 0); // Ajustado para centrar el cuenco tras los iconos
        SpriteRenderer panSr = pan.AddComponent<SpriteRenderer>();
        panSr.sprite = CreatePanSprite();
        panSr.sortingOrder = 5;
        pan.transform.localScale = new Vector3(0.75f, 0.75f, 1f); // Proporción más equilibrada

        // Ingredientes sobre la sarten
        for (int i = 0; i < selectedIngredients.Count; i++)
        {
            float ang = (selectedIngredients.Count > 0) ? (i / (float)selectedIngredients.Count) * Mathf.PI * 2f : 0f;
            Vector3 offset = new Vector3(Mathf.Cos(ang) * 0.8f, Mathf.Sin(ang) * 0.3f, -0.1f);

            GameObject ing = new GameObject($"Cook_{selectedIngredients[i]}");
            ing.transform.SetParent(gameContainer.transform);
            ing.transform.position = pan.transform.position + offset;

            SpriteRenderer sr = ing.AddComponent<SpriteRenderer>();
            Color c = GetIngredientColor(selectedIngredients[i]);
            sr.sprite = CreateIngredientSprite(selectedIngredients[i], c);
            sr.sortingOrder = 6;

            if (sr.sprite != null)
            {
                float maxSize = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
                if (maxSize > 0)
                {
                    float targetSize = 0.8f;
                    float scaleFactor = targetSize / maxSize;
                    ing.transform.localScale = Vector3.one * scaleFactor;
                }
            }
        }

        // Humo
        for (int i = 0; i < 12; i++)
        {
            GameObject smoke = new GameObject("Smoke");
            smoke.transform.SetParent(gameContainer.transform);
            smoke.transform.position = pan.transform.position + new Vector3(Random.Range(-0.6f, 0.6f), 0.4f, -0.2f);

            SpriteRenderer sr = smoke.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSmokeSprite();
            sr.color = new Color(1f, 1f, 1f, 0.35f);
            sr.sortingOrder = 7;
            smoke.transform.localScale = Vector3.one * Random.Range(0.5f, 0.9f);

            ParticleController pc = smoke.AddComponent<ParticleController>();
            pc.velocity = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(0.6f, 1.2f));
            pc.lifetime = Random.Range(0.8f, 1.6f);
        }

        // UI: contenedor de ingredientes y barra de progreso
        GameObject cont = new GameObject("CookingContainer");
        cont.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtCont = cont.AddComponent<RectTransform>();
        rtCont.anchorMin = Vector2.zero;
        rtCont.anchorMax = Vector2.one;
        rtCont.sizeDelta = Vector2.zero;

        GameObject label = new GameObject("CookingLabel");
        label.transform.SetParent(cont.transform, false);
        TextMeshProUGUI labelTxt = label.AddComponent<TextMeshProUGUI>();
        labelTxt.text = "INGREDIENTES EN COCCION";
        labelTxt.fontSize = 24;
        labelTxt.alignment = TextAlignmentOptions.Center;
        labelTxt.color = new Color(1f, 0.95f, 0.8f);
        RectTransform rtLabel = labelTxt.GetComponent<RectTransform>();
        rtLabel.anchorMin = new Vector2(0.5f, 1f);
        rtLabel.anchorMax = new Vector2(0.5f, 1f);
        rtLabel.anchoredPosition = new Vector2(0, -140); // Subido
        rtLabel.sizeDelta = new Vector2(600, 40);

        int cols = 5;
        float spacingX = 140f;
        float spacingY = 130f;
        int rows = Mathf.CeilToInt(selectedIngredients.Count / (float)cols);
        float startX = -((cols - 1) * spacingX) * 0.5f;
        float startY = -180f; // Centrado en la sarten

        for (int i = 0; i < selectedIngredients.Count; i++)
        {
            int r = i / cols;
            int c = i % cols;

            GameObject iconObj = new GameObject($"CookIcon_{i}");
            iconObj.transform.SetParent(cont.transform, false);
            Image icon = iconObj.AddComponent<Image>();
            icon.sprite = CreateIngredientSprite(selectedIngredients[i], GetIngredientColor(selectedIngredients[i]));
            icon.preserveAspect = true;

            RectTransform rtIcon = iconObj.GetComponent<RectTransform>();
            rtIcon.anchorMin = new Vector2(0.5f, 1f);
            rtIcon.anchorMax = new Vector2(0.5f, 1f);
            rtIcon.anchoredPosition = new Vector2(startX + c * spacingX, startY - r * spacingY);
            rtIcon.sizeDelta = new Vector2(80, 80);

            GameObject nameObj = new GameObject($"CookName_{i}");
            nameObj.transform.SetParent(cont.transform, false);
            TextMeshProUGUI nameTxt = nameObj.AddComponent<TextMeshProUGUI>();
            nameTxt.text = selectedIngredients[i].ToUpper();
            nameTxt.fontSize = 16;
            nameTxt.alignment = TextAlignmentOptions.Center;
            nameTxt.color = Color.white;

            RectTransform rtName = nameObj.GetComponent<RectTransform>();
            rtName.anchorMin = new Vector2(0.5f, 1f);
            rtName.anchorMax = new Vector2(0.5f, 1f);
            rtName.anchoredPosition = new Vector2(startX + c * spacingX, startY - r * spacingY - 55f);
            rtName.sizeDelta = new Vector2(120, 30);
        }

        // Barra de progreso (visual)
        GameObject barBg = new GameObject("CookBar_BG");
        barBg.transform.SetParent(cont.transform, false);
        Image barBgImg = barBg.AddComponent<Image>();
        barBgImg.sprite = GenerateSimpleBackground(Color.white);
        barBgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        RectTransform rtBarBg = barBg.GetComponent<RectTransform>();
        rtBarBg.anchorMin = new Vector2(0.5f, 0f);
        rtBarBg.anchorMax = new Vector2(0.5f, 0f);
        rtBarBg.anchoredPosition = new Vector2(0, 180); // Cerca de la sarten
        rtBarBg.sizeDelta = new Vector2(620, 28);

        GameObject barFill = new GameObject("CookBar_Fill");
        barFill.transform.SetParent(barBg.transform, false);
        Image barFillImg = barFill.AddComponent<Image>();
        barFillImg.sprite = GenerateSimpleBackground(Color.white);
        barFillImg.color = new Color(0.9f, 0.6f, 0.2f, 1f);
        barFillImg.type = Image.Type.Filled;
        barFillImg.fillMethod = Image.FillMethod.Horizontal;
        barFillImg.fillOrigin = 0;
        barFillImg.fillAmount = 0.0f;
        RectTransform rtBarFill = barFill.GetComponent<RectTransform>();
        rtBarFill.anchorMin = Vector2.zero;
        rtBarFill.anchorMax = Vector2.one;
        rtBarFill.sizeDelta = Vector2.zero;
        cookingBarFill = barFillImg;

        GameObject pctObj = new GameObject("CookBar_Pct");
        pctObj.transform.SetParent(barBg.transform, false);
        TextMeshProUGUI pctTxt = pctObj.AddComponent<TextMeshProUGUI>();
        pctTxt.text = "0%";
        pctTxt.fontSize = 18;
        pctTxt.alignment = TextAlignmentOptions.Center;
        pctTxt.color = new Color(1f, 1f, 1f, 0.9f);
        RectTransform rtPct = pctObj.GetComponent<RectTransform>();
        rtPct.anchorMin = Vector2.zero;
        rtPct.anchorMax = Vector2.one;
        rtPct.sizeDelta = Vector2.zero;
        cookingPercentText = pctTxt;

        // Texto de estado (Restaurado)
        GameObject statusObj = new GameObject("CookStatusText");
        statusObj.transform.SetParent(cont.transform, false);
        TextMeshProUGUI statusTxt = statusObj.AddComponent<TextMeshProUGUI>();
        statusTxt.text = "Mantén presionado para cocinar";
        statusTxt.fontSize = 28;
        statusTxt.fontStyle = FontStyles.Bold;
        statusTxt.alignment = TextAlignmentOptions.Center;
        statusTxt.color = new Color(1f, 0.9f, 0.4f);
        RectTransform rtStatus = statusObj.GetComponent<RectTransform>();
        rtStatus.anchorMin = new Vector2(0.5f, 0f);
        rtStatus.anchorMax = new Vector2(0.5f, 0f);
        rtStatus.anchoredPosition = new Vector2(0, 240); // Sobre la barra
        rtStatus.sizeDelta = new Vector2(800, 50);
        cookingStatusText = statusTxt;

    }
}
