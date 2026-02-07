using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NutritionGame2D - Configuración del HUD (Canvas overlay)
/// </summary>
public partial class NutritionGame2D
{
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
        titleHudObj = new GameObject("TitleHud");
        titleHudObj.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtTitleObj = titleHudObj.AddComponent<RectTransform>();
        rtTitleObj.anchorMin = new Vector2(0, 1);
        rtTitleObj.anchorMax = new Vector2(1, 1);
        rtTitleObj.pivot = new Vector2(0.5f, 1);
        rtTitleObj.anchoredPosition = new Vector2(0, 0);
        rtTitleObj.sizeDelta = new Vector2(0, 120);

        // Fondo del título
        GameObject titleBg = new GameObject("TitleBG");
        titleBg.transform.SetParent(titleHudObj.transform, false);
        Image bgImg = titleBg.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.6f);
        RectTransform rtBg = titleBg.GetComponent<RectTransform>();
        rtBg.anchorMin = Vector2.zero;
        rtBg.anchorMax = Vector2.one;
        rtBg.sizeDelta = Vector2.zero;
        
        // Texto del título
        GameObject txtObj = new GameObject("TitleText");
        txtObj.transform.SetParent(titleHudObj.transform, false);
        titleText = txtObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.95f, 0.6f);
        titleText.fontStyle = FontStyles.Bold;
        
        RectTransform trt = titleText.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one; 
        trt.sizeDelta = Vector2.zero;
        trt.offsetMin = new Vector2(0, 10);
        trt.offsetMax = new Vector2(0, -10);

        // INSTRUCCIONES (Panel inferior)
        instructionsHudObj = new GameObject("InstructionsHud");
        instructionsHudObj.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtInstrObj = instructionsHudObj.AddComponent<RectTransform>();
        rtInstrObj.anchorMin = Vector2.zero;
        rtInstrObj.anchorMax = Vector2.one;
        rtInstrObj.sizeDelta = Vector2.zero;
        
        GameObject instrBg = new GameObject("InstrBG");
        instrBg.transform.SetParent(instructionsHudObj.transform, false);
        Image instrBgImg = instrBg.AddComponent<Image>();
        instrBgImg.color = new Color(0f, 0f, 0f, 0.55f);
        RectTransform instrBgRt = instrBg.GetComponent<RectTransform>();
        instrBgRt.anchorMin = new Vector2(0.5f, 0f);
        instrBgRt.anchorMax = new Vector2(0.5f, 0f);
        instrBgRt.anchoredPosition = new Vector2(0, 70);
        instrBgRt.sizeDelta = new Vector2(900, 90);

        GameObject txtInstrObj = new GameObject("InstrText");
        txtInstrObj.transform.SetParent(instrBg.transform, false);
        instructionsText = txtInstrObj.AddComponent<TextMeshProUGUI>();
        instructionsText.fontSize = 28;
        instructionsText.alignment = TextAlignmentOptions.Center;
        instructionsText.color = Color.white;
        instructionsText.fontStyle = FontStyles.Bold;
        instructionsText.enableWordWrapping = true;
        
        instructionsText.outlineWidth = 0.2f;
        instructionsText.outlineColor = new Color(0, 0, 0, 1f);
        
        RectTransform instrRt = instructionsText.GetComponent<RectTransform>();
        instrRt.anchorMin = Vector2.zero;
        instrRt.anchorMax = Vector2.one;
        instrRt.offsetMin = new Vector2(20, 10);
        instrRt.offsetMax = new Vector2(-20, -10);

        // PUNTUACIÓN / CESTA
        scoreHudObj = new GameObject("ScoreHud");
        scoreHudObj.transform.SetParent(hudCanvas.transform, false);
        RectTransform rtScoreObj = scoreHudObj.AddComponent<RectTransform>();
        rtScoreObj.anchorMin = new Vector2(1, 1);
        rtScoreObj.anchorMax = new Vector2(1, 1);
        rtScoreObj.anchoredPosition = new Vector2(-150, -50);
        rtScoreObj.sizeDelta = new Vector2(300, 60);
        
        GameObject scoreBg = new GameObject("ScoreBG");
        scoreBg.transform.SetParent(scoreHudObj.transform, false);
        Image sBg = scoreBg.AddComponent<Image>();
        sBg.color = new Color(0, 0, 0, 0.7f);
        RectTransform rtSBg = scoreBg.GetComponent<RectTransform>();
        rtSBg.anchorMin = Vector2.zero;
        rtSBg.anchorMax = Vector2.one;
        rtSBg.sizeDelta = Vector2.zero;

        GameObject txtScoreObj = new GameObject("ScoreText");
        txtScoreObj.transform.SetParent(scoreHudObj.transform, false);
        scoreText = txtScoreObj.AddComponent<TextMeshProUGUI>();
        scoreText.fontSize = 32;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.color = new Color(0.5f, 1f, 0.5f);
        
        RectTransform srt = scoreText.GetComponent<RectTransform>();
        srt.anchorMin = Vector2.zero;
        srt.anchorMax = Vector2.one;
        srt.sizeDelta = Vector2.zero;
    }
}
