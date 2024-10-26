//Initialize scene-specific global variables at top of file.
VAR times_visited_BasicScene = 0
VAR intro_trigger_called = false

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
Mysterious Voice: Welcome to the Chapel of Altus. We have been expecting you.
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
Cmd=LoadPrefab:Target="PlayerCharacter":Enabled=true:Pos={player_spawn_ref}:Name="PlayerCharacter"
~ player_object_name = result_string

Cmd=CamFollow:Target="{player_object_name}"

->->