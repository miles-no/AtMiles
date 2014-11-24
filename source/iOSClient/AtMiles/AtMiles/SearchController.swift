//
//  SearchController.swift
//  AtMiles
//
//  Created by Thorfinn Sørensen on 17/11/14.
//  Copyright (c) 2014 Thorfinn Sørensen. All rights reserved.
//

import UIKit

class SearchController: UITableViewController, UISearchBarDelegate, UISearchControllerDelegate{
    
    @IBOutlet weak var searchBar: UISearchBar!;
    
    @IBOutlet var welcomeLabel: UILabel?;
    
    var loader : UIActivityIndicatorView?;

    var Employees = [Employee]();

    
    override func viewDidLoad() {
        super.viewDidLoad()
        let keychain = MyApplication.sharedInstance.keychain
        let profileData:NSData! = keychain.dataForKey("profile")
        let profile:A0UserProfile = NSKeyedUnarchiver.unarchiveObjectWithData(profileData) as A0UserProfile
        self.welcomeLabel?.text = "Welcome \(profile.name)!"
        
        searchBar.delegate = self;
        
        loader = UIActivityIndicatorView(frame: CGRectMake(0, 0, 50, 50)) as UIActivityIndicatorView;
        loader?.center = self.view.center;
        loader?.hidesWhenStopped = true;
        loader?.activityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
        view.addSubview(loader!);
        
    }
    
    private func showMessage(message: NSString) {
        let alert = UIAlertView(title: message, message: nil, delegate: nil, cancelButtonTitle: "OK")
        alert.show()
    }

    
    override func tableView(tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return Employees.count
    }
    
    
    func searchBarSearchButtonClicked(searchBar: UISearchBar) {
        if(searchBar.text != nil){
            performEmployeeSearch(searchBar.text);
            searchBar.resignFirstResponder();
        }
    }
    
    
    func searchDisplayController(controller: UISearchDisplayController!, shouldReloadTableForSearchString searchString: String!) -> Bool {
        return true
    }
    
    override func tableView(tableView: UITableView, cellForRowAtIndexPath indexPath: NSIndexPath) -> UITableViewCell {
        
        var cell = tableView.dequeueReusableCellWithIdentifier("NameCell") as UITableViewCell!;
        
        if(cell == nil){
            cell = UITableViewCell(style: .Default, reuseIdentifier: "NameCell");
        }
        
        cell.textLabel.text = Employees[indexPath.row].Name;
        cell.detailTextLabel?.text = Employees[indexPath.row].Title;
        cell.imageView.image = Employees[indexPath.row].Thumb;
        
        return cell;
        
    }
    
    private func performEmployeeSearch(searchString : String){
        self.loader?.startAnimating();
        
        var query = searchString.stringByAddingPercentEncodingWithAllowedCharacters(.URLHostAllowedCharacterSet());
        let searchUrl = "https://api-at.miles.no/api/search/Fulltext?query=" + query! + "&take=200";

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
                let jsonResult : AnyObject! = NSJSONSerialization.JSONObjectWithData(data, options: NSJSONReadingOptions.MutableContainers, error: nil) as NSDictionary
                
                var result : NSArray = jsonResult["Results"] as NSArray;
                
                dispatch_async(dispatch_get_main_queue(), {
                
                    self.Employees.removeAll(keepCapacity: false);
                    
                    for employee : AnyObject in result{
                        
                        var tmpEmployee : Employee;
                        
                        
                        var name : NSString = employee["Name"] as NSString;
                        
                        var id : NSString = employee["GlobalId"] as NSString;
                        
                        //has to be one space to avoid messing up the graphics
                        var title = " ";
                        
                        if let tmpTitle = employee["JobTitle"] as? String{
                            title = tmpTitle;
                        }
                        
                        if let tmpThumb = employee["Thumb"] as? String{
                            
                            var replacedstring = tmpThumb.stringByReplacingOccurrencesOfString("data:image/png;base64,", withString: "", options: NSStringCompareOptions.LiteralSearch, range: nil);
                            
                            replacedstring = replacedstring.stringByReplacingOccurrencesOfString("data:image/jpg;base64,", withString: "", options: NSStringCompareOptions.LiteralSearch, range: nil);
                            
                            let imagedata = NSData(base64EncodedString: replacedstring, options: NSDataBase64DecodingOptions(rawValue: 0));
                            
                            var decodedImage : UIImage?;
                            
                            if(imagedata == nil){
                                decodedImage = nil;
                            }else{
                                decodedImage = UIImage(data: imagedata!);
                            }
                            tmpEmployee = Employee(Name: name, Title: title, Thumb: decodedImage, Id: id);
                            
                        }else{
                            tmpEmployee = Employee(Name: name, Title: title, Thumb: nil, Id: id);
                        }
                        
                        
                        self.Employees.append(tmpEmployee);
                        
                    }
                   self.tableView.reloadData()
                   self.loader?.stopAnimating();
                })
            }
        }
        );
        
        task.resume();
        
    }
    
    override func prepareForSegue(segue: UIStoryboardSegue, sender: AnyObject?) {
        var detailsViewController : DetailsController = segue.destinationViewController as DetailsController;
        
        var selectedItem = self.tableView!.indexPathForSelectedRow()!.row;
        
        var selectedEmployee = self.Employees[selectedItem];

        detailsViewController.EmployeeId = selectedEmployee.Id;
        
        
    }
    

}