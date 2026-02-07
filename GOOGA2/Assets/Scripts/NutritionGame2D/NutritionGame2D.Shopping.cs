using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// NutritionGame2D - Fase de Compras (mercado)
/// </summary>
public partial class NutritionGame2D
{
    private void ShowShopping()
    {
        ClearScene();
        currentPhase = GamePhase.Shopping;
        
        // MOSTRAR HUD COMPLETO
        if(titleHudObj) titleHudObj.SetActive(true);
        if(scoreHudObj) scoreHudObj.SetActive(true);
        if(instructionsHudObj) instructionsHudObj.SetActive(true);

        CreateShopBackground();
        
        titleText.text = "<color=#FFEAB0>EL MERCADO GOOGAZ</color>";
        titleText.fontSize = 65;
        
        instructionsText.text = "Selecciona los ingredientes más saludables para completar tu misión.";
        instructionsText.color = new Color(0.9f, 0.9f, 0.9f);
        instructionsText.fontSize = 28;
        instructionsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -440);
        SetInstructionsPanelAlpha(0f);

        // BOTON COCINAR (ROJO/NARANJA)
        GameObject btnEnd = new GameObject("Btn_Cocinar");
        btnEnd.transform.SetParent(hudCanvas.transform, false);
        Image imgEnd = btnEnd.AddComponent<Image>();
        imgEnd.color = new Color(0.9f, 0.3f, 0.2f);
        Button btn = btnEnd.AddComponent<Button>();
        btn.onClick.AddListener(()=> ShowCooking());
        RectTransform rtEnd = btnEnd.GetComponent<RectTransform>();
        rtEnd.anchorMin = new Vector2(0.5f, 0);
        rtEnd.anchorMax = new Vector2(0.5f, 0);
        rtEnd.anchoredPosition = new Vector2(0, 80);
        rtEnd.sizeDelta = new Vector2(300, 80);
        
        GameObject txtEnd = new GameObject("Text");
        txtEnd.transform.SetParent(btnEnd.transform, false);
        TextMeshProUGUI tmpEnd = txtEnd.AddComponent<TextMeshProUGUI>();
        tmpEnd.text = "COCINAR";
        tmpEnd.color = Color.white;
        tmpEnd.fontSize = 32;
        tmpEnd.fontStyle = FontStyles.Bold;
        tmpEnd.alignment = TextAlignmentOptions.Center;
        txtEnd.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        txtEnd.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        txtEnd.GetComponent<RectTransform>().anchorMax = Vector2.one;

        // Estilo de Cesta (Score)
        scoreText.text = "CESTA: 0";
        scoreText.color = new Color(1f, 1f, 1f);
        scoreText.fontSize = 40;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.outlineWidth = 0.2f;
        scoreText.outlineColor = Color.black;
        
        RectTransform srt = scoreText.GetComponent<RectTransform>();
        srt.anchorMin = Vector2.zero;
        srt.anchorMax = Vector2.one;
        srt.offsetMin = Vector2.zero;
        srt.offsetMax = Vector2.zero;
        
        scoreHudObj.SetActive(true);
        
        RectTransform scoreContRT = scoreHudObj.GetComponent<RectTransform>();
        scoreContRT.anchorMin = Vector2.zero;
        scoreContRT.anchorMax = Vector2.zero;
        scoreContRT.pivot = new Vector2(0.5f, 0.5f);
        scoreContRT.anchoredPosition = new Vector2(250, 150); 
        scoreContRT.sizeDelta = new Vector2(300, 100);
        
        // ICONO DE BOLSA DE LA COMPRA (Recrear siempre para evitar referencias rotas)
        Transform existingIcon = scoreHudObj.transform.Find("BagIcon");
        if (existingIcon) Destroy(existingIcon.gameObject);

        GameObject iconObj = new GameObject("BagIcon");
        iconObj.transform.SetParent(scoreHudObj.transform, false);
        Image img = iconObj.AddComponent<Image>();
        img.sprite = CreateShoppingBagSprite();
        img.preserveAspect = true;
        RectTransform rtIcon = iconObj.GetComponent<RectTransform>();
        rtIcon.anchorMin = new Vector2(0, 0.5f);
        rtIcon.anchorMax = new Vector2(0, 0.5f);
        rtIcon.pivot = new Vector2(0, 0.5f);
        rtIcon.anchoredPosition = new Vector2(20, 0);
        rtIcon.sizeDelta = new Vector2(80, 80);
        
        iconObj.SetActive(true);

        scoreText.text = "x 0";
        scoreText.alignment = TextAlignmentOptions.Left;
        
        srt.offsetMin = new Vector2(110, 0);
        srt.offsetMax = Vector2.zero;
        
        if(scoreText) scoreText.gameObject.SetActive(true);

        CreateShopkeeper();
        CreateShopShelves();
    }

    private void CreateShopkeeper()
    {
        // El mostrador
        GameObject counter = new GameObject("Counter");
        counter.transform.SetParent(gameContainer.transform);
        counter.transform.position = new Vector3(-6.5f, -2.5f, 2);
        SpriteRenderer csr = counter.AddComponent<SpriteRenderer>();
        csr.sprite = CreateBoxSprite(400, 250, new Color(0.25f, 0.15f, 0.1f), true);
        csr.sortingOrder = 10;
        counter.transform.localScale = new Vector3(1f, 1f, 1f);

        shopkeeper = new GameObject("Shopkeeper");
        shopkeeper.transform.SetParent(gameContainer.transform);
        shopkeeper.transform.position = new Vector3(-6.5f, -1.5f, 1);
        SpriteRenderer sr = shopkeeper.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCocineroOakSprite(); 
        sr.sortingOrder = 5;
        
        if (sr.sprite != null && sr.sprite.texture.width > 64) {
            sr.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            sr.transform.localPosition += new Vector3(0, 0.5f, 0);
        } else {
            sr.transform.localScale = new Vector3(2.0f, 2.0f, 1f);
        }
        
        shopkeeper.AddComponent<ShopkeeperAnimator>();
    }

    private void CreateShopShelves()
    {
        float[] shelfHeights = { 1.8f, 0.1f, -1.5f, -3.2f };
        int cols = 6;
        float startX = -3.5f; 
        
        int ingredientIndex = 0;
        Sprite[] atlasSprites = LoadAndSliceAtlas();
        
        foreach (var ingredientName in availableIngredients)
        {
            int row = ingredientIndex / cols;
            int col = ingredientIndex % cols;
            if (row >= shelfHeights.Length) break;

            Vector3 pos = new Vector3(startX + col * 1.8f, shelfHeights[row], 0);
            
            GameObject item = new GameObject($"Item_{ingredientName}");
            item.transform.SetParent(gameContainer.transform);
            item.transform.position = pos;
            
            Color color = GetIngredientColor(ingredientName);
            SpriteRenderer sr = item.AddComponent<SpriteRenderer>();
            
            if (atlasSprites != null && ingredientIndex < atlasSprites.Length) {
                sr.sprite = atlasSprites[ingredientIndex];
            } else {
                sr.sprite = CreateIngredientSprite(ingredientName, color);
            }
            sr.sortingOrder = 5;

            if (sr.sprite.name != "DefaultCircle")
            {
                float maxSize = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
                if (maxSize > 0)
                {
                    float targetSize = 1.4f;
                    float scaleFactor = targetSize / maxSize;
                    item.transform.localScale = Vector3.one * scaleFactor;
                }
            }
            else
            {
                item.transform.localScale = Vector3.one * 1.2f;
            }

            // Etiqueta de precio/nombre MEJORADA
            GameObject label = new GameObject("PriceLabel");
            label.transform.SetParent(item.transform, false);
            label.transform.localPosition = new Vector3(0, -0.7f, -0.1f);
            
            SpriteRenderer labelBg = label.AddComponent<SpriteRenderer>();
            labelBg.sprite = CreateBoxSprite(140, 40, new Color(0.1f, 0.1f, 0.1f, 0.9f), true);
            
            float pScale = item.transform.localScale.x;
            if (pScale == 0) pScale = 1;
            label.transform.localScale = new Vector3(0.015f / pScale, 0.015f / pScale, 1);
            
            GameObject txtObj = new GameObject("Txt");
            txtObj.transform.SetParent(label.transform, false);
            txtObj.transform.localPosition = new Vector3(0, 0, -0.1f);
            TextMeshPro labelTxt = txtObj.AddComponent<TextMeshPro>();
            labelTxt.text = ingredientName.ToUpper();
            labelTxt.fontSize = 5;
            labelTxt.alignment = TextAlignmentOptions.Center;
            labelTxt.color = Color.white;
            labelTxt.GetComponent<MeshRenderer>().sortingOrder = 20; 
            
            labelTxt.rectTransform.sizeDelta = new Vector2(14, 4);

            item.AddComponent<BoxCollider2D>();
            IngredientItem ii = item.AddComponent<IngredientItem>();
            ii.ingredientName = ingredientName;
            ii.color = color;
            ii.onSelect = (ing) => {
                if (!selectedIngredients.Contains(ing.ingredientName)) {
                    selectedIngredients.Add(ing.ingredientName);
                    scoreText.text = $"x {selectedIngredients.Count}";
                    CreateParticles(pos, ing.color);
                    ing.transform.localScale *= 1.2f;
                }
            };

            ingredientIndex++;
        }
    }

    private void CreateShopBackground()
    {
        GameObject bg = new GameObject("BG_Market_Full");
        bg.transform.SetParent(gameContainer.transform);
        bg.transform.position = new Vector3(0, 0, 10);
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        Sprite s = LoadLocalSprite("Backgrounds/market_bg.png");
        if (s != null) {
            sr.sprite = s;
        } else {
            sr.sprite = GenerateSimpleBackground(new Color(0.8f, 0.7f, 0.6f));
        }
        
        sr.sortingOrder = -10;
        ScaleToFillScreen(bg, 1.0f);
    }
}
