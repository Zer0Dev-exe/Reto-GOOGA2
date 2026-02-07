using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NutritionGame2D - Fase de Aprendizaje (nota de misión)
/// </summary>
public partial class NutritionGame2D
{
    private void ShowLearning()
    {
        ClearScene();
        currentPhase = GamePhase.Learning;
        
        if (titleHudObj) titleHudObj.SetActive(false);
        if (scoreHudObj) scoreHudObj.SetActive(false);
        if (instructionsHudObj) instructionsHudObj.SetActive(true);

        CreateChalkboardBackground();
        CreateLearningNote(scenarios[currentScenarioIndex]);
        
        instructionsText.text = "Pulsa ENTER para comenzar la compra";
    }

    private void CreateChalkboardBackground()
    {
        GameObject bg = new GameObject("BG_Chalk");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/learning_bg.png");
        if (s != null)
        {
            sr.sprite = s;
        }
        else
        {
            int w = 128; int h = 128;
            Texture2D tex = new Texture2D(w, h);
            Color chalkBase = new Color(0.12f, 0.2f, 0.15f);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
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

    private void CreateLearningNote(Scenario scenario)
    {
        // 1. Overlay oscuro
        GameObject overlay = new GameObject("UI_Overlay");
        overlay.transform.SetParent(hudCanvas.transform, false);
        overlay.transform.SetSiblingIndex(0);
        
        Image imgOverlay = overlay.AddComponent<Image>();
        imgOverlay.color = new Color(0, 0, 0, 0.85f);
        RectTransform rtOverlay = overlay.GetComponent<RectTransform>();
        rtOverlay.anchorMin = Vector2.zero;
        rtOverlay.anchorMax = Vector2.one;
        rtOverlay.sizeDelta = Vector2.zero;

        // 2. Hoja de Papel
        GameObject noteObj = new GameObject("UI_MissionPaper");
        noteObj.transform.SetParent(hudCanvas.transform, false);
        noteObj.transform.SetSiblingIndex(1);
        
        Image imgPaper = noteObj.AddComponent<Image>();
        imgPaper.sprite = CreateBoxSprite(800, 900, new Color(0.96f, 0.96f, 0.94f), true);
        
        RectTransform rtPaper = noteObj.GetComponent<RectTransform>();
        rtPaper.anchoredPosition = new Vector2(350, 20);
        rtPaper.sizeDelta = new Vector2(700, 850);

        // 2.5 Cocinero Oak (UI Image)
        GameObject oakObj = new GameObject("UI_Boss");
        oakObj.transform.SetParent(hudCanvas.transform, false);
        oakObj.transform.SetSiblingIndex(2);
        
        Image imgOak = oakObj.AddComponent<Image>();
        imgOak.sprite = CreateCocineroOakSprite();
        imgOak.preserveAspect = true;
        
        RectTransform rtOak = oakObj.GetComponent<RectTransform>();
        rtOak.anchorMin = new Vector2(0.5f, 0.5f);
        rtOak.anchorMax = new Vector2(0.5f, 0.5f);
        rtOak.anchoredPosition = new Vector2(-450, -50);
        rtOak.sizeDelta = new Vector2(500, 800);

        // 3. Clip metálico
        GameObject clip = new GameObject("UI_MetalClip");
        clip.transform.SetParent(noteObj.transform, false);
        Image imgClip = clip.AddComponent<Image>();
        imgClip.sprite = CreateBoxSprite(300, 60, new Color(0.3f, 0.35f, 0.4f), true);
        
        RectTransform rtClip = clip.GetComponent<RectTransform>();
        rtClip.anchoredPosition = new Vector2(0, 430);
        rtClip.sizeDelta = new Vector2(300, 60);

        // 4. Contenido de texto
        GameObject noteTextObj = new GameObject("MissionContent");
        noteTextObj.transform.SetParent(noteObj.transform, false);
        TextMeshProUGUI txt = noteTextObj.AddComponent<TextMeshProUGUI>();
        txt.alignment = TextAlignmentOptions.TopLeft;
        txt.enableWordWrapping = true;
        
        string content = "";
        content += $"<align=center><size=32><B><color=#2C3E50>CONFIDENCIAL</color></B></size></align>\n";
        content += $"<align=center><size=14><color=#7F8C8D>• MINISTERIO DE NUTRICIÓN GOOGAZ •</color></size></align>\n";
        content += $"<align=center><size=24><color=#BDC3C7>_______________________________________</color></size></align>\n\n";
        content += $"<size=18><color=#95A5A6>SUJETO DE ESTUDIO:</color></size>\n";
        content += $"<size=28><B><color=#E67E22>{scenario.name.ToUpper()}</color></B></size>\n\n";
        content += $"<size=18><color=#95A5A6>OBJETIVO CLÍNICO:</color></size>\n";
        content += $"<size=22><color=#34495E>{scenario.description}</color></size>\n\n";
        content += $"<align=center><size=24><color=#BDC3C7>_______________________________________</color></size></align>\n\n";
        content += $"<size=22><B><color=#27AE60>PROTOCOLOS NUTRICIONALES:</color></B></size>\n";
        content += $"<color=#2C3E50><line-height=130%>";
        foreach (var item in scenario.requiredIngredients)
        {
            content += $"  <b>[ ]</b> <size=26>{item.ToUpper()}</size>\n";
        }
        content += $"</color>";

        txt.text = content;
        
        RectTransform rtTxt = txt.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero; rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = new Vector2(-80, -100);
        rtTxt.anchoredPosition = new Vector2(0, -20);
        
        // 5. Sello "PRIORIDAD ALTA"
        GameObject stampObj = new GameObject("UI_Stamp");
        stampObj.transform.SetParent(noteObj.transform, false);
        
        Image stampImg = stampObj.AddComponent<Image>();
        stampImg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
        RectTransform rtStamp = stampObj.GetComponent<RectTransform>();
        rtStamp.sizeDelta = new Vector2(280, 70);
        rtStamp.anchoredPosition = new Vector2(180, -350);
        rtStamp.localRotation = Quaternion.Euler(0, 0, 15);
        
        GameObject stampTxtObj = new GameObject("StampText");
        stampTxtObj.transform.SetParent(stampObj.transform, false);
        TextMeshProUGUI stampTxt = stampTxtObj.AddComponent<TextMeshProUGUI>();
        stampTxt.text = "PRIORIDAD ALTA";
        stampTxt.fontSize = 28;
        stampTxt.fontStyle = FontStyles.Bold;
        stampTxt.alignment = TextAlignmentOptions.Center;
        stampTxt.color = Color.white;
        stampTxtObj.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 70);
        
        // 6. Botón ACEPTAR
        if (instructionsHudObj) instructionsHudObj.SetActive(false);

        GameObject btnObj = new GameObject("Btn_Aceptar");
        btnObj.transform.SetParent(hudCanvas.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.8f, 0.2f);
        
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() => ShowShopping());

        RectTransform rtBtn = btnObj.GetComponent<RectTransform>();
        rtBtn.anchorMin = new Vector2(0.5f, 0);
        rtBtn.anchorMax = new Vector2(0.5f, 0);
        rtBtn.anchoredPosition = new Vector2(0, 80);
        rtBtn.sizeDelta = new Vector2(300, 80);

        GameObject txtBtn = new GameObject("Text");
        txtBtn.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI tmpBtn = txtBtn.AddComponent<TextMeshProUGUI>();
        tmpBtn.text = "ACEPTAR";
        tmpBtn.color = Color.white;
        tmpBtn.fontSize = 36;
        tmpBtn.fontStyle = FontStyles.Bold;
        tmpBtn.alignment = TextAlignmentOptions.Center;
        txtBtn.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        txtBtn.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        txtBtn.GetComponent<RectTransform>().anchorMax = Vector2.one;
        
        Transform instrBg = hudCanvas.transform.Find("InstrBG");
        if (instrBg != null)
        {
            instrBg.SetAsLastSibling();
            instrBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -480);
        }
    }
}
