// In the basic example, I made this into a tunnel, but I think that's unnecessary.
// Initializing...

//// Type variables are used in generating Cmds that require a type.
//// You can't just pass "float" or "int" for Type Names. It needs to be 
//// the actual Full name.
VAR float_type = "System.Single"
VAR int_type = "System.Int32"
VAR bool_type = "System.Boolean"
VAR string_type = "System.String"

//// These result variables are used with GetVar Cmd.
//// Ink only supports a handful of data types.
//// When you call GetVar, it'll assign one of the following variables
//// based on its type.
VAR result_int = 0
VAR result_float = 0.0
VAR result_string = ""
VAR result_bool = false

//// These are story-wide variables. If you have a global variable
//// that isn't tied to a specific scene, this is where you want to set them.

VAR player_name = "Damien"
VAR player_object_name = ""
VAR player_spawn_ref = ""

VAR love_interest_1_name = "Ayane"
VAR love_interest_1_affection = 0

// Keeping these together for now. If we add more utility functions, will likely have to group differently.
EXTERNAL isCutscene()
===IsCutscene===
~ isCutscene()
->->