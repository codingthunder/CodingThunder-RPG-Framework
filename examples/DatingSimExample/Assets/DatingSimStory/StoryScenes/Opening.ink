VAR __times_hit_opening_patch_trigger = 0
VAR __distance_between_patches = 0
VAR __truck_speed = 5

===Opening===


//// CUTSCENES BELOW

=pre_start
We're pre-starting.
->FadeToBlack(1)->
Cmd=LoadScene:SceneName=Opening
-> start

=start
Cmd=LerpColor:Target=$$Scene.Sunlight.{light_component}.color:R=0:G=0.06293058:B=1:A=1:Dur=0.0 # auto
Cmd=SetVar:Target=$$Scene.Sunlight.{light_component}.intensity:Source=0.14 # auto

Cmd=LerpColor:Target=$$Scene.Sunlight.{light_component}.color:R=0.6650944:G=0.6861081:B=1:A=1:Dur=47.0 # auto

Cmd=GetVar:Type={float_type}:Target=($$Scene.ForestPatch0.transform.position - $$Scene.ForestPatch1.transform.position).magnitude

Cmd=GetVar:Type={float_type}:Target=($$Scene.ForestPatch0.transform.position - $$Scene.ForestPatch1.transform.position).magnitude
~ __distance_between_patches = result_float
->IsCutscene->
->HideClock->
Cmd=LerpF:Target=$$Scene.Truck_Audio.{audio_source_component}.volume:Value=0.5:Dur=35.0 # auto
Cmd=Wait:Dur=1.0
->ChangeCardText("Ages come and go...")->
->FadeInCardText(2)->
Cmd=Wait:Dur=3.0
->FadeOutCardText(2)->
Cmd=Wait:Dur=2.0

->ChangeCardText("... but the blessings of the Goddess...")->
->FadeInCardText(2)->
Cmd=Wait:Dur=3.0
->FadeOutCardText(2)->
Cmd=Wait:Dur=2.0

->ChangeCardText("... linger upon our valley...")->
->FadeInCardText(2)->
Cmd=Wait:Dur=3.0
->FadeOutCardText(2)->
Cmd=Wait:Dur=2.0

->ChangeCardText("... eternal.")->
->FadeInCardText(2)->
Cmd=Wait:Dur=3.0
->FadeOutCardText(2)->

// Cmd=MoveTo:Target=$$Scene.Truck_Left:Position=$$Scene.patch_trigger_0.transform.position:Speed=10.0:KeepGoing=true # auto
Cmd=Move:Target=$$Scene.Truck_Left:Dir=270:Dist=2:Speed=10.0:KeepGoing=true # auto
->FadeFromBlack(3, true)->
-> END

//// INKTRIGGERS BELOW
=patch_trigger
~ __times_hit_opening_patch_trigger += 1
~ temp target_trigger_offset = __times_hit_opening_patch_trigger mod 3
~ temp patch_offset = (__times_hit_opening_patch_trigger + 1) mod 3
Cmd=KMoveOverTime:Target=$$Scene.ForestPatch{patch_offset}:Dur=0.0:Position=((Vector2) $$Scene.ForestPatch{target_trigger_offset}.transform.position) + (Vector2.left * {__distance_between_patches})
// Cmd=MoveTo:Target=$$Scene.Truck_Left:Position=$$Scene.patch_trigger_{target_trigger_offset}.transform.position:Speed=10.0:KeepGoing=true # auto
// ->IsCutscene->
// patch_offset = {patch_offset}
// target_trigger_offset = {target_trigger_offset}
-> END

=bridge_sequence
~ temp target_trigger_offset = __times_hit_opening_patch_trigger mod 3

Cmd=KMoveOverTime:Target=$$Scene.BridgeSequence:Dur=0.0:Position=$$Scene.ForestPatch{target_trigger_offset}.transform.position
Cmd=Despawn:Target=ForestPatch0
Cmd=Despawn:Target=ForestPatch1
Cmd=Despawn:Target=ForestPatch2

-> END