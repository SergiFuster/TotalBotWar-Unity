using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Unity.VisualScripting;

public class Requester
{
    enum ConexionStates
    {
        CONNECTED,
        FAILED,
        EMPTY
    }
    //Screen width
    public int width;
    //Screen height
    public int height;

    #region SOCKET
    private string gameUrl;
    private string gameId;
    private int gamePort;
    private const int BUFFER_SIZE = 8192;

    private Socket clientSocket;
    private byte[] receiveBuffer = new byte[BUFFER_SIZE];
    #endregion
    private const string SERVER_BASE_URL = "localhost:4000/";
    private const string SERVER_RELATIVE_URL = "game/start";
    private const string HEADER_NAME = "X-Game-Identifier";
    private const string HEADER_VALUE = "TotalBotWarClient";

    private const string BOT0_VARIABLE_KEY = "bot0";
    private const string BOT1_VARIABLE_KEY = "bot1";

    private const string SCREEN_WIDTH_KEY = "width";
    private const string SCREEN_HEIGHT_KEY = "height";


    public string bot0;
    public string bot1;


    private Converter converter;

    // Start is called before the first frame update
    public void GetInfo()
    {
        bot0 = PlayerPrefs.GetString("bot0");
        bot1 = PlayerPrefs.GetString("bot1");

        width = 1000;
        height = 500;

        converter = GameObject.Find("Converter").GetComponent<Converter>();
    }

    public void GetDefaultInfo()
    {
        bot0 = "RandomPlayer";
        bot1 = "OSLAPlayer";

        width = 1000;
        height = 500;

        converter = GameObject.Find("Converter").GetComponent<Converter>();
    }

    public IEnumerator Connect()
    {
        /*
         * Se conecta al servidor, recibe la información del socket,
         * se conecta a él y se auntentifica.
         */

        // Se inicializan los atributos necesarios para conectarse al socket
        yield return StartGameWebRequest();
        //SetPredeterminetSocketAttributes();

        if (!ConnectServer()) yield break;

        // Enviar id
        Send(gameId);
    }

    public IEnumerator StartGame()
    { 
        // Recibir primer game state
        string firstGameState = Receive(BUFFER_SIZE);
        Debug.Log(firstGameState);

        // Hacer setup
        converter.CreateSquadsFromJson(firstGameState);

        // Enviar mensaje de inicio
        Send("START");

        yield return null;
    }

    private void SetPredeterminetSocketAttributes()
    {
        gameUrl = "localhost";
        gamePort =1024;
        gameId = "1";
    }

    public IEnumerator GameLoop()
    {
        while (true)
        {
            try
            {
                string gameState = ChangueInformation();
                Debug.Log(gameState);
                converter.UpdateSquadsFromJson(gameState);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            yield return null;
        }
       
    }

    public string ChangueInformation()
    {
        string gameState = Receive(BUFFER_SIZE);
        Send(InputToSend());
        return gameState;
    }

    public string SendInitialPositions(string initialPositions)
    {
        Send(initialPositions);
        string receivedMessage = Receive(1024);
        return receivedMessage;
    }
    
    private string InputToSend()
    {
        if (Data.Touches.Count <= 0)
        {
            return "no input";
        }
        string input = converter.VectorToJson(Data.Touches.Dequeue());
        return input;
    }
    
    private bool Send(string message)
    {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        try
        {
            clientSocket.BeginSend(messageBytes, 0, messageBytes.Length, 0, new AsyncCallback(SendCallback), clientSocket);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    public string Receive(int bufferSize)
    {
        byte[] buffer = new byte[bufferSize];
        int bytesRead = clientSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }

    private bool ConnectServer()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Conectarse al socket
        try
        {
            clientSocket.Connect(gameUrl, gamePort);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Finalizamos el envío del mensaje
            int bytesSent = clientSocket.EndSend(ar);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    IEnumerator StartGameWebRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{SERVER_BASE_URL}{SERVER_RELATIVE_URL}?{BOT0_VARIABLE_KEY}={bot0}&{BOT1_VARIABLE_KEY}={bot1}&{SCREEN_WIDTH_KEY}={width}&{SCREEN_HEIGHT_KEY}={height}"))
        {
            request.SetRequestHeader(HEADER_NAME, HEADER_VALUE);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                yield return ParseStartWebRequestToGlobalAttributes(request.downloadHandler.text);
            }
        }

        yield return null;
        
    }

    IEnumerator ParseStartWebRequestToGlobalAttributes(string jsonWithStartGameInfo)
    {
        dynamic data = JsonConvert.DeserializeObject(jsonWithStartGameInfo);
        gameUrl = data["host"];
        gamePort = data["port"];
        gameId = data["id"];

        Debug.Log($"host : {gameUrl}, port : {gamePort}, id : {gameId}");
        yield return null;
    }
}
