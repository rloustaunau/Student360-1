SMCISD Student360 App to Trigger Alerts
============

This project is a console application that calls an API endpoint to schedule the execution of alerts. It should be configured in a Azure Web Job or in a Windows server task scheduler.

Notes and Setup
------------
1. Update the appsettings.Production.json to point to your API.
2. Compile in Release mode.
3. Create Zip with all files in ~/bin/release/*.*
4. Upload to Azure App Services -> app -> settings -> WebJobs and set the schedule to 0 0 6 * * *  (Every day at 6am)

**Notes: Make sure you update the NotificationsApi in the appsettings.Production.json**