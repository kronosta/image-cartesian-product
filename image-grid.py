from PIL import Image
import sys

def image_grid(imgs, rows, cols):
    assert len(imgs) == rows * cols

    w, h = imgs[0].size
    grid = Image.new('RGB', size=(cols * w, rows * h))
    grid_w, grid_h = grid.size

    for i, img in enumerate(imgs):
        grid.paste(img, box=(i % cols * w, i // cols * h))
    return grid

if len(sys.argv) < 4:
    print("""
Usage:
python image-grid.py [prefix] [width] [height] [output]

Prefix should be the file prefix, the file names used will be [prefix]-[z]-[w].png
Width and height is the width and height of the image grid and the number after the last z and w coordinates
    in the filenames
Output should be the name of the output grid file
""")
else:
    prefix = sys.argv[1]
    width = int(sys.argv[2])
    height = int(sys.argv[3])
    output = sys.argv[4]

    imgs = []
    for i in range(width):
        for j in range(height):
            imgs.append(Image.open(prefix + "-" + str(i) + "-" + str(j) + ".png"))
    result = image_grid(imgs, width, height)
    result.save(output)

