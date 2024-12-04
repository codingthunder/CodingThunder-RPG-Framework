// Use this for frequently used Cmds throughout the project

===FadeFromBlack(dur, auto)===
{auto:
// Technically, naming CmdSequences shouldn't be necessary, but the parser expects something on the right side of the equal signs.
    CmdSequence=FadeFromBlack # auto
     # auto
- else:
    CmdSequence=FadeFromBlack
    // Cmd=LerpColor:Target=$$Scene.FadePanel.{image_component}.color:A=0.0:Dur={dur}
}
Cmd=LerpColor:Target=$$Scene.FadePanel.{image_component}.color:A=0.0:Dur={dur}
Cmd=Despawn:Target=FadePanel
ENDSEQUENCE
->->

===FadeToBlack(dur)===
Cmd=Spawn:Target=FadePanel
Cmd=LerpColor:Target=$$Scene.FadePanel.{image_component}.color:A=1.0:Dur={dur}
->->

===FadeInText(text_name,dur)===
Cmd=LerpColor:Target=$$Scene.{text_name}.{text_component}.color:A=1.0:Dur={dur}:RATE=CUBE
->->

===FadeOutText(text_name,dur)===
Cmd=LerpColor:Target=$$Scene.{text_name}.{text_component}.color:A=0.0:Dur={dur}:RATE=CBRT
->->

===FadeInCardText(dur)===
Cmd=Spawn:Target=$$Scene.TextCardPanel
~ temp card_text = "Card_Text"
->FadeInText(card_text,dur)->
->->

===FadeOutCardText(dur)===
~ temp card_text = "Card_Text"
->FadeOutText(card_text,dur)->
Cmd=Despawn:Target=$$Scene.TextCardPanel
->->

===ChangeCardText(text)===
Cmd=SetVar:Target=$$Scene.Card_Text.{text_component}.text:Source="{text}"
->->