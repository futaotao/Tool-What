#!/system/bin/sh

unset LD_PRELOAD

vbox_graph_mode="800x600-16"
vbox_dpi="160"
vbox_sdcard_drive="/dev/block/sdc"

# Disable cursor blinking - Thanks android-x86 :-)
echo -e '\033[?17;0;0c' > /dev/tty0

prop_hardware_opengl=`/system/bin/androVM-prop get hardware_opengl`
# Starting eth0 management
# First check if eth0 is 'plugged'
if [ $prop_hardware_opengl ]; then
  /system/bin/netcfg eth0 up
  CARRIER=`cat /sys/class/net/eth0/carrier`
  if [ $CARRIER -eq 1 ]; then
    /system/bin/netcfg eth0 dhcp
    IPETH0=(`ifconfig eth0`)
    IPMGMT=${IPETH0[2]}
    /system/bin/androVM-prop set androvm_ip_management $IPMGMT
    echo "IP Management : $IPMGMT" > /dev/tty0
  else
    /system/bin/androVM-prop set androvm_ip_management 0.0.0.0
    echo "eth0 interface is not connected" > /dev/tty0
  fi
else
  (
    /system/bin/netcfg eth0 dhcp
    IPETH0=(`ifconfig eth0`)
    IPMGMT=${IPETH0[2]}
    /system/bin/androVM-prop set androvm_ip_management $IPMGMT
    echo "IP Management : $IPMGMT" > /dev/tty0
  )&
fi

# Load parameters from virtualbox guest properties

prop_vbox_graph_mode=`/system/bin/androVM-prop get vbox_graph_mode`
if [ -n "$prop_vbox_graph_mode" ]; then
  vbox_graph_mode="$prop_vbox_graph_mode"
  setprop androVM.vbox_graph_mode "$prop_vbox_graph_mode"
fi

prop_vbox_dpi=`/system/bin/androVM-prop get vbox_dpi`
if [ -n "$prop_vbox_dpi" ]; then
  vbox_dpi="$prop_vbox_dpi"
  setprop androVM.vbox_dpi "$prop_vbox_dpi"
fi

prop_vbox_sdcard_drive=`/system/bin/androVM-prop get vbox_sdcard_drive`
if [ -n "$prop_vbox_sdcard_drive" ]; then
  vbox_sdcard_drive="$prop_vbox_sdcard_drive"
  setprop androVM.vbox_sdcard_drive "$prop_vbox_sdcard_drive"
fi

prop_vkeyboard_mode=`/system/bin/androVM-prop get vkeyboard_mode`
if [ -n "$prop_vkeyboard_mode" ]; then
  vkeyboard_mode="$prop_vkeyboard_mode"
  setprop androVM.vkeyboard_mode "$prop_vkeyboard_mode"
fi

prop_force_navbar=`/system/bin/androVM-prop get genymotion_force_navbar`
if [ -n "$prop_force_navbar" -a "$prop_force_navbar" == "1" ]; then
  # No hw buttons => add virtual navbar
  setprop qemu.hw.mainkeys 0
fi

prop_su_bypass=`/system/bin/androVM-prop get su_bypass`
if [ $prop_su_bypass ]; then
  setprop genyd.su.bypass 1
fi

prop_player_version=`/system/bin/androVM-prop get genymotion_player_version`
if [ -n "$prop_player_version" ]; then
  setprop ro.genymotion.player.version "$prop_player_version"
fi

prop_device_version=`getprop ro.genymotion.device.version`
if [ -n "$prop_device_version" ]; then
  /system/bin/androVM-prop set genymotion_device_version $prop_device_version
fi

# Setting Device Id system properties from VirtualBox properties
prop_device_id=$(/system/bin/androVM-prop get genymotion_device_id)
if [ $? -ne 0 ]; then
  # Default value if unset
setprop genyd.device.id "860126350435998"
else
  # Set user defined value. "[none]" keyword means empty value
setprop genyd.device.id "860126350435998"
fi

insmod /system/lib/cfbcopyarea.ko
insmod /system/lib/cfbfillrect.ko
insmod /system/lib/cfbimgblt.ko
insmod /system/lib/uvesafb.ko mode_option=$vbox_graph_mode scroll=redraw

setprop ro.sf.lcd_density $vbox_dpi

if [ $prop_hardware_opengl ]; then
  if [ $IPMGMT ]; then
    setprop androVM.gles 1
    prop_hardware_opengl_disable_render=`/system/bin/androVM-prop get hardware_opengl_disable_render`
    if [ -z "$prop_hardware_opengl_disable_render" -o "$prop_hardware_opengl_disable_render" != "1" ]; then
      setprop androVM.gles.renderer 1
    fi
  else
    echo "eth0 is not configured correctly - HARDWARE OPENGL IS DISABLED !!!"  > /dev/tty0
    sleep 10
  fi
fi

# SDCARD
if [ -b $vbox_sdcard_drive ]; then
  echo "Trying to mount $vbox_sdcard_drive" > /dev/tty0
  mount -t vfat -o fmask=0000,dmask=0000 $vbox_sdcard_drive /mnt/shell/emulated
  if [ $? -ne 0 ]; then
    echo "Unable to mount $vbox_sdcard_drive, try to create FAT" > /dev/tty0
    newfs_msdos $vbox_sdcard_drive
    mount -t vfat -o fmask=0000,dmask=0000 $vbox_sdcard_drive /mnt/shell/emulated
    if [ $? -ne 0 ]; then
      echo "Unable to create FAT" > /dev/tty0
    fi
  fi
else
  echo "NO SDCARD" > /dev/tty0
fi

# ARM ABI
abi2_set=`getprop ro.product.cpu.abi2`
if [ ! $abi2_set ]; then
  if [ -f /system/lib/libhoudini.so ]; then
    setprop ro.product.cpu.abi2 armeabi-v7a
  fi
fi
if [ -f /system/lib/libhoudini.so ]; then
  setprop dalvik.vm.houdini on
fi

# Set Wifi MAC address
setprop wifi.interface.mac `cat /sys/class/net/eth1/address`

# Update system versions
android_version=`getprop ro.build.version.release`
genymotion_version=`getprop ro.genymotion.version`
/system/bin/androVM-prop set android_version "$android_version"
/system/bin/androVM-prop set genymotion_version "$genymotion_version"

# Add platform guestproperty and Android property
genymotion_platform=$(/system/bin/androVM-prop get genymotion_platform)
if [ -z "$genymotion_platform" ]; then
    genymotion_product_name=$(getprop ro.product.name.geny-def)
    if [ "$genymotion_product_name" == "vbox86tp" ]; then
        genymotion_platform="tp"
    elif [ "$genymotion_product_name" == "vbox86t" ]; then
        genymotion_platform="t"
    elif [ "$genymotion_product_name" == "vbox86p" ]; then
        genymotion_platform="p"
    else
        genymotion_platform="unknown"
    fi
    /system/bin/androVM-prop set genymotion_platform "$genymotion_platform"
fi
setprop ro.genymotion.platform "$genymotion_platform"

# Set ro.build.characteristics at boot
build_characteristics_property=ro.build.characteristics
if [ $genymotion_platform == "p" ]; then
    setprop $build_characteristics_property "default"
else
    setprop $build_characteristics_property "tablet"
fi

# We want to allow user to override values of android ro.* properties (read-only)
# So we first check the value of a guest_property for this property
# If it set, we use it as the android property
# If not, we use the default value, that we mirrored before in a shadow_property
set_prop_from_guest_property() {
    local android_property="$1"
    local shadow_property="$2"
    local guest_property="$3"
    local value=$(/system/bin/androVM-prop get $guest_property)
    if [ -z "$value" ] ; then
        value=$(getprop $shadow_property)
        /system/bin/androVM-prop set $guest_property "$value"
    fi
    setprop $android_property "$value"
}

set_prop_from_guest_property "ro.product.name" "ro.product.name.geny-def" "product_name"
set_prop_from_guest_property "ro.product.manufacturer" "ro.manufacturer.geny-def" "product_manufacturer"
set_prop_from_guest_property "ro.product.device" "ro.product.device.geny-def" "product_device"
set_prop_from_guest_property "ro.product.board" "ro.product.board.geny-def" "product_board"
set_prop_from_guest_property "ro.product.brand" "ro.product.brand.geny-def" "product_brand"
set_prop_from_guest_property "ro.build.display.id" "ro.build.display.id.geny-def" "build_display_id"

# Set ro.product.model
# ro.product.model is different from the other ro.product.* properties
# The value of ro.product.model is based on, by priority :
# - The vm property product_model, used to override the product model (done by the user)
# - The vm property genymotion_vm_name, which is set by the Genymotion Player before the first boot
# - The value ro.genymotion.product.model we stored in /system/build.prop,
#   that shadows the value of ro.product.model set during the build
product_model=$(/system/bin/androVM-prop get product_model)
if [ -z "$product_model" ]; then
    product_model=$(/system/bin/androVM-prop get genymotion_vm_name)
    if [ -z "$product_model" ]; then
        product_model=$(getprop ro.product.model.geny-def)
    fi
    /system/bin/androVM-prop set product_model "$product_model"
fi
setprop ro.product.model "$product_model"

# Camera
prop_camera=$(/system/bin/androVM-prop get genymotion_camera)
if [ -n "$prop_camera" ]; then
  setprop ro.genymotion.cameras "$prop_camera"
fi

# We are done
setprop androVM.inited 1
