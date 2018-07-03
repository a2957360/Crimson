using System.Collections;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class CombatAnimationController : MonoBehaviour
{

    private SkeletonAnimation _animation;
    private Spine.AnimationState spineAnimationState;
    public ChrController _chr = null;

    // Use this for initialization
    void Start()
    {
        _animation = GetComponentInChildren<SkeletonAnimation>();
        spineAnimationState = _animation.AnimationState;
    }

    public void StartAttackAnimation(string name)
    {
        if (_chr != null)
        {
            switch (name)
            {
                case "dodge":
                    {
                        Invoke("playdodge", 0.5f);
                        //_chr.PlayDodgeSound();
                    }
                    break;
                case "guard":
                    {
                        Invoke("playguard", 0.5f);
                        //_chr.PlayBlockSound();
                    }
                    break;
                case "hurt":
                    {
                        Invoke("playhurt", 0.5f);
                        //_chr.PlayHurtSound();
                    }
                    break;
                default:
                    {
                        Invoke("playattack", 0.5f);
                        //_chr.PlayAttackSound();
                    }
                    break;
            }
        }
        StartCoroutine(AttackAnimation(name));
    }

    public void StartDefenseAnimation(string name)
    {
        if (_chr != null)
        {
            switch (name)
            {
                case "dodge":
                    {
                        Invoke("playdodge", 0.55f);
                        //_chr.PlayDodgeSound();
                    }
                    break;
                case "guard":
                    {
                        Invoke("playguard", 0.55f);
                        //_chr.PlayBlockSound();
                    }
                    break;
                case "hurt":
                    {
                        Invoke("playhurt", 0.55f);
                        //_chr.PlayHurtSound();
                    }
                    break;
                default:
                    {
                        Invoke("playattack", 0.55f);
                        //_chr.PlayAttackSound();
                    }
                    break;
            }
        }
        StartCoroutine(DefendeAnimation(name));
    }

    void playdodge()
    {
        _chr.PlayDodgeSound();
    }

    void playguard()
    {
        _chr.PlayBlockSound();
    }

    void playhurt()
    {
        _chr.PlayHurtSound();
    }

    void playattack()
    {
        _chr.PlayAttackSound();
    }

    IEnumerator AttackAnimation(string name)
    {
        yield return new WaitForSeconds(0.5f);
        spineAnimationState.SetAnimation(0, name, false);
    }

    IEnumerator DefendeAnimation(string name)
    {
        yield return new WaitForSeconds(0.55f);
        spineAnimationState.SetAnimation(0, name, false);
    }
}
