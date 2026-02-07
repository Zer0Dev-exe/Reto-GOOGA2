using UnityEngine;
using TMPro;

/// <summary>
/// NutritionGame2D - Fase de Menú (selección de escenario)
/// </summary>
public partial class NutritionGame2D
{
    private void ShowMenu()
    {
        ClearScene();
        
        // Seguridad extra: limpiar restos del HUD de resultados
        if (hudCanvas != null)
        {
            Transform resCont = hudCanvas.transform.Find("ResultsContainer");
            if (resCont) Destroy(resCont.gameObject);
        }

        currentPhase = GamePhase.Menu;
        
        if (titleHudObj) titleHudObj.SetActive(true);
        if (scoreHudObj) scoreHudObj.SetActive(false);
        if (instructionsHudObj) instructionsHudObj.SetActive(true);

        CreateSelectionBackground();

        CreateCharacterSelectionButton(1, "Characters/adolescencia.png", scenarios[1].name.ToUpper(), new Vector3(-6, 0, 0), 7.0f, new Color(0.4f, 1f, 0.4f));
        CreateCharacterSelectionButton(0, "Characters/embarazo.png", scenarios[0].name.ToUpper(), new Vector3(0, 0, 0), 7.2f, new Color(0.4f, 0.8f, 1f));
        CreateCharacterSelectionButton(2, "Characters/senectud.png", scenarios[2].name.ToUpper(), new Vector3(6, 0, 0), 7.2f, new Color(1f, 0.85f, 0.4f));
    }

    private void CreateSelectionBackground()
    {
        GameObject bg = new GameObject("SelectionBG");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        int w = 2, h = 64;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Bilinear;
        Color topC = new Color(0.05f, 0.05f, 0.1f);
        Color botC = new Color(0.12f, 0.08f, 0.15f);
        for (int y = 0; y < h; y++)
        {
            Color t = Color.Lerp(botC, topC, (float)y / h);
            tex.SetPixel(0, y, t); tex.SetPixel(1, y, t);
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 10f);
        ScaleToFillScreen(bg, 1.0f, true);
        sr.sortingOrder = -10;

        // Suelo estilizado
        GameObject floor = new GameObject("SelectionFloor");
        floor.transform.SetParent(gameContainer.transform);
        floor.transform.position = new Vector3(0, -7, 9);
        SpriteRenderer srFloor = floor.AddComponent<SpriteRenderer>();
        srFloor.sprite = CreateBoxSprite(20, 20, new Color(0.03f, 0.03f, 0.05f, 1f), false);
        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        floor.transform.localScale = new Vector3(screenWidth * 2, 8, 1);
        srFloor.sortingOrder = -9;
    }

    private Sprite CreateSelectionLightSprite()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2, size / 2));
                float alpha = Mathf.Clamp01(1.0f - (dist / (size / 2)));
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
        if (s != null)
        {
            s.texture.filterMode = FilterMode.Point;
            sr.sprite = s;
            float h = s.bounds.size.y;
            float scale = targetH / h;
            charObj.transform.localScale = new Vector3(scale, scale, 1);
        }
        sr.sortingOrder = 5;

        // Base circular (Plataforma de luz)
        GameObject pedestal = new GameObject("Pedestal");
        pedestal.transform.SetParent(charObj.transform, false);
        float spriteBottomOffset = (sr.sprite != null) ? sr.sprite.bounds.min.y * charObj.transform.localScale.y : -targetH / 2;
        pedestal.transform.position = charObj.transform.position + new Vector3(0, spriteBottomOffset, 0.5f);
        pedestal.transform.localScale = new Vector3(4.0f, 1.2f, 1);
        SpriteRenderer psr = pedestal.AddComponent<SpriteRenderer>();
        psr.sprite = CreateSelectionLightSprite();
        psr.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.2f);
        psr.sortingOrder = 4;

        // Borde de color (Glow)
        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(charObj.transform, false);
        glow.transform.localPosition = Vector3.zero;
        SpriteRenderer gsr = glow.AddComponent<SpriteRenderer>();
        gsr.sprite = sr.sprite;
        gsr.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.6f);
        gsr.sortingOrder = 6;
        glow.transform.localScale = Vector3.one * 1.05f;
        glow.SetActive(false);

        // Click logic
        charObj.AddComponent<BoxCollider2D>().size = sr.sprite != null ? sr.sprite.bounds.size : Vector2.one;
        MenuButton mb = charObj.AddComponent<MenuButton>();
        mb.onClick = () => StartScenario(index);
        mb.hoverGlow = glow;

        // UI Label con outline
        GameObject labelBg = new GameObject($"LabelBg_{index}");
        labelBg.transform.SetParent(hudObject.transform, false);
        
        GameObject textObj = new GameObject($"LabelText_{index}");
        textObj.transform.SetParent(labelBg.transform, false);
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 22;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = new Color(1f, 0.9f, 0.7f);
        txt.fontStyle = FontStyles.Bold;
        txt.enableWordWrapping = true;
        
        txt.outlineWidth = 0.3f;
        txt.outlineColor = new Color(0, 0, 0, 1f);
        
        // Sombra suave
        txt.fontSharedMaterial.EnableKeyword("UNDERLAY_ON");
        txt.fontSharedMaterial.SetFloat("_UnderlayOffsetX", 1f);
        txt.fontSharedMaterial.SetFloat("_UnderlayOffsetY", -1f);
        txt.fontSharedMaterial.SetFloat("_UnderlayDilate", 1f);
        txt.fontSharedMaterial.SetFloat("_UnderlaySoftness", 0.5f);
        
        RectTransform rtb = labelBg.AddComponent<RectTransform>();
        rtb.sizeDelta = new Vector2(300, 80);
        
        RectTransform rtText = txt.GetComponent<RectTransform>();
        rtText.sizeDelta = new Vector2(280, 70);
        
        Vector3 screenPos = mainCamera.WorldToScreenPoint(pos + Vector3.down * 4.2f);
        rtb.position = screenPos;
    }
}
