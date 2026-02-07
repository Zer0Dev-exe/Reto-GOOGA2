/// <summary>
/// NutritionGame2D - Datos de escenarios e ingredientes
/// </summary>
public partial class NutritionGame2D
{
    private class Scenario
    {
        public string name;
        public string description;
        public string[] requiredIngredients;
        public string[] goodIngredients;
        public string[] badIngredients;
    }

    private Scenario[] scenarios = new Scenario[]
    {
        new Scenario {
            name = "Embarazo y Deporte",
            description = "Necesidades nutricionales antes y después de realizar actividad física",
            requiredIngredients = new string[] { "quinoa", "pollo", "calabaza", "salmón", "boniato", "verduras" },
            goodIngredients = new string[] { "avena", "nueces", "fruta", "queso", "merluza", "zanahoria" },
            badIngredients = new string[] { "frituras", "azúcar", "alcohol", "procesados" }
        },
        new Scenario {
            name = "Adolescencia y Estrés",
            description = "Combatir el estrés a través de la alimentación",
            requiredIngredients = new string[] { "avena", "fresas", "arándanos", "almendras", "lentejas", "merluza" },
            goodIngredients = new string[] { "nueces", "queso fresco", "calabaza", "zanahoria", "verduras", "rodaballo" },
            badIngredients = new string[] { "café", "energéticas", "azúcar", "frituras" }
        },
        new Scenario {
            name = "Senectud - Gestión de Migraña",
            description = "Alimentación para la tercera edad con gestión de migraña",
            requiredIngredients = new string[] { "avena", "pera", "yogurt", "verduras", "pollo", "calabaza" },
            goodIngredients = new string[] { "fruta", "tomate", "calabacín", "boniato", "sopa" },
            badIngredients = new string[] { "queso curado", "chocolate", "vino", "embutidos", "cítricos" }
        }
    };

    private string[] availableIngredients = new string[]
    {
        "avena", "quinoa", "arroz", "pasta",
        "pollo", "salmón", "merluza", "rodaballo",
        "lentejas", "garbanzos",
        "calabaza", "zanahoria", "tomate", "calabacín", "verduras",
        "fresas", "arándanos", "manzana", "pera", "plátano", "fruta",
        "almendras", "nueces",
        "queso", "queso fresco", "yogurt",
        "boniato", "patata"
    };
}
