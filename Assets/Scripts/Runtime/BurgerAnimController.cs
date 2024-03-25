using UnityEngine;

namespace FoodMenuAR.Assets.Scripts
{
    public class BurgerAnimController : MonoBehaviour
    {
        /// <summary>
        /// Коллайдер целого блюда, который будет исчезать
        /// при его разложении на ингредиенты.
        /// </summary>
        [SerializeField] private GameObject burgerCollider;

        private Animator _animator;
        private bool _isAssembled = true;

        private void Awake() => _animator = GetComponent<Animator>();

        public void ChangeAssembledState()
        {
            if (_isAssembled)
            {
                // Разбираем
                _animator.SetBool("startAssembling", false);
                _animator.SetBool("startDisassembling", true);
                _isAssembled = false;
                burgerCollider.SetActive(false);
            }
            else
            {
                // Собираем
                _animator.SetBool("startAssembling", true);
                _animator.SetBool("startDisassembling", false);
                _isAssembled = true;
                burgerCollider.SetActive(true);
            }
        }
    }
}