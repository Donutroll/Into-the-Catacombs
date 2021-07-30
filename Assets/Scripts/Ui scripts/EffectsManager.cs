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

    public void InstantiateEffect(GameObject effect, Vector3 position, Vector3 targetPosition)
    {
        Quaternion rotation = new Quaternion();
        if (targetPosition.x < position.x)
            rotation = Quaternion.Euler(0, 180, 0);
        if (targetPosition.y < position.y)
            rotation = Quaternion.Euler(0, 0, -90);
        if (targetPosition.y > position.y)
            rotation = Quaternion.Euler(0, 0, 90);

        Vector3 dxdyPos = (targetPosition - position).normalized;

        AnimationClip[] clips = effect.GetComponent<Animator>().runtimeAnimatorController.animationClips;

        Destroy(Instantiate(effect, position + dxdyPos, rotation), clips[0].length);
    }

}
