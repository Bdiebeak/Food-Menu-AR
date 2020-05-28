using UnityEngine;

namespace FoodMenuAR.Assets.Scripts
{
    public class BurgerAnimController : MonoBehaviour
    {
        private Animator anim;
        private bool isAssembled = true;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void ChangeState()
        {
            if (isAssembled)
            {
                // Disassemble
                anim.SetBool("startAssembling", false);
                anim.SetBool("startDisassembling", true);
                isAssembled = false;
            }
            else
            {
                // Assemble
                anim.SetBool("startAssembling", true);
                anim.SetBool("startDisassembling", false);
                isAssembled = true;
            }
        }


    }
}