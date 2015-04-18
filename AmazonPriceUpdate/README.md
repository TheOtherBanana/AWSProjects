# Amazon-Price-Update-Repo
Contains the POC and the web apis created for the Amazon Price Update work
AmazonPriceUpdateAPI: Contains the WebAPI calls, Controller definitions, and the Quartz.NET impl. 
(Quartz.NET is an open source library to run scheduled jobs)
Go through: Controllers, Global.asax.cs, COnfigManager, ScheduledJobUtils

PriceUpdateWebAPIBL: (BL stands for Business Logic)
Workhorse for the API code. API layer calls into this and this layer does orchestration. Initialized the context needed to run
the API code. Go through this and let me know if you need some walkthrough

ProductUpdateCatalogProvider: Contains the layer that manages the DynamoDB interactions from my code. DynamoDB is similar to Azure Tables
Go through the documentation here: http://docs.aws.amazon.com/amazondynamodb/latest/developerguide/WorkingWithTables.html
Just go through Low-Level Table creation documentaiton
Also go through working with items, I have created DynamoDB objects. Go through that if you've time
2 catalogs are there: One for products registered, one for DoS (needs some working)


AmazonProductAPIWrapper: Wrapper for product apis provided by Amazon. Singleton class is initialized at start with the keys.
Go through: How calls are made, and through this: https://affiliate-program.amazon.in/gp/advertising/api/detail/main.html
Also see everything except External
Also contains the HTML mail builder for products.
To Jaee: Check how Html files are built in code, suggest alternate approaches, better implementations if possible

CommonUtils: Meh files, Deserialization, serialization, and tracing wrappers. Generic boiler-plate code goes here

EmailManager: Helper to send Emails. THIS NEEDS A LOT OF REWORKING. Consider using GMail APIs to push mails.

PeriodicTask: Background periodic tasks that checks prices, sends emails. Very stupid code written for now. Go through if you've time
This will in future include some garbage collection stuff of old products, etc.

To Jaee: Ping me for the AWS Secret keys and passwords. Removed it from config. Hitting F5 will fail if not filled in.




