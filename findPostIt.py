import numpy as np
import matplotlib.pyplot as plt
import cv2
import sys
MIN_MATCH_COUNT = int(sys.argv[3])

wall_img = sys.argv[1]
postit_img = sys.argv[2]

wall = cv2.imread(wall_img, 0)
postit = cv2.imread(postit_img, 0)
w, h = postit.shape

# rotation test
# rot = cv2.getRotationMatrix2D((int(h/2), int(w/2)), 30, 1)
# postit = cv2.warpAffine(postit, rot, (h, w))
                              
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
    if m.distance < float(sys.argv[4])*n.distance:
        good.append(m)

if len(good)>MIN_MATCH_COUNT:
    src_pts = np.float32([ kp_w[m.queryIdx].pt for m in good ]).reshape(-1,1,2)
    dst_pts = np.float32([ kp_p[m.trainIdx].pt for m in good ]).reshape(-1,1,2)

    M, mask = cv2.findHomography(src_pts, dst_pts, cv2.RANSAC,5.0)
    matchesMask = mask.ravel().tolist()

    h,w = wall.shape
    pts = np.float32([ [0,0],[0,h-1],[w-1,h-1],[w-1,0] ]).reshape(-1,1,2)
    dst = cv2.perspectiveTransform(pts,M)

    postit = cv2.polylines(postit,[np.int32(dst)],True,255,3, cv2.LINE_AA)

else:
    print "Not enough matches are found - %d/%d" % (len(good),MIN_MATCH_COUNT)
    matchesMask = None

draw_params = dict(matchColor = (0,255,0),
                   singlePointColor = None,
                   matchesMask = matchesMask,
                   flags = 2)

res = cv2.drawMatches(wall,kp_w,postit,kp_p,good,None,**draw_params)
fig, ax = plt.subplots(2,1)
ax = ax.flatten()
ax[0].imshow(postit, 'gray')
ax[1].imshow(res, 'gray')
plt.show()
