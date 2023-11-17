using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net.Sockets;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject GameContainer;
    [SerializeField]
    private Transform PlayersContainer;
    [SerializeField]
    private Transform CoinsContainer;
    [SerializeField]
    private Transform BulletContainer;

    [SerializeField]
    private GameObject PlayerPrefab;
    [SerializeField]
    private GameObject CoinPrefab;
    [SerializeField]
    private GameObject BulletPrefab;
    [SerializeField]
    private GameState state;
    private Dictionary<string, Transform> PlayersToRender;
    private Dictionary<string, Transform> CoinsToRender;
    private Dictionary<string, Transform> BulletsToRender;

    public GameState State { get => state; }

    internal void StartGame(GameState state)
    {
        PlayersToRender = new Dictionary<string, Transform>();
        CoinsToRender = new Dictionary<string, Transform>();
        BulletsToRender = new Dictionary<string, Transform>();

        GameObject.Find("PanelConnect").SetActive(false);
        GameContainer.SetActive(true);


        foreach (Player player in state.Players)
        {
            InstantiatePlayer(player);
        }

        var Socket = NetworkController._Instance.Socket;

        InputController._Instance.onAxisChange += (axis) => { Socket.Emit("move", axis); };

        this.state = state;
        Socket.On("updateState", UpdateState);

        
    }

    private void InstantiatePlayer(Player player)
    {
        GameObject playerGameObject = Instantiate(PlayerPrefab, PlayersContainer);
        playerGameObject.transform.position = new Vector2(player.x, player.y);
        playerGameObject.GetComponent<GamePlayer>().Id = player.Id;
        playerGameObject.GetComponent<GamePlayer>().Username = player.Id;

        PlayersToRender[player.Id] = playerGameObject.transform;
    }

    private void UpdateState(string json)
    {
        GameStateData jsonData = JsonUtility.FromJson<GameStateData>(json);
        state = jsonData.State;
    }

    internal void NewPlayer(string id, string username)
    {
        InstantiatePlayer(new Player { Id = id, Username = username });
    }

    void Update()
    {
        Debug.Log(state);
        if (state != null)
        {
            foreach (Player player in state.Players)
            {
                if (PlayersToRender.ContainsKey(player.Id))
                {
                    PlayersToRender[player.Id].position = new Vector2(player.x, player.y);
                }
                else
                {
                    InstantiatePlayer(player);
                }
              
            }

            var plarersToDelete = PlayersToRender.Where(item => !state.Players.Any(player => player.Id == item.Key)).ToList();

            foreach (var playerItem in plarersToDelete)
            {
                Destroy(playerItem.Value.gameObject);
                PlayersToRender.Remove(playerItem.Key);
            }
            foreach (Coin coin in state.Coins)
            {
                if (CoinsToRender.ContainsKey(coin.Id))
                {
                    CoinsToRender[coin.Id].position = new Vector2(coin.x, coin.y);
                }
                else
                {
                    InstantiateCoin(coin);
                }
            }
            var coinsToDelete = CoinsToRender.Where(item => !state.Coins.Any(coin => coin.Id == item.Key)).ToList();

            foreach (var coinItem in coinsToDelete)
            {
                Destroy(coinItem.Value.gameObject);
                CoinsToRender.Remove(coinItem.Key);
            }

            foreach (Bullet bullet in state.Bullets)
            {
                if (BulletsToRender.ContainsKey(bullet.Id))
                {
                    BulletsToRender[bullet.Id].position = new Vector2(bullet.x, bullet.y);
                }
                else
                {
                    InstantiateBullet(bullet);
                }
            }
            var bulletsToDelete = BulletsToRender.Where(item => !state.Bullets.Any(Bullet => Bullet.Id == item.Key)).ToList();

            foreach (var bulletItem in bulletsToDelete)
            {
                Destroy(bulletItem.Value.gameObject);
                BulletsToRender.Remove(bulletItem.Key);
            }
        }
    }
    private void InstantiateCoin(Coin coin)
    {
        GameObject coinGameObject = Instantiate(CoinPrefab, CoinsContainer);
        coinGameObject.transform.position = new Vector2(coin.x, coin.y);
        coinGameObject.GetComponent<GameCoin>().Id = coin.Id;

        CoinsToRender[coin.Id] = coinGameObject.transform;
    }
    private void InstantiateBullet(Bullet bullet)
    {
        GameObject coinGameObject = Instantiate(BulletPrefab, BulletContainer);
        coinGameObject.transform.position = new Vector2(bullet.x, bullet.y);
        coinGameObject.GetComponent<GameBullet>().Id = bullet.Id;

        BulletsToRender[bullet.Id] = coinGameObject.transform;
    }
}

[Serializable]
public class GameStateData
{
    public GameState State;
}
