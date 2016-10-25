@echo off
set basepath=%~dp0
set device=%1
set screen=%2
if %device% equ "" (
adb push %basepath%anjian/%screen%/share_float_view_file.xml /data/data/com.cyjh.mobileanjian/shared_prefs
) else (
adb -s %device% push %basepath%anjian/%screen%/share_float_view_file.xml /data/data/com.cyjh.mobileanjian/shared_prefs
)