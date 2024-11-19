//Initialize scene-specific global variables at top of file.
VAR times_visited_BasicScene = 0
VAR intro_trigger_called = false
VAR goal = "unknown"

===BasicScene===
-> Build_BasicScene -> END

////CUTSCENES BELOW
//Cutscenes usually will need to move to a scene or initialize a scene, but not always.
=OnGameStart
-> Build_BasicScene ->
Hello, and welcome to the Coding Thunder RPG Framework basic example. As you can see, it's pretty barebones.
Hopefully, that'll change.
-> END

////INKTRIGGERS BELOW
//InkTriggers should operate under the assumption that the player is already in the scene and that this stitch
//is being called from an InkTrigger.
=IntroTrigger
~ intro_trigger_called = true
Cmd=Brake:Target=$$Scene.{player_object_name}
Cmd=CamFollow:Target="CameraTarget"
Cmd=KMoveOverTime:Target=$$Scene.CameraTarget:Position=$$Scene.SisterElizabeth.transform.position:Dur=3
Mysterious Woman: Welcome, traveler. My name is Sister Elizabeth. What brings you to my humble abbey?
Prompt: Why are you here?
    * [I seek penance.]
        ~ goal = "penance"
        Elizabeth: So do many who find this place.
    * [I seek adventure.]
        ~ goal = "adventure"
        Elizabeth: A noble calling in its own right, though somewhat unusual.
    * [...]
        ~ goal = "unknown"
        Elizabeth: Could it be that you do not trust me?

- Elizabeth: Regardless, you shall find what you seek at the Abbey of Fate.
Cmd=Wait:Dur=1.0
Elizabeth: May I ask your name, Traveler?
Cmd=Wait:Dur=1.0
...# auto
Elizabeth: Could it be that you do not remember?
Elizabeth: How unfortunate.
{goal == "penance": Elizabeth: Perhaps that shall be your first step toward purification. Or perhaps, it is the final key?}

Elizabeth: For now, I shall call you Traveler.
Elizabeth: Behind me are the Doors of Solace. Many travelers before you have entered, but none have returned.
Elizabeth: I cannot say what it is you shall find beyond...
Elizabeth: Shall you pass through the doors? The choice is yours.
Cmd=KMove:Target=$$Scene.SisterElizabeth:Dir=90:Speed=2.0:Dist=2.0
Cmd=KMoveOverTime:Target=$$Scene.CameraTarget:Position=$$Scene.{player_object_name}.transform.position:Dur=3
Cmd=CamFollow:Target="{player_object_name}"
-> END

=EndExampleTrigger
Cmd=Brake:Target=$$Scene.{player_object_name}
Unfortunately for the Traveler, the Doors of Solace would not open.
This is because JC's writing this script at 5 in the damn morning.
Hopefully, this brief scene is sufficient for a basic example.
A more thorough example shall come later.
Thank you for looking at the Coding Thunder RPG Framework.
-> END


////TUNNELS SECTION BELOW

//Basically, this tunnel here is what allows us to consistently recreate the in-game scene as needed.
===Build_BasicScene===

Cmd=LoadScene:SceneName=BasicScene

{times_visited_BasicScene == 0: 
    ~ player_spawn_ref = "$$Scene.PlayerStart.transform.position"
}

~ times_visited_BasicScene++

//Initialize scene below.

//LoadPrefab returns the name of the instanced object as a string, so it is accessible as a string.
Cmd=LoadPrefab:PrefabId="PlayerCharacter":Enabled=true:Pos={player_spawn_ref}:Name="PlayerCharacter"
~ player_object_name = result_string

Cmd=CamFollow:Target="{player_object_name}"

->->