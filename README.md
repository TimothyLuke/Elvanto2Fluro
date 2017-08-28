# Elvanto2Fluro

is an ETL Process for migrating from Elvanto to Fluro  Its designed to be run from Visual Studio as a Console Application.  Visual Studio Community is a free download from Microsoft.

Thyis project also assumes you are familiar with: 
 - https://www.elvanto.com/api
 - https://developers.fluro.io/
 - https://console.developers.google.com/apis/dashboard
 - Newtonsoft JSON.Net
 - NLog
 - GitHub Plugin for Visual Studio

To use this in another setting you will need to:

1. Obtain your Elvanto API key and replace the static variable Util.ElvantoAPIKey
2. Obtain your Fluro API Key and replace the static variable Util.FluroAPIKey
3. Obtain your Youtube API key and replace the static variable Util.YoutubeAPIKey
4. Obtain the realm keys you wish to use.  I have defined:
   -  FluroCreativeRealm -- This is the realm used for my creative team for songs and uploads
   -  FluroRidgehavenRealm -- This is the realm that most people go into as a campus.
5. Update the Fluro Tag IDs you wish to use.
6. Update the Elvanto Category IDs
   
## Main Methods
### MigrateSongs();
This copies the songs, and files attached to Arrangements or Keys to Fluro

### MigratePeople();
This obtains the contacts from Elvanto and sots them into families.  Form there it creates the family, then creates the contacts and links the contacts to the family.
We use a custom field to identify members - custom_95d1c84c-6196-11e5-9d36-06ba798128be  

### MigrateGroups()
This picks up all the Elvanto groups and adds them to FLuro.  It then iterates through the users and adds them in joining them as Provisional members.  It then marks as archived any closed groups.  (You cant add people to archived groups in Fluro.  This makes migrating a three step process.)  This also depends on a Fluro dynamic query that returns the the Fluro id as well as the importId so that matching can be done effectively.

## Logging
Instead of using Console.Writeline this uses the NLog library (https://github.com/NLog/NLog/wiki/Tutorial).  You can use this to log to both screen and file in case you want to more fine tune the console output.

## Getting Started
Within Visual Studio check out the project from GitHub.  All the heavy lifting is within Program.cs


