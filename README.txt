# 🎵 [Nombre de tu Juego]

![Unity](https://img.shields.io/badge/Unity-6-black?style=flat-square&logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android-green?style=flat-square&logo=android)
![Version](https://img.shields.io/badge/Version-1.0.0-blue?style=flat-square)

Un juego de ritmo para dispositivos móviles (Android) de 5 carriles, diseñado para ofrecer una experiencia fluida, soporte multitáctil real y fondos de video inmersivos.

## 💡 Inspiración
Este proyecto nace de la pasión por los juegos de ritmo arcade y el deseo de trasladar la intensidad de los 5 paneles directamente a la pantalla de un teléfono móvil. El diseño se centró en lograr una jugabilidad cómoda a dos pulgares y un motor de precisión que recompense el ritmo exacto del jugador.

## ✨ Características Principales
* **Sistema Multitáctil de Alta Respuesta:** Controles físicos en pantalla construidos con `SpriteRenderer` y `BoxCollider2D`, procesando múltiples toques de forma instantánea para permitir notas dobles y triples sin latencia.
* **Motor de Calificación (Hit Judger):** Sistema de evaluación preciso con ventanas de tiempo dinámicas para *Perfect*, *Good*, *Bad* y *Miss*, incluyendo rastreo de Combo máximo y Score final.
* **Inmersión Audiovisual:** Soporte para videos de fondo sincronizados con las pistas de audio y *beat charts* personalizados.
* **Ciclo de Juego Completo:** Flujo de aplicación pulido con transiciones (Fade) entre el Menú Principal, la Selección de Canciones, la partida (con menú de pausa funcional) y la Pantalla de Resultados.
* **Optimización Móvil:** Interfaz dinámica adaptable (Canvas Scaler) y relación de aspecto bloqueada en horizontal para asegurar consistencia visual en cualquier celular.

## 🛠️ Tecnologías Utilizadas
* **Motor:** Unity 6
* **Lenguaje:** C#
* **Plataforma Objetivo:** Android (APK)

## 📥 Descarga e Instalación
Puedes descargar la versión más reciente del juego directamente desde este repositorio.

1. Ve a la pestaña de [Releases](../../releases/latest) a la derecha de esta página.
2. Descarga el archivo `[NombreDelJuego]_v1.0.0.apk` desde la sección *Assets*.
3. Pásalo a tu dispositivo Android.
4. Abre el archivo y permite la instalación de aplicaciones de orígenes desconocidos si tu dispositivo lo solicita.
5. ¡A jugar!

## 🎮 Controles
* **Android (Nativo):** Toca las 5 zonas de los carriles en pantalla usando tus pulgares.
* **PC (Modo Pruebas):** Usa el teclado numérico (`1`, `5`, `3`, `7`, `9`).

## 👨‍💻 Autor
**Fernando Cilia (IgniferDev)**
* GitHub: [@IgniferDev](https://github.com/IgniferDev)