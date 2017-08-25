# Elvanto2Fluro

This was created as a way of extracting data from Elvanto's API to push to Fluro via its API.  Its designed to be run from Visual Studio as a Console Application.  Visual Studio Community is a free download from Microsoft.

Thyis project also assumes you are familiar with: 
 - https://www.elvanto.com/api
 - https://developers.fluro.io/
 - Newtonsoft JSON.Net
 - NLog

To use this in another setting you will need to:

1/ Obtain your Elvanto API key and replace the static variable ElvantoAPIKey
2/ Obtain your Fluro API Key and replace the static variable FluroAPIKey
3/ Obtain the realm keys you wish to use.  I have defined:
   -  FluroCreativeRealm -- This is the realm used for my creative team for songs and uploads
   -  FluroRidgehavenRealm -- This is the realm that most people go into as a campus.
   
## Main Methods
### MigrateSongs();
This copies the songs, and files attached to Arrangements or Keys to Fluro

### MigratePeople();
This obtains the contacts from Elvanto and sots them into families.  Form there it creates the family, then creates the contacts and links the contacts to the family.
We use a custom field to identify members - custom_95d1c84c-6196-11e5-9d36-06ba798128be  

## Logging
Instead of using Console.Writeline this uses the NLog library (https://github.com/NLog/NLog/wiki/Tutorial).  You can use this to log to both screen and file in case you want to more fine tune the console output.
