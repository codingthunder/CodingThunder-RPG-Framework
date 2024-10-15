VAR float_string = "System.Single"

VAR result_int = 0
VAR result_float = 0.0
VAR result_string = ""
VAR result_bool = false
->main
===main===
=opening
Cmd=LoadScene:Scene=BlackBackdrop
Cmd=Wait:Dur=3
Many years have passed since the days of yore
When dragons roamed the clouds and giants walked the mountains.
My father told me stories of many mighty heroes in that time.
One was greatest among them all...
The Dungeon King!

Prompt: Say, have you heard the tale of the Dungeon King?
* [I have not.] -> opening_a
* [Once or twice.] -> opening_b
* [Many times.] -> opening_c

=opening_a
Gyeh, gyeh, gyeh. Then you are in for a treat.
->opening_2
=opening_b
From some frilly minstrel, no doubt. Such pomp and primness, but they couldn't tell a tale if it crawled out their mouths and into listening ears.
->opening_2
=opening_c
Well aren't you the erudite little brat? Well, you've never heard it told this way, I can say that much.
->opening_2
=opening_2
Take a seat, my child, and let this old man spin one last tale...
Our story begins, you guessed it, in a dungeon. But it was no ordinary dungeon, no.
You'll see what I mean in time.
Cmd=Wait:Dur=1
-> wake_in_dungeon

===wake_in_dungeon===
Cmd=LoadScene:Scene=DungeonCell
Cmd=Wait:Dur=1
So as I was saying, our story begins in a dungeon.
Look at this sorry little sap.
Cmd=GetVar:Target=(($$Scene.PlayerCharacter).GetComponentInChildren(typeof(Movement2D)) as Movement2D).walkingSpeed:Type={float_string}
Cmd=KMove:Target=$$Scene.PlayerCharacter:X=1.0:Y=0.0:Speed={result_float}:Dist=2.0
Cmd=KMove:Target=$$Scene.PlayerCharacter:X=-1.0:Y=0.0:Speed={result_float}:Dist=2.0
Cmd=KMove:Target=$$Scene.PlayerCharacter:X=1.0:Y=0.0:Speed={result_float}:Dist=2.0
Cmd=KMove:Target=$$Scene.PlayerCharacter:X=-1.0:Y=0.0:Speed={result_float}:Dist=2.0
Cmd=KMove:Target=$$Scene.PlayerCharacter:X=1.0:Y=0.0:Speed={result_float}:Dist=2.0
Cmd=KMove:Target=$$Scene.PlayerCharacter:X=-1.0:Y=0.0:Speed={result_float}:Dist=2.0
He looks so lonely and scared. Let's see if he can escape this place, shall we?
    -> END
