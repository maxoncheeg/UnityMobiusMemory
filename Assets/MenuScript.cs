using System.Collections;
using System.Collections.Generic;
using Model.Games;
using TMPro;using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSingleton
{
    private static GameSingleton _game;
    
    public int BotsAmount { get; set; }
    public int PlayersAmount { get; set; }
    public BotDifficulty Difficulty { get; set; }
    public int CardsWidth { get; set; }

    private GameSingleton()
    {
        _game = this;
    }

    public static GameSingleton Create()
    {
        if (_game == null) _game = new();
        return _game;
    }
}

public class MenuScript : MonoBehaviour
{
    [SerializeField]private TMP_Text _bots;
    [SerializeField]private TMP_Text _players;
    [SerializeField]private TMP_Text _dif;
    [SerializeField]private TMP_Text _cards;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClick()
    {
        if (_bots.text == _players.text && _bots.text == "0") return;
        GameSingleton singleton = GameSingleton.Create();
        singleton.BotsAmount = int.Parse(_bots.text);
        singleton.PlayersAmount = int.Parse(_players.text);
        
        singleton.Difficulty = _dif.text switch
        {
            "Легкие" => BotDifficulty.Easy,
            "Нормальные" => BotDifficulty.Medium,
            "Сложные"=> BotDifficulty.Hard,
            "Невыносимые" => BotDifficulty.Impossible
        };

        singleton.CardsWidth = _cards.text switch
        {
            "4" => 2,
            "16" => 4,
            "36" => 6
        };
        
        SceneManager.LoadScene("Game");
    }
}
