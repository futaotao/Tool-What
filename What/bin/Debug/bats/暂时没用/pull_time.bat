@echo off
set target=%1
adb pull /sdcard/time.txt %target%
