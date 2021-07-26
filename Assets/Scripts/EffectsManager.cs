using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public void InstantiateEffect(GameObject effect, Vector3 position)
    {
        AnimationClip[] clips = effect.GetComponent<Animator>().runtimeAnimatorController.animationClips;

        Destroy(Instantiate(effect, position, Quaternion.identity),clips[0].length);
    }

    public void InstantiateAreaEffect()
    { }

    public void InstantiateCircleEffect()
    { }

}
