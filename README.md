# X-Ray
Code, data, and other development work for an augmented reality window. Created by Brady Kruse as part of a project for Dr. J. Edward Swan and Dr. Cindy Bethel at Mississippi State University

## Abstract
In room-clearing tasks, SWAT team members suffer from alack  of  initial  environmental  information:  knowledge  about  what  is  in a room and what relevance or threat level it represents for mission parameters. Normally this gap in situation awareness is rectified only uponroom entry, forcing SWAT team members to rely on quick responses and near-instinctual reactions. This can lead to dangerously escalating situations or important missed information which, in turn, can increase the likelihood of injury and even mortality. Thus, we present an x-ray vision system for the dynamic scanning and display of room content, using a robotic platform to mitigate operator risk. This system maps a room using a robot-equipped stereo depth camera and, using an augmented reality (AR) system, presents the resulting geographic information accordingto the perspective of each officer. This intervention has the potential to notably lower risk and increase officer situation awareness, all while teammembers are in the relative safety of cover. With these potential stakes, it is  important to test the viability of this system natively  and in an operational SWAT team context

Phillips, N., Kruse, B., Khan, F. A., Swan II, J. E., & Bethel, C. L. (2020, July). A Robotic Augmented Reality Virtual Window for Law Enforcement Operations. In International Conference on Human-Computer Interaction (pp. 591-610). Springer, Cham.

## System

The X-Ray system is comprised of the following parts:

* An Ubuntu machine running ros-melodic and configured with the Intel Realsense d435 wrapper
* An Intel Realsense d435 depth camera connected to the Ubuntu machine via SS USB (This is important--a slower cable will not allow data to be generated)
* A computer running Unity with the ROS# package installed.
* The Magic Leap headset with development tools enabled as to compile and load the Unity project to it
* A network that allows open ports that all devices are connected to. Eduroam *does not* allow this. Use the CAVS router / network

## Set up

1. Install ros-melodic on the Unity machine.

2. Install the Intel Realsense d435 wrapper into the catkin_ws by following [this guide](https://github.com/IntelRealSense/realsense-ros) and then [this guide](https://github.com/IntelRealSense/realsense-ros/wiki/SLAM-with-D435i) or by cloning this git repo and building as necessary. Verify that the Realsense camera is working properly by running the example commands.

3. Install Unity onto the Windows machine.

4. Install Magic Leap tools & sample projects found [here](https://developer.magicleap.com/en-us/learn/guides/get-started-developing-in-unity)

   1. You can build a Unity project from scratch *or* copy one from the Alienware machine, flash drive, or this git repo (if I can add them. As of Dec. 12th, 2020 they *are not* yet added.)

5. Add the ROS# Unity package *DO NOT USE* the latest release. Use pull request #190 found [here](https://github.com/siemens/ros-sharp/pull/190)

6. If building your own project, configure it by following [these steps](https://github.com/siemens/ros-sharp/issues/184#issuecomment-664647025). If cloning one of mine or Nate's, you *shouldn't* need to do this, but if you run into errors start by verifying all of this.

7. On the Ubuntu machine, open a new terminal and run ifconfig. Note the local IP address.

8. In Unity, in the ROS Connector object, replace the IP with the Ubuntu machine IP. If you run into "Ros connector not connected errors," start by double-checking this. IP's can change.

9. Configure your Unity project to subscribe to the proper topics. More on this in the Usage section.

10. At this point, you should have a Unity machine configured to use ROS and the Intel Realsense d435, a Windows / Mac machine with Unity and the proper ROS# package, and a Magic Leap configured to run developmental Unity projects.

## Repo Breakdown

* KScan is an outdated method of capturing and transferring pointclouds from the Ubuntu machine to the Unity project, primarily through various shell commands. It can still be very useful for capturing example pointclouds, hence it's inclusion. See tutorial.txt for basic usage. Make sure to fork before any edits are made--it's very configurable but the base should be left alone.
* PCLProjects contains several builds of useful functions from the pcl library for converting pointclouds between formats. It's primarily used by the KScan program.
* Posters_Presentations & RelevantPapers are fairly self-explanatory. Backups of various presentations.
* catkin_ws is what your local ROS catkin_ws should resemble.

## Usage

1. Complete the Set up section *first*.

2. On the Ubuntu machine, open a new terminal and run:

        roslaunch realsense2_camera opensource_tracking.launch

   Note that the .launch file can be edited to open Rviz or not. Rviz is just for visualization--it will not affect the pointcloud transfer but could save computing power by not being run.

3. Open a new terminal tab and run:
   
        roslaunch rosbridge_server rosbridge_websocket.launch

   This will open the Rosbridge server. Incoming connections (like Unity or the Leap) will be listed as appropriate.

4. On the Unity project, choose the desired topics to subscribe to. All topics can be listed by running 
   
        rostopic list

   on the Ubuntu machine. Make sure to attach the topics to an appropriate Unity mesh (see example projects.) Also make sure to include the '/' in the name. You can also run 

        nmap -Pn <Ubuntu machine IP>

   to see if the correct ports are open for the Rosbridge server.

5. On the Unity project, press play to verify that all connections are working properly.

6. Build the Unity project for the Magic Leap, ensure that the Magic Leap is connected to the network, then run the compiled project on the Leap. This project can be started and stopped and still work properly *until and if* the Ubuntu IP changes, at which point the IP needs to be updated in the Rosconnector object in Unity and the project should be recompiled.

Congratulations. You have successfully transferred and rendered Ros topics wirelessly. 

## Future Directions & Ideas

* Dockerize this system and integrate it via Docker compose. Make it much more scalable and accesible. 
* Move to Hololens & the Mixed-Reailty Toolkit
* Write papers, conferences, etc.
* Move to robot, actually render a dynamic, legitmate environment.

## Brady ToDos

* Writing another paper would be fun
* Assist w/ triangulation experiments. Learn R better.
* Get the pointclouds to render dynamically
* Document, organize Git repos, write thorough Latex document w/ pictures
* Design & run memory, spatial awareness in virtual environment experiments
* Networking security, secure transfer