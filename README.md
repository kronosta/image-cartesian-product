# image-cartesian-product
Take a sort of colored Cartesian product to get 4-dimensional image slices out of two component images

# Usage
```
ImageCartesianProduct [rotation] [image1] [image2] [output-prefix]
ImageCartesianProduct delete [output-prefix]
ImageCartesianProduct verbose [rotation] [image1] [image2] [output-prefix]

Rotation should be 6 comma-separated numbers in degrees, to rotate the 6 4-dimensional planes.
[1st]: xy
[2nd]: xz
[3rd]: xw
[4th]: yz
[5th]: yw
[6th]: zw

Image1 and image2 should be file paths to PNG images. For best speed they should be small in width and height,
50x50 is good for example.

Output prefix should be a file path without '.png' at the end, output images will be named
[output-prefix]-[axis1-value]-[axis2-value].png

* delete will delete all .png files with the prefix specified
* verbose is like the standard command but it prints out all the trig results for debug purposes
```
