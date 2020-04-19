SetTitleMatchMode, 2
DetectHiddenText, Off

pid:=6400

; freerdp /u:administrator /p:111111  /cert-ignore /sec:nla /log-level:trace /size:800x500 /v:192.168.204.233

clipboard =  cmd.exe /c ping 127.0.0.1 ; 在剪贴板中存入新内容.
WinActivate, FreeRDP
; FreeRDP在一次命令注入完成后需要立即关闭，否则可能发生键盘粘滞发送问题；
;ControlSend, , #r , ahk_pid %pid%
SendInput, ^ ; 刷新键盘缓冲区，避免奇怪的问题
Sleep, 600

SendInput, #r
Sleep, 500

;ControlSend, , ^V , ahk_pid %pid%  ; ctrl+V
SendInput, ^V 

Sleep, 500
;ControlSend, , {Enter}, ahk_pid %pid%
SendInput, {Enter} 
Sleep, 500


ExitApp 

