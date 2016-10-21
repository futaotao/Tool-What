@echo off
set device=%1
set pkg=%2%
if %device% equ "" (
	adb uninstall %pkg%
) else (
	adb -s %device% uninstall %pkg%
)