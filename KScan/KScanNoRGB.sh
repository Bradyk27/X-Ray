#!/bin/sh

filename=$1

mkdir "${filename}"

python3 PCDConverterNoRGB.py "${filename}"

cp "${filename}".pcd "${filename}/${filename}.pcd"

mv "${filename}"_conv_NORGB.pcd "${filename}/${filename}"_conv_NORGB.pcd

./convert_pcd_ascii_binary "${filename}/${filename}"_conv_NORGB.pcd "${filename}/${filename}"_binary_NORGB.pcd 1

./pcd2ply "${filename}/${filename}"_binary_NORGB.pcd "${filename}/${filename}"_NORGB.ply

printf "\n[DONE] All files found in "${filename}"\n"