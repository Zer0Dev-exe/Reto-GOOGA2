using System.Collections.Generic;

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
        /// <summary>
        /// Diccionario que asocia cada ingrediente con una explicación
        /// de por qué es beneficioso o perjudicial para este escenario.
        /// </summary>
        public Dictionary<string, string> ingredientBenefits;
    }

    private Scenario[] scenarios = new Scenario[]
    {
        new Scenario {
            name = "Embarazo y Deporte",
            description = "Necesidades nutricionales antes y después de realizar actividad física",
            requiredIngredients = new string[] { "quinoa", "pollo", "calabaza", "salmón", "boniato", "verduras" },
            goodIngredients = new string[] { "avena", "nueces", "fruta", "queso", "merluza", "zanahoria" },
            badIngredients = new string[] { "frituras", "azúcar", "alcohol", "procesados" },
            ingredientBenefits = new Dictionary<string, string>
            {
                // Requeridos
                { "quinoa",   "Rica en proteínas completas y hierro, ideal para la recuperación muscular durante el embarazo" },
                { "pollo",    "Fuente de proteínas magras esenciales para el desarrollo fetal y la energía deportiva" },
                { "calabaza", "Alta en betacarotenos y fibra, fortalece el sistema inmune y mejora la digestión" },
                { "salmón",   "Omega-3 DHA fundamental para el desarrollo cerebral del bebé y antiinflamatorio natural" },
                { "boniato",  "Carbohidratos complejos y vitamina A, aporta energía sostenida para el ejercicio" },
                { "verduras", "Vitaminas, minerales y ácido fólico esenciales para prevenir defectos del tubo neural" },
                // Buenos
                { "avena",      "Fibra soluble que estabiliza el azúcar en sangre y aporta energía de liberación lenta" },
                { "nueces",     "Grasas saludables y omega-3 que favorecen el desarrollo neuronal del bebé" },
                { "fruta",      "Vitaminas C y antioxidantes que refuerzan las defensas y ayudan a la absorción de hierro" },
                { "queso",      "Calcio y proteínas que fortalecen los huesos de la madre y el bebé en formación" },
                { "merluza",    "Proteína magra y fósforo, baja en mercurio y segura durante el embarazo" },
                { "zanahoria",  "Vitamina A y betacarotenos que mejoran la visión y la salud de la piel" },
                // Malos
                { "frituras",   "⚠ Grasas trans que aumentan el colesterol y dificultan la circulación sanguínea" },
                { "azúcar",     "⚠ Picos de glucosa perjudiciales, riesgo de diabetes gestacional" },
                { "alcohol",    "⚠ Totalmente contraindicado: causa daño irreversible al desarrollo del feto" },
                { "procesados", "⚠ Exceso de sodio y conservantes que aumentan la retención de líquidos y la tensión arterial" }
            }
        },
        new Scenario {
            name = "Adolescencia y Estrés",
            description = "Combatir el estrés a través de la alimentación",
            requiredIngredients = new string[] { "avena", "fresas", "arándanos", "almendras", "lentejas", "merluza" },
            goodIngredients = new string[] { "nueces", "queso fresco", "calabaza", "zanahoria", "verduras", "rodaballo" },
            badIngredients = new string[] { "café", "energéticas", "azúcar", "frituras" },
            ingredientBenefits = new Dictionary<string, string>
            {
                // Requeridos
                { "avena",      "Libera serotonina que mejora el ánimo y aporta energía estable para el estudio" },
                { "fresas",     "Vitamina C que reduce el cortisol (hormona del estrés) y refuerza las defensas" },
                { "arándanos",  "Antioxidantes que protegen las neuronas y mejoran la memoria y concentración" },
                { "almendras",  "Magnesio y vitamina E que relajan los músculos y reducen la ansiedad" },
                { "lentejas",   "Hierro y triptófano que combaten la fatiga y favorecen la producción de serotonina" },
                { "merluza",    "Omega-3 y fósforo que mejoran la función cerebral y reducen la inflamación" },
                // Buenos
                { "nueces",       "Ácidos grasos omega-3 que mejoran la comunicación neuronal y el estado de ánimo" },
                { "queso fresco", "Triptófano y calcio que favorecen el descanso y fortalecen los huesos en crecimiento" },
                { "calabaza",     "Zinc y magnesio que regulan el sistema nervioso y combaten el insomnio" },
                { "zanahoria",    "Betacarotenos que protegen la vista (importante para pantallas) y refuerzan la piel" },
                { "verduras",     "Vitaminas del grupo B que regulan el sistema nervioso y aportan fibra digestiva" },
                { "rodaballo",    "Proteínas de alta calidad y bajo en grasa, ideal para mantener la energía" },
                // Malos
                { "café",        "⚠ La cafeína aumenta la ansiedad, altera el sueño y sube la tensión arterial" },
                { "energéticas", "⚠ Exceso de cafeína y azúcar que provocan nerviosismo y crash energético" },
                { "azúcar",      "⚠ Picos de glucosa seguidos de bajones que empeoran la irritabilidad y el cansancio" },
                { "frituras",    "⚠ Grasas saturadas que provocan pesadez, dificultan la concentración y el rendimiento" }
            }
        },
        new Scenario {
            name = "Senectud - Gestión de Migraña",
            description = "Alimentación para la tercera edad con gestión de migraña",
            requiredIngredients = new string[] { "avena", "pera", "yogurt", "verduras", "pollo", "calabaza" },
            goodIngredients = new string[] { "fruta", "tomate", "calabacín", "boniato", "sopa" },
            badIngredients = new string[] { "queso curado", "chocolate", "vino", "embutidos", "cítricos" },
            ingredientBenefits = new Dictionary<string, string>
            {
                // Requeridos
                { "avena",    "Fibra y magnesio que regulan la presión arterial y previenen episodios de migraña" },
                { "pera",     "Baja en histaminas y rica en fibra, ideal para la digestión sin desencadenar migrañas" },
                { "yogurt",   "Probióticos que mejoran la flora intestinal y calcio para prevenir la osteoporosis" },
                { "verduras", "Vitaminas y minerales esenciales que fortalecen el sistema inmune debilitado por la edad" },
                { "pollo",    "Proteína magra fácil de digerir, mantiene la masa muscular y aporta vitamina B6" },
                { "calabaza", "Magnesio natural que relaja los vasos sanguíneos y reduce la frecuencia de migrañas" },
                // Buenos
                { "fruta",     "Hidratación natural y vitaminas que combaten el envejecimiento celular" },
                { "tomate",    "Licopeno antioxidante que protege el corazón y mejora la circulación" },
                { "calabacín", "Ligero y fácil de digerir, rico en potasio para regular la tensión arterial" },
                { "boniato",   "Vitamina A y fibra que protegen la visión y regulan el tránsito intestinal" },
                { "sopa",      "Hidratación esencial, fácil digestión y aporte mineral en un formato reconfortante" },
                // Malos
                { "queso curado", "⚠ Alto en tiramina, un potente desencadenante de episodios de migraña severa" },
                { "chocolate",    "⚠ Contiene feniletilamina y cafeína que dilatan los vasos y provocan migrañas" },
                { "vino",         "⚠ Los sulfitos y la histamina del vino son desencadenantes clásicos de migraña" },
                { "embutidos",    "⚠ Nitratos y tiramina que provocan vasodilatación y aumentan el riesgo de migraña" },
                { "cítricos",     "⚠ La acidez y la tiramina de los cítricos pueden desencadenar episodios de cefalea" }
            }
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
