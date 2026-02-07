using UnityEngine;
using TMPro;

/// <summary>
/// NutritionGame2D - Fase de Intro
/// </summary>
public partial class NutritionGame2D
{
    private void ShowIntro()
    {
        ClearScene();
        currentPhase = GamePhase.Intro;
        
        if (titleHudObj) titleHudObj.SetActive(true);
        if (scoreHudObj) scoreHudObj.SetActive(false);
        if (instructionsHudObj) instructionsHudObj.SetActive(true);

        GameObject bgLayer = new GameObject("Intro_BG");
        bgLayer.transform.SetParent(gameContainer.transform);
        bgLayer.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bgLayer.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/intro_bg.png");
        if (s != null) sr.sprite = s;
        else CreateStarryBackground();

        ScaleToFillScreen(bgLayer, 1.0f);

        titleText.text = "";
        if (titleHudObj) titleHudObj.SetActive(false);
        
        instructionsText.text = "PRESIONA <size=50><B><color=#FFD700>ENTER</color></B></size> PARA EMPEZAR";
        instructionsText.color = Color.white;
    }

    private void CreateStarryBackground()
    {
        GameObject bg = new GameObject("BG_Sky");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/space_bg.png");
        if (s != null)
        {
            sr.sprite = s;
        }
        else
        {
            int w = 256; int h = 256;
            Texture2D tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            Color topColor = new Color(0.02f, 0.05f, 0.15f);
            Color botColor = new Color(0.05f, 0.02f, 0.05f);
            for (int y = 0; y < h; y++)
            {
                Color rowColor = Color.Lerp(botColor, topColor, (float)y / h);
                for (int x = 0; x < w; x++)
                {
                    Color c = rowColor;
                    if (Random.value > 0.997f) c = Color.white;
                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 10f);
        }
        
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }
}
