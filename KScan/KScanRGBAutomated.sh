#!/bin/sh
#Consider using pipes here

filename=$1

mkdir "KScan/${filename}"

python3 KScan/PCDConverterRGB.py "${filename}"

cp "${filename}".pcd "KScan/${filename}/${filename}.pcd"

mv "${filename}"_conv_RGB.pcd "KScan/${filename}/${filename}"_conv_RGB.pcd

KScan/convert_pcd_ascii_binary "KScan/${filename}/${filename}"_conv_RGB.pcd "KScan/${filename}/${filename}"_binary_RGB.pcd 1
    
KScan/pcd2ply "KScan/${filename}/${filename}"_binary_RGB.pcd "PointcloudRendering/Assets/pointcloud.ply"

printf "\n[DONE] Files Uploaded to Unity"