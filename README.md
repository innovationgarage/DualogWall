# DualogWall
Script to get the GoPro image, correct the lens distortion, sharpen it and upload the results to the website

# Setup Raspberry Pi to get the GoPro images

        sudo apt-get update
        sudo apt-get upgrade
        sudo apt-get install mono-complete network-manager

## Boot to console
`sudo raspi-config` start mode should be "console" not window manager/desktop or the other one

## Allow the network manager to control wifi
`sudo nano /etc/network/interfaces`

Comment, save and reboot

    #allow-hotplug wlan0
    #iface wlan0 inet manual
    #    wpa-conf /etc/wpa_supplicant/wpa_supplicant.conf
    
Disable dhcpcd for wlan0, `nano /etc/dhcpcd.conf`:

    denyinterfaces wlan0

# Test it
---

# Set a cron task to get the images
---
