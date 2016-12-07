Twitch IRC v1.0

--- DESCRIPTION

Twitch IRC lets you simply connect to Twitch channels and send/receive messages. Tested on standalone and mobile platforms.

NOTE: Twitch IRC is not working in Web build according to cross-domain policy.

Available commands:

- Connect to server
- Disconnect from server
- Join channel
- Leave channel
- Send message

Available responses from server:

- On connected to server
- On user joined (
- On user left
- On channel message
- On server message

--- QUICK START

1. Place TwitchIRC prefab on the scene and select it. Before start you must fill these fields in TwitchIRC component:

* Username: Your Twitch account name 
* OauthToken: Your generated OAuth token (Get it here: http://twitchapps.com/tmi/)
* Channel: Name of the channel to join

2. Enable ConnectOnAwake and start the scene. If connection was successful you will be able to send and receive messages.

3. Read comments in TwitchIrcExample.cs script to see how Twitch IRC works.

---

Also, check out more useful things for Unity3D here: 
https://www.assetstore.unity3d.com/#/publisher/979

