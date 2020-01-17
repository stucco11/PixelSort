![Build](https://img.shields.io/badge/Release%20Version-0.1.1--alpha-blue)

# PixelSort

For those who don't know, PixelSorting is exactly what it sounds like: sorting pixels based on different criteria in a number of different ways. For instance, a script that does this would scrub through a photo line by line and then sort the pixels in the line based on different things, such as hue, luminosity, transparency, ect. For some examples of this, check out these scripts if you are so inclined: 

* [Pixelsort.me](http://www.pixelsort.me/)
* [Jeff Thompson](https://github.com/jeffThompson/PixelSorting)

PixelSorting is something that I've been interested in for a while, but I haven't yet found a program that has a convenient GUI to enable users a simple way to do these pixel sorts. I decided to just make one myself! This program is very much a work in progress, but I'm hoping to finish it quickly.

## Currently Available Sorting Methods

* Brightness - Evaluates the brightness of each pixel and sorts in order from 0 to 1
  * The user is able to manually set the upper and lower bounds for filtering out specific brightness values
* Hue - Goes in the order of Red, Orange, Yellow, Green, Blue, Violet
* Saturation - Evaluates the saturation of each pixel and sorts in order from 0 to 1
* RGB - Sorts pixels based on the respective amounts of Red, Green, or Blue in the row (whether the program sorts by Reds, Greens, or Blues is specified by the user)

*Able to be applied to every sorting method*

* Extending - this sorts the image as though it was a single line of pixels
* Spiral Sort - this recursively sorts the enclosing perimeter of the image as though it was a singular line before stopping at the middle
* Partitioning (*Vertical and Horizontal*) - this will partition the image by a number of user defined lines. This allows the program to sort each partition as though it were an individual image before stitching everything back together 
* Direction - this allows the user to specify which direction the want the pixels in the image to be sorted in. The available options are right to left, left to right, top to bottom, and bottom to top

## Examples

This image had the Brightness Sorting method applied with lower and upper bounds.

<img src="https://user-images.githubusercontent.com/7595163/72641311-7527a500-392f-11ea-9f6a-070040b79357.jpg" width="200">

This image had the Brightness Sorting method applied, 3 horizontal partitions and 1 vertical partition, and the spiral function checked.

<img src="https://user-images.githubusercontent.com/7595163/72641443-d8b1d280-392f-11ea-85ee-7048753091de.jpg" width="200">

## How to Download

You can find the most resent release [here](https://github.com/stucco11/PixelSort/releases/tag/0.1.0-alpha).

**Please note that PixelSort is still in Alpha. It does run properly but is still being actively worked on.**

## Acknowledgments

* [This sample code was used to enable the form switching](https://www.technical-recipes.com/2018/navigating-between-views-in-wpf-mvvm/)
