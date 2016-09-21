@echo off
set device="%1"
if %device% equ "" (
	adb reboot
) else (
	adb -s %device% reboot
)