using UnityEngine;

/// <summary>
/// Genera sprites procedurales para ingredientes con estilo pixel art
/// </summary>
public static class SpriteGenerator
{
    public static Sprite GenerateIngredientSprite(string ingredientName, Color baseColor)
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point; // Pixel perfect
        
        Color[] pixels = new Color[size * size];
        
        // Inicializar con transparente
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        // Generar forma según el tipo de ingrediente
        switch (ingredientName.ToLower())
        {
            case "avena":
            case "quinoa":
            case "arroz":
                DrawGrain(pixels, size, baseColor);
                break;
            case "pollo":
            case "salmón":
            case "merluza":
            case "rodaballo":
                DrawFish(pixels, size, baseColor);
                break;
            case "calabaza":
            case "tomate":
                DrawRoundVegetable(pixels, size, baseColor);
                break;
            case "zanahoria":
                DrawCarrot(pixels, size, baseColor);
                break;
            case "fresas":
            case "manzana":
            case "pera":
                DrawFruit(pixels, size, baseColor);
                break;
            case "almendras":
            case "nueces":
                DrawNut(pixels, size, baseColor);
                break;
            case "queso":
            case "yogurt":
                DrawDairy(pixels, size, baseColor);
                break;
            default:
                DrawGenericFood(pixels, size, baseColor);
                break;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
    
    private static void DrawGrain(Color[] pixels, int size, Color baseColor)
    {
        // Dibujar un montoncito de granos
        int centerX = size / 2;
        int centerY = size / 2;
        int radius = size / 3;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - centerX;
                float dy = y - centerY;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (distance < radius)
                {
                    // Forma ovalada
                    float ellipse = (dx * dx) / (radius * radius) + (dy * dy * 1.5f) / (radius * radius);
                    if (ellipse < 1f)
                    {
                        Color color = baseColor;
                        // Añadir textura de granos
                        if ((x + y) % 8 < 4)
                        {
                            color = Color.Lerp(baseColor, Color.white, 0.2f);
                        }
                        pixels[y * size + x] = color;
                    }
                }
            }
        }
        
        // Borde oscuro
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawFish(Color[] pixels, int size, Color baseColor)
    {
        // Dibujar un pez simple
        int centerX = size / 2;
        int centerY = size / 2;
        
        // Cuerpo del pez (óvalo horizontal)
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - centerX) / (float)(size * 0.35f);
                float dy = (y - centerY) / (float)(size * 0.25f);
                float distance = dx * dx + dy * dy;
                
                if (distance < 1f)
                {
                    Color color = baseColor;
                    // Escamas
                    if ((x / 8 + y / 8) % 2 == 0)
                    {
                        color = Color.Lerp(baseColor, Color.white, 0.15f);
                    }
                    pixels[y * size + x] = color;
                }
            }
        }
        
        // Cola
        for (int y = centerY - size / 6; y < centerY + size / 6; y++)
        {
            for (int x = size - size / 4; x < size; x++)
            {
                if (y >= 0 && y < size && x >= 0 && x < size)
                {
                    float tailFactor = (float)(x - (size - size / 4)) / (size / 4);
                    if (Mathf.Abs(y - centerY) < size / 6 * (1f - tailFactor))
                    {
                        pixels[y * size + x] = Color.Lerp(baseColor, new Color(baseColor.r * 0.8f, baseColor.g * 0.8f, baseColor.b * 0.8f), 0.3f);
                    }
                }
            }
        }
        
        // Ojo
        int eyeX = centerX - size / 6;
        int eyeY = centerY + size / 12;
        DrawCircle(pixels, size, eyeX, eyeY, 4, Color.white);
        DrawCircle(pixels, size, eyeX, eyeY, 2, Color.black);
        
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawRoundVegetable(Color[] pixels, int size, Color baseColor)
    {
        // Dibujar una verdura redonda (tomate, calabaza)
        int centerX = size / 2;
        int centerY = size / 2;
        int radius = size / 3;
        
        DrawCircle(pixels, size, centerX, centerY, radius, baseColor);
        
        // Brillo
        DrawCircle(pixels, size, centerX - radius / 3, centerY + radius / 3, radius / 6, 
            Color.Lerp(baseColor, Color.white, 0.6f));
        
        // Tallo
        for (int y = centerY + radius; y < centerY + radius + size / 8; y++)
        {
            for (int x = centerX - 3; x < centerX + 3; x++)
            {
                if (y >= 0 && y < size && x >= 0 && x < size)
                {
                    pixels[y * size + x] = new Color(0.3f, 0.5f, 0.2f);
                }
            }
        }
        
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawCarrot(Color[] pixels, int size, Color baseColor)
    {
        int centerX = size / 2;
        int topY = size / 4;
        int bottomY = size * 3 / 4;
        
        // Cuerpo de la zanahoria (triángulo)
        for (int y = topY; y < bottomY; y++)
        {
            float progress = (float)(y - topY) / (bottomY - topY);
            int width = (int)(size / 8 + progress * size / 4);
            
            for (int x = centerX - width; x < centerX + width; x++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    Color color = baseColor;
                    // Líneas horizontales
                    if (y % 12 < 2)
                    {
                        color = Color.Lerp(baseColor, new Color(0.6f, 0.3f, 0.1f), 0.3f);
                    }
                    pixels[y * size + x] = color;
                }
            }
        }
        
        // Hojas verdes arriba
        for (int y = topY - size / 8; y < topY; y++)
        {
            for (int x = centerX - size / 12; x < centerX + size / 12; x++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    pixels[y * size + x] = new Color(0.2f, 0.6f, 0.2f);
                }
            }
        }
        
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawFruit(Color[] pixels, int size, Color baseColor)
    {
        int centerX = size / 2;
        int centerY = size / 2;
        int radius = size / 3;
        
        // Cuerpo de la fruta
        DrawCircle(pixels, size, centerX, centerY, radius, baseColor);
        
        // Brillo
        DrawCircle(pixels, size, centerX - radius / 4, centerY + radius / 4, radius / 5, 
            Color.Lerp(baseColor, Color.white, 0.7f));
        
        // Hoja
        for (int y = centerY + radius - 5; y < centerY + radius + size / 10; y++)
        {
            for (int x = centerX - 8; x < centerX + 2; x++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    pixels[y * size + x] = new Color(0.2f, 0.6f, 0.2f);
                }
            }
        }
        
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawNut(Color[] pixels, int size, Color baseColor)
    {
        int centerX = size / 2;
        int centerY = size / 2;
        int radius = size / 4;
        
        // Forma ovalada
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - centerX) / (float)radius;
                float dy = (y - centerY) / (float)(radius * 1.3f);
                float distance = dx * dx + dy * dy;
                
                if (distance < 1f)
                {
                    Color color = baseColor;
                    // Textura
                    if ((x / 4 + y / 4) % 3 == 0)
                    {
                        color = Color.Lerp(baseColor, new Color(0.4f, 0.3f, 0.2f), 0.3f);
                    }
                    pixels[y * size + x] = color;
                }
            }
        }
        
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawDairy(Color[] pixels, int size, Color baseColor)
    {
        // Dibujar un cubo de queso o vaso de yogurt
        int left = size / 4;
        int right = size * 3 / 4;
        int top = size / 4;
        int bottom = size * 3 / 4;
        
        for (int y = top; y < bottom; y++)
        {
            for (int x = left; x < right; x++)
            {
                Color color = baseColor;
                // Agujeros en el queso
                if ((x / 16 + y / 16) % 3 == 0)
                {
                    int holeX = (x / 16) * 16 + 8;
                    int holeY = (y / 16) * 16 + 8;
                    float dist = Mathf.Sqrt((x - holeX) * (x - holeX) + (y - holeY) * (y - holeY));
                    if (dist < 5)
                    {
                        color = Color.Lerp(baseColor, new Color(0.9f, 0.85f, 0.6f), 0.5f);
                    }
                }
                pixels[y * size + x] = color;
            }
        }
        
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawGenericFood(Color[] pixels, int size, Color baseColor)
    {
        // Dibujar un círculo simple
        DrawCircle(pixels, size, size / 2, size / 2, size / 3, baseColor);
        AddOutline(pixels, size, new Color(0.2f, 0.2f, 0.2f, 1f));
    }
    
    private static void DrawCircle(Color[] pixels, int size, int centerX, int centerY, int radius, Color color)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - centerX;
                float dy = y - centerY;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (distance < radius)
                {
                    pixels[y * size + x] = color;
                }
            }
        }
    }
    
    private static void AddOutline(Color[] pixels, int size, Color outlineColor)
    {
        Color[] temp = new Color[pixels.Length];
        System.Array.Copy(pixels, temp, pixels.Length);
        
        for (int y = 1; y < size - 1; y++)
        {
            for (int x = 1; x < size - 1; x++)
            {
                if (temp[y * size + x].a > 0.5f)
                {
                    // Verificar si está en el borde
                    bool isBorder = false;
                    if (temp[y * size + (x - 1)].a < 0.5f ||
                        temp[y * size + (x + 1)].a < 0.5f ||
                        temp[(y - 1) * size + x].a < 0.5f ||
                        temp[(y + 1) * size + x].a < 0.5f)
                    {
                        isBorder = true;
                    }
                    
                    if (isBorder)
                    {
                        pixels[y * size + x] = outlineColor;
                    }
                }
            }
        }
    }
}
