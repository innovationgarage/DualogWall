# DualogWall
Script to get the GoPro image, correct the lens distortion, sharpen it and upload the results to the website

# Script for testing connectivity 
Connect to the camera wifi and run (change `CAMERAWIFINAME`):

        #!/bin/bash
        while :
        do
        wget -O - http://10.5.5.9/gp/gpControl/status
        #http://10.5.5.9/gp/gpMediaList > /dev/null
        sleep 10
        wget -O - http://10.5.5.9/gp/gpControl/command/wireless/ap/ssid?ssid=CAMERAWIFINAME > /dev/null
        echo Waiting ...
        sleep 30
        done

# Remove some clutter

        sudo apt-get remove libreoff* bluej claws* greenfoot minecraft* nodered scratch sonic-pi wolfram* java*
        sudo apt-get autoremove
        
# Setup Raspberry Pi to get the GoPro images
        
        sudo apt-get update
        sudo apt-get upgrade
        sudo apt-get install mono-complete imagemagick


## OpenCV
        sudo apt-get install build-essential cmake cmake-curses-gui pkg-config 
        sudo apt-get install libgdk-pixbuf2.0-dev libpango1.0-dev libcairo2-dev 
        sudo apt-get install libjpeg-dev libtiff5-dev libjasper-dev libpng12-dev libavcodec-dev libavformat-dev libswscale-dev libeigen3-dev libxvidcore-dev libx264-dev libgtk2.0-dev
        sudo apt-get install libatlas-base-dev gfortran
        wget -O opencv.zip https://github.com/opencv/opencv/archive/3.4.0.zip
        wget -O opencv_contrib.zip https://github.com/opencv/opencv_contrib/archive/3.4.0.zip 
        unzip opencv.zip
        unzip opencv_contrib.zip
        cd ~/opencv-3.4.0/
        mkdir build
        cd build
        cmake -D CMAKE_BUILD_TYPE=RELEASE -D CMAKE_INSTALL_PREFIX=/usr/local -D INSTALL_C_EXAMPLES=OFF -D BUILD_DOCS=OFF -D BUILD_PERF_TESTS=OFF -D BUILD_TESTS=OFF -D INSTALL_PYTHON_EXAMPLES=OFF -D OPENCV_EXTRA_MODULES_PATH=~/opencv_contrib-3.4.0/modules -D BUILD_EXAMPLES=OFF ..
        make
        sudo make install
        sudo ldconfig

## Increase swap




# Test it
---

# Set a cron task to get the images
---
