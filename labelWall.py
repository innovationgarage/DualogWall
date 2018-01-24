# base code from http://opencv-python-tutroals.readthedocs.io/en/latest/py_tutorials/py_feature2d/py_feature_homography/py_feature_homography.html
import numpy as np
import matplotlib.pyplot as plt
import cv2
import sys
import os
import pdb

def find_postit(wall_img, postit_img, MIN_MATCH_COUNT, uniqueness):
    wall_color = cv2.imread(wall_img)
    wall = cv2.cvtColor(wall_color, cv2.COLOR_BGR2GRAY)
    postit_color = cv2.imread(os.path.join('crops', postit_img))
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
    #    pdb.set_trace()    

    good = []
    for m,n in matches:
        if m.distance < uniqueness*n.distance:
            good.append(m)

    if len(good)>MIN_MATCH_COUNT:
        src_pts = np.float32([ kp_w[m.queryIdx].pt for m in good ]).reshape(-1,1,2)
        dst_pts = np.float32([ kp_p[m.trainIdx].pt for m in good ]).reshape(-1,1,2)

        b_x, b_y, b_w, b_h = cv2.boundingRect(src_pts)
        top_left = (b_x-1, int(b_y-1 + postit_w-1))
        bottom_right = (int(b_x-1 + postit_h-1), b_y-1)

        res = cv2.rectangle(wall, top_left, bottom_right, (0,0,0), 3)

        # M, mask = cv2.findHomography(src_pts, dst_pts, cv2.RANSAC,5.0)
        # matchesMask = mask.ravel().tolist()

        # h,w = wall.shape
        # pts = np.float32([ [0,0],[0,h-1],[w-1,h-1],[w-1,0] ]).reshape(-1,1,2)
        # dst = cv2.perspectiveTransform(pts,M)
        # box = cv2.polylines(postit_color,[np.int64(dst)],True,255,4, cv2.LINE_AA)

        print "good matches for  %s: %s"%(postit_img, len(good))

    else:
        print "Not enough matches are found in %s- %d/%d" %(postit_img, len(good),MIN_MATCH_COUNT)
        matchesMask = None
        res = wall
        
    # draw_params = dict(matchColor = (255,255,255),
    #                    singlePointColor = None,
    #                    matchesMask = matchesMask,
    #                    flags = 2)
    # res = cv2.drawMatches(res,kp_w,box,kp_p,good,None,**draw_params)

    return res
    
wall_img = 'wallNow.jpg'
res_path = 'res/'

# MIN_MATCH_COUNTs = range(5, 30, 5)
# uniquenesses = np.arange(0.1, 0.9, 0.2)

#empirically best parameters
MIN_MATCH_COUNTs = [2]
uniquenesses = [0.1]

for postit_img in os.listdir('crops/'):
    fig, ax = plt.subplots(len(MIN_MATCH_COUNTs), len(uniquenesses), figsize=(20,20))
    if len(MIN_MATCH_COUNTs)*len(uniquenesses)==1:
        for i, min_match_count in enumerate(MIN_MATCH_COUNTs):
            for j, uniqueness in enumerate(uniquenesses):
                res = find_postit(wall_img, postit_img, min_match_count, uniqueness)
                ax.imshow(res, 'gray')
                ax.set_title("%s - %s"%(min_match_count, uniqueness))
    else:
        ax = ax.flatten()
        c = 0
        for i, min_match_count in enumerate(MIN_MATCH_COUNTs):
            for j, uniqueness in enumerate(uniquenesses):
                res = find_postit(wall_img, postit_img, min_match_count, uniqueness)
                ax[c].imshow(res, 'gray')
                ax[c].set_title("%s - %s"%(min_match_count, uniqueness))
                c += 1
    plt.savefig(os.path.join(res_path, postit_img))
    
