using UnityEngine;
using Work.Input.Code;

public sealed class InputRoot : MonoBehaviour
{
    public static InputRoot Instance { get; private set; }

    public InputContainer Input { get; private set; }

    public Vector2 MoveVector => Input.MoveVector;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Input = new InputContainer();
        Input.Init();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Input.Deinit();
            Instance = null;
        }
    }
}
