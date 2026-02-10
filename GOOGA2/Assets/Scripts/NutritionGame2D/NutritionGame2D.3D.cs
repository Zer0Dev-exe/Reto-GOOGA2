using UnityEngine;

/// <summary>
/// NutritionGame2D - Funciones específicas para la transición a 3D real
/// Compatible con URP (Universal Render Pipeline)
/// </summary>
public partial class NutritionGame2D
{
    private Light directionalLight;

    /// <summary>
    /// Busca el shader URP correcto con fallbacks
    /// </summary>
    private Shader GetURPLitShader()
    {
        // Intentar URP Lit primero
        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s != null) return s;

        // Fallback a URP Simple Lit (más ligero)
        s = Shader.Find("Universal Render Pipeline/Simple Lit");
        if (s != null) return s;

        // Último fallback al Standard (Built-in pipeline)
        s = Shader.Find("Standard");
        if (s != null) return s;

        // Si nada funciona, usar el shader por defecto
        return Shader.Find("Sprites/Default");
    }

    /// <summary>
    /// Crea un material URP con color y suavidad
    /// </summary>
    private Material CreateURPMaterial(Color color, float smoothness = 0.5f)
    {
        Material mat = new Material(GetURPLitShader());
        // URP usa "_BaseColor" en lugar de "_Color"
        mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
        return mat;
    }

    private void Setup3DCamera()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        
        mainCamera.orthographic = false;
        mainCamera.fieldOfView = 50;
        mainCamera.transform.position = new Vector3(0, 5, -8);
        mainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
        mainCamera.backgroundColor = new Color(0.1f, 0.08f, 0.08f);
    }

    private void Setup3DLighting()
    {
        if (directionalLight == null)
        {
            GameObject lightGo = new GameObject("Cooking_Light");
            directionalLight = lightGo.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.intensity = 1.3f;
            directionalLight.color = new Color(1f, 0.95f, 0.8f);
            lightGo.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
        directionalLight.gameObject.SetActive(true);
    }

    private GameObject Create3DPan()
    {
        GameObject panRoot = new GameObject("3D_Pan_Complex");
        panRoot.transform.SetParent(gameContainer.transform);
        panRoot.transform.position = new Vector3(0, 0, 0);

        // Base de la sarten - material URP metálico oscuro
        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObj.name = "Pan_Base";
        baseObj.transform.SetParent(panRoot.transform);
        baseObj.transform.localScale = new Vector3(5f, 0.1f, 5f);
        Material panMat = CreateURPMaterial(new Color(0.15f, 0.15f, 0.18f), 0.6f);
        baseObj.GetComponent<Renderer>().material = panMat;

        // Paredes de la sarten (anillo)
        for (int i = 0; i < 24; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float angle = (i / 24f) * Mathf.PI * 2f;
            wall.transform.SetParent(panRoot.transform);
            wall.transform.localPosition = new Vector3(Mathf.Cos(angle) * 2.5f, 0.4f, Mathf.Sin(angle) * 2.5f);
            wall.transform.localRotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0);
            wall.transform.localScale = new Vector3(0.8f, 1f, 0.2f);
            wall.GetComponent<Renderer>().material = panMat;
        }

        // Mango - material madera
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handle.transform.SetParent(panRoot.transform);
        handle.transform.localPosition = new Vector3(4.5f, 0.3f, 0);
        handle.transform.localScale = new Vector3(4f, 0.25f, 0.5f);
        Material handleMat = CreateURPMaterial(new Color(0.3f, 0.2f, 0.1f), 0.3f);
        handle.GetComponent<Renderer>().material = handleMat;

        return panRoot;
    }

    private void Create3DIngredients()
    {
        for (int i = 0; i < selectedIngredients.Count; i++)
        {
            GameObject ing = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            ing.name = "3D_" + selectedIngredients[i];
            ing.transform.SetParent(gameContainer.transform);
            
            float angle = (i / (float)selectedIngredients.Count) * Mathf.PI * 2f;
            float r = Random.Range(0.5f, 2.0f);
            ing.transform.position = new Vector3(Mathf.Cos(angle) * r, 0.5f, Mathf.Sin(angle) * r);
            ing.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            ing.transform.localScale = Vector3.one * 0.4f;

            // Material URP con el color del ingrediente
            Color ingColor = GetIngredientColor(selectedIngredients[i]);
            Material mat = CreateURPMaterial(ingColor, 0.4f);
            ing.GetComponent<Renderer>().material = mat;

            // Añadir un pequeño toque de física o animación
            ing.AddComponent<ThreeDIngredientEffect>().Setup(Random.Range(0.5f, 2.0f));
        }
    }
}

public class ThreeDIngredientEffect : MonoBehaviour
{
    private float speed;
    private float startPos;

    public void Setup(float s) { 
        speed = s; 
        startPos = transform.position.y;
    }

    void Update()
    {
        // Saltar un poquito al cocinar
        float y = startPos + Mathf.Abs(Mathf.Sin(Time.time * speed * 5f)) * 0.3f;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        transform.Rotate(Vector3.up * speed * 40f * Time.deltaTime);
    }
}
