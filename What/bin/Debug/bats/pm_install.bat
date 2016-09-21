@echo off
set basepath=%~dp0
set device=%1%

adb shell < %basepath%pm_install.txt

