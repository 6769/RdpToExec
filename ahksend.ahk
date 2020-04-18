SetTitleMatchMode, 2
DetectHiddenText, Off

pid:=1240


clipboard =  cmd.exe     ; 在剪贴板中存入新内容.

ControlSend, , #r , ahk_pid %pid%
Sleep, 500

ControlSend, , ^v , ahk_pid %pid%  ; ctrl+V
Sleep, 500
ControlSend, , {Enter}, ahk_pid %pid%



