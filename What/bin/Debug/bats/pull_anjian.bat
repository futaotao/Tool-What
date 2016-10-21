@echo off
set target=%1
adb pull /data/data/com.cyjh.mobileanjian/shared_prefs/share_float_view_file.xml %target%
