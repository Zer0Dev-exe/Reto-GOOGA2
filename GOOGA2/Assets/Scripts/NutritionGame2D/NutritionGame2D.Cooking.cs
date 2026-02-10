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

        // --- TRANSICIÓN A 3D REAL ---
        Setup3DCamera();
        Setup3DLighting();

        // Fondo (estrellas o fogón)
        GameObject bg3d = GameObject.CreatePrimitive(PrimitiveType.Plane);
        bg3d.name = "Kitchen_Floor";
        bg3d.transform.SetParent(gameContainer.transform);
        bg3d.transform.position = new Vector3(0, -0.1f, 0);
        bg3d.transform.localScale = new Vector3(10, 1, 10);
        Material bgMat = CreateURPMaterial(new Color(0.1f, 0.08f, 0.08f), 0.2f);
        bg3d.GetComponent<Renderer>().material = bgMat;

        // Sartén 3D
        Create3DPan();

        // Ingredientes 3D
        Create3DIngredients();

        // Humo (Uso partículas 2D pero en el espacio 3D para estilo híbrido)
        for (int i = 0; i < 15; i++)
        {
            GameObject smoke = new GameObject("Smoke3D");
            smoke.transform.SetParent(gameContainer.transform);
            smoke.transform.position = new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));

            SpriteRenderer sr = smoke.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSmokeSprite();
            sr.color = new Color(1f, 1f, 1f, 0.25f);
            sr.sortingOrder = 50;
            smoke.transform.localScale = Vector3.one * Random.Range(1f, 2.5f);
            smoke.transform.rotation = Quaternion.Euler(90, 0, 0); // Mirando a cámara

            ParticleController pc = smoke.AddComponent<ParticleController>();
            pc.velocity = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(1.5f, 3.0f));
            pc.lifetime = Random.Range(1f, 2.5f);
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
        float spacingX = 130f;
        float spacingY = 120f;
        int rows = Mathf.CeilToInt(selectedIngredients.Count / (float)cols);
        float startX = -((cols - 1) * spacingX) * 0.5f;
        float startY = -150f; // Ajustado para que la cuadrícula quede dentro del cuenco

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
            rtIcon.sizeDelta = new Vector2(70, 70); // Un poco más compactos

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
