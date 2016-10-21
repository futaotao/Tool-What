@echo off
set basepath=%~dp0
set device=%1
if %device% equ "" (
adb shell < %basepath%delete_random.txt
) else (
adb -s %device% shell < %basepath%delete_random.txt
)