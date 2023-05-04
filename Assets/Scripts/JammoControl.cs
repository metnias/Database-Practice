using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterSkinController;

[RequireComponent(typeof(CharacterSkinController))]
public class JammoControl : MonoBehaviour
{
    private CharacterSkinController skin;

    private void Awake()
    {
        skin = GetComponent<CharacterSkinController>();
    }

    private bool camReady = false;
    private bool onAct = false;

    private float idleTime = 0f;

    private void Update()
    {
        if (!onAct) return;

        idleTime -= Time.deltaTime;
        if (idleTime > 0f) return;
        switch (Random.Range(0, 4))
        {
            default:
            case 0:
                skin.ChangeEyeOffset(EyePosition.normal);
                skin.ChangeAnimatorIdle(EyePosition.normal);
                idleTime = Random.Range(5f, 15f);
                break;
            case 1:
                skin.ChangeEyeOffset(EyePosition.happy);
                skin.ChangeAnimatorIdle(EyePosition.happy);
                idleTime = Random.Range(4f, 9f);
                break;
            case 2:
                skin.ChangeEyeOffset(EyePosition.angry);
                skin.ChangeAnimatorIdle(EyePosition.angry);
                idleTime = Random.Range(2f, 5f);
                break;
        }
    }

    public void ReadyPlayerOne(string id)
    {
        skin.ChangeMaterialSettings(id.GetHashCode() % skin.albedoList.Length);
        skin.ChangeEyeOffset(EyePosition.normal);
        skin.ChangeAnimatorIdle(EyePosition.normal);
        camReady = true;
        onAct = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!camReady) return;
        if (!other.CompareTag("MainCamera")) return;

        GetComponent<Animator>().Play("Jump");
        skin.ChangeEyeOffset(EyePosition.happy);
        camReady = false;
        onAct = true;
        idleTime = Random.Range(5f, 15f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("MainCamera")) return;
        onAct = false;

        skin.ChangeEyeOffset(EyePosition.dead);
        skin.ChangeAnimatorIdle(EyePosition.dead);
    }
}
