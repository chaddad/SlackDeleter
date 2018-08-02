```
(`|   _|   |\ _ | _ |- _   
_)|(|(_|<  |/(/_|(/_|_(/_|`
```

# SlackDeleter
Don't you hate when you upload a new file to your favorite Slack, and get a message that your team is almost out of storage space? 

Me too. 

SlackDeleter uses the legacy Web APIs for Slack to retrieve a list of files that are older than a few days, and then issues a delete request for each file. Chances are, you can't even see these in chats anymore because you've had too many messages.

## How To Use
1. Download or clone this repo
1. Open the `app.Sample.config` file, [generate a legacy token for your Slack user and instance](https://api.slack.com/custom-integrations/legacy-tokens), and update the `authToken` value in the config
1. Save the config as `app.config`
1. As a .Net Core app, you can compile and run by using `dotnet run` from a command line prompt in the project directory, or open the solution file with Visual Studio

## Default values
* SlackDeleter will default to only request files that are over 5 days old, but this can be modified in the `GetFiles()` function in `Program.cs`.
* SlackDeleter will default to sleep for 2 seconds between delete requests. Slack rate-limits the calls to this request, and by delaying 2 seconds between requests you should be able to stay within the rate-limits set by Slack

## TODOs
- [ ] move "days old" to config file
- [ ] update to use newer Slack APIs and authentication
