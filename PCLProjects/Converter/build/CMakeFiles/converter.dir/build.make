# CMAKE generated file: DO NOT EDIT!
# Generated by "Unix Makefiles" Generator, CMake Version 3.16

# Delete rule output on recipe failure.
.DELETE_ON_ERROR:


#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:


# Remove some rules from gmake that .SUFFIXES does not remove.
SUFFIXES =

.SUFFIXES: .hpux_make_needs_suffix_list


# Suppress display of executed commands.
$(VERBOSE).SILENT:


# A target that is always out of date.
cmake_force:

.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

# The shell in which to execute make rules.
SHELL = /bin/sh

# The CMake executable.
CMAKE_COMMAND = /opt/local/bin/cmake

# The command to remove a file.
RM = /opt/local/bin/cmake -E remove -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter"

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/build"

# Include any dependencies generated for this target.
include CMakeFiles/converter.dir/depend.make

# Include the progress variables for this target.
include CMakeFiles/converter.dir/progress.make

# Include the compile flags for this target's objects.
include CMakeFiles/converter.dir/flags.make

CMakeFiles/converter.dir/converter.cpp.o: CMakeFiles/converter.dir/flags.make
CMakeFiles/converter.dir/converter.cpp.o: ../converter.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir="/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/build/CMakeFiles" --progress-num=$(CMAKE_PROGRESS_1) "Building CXX object CMakeFiles/converter.dir/converter.cpp.o"
	/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/converter.dir/converter.cpp.o -c "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/converter.cpp"

CMakeFiles/converter.dir/converter.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/converter.dir/converter.cpp.i"
	/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/converter.cpp" > CMakeFiles/converter.dir/converter.cpp.i

CMakeFiles/converter.dir/converter.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/converter.dir/converter.cpp.s"
	/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/converter.cpp" -o CMakeFiles/converter.dir/converter.cpp.s

# Object files for target converter
converter_OBJECTS = \
"CMakeFiles/converter.dir/converter.cpp.o"

# External object files for target converter
converter_EXTERNAL_OBJECTS =

converter: CMakeFiles/converter.dir/converter.cpp.o
converter: CMakeFiles/converter.dir/build.make
converter: /usr/local/lib/libpcl_io.dylib
converter: /opt/local/lib/libboost_system-mt.dylib
converter: /opt/local/lib/libboost_filesystem-mt.dylib
converter: /opt/local/lib/libboost_date_time-mt.dylib
converter: /opt/local/lib/libboost_iostreams-mt.dylib
converter: /opt/local/lib/libboost_regex-mt.dylib
converter: /usr/local/lib/libpcl_octree.dylib
converter: /usr/local/lib/libpcl_common.dylib
converter: CMakeFiles/converter.dir/link.txt
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --bold --progress-dir="/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/build/CMakeFiles" --progress-num=$(CMAKE_PROGRESS_2) "Linking CXX executable converter"
	$(CMAKE_COMMAND) -E cmake_link_script CMakeFiles/converter.dir/link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
CMakeFiles/converter.dir/build: converter

.PHONY : CMakeFiles/converter.dir/build

CMakeFiles/converter.dir/clean:
	$(CMAKE_COMMAND) -P CMakeFiles/converter.dir/cmake_clean.cmake
.PHONY : CMakeFiles/converter.dir/clean

CMakeFiles/converter.dir/depend:
	cd "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/build" && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter" "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter" "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/build" "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/build" "/Users/admin/desktop/Research/Dr. Bethel/ROSResearch2/PCL Projects/Converter/build/CMakeFiles/converter.dir/DependInfo.cmake" --color=$(COLOR)
.PHONY : CMakeFiles/converter.dir/depend

