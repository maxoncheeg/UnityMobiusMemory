using System;
using System.Collections.Generic;
using System.Linq;
using Model.Cards;
using Model.Decks;
using Model.Games;
using Model.Games.Players;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityModel;
using Position = Model.Cards.Positioning.Position;
using Random = System.Random;

namespace Scenes
{
    public class GameScript : MonoBehaviour
    {
        [SerializeField] private float _minCam;
        [SerializeField] private float _gameAreaSize;
        [SerializeField] private float _extraAreaSize;
        [SerializeField] private float _extraCamSize;
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _light;


        [SerializeField] private TMP_Text _step;
        [SerializeField] private TMP_Text _points;
        [SerializeField] private TMP_Text _win;


        private GameSingleton _singleton;

        [SerializeField] private int _cardsWidth;

        private IGame _game;
        private IPlayer _currentPlayer;
        private List<IPlayer> _players;

        private Random _random = new();


        private GameObject[,] _cards;
        private float _maxCam;
        private float _camSize;
        private Vector3 _dragOrigin;
        private Vector2 _center;
        private bool _isMouseDrag = false;
        private bool _isGameFinished = false;

        private float _width, _height;
        private float _waitTime = 1f;
        private float _currentWaitTime = 0f;
        private bool _isWaiting = false;

        private Vector2 _heightBorder, _widthBorder;

        private float _botStepTime;
        [SerializeField] private float _minStepTime;
        [SerializeField] private float _maxStepTime;
        private IPlayer _currentBot;
        private bool _isBotStep;
        private bool _isPlayerStep;
        private bool _showCards;
        private List<Position> _botPositions = new();
        private int _botSelectionAmount = 0;

        private List<Position> _playerPositions = new();

        //private VisualElement _rootUI;


        // Start is called before the first frame update
        void Start()
        {
            _singleton = GameSingleton.Create();
            //_rootUI = GetComponentInChildren<UIDocument>().rootVisualElement;
            //  ServerService service = ServerService.Instantiate();
            //  foreach (var serviceClient in service.Clients)
            //  {
            //      Debug.Log(serviceClient.RemoteEndPoint.ToString());
            //  }
            // // service.Dispose(); 

            _cardsWidth = _singleton.CardsWidth;

            SpawnCards();

            _game.NextStep += OnNextStep;
            _game.GameFinished += OnGameFinished;
            OnNextStep(_game.CurrentTurnPlayer);
        }

        private float _gameFinish = 3;
        private void OnGameFinished(object sender, GameEventArgs e)
        {
            _isGameFinished = true;
            _win.text = "ПОБЕДА " + e.PlayerName;
 
        }

        public void OpenCard(int x, int y, PointerEventData data)
        {
            if (data.button == PointerEventData.InputButton.Left && _isPlayerStep)
            {
                Debug.Log(x + " " + y);


                if (_playerPositions.Contains(new(_cardsWidth - x - 1, y))) return;
                _playerPositions.Add(new(_cardsWidth - x - 1, y));
                _cards[y, _cardsWidth - x - 1].GetComponent<Animator>().SetBool("IsRotated", true);
                if (_playerPositions.Count >= 2)
                {
                    _isPlayerStep = false;
                    _isWaiting = true;
                    _currentWaitTime = _waitTime;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            #region cam

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                _camSize -= scroll * _scrollSpeed;
                if (_camSize > _maxCam) _camSize = _maxCam;
                if (_camSize < _minCam) _camSize = _minCam;
                _camera.orthographicSize = _camSize;

                var res = _camera.transform.position;
                CheckCameraBorders(res);
            }

            if (Input.GetMouseButtonDown(1))
            {
                _dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
            }


            if (Input.GetMouseButton(1))
            {
                Vector3 diff = _dragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);
                var res = _camera.transform.position + diff;

                CheckCameraBorders(res);
            }

            #endregion

            if (_isBotStep)
            {
                _botPositions.Clear();
                var bot = _game.Bots.Keys.FirstOrDefault(k => k.Player.Equals(_currentBot)) ??
                          throw new ArgumentNullException(
                              "_game.Bots.Keys.FirstOrDefault(k => k.Player.Equals(_currentBot))");
                var cards = _game.GetCards();
                Position pos1 = bot.SelectCard(),
                    pos2 = bot.SelectCard(cards[pos1.Y][pos1.X], pos1);

                // Debug.Log("x:" + pos1.X + " y:" + pos1.Y);
                // Debug.Log("x:" + pos2.X + " y:" + pos2.Y);
                _botPositions.Add(pos1);
                _botPositions.Add(pos2);

                _botStepTime = _minStepTime + (float)_random.NextDouble() * (_maxStepTime - _minStepTime);
                _botSelectionAmount = 2;
                _isBotStep = false;
            }

            _botStepTime -= Time.deltaTime;
            if (_botSelectionAmount > 0 && _botStepTime <= 0)
            {
                Position pos = _botPositions[--_botSelectionAmount];

                _cards[pos.Y, pos.X].GetComponent<Animator>().SetBool("IsRotated", true);

                if (_botSelectionAmount <= 0)
                {
                    _showCards = true;
                    _botStepTime = _minStepTime;
                }
                else
                    _botStepTime = _minStepTime + (float)_random.NextDouble() * (_maxStepTime - _minStepTime);
            }

            if (_showCards && _botStepTime <= 0)
            {
                _showCards = false;

                var cards = _game.GetCards();

                if (_game.OpenCard(_currentBot, new(_botPositions[0], _botPositions[1])) == OpenStatus.Success)
                {
                    Debug.Log(_currentBot.Name + " УГАДАЛ");

                    Destroy(_cards[_botPositions[1].Y, _botPositions[1].X]);
                    Destroy(_cards[_botPositions[0].Y, _botPositions[0].X]);

                    _cards[_botPositions[1].Y, _botPositions[1].X] = null;
                    _cards[_botPositions[0].Y, _botPositions[0].X] = null;
                }
                else
                {
                    Debug.Log("НЕ УГАДАЛ");
                }
            }

            if (_currentWaitTime > 0)
                _currentWaitTime -= Time.deltaTime;
            if (_isWaiting && _currentWaitTime <= 0)
            {
                _isWaiting = false;
                Position first = _playerPositions.First(), second = _playerPositions.Last();
                if (_game.OpenCard((_currentPlayer), new(first, second)) ==
                    OpenStatus.Success)
                {
                    Destroy(_cards[first.Y, first.X]);
                    Destroy(_cards[second.Y, second.X]);
                }
                else
                {
                    Debug.Log("ЭТО ПЕЧАЛЬ");
                }
            }

            if (_isGameFinished)
            {
                _gameFinish -= Time.deltaTime;
                if (_gameFinish < 0)
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
        }

        private void OnNextStep(IPlayer player)
        {
            if (_isGameFinished) return;
            for (int i = 0; i < _cardsWidth; i++)
                for (int j = 0; j < _cardsWidth; j++)
                    if (_cards[i, j] != null)
                        _cards[i, j].GetComponent<Animator>().SetBool("IsRotated", false);

            if (_players.Contains(player))
            {
                _currentPlayer = player;
                Debug.Log("ВАШ ХОД " + player.Name);
                //_rootUI.Q<Label>("labelPlayer").text = "ХОДИ ДУРЫНДА";
                _isBotStep = false;
                _isPlayerStep = true;
                _playerPositions = new();
            }
            else
            {
                Debug.Log("БОТОВ ХОД");
                //_rootUI.Q<Label>("labelPlayer").text = player.Name;
                _isBotStep = true;
                _isPlayerStep = false;
                _currentBot = player;
            }

            _step.text = player.Name;

            var players = new List<IPlayer>(_game.Players);
            players.Sort((f, s) =>
            {
                if (f.Points > s.Points) return -1;
                if (f.Points < s.Points) return 1;
                return 0;
            });
            string text =
                $"{Environment.NewLine}<color=\"red\">{players.First().Name}:\t<b>{players.First().Points}</b></color>{Environment.NewLine}";
            for (int i = 1; i < players.Count; i++)
            {
                text += $"{players[i].Name}:\t<b>{players[i].Points}</b>{Environment.NewLine}";
            }

            _points.text = text;
        }

        private void SpawnCards()
        {
            List<string> list = UnityEngine.Resources.LoadAll<Sprite>("Sprites/Images").Select(c => c.name).ToList();

            var factory = new MemoryRandomCardFactory(list);
            Deck deck = new(_cardsWidth, factory, new Random());
            _players = new();

            for (int i = 0; i < _singleton.PlayersAmount; i++)
            {
                _players.Add(new Player(Guid.NewGuid(), "Игрок " + (i + 1)));
            }


            var diffic = new Dictionary<string, BotDifficulty>();

            for (int i = 0; i < _singleton.BotsAmount; i++)
            {
                diffic.Add("Bot" + (i + 1), _singleton.Difficulty);
            }

            _game = new BotGame(deck, new(_players), diffic);

            Vector2 first = Vector2.down, second = Vector2.down;
            GameObject obj = UnityEngine.Resources.Load<GameObject>("Prefabs/card");
            var size = obj.GetComponent<SpriteRenderer>().bounds.size;

            var gameCards = _game.GetCards();
            _cards = new GameObject[_cardsWidth, _cardsWidth];
            for (int i = 0; i < _cardsWidth; i++)
            {
                for (int j = 0; j < _cardsWidth; j++)
                {
                    if (i == 0 && j == 0) first = new Vector2(size.x * i + size.y * i / 2, size.y * j + size.x * j / 2);
                    if (i == _cardsWidth - 1 && j == _cardsWidth - 1)
                        second = new Vector2(size.x * i + size.y * i / 2, size.y * j + size.x * j / 2);

                    GameObject card = Instantiate(obj,
                        new Vector2(size.x * i + size.y * i / 2, size.y * j + size.x * j / 2), Quaternion.identity);

                    #region trigger

                    EventTrigger.Entry entry = new()
                    {
                        eventID = EventTriggerType.PointerUp
                    };
                    int x = i, y = j;
                    entry.callback.AddListener((data) => OpenCard(y, x, data as PointerEventData));
                    card.GetComponent<EventTrigger>().triggers.Add(entry);

                    #endregion

                    var image = card.GetComponentsInChildren<Transform>().First(o => o.name == "image");
                    image.GetComponent<SpriteRenderer>().sprite =
                        UnityEngine.Resources.Load<Sprite>("Sprites/Images/" + gameCards[x][_cardsWidth - y - 1].Name);

                    _cards[x, _cardsWidth - y - 1] = card;
                }
            }


            _camera.transform.position = new Vector3((second.x + first.x) / 2, (second.y + first.y) / 2, -5f);
            _center = new Vector2((second.x + first.x) / 2, (second.y + first.y) / 2);
            _camSize = _maxCam = _camera.orthographicSize = second.y / 2 + 3 + _extraCamSize;
            _light.transform.position = _center;
            _widthBorder = new(_center.x - _maxCam - _gameAreaSize - _extraAreaSize,
                _center.x + _maxCam + _extraAreaSize);
            _heightBorder = new(_center.y + _maxCam + _extraAreaSize, _center.y - _maxCam - _extraAreaSize);
        }

        private void OnApplicationQuit()
        {
            ServerService.Instantiate().Dispose();
        }

        private void CheckCameraBorders(Vector3 pos)
        {
            if (pos.y + _camera.orthographicSize > _heightBorder.x)
                pos.y = _heightBorder.x - _camera.orthographicSize;
            if (pos.y - _camera.orthographicSize < _heightBorder.y)
                pos.y = _heightBorder.y + _camera.orthographicSize;
            if (pos.x + _camera.orthographicSize > _widthBorder.y)
                pos.x = _widthBorder.y - _camera.orthographicSize;
            if (pos.x - _camera.orthographicSize < _widthBorder.x)
                pos.x = _widthBorder.x + _camera.orthographicSize;

            _camera.transform.position = pos;
        }
    }
}