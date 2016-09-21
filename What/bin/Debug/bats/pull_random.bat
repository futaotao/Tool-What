@echo off
set target=%1
adb pull /sdcard/random %target%
