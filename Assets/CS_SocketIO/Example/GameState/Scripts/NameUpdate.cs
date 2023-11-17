using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameUpdate : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    GameController gameController;

    private void Start()
    {
        try
        {
            gameController = (GameController)FindObjectOfType(typeof(GameController));
        }
        catch
        {
            Debug.LogWarning("NetworkController no exist in this scene");
        }
    }

    private void Update()
    {
        if (gameController != null)
        {
            try
            {
                nameText.text = "Lista de jugadores: \n" + PlayerList();
            }
            catch
            {
                Debug.LogWarning("nameText no asigned");
            }
        }
    }

    string PlayerList()
    {
        string listreturn = "";
        for(int i = 0; i < gameController.State.Players.Length; i++)
        {
            listreturn += gameController.State.Players[i].Username + "\n";
        }
        return listreturn;
    }
}
