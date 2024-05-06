using System;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityModel;

namespace UIBuilder
{
    public class UIScript : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _menu;
        private VisualElement _multiplayer;

        int amount = 0;

        private event Action NewClient;

        private bool disposeRequired = true;


        private ServerService _service;

        // Start is called before the first frame update
        void Start()
    {
        _service = ServerService.Instantiate();
        _root = GetComponent<UIDocument>().rootVisualElement;
        _menu = _root.Q("Menu");
        _multiplayer = _root.Q("MultiplayerMenu");
        var label = _multiplayer.Q<Label>("labelClientsAmount");

        NewClient += () =>
            label.text = amount.ToString();

        _menu.Q<Button>("ButtonMultiplayer").clicked += () =>
        {
            _menu.style.display = DisplayStyle.None;
            _multiplayer.style.display = DisplayStyle.Flex;
        };


        _multiplayer.Q<Button>("buttonStartServer").clicked += async () =>
        {
            _service.StartAccept(IPAddress.Parse(_multiplayer.Q<TextField>("textIP").text),
                int.Parse(_multiplayer.Q<TextField>("textPort").text));
        };

        _multiplayer.Q<Button>("buttonStartGame").clicked += () =>
        {
            disposeRequired = false;
            _service.DisposeRequired = false;
            Debug.Log(disposeRequired + " " + _service.DisposeRequired);
            _service.AcceptingClient = false;
            SceneManager.LoadScene("Game");
        };
    }

        private void OnApplicationQuit()
    {
        Debug.Log(disposeRequired + " " + _service.DisposeRequired);
        if (disposeRequired && _service.DisposeRequired)
        {
            _service.AcceptingClient = false;
            ServerService.Instantiate().Dispose();
        }
    }

        // Update is called once per frame
        void Update()
    {
        //NewClient.Invoke();
    }
    }
}