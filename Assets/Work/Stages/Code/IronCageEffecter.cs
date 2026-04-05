using UnityEngine;
using UnityEngine.InputSystem;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Stages.Code;

public class IronCageEffecter : MonoBehaviour
{
    [SerializeField] private AnimationCurve cageMoveAnimationCurve;
    [SerializeField] private float duration;
    [SerializeField] private float maxYPos;
    [SerializeField] private Transform ironCage_1, ironCage_2;

    [SerializeField] private bool nextDoor;

    private void Awake()
    {
        if (nextDoor)
            Bus<StageClearEvent>.Events += HandleOpenEvent;
        else
        {
            Close();
            
        }
    }

    private void OnDestroy()
    {
        if (nextDoor)
            Bus<StageClearEvent>.Events -= HandleOpenEvent;
    }

    private void HandleOpenEvent(StageClearEvent evt)
    {
        Open();
    }

    private void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Open();
        }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            Close();
        }

    }

    [ContextMenu("Open Test")]
    public async void Open()
    {

        float timer = 0;
        ironCage_1.gameObject.SetActive(true);
        ironCage_2.gameObject.SetActive(false);

        Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

        while (duration >= timer)
        {
            float normalizeTime = timer / duration;
            float currentYPos = maxYPos * cageMoveAnimationCurve.Evaluate(normalizeTime);
            currentYPos = Mathf.Clamp(currentYPos, 0, maxYPos);
            timer += Time.fixedDeltaTime;

            Transform target = ironCage_1;

            if (currentYPos >= 4f)
            {
                ironCage_1.gameObject.SetActive(false);
                ironCage_2.gameObject.SetActive(true);

                target = ironCage_2;
            }
            else
            {
                ironCage_1.gameObject.SetActive(true);
                ironCage_2.gameObject.SetActive(false);

                target = ironCage_1;
            }

            target.position = new Vector3(target.position.x, currentYPos, target.position.z);
            await Awaitable.FixedUpdateAsync();
        }

        Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
    }

    [ContextMenu("Close Test")]
    public async void Close()
    {

        float timer = 0;
        ironCage_1.gameObject.SetActive(true);
        ironCage_2.gameObject.SetActive(false);

        Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

        while (duration >= timer)
        {
            float normalizeTime = timer / duration;
            float currentYPos = maxYPos - (maxYPos * cageMoveAnimationCurve.Evaluate(normalizeTime));
            currentYPos = Mathf.Clamp(currentYPos, 0, maxYPos);
            timer += Time.fixedDeltaTime;

            Transform target = ironCage_1;

            if (currentYPos >= 4f)
            {
                ironCage_1.gameObject.SetActive(false);
                ironCage_2.gameObject.SetActive(true);

                target = ironCage_2;
            }
            else
            {
                ironCage_1.gameObject.SetActive(true);
                ironCage_2.gameObject.SetActive(false);

                target = ironCage_1;
            }

            target.position = new Vector3(target.position.x, currentYPos, target.position.z);
            await Awaitable.FixedUpdateAsync();
        }

        Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
    }
}
