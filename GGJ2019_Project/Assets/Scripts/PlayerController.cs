using UnityEngine;

public abstract class PlayerController : MonoBehaviour, IPauseObserver {

	protected const float DEFAULT_MOUSE_SENSITIVITY = 2f;

	public static PlayerController Instance { get; private set; }

    protected static ItemType HeldItemType;

    protected static bool IsHoldingItem;

	public float mouseSensitivity { get; private set; }
	public PlayerKeybinds Keybinds => keybinds;

	[SerializeField] protected PlayerKeybinds keybinds;

	bool hasFocus;

	protected virtual void Awake () {
		Instance = this;
		LoadSettings();
	}

    protected virtual void Start () {
	    Cursor.lockState = CursorLockMode.Locked;
		hasFocus = Application.isFocused;
	    PauseMenu.Instance.AddObserver(this);
	    PauseMenu.Instance.Close();
    }

    void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape)){
			if(PauseMenu.Instance.IsOpen){
				PauseMenu.Instance.Close();
			}else{
				PauseMenu.Instance.Open();
			}
	    }
	    ExecuteUpdate(hasFocus, PauseMenu.Instance.IsOpen);
    }

	protected abstract void ExecuteUpdate (bool hasFocus, bool pauseMenuOpen);

	protected abstract void SaveBeforeLevelChange ();

    void OnApplicationFocus (bool hasFocus) {
	    this.hasFocus = hasFocus;
    }

    public virtual void OnPauseStateChanged (bool newState) {
	    Cursor.lockState = (newState ? CursorLockMode.None : CursorLockMode.Locked);
	    LoadSettings();
    }

    protected virtual void LoadSettings () {
	    mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", DEFAULT_MOUSE_SENSITIVITY);
    }

}
