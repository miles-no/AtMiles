//
//  MyApplication.swift
//  AtMiles
//
//  Created by Thorfinn Sørensen on 17/11/14.
//  Copyright (c) 2014 Thorfinn Sørensen. All rights reserved.
//

import UIKit

class MyApplication: NSObject{
    class var sharedInstance: MyApplication{
        struct Singleton{
            static let instance = MyApplication();
        }
        return Singleton.instance;
    }
    
    let keychain : A0SimpleKeychain;
    
    private override init(){
        keychain = A0SimpleKeychain(service: "Auth0");
    }
}
