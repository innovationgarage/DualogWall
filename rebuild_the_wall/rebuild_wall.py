# base code for feature-matching is from http://opencv-python-tutroals.readthedocs.io/en/latest/py_tutorials/py_feature2d/py_feature_homography/py_feature_homography.html
import numpy as np
import matplotlib.pyplot as plt
plt.rcParams.update({'figure.max_open_warning': 0})
import cv2
import sys
import os
import pdb
import scipy.misc
from PIL import Image
from shutil import copyfile

def find_postit(wall_img, postit_img, postit_path, MIN_MATCH_COUNT, uniqueness):
    wall_color = cv2.imread(wall_img)
    wall = cv2.cvtColor(wall_color, cv2.COLOR_BGR2GRAY)
    postit_color = cv2.imread(os.path.join(postit_path, postit_img))
    postit = cv2.cvtColor(postit_color, cv2.COLOR_BGR2GRAY)
    postit_w, postit_h = postit.shape
    
    sift = cv2.xfeatures2d.SIFT_create()
    
    kp_w, des_w = sift.detectAndCompute(wall,None)
    kp_p, des_p = sift.detectAndCompute(postit,None)
    
    FLANN_INDEX_KDTREE = 0
    index_params = dict(algorithm = FLANN_INDEX_KDTREE, trees = 5)
    search_params = dict(checks = 50)

    flann = cv2.FlannBasedMatcher(index_params, search_params)
    matches = flann.knnMatch(des_w,des_p,k=2)

    good = []
    for m,n in matches:
        if m.distance < uniqueness*n.distance:
            good.append(m)

    if len(good)>MIN_MATCH_COUNT:
        src_pts = np.float32([ kp_w[m.queryIdx].pt for m in good ]).reshape(-1,1,2)
        dst_pts = np.float32([ kp_p[m.trainIdx].pt for m in good ]).reshape(-1,1,2)

        b_x, b_y, b_w, b_h = cv2.boundingRect(src_pts)
        top_left = (b_x, int(b_y + postit_w))
        bottom_right = (int(b_x + postit_h), b_y)

        res = cv2.rectangle(wall, top_left, bottom_right, (0,0,0), 3)

        print "good matches for  %s: %s"%(postit_img, len(good))

    else:
        print "Not enough matches are found in %s- %d/%d" %(postit_img, len(good),MIN_MATCH_COUNT)
        matchesMask = None
        res = wall
        
    return res, (b_x, b_y)

## base code from https://docs.opencv.org/3.2.0/d0/d86/tutorial_py_image_arithmetics.html
def replace_postit(postit_name, pos, wall_pil, src_path):
    wall = np.array(wall_pil)
    src = cv2.imread(os.path.join(src_path, postit_name))
    src = cv2.cvtColor(src, cv2.COLOR_BGR2RGB)
    
    x_offset = pos[0]
    y_offset = pos[1]

    wall[y_offset:y_offset+src.shape[0], x_offset:x_offset+src.shape[1]] = src
    return wall

if __name__ == "__main__":
	wall_img = 'wallNow.jpg'
        wall_img = str(sys.argv[1])
	res_path = 'res/'
	postit_path = 'crop_image/'
	hr_wall_name = 'hr_wall.jpg'
        hr_wall_name = str(sys.argv[2])
	hr_path = 'postit_hr/'

	copyfile(wall_img, hr_wall_name)
	# MIN_MATCH_COUNTs = range(5, 30, 5)
	# uniquenesses = np.arange(0.1, 0.9, 0.2)

	#empirically best parameters as long as there are no duplicate post-its
	MIN_MATCH_COUNTs = [2]
	uniquenesses = [0.1]
	added_pixels = 100
	wall_tmp = cv2.imread(hr_wall_name)
	wall_tmp = cv2.cvtColor(wall_tmp, cv2.COLOR_BGR2RGB)
	wall_arr = np.zeros((wall_tmp.shape[0], wall_tmp.shape[1] + added_pixels, wall_tmp.shape[2]), dtype=np.uint8)
	wall_arr[:,:-added_pixels,:] = wall_tmp
	wall_pil = Image.fromarray(wall_arr)

	for postit_img in os.listdir(hr_path):
	    fig, ax = plt.subplots(len(MIN_MATCH_COUNTs), len(uniquenesses), figsize=(20,20))
	    if len(MIN_MATCH_COUNTs)*len(uniquenesses)==1:
		for i, min_match_count in enumerate(MIN_MATCH_COUNTs):
		    for j, uniqueness in enumerate(uniquenesses):
		        res, pos = find_postit(wall_img, postit_img, postit_path, min_match_count, uniqueness)

		        wall_pil = Image.fromarray(wall_arr)
		        wall_arr = replace_postit(postit_img, pos, wall_pil, hr_path)
		        wall_pil = Image.fromarray(wall_arr)

		        ax.imshow(res, 'gray')
		        ax.set_title("%s - %s"%(min_match_count, uniqueness))
	    else:
		ax = ax.flatten()
		c = 0
		for i, min_match_count in enumerate(MIN_MATCH_COUNTs):
		    for j, uniqueness in enumerate(uniquenesses):
		        res = find_postit(wall_img, postit_img, postit_path, min_match_count, uniqueness)
		        ax[c].imshow(res, 'gray')
		        ax[c].set_title("%s - %s"%(min_match_count, uniqueness))
		        c += 1
	    plt.savefig(os.path.join(res_path, postit_img))
	wall_pil.save(os.path.join('.', hr_wall_name), quality=100)
	    
