# Shrink
Shrink is a Sitecore utility module that gives you insight in the usage of your media library. Pretty much like a disk usage statistics viewer for your hard drives. But next to that, it also shows you which items are actually being used and published, so you can easily find media items that unnecessarily take up space in your database. And last but not least, it offers you multiple ways of cleaning up your media library!

## Why?
I have been working on Sitecore projects for many years and we have multiple customers who saw their database grow over the course of over 5 years of using their constantly evolved and upgraded Sitecore implementation. It's great that we can keep those platforms evolving and that we have sites running on Sitecore for this long, but the downside is that this database growth has a negative impact on the agility of your platform. Both in speed as in the Continuous Integration process, like with a database backup, rolling back deployments or rolling back content to your QA or development environment.

Of course, other solutions exist to keep your database small, like storing your media on disk and / or exposing it via a CDN, but I figured maybe more Sitecore users or implementation partners face the same issue. Or just want to do the annual spring cleaning on their not so huge databases to keep 'm nice and organized!

## How?
The module is installed as a regular Sitecore package. It is build using SPEAK and it can be accessed via the launchpad in the Tools column. Check my blog at http://sitecore.robhabraken.nl for more info on how to install and how to use this module.

## Known issues
The following issues are known and are part of the backlog for future releases:
* You cannot click on the left and right center parts of the donut charts, because the div highlighting the most important metric for that chart is blocking the chart element.
* The default behavior of the TreeView component which I've extended is to not select items in folded folders. So a checked but folded folder does not mean you have selected anything within that folder. You would actually have to expand it and then it automatically selects everything in it. I want to see if I can either change this behavior, or if I can run a script that folds out everything, but the latter could impose a performance issue as well as a UI challenge.

If you encounter an issue with this module, I would like to know what you've found and I would be more than happy if I can help you out or if I can improve my module in any way. Best way to contact me is via my Twitter account @rhabraken.

**_Please note that this module is rather new and only tested on a small number of databases and Sitecore instances. Please backup your original database before using this module and preferably test the module for your situation on any other than your production environemnt , to make sure no data is lost unintentionally upon using it._**
