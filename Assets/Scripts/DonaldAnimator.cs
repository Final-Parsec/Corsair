using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class DonaldAnimator : MonoBehaviour
{
    private Animator animator;
    private ObjectManager objectManager;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        objectManager = ObjectManager.GetInstance();
        objectManager.AudioManager.Talking += Talk;
        objectManager.gameState.Damaged += FeelPain;
    }

    private void Talk()
    {
        animator.SetTrigger("Talk");
        Invoke("StopTalking",objectManager.AudioManager.trumpAudio.clip.length);
    }

    private void StopTalking()
    {
        animator.SetTrigger("StopTalking");
    }

    private void FeelPain()
    {
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Pain"))
        {
            animator.SetTrigger("Pain");
        }
    }
}
