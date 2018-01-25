# base code from https://www.pyimagesearch.com/2014/05/26/opencv-python-k-means-color-clustering/
import numpy as np
import cv2
import matplotlib.pyplot as plt
plt.rcParams.update({'figure.max_open_warning': 0})
from sklearn.cluster import KMeans
import sys
import os
import pdb
import scipy.misc
import PIL
from PIL import ImageFont
from PIL import Image
from PIL import ImageDraw

def centroid_histogram(clt):
    # grab the number of different clusters and create a histogram
    # based on the number of pixels assigned to each cluster
    numLabels = np.arange(0, len(np.unique(clt.labels_)) + 1)
    (hist, _) = np.histogram(clt.labels_, bins = numLabels)

    # normalize the histogram, such that it sums to one
    hist = hist.astype("float")
    hist /= hist.sum()

    # return the histogram
    return hist

def plot_colors(hist, centroids):
    # initialize the bar chart representing the relative frequency
    # of each of the colors
    bar = np.zeros((50, 300, 3), dtype = "uint8")
    startX = 0

    first_color = np.array([])
    highest_percent = 0.

    # loop over the percentage of each cluster and the color of
    # each cluster
    for (percent, color) in zip(hist, centroids):
        # plot the relative percentage of each cluster
        endX = startX + (percent * 300)
        cv2.rectangle(bar, (int(startX), 0), (int(endX), 50),
                      color.astype("uint8").tolist(), -1)
        startX = endX

        if percent>highest_percent:
            first_color = color
            highest_percent = percent

    # return the bar chart, and the most frequent color
    return bar, first_color.astype("uint8")

def get_color(img_name, img_path, hr_path):
    image = cv2.imread(os.path.join(img_path, img_name))
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

    h, w, _ = image.shape
    w_new = int(100 * w / max(w, h) )
    h_new = int(100 * h / max(w, h) )
    image = cv2.resize(image, (w_new, h_new))
    
    image_array = image.reshape((image.shape[0] * image.shape[1], 3))
    
    clt = KMeans(n_clusters = 3)
    clt.fit(image_array)

    # build a histogram of clusters and then create a figure
    # representing the number of pixels labeled to each color
    hist = centroid_histogram(clt)
    bar, first_color = plot_colors(hist, clt.cluster_centers_)

    bg = np.zeros_like(image)
    bg = bg + first_color

    return image, bar, bg

img_path = 'crop_image/'
txt_path = 'crop_text/'
hr_path = 'postit_hr/'

def generate_postit(img_name, img_path, txt_path):
    print img_name
    image, bar, bg = get_colors(img_name, img_path)
    with open(os.path.join(txt_path, '%s.txt'%(img_name.split('.')[0])), 'r') as f:
        img_txt = f.read()

    
for img_name in os.listdir(img_path):
    print img_name
    image, bar, bg = get_color(img_name, img_path, hr_path)
    with open(os.path.join(txt_path, '%s.txt'%(img_name.split('.')[0])), 'r') as f:
        img_txt = f.read()

    # show our color bar
    fig, axs = plt.subplots(1, 3)
    axs = axs.flatten()
    axs[0].imshow(image)
    axs[1].imshow(bar)
    axs[2].imshow(bg)
    axs[2].text(0.5, 0.3, img_txt,
                fontsize=12,
                horizontalalignment='center',
                verticalalignment='center',
                transform=axs[2].transAxes)
    plt.savefig(os.path.join('colors/', img_name))
    
    new_postit = Image.fromarray(bg)
#    new_postit.save(os.path.join(hr_path, img_name), quality=100)

    #add text
    W, H = new_postit.size
    draw = ImageDraw.Draw(new_postit)
    w, h = draw.textsize(img_txt)
    draw.text(((W-w)/2,(H-h)/2), img_txt, fill="black")

    new_postit.save(os.path.join(hr_path, img_name), quality=100)
    
