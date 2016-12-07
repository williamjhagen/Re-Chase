using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Irc;
using UnityEngine;

#region delegates

public delegate void UserJoined(UserJoinedEventArgs userJoinedArgs);
public delegate void UserLeft(UserLeftEventArgs userLeftArgs);
public delegate void ChannelMessage(ChannelMessageEventArgs channelMessageArgs);
public delegate void ServerMessage(string message);
public delegate void Connected();
public delegate void ExceptionThrown(Exception exeption);

#endregion

public class TwitchIrc : MonoBehaviour
{
    #region delegate_instances

    public UserJoined OnUserJoined;
    public UserLeft OnUserLeft;
    public ChannelMessage OnChannelMessage;
    public ServerMessage OnServerMessage;
    public Connected OnConnected;
    public ExceptionThrown OnExceptionThrown;

#endregion

    #region variables
    
    private const string ServerName = "irc.twitch.tv";
    private const int ServerPort = 6667;

    public static TwitchIrc Instance;

    /// <summary>
    /// Connect to twitch server and join channel automatically?
    /// </summary>
    public bool ConnectOnAwake;
    /// <summary>
    /// Registered Twitch profile name
    /// </summary>
    public string Username;
    /// <summary>
    /// OAuth token of the profile
    /// </summary>
    public string OauthToken;
    /// <summary>
    /// Specific channel to connect
    /// </summary>
    public string Channel;

    // private TcpClient used to talk to the server
    private TcpClient irc;
    // private network stream used to read/write from/to
    private NetworkStream stream;
    // global variable used to read input from the client
    private string inputLine;
    // stream reader to read from the network stream
    private StreamReader reader;
    // stream writer to write to the stream
    private StreamWriter writer;
    
    #endregion

    #region public_methods

    /// <summary>
    /// Connect to Twitch IRC server
    /// </summary>
    public void Connect()
    {
        if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(OauthToken))
            return;

        try
        {
            irc = new TcpClient(ServerName, ServerPort);
            stream = irc.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);

            Send("USER " + Username + "tmi twitch :" + Username);
            Send("PASS " + OauthToken);
            Send("NICK " + Username);

            StartCoroutine("Listen");
        }
        catch (Exception ex)
        {
            if (OnExceptionThrown != null)
                OnExceptionThrown(ex);
        }
    }

    /// <summary>
    /// Disconnect from Twitch IRC server
    /// </summary>
    public void Disconnect()
    {
        irc = null;
        StopCoroutine("Listen");

        if (stream != null)
            stream.Dispose();
        if (writer != null)
            writer.Dispose();
        if (reader != null)
            reader.Dispose();
    }

    /// <summary>
    /// Join to channel after succesful connection
    /// </summary>
    public void JoinChannel()
    {
        if (String.IsNullOrEmpty(Channel))
            return;

        if (Channel[0] != '#')
            Channel = "#" + Channel;

        if (irc != null && irc.Connected)
        {
            Send("JOIN " + Channel);
        }
    }

    /// <summary>
    /// Leave current channel
    /// </summary>
    public void LeaveChannel()
    {
        Send("PART " + Channel);
    }

    /// <summary>
    /// Send message to the current channel
    /// </summary>
    /// <param name="message">Message to send</param>
    public void Message(string message)
    {
        Send("PRIVMSG " + Channel + " :" + message);
    }

    #endregion

    #region private_methods
    
    IEnumerator Listen()
    {
        while (true)
        {
            if (stream.DataAvailable)
                if ((inputLine = reader.ReadLine()) != null)
                    ParseData(inputLine);

            yield return null;
        }
        
    }

    private void ParseData(string data)
    {
        // split the data into parts
        string[] ircData = data.Split(' ');

        // if the message starts with PING we must PONG back
        if (data.Length > 4)
        {
            if (data.Substring(0, 4) == "PING")
            {
                Send("PONG " + ircData[1]);
                return;
            }
        }

        // re-act according to the IRC Commands
        switch (ircData[1])
        {
            case "001": // server welcome message, after this we can join
                Send("MODE " + Username + " +B");
                OnConnected();
                break;
            case "JOIN": // someone joined
                if (Instance.OnUserJoined != null)
                    Instance.OnUserJoined(new UserJoinedEventArgs(ircData[2], ircData[0].Substring(1, ircData[0].IndexOf("!") - 1)));
                break;
            case "PRIVMSG": // message was sent to the channel or as private
                // if it's a private message
                if (ircData[2].ToLower() != Username.ToLower())
                {
                    if (OnChannelMessage != null)
                        OnChannelMessage(new ChannelMessageEventArgs(ircData[2], ircData[0].Substring(1, ircData[0].IndexOf('!') - 1), JoinArray(ircData, 3)));
                }
                break;
            case "PART":
            case "QUIT":// someone left
                if (OnUserLeft != null)
                    OnUserLeft(new UserLeftEventArgs(ircData[2], ircData[0].Substring(1, data.IndexOf("!") - 1)));
                break;
            default:
                // still using this while debugging
                if (ircData.Length > 3)
                    if (OnServerMessage != null)
                        OnServerMessage(JoinArray(ircData, 3));
                break;
        }

    }

    //Strips the message of unnecessary characters
    private string StripMessage(string message)
    {
        // remove IRC Color Codes
        foreach (Match m in new Regex((char)3 + @"(?:\d{1,2}(?:,\d{1,2})?)?").Matches(message))
            message = message.Replace(m.Value, "");

        // if there is nothing to strip
        if (message == "")
            return "";
        if (message.Substring(0, 1) == ":" && message.Length > 2)
            return message.Substring(1, message.Length - 1);
        return message;
    }

    //Joins the array into a string after a specific index
    private string JoinArray(string[] strArray, int startIndex)
    {
        return StripMessage(String.Join(" ", strArray, startIndex, strArray.Length - startIndex));
    }

    private void Send(string message)
    {
        writer.WriteLine(message);
        writer.Flush();
    }

    void OnConnectedToServer()
    {
        JoinChannel();
    }

    #endregion

    void Awake()
    {
        Instance = this;

        OnConnected += OnConnectedToServer;

        if (ConnectOnAwake)
            Connect();
    }

    void OnDisable()
    {
        Disconnect();
    }
}
