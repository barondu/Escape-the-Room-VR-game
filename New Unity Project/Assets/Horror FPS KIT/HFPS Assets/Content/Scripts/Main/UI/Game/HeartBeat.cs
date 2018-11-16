using UnityEngine;
using UnityEngine.UI;
using ThunderWire.Helper.Parser;

[System.Serializable]
public class HeartBeatSpeed
{
    public int health;
    public float speed;
}

public class HeartBeat : MonoBehaviour {

	public GameObject m_animation;
    public Text healthText;
    [Space(5)]
	public string heartAnimation;
	public float beatSpeed = 1f;
    [Space(5)]
    public HeartBeatSpeed[] heartBeatSpeeds;

    void Start () {
		m_animation.GetComponent<Animation>().Play (heartAnimation);
	}

	void Update () {
        int health = Parser.Convert<int>(healthText.text);

        if (heartBeatSpeeds.Length > 0)
        {
            foreach(var i in heartBeatSpeeds)
            {
                if(health <= i.health)
                {
                    m_animation.GetComponent<Animation>()[heartAnimation].speed = i.speed;
                }
            }

            if(health >= heartBeatSpeeds[0].health)
            {
                m_animation.GetComponent<Animation>()[heartAnimation].speed = beatSpeed;
            }
        }
        else
        {
            m_animation.GetComponent<Animation>()[heartAnimation].speed = beatSpeed;
        }
	}

	void OnEnable()
	{
		m_animation.GetComponent<Animation>().Play (heartAnimation);
	}
}
