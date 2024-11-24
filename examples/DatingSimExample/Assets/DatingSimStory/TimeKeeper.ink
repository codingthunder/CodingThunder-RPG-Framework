//TimeKeeper Scenes are structured like so: M_D_H
//It's not so important that digit count is correct so long as you're consistent.
//If you're going to add a '0' before single digits, make sure you do so each time.
//So it's either 04_07_09 OR it's 4_7_9. For simplicity's sake, I'm going with the latter.
//Regardless, hours are in a 24-hour format, begin at zero, and top out at 23.
//Months and days start at 1.
//While starting at 1 may make programmers flinch (unless you're a heathen who codes in Lua),
//I'm more focused on following common convention, not code convention.

//// These are used for the in-game TimeKeeper.

//If time_scale is less than 0, Ink will grab it from the TimeKeeper component the first time the story runs.
//Otherwise, it will overwrite what is on the TimeKeeper component.
VAR time_scale = -1.0
VAR ff_scale = 3600

VAR time_keeper_initialized = false

===TimeKeeper===
-> default

=default
->IsCutscene->
    We went default instead.
    This shouldn't happen with the TimeKeeper.
    If the scene doesn't knot or stitch doesn't exist, the game should just continue.
    -> END

//From within Ink, this is how you should navigate to a specific time.
//Do not jump directly to the stitch, or you could cause the TimeKeeper to get out of whack.
//By nature of the TimeKeeper and Ink, this Stitch necessarily MUST end the cutscene.
=skip_to_time(month,day,hour)
    Cmd=SkipTime:Month={month}:Day={day}:Hour={hour}
    -> END

//I'm mainly using the Sleep scene as a good neutral spot to fast-forward time.
//I'd suggest keeping your Sleep scene generally minimalist with little opportunity for interaction.
//Make it pretty as you want, but the whole point of sleep is to skip time without using a hard SkipTime Cmd.
//You could probably make a Wait Sceme as well.

=sleep
Cmd=LoadScene:SceneName=Sleep
->SetGameTimeScale(ff_scale)->
-> END

//Your main knot should route to here when it's ready to start.
=beginning
    -> IsCutscene ->//Not sure if this line needs to be here, but this is just an example.
    //We're going to use this tunnel to initialize TimeKeeper-specific logic at the start of the game.
    -> InitializeTimeKeeper ->
    
    
    
    //TODO: find a better scene for nothing to be happening, or pick a better one.
    Cmd=LoadScene:SceneName=BlackBackdrop
    This is a quick test.
    
    -> skip_to_time(4,10,8)
    // To skip the following example, find this line in TimeKeeper.ink and uncomment the line above it.
    // -> skip_to_time(4,10,4)

//// EXAMPLE_SKIP_STITCHES
//// The following stitches are used to demonstrate the different types of skipping.
//// For actual play, change the two lines above to go to 4_10_8

    //There are two ways to advance time in Ink: TimeKeeper.skip_to_time, and TimeKeeper.sleep.
    //They are both stitches.
    //Do not jump directly to a datetime stitch (such as 4_10_4), or you could cause the TimeKeeper to get out of whack.
    
    //Flipside, if you want to hook any listeners in, it'll be easier on the TimeKeeper itself.
    //We also could also store the Time variables in Ink and have the TimeKeeper check them, but that's not a good idea.
    //It's better to keep time data in one place. The TimeKeeper is the ultimate Source of Truth.
    //For now, we'll do both SetUnityTimeKeeper and the SkipCmd, just to be sure they work.

        =4_10_4
            -> IsCutscene ->
            First, we are using the SkipTime Cmd to jump the game forward to a specific time.
            
            -> skip_to_time(4,10,5)
        
        =4_10_5
        -> IsCutscene ->
        //By NOT setting the time here, we're assuming the time has been correctly set in the TimeKeeper, which is an ill-advised assumption.
            Next, we are going to use the sleep stitch to fast-forward time.
            
            ->sleep
        
        =4_10_6
            -> IsCutscene ->
            //Just in case, it's a good idea to set your timeScale back to the correct amount.
            ->SetGameTimeScale(time_scale)->
            
            //Generally speaking, you want most of your story logic in the specific Ink scene.
            //The TimeKeeper mostly acts as an entry point.
            //I'm leaving this here in the example, but in the real narrative, aside from fixing timeScales, you don't want story logic in this file.
            ...And that's it. Those are the two ways to change time from Ink.
            Today is April 10th, and it is 6 AM.
            It is time to go to your first day of school.
            Let's skip the part where you get ready and jump straight to class, shall we?
        
        //Of the three, I prefer going to the individual stitch and setting the time. So, that's what I'll do.
        
        -> skip_to_time(4,10,8)

//// END EXAMPLE STITCHES

=4_10_8
-> SetGameTimeScale(time_scale) ->
-> Classroom.first_day


//// TUNNELS SECTION BELOW

// Notice how tunnels operate a lot like methods/functions?
// That's why I use them for utilities like this.

===SetGameTimeScale(scale)===
Cmd=SetVar:Target=$$TimeKeeper.timeScale:Source={scale}
->->

===HideClock===
Cmd=Despawn:Target=$$Scene.TimeText
->->

===ShowClock===
Cmd=Spawn:Target=$$Scene.TimeText
->->

===InitializeTimeKeeper===
{time_keeper_initialized: ->->}
~time_keeper_initialized = true

{   time_scale < 0:
        Cmd=GetVar:Target=$$TimeKeeper.timeScale:Type={float_type}
        ~ time_scale = result_float
    - else:
        ->SetGameTimeScale(time_scale)->
}
->->


