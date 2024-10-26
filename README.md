# image-cartesian-product
Take a sort of colored Cartesian product to get 4-dimensional image slices out of two component images. Note that the "verbose" option is really slow since printing is expensive; without the option it takes about 5 seconds on my mid-level gaming PC, with it the program took 13 minutes.

Rotation should be integers in degrees

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

# Image grids
If you download the image-grid.py file and put it where your output images are, you can run `python image-grid.py` to get a usage page. This allows you to turn the slice images into a grid quickly and conveniently
