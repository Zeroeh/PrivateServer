# Private Server
##RotMG Server Emulator - Zeroeh's Source

You can set up the database using the old method or by using xampp. \n
HeidiSQL is recommended for editing the database. I personally imported the struct using MySQL workbench then turning on xampp.

## Getting Started
----------------------------------------
Please ensure that you're using Visual Studio 2015 or newer otherwise this guide may be innaccurate for you.

1) After creating a database, go to GameObjects-->Database.cs

2) On line 15 to match with your database settings.

3) Rebuild the entire solution.

4) Run the ServerEngine normally and ConServer as admin.

This source was made to run efficiently and is optimized for performance. It is more optimized to run off of hamachi since that is what I debugged the source with while testing the 
networking aspects.

- `ConServer == Server`
- `ServerEngine == wServer`
- `GameObjects == db`
- `The other projects are self explanatory.`

Common commands (You can look in the ServerEngine<realm<commands<AdminCommands.cs for the list)
- `/spawn <object>`
- `/gift <item>`
- `/zen`

About console colors:
- `White = Unknown or not colored`
- `Red = Error, most likely doesn't need to be fixed`
- `DarkRed = Critical Error, should be fixed`
- `Cyan = General info, can be disregarded`
- `Green = Everything is good to go`
- `Yellow = Error could cause problems`
- `Anything else: either no color added or dunno`

Please report any bugs or crashes here: http://www.mpgh.net/forum/showthread.php?t=986798 (Please include screenshots of the console window, with the scrollbar starting at the top of the crash :]  )