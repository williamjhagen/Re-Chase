# Re-Chase

This is a personal project I worked on while producing for MixAR (See the above post.)  The project spawned from my work on previous projects related to Twitch, and was in conjunction with the CMU HCI program, who was researching Audience Participation Games.  My focus was on if an AI that is influenced by an audience is engaging, from the perspective of a broadcaster, as well as a viewer.

The project was coded in C#, and Unity, and relied on technology I developed for my voting tool, Hamster Bot (See below.)   I used a Modular design pattern, each Action the NPC can take, which are evaluated using a fairly basic utility function, and action picker.  An “Action,” is comprised of four basic method signatures, Run, Start, Finish, and Interrupt. 

Note: All art was sourced from http://opengameart.org, but the animation engine was written by hand.

Also Note: The primary input device was coded to be a GameCube controller, because I am a Super Smash Bros nerd.  Judge me all you want.
