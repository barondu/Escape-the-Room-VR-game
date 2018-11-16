using System.Collections;
using UnityEngine;

public class Autosave : MonoBehaviour
{
    [SaveableField, HideInInspector]
    public bool isPlayed;

    private GameObject gamemanager;
    private SaveGameHandler saveGame;

    void Awake()
    {
        gamemanager = GameObject.Find("GAMEMANAGER");
        saveGame = gamemanager.GetComponent<SaveGameHandler>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isPlayed)
        {
            isPlayed = true;
            StartCoroutine(Save());
        }
    }

    private IEnumerator Save()
    {
        yield return new WaitUntil(() => isPlayed);
        saveGame.Save(true);
    }
}
