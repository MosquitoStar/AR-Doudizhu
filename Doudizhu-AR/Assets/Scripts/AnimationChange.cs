using UnityEngine;
using System.Collections;

public class AnimationChange : MonoBehaviour
{
    private Animator _animator;


    void Start()
    {
        _animator = this.GetComponent<Animator>();
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            _animator.SetBool("win", true);
        }
        if(Input.GetKeyUp(KeyCode.W))
        {
            _animator.SetBool("win", false);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            _animator.SetBool("lose", true);
        }
        if(Input.GetKeyUp(KeyCode.R))
        {
            _animator.SetBool("lose", false);
        }
        if(Input.GetKeyDown(KeyCode.P)){
            _animator.SetBool("playCard", true);
        }
        if(Input.GetKeyUp(KeyCode.P)){
            _animator.SetBool("playCard",false);
        }
        if(Input.GetKeyDown(KeyCode.Q)){
            _animator.SetBool("claim", true);
        }
                if(Input.GetKeyUp(KeyCode.Q)){
            _animator.SetBool("claim", false);
        }
    }
}