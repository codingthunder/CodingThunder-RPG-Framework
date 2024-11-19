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
->IsCutscene->
// -> build_classroom_scene ->
Cmd=LoadScene:SceneName=Classroom
~player_object_name = "PlayerCharacter"
Cmd=Spawn:Target=PlayerCharacter
Cmd=CamFollow:Target="{player_object_name}"
-> END

/// INKTRIGGERS BELOW
//InkTriggers should operate under the assumption that the player is already in the scene and that this stitch
//is being called from an InkTrigger. Not all triggers need nor should be cutscenes, so use sound judgment.

=exit
->IsCutscene->
~temp playerRef = "$$Scene.PlayerCharacter"
~temp pos = "transform.position"
???: Hey! Wait! Don't go out:: It's unsafe! Wild Po---
???: I've just been informed by my lawyer that I'm not allowed to make that reference.
???: Get your stinky butt back inside the classroom!
Cmd=Move:Target=$$Scene.PlayerCharacter:Dir=0:Speed=3:Dist=2
Cmd=Spawn:Target=$$Scene.Teacher
//Beware literal race conditions with these auto actions. They should be used sparingly.
Cmd=Move:Target=$$Scene.PlayerCharacter:Dir=0:Speed=3:Dist=2 # auto
Cmd=Move:Target=$$Scene.Teacher:Dir=0:Speed=3:Dist=2 # auto
Mr. Pickles: I can't believe you planned to just walk out of class like that. Get back in your seat.
Cmd=MoveTo:Target={playerRef}:Position=$$Scene.PlayerRow.{pos}:Speed=1.0
Cmd=MoveTo:Target=$$Scene.Teacher:Position=$$Scene.TeacherSpot.{pos}:Speed=3.0 # auto
Cmd=MoveTo:Target={playerRef}:Position=$$Scene.NextToDesk.{pos}:Speed=1.0
Cmd=Despawn:Target={playerRef}
Cmd=Despawn:Target=EmptyDesk
Cmd=Spawn:Target=PlayerAtDesk

Mr. Pickles: Good. Now, for today's lesson:: we're going to learn about common tropes in literature, starting with...
//This is when you define the girl's name.
As Mr. Pickles gets going with his lecture, the girl behind you taps you on the shoulder and begins to whisper so that only you can hear.
{love_interest_1_name}: Were you really about to just walk out?

//Use this opportunity to explain prompts, basic choices, and how you can use this to define what the character likes.
Prompt: Were you really about to walk out?
    * [You know it.]
        ~ love_interest_1_affection++
        ~ love_interest_1_affection++
        {love_interest_1_name}: That's crazy! You're crazy!
        She says that, but her eyes gleam with excitement.
        You might be crazy, but maybe it's good to be a little crazy.
    * [Nah, I was just messing around.]
        {love_interest_1_name}: Oh. Well, that's cool, I guess.
    * [I hadn't made up my mind.]
        ~ love_interest_1_affection++
        She rolls her eyes a little, but has a small grin.
        {love_interest_1_name}: You're a little nuts. You know that?

- Mr. Pickles: Are you paying attention back there? This will be on the final exam.
{love_interest_1_affection > 0: {love_interest_1_name}: Let's chat again after school, okay?}

-> END


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
// Cmd=LoadPrefab:PrefabId="PlayerCharacter":Enabled=true:Pos={player_spawn_ref}:Name="PlayerCharacter"
~ player_object_name = "PlayerCharacter"

Cmd=CamFollow:Target="{player_object_name}"
->->