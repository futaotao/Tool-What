@echo off
set basepath=%~dp0
set device=%1%

if %device% equ "" (
	adb shell < %basepath%temp/pm.txt
) else (
	adb -s %device% shell < %basepath%temp/pm.txt
)