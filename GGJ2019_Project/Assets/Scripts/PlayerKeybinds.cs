using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerKeybinds", menuName = "PlayerKeybinds")]
public class PlayerKeybinds : ScriptableObject {

	public KeyCode keyForward;
	public KeyCode keyBack;
	public KeyCode keyLeft;
	public KeyCode keyRight;
	public KeyCode keyJump;
	public KeyCode keySprint;
	public KeyCode keyInteract;

	public override string ToString() {
		return $"Forward: {keyForward}\n" +
				$"Back: {keyBack}\n" +
				$"Left: {keyLeft}\n" +
				$"Right: {keyRight}\n" +
				$"Jump: {keyJump}\n" +
		        $"Sprint: {keySprint}\n" +
		        $"Interact: {keyInteract}";
	}
}
