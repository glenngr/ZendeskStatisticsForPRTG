# ZendeskStatisticsForPRTG
PRTG EXE/Advanced sensor that gets ticket stats from Zendesk

### Install instructions
* Download binaries from https://github.com/glenngr/ZendeskStatisticsForPRTG/releases
* -- or git clone and compile yourself
* Edit the .config files with correct subdomain, username and password for Zendesk
* Copy compiled files to PRTG Network Monitor\Custom Sensors\EXEXML
* Add a new EXE/Advanced sensor in PRTG.
* Select GetZendeskAgentStatsInGroup or GetZendeskGroupStats in the dropdown
* Use the group Zendesk Group ID as paramater for the exe file

To get group ID, look in the Zendesk Web interface, OR run the exe files without parameter. It should then list the available groups.


#### Description of sensors

###### GetZendeskGroupStats
Shows the following channels:
* New tickets today
* Solved tickets today
* Number of on-hold tickets
* Number of pending tickets
* Number of open tickets
* Number of unassigned tickets

###### GetZendesAgentStatsInGroup
Shows one channel for each agent in the group.
The channel data will be number of open tickets for the agent.
