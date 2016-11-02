# Shrink
Shrink is a Sitecore utility module that gives you insight in the usage of your media library. Pretty much like a disk usage statistics viewer for your hard drives. But next to that, it also shows you which items are actually being used and published, so you can easily find media items that unnecessarily take up space in your database. And last but not least, it offers you multiple ways of cleaning up your media library!

## Why?
I have been working on Sitecore projects for many years and we have multiple customers who saw their database grow over the course of over 5 years of using their constantly evolved and upgraded Sitecore implementation. It's great that we can keep those platforms evolving and that we have sites running on Sitecore for this long, but the downside is that this database growth has a negative impact on the agility of your platform. Both in speed as in the Continuous Integration process, like with a database backup, rolling back deployments or rolling back content to your QA or development environment.

Of course, other solutions exist to keep your database small, like storing your media on disk and / or exposing it via a CDN, but I figured maybe more Sitecore users or implementation partners face the same issue. Or just want to do the annual spring cleaning on their not so huge databases to keep 'm nice and organized!

## How?
The module is installed as a regular Sitecore package. It is build using SPEAK and it can be accessed via the launchpad in the Tools column. Check my blog at http://sitecore.robhabraken.nl for more info on how to install and how to use this module.

## Features

chive data that you want to keep (for example, for audit purposes); recycle data that you may want to restore; delete data that you want to remove. For optimal performance and usability, recycle or remove as much data as you can, and archive whatever else you do not need in the Master database.
	http://www.sitecore.net/learn/blogs/technical-blogs/john-west-sitecore-blog/posts/2013/08/archiving-recycling-restoring-and-deleting-items-and-versions-in-the-sitecore-aspnet-cms.aspx
  
  
### Sunburst chart
The sunburst chart shows you the relative file sizes of the different folders in your media libarry. You can hover over folders and files to see their item name and file size, and you can zoom in on folders too. Clicking on a folder does not only zoom in your view, but also opens up the corresponding folder in the tree view, to show you the actual items and folder structure for that part.

### Donut charts
The donut charts show you the most important metrics of the analysis, showing you how much of your media items are not being used (not referenced by any items at all) or not being published to any of your publishing targets. It also gives you an overview of how many items have old versions within your media library.

Clicking on one of the slices of the donut charts filters the tree view to only show you the items of that category. So if you want to browse through all unreferenced items, just click on that slice of the corresponding donut chart!

### Archive, Recycle, Delete 
The best explanation on the different options for cleaning up your items comes from John West in his blog post https://community.sitecore.net/technical_blogs/b/sitecorejohn_blog/posts/archiving-recycling-deleting-and-restoring-items-and-versions-in-the-sitecore-asp-net-cms:

"Achive data that you want to keep (for example, for audit purposes); recycle data that you may want to restore; delete data that you want to remove. For optimal performance and usability, recycle or remove as much data as you can, and archive whatever else you do not need in the Master database."

Please keep in mind, that archiving and recycling only speeds up your queries, because it makes your indexes smaller, but if you really want to cut down the size of your database, only deleting items is going to help you out.

### Download

### Delete old versions
Deletes any version other than the latest and the active one.

### Tree view

### Don't forget!
Mind that you need to publish (parts of) the media library after cleaning up to also clean up your web database or other publishing targets. And next to that, you might want to clean up your database by removing orphaned blobs and run a database File Shrink to released the freed and now unallocated space within your SQL database.

## Future releases
For future releases, next to improving the existing media library usage statistics and clean up actions, I may extend this module with additional utilities to enhance the performance of your Sitecore implementations, by offering additional analyzing or clean up features like optimizing the templates, content structure or maybe even the size of your rendered Sitecore pages.

## Feature requests
If you have ideas or suggestions that I can use to improve my module, please do not hesitate to contact me! Best way to contact me is via my Twitter account @rhabraken.

## Known issues
The following issues are known and are part of the backlog for future releases:
* You cannot click on the left and right center parts of the donut charts, because the div highlighting the most important metric for that chart is blocking the chart element.
* The default behavior of the TreeView component which I've extended is to not select items in folded folders. So a checked but folded folder does not mean you have selected anything within that folder. You would actually have to expand it and then it automatically selects everything in it. I want to see if I can either change this behavior, or if I can run a script that folds out everything, but the latter could impose a performance issue as well as a UI challenge.

If you encounter an issue with this module, I would like to know what you've found and I would be more than happy if I can help you out or if I can improve my module in any way. Best way to contact me is via my Twitter account @rhabraken.

**_Please note that this module is rather new and only tested on a small number of databases and Sitecore instances. Please backup your original database before using this module and preferably test the module for your situation on any other than your production environemnt , to make sure no data is lost unintentionally upon using it._**
