@echo off
set device=%1
set target=%2
if %device% equ "" (
adb pull /sdcard/random %target%
) else (
adb -s %device% pull /sdcard/random %target%
)