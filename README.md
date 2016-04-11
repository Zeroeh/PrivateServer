# Private Server
RotMG Server Emulator - Zeroeh's Source

You can set up the database using the old method or by using xampp.
HeidiSQL is recommended for editing the database. I personally imported the struct using MySQL workbench then turning on xampp.

I'm going to post a short tutorial on getting this setup.
1. Import the struct-whitelotus.sql located in GameObjects into workbench
2. Go into GameObject<Database.cs and edit the parameters in line 15.
3. Rebuild the source using Visual Studio 2015.
4. If you're smart enough you should know the rest from here.

This source was made to run efficiently and is optimized for performance. It is more optimized to run off of hamachi since that is what I debugged the source with while testing the 
networking aspects.

As for the projects getting renamed:
ServerEngine = wServer
ConServer = Server
GameObjects = db
The others are pretty self explanatory.

Common commands (You can look in the ServerEngine<realm<commands<AdminCommands.cs for the list)
/gift <itemname>
/spawn <name>
/zen 
/killall <name>

About console colors:
White = Unknown or not colored
Red = Error, most likely doesn't need to be fixed
DarkRed = Critical Error, should be fixed
Cyan = General info, can be disregarded
Green = Everything is good to go :)
Yellow = Error could cause problems
Anything else: either no color added or dunno

Please report any bugs or crashes to nearrealitydotcom@gmail.com (Please include screenshots of the console window, with the scrollbar starting at the top of the crash :]  )