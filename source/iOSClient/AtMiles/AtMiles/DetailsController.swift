//
//  DetailsController.swift
//  AtMiles
//
//  Created by Thorfinn Sørensen on 19/11/14.
//  Copyright (c) 2014 Thorfinn Sørensen. All rights reserved.
//

import UIKit;

class DetailsController : UIViewController{


    @IBOutlet weak var AddToContacts: UIBarButtonItem!

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
    
    var adbk : ABAddressBook?;
    
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
    
    override func viewDidAppear(animated: Bool) {
        super.viewDidAppear(animated)
            let stat = ABAddressBookGetAuthorizationStatus()
            switch stat {
            case .Denied, .Restricted:
                println("no access")
            case .Authorized, .NotDetermined:
                var err : Unmanaged<CFError>? = nil
                var adbk : ABAddressBook? = ABAddressBookCreateWithOptions(nil, &err).takeRetainedValue()
                if adbk == nil {
                    println(err)
                    return
                }
                ABAddressBookRequestAccessWithCompletion(adbk) {
                    (granted:Bool, err:CFError!) in
                    if granted {
                        self.adbk = adbk
                    } else {
                        println(err)
                    }//if
                }//ABAddressBookReqeustAccessWithCompletion
            }//case
    }//viewDidAppear
    
    @IBAction func addToContacts(){

        var description = "Are you sure you want to add " + self.details.Name + " to your contacts ?";
        
        var alert = UIAlertController(title: "Add to contacts", message: description, preferredStyle: UIAlertControllerStyle.Alert);
        
        self.presentViewController(alert, animated: true, completion: nil);
        
        alert.addAction(UIAlertAction(title: "Ok", style: .Default, handler: { action in
            switch action.style{
            case .Default:
                self.addContact();
            case .Cancel:
                println("cancel")
            case .Destructive:
                println("destructive")
            }
        }))
        
        alert.addAction(UIAlertAction(title: "Cancel", style: UIAlertActionStyle.Cancel, handler: nil));

        
    }
    
    private func addContact(){
        var newContact:ABRecordRef! = ABPersonCreate().takeRetainedValue()
        var success:Bool = false
        
        var error: Unmanaged<CFErrorRef>? = nil
        
        success = ABRecordSetValue(newContact, kABPersonFirstNameProperty, self.details.FirstName, &error)
        println("setting first name was successful? \(success)")
        success = ABRecordSetValue(newContact, kABPersonLastNameProperty, self.details.LastName, &error)
        println("setting last name was successful? \(success)")
        
        let multiPhone:ABMutableMultiValue = ABMultiValueCreateMutable(
            ABPropertyType(kABStringPropertyType)).takeRetainedValue();
        
        ABMultiValueAddValueAndLabel(multiPhone, self.details.Phone, kABPersonPhoneMobileLabel, nil);
        
        success = ABRecordSetValue(newContact, kABPersonPhoneProperty, multiPhone,nil);
        
        let multiEmail:ABMutableMultiValue = ABMultiValueCreateMutable(
            ABPropertyType(kABStringPropertyType)).takeRetainedValue();
        
        ABMultiValueAddValueAndLabel(multiEmail, self.details.Email, kABWorkLabel, nil);
        
        success = ABRecordSetValue(newContact, kABPersonEmailProperty, multiEmail, &error);
        
        var image : NSData = UIImagePNGRepresentation(self.details.Thumb);
        success = ABPersonSetImageData(newContact, image, &error);
        
        success = ABAddressBookAddRecord(adbk, newContact, &error);
        success = ABAddressBookSave(adbk, &error);
        
        
        var description = self.details.Name + " was successfully added to your contact list";
        
        if(success){
            var alert = UIAlertController(title: "Contact added", message: description, preferredStyle: UIAlertControllerStyle.Alert);
            
            self.presentViewController(alert, animated: true, completion: nil);
            alert.addAction(UIAlertAction(title: "Ok", style: UIAlertActionStyle.Cancel, handler: nil));
            
        }
        

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
                self.sendMail();
            case .Cancel:
                println("cancel")
            case .Destructive:
                println("destructive")
            }
        }))
        
        alert.addAction(UIAlertAction(title: "Cancel", style: UIAlertActionStyle.Cancel, handler: nil));

        
    }
    
    private func sendMail(){
        var sendMailString = "mailto:" + self.details.Email!;
        
        UIApplication.sharedApplication().openURL(NSURL(string: sendMailString)!);
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

                    var firstName : String = jsonResult["FirstName"] as String;
                    var lastName : String = jsonResult["LastName"] as String;
                    
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
                        self.details = EmployeeDetails(Name: name, Title: title, Thumb: decodedImage, Description: description, Phone: phone, Email: email, Office: office, FirstName : firstName, LastName : lastName);
                    }else{
                        self.details = EmployeeDetails(Name: name, Title: title, Thumb: nil, Description: description, Phone: phone, Email: email, Office: office, FirstName : firstName, LastName : lastName);
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