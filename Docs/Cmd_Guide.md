# Cmd Guide

Below is a list of all current Cmds, their summaries, and parameters. Note: writing docs sucks, so I asked ChatGPT to do it for me. This way, I have time to write the stuff I actually want to (I don't use GPT to write fiction in any way whatsoever).

## AutoSave
Exists because JC is lazy. Will eventually develop a more robust save Cmd. For now, it'll always save and load the "auto_save" file.

### Parameters:
No parameters.

## Brake
Literally just stops a Movement2D object. "Hits the brakes." Useful for cutscenes and triggers because Movement2D will keep moving as long as `m_speed` is greater than 0.

### Parameters:
*Target*: The GameObject to stop. Resolves to a `GameObject`.

## CamFollow
Uses the `SimpleCameraFollow` component on the main camera to have it follow the transform of a target. The Cmd itself is only active for a single frame. Will consider a longer-term Cmd for more complex camera movements.

### Parameters:
*Target*: The name of the GameObject (or a reference ID) for the camera to follow. Resolves to a `string`.

## Despawn
Removes a GameObject by disabling it (does not destroy it). Because any target despawned must be in the scene, the `$$Scene` root keyword is assumed, though you can add it manually if you wish.

### Parameters:
*Target*: The target GameObject to despawn. Resolves to a `GameObject`.

## GetVar
Sets the `ReturnValue` to whatever you are getting. If called from Ink, will set the appropriate global variable. There is literally no reason to ever call this from code, so `Target` or `Type` are not included as properties.

### Parameters:
*Target*: The target object to get. Resolves to an object of the specified type.
*Type*: The type of the target object to resolve. Resolves to a `Type`.

## KMove
Kinematically moves a GameObject without the need for a Rigidbody attached. Useful if physics doesn't affect it.

### Parameters:
*Target*: The GameObject to move. Resolves to a `GameObject`.
*Dir*: Direction in degrees (0 degrees = UP). Resolves to a `float`.
*Speed*: Speed in Unity units per second. Resolves to a `float`.
*Dist*: Distance to move in Unity units. Resolves to a `float`.

## KMoveOverTime
Moves a GameObject to a specified position over a given duration. Speed is determined by the duration. Use with kinematic targets or targets without rigidbodies.

### Parameters:
*Target*: The GameObject to move. Resolves to a `GameObject`.
*Position*: The target position to move to. Resolves to a `Vector2`.
*Dur*: The duration over which to move. Resolves to a `float` (seconds).

## LoadPrefab
Instantiates (loads) a prefab from the Resources folder. Loading prefabs does not use the `LookupResolver` because it can't differentiate between active GameObjects and prefabs. You can still use labels inside your prefab target ID though. Note: In this context, "Load" means "Instantiate".

### Parameters:
*PrefabId*: The path to the prefab in the Resources folder (exclude "Assets" or "Resources"). Resolves to a `string`.
*Enabled*: Whether the instantiated object is immediately enabled. Resolves to a `bool`.
*Pos*: The position to instantiate the object at. Resolves to a `Vector2`.
*Name*: The name to assign to the instantiated object. Resolves to a `string`.

## LoadScene
Loads a new scene by name. Generally, this should be called from within Ink, not from within a scene itself.

### Parameters:
*SceneName*: The name of the scene to load. Resolves to a `string`.

## Move
Uses the `Movement2D` component to move a GameObject in a specified direction, speed, and distance.

### Parameters:
*Target*: The GameObject to move. Resolves to a `GameObject`.
*Dir*: Direction angle in degrees (0 degrees = UP). Resolves to a `float`.
*Speed*: Speed of movement. Resolves to a `float`.
*Dist*: Distance to move. Resolves to a `float`.

## MoveTo
Moves a GameObject towards a specified position at a given speed using the `Movement2D` component.

### Parameters:
*Target*: The GameObject to move. Resolves to a `GameObject`.
*Position*: The position to move towards. Resolves to a `Vector2`.
*Speed*: The speed at which to move. Resolves to a `float`.

## PersistData
Stores data as-is in the GameData dictionary. Primitives are copied; objects are stored by reference, so changes you make to the data object will be persisted. Useful for storing things like a character's current stats.

### Parameters:
*Key*: The key under which to store the data in the GameData dictionary. Resolves to a `string`.
*Target*: The data to persist. Resolves to an object of the specified type.
*Type*: The type of the target data. Resolves to a `Type`.

## PersistReference
Stores a reference (as a string) to data, instead of the actual data itself. More pertinent for persisting data from Unity itself than from Ink. To use it, you'll need to fetch this reference as a string, then use it as the `Target` in a `GetVar`.

### Parameters:
*Key*: The key under which to store the reference in the GameData dictionary. Resolves to a `string`.
*Reference*: The reference to store. Resolves to a `string`.

## SetVar
Used to set a variable on one object to either a value or a variable of another object. There is literally no reason to use this in code; it's much slower than assigning something normally.

### Parameters:
*Source*: The source value or variable to assign. Resolves to an object.
*Target*: The target variable to set, specified as a string chain (e.g., "object.property"). Resolves to a `string`.

## Spawn
Sets a GameObject to active (does not instantiate an object).

### Parameters:
*Target*: The target GameObject to activate. Resolves to a `GameObject`.

## StoryScene
Starts a new story flow by changing the scene. After using an `InkTrigger` (which is best), this is the second-best way to change scenes in the framework. You usually want to trigger the Unity scene change from within Ink to consolidate all scene initialization logic into a single location.

### Parameters:
*SceneName*: The name of the Ink knot/stitch to load. Resolves to a `string`.

## Wait
Waits for a specified duration. Positive value is in seconds; negative value is in frames.

### Parameters:
*Dur*: The duration to wait. Positive values are in seconds; negative values are in frames. Resolves to a `float`.