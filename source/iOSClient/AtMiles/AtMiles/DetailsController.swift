//
//  DetailsController.swift
//  AtMiles
//
//  Created by Thorfinn Sørensen on 19/11/14.
//  Copyright (c) 2014 Thorfinn Sørensen. All rights reserved.
//

import UIKit;

class DetailsController : UIViewController{



    @IBOutlet weak var EmployeeImage: UIImageView!

    
    @IBOutlet weak var Call: UILabel!

    @IBOutlet weak var SendMail: UILabel!

    @IBOutlet weak var Phone: UILabel!
    
    @IBOutlet weak var Email: UILabel!
    
    @IBOutlet weak var Name: UILabel!
    
    @IBOutlet weak var Title: UILabel!
    
    @IBOutlet weak var Description: UITextView!
    
    var EmployeeId : NSString?;
    
    var details : EmployeeDetails!;

    var loader : UIActivityIndicatorView?;
    
    override func viewDidLoad() {
        super.viewDidLoad();
        
        loader = UIActivityIndicatorView(frame: CGRectMake(0, 0, 50, 50)) as UIActivityIndicatorView;
        loader?.center = self.view.center;
        loader?.hidesWhenStopped = true;
        loader?.activityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
        view.addSubview(loader!);

        self.Description.editable = false;
        
        fetchEmployeeDetails();
    }

    private func setViewProperties(){
        self.EmployeeImage.image = self.details.Thumb;
        
        self.Name.text = self.details.Name;
        
        self.Title.text = self.details.Title;
        self.Description.text = self.details.Description;
        
        var phoneTouchRecognizer = UITapGestureRecognizer();
        phoneTouchRecognizer.addTarget(self, action: "phoneTouched");
        
        var callTouchRecognizer = UITapGestureRecognizer();
        callTouchRecognizer.addTarget(self, action: "phoneTouched");
        
        var mailTouchRecognizer = UITapGestureRecognizer();
        mailTouchRecognizer.addTarget(self, action: "mailTouched");
        
        var mailToTouchRecognizer = UITapGestureRecognizer();
        mailToTouchRecognizer.addTarget(self, action: "mailTouched");

        //Add touchhandler for mailicon
        self.SendMail.userInteractionEnabled = true;
        self.SendMail.addGestureRecognizer(mailToTouchRecognizer);

        //Add text and touchandler for mailaddress label
        self.Email.text = self.details.Email;
        self.Email.userInteractionEnabled = true;
        self.Email.addGestureRecognizer(mailTouchRecognizer);
        self.Email.sizeToFit();
        self.Email.numberOfLines = 0;
        
        
        //Add touchandler for callicon
        self.Call.userInteractionEnabled = true;
        self.Call.addGestureRecognizer(callTouchRecognizer);
        
        //Add text and touchandler for phone-number
        self.Phone.text = self.details.Phone;
        self.Phone.userInteractionEnabled = true;
        self.Phone.addGestureRecognizer(phoneTouchRecognizer);

        
    }
    
    func phoneTouched(){
        var phoneUrl = "tel:" + self.details.Phone!;
        var description = "Call " + self.details.Phone! + " ?";
        
        var alert = UIAlertController(title: "Call", message: description, preferredStyle: UIAlertControllerStyle.Alert);
        
        self.presentViewController(alert, animated: true, completion: nil);
        
        alert.addAction(UIAlertAction(title: "Ok", style: .Default, handler: { action in
            switch action.style{
            case .Default:
                UIApplication.sharedApplication().openURL(NSURL(string:phoneUrl)!);
            case .Cancel:
                println("cancel")
            case .Destructive:
                println("destructive")
            }
        }))
        
        alert.addAction(UIAlertAction(title: "Cancel", style: UIAlertActionStyle.Cancel, handler: nil));
        
    }
    
    func mailTouched(){
        
        var mailText = "Do you want to compose a new mail to " + self.details.Email! + " ?";
        
        var alert = UIAlertController(title: "Send mail", message: mailText, preferredStyle: UIAlertControllerStyle.Alert);
        
        self.presentViewController(alert, animated: true, completion: nil);

        alert.addAction(UIAlertAction(title: "Ok", style: .Default, handler: { action in
            switch action.style{
            case .Default:
                println("Sendmail");
            case .Cancel:
                println("cancel")
            case .Destructive:
                println("destructive")
            }
        }))
        
        alert.addAction(UIAlertAction(title: "Cancel", style: UIAlertActionStyle.Cancel, handler: nil));

        
    }
    
    private func fetchEmployeeDetails(){

        self.loader?.startAnimating();
        var query = EmployeeId!.stringByAddingPercentEncodingWithAllowedCharacters(.URLHostAllowedCharacterSet());
        let searchUrl = "https://api-at.miles.no/api/company/miles/employee/" + query!;
            
        var url = NSURL(string: searchUrl);
            
        var key = MyApplication.sharedInstance.keychain;
            
        let token = key.stringForKey("id_token");
            
        let request = NSMutableURLRequest(URL: url!)
            request.HTTPMethod = "GET";
            
            request.setValue("Bearer " + token, forHTTPHeaderField: "Authorization");
            
            let session = NSURLSession.sharedSession().configuration;
        
            let task = NSURLSession.sharedSession().dataTaskWithRequest(request, completionHandler: { (data, response, error) -> Void in
                
            if (error != nil) {
                println(error)
            }
            else {
                let jsonResult : NSDictionary = NSJSONSerialization.JSONObjectWithData(data, options: NSJSONReadingOptions.MutableContainers, error: nil) as NSDictionary
                
                dispatch_async(dispatch_get_main_queue(), {

                    var title = " ";

                    if let tmpTitle = jsonResult["JobTitle"] as? String{
                        title = tmpTitle;
                    }
                    
                    var name : String = jsonResult["Name"] as String;

                    var description : String! = "";
                    
                    var descriptionArray : NSArray! = jsonResult["Descriptions"] as NSArray!;
                    
                    var descriptionDict : NSDictionary! = descriptionArray[0] as NSDictionary;
                    
                    if let internationalDescription = descriptionDict["InternationalDescription"] as? String{
                        
                        if(!internationalDescription.isEmpty){
                            description = internationalDescription;
                        }
                    }else{

                        if let localDescription = descriptionDict["LocalDescription"] as? String{
                            if(!localDescription.isEmpty && description.isEmpty){
                                description = localDescription;
                            }
                        }
                    }
                    
                    var phone : String = jsonResult["PhoneNumber"] as String;
                    
                    phone = phone.stringByReplacingOccurrencesOfString(" ", withString: "", options: NSStringCompareOptions.LiteralSearch, range: nil);
                    
                    var office : String = jsonResult["OfficeName"] as String;
                    var email : String = jsonResult["Email"] as String;
                    
                    if let tmpThumb = jsonResult["Thumb"] as? String{
                        
                        var replacedstring = tmpThumb.stringByReplacingOccurrencesOfString("data:image/png;base64,", withString: "", options: NSStringCompareOptions.LiteralSearch, range: nil);
                                
                        replacedstring = replacedstring.stringByReplacingOccurrencesOfString("data:image/jpg;base64,", withString: "", options: NSStringCompareOptions.LiteralSearch, range: nil);
                            
                        let imagedata = NSData(base64EncodedString: replacedstring, options: NSDataBase64DecodingOptions(rawValue: 0));
                        
                        var decodedImage : UIImage?;
                            
                        if(imagedata == nil){
                                decodedImage = nil;
                        }else{
                            decodedImage = UIImage(data: imagedata!);
                        }
                        self.details = EmployeeDetails(Name: name, Title: title, Thumb: decodedImage, Description: description, Phone: phone, Email: email, Office: office);
                    }else{
                        self.details = EmployeeDetails(Name: name, Title: title, Thumb: nil, Description: description, Phone: phone, Email: email, Office: office);
                    }
                    
                    self.setViewProperties();
                    self.loader?.stopAnimating();
                })
            }
        }
        );
        
        task.resume();
        

        
            
    }
    
}