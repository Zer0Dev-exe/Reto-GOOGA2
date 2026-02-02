using UnityEngine;
using System;
using System.Collections.Generic;

namespace GOOGAZ.Config
{
    /// <summary>
    /// Datos de progreso de un escenario específico
    /// </summary>
    [Serializable]
    public class ScenarioProgress
    {
        public string escenarioId;
        public bool completado;
        public bool desbloqueado;
        public float mejorPuntuacion;
        public int intentos;
        public DateTime ultimoIntento;
        public List<string> recetasUsadas = new List<string>();
    }
    
    /// <summary>
    /// Estadísticas del jugador
    /// </summary>
    [Serializable]
    public class PlayerStats
    {
        [Header("Estadísticas Generales")]
        public int escenariosCompletados;
        public int escenariosAprobados;
        public int intentosTotales;
        public float puntuacionPromedio;
        public float mejorPuntuacion;
        
        [Header("Tiempo de Juego")]
        public float tiempoTotalJugado; // en segundos
        public DateTime primeraPartida;
        public DateTime ultimaPartida;
        
        [Header("Recetas")]
        public int recetasDescubiertas;
        public int recetasReferenciaDominadas;
        public List<string> recetasFavoritas = new List<string>();
        
        [Header("Ingredientes")]
        public int ingredientesDescubiertos;
        public List<string> ingredientesDesbloqueados = new List<string>();
        
        [Header("Logros")]
        public List<string> logrosDesbloqueados = new List<string>();
        
        /// <summary>
        /// Actualiza las estadísticas después de completar un escenario
        /// </summary>
        public void ActualizarDespuesDeEscenario(RecipeEvaluation evaluacion, float tiempoEmpleado)
        {
            intentosTotales++;
            tiempoTotalJugado += tiempoEmpleado;
            ultimaPartida = DateTime.Now;
            
            if (evaluacion.aprobado)
            {
                escenariosAprobados++;
            }
            
            if (evaluacion.puntuacionTotal > mejorPuntuacion)
            {
                mejorPuntuacion = evaluacion.puntuacionTotal;
            }
            
            // Recalcular promedio
            puntuacionPromedio = ((puntuacionPromedio * (intentosTotales - 1)) + evaluacion.puntuacionTotal) / intentosTotales;
        }
    }
    
    /// <summary>
    /// Configuración de progreso del jugador
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerProgressConfig", menuName = "GOOGAZ/Config/Player Progress", order = 5)]
    public class PlayerProgressConfig : ScriptableObject
    {
        [Header("Progreso del Jugador")]
        public PlayerStats estadisticas = new PlayerStats();
        
        [Header("Progreso por Escenario")]
        public List<ScenarioProgress> progresoEscenarios = new List<ScenarioProgress>();
        
        [Header("Configuración de Guardado")]
        [Tooltip("¿Guardar automáticamente después de cada escenario?")]
        public bool autoGuardado = true;
        
        [Tooltip("Intervalo de auto-guardado (segundos)")]
        public float intervaloAutoGuardado = 60f;
        
        [Header("Referencias")]
        public ScenarioConfig scenarioDatabase;
        
        /// <summary>
        /// Inicializa el progreso del jugador
        /// </summary>
        public void InicializarProgreso()
        {
            if (estadisticas.primeraPartida == DateTime.MinValue)
            {
                estadisticas.primeraPartida = DateTime.Now;
            }
            
            estadisticas.ultimaPartida = DateTime.Now;
            
            // Inicializar progreso de escenarios si no existe
            if (scenarioDatabase != null)
            {
                foreach (var escenario in scenarioDatabase.escenarios)
                {
                    if (GetProgresoEscenario(escenario.id) == null)
                    {
                        ScenarioProgress progreso = new ScenarioProgress
                        {
                            escenarioId = escenario.id,
                            completado = false,
                            desbloqueado = escenario.desbloqueadoInicio,
                            mejorPuntuacion = 0f,
                            intentos = 0
                        };
                        progresoEscenarios.Add(progreso);
                    }
                }
            }
        }
        
        /// <summary>
        /// Obtiene el progreso de un escenario específico
        /// </summary>
        public ScenarioProgress GetProgresoEscenario(string escenarioId)
        {
            return progresoEscenarios.Find(p => p.escenarioId == escenarioId);
        }
        
        /// <summary>
        /// Actualiza el progreso después de completar un escenario
        /// </summary>
        public void ActualizarProgresoEscenario(string escenarioId, RecipeEvaluation evaluacion, float tiempoEmpleado)
        {
            ScenarioProgress progreso = GetProgresoEscenario(escenarioId);
            
            if (progreso != null)
            {
                progreso.intentos++;
                progreso.ultimoIntento = DateTime.Now;
                
                if (!progreso.recetasUsadas.Contains(evaluacion.receta.id))
                {
                    progreso.recetasUsadas.Add(evaluacion.receta.id);
                    estadisticas.recetasDescubiertas++;
                }
                
                if (evaluacion.aprobado)
                {
                    if (!progreso.completado)
                    {
                        progreso.completado = true;
                        estadisticas.escenariosCompletados++;
                        DesbloquearEscenariosSiguientes(escenarioId);
                    }
                    
                    if (evaluacion.puntuacionTotal > progreso.mejorPuntuacion)
                    {
                        progreso.mejorPuntuacion = evaluacion.puntuacionTotal;
                    }
                }
            }
            
            // Actualizar estadísticas generales
            estadisticas.ActualizarDespuesDeEscenario(evaluacion, tiempoEmpleado);
            
            // Verificar logros
            VerificarLogros();
            
            if (autoGuardado)
            {
                GuardarProgreso();
            }
        }
        
        /// <summary>
        /// Desbloquea escenarios que requieren completar el escenario actual
        /// </summary>
        private void DesbloquearEscenariosSiguientes(string escenarioCompletadoId)
        {
            if (scenarioDatabase == null) return;
            
            foreach (var escenario in scenarioDatabase.escenarios)
            {
                if (escenario.escenariosRequeridos.Contains(escenarioCompletadoId))
                {
                    ScenarioProgress progreso = GetProgresoEscenario(escenario.id);
                    if (progreso != null && !progreso.desbloqueado)
                    {
                        // Verificar si todos los escenarios requeridos están completados
                        bool todosCompletados = true;
                        foreach (string requiridoId in escenario.escenariosRequeridos)
                        {
                            ScenarioProgress requerido = GetProgresoEscenario(requiridoId);
                            if (requerido == null || !requerido.completado)
                            {
                                todosCompletados = false;
                                break;
                            }
                        }
                        
                        if (todosCompletados)
                        {
                            progreso.desbloqueado = true;
                            Debug.Log($"[PlayerProgress] Escenario desbloqueado: {escenario.nombre}");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Verifica y desbloquea logros
        /// </summary>
        private void VerificarLogros()
        {
            // Logro: Primera victoria
            if (estadisticas.escenariosAprobados == 1 && !estadisticas.logrosDesbloqueados.Contains("primera_victoria"))
            {
                estadisticas.logrosDesbloqueados.Add("primera_victoria");
                Debug.Log("[PlayerProgress] ¡Logro desbloqueado: Primera Victoria!");
            }
            
            // Logro: Maestro nutricional (todos los escenarios completados)
            if (estadisticas.escenariosCompletados == progresoEscenarios.Count && 
                !estadisticas.logrosDesbloqueados.Contains("maestro_nutricional"))
            {
                estadisticas.logrosDesbloqueados.Add("maestro_nutricional");
                Debug.Log("[PlayerProgress] ¡Logro desbloqueado: Maestro Nutricional!");
            }
            
            // Logro: Perfeccionista (puntuación perfecta)
            if (estadisticas.mejorPuntuacion >= 100f && !estadisticas.logrosDesbloqueados.Contains("perfeccionista"))
            {
                estadisticas.logrosDesbloqueados.Add("perfeccionista");
                Debug.Log("[PlayerProgress] ¡Logro desbloqueado: Perfeccionista!");
            }
            
            // Logro: Explorador culinario (10 recetas descubiertas)
            if (estadisticas.recetasDescubiertas >= 10 && !estadisticas.logrosDesbloqueados.Contains("explorador_culinario"))
            {
                estadisticas.logrosDesbloqueados.Add("explorador_culinario");
                Debug.Log("[PlayerProgress] ¡Logro desbloqueado: Explorador Culinario!");
            }
        }
        
        /// <summary>
        /// Reinicia el progreso del jugador
        /// </summary>
        public void ReiniciarProgreso()
        {
            estadisticas = new PlayerStats();
            progresoEscenarios.Clear();
            InicializarProgreso();
            GuardarProgreso();
        }
        
        /// <summary>
        /// Guarda el progreso (implementar con PlayerPrefs o sistema de guardado)
        /// </summary>
        public void GuardarProgreso()
        {
            // TODO: Implementar guardado con PlayerPrefs o sistema de archivos JSON
            string json = JsonUtility.ToJson(estadisticas, true);
            PlayerPrefs.SetString("GOOGAZ_PlayerStats", json);
            
            // Guardar progreso de escenarios
            for (int i = 0; i < progresoEscenarios.Count; i++)
            {
                string progresoJson = JsonUtility.ToJson(progresoEscenarios[i], true);
                PlayerPrefs.SetString($"GOOGAZ_Scenario_{i}", progresoJson);
            }
            
            PlayerPrefs.Save();
            Debug.Log("[PlayerProgress] Progreso guardado");
        }
        
        /// <summary>
        /// Carga el progreso guardado
        /// </summary>
        public void CargarProgreso()
        {
            // TODO: Implementar carga con PlayerPrefs o sistema de archivos JSON
            if (PlayerPrefs.HasKey("GOOGAZ_PlayerStats"))
            {
                string json = PlayerPrefs.GetString("GOOGAZ_PlayerStats");
                estadisticas = JsonUtility.FromJson<PlayerStats>(json);
                
                // Cargar progreso de escenarios
                progresoEscenarios.Clear();
                int i = 0;
                while (PlayerPrefs.HasKey($"GOOGAZ_Scenario_{i}"))
                {
                    string progresoJson = PlayerPrefs.GetString($"GOOGAZ_Scenario_{i}");
                    ScenarioProgress progreso = JsonUtility.FromJson<ScenarioProgress>(progresoJson);
                    progresoEscenarios.Add(progreso);
                    i++;
                }
                
                Debug.Log("[PlayerProgress] Progreso cargado");
            }
            else
            {
                InicializarProgreso();
            }
        }
        
        /// <summary>
        /// Obtiene el porcentaje de completitud del juego
        /// </summary>
        public float GetPorcentajeCompletitud()
        {
            if (progresoEscenarios.Count == 0) return 0f;
            return (float)estadisticas.escenariosCompletados / progresoEscenarios.Count * 100f;
        }
        
        /// <summary>
        /// Obtiene el tiempo de juego formateado
        /// </summary>
        public string GetTiempoJuegoFormateado()
        {
            TimeSpan tiempo = TimeSpan.FromSeconds(estadisticas.tiempoTotalJugado);
            return $"{tiempo.Hours}h {tiempo.Minutes}m";
        }
    }
}
