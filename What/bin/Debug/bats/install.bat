echo off
set device=%1
set apkPath=%2
if %device% equ "" ( 
	adb install -r %apkPath%
) else (
	echo %device%
	adb -s %device% install -r %apkPath%
)