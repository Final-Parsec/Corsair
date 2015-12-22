using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Animator))]
public class AlertText: MonoBehaviour
{
    private static Animator animator;
    private static Text textScript;

    public static void Alert(string text)
    {
        if (animator == null || textScript == null)
        {
            throw new System.Exception("Object not instantiated");
        }

        animator.gameObject.SetActive(true);

        textScript.text = text;
        animator.SetTrigger("alert");
    }

    void Start()
    {
        textScript = GetComponent<Text>();
        animator = GetComponent<Animator>();
        animator.gameObject.SetActive(false);
    }
}
