using UnityEngine;
using TMPro;

/// <summary>
/// NutritionGame2D - Utilidades de creación de sprites, texturas y helpers visuales
/// </summary>
public partial class NutritionGame2D
{
    private Sprite CreateBoxSprite(int w, int h, Color color, bool border)
    {
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color[] pix = new Color[w * h];
        for (int i = 0; i < pix.Length; i++) pix[i] = color;
        if (border)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x < 6 || x > w - 6 || y < 6 || y > h - 6) pix[y * w + x] = Color.black;
                }
            }
        }
        tex.SetPixels(pix);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    private void CreateButton(string label, Vector3 pos, Color color, System.Action action)
    {
        GameObject btnObj = new GameObject($"Btn_{label}");
        btnObj.transform.SetParent(gameContainer.transform);
        btnObj.transform.position = pos;
        
        SpriteRenderer sr = btnObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateBoxSprite(600, 140, color, true);
        sr.sortingOrder = 5;
        
        btnObj.AddComponent<BoxCollider2D>().size = new Vector2(6, 1.4f);
        MenuButton mb = btnObj.AddComponent<MenuButton>();
        mb.onClick = action;
        mb.originalColor = color;
        
        GameObject textObj = new GameObject($"Text_{label}");
        textObj.transform.SetParent(hudObject.transform, false);
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 42;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        txt.fontStyle = FontStyles.Bold;
        txt.enableWordWrapping = false;
        txt.raycastTarget = false;
        
        RectTransform rt = txt.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 100);
        
        mb.buttonText = textObj;
    }

    private Sprite CreateIngredientSprite(string name, Color c)
    {
        string simpleName = RemoveAccents(name.ToLower());
        Sprite s = FindSpriteRecursive(simpleName);
        
        if (s != null)
        {
            s.name = simpleName;
            return s;
        }

        return CreateFallbackSprite(name, c);
    }

    private Sprite FindSpriteRecursive(string filenameNoExt)
    {
        if (filenameNoExt == "boniato") filenameNoExt = "bonito";
        
        string[] searchPatterns = new string[] 
        { 
            filenameNoExt + "*",
            "*" + filenameNoExt + "*",
            filenameNoExt.Replace(" ", "") + "*",
            "*" + filenameNoExt.Replace(" ", "") + "*",
        };

        foreach (var pattern in searchPatterns)
        {
            Sprite s = TryFindSprite(pattern);
            if (s != null) return s;
        }

        if (filenameNoExt.EndsWith("s"))
        {
            string singular = filenameNoExt.Substring(0, filenameNoExt.Length - 1);
            if (TryFindSprite(singular + "*") != null) return TryFindSprite(singular + "*");
            if (TryFindSprite("*" + singular + "*") != null) return TryFindSprite("*" + singular + "*");
        }

        return null;
    }

    private Sprite TryFindSprite(string pattern)
    {
        try 
        {
            string patternClean = pattern.Replace("*", "");
            bool start = pattern.StartsWith("*");
            bool end = pattern.EndsWith("*");
            
            string foodPath = System.IO.Path.Combine(Application.dataPath, "Sprites", "Food");
            if (System.IO.Directory.Exists(foodPath))
            {
                var files = System.IO.Directory.GetFiles(foodPath);
                foreach(var f in files) {
                    string fname = System.IO.Path.GetFileNameWithoutExtension(f);
                    if (IsMatch(fname, patternClean, start, end))
                        return LoadSpriteFromFiles(new string[]{f});
                }
            }

            string resPath = System.IO.Path.Combine(Application.dataPath, "Resources");
            if (System.IO.Directory.Exists(resPath))
            {
                var files = System.IO.Directory.GetFiles(resPath, "*", System.IO.SearchOption.AllDirectories);
                foreach(var f in files) {
                    string fname = System.IO.Path.GetFileNameWithoutExtension(f);
                    if (IsMatch(fname, patternClean, start, end))
                        return LoadSpriteFromFiles(new string[]{f});
                }
            }
            
            return null;
        } 
        catch { return null; }
    }
    
    private bool IsMatch(string filename, string pattern, bool startWild, bool endWild) {
        if (startWild && endWild) return filename.IndexOf(pattern, System.StringComparison.OrdinalIgnoreCase) >= 0;
        if (startWild) return filename.EndsWith(pattern, System.StringComparison.OrdinalIgnoreCase);
        if (endWild) return filename.StartsWith(pattern, System.StringComparison.OrdinalIgnoreCase);
        return filename.Equals(pattern, System.StringComparison.OrdinalIgnoreCase);
    }

    private Sprite LoadSpriteFromFiles(string[] files)
    {
        foreach (string f in files)
        {
            if (f.EndsWith(".meta")) continue;
            string ext = System.IO.Path.GetExtension(f).ToLower();
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                byte[] data = System.IO.File.ReadAllBytes(f);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(data))
                {
                    tex.filterMode = FilterMode.Point;
                    tex.Apply();
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
            }
        }
        return null;
    }

    private Sprite CreateFallbackSprite(string name, Color c)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        
        for(int i=0; i<size*size; i++) tex.SetPixel(i%size, i/size, Color.clear);
        
        float cx = size/2, cy = size/2, r = size/2 - 2;
        for(int y=0; y<size; y++)
        {
            for(int x=0; x<size; x++)
            {
                if(Vector2.Distance(new Vector2(x,y), new Vector2(cx,cy)) <= r)
                {
                    tex.SetPixel(x, y, c);
                }
                else if(Vector2.Distance(new Vector2(x,y), new Vector2(cx,cy)) <= r + 2)
                {
                    tex.SetPixel(x, y, new Color(0,0,0,0.8f));
                }
            }
        }
        
        Color textColor = Color.white;
        char letter = char.ToUpper(name[0]);
        DrawLetter(tex, letter, (int)cx, (int)cy, textColor);

        tex.Apply();
        Sprite fallback = Sprite.Create(tex, new Rect(0,0,size,size), new Vector2(0.5f,0.5f), 16f);
        fallback.name = "Fallback_" + name;
        return fallback;
    }

    private void DrawLetter(Texture2D tex, char l, int cx, int cy, Color c)
    {
        int[,] dotMatrix = GetCharMatrix(l);
        int scale = 4;
        int w = 5 * scale;
        int h = 7 * scale;
        int startX = cx - w/2;
        int startY = cy - h/2;

        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (dotMatrix[6-y, x] == 1)
                {
                    for(int sy=0; sy<scale; sy++)
                        for(int sx=0; sx<scale; sx++)
                            tex.SetPixel(startX + x*scale + sx, startY + y*scale + sy, c);
                }
            }
        }
    }

    private int[,] GetCharMatrix(char c)
    {
        int[,] matrix = new int[7,5];
        for(int y=0; y<7; y++) for(int x=0; x<5; x++) matrix[y,x] = 1;
        
        if (c == 'A') return new int[,] {{0,1,1,1,0},{1,0,0,0,1},{1,0,0,0,1},{1,1,1,1,1},{1,0,0,0,1},{1,0,0,0,1},{1,0,0,0,1}};
        if (c == 'P') return new int[,] {{1,1,1,1,0},{1,0,0,0,1},{1,0,0,0,1},{1,1,1,1,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0}};
        if (c == 'Q') return new int[,] {{0,1,1,1,0},{1,0,0,0,1},{1,0,0,0,1},{1,0,0,0,1},{1,0,1,0,1},{1,0,0,1,0},{0,1,1,0,1}};
        if (c == 'L') return new int[,] {{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,1,1,1,1}};
        
        return matrix;
    }

    private string RemoveAccents(string text)
    {
        return text.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n").Replace("ü", "u");
    }

    private Sprite[] LoadAndSliceAtlas()
    {
        Sprite atlas = LoadLocalSprite("Ingredients/food_icons_atlas.png");
        if (atlas == null) return null;

        Texture2D tex = atlas.texture;
        tex.filterMode = FilterMode.Point;
        
        int cols = 7;
        int rows = 4;
        int total = cols * rows;
        Sprite[] sprites = new Sprite[total];

        int w = tex.width / cols;
        int h = tex.height / rows;

        for (int i = 0; i < total; i++)
        {
            int c = i % cols;
            int r = rows - 1 - (i / cols);
            
            Rect rect = new Rect(c * w, r * h, w, h);
            sprites[i] = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), 16f);
        }
        return sprites;
    }

    private void CreateParticles(Vector3 pos, Color color)
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject p = new GameObject("Particle");
            p.transform.SetParent(gameContainer.transform);
            p.transform.position = pos;
            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = CreateBoxSprite(8, 8, color, false);
            sr.sortingOrder = 100;
            p.transform.localScale = Vector3.one * Random.Range(0.5f, 1.0f);
            
            ParticleController pc = p.AddComponent<ParticleController>();
            pc.velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(2f, 5f));
            pc.lifetime = 0.6f;
        }
    }

    private Sprite LoadLocalSprite(string path)
    {
        string fullPath = System.IO.Path.Combine(Application.dataPath, "Sprites", path);
        if (System.IO.File.Exists(fullPath))
        {
            byte[] data = System.IO.File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(data))
            {
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }
        }
        return null;
    }

    private void ScaleToFillScreen(GameObject obj, float padding = 1.0f, bool preserveAspect = true)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        if (preserveAspect)
        {
            float scaleX = screenWidth / spriteWidth;
            float scaleY = screenHeight / spriteHeight;
            float finalScale = Mathf.Max(scaleX, scaleY) * padding;
            obj.transform.localScale = new Vector3(finalScale, finalScale, 1);
        }
        else
        {
            obj.transform.localScale = new Vector3(
                (screenWidth / spriteWidth) * padding,
                (screenHeight / spriteHeight) * padding,
                1);
        }
    }

    private Sprite CreateCocineroOakSprite()
    {
        Sprite s = LoadLocalSprite("Characters/cocinero_oak.png");
        
        if (s == null) s = FindSpriteRecursive("cocinero_oak");
        if (s == null) s = FindSpriteRecursive("oak");
        if (s == null) s = FindSpriteRecursive("chef");
        
        if (s != null) {
            s.texture.filterMode = FilterMode.Point;
            return s;
        }

        int w=128, h=256;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color bodyColor = new Color(0.9f, 0.9f, 0.9f);
        Color skinColor = new Color(1f, 0.8f, 0.6f);
        
        for(int y=0; y<h; y++) {
            for(int x=0; x<w; x++) {
                if (y > 100) tex.SetPixel(x, y, bodyColor);
                else if (y > 60) tex.SetPixel(x,y, skinColor);
                else tex.SetPixel(x,y, Color.clear);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(0.5f,0.5f));
    }

    private Sprite CreateShopkeeperSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        
        for(int i=0; i<size*size; i++) tex.SetPixel(i%size, i/size, Color.clear);

        Color skin = new Color(0.85f, 0.65f, 0.55f);
        Color hair = new Color(0.2f, 0.15f, 0.1f);
        Color apron = new Color(0.3f, 0.5f, 0.3f);
        Color shirt = Color.white;

        DrawPixelRect(tex, 24, 40, 16, 16, skin);
        DrawPixelRect(tex, 22, 52, 20, 6, hair);
        tex.SetPixel(28, 48, Color.black); tex.SetPixel(36, 48, Color.black);
        DrawPixelRect(tex, 20, 15, 24, 25, shirt);
        DrawPixelRect(tex, 22, 15, 20, 20, apron);
        DrawPixelRect(tex, 16, 25, 6, 12, skin);
        DrawPixelRect(tex, 42, 25, 6, 12, skin);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 10f);
    }

    private void DrawPixelRect(Texture2D tex, int x, int y, int w, int h, Color c)
    {
        for (int i = x; i < x + w; i++)
            for (int j = y; j < y + h; j++)
                tex.SetPixel(i, j, c);
    }

    private Sprite CreateShoppingBagSprite()
    {
        int w=32, h=32;
        Texture2D t = new Texture2D(w, h);
        t.filterMode = FilterMode.Point;
        Color clear = Color.clear;
        Color bagColor = new Color(0.8f, 0.6f, 0.4f);
        Color shadow = new Color(0.6f, 0.4f, 0.2f);
        
        for(int y=0; y<h; y++) {
            for(int x=0; x<w; x++) {
                t.SetPixel(x, y, clear);
                if(x >= 6 && x <= 26 && y >= 4 && y <= 24) {
                    t.SetPixel(x, y, bagColor);
                    if(x == 6 || x == 26 || y == 4) t.SetPixel(x,y, shadow); 
                }
                if ((x >= 10 && x <= 12 && y > 24 && y < 29) || (x >= 20 && x <= 22 && y > 24 && y < 29) || (x>=10 && x<=22 && y>=28 && y<=29)) {
                    if(!(x > 12 && x < 20 && y < 28))
                        t.SetPixel(x, y, shadow);
                }
            }
        }
        t.Apply();
        return Sprite.Create(t, new Rect(0,0,w,h), new Vector2(0.5f,0.5f));
    }

    private Sprite GenerateSimpleBackground(Color c)
    {
        Texture2D t = new Texture2D(16, 16);
        for (int i = 0; i < 256; i++) t.SetPixel(i % 16, i / 16, c);
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 1f);
    }

    private void CreateSelectionParticles(Vector3 pos)
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject p = new GameObject("P");
            p.transform.position = pos;
            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = GenerateSimpleBackground(Color.yellow);
            sr.sortingOrder = 100;
            p.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            ParticleController pc = p.AddComponent<ParticleController>();
            pc.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f));
            pc.lifetime = 0.5f;
        }
    }

    private Color GetIngredientColor(string ingredient)
    {
        ingredient = ingredient.ToLower();
        if (ingredient == "avena" || ingredient == "arroz" || ingredient == "pasta" || ingredient == "quinoa") return new Color(0.9f, 0.8f, 0.5f);
        if (ingredient == "pollo" || ingredient == "pavo") return new Color(1f, 0.8f, 0.7f);
        if (ingredient == "salmón" || ingredient == "trucha") return new Color(1f, 0.6f, 0.5f);
        if (ingredient == "merluza" || ingredient == "rodaballo") return new Color(0.9f, 0.9f, 1f);
        if (ingredient == "lentejas" || ingredient == "garbanzos") return new Color(0.7f, 0.4f, 0.2f);
        if (ingredient.Contains("verdura") || ingredient == "calabacín" || ingredient == "brócoli") return new Color(0.3f, 0.7f, 0.3f);
        if (ingredient == "tomate" || ingredient == "fresas" || ingredient == "manzana" || ingredient.Contains("fruta")) return new Color(0.9f, 0.2f, 0.2f);
        if (ingredient == "calabaza" || ingredient == "zanahoria" || ingredient == "boniato") return new Color(1f, 0.5f, 0.1f);
        if (ingredient == "plátano" || ingredient == "banana" || ingredient == "patata") return new Color(1f, 0.9f, 0.2f);
        if (ingredient == "queso" || ingredient.Contains("queso") || ingredient == "yogurt" || ingredient == "leche") return new Color(0.95f, 0.95f, 1f);
        if (ingredient == "almendras" || ingredient == "nueces") return new Color(0.6f, 0.4f, 0.3f);
        if (ingredient == "pera") return new Color(0.7f, 0.8f, 0.2f);
        if (ingredient == "arándanos") return new Color(0.4f, 0.2f, 0.7f);
        return new Color(0.6f, 0.6f, 0.6f);
    }
}
