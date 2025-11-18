using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    [Header("Настройки прятания")]
    public HideSpot currentHideSpot;
    public bool isHidden = false;
    
    // Для хранения позиции перед прятанием
    private Vector3 positionBeforeHiding;
    private PlayerController movementScript;

    void Start()
    {
        // Автоматически находим скрипт движения
        movementScript = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHidden && currentHideSpot != null)
            {
                Hide();
            }
            else if (isHidden)
            {
                Unhide();
            }
        }
    }

    void Hide()
    {
        isHidden = true;
        
        // Сохраняем позицию ДО прятания
        positionBeforeHiding = transform.position;
        
        // Отключаем видимость
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        
        // ОТКЛЮЧАЕМ ДВИЖЕНИЕ - несколько способов:
        
        // Способ 1: Отключаем скрипт движения
        if (movementScript != null)
        {
            movementScript.enabled = false;
            Debug.Log("Движение отключено (скрипт)");
        }
        
        // Способ 2: Отключаем Rigidbody2D (на всякий случай)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Останавливаем движение
            rb.simulated = false; // Отключаем физику
            Debug.Log("Физика отключена (Rigidbody2D)");
        }
        
        Debug.Log("Спрятался! Движение заблокировано");
    }

    void Unhide()
    {
        isHidden = false;
        
        // ВОССТАНАВЛИВАЕМ позицию ДО прятания
        transform.position = positionBeforeHiding;
        
        // Включаем видимость
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.enabled = true;
        }
        
        // ВКЛЮЧАЕМ ДВИЖЕНИЕ обратно:
        
        // Способ 1: Включаем скрипт движения
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
        
        // Способ 2: Включаем Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true; // Включаем физику
        }
        
        // Сбрасываем текущее укрытие
        currentHideSpot = null;
        
        Debug.Log("Вышел из укрытия! Движение восстановлено");
    }
}