//
//  CameraPermission.m
//  SeeSo_Unity
//
//  Created by david on 2020/11/13.
//  Copyright © 2020 VisaulCamp. All rights reserved.
//

#import <UIKit/UIDevice.h>
#import <AVFoundation/AVCaptureDevice.h>

extern "C" {
    void requestIOSCameraPermission(){
        [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
            if(granted) {
                NSLog(@"granted");
            }
        }];
    }
    
    BOOL hasIOSCameraPermission(){
        return [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo] == AVAuthorizationStatusAuthorized;
    }
}
