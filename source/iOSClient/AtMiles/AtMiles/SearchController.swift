//
//  SearchController.swift
//  AtMiles
//
//  Created by Thorfinn Sørensen on 17/11/14.
//  Copyright (c) 2014 Thorfinn Sørensen. All rights reserved.
//

import UIKit

class SearchController: UITableViewController, UISearchBarDelegate, UISearchControllerDelegate{
    
    @IBOutlet weak var searchBar: UISearchBar!
    
    @IBOutlet var welcomeLabel: UILabel?
    
    var Employees = [Employee]();

    
    override func viewDidLoad() {
        super.viewDidLoad()
        let keychain = MyApplication.sharedInstance.keychain
        let profileData:NSData! = keychain.dataForKey("profile")
        let profile:A0UserProfile = NSKeyedUnarchiver.unarchiveObjectWithData(profileData) as A0UserProfile
        self.welcomeLabel?.text = "Welcome \(profile.name)!"
        
        searchBar.delegate = self;
        
    }
    
    private func showMessage(message: NSString) {
        let alert = UIAlertView(title: message, message: nil, delegate: nil, cancelButtonTitle: "OK")
        alert.show()
    }

    
    override func tableView(tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return Employees.count
    }
    
    
    func searchBarSearchButtonClicked(searchBar: UISearchBar) {
        self.Employees.removeAll(keepCapacity: false);
        
        if(searchBar.text != nil){
            performEmployeeSearch(searchBar.text);
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
        
        return cell;
        
    }
    
    private func performEmployeeSearch(searchString : String){
        
        var query = searchString.stringByAddingPercentEncodingWithAllowedCharacters(.URLHostAllowedCharacterSet());
        let searchUrl = "https://api-at.miles.no/api/search/Fulltext?query=" + query! + "&take=200";

        var url = NSURL(string: searchUrl);
        
        var key = MyApplication.sharedInstance.keychain;
        
        let token = key.stringForKey("id_token");
        
        let request = NSMutableURLRequest(URL: url!)
        request.HTTPMethod = "GET";
        
        request.setValue("Bearer " + token, forHTTPHeaderField: "Authorization");
        
        println(token);
        
        let session = NSURLSession.sharedSession().configuration;

        let task = NSURLSession.sharedSession().dataTaskWithRequest(request, completionHandler: { (data, response, error) -> Void in

            if (error != nil) {
                println(error)
            }
            else {
                let jsonResult : AnyObject! = NSJSONSerialization.JSONObjectWithData(data, options: NSJSONReadingOptions.MutableContainers, error: nil) as NSDictionary
                
                var result : NSArray = jsonResult["Results"] as NSArray;
                
                dispatch_async(dispatch_get_main_queue(), {
                
                    for employee : AnyObject in result{
                        
                        
                        var name : NSString = employee["Name"] as NSString;
                        var title : NSString! = employee["JobTitle"] as NSString!;
                        
                        self.Employees.append(Employee(Name: name, Title: title));
                        
                    }
                   self.tableView.reloadData()
                

                })
            }
        }
        );
        
        task.resume();
        

        
    }

}