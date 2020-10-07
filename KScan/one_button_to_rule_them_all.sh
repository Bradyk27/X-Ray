#!/bin/bash

# Assign the variables. #Obvioulsy variables need to be updated to refelct actual machine & folder. #also might consider only accepting pointcloud files instead of anything
USER='brady';
HOST='10.42.0.1';
LOCAL_PATH='~/Desktop/';
REMOTE_PATH='~/catkin_ws/';
TIME=.01;

while :
do
  # Get the most recent file and assign it to `RECENT`.
  RECENT=$(ssh ${USER}@${HOST} ls -lrt ${REMOTE_PATH} | awk '/.pcd/ { f=$NF }; END { print f }');

  # Run the actual SCP command.
  scp ${USER}@${HOST}:${REMOTE_PATH}${RECENT} pointcloud.pcd;
  ./KScanRGBAutomated.sh pointcloud
  sleep $TIME;
 done