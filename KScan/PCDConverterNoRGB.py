import sys

file = open(str(sys.argv[1]) + ".pcd", "r")
file_new = open(str(sys.argv[1]) + "_conv_NORGB.pcd", "w")

lines = file.readlines()

file_new.write("# .PCD v0.7 - Point Cloud Data file format\nVERSION 0.7\nFIELDS x y z\nSIZE 4 4 4 \nTYPE F F F \nCOUNT 1 1 1 \n")
file_new.write(lines[6])
file_new.write("HEIGHT 1\n")
file_new.write(lines[9])
file_new.write("DATA ascii\n")

count = 0
for line in lines[11:]:
    count += 1
    temp = line.split(" ")
    file_new.write(temp[0] +  " " + temp[1] + " " + temp[2] + "\n")
    print("\rPercent Complete: ", "%.2f" % (count/len(lines[11:]) * 100), "%", end = "")






