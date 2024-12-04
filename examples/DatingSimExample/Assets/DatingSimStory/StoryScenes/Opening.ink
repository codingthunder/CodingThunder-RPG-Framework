VAR __times_hit_opening_patch_trigger = 0
VAR __distance_between_patches = 0
VAR __truck_speed = 5

===Opening===


//// CUTSCENES BELOW

=pre_start
->FadeToBlack(1)->
Cmd=LoadScene:SceneName=Opening
-> start

=start
Cmd=LerpColor:Target=$$Scene.Sunlight.{light_component}.color:R=0:G=0.06293058:B=1:A=1:Dur=0.0 # auto
Cmd=SetVar:Target=$$Scene.Sunlight.{light_component}.intensity:Source=0.14 # auto
// Cmd=SetVar:Target=$$Scene.Truck_Rimlight.{light_component}.intensity:Source=0 # auto
Cmd=LerpVector:Target=$$Scene.Truck_Left.transform.localPosition:X=3.09:Y=-0.72:Dur=0.0


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

Cmd=LerpColor:Target=$$Scene.Sunlight.{light_component}.color:R=0.6650944:G=0.6861081:B=1:A=1:Dur=47.0 # auto
Cmd=LerpF:Target=$$Scene.Sunlight.{light_component}.intensity:Value=0.35:Dur=47.0 # auto
// Cmd=Wait:Dur=2.0



// Cmd=MoveTo:Target=$$Scene.Truck_Left:Position=$$Scene.patch_trigger_0.transform.position:Speed=10.0:KeepGoing=true # auto
Cmd=Wait:Dur=2.0
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
//TODO: Make a PixelPerfectCameraZoomy!
~ temp target_trigger_offset = __times_hit_opening_patch_trigger mod 3

Cmd=SetVar:Target=$$Scene.wing_light.{light_component}.intensity:Source=0.1
Cmd=SetVar:Target=$$Scene.angel_head_light.{light_component}.intensity:Source=0.1
Cmd=KMoveOverTime:Target=$$Scene.BridgeSequence:Dur=0.0:Position=$$Scene.ForestPatch{target_trigger_offset}.transform.position
Cmd=Despawn:Target=ForestPatch0
Cmd=Despawn:Target=ForestPatch1
Cmd=Despawn:Target=ForestPatch2
-> END


=start_of_bridge
//TODO: Set camera zoom based on ratio, not based on px.

// Cmd=LerpVector:Target=$$Scene.sun.transform.localPosition:Y=4.5:Dur=90:Rate=LINE # auto
Cmd=Anim:Target=$$Scene.sunrise_anim:Playback=PLAY

// Cmd=LerpF:Target=$$Scene.Truck_Rimlight.{light_component}.intensity:Value=9.24:Dur=3 # auto
Cmd=LerpVector:Target=$$Scene.CarCamOffset.transform.localPosition:X=-5:Y=20:Dur=10:Rate=SQUEEZE # auto
Cmd=Wait:Dur=3.0
Cmd=ZoomPixelCam:X=1920:Y=1080:Dur=10.0:Rate=SQUEEZE # auto
Cmd=LerpVector:Target=$$Scene.sunrise_anim.transform.localScale:X=6.5:Y=6.5:Dur=10.0:Rate=SQUEEZE # auto
Cmd=LerpVector:Target=$$Scene.sunrise_anim.transform.localPosition:Y=11.42:Dur=10:Rate=SQUEEZE # auto

//Cmd=KMove:Target=$$Scene.CarCamOffset:Dir=0:Speed=2.0:Dist=20

-> END

=pan_to_angel
Cmd=LerpF:Target=$$Scene.wing_light.{light_component}.intensity:Value=1.0:Dur=35.0 # auto
Cmd=LerpF:Target=$$Scene.angel_head_light.{light_component}.intensity:Value=1.0:Dur=35.0 # auto
Cmd=LerpVector:Target=$$Scene.CarCamOffset.transform.localPosition:X=-5:Y=2:Dur=4.5:RATE=SQUEEZE
-> END

=end_of_bridge

// Cmd=KMove:Target=$$Scene.CarCamOffset:Dir=180:Speed=2.0:Dist=20
// Cmd=Wait:Dur=5.0
Cmd=LerpVector:Target=$$Scene.sunrise_anim.transform.localScale:X=2:Y=2:Dur=10.0:Rate=SQUEEZE # auto
Cmd=LerpVector:Target=$$Scene.sunrise_anim.transform.localPosition:Y=8:Dur=10:Rate=SQUEEZE # auto
Cmd=LerpVector:Target=$$Scene.CarCamOffset.transform.localPosition:X=-5:Y=3:Dur=10:Rate=SQUEEZE # auto
Cmd=ZoomPixelCam:X=320:Y=180:Dur=10.0:Rate=SQUEEZE # auto

-> END

=sign_trigger
Cmd=Despawn:Target=$$Scene.sign_trigger
Cmd=SetVar:Target=$$Scene.SignCamOffset.transform.position:Source=$$Scene.CarCamOffset.transform.position
Cmd=CamFollow:Target="SignCamOffset"
Cmd=LerpVector:Target=$$Scene.SignCamOffset.transform.localPosition:X=0.5:Y=0:Dur=4:Rate=CBRT # auto

-> END

=end_bridge_sequence

//TODO: move this card to the end of the scene.
Cmd=Wait:Dur=5.0
->FadeToBlack(3.0)->
Cmd=Wait:Dur=3.0
->ChangeCardText("... like the echoes of a song...")->
->FadeInCardText(5)->
Cmd=Wait:Dur=3.0
->FadeOutCardText(5)->
-> END

=end_scene
// End of scene.
-> Opening_2.pre_start
