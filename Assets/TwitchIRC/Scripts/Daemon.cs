using System;
using System.Collections.Generic;
using Irc;
using UnityEngine;
using UnityEngine.UI;

public class Daemon : MonoBehaviour
{
    public NPC npc;

    void Start()
    {
        //Subscribe for events
        TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;
        TwitchIrc.Instance.OnUserLeft += OnUserLeft;
        TwitchIrc.Instance.OnUserJoined += OnUserJoined;
        TwitchIrc.Instance.OnServerMessage += OnServerMessage;
        TwitchIrc.Instance.OnExceptionThrown += OnExceptionThrown;
    }
    
    //Send message
    public void MessageSend(string message)
    {
        if (String.IsNullOrEmpty(message))
            return;

        TwitchIrc.Instance.Message(message);
        //ChatText.text += "<b>" + TwitchIrc.Instance.Username + "</b>: " + MessageText.text + "\n";
        //MessageText.text = "";
    }
    
    //Receive message from server
    void OnServerMessage(string message)
    {
        //ChatText.text += "<b>SERVER:</b> " + message + "\n";
        Debug.Log(message);
    }

    //Receive username that has been left from channel 
    void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
    {
        //ChatText.text += "<b>" + channelMessageArgs.From + ":</b> " + channelMessageArgs.Message + "\n";
        Debug.Log("MESSAGE: " + channelMessageArgs.From + ": " + channelMessageArgs.Message);
        string cleanedMsg = channelMessageArgs.Message.ToLower().Replace(' ', '\0');
        if (cleanedMsg.Contains("bravery+")) {
            npc.Closer += .2f;
            npc.Caution -= .2f;
        }
        if (cleanedMsg.Contains("bravery-"))
        {
            npc.Closer -= .2f;
            npc.Caution += .2f;
        }
        if (cleanedMsg.Contains("aggression+"))
            npc.Aggression += .2f;
        if (cleanedMsg.Contains("aggression-"))
            npc.Aggression -= .2f;
        if (cleanedMsg.Contains("spontaneity+"))
            npc.Spontaneity += .2f;
        if (cleanedMsg.Contains("spontaneity-"))
            npc.Spontaneity -= .2f;
        if (cleanedMsg.Contains("energy+"))
        {
            npc.Energy += .2f;
            npc.Bounciness += .2f;
        }
        if (cleanedMsg.Contains("energy-"))
        {
            npc.Energy -= .2f;
            npc.Bounciness -= .2f;
        }


    }

    //Get the name of the user who joined to channel 
    void OnUserJoined(UserJoinedEventArgs userJoinedArgs)
    {
        //ChatText.text += "<b>" + "USER JOINED" + ":</b> " + userJoinedArgs.User + "\n";
        Debug.Log("USER JOINED: " + userJoinedArgs.User);
    }


    //Get the name of the user who left the channel.
    void OnUserLeft(UserLeftEventArgs userLeftArgs)
    {
       // ChatText.text += "<b>" + "USER JOINED" + ":</b> " + userLeftArgs.User + "\n";
        Debug.Log("USER JOINED: " + userLeftArgs.User);
    }

    //Receive exeption if something goes wrong
    private void OnExceptionThrown(Exception exeption)
    {
        Debug.Log(exeption);
    }

}
