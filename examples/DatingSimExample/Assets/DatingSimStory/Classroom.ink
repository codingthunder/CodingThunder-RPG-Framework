// This Ink scene should focus on things that happen at school and in the classroom.
// This is just convention, of course, but convention makes things easier to remember later.
// Notice that I'm copying a lot from the BasicExample. This is because if it works, don't fix it.

//// Initialize global variables at the top.
VAR times_visited_Classroom = 0

===Classroom===

=default
-> build_classroom_scene ->
-> END

//// CUTSCENES BELOW
//// Cutscenes will usually need to actually open the scene and initialize it, though not always.
=first_day
// -> build_classroom_scene ->
Cmd=LoadScene:SceneName=Classroom
~player_object_name = "PlayerCharacter"
Cmd=Spawn:Target=PlayerCharacter
Cmd=CamFollow:Target="{player_object_name}"
-> END

/// INKTRIGGERS BELOW
//InkTriggers should operate under the assumption that the player is already in the scene and that this stitch
//is being called from an InkTrigger.


//// TUNNELS SECTION BELOW.
//// Like with the BasicScene, we can use these to initialize scene.
//// Unity scene breakdown can be tricky, so for the purposes of this example, we'll operate
/// under the assumption that the classroom is a separate scene from the school itself.
===build_classroom_scene===
Cmd=LoadScene:SceneName=Classroom

{times_visited_Classroom == 0: 
    ~ player_spawn_ref = "$$Scene.PlayerStart.transform.position"
}

~ times_visited_Classroom++

//Initialize scene below.

//LoadPrefab returns the name of the instanced object as a string, so it is accessible as a string.
Cmd=LoadPrefab:PrefabId="PlayerCharacter":Enabled=true:Pos={player_spawn_ref}:Name="PlayerCharacter"
~ player_object_name = result_string

Cmd=CamFollow:Target="{player_object_name}"
->->