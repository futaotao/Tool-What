@echo off
set device=%1
set target=%2
if %device% equ "" (
adb pull /data/data/com.cyjh.mobileanjian/shared_prefs/share_float_view_file.xml %target%
) else (
adb -s %device% pull /data/data/com.cyjh.mobileanjian/shared_prefs/share_float_view_file.xml %target%
)