@echo off
set basepath=%~dp0
adb shell < %basepath%delete_time.txt
