# DualogWall
Script to get the GoPro image, correct the lens distortion, sharpen it and upload the results to the website

# Script for testing connectivity 
Connect to the camera wifi and run (change `CAMERAWIFINAME`):

```bash
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
```

# Remove some clutter

        sudo apt-get remove libreoff* bluej claws* greenfoot minecraft* nodered scratch sonic-pi wolfram* java*
        sudo apt-get autoremove
        
# Setup Raspberry Pi to get the GoPro images
        
        sudo apt-get update
        sudo apt-get upgrade
        sudo apt-get install mono-complete imagemagick


## Increase swap
OpenCV may not compile if the swap is too small. You might get errors similar to:

```makefile
        c++: internal compiler error: Killed (program cc1plus)
        Please submit a full bug report,
        with preprocessed source if appropriate.
        See <file:///usr/share/doc/gcc-6/README.Bugs> for instructions.
        modules/python2/CMakeFiles/opencv_python2.dir/build.make:62: recipe for target 'modules/python2/CMakeFiles/opencv_python2.dir/__/src2/cv2.cpp.o' failed
        make[2]: *** [modules/python2/CMakeFiles/opencv_python2.dir/__/src2/cv2.cpp.o] Error 4
        CMakeFiles/Makefile2:7998: recipe for target 'modules/python2/CMakeFiles/opencv_python2.dir/all' failed
        make[1]: *** [modules/python2/CMakeFiles/opencv_python2.dir/all] Error 2
        Makefile:160: recipe for target 'all' failed
        make: *** [all] Error 2
```

Increase the size to 512 or more here:

        sudo nano /etc/dphys-swapfile

## OpenCV
From: https://www.pyimagesearch.com/2015/12/14/installing-opencv-on-your-raspberry-pi-zero/

        sudo apt-get install build-essential cmake pkg-config
        sudo apt-get install libjpeg-dev libtiff5-dev libjasper-dev libpng12-dev
        sudo apt-get install libavcodec-dev libavformat-dev libswscale-dev libv4l-dev
        sudo apt-get install libxvidcore-dev libx264-dev
        sudo apt-get install libgtk2.0-dev
        sudo apt-get install libatlas-base-dev gfortran
        sudo apt-get install python2.7-dev
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



# Test it
---

# Set a cron task to get the images
---
