#!/bin/sh

filename=$1

mkdir "${filename}"

python3 PCDConverterRGB.py "${filename}"

cp "${filename}".pcd "${filename}/${filename}.pcd"

mv "${filename}"_conv_RGB.pcd "${filename}/${filename}"_conv_RGB.pcd

./convert_pcd_ascii_binary "${filename}/${filename}"_conv_RGB.pcd "${filename}/${filename}"_binary_RGB.pcd 1
    
./pcd2ply "${filename}/${filename}"_binary_RGB.pcd "${filename}/${filename}"_RGB.ply

printf "\n[DONE] All files found in "${filename}"\n"