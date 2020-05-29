using UnityEngine;

namespace FoodMenuAR.Assets.Scripts
{
    public class BurgerAnimController : MonoBehaviour
    {
        /// <summary>
        /// Коллайдер целого блюда, который будет исчезать
        /// при его разложении на ингредиенты.
        /// </summary>
        [SerializeField]
        private GameObject burgerCollider;

        /// <summary>
        /// Animator бургера для контроля анимаций.
        /// Инициализируется автоматически в Start().
        /// </summary>
        private Animator anim;

        /// <summary>
        /// Текущее состояние: собран/разобран бургер.
        /// </summary>
        private bool isAssembled = true;

        /// <summary>
        /// Инициализация переменных.
        /// </summary>
        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// Меняем текущее состояние бургера на противоположное.
        /// </summary>
        public void ChangeState()
        {
            if (isAssembled)
            {
                // Разбираем
                anim.SetBool("startAssembling", false);
                anim.SetBool("startDisassembling", true);
                isAssembled = false;
                burgerCollider.SetActive(false);
            }
            else
            {
                // Собираем
                anim.SetBool("startAssembling", true);
                anim.SetBool("startDisassembling", false);
                isAssembled = true;
                burgerCollider.SetActive(true);
            }
        }
    }
}