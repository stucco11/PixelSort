![Status](https://img.shields.io/badge/Status-Incomplete-yellow)
![Build](https://img.shields.io/badge/Build-Unavailable-blue)

# PixelSort

For those who don't know, PixelSorting is exactly what it sounds like: sorting pixels based on different criteria in a number of different ways. For instance, a script that does this would scrub through a photo line by line and then sort the pixels in the line based on different things, such as hue, luminosity, transparency, ect. For some examples of this, check out these scripts if you are so inclined: 

* [Pixelsort.me](http://www.pixelsort.me/)
* [Jeff Thompson](https://github.com/jeffThompson/PixelSorting)

PixelSorting is something that I've been interested in for a while, but I haven't yet found a program that has a convenient GUI to enable users a simple way to do these pixel sorts. I decided to just make one myself! This program is very much a work in progress, but I'm hoping to finish it quickly.

## Currently Available Sorting Methods

* Brightness - Evaluates the brightness of each pixel and sorts in order from 0 to 1
* Hue - Goes in the order of Red, Orange, Yellow, Green, Blue, Violet
* Saturation - Evaluates the saturation of each pixel and sorts in order from 0 to 1
* RGB - the user is able to select whether to sort pixels based on the respective amounts of Red, Green, or Blue in the row

*Able to be applied to every sorting method*

* Extending - this sorts the image as though it was a single line of pixels
* Spiral Sort - this recursively sorts the enclosing perimeter of the image as though it was a singular line before stopping at the middle
* Partitioning (*Vertical and Horizontal*) - this will partition the image by a number of user defined lines. This allows the program to sort each partition as though it were an individual image before stitching everything back together 

## Acknowledgments

* [This sample code was used to enable the form switching](https://www.technical-recipes.com/2018/navigating-between-views-in-wpf-mvvm/)
