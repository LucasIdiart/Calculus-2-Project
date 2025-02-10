using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class CustomPointer : MonoBehaviour {

    public Texture pointerTexture; // Imagen del puntero
    public bool use_mouse_input = true; // Se controlará con el mouse
    public bool use_gamepad_input = false; // Se controlará con joystick
    //public bool use_accelerometer_input = false; // Opción no testeada
    public bool pointer_returns_to_center = false; // Puntero regresa al centro
    public bool instant_snapping = false; // Regresa instantáneamente al centro sin input
    public float center_speed = 5f; // Velocidad de retorno al centro

    public bool invert_y_axis = false; // Invertir el eje Y

    public float deadzone_radius = 0f; // Radio de la zona muerta en el centro
    public float thumbstick_speed_modifier = 1f; // Multiplicador de velocidad para gamepad
    public float mouse_sensitivity_modifier = 15f; // Multiplicador de sensibilidad para el mouse

    public static Vector2 pointerPosition; // Posición en pantalla del puntero

    [HideInInspector]
    public Rect deadzone_rect; // Rectángulo que representa la zona muerta

    public static CustomPointer instance; // Instancia única de la clase

    void Awake() {
        // Inicializa la posición del puntero al centro de la pantalla
        pointerPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        instance = this;
    }

    void Start () {
        // Actualiza el cursor utilizando la nueva API:
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Calcula el rectángulo de la zona muerta
        deadzone_rect = new Rect((Screen.width / 2) - deadzone_radius, (Screen.height / 2) - deadzone_radius, deadzone_radius * 2, deadzone_radius * 2);

        if (!use_mouse_input && !use_gamepad_input)
            Debug.LogError("(FlightControls) ¡No se seleccionó ningún método de input! Revisa el script CustomPointer y selecciona mouse o gamepad.");
    }

    void Update () {
        // Asegura que el cursor se mantenga bloqueado y oculto cada frame
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (use_mouse_input) {
            float x_axis = Input.GetAxis("Mouse X");
            float y_axis = Input.GetAxis("Mouse Y");

            if (invert_y_axis)
                y_axis = -y_axis;

            // Actualiza la posición del puntero según el input del mouse
            pointerPosition += new Vector2(x_axis * mouse_sensitivity_modifier,
                                           y_axis * mouse_sensitivity_modifier);
        } else if (use_gamepad_input) {
            float x_axis = Input.GetAxis("Horizontal");
            float y_axis = Input.GetAxis("Vertical");

            if (invert_y_axis)
                y_axis = -y_axis;

            pointerPosition += new Vector2(x_axis * thumbstick_speed_modifier * Mathf.Pow(x_axis, 2),
                                           y_axis * thumbstick_speed_modifier * Mathf.Pow(y_axis, 2));
        }

        // Si se activa el retorno al centro y el puntero está fuera de la zona muerta…
        if (pointer_returns_to_center && !deadzone_rect.Contains(pointerPosition)) {
            // Si no hay input y se ha activado el "snap" instantáneo…
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && instant_snapping) {
                pointerPosition = new Vector2(Screen.width / 2, Screen.height / 2);
            } else {
                // Suaviza el retorno al centro
                pointerPosition.x = Mathf.Lerp(pointerPosition.x, Screen.width / 2, center_speed * Time.deltaTime);
                pointerPosition.y = Mathf.Lerp(pointerPosition.y, Screen.height / 2, center_speed * Time.deltaTime);
            }
        }

        // Asegura que el puntero no se salga de la pantalla
        pointerPosition.x = Mathf.Clamp(pointerPosition.x, 0, Screen.width);
        pointerPosition.y = Mathf.Clamp(pointerPosition.y, 0, Screen.height);
    }

    void OnGUI() {
        // Dibuja la textura del puntero centrada en su posición actual
        if (pointerTexture != null)
            GUI.DrawTexture(new Rect(pointerPosition.x - (pointerTexture.width / 2),
                                      Screen.height - pointerPosition.y - (pointerTexture.height / 2),
                                      pointerTexture.width, pointerTexture.height), pointerTexture);
    }
}
