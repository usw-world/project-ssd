using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour {
    [SerializeField] private Animator animator;
    public void OnEndLoading() {
        animator.SetBool("Loading", false);
    }
    public void OnStartLoading() {
        animator.SetBool("Loading", true);
    }
}
