using UnityEngine;

public class AudioVolumeController : MonoBehaviour {

    private ConfigHandler configHandler;

	void Awake () {
        configHandler = GetComponent<ConfigHandler>();
    }
	
	void Update () {
        if (configHandler.ContainsSection("Game") && configHandler.ContainsSectionKey("Game", "Volume"))
        {
            AudioListener.volume = float.Parse(configHandler.Deserialize("Game", "Volume"));
        }
    }
}
