// Use this for frequently used Cmds throughout the project

===FadeFromBlack(dur, auto)===
{auto:
    Cmd=LerpColor:Target=$$Scene.FadePanel.{image_component}.color:A=0.0:Dur={dur} # auto
- else:
    Cmd=LerpColor:Target=$$Scene.FadePanel.{image_component}.color:A=0.0:Dur={dur}
}
->->

===FadeToBlack(dur)===
Cmd=LerpColor:Target=$$Scene.FadePanel.{image_component}.color:A=1.0:Dur={dur}
->->

===FadeInText(text_name,dur)===
Cmd=LerpColor:Target=$$Scene.{text_name}.{text_component}.color:A=1.0:Dur={dur}
->->

===FadeOutText(text_name,dur)===
Cmd=LerpColor:Target=$$Scene.{text_name}.{text_component}.color:A=0.0:Dur={dur}
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