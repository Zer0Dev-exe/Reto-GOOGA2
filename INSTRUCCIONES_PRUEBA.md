# 🎮 INSTRUCCIONES PARA PROBAR EL JUEGO

## ⚠️ IMPORTANTE: Sigue estos pasos EXACTAMENTE

### Paso 1: Cerrar Unity completamente
1. Si Unity está abierto, **ciérralo completamente**
2. Asegúrate de que no haya procesos de Unity ejecutándose

### Paso 2: Abrir Unity de nuevo
1. Abre Unity Hub
2. Abre el proyecto "GOOGA2"
3. **ESPERA** a que Unity termine de importar todos los archivos
4. Verás en la esquina inferior derecha un indicador de progreso - espera a que desaparezca

### Paso 3: Verificar la escena
1. En el panel "Project", navega a: `Assets > Scenes`
2. Haz doble clic en `SampleScene.unity` para abrirla
3. En el panel "Hierarchy" deberías ver:
   - Main Camera
   - Global Light 2D
   - SimpleGame (este GameObject tiene el script TestButton.cs)

### Paso 4: Verificar el script
1. En el panel "Hierarchy", haz clic en "SimpleGame"
2. En el panel "Inspector" (derecha), deberías ver:
   - Transform
   - **Test Button (Script)** ← Este debe estar presente

### Paso 5: Presionar Play
1. Haz clic en el botón **Play** ▶️ en la parte superior central
2. **ESPERA** unos segundos a que cargue

### Paso 6: Verificar la consola
1. Abre la consola: `Window > General > Console` (o Ctrl+Shift+C)
2. Deberías ver estos mensajes:
   ```
   === TEST BUTTON INICIADO ===
   ✅ EventSystem creado!
   Creando UI...
   ✅ Canvas creado
   ✅ Textos creados
   ✅ Botón creado
   ✅✅✅ TODO CREADO CORRECTAMENTE ✅✅✅
   ```

### Paso 7: Probar el botón
1. Haz clic en el **botón verde** que dice "INICIAR JUEGO"
2. En la consola deberías ver:
   ```
   🎉🎉🎉 ¡¡¡BOTON PRESIONADO!!! 🎉🎉🎉
   ```
3. El mensaje en pantalla debería cambiar
4. El botón debería cambiar de color y decir "¡FUNCIONANDO!"

---

## 🔍 Si NO funciona:

### Problema: No veo los mensajes en la consola
**Solución:** El script no está ejecutándose. Verifica el Paso 4.

### Problema: Veo los mensajes pero el botón no responde
**Solución:** 
1. Haz clic en el botón verde en el **centro** del botón
2. Verifica que en la consola no haya errores (mensajes rojos)
3. Toma una captura de la consola y envíamela

### Problema: Unity no detecta los cambios
**Solución:**
1. En Unity, haz clic derecho en el panel "Project"
2. Selecciona "Refresh" o presiona Ctrl+R
3. Espera a que termine de reimportar

---

## 📸 Si sigue sin funcionar:
Por favor envíame capturas de pantalla de:
1. La ventana de Unity completa (con el juego en Play)
2. La consola de Unity (Window > General > Console)
3. El Inspector del GameObject "SimpleGame"
