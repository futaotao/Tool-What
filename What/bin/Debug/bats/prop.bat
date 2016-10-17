@echo off
set basepath=%~dp0
set device=%1%
set propPath=%2%
if %device% equ "" (
	adb shell < %basepath%rw.txt
	adb push %propPath% /system
	adb pull /system/etc/init.androVM.sh %basepath%temp
	java -jar %basepath%imei.jar %basepath%temp/init.androVM.sh
	adb push %basepath%temp/init.androVM.sh /system/etc
	adb shell < %basepath%ro.txt
) else (
	adb -s %device% shell < %basepath%rw.txt
	adb -s %device% push %propPath% /system
	adb -s %device% pull /system/etc/init.androVM.sh %basepath%temp
	java -jar %basepath%imei.jar %basepath%temp/init.androVM.sh
	adb -s %device% push %basepath%temp/init.androVM.sh /system/etc
	adb -s %device% shell < %basepath%ro.txt
)
